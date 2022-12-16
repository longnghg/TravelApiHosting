using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using PrUtility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Travel.Context.Models;
using Travel.Context.Models.Travel;
using Travel.Data.Interfaces;
using Travel.Shared.Ultilities;
using Travel.Shared.ViewModels;
using Travel.Shared.ViewModels.Travel;
using static Travel.Shared.Ultilities.Enums;


namespace Travel.Data.Repositories
{
    public class EmployeeRes : IEmployee
    {
        private readonly TravelContext _db;
        private Notification message;
        private Response res;
        private readonly IConfiguration _config;
        private readonly ILog _log;
        public EmployeeRes(TravelContext db, IConfiguration config , ILog log)
        {
            _db = db;
            message = new Notification();
            res = new Response();
            _config = config;
            _log = log;
        }
        private void UpdateDatabase(Employee input)
        {
            _db.Entry(input).State = EntityState.Modified;
            _db.SaveChanges();
        }
        // validate vd create
        public string CheckBeforeSave(IFormCollection frmdata, IFormFile file, ref Notification _message, bool isUpdate) // hàm đăng nhập  sử cho create update delete
        {
            try
            {
                JObject frmData = JObject.Parse(frmdata["data"]);
                if (frmData != null)
                {
                    var idEmployee = PrCommon.GetString("idEmployee", frmData);
                    if (String.IsNullOrEmpty(idEmployee))
                    {
                        idEmployee = Guid.NewGuid().ToString();
                    }

                    var nameEmployee = PrCommon.GetString("nameEmployee", frmData);
                    if (String.IsNullOrEmpty(nameEmployee))
                    {

                    }

                    var email = PrCommon.GetString("email", frmData);
                    if (!String.IsNullOrEmpty(email) && isUpdate == false)
                    {
                        var check = CheckEmailEmployee(email);
                        if (check.Notification.Type == "Validation" || check.Notification.Type == "Error")
                        {
                            _message = check.Notification;
                            return string.Empty;
                        }
                    }


                    var phone = PrCommon.GetString("phone", frmData);
                    if (!String.IsNullOrEmpty(phone) && isUpdate == false)
                    {
                        var check = CheckPhoneEmployee(phone);
                        if (check.Notification.Type == "Validation" || check.Notification.Type == "Error")
                        {
                            _message = check.Notification;
                            return string.Empty;
                        }
                    }

                    var roleId = PrCommon.GetString("roleId", frmData);
                    if (String.IsNullOrEmpty(roleId))
                    {
                    }



                    var birthday = PrCommon.GetString("birthday", frmData);
                    if (String.IsNullOrEmpty(birthday))
                    {
                    }


                    var address = PrCommon.GetString("address", frmData);
                    if (String.IsNullOrEmpty(address))
                    {
                    }

                    var gender = PrCommon.GetString("gender", frmData);
                    if (String.IsNullOrEmpty(birthday))
                    {
                    }


                    var image = PrCommon.GetString("image", frmData);
                    if (String.IsNullOrEmpty(image))
                    {
                    }

                    var modifyBy = PrCommon.GetString("modifyBy", frmData);
                    if (String.IsNullOrEmpty(modifyBy))
                    {
                    }

                    var isActive = PrCommon.GetString("isActive", frmData);
                    if (String.IsNullOrEmpty(isActive))
                    {
                    }

                    if (file != null)
                    {
                        image = Ultility.WriteFile(file, "Employee", idEmployee, ref _message).FilePath;
                        if (_message != null)
                        {
                            message = _message;
                        }
                    }

                    if (isUpdate)
                    {
                        UpdateEmployeeViewModel objUpdate = new UpdateEmployeeViewModel();
                        objUpdate.IdEmployee = Guid.Parse(idEmployee);
                        objUpdate.NameEmployee = nameEmployee;
                        objUpdate.Phone = phone;
                        objUpdate.Email = email;
                        objUpdate.Address = address;
                        try
                        {
                            var date = DateTime.Parse(birthday);
                            objUpdate.Birthday = Ultility.ConvertDatetimeToUnixTimeStampMiliSecond(date);
                        }
                        catch (Exception)
                        {
                            objUpdate.Birthday = long.Parse(birthday);
                        }
                        objUpdate.RoleId = (TitleRole)int.Parse(roleId);
                        if (!String.IsNullOrEmpty(image))
                        {
                            objUpdate.Image = image;

                        }
                        objUpdate.IsActive = bool.Parse(isActive);
                        objUpdate.ModifyBy = modifyBy;
                        return JsonSerializer.Serialize(objUpdate);
                    }

                    CreateEmployeeViewModel objCreate = new CreateEmployeeViewModel();
                    objCreate.IdEmployee = Guid.Parse(idEmployee);
                    objCreate.NameEmployee = nameEmployee;
                    objCreate.Phone = phone;
                    objCreate.Email = email;
                    objCreate.Address = address;
                    try
                    {
                        var date = DateTime.Parse(birthday);
                        objCreate.Birthday = Ultility.ConvertDatetimeToUnixTimeStampMiliSecond(date);
                    }
                    catch (Exception)
                    {
                        objCreate.Birthday = long.Parse(birthday);
                    }
                    objCreate.RoleId = (TitleRole)int.Parse(roleId);
                    objCreate.Image = image;
                    objCreate.ModifyBy = modifyBy;
                    return JsonSerializer.Serialize(objCreate);


                }
                return string.Empty;
            }
            catch (Exception e)
            {
                message.DateTime = DateTime.Now;
                message.Description = e.Message;
                message.Messenge = "Có lỗi xảy ra !";
                message.Type = "Error";

                _message = message;
                return string.Empty;
            }
        }
        public Response GetsEmployee(bool isDelete)
        {
            try
            {

                //var isDelete = false;
                //var check = PrCommon.GetString("isDelete", frmData);
                //if (!String.IsNullOrEmpty(check))
                //{
                //    isDelete = Boolean.Parse(check);
                //}
                #region đo tốc độ EF và linq
                //var stopWatch4 = Stopwatch.StartNew();
                //var result5 = _db.Employees.ToList();
                //var b4 = stopWatch4.Elapsed;

                //var stopWatch5 = Stopwatch.StartNew();
                //var result6 = (from x in _db.Employees select x).ToList();
                //var b5 = stopWatch5.Elapsed;
                #endregion

                var queryListEmp = (from x in _db.Employees.AsNoTracking()
                               where x.IsDelete == isDelete && x.IsActive
                               orderby x.RoleId
                               select new Employee
                               {
                                   CreateDate = x.CreateDate,
                                   AccessToken = x.AccessToken,
                                   Address = x.Address,
                                   Birthday = x.Birthday,
                                   Email = x.Email,
                                   IsDelete = x.IsDelete,
                                   Gender = x.Gender,
                                   ModifyDate = x.ModifyDate,
                                   IdEmployee = x.IdEmployee,
                                   Image = x.Image,
                                   IsActive = x.IsActive,
                                   ModifyBy = x.ModifyBy,
                                   NameEmployee = x.NameEmployee,
                                   Password = x.Password,
                                   Phone = x.Phone,
                                   Role = (from r in _db.Roles.AsNoTracking() where r.IdRole == x.RoleId select r).First(),
                                   RoleId = x.RoleId,
                               });
                int totalResult = queryListEmp.Count();
                var listEmp = queryListEmp.ToList();
                var result = Mapper.MapEmployee(listEmp);
                var res = Ultility.Responses("", Enums.TypeCRUD.Success.ToString(), result);
                res.TotalResult = totalResult;
                return res;
            }
            catch (Exception e)
            {

                return Ultility.Responses("Có lỗi xảy ra !", Enums.TypeCRUD.Error.ToString(), description: e.Message);

            }
        }

