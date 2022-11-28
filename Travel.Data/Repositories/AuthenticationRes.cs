using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Travel.Context.Models;
using Travel.Context.Models.Travel;
using Travel.Data.Interfaces;
using Travel.Shared.Ultilities;
using Travel.Shared.ViewModels;

namespace Travel.Data.Responsives
{
    public class AuthenticationRes : IAuthentication
    {
        private readonly TravelContext _db;
        private Response res;
        IConfiguration _config;
        public AuthenticationRes(TravelContext db, IConfiguration config)
        {
            _db = db;
            res = new Response();
            _config = config;
        }
        public Employee EmpLogin(string email)
        {
            try
            {
                //var result = _db.Employees.Where(x => x.IsDelete == false &&
                //                                      x.Email == email).FirstOrDefault();
                var result = (from x in _db.Employees
                              where x.IsDelete == false &&
                                    x.Email == email
                              select x).FirstOrDefault();
                return result;

            }
            catch (Exception e)
            {
                return null;
            }
        }

        public Employee EmpLogin(string email, string password)
        {
            try
            {
                //var result = _db.Employees.Where(x => x.IsDelete == false &&
                //                                      x.Password  == password &&
                //                                      x.Email == email).FirstOrDefault();
                var result = (from x in _db.Employees
                              where x.IsDelete == false &&
                                    x.Email == email &&
                                    x.Password == password
                              select x).FirstOrDefault();
                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public string RoleName(int roleId)
        {
            return  _db.Roles.Find(roleId).NameRole;
        }
        public bool EmpAddToken(string token, Guid idEmp)
        {
            try
            {
                _db.Employees.Find(idEmp).AccessToken = token;
                _db.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool EmpActive(string email)
        {
            try
            {
                //var result = _db.Employees.Where(x => x.IsDelete == false &&
                //                                      x.IsActive == true &&
                //                                      x.Email == email).FirstOrDefault();
                var result = (from x in _db.Employees
                              where x.IsDelete == false &&
                                    x.IsActive == true &&
                                    x.Email == email
                              select x).FirstOrDefault();
                return (result != null) ? true : false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public Response EmpBlock(string email)
        {
            try
            {
                var timeBlock = Ultility.ConvertDatetimeToUnixTimeStampMiliSecond(DateTime.Now.AddMinutes(30));
                var result = (from x in _db.Employees
                              where x.IsDelete == false &&
                                    x.Email == email    
                              select x).FirstOrDefault();
                if (result != null)
                {
                    result.IsBlock = true;
                    result.TimeBlock = timeBlock;
                    _db.SaveChanges();
                    return Ultility.Responses("", Enums.TypeCRUD.Success.ToString());
                }
                return Ultility.Responses("", Enums.TypeCRUD.Error.ToString());
            }
            catch (Exception e)
            {
                return Ultility.Responses("Có lỗi xảy ra !", Enums.TypeCRUD.Error.ToString(), description: e.Message);
            }
        }

        public Employee EmpCheckBlock(string email)
        {
            try
            {
                var result = (from x in _db.Employees
                              where x.IsDelete == false &&
                                    x.IsBlock == true &&
                                    x.Email == email
                              select x).FirstOrDefault();
                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public Response EmpUnBlock(string email)
        {
            try
            {
                var result = (from x in _db.Employees
                              where x.IsDelete == false &&
                                    x.IsBlock == true &&
                                    x.Email == email
                              select x).FirstOrDefault();
                if (result != null)
                {
                    result.IsBlock = false;
                    result.TimeBlock = 0;
                    _db.SaveChanges();
                    return Ultility.Responses("", Enums.TypeCRUD.Success.ToString());
                }
                return Ultility.Responses("", Enums.TypeCRUD.Error.ToString());
            }
            catch (Exception e)
            {
                return Ultility.Responses("Có lỗi xảy ra !", Enums.TypeCRUD.Error.ToString(), description: e.Message);
            }
        }

        public bool EmpIsNew(string email)
        {
            try
            {
                //var result = _db.Users.Where(x => x.IsDelete == false &&
                //                                      x.IsNew == false &&
                //                                      x.UserEmail == email).FirstOrDefault();
                //return (result != null) ? true : false;
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool EmpDeleteToken(string Id)
        {
            try
            {
                try
                {
                    //var user = _db.Users.Where(x => x.UserId == userId).FirstOrDefault();
                    //user.UserToken = null;
                    //user.UserStatus = false;
                    //_db.SaveChanges();
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public Response CusDeleteToken(Guid idCus)
        {
            try
            {
                var cus = (from x in _db.Customers
                           where x.IsDelete == false &&
                                 x.IdCustomer == idCus
                           select x).FirstOrDefault();
                if (cus != null)
                {
                    cus.AccessToken = null;
                    cus.GoogleToken = null;
                    cus.FbToken = null;
                    _db.SaveChanges();
                }
                res.Notification.DateTime = DateTime.Now;
                res.Notification.Messenge = "Đăng xuất thành công !";
                res.Notification.Type = "Success";
                return res;
            }
            catch (Exception e)
            {
                res.Notification.DateTime = DateTime.Now;
                res.Notification.Description = e.Message;
                res.Notification.Messenge = "Có lỗi xảy ra !";
                res.Notification.Type = "Error";
                return res;
            }
        }

        public Response EmpDeleteToken(Guid idEmp)
        {
            try
            {
                var emp = (from x in _db.Employees
                           where x.IsDelete == false &&
                                 x.IdEmployee == idEmp
                           select x).FirstOrDefault();
                emp.AccessToken = null;
                _db.SaveChanges();
                res.Notification.DateTime = DateTime.Now;
                res.Notification.Messenge = "Đăng xuất thành công !";
                res.Notification.Type = "Success";
                return res;
            }
            catch (Exception e)
            {
                res.Notification.DateTime = DateTime.Now;
                res.Notification.Description = e.Message;
                res.Notification.Messenge = "Có lỗi xảy ra !";
                res.Notification.Type = "Error";
                return res;
            }
        }

        public Customer CusLogin(string email)
        {
            try
            {
                //var result = _db.Employees.Where(x => x.IsDelete == false &&
                //                                      x.Email == email).FirstOrDefault();
                var result = (from x in _db.Customers
                              where x.IsDelete == false &&
                                    x.Email == email
                              select x).FirstOrDefault();
                return result;

            }
            catch (Exception)
            {
                return null;
            }
        }

        public Customer CusLogin(string email, string password)
        {
            try
            {
                var result = (from x in _db.Customers
                              where x.IsDelete == false &&
                                    x.Email == email &&
                                    x.Password == password
                              select x).FirstOrDefault();

                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public bool CusAddTokenGoogle(string token, Guid idCus)
        {
            try
            {
                _db.Customers.Find(idCus).GoogleToken = token;
                _db.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }


        public bool CusAddToken(string token, Guid idCus)
        {
            try
            {
                _db.Customers.Find(idCus).AccessToken = token;
                _db.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool CreateAccountGoogle(Customer cus)
        {
            try
            {
                cus.CreateDate = Ultility.ConvertDatetimeToUnixTimeStampMiliSecond(DateTime.Now);
                _db.Customers.Add(cus);
                _db.SaveChanges();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
        //public bool CusActive(string email)
        //{
        //    try
        //    {
        //        //var result = _db.Employees.Where(x => x.IsDelete == false &&
        //        //                                      x.IsActive == true &&
        //        //                                      x.Email == email).FirstOrDefault();
        //        var result = (from x in _db.Customers
        //                      where x.IsDelete == false &&
        //                            x.IsActive == true &&
        //                            x.Email == email
        //                      select x).FirstOrDefault();
        //        return (result != null) ? true : false;


        //    }
        //    catch (Exception)
        //    {
        //        return false;
        //    }
        //}
        public Customer CusCheckBlock(string email)
        {
            try
            {
                var result = (from x in _db.Customers
                              where x.IsDelete == false &&
                                    x.IsBlock == true &&
                                    x.Email == email
                              select x).FirstOrDefault();

                if (result != null)
                {
                    var dateNow = Ultility.ConvertDatetimeToUnixTimeStampMiliSecond(DateTime.Now);
                    if (result.TimeBlock <= dateNow)
                    {
                        result.IsBlock = false;
                        result.TimeBlock = 0;
                        _db.SaveChanges();
                        return null;
                    }
                }
                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public Response CusBlock(string email)
        {
            try
            {
                var timeBlock = Ultility.ConvertDatetimeToUnixTimeStampMiliSecond(DateTime.Now.AddMinutes(30));
                var result = (from x in _db.Customers
                              where x.IsDelete == false &&
                                    x.Email == email
                              select x).FirstOrDefault();
                if (result != null)
                {
                    result.IsBlock = true;
                    result.TimeBlock = timeBlock;
                    _db.SaveChanges();
                    return Ultility.Responses("", Enums.TypeCRUD.Success.ToString());
                }
                return Ultility.Responses("", Enums.TypeCRUD.Error.ToString());

            }
            catch (Exception e)
            {
                return Ultility.Responses("Có lỗi xảy ra !", Enums.TypeCRUD.Error.ToString(), description: e.Message);
            }
        }

        public Response CusUnBlock(string email)
        {
            try
            {
                var result = (from x in _db.Customers
                              where x.IsDelete == false &&
                                    x.IsBlock == true &&
                                    x.Email == email
                              select x).FirstOrDefault();
                if (result != null)
                {
                    result.IsBlock = false;
                    result.TimeBlock = 0;
                    _db.SaveChanges();
                    return Ultility.Responses("", Enums.TypeCRUD.Success.ToString());
                }
                return Ultility.Responses("", Enums.TypeCRUD.Error.ToString());
            }
            catch (Exception e)
            {
                return Ultility.Responses("Có lỗi xảy ra !", Enums.TypeCRUD.Error.ToString(), description: e.Message);
            }
        }

        public Response CusChangePassword(Guid idCus, string password, string newPassword)
        {
            try
            {
                Customer cus = _db.Customers.Find(idCus);
                if (cus != null)
                {

                    if (cus.Password == Ultility.Encryption(password))
                    {
                        cus.Password = Ultility.Encryption(newPassword);
                        _db.SaveChanges();


                        res.Notification.DateTime = DateTime.Now;
                        res.Notification.Messenge = "Đổi mật khẩu thành công, mời đăng nhập lại !";
                        res.Notification.Type = "Success";
                    }
                    else
                    {
                        res.Notification.DateTime = DateTime.Now;
                        res.Notification.Messenge = "Mật khẩu cũ không đúng, mời nhập lại !";
                        res.Notification.Type = "Warning";
                    }
                }
                return res;
            }
            catch (Exception e)
            {
                res.Notification.DateTime = DateTime.Now;
                res.Notification.Description = e.Message;
                res.Notification.Messenge = "Có lỗi xảy ra !";
                res.Notification.Type = "Error";
                return res;
            }
        }

        public Response CusForgotPassword(string email, string password)
        {
            try
            {
                var account = (from x in _db.Customers
                               where x.Email.ToLower() == email.ToLower()
                               select x).FirstOrDefault();
                if (account != null)
                {
                    account.Password = Ultility.Encryption(password);
                    _db.SaveChanges();

                    return Ultility.Responses("Cập nhật mật khẩu thành công, mời đăng nhập lại !", Enums.TypeCRUD.Success.ToString());
                }
                else
                {
                    return Ultility.Responses($"{email} không tồn tại!", Enums.TypeCRUD.Warning.ToString());
                }
            }
            catch (Exception e)
            {
                return Ultility.Responses("Có lỗi xảy ra !", Enums.TypeCRUD.Error.ToString(), description: e.Message);
            }
        }

        public string Encryption(string password)
        {
            MD5CryptoServiceProvider MD5 = new MD5CryptoServiceProvider();
            byte[] Encrypt;
            UTF8Encoding Encode = new UTF8Encoding();
            Encrypt = MD5.ComputeHash(Encode.GetBytes(password));
            StringBuilder Encryptdata = new StringBuilder();
            for (int i = 0; i < Encrypt.Length; i++)
            {
                Encryptdata.Append(Encrypt[i].ToString());
            }
            return Encryptdata.ToString();
        }

        public Response EmpChangePassword(Guid idEmp, string password, string newPassword)
        {
            try
            {
                Employee emp = _db.Employees.Find(idEmp);
                if (emp != null)
                {

                    if (emp.Password == Ultility.Encryption(password))
                    {
                        emp.Password = Ultility.Encryption(newPassword);
                        _db.SaveChanges();

                        return Ultility.Responses("Đổi mật khẩu thành công, mời đăng nhập lại !", Enums.TypeCRUD.Success.ToString());
                    }
                    else
                    {
                        return Ultility.Responses("Sai mật khẩu cũ !", Enums.TypeCRUD.Error.ToString());

                    }
                }
                return Ultility.Responses("", Enums.TypeCRUD.Error.ToString());
            }
            catch (Exception e)
            {
                return Ultility.Responses("Có lỗi xảy ra !", Enums.TypeCRUD.Error.ToString(), description: e.Message);
            }
        }

        public Response EmpForgotPassword(string email, string password)
        {
            try
            {
                var account = (from x in _db.Employees
                               where x.Email.ToLower() == email.ToLower()
                               select x).FirstOrDefault();
                if (account != null)
                {
                    account.Password = Ultility.Encryption(password);
                    _db.SaveChanges();

                    return Ultility.Responses("Cập nhật mật khẩu thành công, mời đăng nhập lại !", Enums.TypeCRUD.Success.ToString());
                }
                else
                {
                    return Ultility.Responses($"{email} không tồn tại!", Enums.TypeCRUD.Warning.ToString());
                }
            }
            catch (Exception e)
            {
                return Ultility.Responses("Có lỗi xảy ra !", Enums.TypeCRUD.Error.ToString(), description: e.Message);
            }
        }
    }
}
