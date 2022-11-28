using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using PrUtility;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Travel.Context.Models;
using Travel.Data.Interfaces;
using Travel.Shared.Ultilities;
using Travel.Shared.ViewModels;
using TravelApi.Hubs.HubServices;

namespace TravelApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : Controller
    {
        private IConfiguration configuration;
        private IAuthentication authentication;
        private IHubRepository hubRepo;
        private Response res;
        private readonly int TimeExpiredInMinutes;
        public AuthenticationController(IConfiguration _configuration, IAuthentication _authentication, IHubRepository _hubRepo)
        {
            configuration = _configuration;
            authentication = _authentication;
            hubRepo = _hubRepo;
            res = new Response();
            TimeExpiredInMinutes = Convert.ToInt16(configuration["Token:TimeExpired"]);
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("login-employee")]
        public object EmpLogin([FromBody] JObject frmData)
        {
            try
            {
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
                                    var roleName = authentication.RoleName(result.RoleId);
                                    var claim = new[]
                                    {
                                        new Claim(JwtRegisteredClaimNames.Sub, configuration["Token:Subject"]),
                                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                                        new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                                        new Claim(JwtRegisteredClaimNames.Aud, configuration["Token:Audience"]),
                                        new Claim(ClaimTypes.Role, result.RoleId.ToString()),
                                        new Claim(ClaimTypes.NameIdentifier, result.IdEmployee.ToString()),
                                        new Claim(ClaimTypes.Name, roleName),
                                        new Claim("EmployeeId", result.IdEmployee.ToString())
                                     };
                                    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Token:key"]));
                                    var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                                    var token = new JwtSecurityToken(configuration["Token:Issuer"],
                                        configuration["Token:Audience"], claim, expires: DateTime.UtcNow.AddMinutes(TimeExpiredInMinutes),
                                        //configuration["Token:Audience"], claim, expires: DateTime.UtcNow.AddMinutes(525600),
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
                                    auth.DateExpired = dateTimeNow.AddMinutes(TimeExpiredInMinutes);
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
                        return Ok(Ultility.Responses("Không tìm thấy email [" + email + "] trên hệ thống !", Enums.TypeCRUD.Error.ToString()));
                    }
                }
                else
                {
                    return Ok(Ultility.Responses("", Enums.TypeCRUD.Block.ToString(), result.TimeBlock));
                }
            }
            catch (Exception e)
            {
                return Ok(Ultility.Responses("Có lỗi xảy ra !", Enums.TypeCRUD.Block.ToString(), description: e.Message));
            }

        }

        [HttpPost]
        [AllowAnonymous]
        [Route("login-customer")]
        public object CusLogin([FromBody] JObject frmData)
        {
            try
            {
                string email = PrCommon.GetString("email", frmData);
                string password = PrCommon.GetString("password", frmData);
                string googleToken = PrCommon.GetString("googleToken", frmData);

                var result = authentication.CusCheckBlock(email);

                if (result == null)
                {
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

                    if (result != null)
                    {
                        if (string.IsNullOrEmpty(googleToken))
                        {
                            string encryption = authentication.Encryption(password);
                            result = authentication.CusLogin(email, encryption);
                        }
                        else
                        {
                            var status = authentication.CusAddTokenGoogle(googleToken, result.IdCustomer);
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
                            var claim = new[]
                                    {
                                    new Claim(JwtRegisteredClaimNames.Sub, configuration["Token:Subject"]),
                                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                                    new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                                    new Claim(JwtRegisteredClaimNames.Aud, configuration["Token:Audience"]),
                                    new Claim("CustomerId", result.IdCustomer.ToString())
                                };

                            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Token:key"]));
                            var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                            var token = new JwtSecurityToken(configuration["Token:Issuer"],
                                configuration["Token:Audience"], claim, expires: DateTime.UtcNow.AddMinutes(60),
                                //configuration["Token:Audience"], claim, expires: DateTime.UtcNow.AddMinutes(525600),
                                signingCredentials: signIn);

                            var tokenJWT = new JwtSecurityTokenHandler().WriteToken(token);

                            authentication.CusAddToken(tokenJWT, result.IdCustomer);

                            Authentication auth = new Authentication();
                            auth.Token = tokenJWT;
                            auth.Id = result.IdCustomer;
                            auth.Name = result.NameCustomer;
                            auth.Phone = result.Phone;
                            auth.Email = result.Email;

                            return Ok(Ultility.Responses("Đăng nhập thành công !", Enums.TypeCRUD.Success.ToString(), auth));
                        }
                        else
                        {
                            return Ok(Ultility.Responses("Sai mật khẩu !", Enums.TypeCRUD.Error.ToString()));
                        }

                    }
                    else
                    {
                        return Ok(Ultility.Responses("Không tìm thấy email [" + email + "] trên hệ thống !", Enums.TypeCRUD.Error.ToString()));
                    }
                }
                else
                {
                    return Ok(Ultility.Responses("", Enums.TypeCRUD.Block.ToString(), result.TimeBlock));
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