        public Response CreateEmployee(CreateEmployeeViewModel input, string emailUser)
        {
            try
            {
                Employee employee = Mapper.MapCreateEmployee(input);
                employee.IsActive = true;
                employee.Password = "3244185981728979115075721453575112";
                string jsonContent = JsonSerializer.Serialize(employee);
                _db.Employees.Add(employee);
                _db.SaveChanges();
                bool result = _log.AddLog(content: jsonContent, type: "create", emailCreator: emailUser, classContent: "Employee");
                if (result)
                {    return Ultility.Responses("Tạo mới thành công !", Enums.TypeCRUD.Success.ToString());
                }
                else
                {
                    return Ultility.Responses("Lỗi log!", Enums.TypeCRUD.Error.ToString());
                }
            
            }
            catch (Exception e)
            {
                return Ultility.Responses("Có lỗi xảy ra !", Enums.TypeCRUD.Error.ToString(), description: e.Message);
            }
        }

        public Response UpdateEmployee(UpdateEmployeeViewModel input, string emailUser)
        {
            try
            {
                Employee employee = Mapper.MapCreateEmployee(input);
                string jsonContent = JsonSerializer.Serialize(employee);
                _db.Employees.Update(employee);
                _db.SaveChanges();
                bool result = _log.AddLog(content: jsonContent, type: "update", emailCreator: emailUser, classContent: "Employee");
                if (result)
                {
                    return Ultility.Responses("Chỉnh sửa thành công !", Enums.TypeCRUD.Success.ToString());
                }
                else
                {
                    return Ultility.Responses("Lỗi log!", Enums.TypeCRUD.Error.ToString());
                }
               
            }
            catch (Exception e)
            {

                return Ultility.Responses("Có lỗi xảy ra !", Enums.TypeCRUD.Error.ToString(), description: e.Message);

            }
        }

