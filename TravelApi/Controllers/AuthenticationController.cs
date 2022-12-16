using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PrUtility;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Travel.Context.Models;
using Travel.Context.Models.Travel;
using Travel.Data.Interfaces;
using Travel.Shared.Ultilities;
using Travel.Shared.ViewModels;
using TravelApi.Helpers;
using TravelApi.Hubs.HubServices;

namespace TravelApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : Controller
    {
        private IConfiguration configuration;
        private IAuthentication authentication;
        private Response res;
        private readonly int TimeExpiredInMinutes;
        private readonly IWebHostEnvironment _env;
        public AuthenticationController(IConfiguration _configuration, IAuthentication _authentication,
            IWebHostEnvironment env
         )
        {
            _env = env;
            configuration = _configuration;
            authentication = _authentication;
            res = new Response();
            TimeExpiredInMinutes = Convert.ToInt16(configuration["Token:TimeExpired"]);
           
        }
        [NonAction]
        private Authentication GenerateTokenEmployee(Employee result)
        {
            var claim = new[]
            {
                                        new Claim(JwtRegisteredClaimNames.Sub, configuration["Token:Subject"]),
                                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                                        new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                                        new Claim(JwtRegisteredClaimNames.Aud, configuration["Token:Audience"]),
                                        new Claim(ClaimTypes.Email, result.Email),
                                        new Claim(ClaimTypes.NameIdentifier, result.IdEmployee.ToString()),
                                        new Claim("RoleId", result.RoleId.ToString()),
                                        new Claim("UserId", result.IdEmployee.ToString())
                                     };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Token:key"]));
            var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(configuration["Token:Issuer"],
                configuration["Token:Audience"], claim, expires: DateTime.UtcNow.AddMinutes(TimeExpiredInMinutes),
                //configuration["Token:Audience"], claim, expires: DateTime.UtcNow.AddSeconds(25),
                signingCredentials: signIn);

            var tokenJWT = new JwtSecurityTokenHandler().WriteToken(token);

            authentication.EmpAddToken(tokenJWT, result.IdEmployee);


            Authentication auth = new Authentication();
            auth.Token = tokenJWT;
            auth.RoleId = result.RoleId;
            auth.Id = result.IdEmployee;
            auth.Name = result.NameEmployee;
            auth.Phone = result.Phone;
            auth.Image = result.Image;
            auth.Email = result.Email;
            auth.DateExpired = DateTime.Now.AddMinutes(TimeExpiredInMinutes);
            return auth;
        }

        [NonAction]
        private async Task<Authentication> GenerateTokenCustomer(Customer result)
        {
            var claim = new[]
                                      {
                                    new Claim(JwtRegisteredClaimNames.Sub, configuration["Token:Subject"]),
                                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                                    new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                                    new Claim(JwtRegisteredClaimNames.Aud, configuration["Token:Audience"]),
                                    new Claim("UserId", result.IdCustomer.ToString()),
                                    new Claim(ClaimTypes.Email, result.Email.ToString())
                                };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Token:key"]));
            var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(configuration["Token:Issuer"],
                configuration["Token:Audience"], claim, expires: DateTime.UtcNow.AddMinutes(TimeExpiredInMinutes),
                //configuration["Token:Audience"], claim, expires: DateTime.UtcNow.AddMinutes(525600),
                signingCredentials: signIn);




            #region create refreshtoken
            var refreshToken = GenerateRefreshToken();
            // add refresh token to database 
            var refreshToeknEntity = new RefreshToken
            {
                Id = Guid.NewGuid(),
                JwtId = token.Id,
                RefToken = refreshToken,
                UserId = result.IdCustomer,
                IsUsed = false,
                IsRevoked = false,
                IssuedAt = DateTime.UtcNow,
                ExpriedAt = DateTime.UtcNow.AddMinutes(TimeExpiredInMinutes)
            };
            var statusAddRefreshToken = await authentication.AddRefeshToken(refreshToeknEntity);
            if (!statusAddRefreshToken)
            {
                return new Authentication();
            }

            #endregion


            var tokenJWT = new JwtSecurityTokenHandler().WriteToken(token);

            authentication.CusAddToken(tokenJWT, result.IdCustomer);


            Authentication auth = new Authentication();
            auth.Token = tokenJWT;
            auth.Id = result.IdCustomer;
            auth.Name = result.NameCustomer;
            auth.Phone = result.Phone;
            auth.Email = result.Email;
            auth.RefToken = refreshToken;
            auth.DateExpired = DateTime.Now.AddMinutes(TimeExpiredInMinutes);
            return auth;
        }

        [NonAction]
        private string GenerateRefreshToken()
        {
            var random = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(random);
                return Convert.ToBase64String(random);
            }
        }

        [NonAction]
        private DateTime ConvertUnixTimeToDateTime(long utcExpireDate)
        {
            var dateinterval = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateinterval = dateinterval.AddSeconds(utcExpireDate).ToUniversalTime();
            return dateinterval;
        }
        [HttpPost("refresh-token")]
        public async Task<object> RenewToken(TokenModel model)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var tokenValidateParams = new TokenValidationParameters()
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidAudience = configuration["Token:Audience"],
                ValidIssuer = configuration["Token:Issuer"],
                ValidateLifetime = false,
                //ClockSkew = TimeSpan.FromMinutes(Convert.ToInt16(Configuration["Token:TimeExpired"])),
                //ClockSkew = TimeSpan.FromSeconds(TimeExpiredInMinutes),
                ClockSkew = TimeSpan.Zero,

                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Token:Key"])),

            };
            try
            {
                // check 1: AccessToekn có valid ko
                var tokenInVerification = jwtTokenHandler.ValidateToken(model.AccessToken, tokenValidateParams, out var validatedToken);

                //// check 2: check alg
                //if (validatedToken is JwtSecurityToken jwtSecurityToken)
                //{
                //    var result = jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha512, StringComparison.InvariantCultureIgnoreCase);
                //    if (!result)
                //    {
                //        return Ok(new
                //        {
                //            Success = false,
                //            Message = "Invalid Token"
                //        });
                //    }
                //}

                // check 3: check accessToken Expire?
                var utcExpireDate = long.Parse(tokenInVerification.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Exp).Value);
                var expireDate = ConvertUnixTimeToDateTime(utcExpireDate);
                if (expireDate > DateTime.UtcNow)
                {
                    return (new
                    {
                        Success = false,
                        Message = "Token has not expired"
                    });
                }

                // check 4: Check refreskToken exist in DB
                var storedToken = await authentication.GetRefreshToken(model.RefreshToken);
                if (storedToken == null)
                {
                    return (new
                    {
                        Success = false,
                        Message = "Refresh token does not exist"
                    });
                }
                // check 5: check refreshToken is used/revoke
                if (storedToken.IsUsed)
                {
                    return (new
                    {
                        Success = false,
                        Message = "Has been used"
                    });
                }
                if (storedToken.IsRevoked)
                {
                    return (new
                    {
                        Success = false,
                        Message = "Has been revoked"
                    });
                }

                // check 6: AccessToken id == JwtId in refreshtoken
                var jti = tokenInVerification.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti).Value;
                if (storedToken.JwtId != jti)
                {
                    return (new
                    {
                        Success = false,
                        Message = "Token not mapped"
                    });
                }
                // update token is used
                storedToken.IsRevoked = true;
                storedToken.IsUsed = true;
                await authentication.UpdateToken(storedToken);

                var user = await authentication.GetCustomerById(storedToken.UserId);
                var token = await GenerateTokenCustomer(user);
                return (new
                {
                    Success = true,
                    Message = "Renew token success",
                    Data = token
                });





            }
            catch (Exception e)
            {
                return (new
                {
                    Success = false,
                    Message = "Something wrong"
                });
            }
        }



        [HttpPost]
        [AllowAnonymous]
        [Route("login-employee")]
        public object EmpLogin([FromBody] JObject frmData)
        {
            try
            {
                //RequestCache.Get<Employee>("d"); 
                var dateTimeNow = DateTime.Now;
                string email = PrCommon.GetString("email", frmData);
                string password = PrCommon.GetString("password", frmData);
                var result = authentication.EmpCheckBlock(email);
                if (result == null)
                {
                    result = authentication.EmpLogin(email);
                    if (result != null)
                    {
                        string encryption = authentication.Encryption(password);
                        result = authentication.EmpLogin(email, encryption);
                        if (result != null)
                        {
                            var isNew = authentication.EmpIsNew(email);
                            if (isNew)
                            {
                                var active = authentication.EmpActive(email);
                                if (active)
                                {
                                    #region code cũ
                                    //var roleName = authentication.RoleName(result.RoleId);
                                    //var claim = new[]
                                    //{
                                    //    new Claim(JwtRegisteredClaimNames.Sub, configuration["Token:Subject"]),
                                    //    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                                    //    new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                                    //    new Claim(JwtRegisteredClaimNames.Aud, configuration["Token:Audience"]),
                                    //    new Claim(ClaimTypes.Email, result.Email),
                                    //    new Claim(ClaimTypes.NameIdentifier, result.IdEmployee.ToString()),
                                    //    new Claim("RoleId", result.RoleId.ToString()),
                                    //    new Claim("UserId", result.IdEmployee.ToString())
                                    // };
                                    //var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Token:key"]));
                                    //var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                                    //var token = new JwtSecurityToken(configuration["Token:Issuer"],
                                    //    configuration["Token:Audience"], claim, expires: DateTime.UtcNow.AddMinutes(TimeExpiredInMinutes),
                                    //    //configuration["Token:Audience"], claim, expires: DateTime.UtcNow.AddSeconds(25),
                                    //    signingCredentials: signIn);

                                    //var tokenJWT = new JwtSecurityTokenHandler().WriteToken(token);

                                    //authentication.EmpAddToken(tokenJWT, result.IdEmployee);

                                    //Authentication auth = new Authentication();
                                    //auth.Token = tokenJWT;
                                    //auth.RoleId = result.RoleId;
                                    //auth.Id = result.IdEmployee;
                                    //auth.Name = result.NameEmployee;
                                    //auth.Phone = result.Phone;
                                    //auth.Image = result.Image;
                                    //auth.Email = result.Email;
                                    //auth.DateExpired = dateTimeNow.AddMinutes(TimeExpiredInMinutes);
                                    #endregion
                                    var auth =  GenerateTokenEmployee(result);
                                    return Ok(Ultility.Responses("Đăng nhập thành công !", Enums.TypeCRUD.Success.ToString(), auth));
                                }
                                else
                                {
                                    return Ok(Ultility.Responses("Tài khoản của bạn chưa được kích hoạt !", Enums.TypeCRUD.Error.ToString()));
                                }
                            }
                            else
                            {
                                return Ok(Ultility.Responses("Tài khoản của bạn chưa xác nhận email !", Enums.TypeCRUD.Error.ToString()));
                            }

                        }
                        else
                        {
                            return Ok(Ultility.Responses("Sai mật khẩu !", Enums.TypeCRUD.Error.ToString()));
                        }

                    }
                    else
                    {
                        return Ok(Ultility.Responses("Không tìm thấy email [" + email + "] trên hệ thống !", Enums.TypeCRUD.Warning.ToString()));
                    }
                }
                else
                {
                    return Ok(Ultility.Responses("", Enums.TypeCRUD.Block.ToString(), result.TimeBlock));
                }
            }
            catch (Exception e)
            {
                return Ok(Ultility.Responses("Có lỗi xảy ra !", Enums.TypeCRUD.Error.ToString(), description: e.Message));
            }

        }

        [HttpPost]
        [AllowAnonymous]
        [Route("login-customer")]
        public async Task<object> CusLogin([FromBody] JObject frmData)
        {
            try
            {
                string email = PrCommon.GetString("email", frmData);
                string password = PrCommon.GetString("password", frmData);
                string googleToken = PrCommon.GetString("googleToken", frmData);

                var result = authentication.CusCheckBlock(email);
                #region code cũ
                //if (result == null)
                //{
                //    result = authentication.CusLogin(email);
                //    if (result == null && !string.IsNullOrEmpty(googleToken))
                //    {
                //        result = new Customer();
                //        result.Email = PrCommon.GetString("email", frmData);
                //        result.Password = googleToken;
                //        result.NameCustomer = PrCommon.GetString("nameCustomer", frmData);
                //        var status = authentication.CreateAccountGoogle(result);
                //        if (!status)
                //        {
                //            return Ok(Ultility.Responses("Đăng nhập bằng google thất bại!", Enums.TypeCRUD.Error.ToString()));
                //        }
                //    }

                //    if (result != null)
                //    {
                //        if (string.IsNullOrEmpty(googleToken))
                //        {
                //            string encryption = authentication.Encryption(password);
                //            result = authentication.CusLogin(email, encryption);
                //        }
                //        else
                //        {
                //            var status = authentication.CusAddTokenGoogle(googleToken, result.IdCustomer);
                //            if (!status)
                //            {
                //                res.Notification.DateTime = DateTime.Now;
                //                res.Notification.Messenge = "Đăng nhập bằng google thất bại !";
                //                res.Notification.Type = "Error";
                //                return Ok(res);
                //            }
                //        }

                //        if (result != null)
                //        {
                //            #region code cũ



                //            //var claim = new[]
                //            //        {
                //            //        new Claim(JwtRegisteredClaimNames.Sub, configuration["Token:Subject"]),
                //            //        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                //            //        new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                //            //        new Claim(JwtRegisteredClaimNames.Aud, configuration["Token:Audience"]),
                //            //        new Claim("UserId", result.IdCustomer.ToString()),
                //            //        new Claim(ClaimTypes.Email, result.Email.ToString())
                //            //    };

                //            //var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Token:key"]));
                //            //var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                //            //var token = new JwtSecurityToken(configuration["Token:Issuer"],
                //            //    configuration["Token:Audience"], claim, expires: DateTime.UtcNow.AddMinutes(TimeExpiredInMinutes),
                //            //    //configuration["Token:Audience"], claim, expires: DateTime.UtcNow.AddMinutes(525600),
                //            //    signingCredentials: signIn);

                //            //var tokenJWT = new JwtSecurityTokenHandler().WriteToken(token);

                //            //authentication.CusAddToken(tokenJWT, result.IdCustomer);








                //            //Authentication auth = new Authentication();
                //            //auth.Token = tokenJWT;
                //            //auth.Id = result.IdCustomer;
                //            //auth.Name = result.NameCustomer;
                //            //auth.Phone = result.Phone;
                //            //auth.Email = result.Email;
                //            #endregion
                //            var auth = await GenerateTokenCustomer(result);

                //            return Ok(Ultility.Responses("Đăng nhập thành công !", Enums.TypeCRUD.Success.ToString(), auth));
                //        }
                //        else
                //        {
                //            return Ok(Ultility.Responses("Sai mật khẩu !", Enums.TypeCRUD.Error.ToString()));
                //        }

                //    }
                //    else
                //    {
                //        return Ok(Ultility.Responses("Không tìm thấy email [" + email + "] trên hệ thống !", Enums.TypeCRUD.Error.ToString()));
                //    }
                //}
                //else
                //{
                //    return Ok(Ultility.Responses("", Enums.TypeCRUD.Block.ToString(), result.TimeBlock));
                //}
                #endregion

                if (result != null)
                {
                    return Ok(Ultility.Responses("", Enums.TypeCRUD.Block.ToString(), result.TimeBlock));

                }
                result = authentication.CusLogin(email);



                if (result == null && !string.IsNullOrEmpty(googleToken))
                {
                    result = new Customer();
                    result.Email = PrCommon.GetString("email", frmData);
                    result.Password = googleToken;
                    result.NameCustomer = PrCommon.GetString("nameCustomer", frmData);
                    var status = authentication.CreateAccountGoogle(result);
                    if (!status)
                    {
                        return Ok(Ultility.Responses("Đăng nhập bằng google thất bại!", Enums.TypeCRUD.Error.ToString()));
                    }
                }

                if (result == null)
                {
                    return Ok(Ultility.Responses("Không tìm thấy email [" + email + "] trên hệ thống !", Enums.TypeCRUD.Error.ToString()));

                }

                if (string.IsNullOrEmpty(googleToken))
                {
                    string encryption = authentication.Encryption(password);
                    result = authentication.CusLogin(email, encryption);
                }
                else
                {
                    var status = await authentication.CusAddTokenGoogle(googleToken, result.IdCustomer);
                    if (!status)
                    {
                        res.Notification.DateTime = DateTime.Now;
                        res.Notification.Messenge = "Đăng nhập bằng google thất bại !";
                        res.Notification.Type = "Error";
                        return Ok(res);
                    }
                }

                if (result != null)
                {
                    #region code cũ



                    //var claim = new[]
                    //        {
                    //        new Claim(JwtRegisteredClaimNames.Sub, configuration["Token:Subject"]),
                    //        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    //        new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                    //        new Claim(JwtRegisteredClaimNames.Aud, configuration["Token:Audience"]),
                    //        new Claim("UserId", result.IdCustomer.ToString()),
                    //        new Claim(ClaimTypes.Email, result.Email.ToString())
                    //    };

                    //var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Token:key"]));
                    //var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                    //var token = new JwtSecurityToken(configuration["Token:Issuer"],
                    //    configuration["Token:Audience"], claim, expires: DateTime.UtcNow.AddMinutes(TimeExpiredInMinutes),
                    //    //configuration["Token:Audience"], claim, expires: DateTime.UtcNow.AddMinutes(525600),
                    //    signingCredentials: signIn);

                    //var tokenJWT = new JwtSecurityTokenHandler().WriteToken(token);

                    //authentication.CusAddToken(tokenJWT, result.IdCustomer);








                    //Authentication auth = new Authentication();
                    //auth.Token = tokenJWT;
                    //auth.Id = result.IdCustomer;
                    //auth.Name = result.NameCustomer;
                    //auth.Phone = result.Phone;
                    //auth.Email = result.Email;
                    #endregion
                    var auth = await GenerateTokenCustomer(result);

                    return Ok(Ultility.Responses("Đăng nhập thành công !", Enums.TypeCRUD.Success.ToString(), auth));
                }
                else
                {
                    return Ok(Ultility.Responses("Sai mật khẩu !", Enums.TypeCRUD.Error.ToString()));
                }

            }
            catch (Exception e)
            {
                return Ok(Ultility.Responses("Có lỗi xảy ra !", Enums.TypeCRUD.Block.ToString(), description: e.Message));
            }

        }

        [HttpGet]
        [AllowAnonymous]
        [Route("logout-customer")]
        public object CusLogout(Guid idCus)
        {
            res = authentication.CusDeleteToken(idCus);
            return Ok(res);
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("logout-employee")]
        public object EmpLogout(Guid idEmp)
        {
            res = authentication.EmpDeleteToken(idEmp);
            return Ok(res);
        }

        [HttpPut]
        [AllowAnonymous]
        [Route("change-pass-customer")]
        public object CusChangePass(Guid idCus, string password, string newPassword)
        {
            res = authentication.CusChangePassword(idCus, password, newPassword);
            return Ok(res);
        }

        [HttpPut]
        [AllowAnonymous]
        [Route("forgot-pass-customer")]
        public object CusForgotPass(string email, string password)
        {
            res = authentication.CusForgotPassword(email, password);
            return Ok(res);
        }


        [HttpPut]
        [AllowAnonymous]
        [Route("block-customer")]
        public object CusBlock(string email)
        {
            res = authentication.CusBlock(email);
            return Ok(res);
        }

        [HttpPut]
        [AllowAnonymous]
        [Route("block-employee")]
        public object EmpBlock(string email)
        {
            res = authentication.EmpBlock(email);
            return Ok(res);
        }

        [HttpPut]
        [AllowAnonymous]
        [Route("change-pass-employee")]
        public object EmpChangePassword(Guid idEmp, string password, string newPassword)
        {
            res = authentication.EmpChangePassword(idEmp, password, newPassword);
            return Ok(res);
        }

        [HttpPut]
        [AllowAnonymous]
        [Route("forgot-pass-employee")]
        public object EmpForgotPass(string email, string password)
        {
            res = authentication.EmpForgotPassword(email, password);
            return Ok(res);
        }
    }
}