        public Response SearchEmployee(JObject frmData)
        {
            try
            {
                var totalResult = 0;
                Keywords keywords = new Keywords();
                var pageSize = PrCommon.GetString("pageSize", frmData) == null ? 10 : Convert.ToInt16(PrCommon.GetString("pageSize", frmData));
                var pageIndex = PrCommon.GetString("pageIndex", frmData) == null ? 1 : Convert.ToInt16(PrCommon.GetString("pageIndex", frmData));

                var isDelete = PrCommon.GetString("isDelete", frmData);
                if (!String.IsNullOrEmpty(isDelete))
                {
                    keywords.IsDelete = Boolean.Parse(isDelete);
                }

                var kwId = PrCommon.GetString("idEmployee", frmData);
                if (!String.IsNullOrEmpty(kwId))
                {
                    keywords.KwId = kwId.Trim().ToLower();
                }
                else
                {
                    keywords.KwId = "";

                }

                var kwName = PrCommon.GetString("nameEmployee", frmData).Trim();
                if (!String.IsNullOrEmpty(kwName))
                {
                    keywords.KwName = kwName.Trim().ToLower();
                }
                else
                {
                    keywords.KwName = "";

                }

                var kwPhone = PrCommon.GetString("phone", frmData).Trim();
                if (!String.IsNullOrEmpty(kwPhone))
                {
                    keywords.KwPhone = kwPhone.Trim().ToLower();
                }
                else
                {
                    keywords.KwPhone = "";
                }

                var kwEmail = PrCommon.GetString("email", frmData).Trim();
                if (!String.IsNullOrEmpty(kwEmail))
                {
                    keywords.KwEmail = kwEmail.Trim().ToLower();
                }
                else
                {
                    keywords.KwEmail = "";

                }

                var kwIdRole = PrCommon.GetString("roleId", frmData);
                keywords.KwIdRole = PrCommon.getListInt(kwIdRole, ',', false);

                var kwIsActive = PrCommon.GetString("isActive", frmData);
                if (!String.IsNullOrEmpty(kwIsActive))
                {
                    keywords.KwIsActive = Boolean.Parse(kwIsActive);
                }


                //var listEmp = _db.Employees.FromSqlRaw("[SearchEmployees] {0}, {1}, {2}, {3}, {4}, {5}", kwId, kwName, kwEmail, kwPhone, kwRoleId, kwIsActive).ToList();
                var listEmp = new List<Employee>();
                if (keywords.KwIdRole.Count > 0)
                {
                    if (!string.IsNullOrEmpty(kwIsActive))
                    {

                        var queryListEmployee = (from x in _db.Employees.AsNoTracking()
                                   where x.IsDelete == keywords.IsDelete &&
                                                   x.IdEmployee.ToString().ToLower().Contains(keywords.KwId) &&
                                                   x.NameEmployee.ToLower().Contains(keywords.KwName) &&
                                                   x.Email.ToLower().Contains(keywords.KwEmail) &&
                                                   x.Phone.ToLower().Contains(keywords.KwPhone) &&
                                                   x.IsActive == keywords.KwIsActive &&
                                                   keywords.KwIdRole.Contains(x.RoleId)
                                   orderby x.RoleId
                                   select new Employee
                                   {
                                       CreateDate = x.CreateDate,
                                       AccessToken = x.AccessToken,
                                       Address = x.Address,
                                       Birthday = x.Birthday,
                                       Email = x.Email,
                                       IsDelete = x.IsDelete,
                                       Gender = x.Gender,
                                       ModifyDate = x.ModifyDate,
                                       IdEmployee = x.IdEmployee,
                                       Image = x.Image,
                                       IsActive = x.IsActive,
                                       ModifyBy = x.ModifyBy,
                                       NameEmployee = x.NameEmployee,
                                       Password = x.Password,
                                       Phone = x.Phone,
                                       Role = (from r in _db.Roles.AsNoTracking()
                                               where r.IdRole == x.RoleId select r).FirstOrDefault(),
                                       RoleId = x.RoleId
                                   });
                        totalResult = queryListEmployee.Count();
                        listEmp = queryListEmployee.Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();
                    }
                    else
                    {
                        var queryListEmployee = (from x in _db.Employees.AsNoTracking()
                                             where x.IsDelete == keywords.IsDelete &&
                                                             x.IdEmployee.ToString().ToLower().Contains(keywords.KwId) &&
                                                             x.NameEmployee.ToLower().Contains(keywords.KwName) &&
                                                             x.Email.ToLower().Contains(keywords.KwEmail) &&
                                                             x.Phone.ToLower().Contains(keywords.KwPhone) &&
                                                             keywords.KwIdRole.Contains(x.RoleId)
                                             orderby x.RoleId
                                             select new Employee
                                             {
                                                 CreateDate = x.CreateDate,
                                                 AccessToken = x.AccessToken,
                                                 Address = x.Address,
                                                 Birthday = x.Birthday,
                                                 Email = x.Email,
                                                 IsDelete = x.IsDelete,
                                                 Gender = x.Gender,
                                                 ModifyDate = x.ModifyDate,
                                                 IdEmployee = x.IdEmployee,
                                                 Image = x.Image,
                                                 IsActive = x.IsActive,
                                                 ModifyBy = x.ModifyBy,
                                                 NameEmployee = x.NameEmployee,
                                                 Password = x.Password,
                                                 Phone = x.Phone,
                                                 Role = (from r in _db.Roles.AsNoTracking() where r.IdRole == x.RoleId select r).First(),
                                                 RoleId = x.RoleId
                                             });
                        totalResult = queryListEmployee.Count();
                        listEmp = queryListEmployee.Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(kwIsActive))
                    {
                        var queryListEmployee = (from x in _db.Employees.AsNoTracking()
                                   where x.IsDelete == keywords.IsDelete &&
                                                   x.IdEmployee.ToString().ToLower().Contains(keywords.KwId) &&
                                                   x.NameEmployee.ToLower().Contains(keywords.KwName) &&
                                                   x.Email.ToLower().Contains(keywords.KwEmail) &&
                                                   x.Phone.ToLower().Contains(keywords.KwPhone) &&
                                                   x.IsActive == keywords.KwIsActive
                                   orderby x.RoleId
                                   select new Employee
                                   {
                                       CreateDate = x.CreateDate,
                                       AccessToken = x.AccessToken,
                                       Address = x.Address,
                                       Birthday = x.Birthday,
                                       Email = x.Email,
                                       IsDelete = x.IsDelete,
                                       Gender = x.Gender,
                                       ModifyDate = x.ModifyDate,
                                       IdEmployee = x.IdEmployee,
                                       Image = x.Image,
                                       IsActive = x.IsActive,
                                       ModifyBy = x.ModifyBy,
                                       NameEmployee = x.NameEmployee,
                                       Password = x.Password,
                                       Phone = x.Phone,
                                       Role = (from r in _db.Roles.AsNoTracking() where r.IdRole == x.RoleId select r).First(),
                                       RoleId = x.RoleId
                                   });
                        totalResult = queryListEmployee.Count();
                        listEmp = queryListEmployee.Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();
                    }
                    else
                    {
                        var queryListEmployee = (from x in _db.Employees.AsNoTracking()
                                   where x.IsDelete == keywords.IsDelete &&
                                                   x.IdEmployee.ToString().ToLower().Contains(keywords.KwId) &&
                                                   x.NameEmployee.ToLower().Contains(keywords.KwName) &&
                                                   x.Email.ToLower().Contains(keywords.KwEmail) &&
                                                   x.Phone.ToLower().Contains(keywords.KwPhone)
                                   orderby x.RoleId
                                   select new Employee
                                   {
                                       CreateDate = x.CreateDate,
                                       AccessToken = x.AccessToken,
                                       Address = x.Address,
                                       Birthday = x.Birthday,
                                       Email = x.Email,
                                       IsDelete = x.IsDelete,
                                       Gender = x.Gender,
                                       ModifyDate = x.ModifyDate,
                                       IdEmployee = x.IdEmployee,
                                       Image = x.Image,
                                       IsActive = x.IsActive,
                                       ModifyBy = x.ModifyBy,
                                       NameEmployee = x.NameEmployee,
                                       Password = x.Password,
                                       Phone = x.Phone,
                                       Role = (from r in _db.Roles.AsNoTracking() where r.IdRole == x.RoleId select r).First(),
                                       RoleId = x.RoleId
                                   });
                        totalResult = queryListEmployee.Count();
                        listEmp = queryListEmployee.Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();
                    }
                }
                var result = Mapper.MapEmployee(listEmp);
                if (result.Count() > 0)
                {
                    var res = Ultility.Responses("", Enums.TypeCRUD.Success.ToString(), result);
                    res.TotalResult = totalResult;
                    return res;
                }
                else
                {
                    var res = Ultility.Responses("Không tìm thấy dữ liệu !", Enums.TypeCRUD.Warning.ToString());
                    res.TotalResult = totalResult;
                    return res;
                }
            }
            catch (Exception e)
            {
                return Ultility.Responses("Có lỗi xảy ra !", Enums.TypeCRUD.Error.ToString(), description: e.Message);
            }
        }

        public Response RestoreEmployee(Guid idEmployee, string emailUser)
        {
            try
            {
                var employee = (from x in _db.Employees.AsNoTracking()
                                where x.IdEmployee == idEmployee
                                select x).FirstOrDefault();
                if (employee != null)
                {
                    string jsonContent = JsonSerializer.Serialize(employee);

                    employee.IsDelete = false;
                    UpdateDatabase(employee);
                    _db.SaveChanges();
                    bool result = _log.AddLog(content: jsonContent, type: "restore", emailCreator: emailUser, classContent: "Employee");
                    if (result)
                    {
                        return Ultility.Responses($"Khôi phục thành công !", Enums.TypeCRUD.Success.ToString());

                    }
                    else
                    {
                        return Ultility.Responses("Lỗi log!", Enums.TypeCRUD.Error.ToString());
                    }

               

                }
                else
                {
                    return Ultility.Responses($"Không tìm thấy !", Enums.TypeCRUD.Warning.ToString());

                }
            }
            catch (Exception e)
            {
                return Ultility.Responses("Có lỗi xảy ra !", Enums.TypeCRUD.Error.ToString(), description: e.Message);


            }
        }

        public Response DeleteEmployee(Guid idEmployee, string emailUser)
        {
            try
            {
                var employee = (from x in _db.Employees.AsNoTracking()
                                where x.IdEmployee == idEmployee
                                select x).FirstOrDefault();
                if (employee.RoleId != -1)
                {
                    if (employee != null)
                    {
                        string jsonContent = JsonSerializer.Serialize(employee);

                        employee.IsDelete = true;
                        UpdateDatabase(employee);
                        _db.SaveChanges();

                        bool result = _log.AddLog(content: jsonContent, type: "delete", emailCreator: emailUser, classContent: "Employee");
                        if (result)
                        {
                            return Ultility.Responses($"Xóa thành công !", Enums.TypeCRUD.Success.ToString());

                        }
                        else
                        {
                            return Ultility.Responses("Lỗi log!", Enums.TypeCRUD.Error.ToString());
                        }
                    }
                    else
                    {
                        return Ultility.Responses($"Không tìm thấy !", Enums.TypeCRUD.Warning.ToString());

                    }
                }
                else
                {
                    return Ultility.Responses($"Không được xóa ADMIN !", Enums.TypeCRUD.Warning.ToString());

                }

            }
            catch (Exception e)
            {
                return Ultility.Responses("Có lỗi xảy ra !", Enums.TypeCRUD.Error.ToString(), description: e.Message);


            }
        }

        public Response CheckEmailEmployee(string email)
        {
            try
            {
                var emp = (from x in _db.Employees.AsNoTracking()
                           where x.IsDelete == false && x.Email == email select x).Count();
                if (emp > 0)
                {
                    return Ultility.Responses("[" + email + "] này đã được đăng ký !", Enums.TypeCRUD.Validation.ToString());
                }
                return res;
            }
            catch (Exception e)
            {

                return Ultility.Responses("Có lỗi xảy ra !", Enums.TypeCRUD.Error.ToString(), description: e.Message);

            }
        }

        public Response CheckPhoneEmployee(string phone, string idEmployee = null)
        {
            try
            {
                if (!string.IsNullOrEmpty(idEmployee)) // update
                {
                    Guid id = Guid.Parse(idEmployee);
                    string oldPhone = (from x in _db.Employees.AsNoTracking()
                                       where x.IdEmployee == id select x).First().Phone;
                    if (phone != oldPhone) // có thay đổi  sdt
                    {
                        var obj = (from x in _db.Employees where x.Phone != oldPhone && x.Phone == phone select x).Count();
                        if (obj > 0)
                        {
                            return Ultility.Responses("[" + phone + "] này đã được đăng ký !", Enums.TypeCRUD.Validation.ToString());
                        }
                    }
                }
                else // create
                {
                    var emp = (from x in _db.Employees where x.Phone == phone select x).Count();
                    if (emp > 0)
                    {
                        return Ultility.Responses("[" + phone + "] này đã được đăng ký !", Enums.TypeCRUD.Validation.ToString());
                    }
                }
                return res;

            }
            catch (Exception e)
            {

                return Ultility.Responses("Có lỗi xảy ra !", Enums.TypeCRUD.Error.ToString(), description: e.Message);

            }
        }

        public Response GetEmployee(Guid idEmployee)
        {
            try
            {
                var employee = (from x in _db.Employees.AsNoTracking()
                                where x.IdEmployee == idEmployee
                                select x).First();
                if (employee != null)
                {
                    var result = Mapper.MapEmployee(employee);
                    res = Ultility.Responses("", Enums.TypeCRUD.Success.ToString(), result);
                }
                return res;
            }
            catch (Exception e)
            {
                return Ultility.Responses("Có lỗi xảy ra !", Enums.TypeCRUD.Error.ToString(), description: e.Message);

            }
        }

        public Response StatisticEmployee()
        {
            try
            {
                var lsEmployeeOnline = (from x in _db.Employees.AsNoTracking()
                                        where x.IsDelete == false
                                        && x.IsOnline == true
                                        select x).ToList();
                var lsEmployee = (from x in _db.Employees.AsNoTracking()
                                  where x.IsDelete == false
                                        && x.IsActive == true
                                        select x).ToList();
                var lsEmployeeUnActive = (from x in _db.Employees.AsNoTracking()
                                          where x.IsDelete == false
                                          && x.IsActive == false
                                          select x).ToList();
                var result = lsEmployeeOnline.Concat(lsEmployee).Concat(lsEmployeeUnActive);
                if (result.Count() > 0)
                {
                    res = Ultility.Responses("", Enums.TypeCRUD.Success.ToString(), result);
                }
                return res;

            }
            catch (Exception e)
            {
                return Ultility.Responses("Có lỗi xảy ra !", Enums.TypeCRUD.Error.ToString(), description: e.Message);

            }
        }

        public async Task<Response> SendOTP(string email)
        {
            try
            {
                var account = (from x in _db.Employees.AsNoTracking()
                               where x.Email.ToLower() == email.ToLower()
                               select x).FirstOrDefault();
                if (account != null)
                {
                    string otpCode = Ultility.RandomString(8, false);
                    OTP obj = new OTP();
                    var dateTime = DateTime.Now;
                    var begin = dateTime;
                    var end = dateTime.AddMinutes(2);
                    obj.BeginTime = Ultility.ConvertDatetimeToUnixTimeStampMiliSecond(begin);
                    obj.EndTime = Ultility.ConvertDatetimeToUnixTimeStampMiliSecond(end);
                    obj.OTPCode = otpCode;

                    var subjectOTP = _config["OTPSubject"];
                    var emailSend = _config["emailSend"];
                    var keySecurity = _config["keySecurity"];
                    var stringHtml = Ultility.getHtml(otpCode, subjectOTP, "OTP");

                    Ultility.sendEmail(stringHtml, email, "Yêu cầu quên mật khẩu", emailSend, keySecurity);
                    return Ultility.Responses($"Mã OTP đã gửi vào email {email}!", Enums.TypeCRUD.Success.ToString(), obj);

                }
                else
                {
                    return Ultility.Responses($"{email} không tồn tại!", Enums.TypeCRUD.Error.ToString());
                }
            }
            catch (Exception e)
            {
                return Ultility.Responses("Có lỗi xảy ra !", Enums.TypeCRUD.Error.ToString(), description: e.Message);
            }
        }
        public async Task<Response> SendFile(string email)
        {
            try
            {
                var account = (from x in _db.Employees.AsNoTracking()
                               where x.Email.ToLower() == email.ToLower()
                               select x).FirstOrDefault();
                if (account != null)
                {
                    string otpCode = Ultility.RandomString(8, false);
                    OTP obj = new OTP();
                    var dateTime = DateTime.Now;
                    var begin = dateTime;
                    var end = dateTime.AddMinutes(2);
                    obj.BeginTime = Ultility.ConvertDatetimeToUnixTimeStampMiliSecond(begin);
                    obj.EndTime = Ultility.ConvertDatetimeToUnixTimeStampMiliSecond(end);
                    obj.OTPCode = otpCode;

                    // var subjectOTP = _config["OTPSubject"];
                    var emailSend = _config["emailSend"];
                    var keySecurity = _config["keySecurity"];
                    var stringHtml = Ultility.getHtmtFile();

                    Ultility.sendEmailUploadFile(stringHtml, email, "TravelRover gửi đến quý khách hàng File báo cáo", emailSend, keySecurity);
                    return Ultility.Responses($"File Đã được gửi {email}!", Enums.TypeCRUD.Success.ToString(), obj);

                }
                else
                {
                    return Ultility.Responses($"{email} không tồn tại!", Enums.TypeCRUD.Error.ToString());
                }
            }
            catch (Exception e)
            {
                return Ultility.Responses("Có lỗi xảy ra !", Enums.TypeCRUD.Error.ToString(), description: e.Message);
            }
        }

        public Response GetsSelectBoxEmployee(long fromDate, long toDate)
        {
            try
            {
                var unixTimeOneDay = 86400000;

                var listEmployeeShouldRemove1 = (from x in _db.Schedules.AsNoTracking()
                                            where (fromDate >= x.DepartureDate && fromDate < (x.ReturnDate + unixTimeOneDay))
                                            && x.Isdelete == false
                                            orderby x.ReturnDate ascending
                                            select x.EmployeeId);

                var scheduleDepartDateLargerToDate = (from x in _db.Schedules.AsNoTracking()
                                                      where x.DepartureDate >= fromDate
                                                        && x.Isdelete == false
                                                      orderby x.DepartureDate ascending
                                                      select x);
                var listEmployeeShouldRemove2 = (from x in scheduleDepartDateLargerToDate
                                            where !(from s in listEmployeeShouldRemove1 select s).Contains(x.EmployeeId)
                                              && x.Isdelete == false
                                            && (toDate + unixTimeOneDay) > x.DepartureDate
                                            select x.EmployeeId).Distinct();

                var listShouldRemove = listEmployeeShouldRemove1.Concat(listEmployeeShouldRemove2);

                var listEmployee = (from x in _db.Employees.AsNoTracking()
                               where !listShouldRemove.Any(e => e == x.IdEmployee)
                                 && x.IsDelete == false && x.IsActive == true && x.RoleId == Convert.ToInt32(TitleRole.TourGuide)
                               select x).ToList();
                if (listEmployee.Count() == 0)
                {
                    return Ultility.Responses("Ngày bạn chọn hiện tại không có hướng dẫn viên !", Enums.TypeCRUD.Warning.ToString());
                }
                var result = Mapper.MapEmployee(listEmployee);
                return Ultility.Responses("", Enums.TypeCRUD.Success.ToString(), result);
            }
            catch (Exception e)
            {
                return Ultility.Responses("Có lỗi xảy ra !", Enums.TypeCRUD.Error.ToString(), description: e.Message);
            }
        }

        public Response GetsSelectBoxEmployeeUpdate(long fromDate, long toDate, string idSchedule)
        {
            try
            {
                var unixTimeOneDay = 86400000;
                var employeeOfSchedule = (from x in _db.Schedules.AsNoTracking()
                                     where x.IdSchedule == idSchedule
                                     && x.Isdelete == false
                                     select x).FirstOrDefault();
                var fromDateCurrentUpdate = employeeOfSchedule.DepartureDate;
                var toDateCurrentUpdate = employeeOfSchedule.ReturnDate;
                IQueryable<Guid> listEmployeeShouldRemove1;
                IQueryable<Schedule> scheduleDepartDateLargerToDate;
                if (fromDate == fromDateCurrentUpdate && toDate == toDateCurrentUpdate)
                {
                    listEmployeeShouldRemove1 = (from x in _db.Schedules.AsNoTracking()
                                            where x.EmployeeId != employeeOfSchedule.EmployeeId
                                             && x.Isdelete == false
                                            && (fromDate >= x.DepartureDate && fromDate < (x.ReturnDate + unixTimeOneDay))
                                            orderby x.ReturnDate ascending
                                            select x.CarId);
                    scheduleDepartDateLargerToDate = (from x in _db.Schedules.AsNoTracking()
                                                      where x.EmployeeId != employeeOfSchedule.EmployeeId
                                                       && x.Isdelete == false
                                                      && x.DepartureDate >= fromDate
                                                      orderby x.DepartureDate ascending
                                                      select x);
                }
                else
                {
                    if ((fromDate >= fromDateCurrentUpdate && fromDate <= toDateCurrentUpdate) || toDate >= fromDateCurrentUpdate && toDate <= toDateCurrentUpdate)
                    {
                        listEmployeeShouldRemove1 = (from x in _db.Schedules.AsNoTracking()
                                                where (fromDate >= x.DepartureDate && fromDate < (x.ReturnDate + unixTimeOneDay))
                                                 && x.Isdelete == false
                                                && x.IdSchedule != idSchedule
                                                orderby x.ReturnDate ascending
                                                select x.EmployeeId);

                        scheduleDepartDateLargerToDate = (from x in _db.Schedules.AsNoTracking()
                                                          where x.DepartureDate >= fromDate
                                                           && x.Isdelete == false
                                                                && x.IdSchedule != idSchedule
                                                          orderby x.DepartureDate ascending
                                                          select x);
                    }
                    else
                    {
                        listEmployeeShouldRemove1 = (from x in _db.Schedules.AsNoTracking()
                                                where (fromDate >= x.DepartureDate && fromDate < (x.ReturnDate + unixTimeOneDay))
                                                 && x.Isdelete == false
                                                orderby x.ReturnDate ascending
                                                select x.EmployeeId);

                        scheduleDepartDateLargerToDate = (from x in _db.Schedules.AsNoTracking()
                                                          where x.DepartureDate >= fromDate
                                                           && x.Isdelete == false
                                                          orderby x.DepartureDate ascending
                                                          select x);
                    }

                }





                var listEmployeeShouldRemove2 = (from x in scheduleDepartDateLargerToDate
                                            where !(from s in listEmployeeShouldRemove1 select s).Contains(x.EmployeeId)
                                            && (toDate + unixTimeOneDay) > x.DepartureDate
                                             && x.Isdelete == false
                                            select x.EmployeeId).Distinct();

                var listShouldRemove = listEmployeeShouldRemove1.Concat(listEmployeeShouldRemove2);


                var listEmployeeCanChoose = (from x in _db.Employees.AsNoTracking()
                                        where !listShouldRemove.Any(c => c == x.IdEmployee)
                                         && x.IsDelete == false && x.IsActive == true && x.RoleId == Convert.ToInt32(TitleRole.TourGuide)
                                             select x).ToList();
                if (listEmployeeCanChoose.Count() == 0)
                {
                    return Ultility.Responses("Ngày bạn chọn hiện tại không có hướng dẫn viên !", Enums.TypeCRUD.Warning.ToString());
                }
                var result = Mapper.MapEmployee(listEmployeeCanChoose);
                return Ultility.Responses("", Enums.TypeCRUD.Success.ToString(), result);
            }
            catch (Exception e)
            {
                return Ultility.Responses("Có lỗi xảy ra !", Enums.TypeCRUD.Error.ToString(), description: e.Message);
            }
        }
    }
}
