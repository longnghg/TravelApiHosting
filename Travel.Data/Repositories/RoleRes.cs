using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using PrUtility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Travel.Context.Models;
using Travel.Context.Models.Travel;
using Travel.Data.Interfaces;
using Travel.Shared.Ultilities;
using Travel.Shared.ViewModels;
using Travel.Shared.ViewModels.Travel;

namespace Travel.Data.Repositories
{
    public class RoleRes : IRole
    {
        private readonly TravelContext _db;
        private Notification message;
        private Response res;
        private readonly ILog _log;
        public RoleRes(TravelContext db, ILog log)
        {
            _db = db;
            _log = log;
            message = new Notification();
            res = new Response();
        }

        public string CheckBeforSave(JObject frmData, ref Notification _message, bool isUpdate)
        {
            try
            {
                var idRole = PrCommon.GetString("idRole", frmData);
                if (String.IsNullOrEmpty(idRole))
                {
                }
                var nameRole = PrCommon.GetString("nameRole", frmData);
              

                var description = PrCommon.GetString("description", frmData);
               
                #region validate
                if (isUpdate)
                {
                    if (!String.IsNullOrEmpty(nameRole))
                    {
                        var check = CheckSameRole(nameRole, "nameRole",idRole);
                        if (check.Notification.Type == Enums.TypeCRUD.Validation.ToString() || check.Notification.Type == Enums.TypeCRUD.Error.ToString())
                        {
                            _message = check.Notification;
                            return string.Empty;
                        }
                    }
                    if (!String.IsNullOrEmpty(description))
                    {
                        var check = CheckSameRole(description, "description" , idRole);
                        if (check.Notification.Type == Enums.TypeCRUD.Validation.ToString() || check.Notification.Type == Enums.TypeCRUD.Error.ToString())
                        {
                            _message = check.Notification;
                            return string.Empty;
                        }
                    }
                }
                else
                {
                    if (!String.IsNullOrEmpty(nameRole))
                    {
                        var check = CheckSameRole(nameRole, "nameRole");
                        if (check.Notification.Type == Enums.TypeCRUD.Validation.ToString() || check.Notification.Type == Enums.TypeCRUD.Error.ToString())
                        {
                            _message = check.Notification;
                            return string.Empty;
                        }
                    }
                    if (!String.IsNullOrEmpty(description))
                    {
                        var check = CheckSameRole(description, "description");
                        if (check.Notification.Type == Enums.TypeCRUD.Validation.ToString() || check.Notification.Type == Enums.TypeCRUD.Error.ToString())
                        {
                            _message = check.Notification;
                            return string.Empty;
                        }
                    }
                }
                #endregion
                if (isUpdate)
                {
                    UpdateRoleViewModel objUpdate = new UpdateRoleViewModel();
                    objUpdate.IdRole = int.Parse(idRole);
                    objUpdate.NameRole = nameRole;
                    objUpdate.Description = description;
                    return JsonSerializer.Serialize(objUpdate);
                }
                CreateRoleViewModel objCreate = new CreateRoleViewModel();
                //objCreate.IdRole = int.Parse(idRole);
                objCreate.NameRole = nameRole;
                objCreate.Description = description;
                return JsonSerializer.Serialize(objCreate);
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

        public Response GetsRole(bool isDelete)
        {
            try
            {
                var listRole = (from x in _db.Roles.AsNoTracking()
                                where x.IsDelete == isDelete
                                select x).ToList();
                var result = Mapper.MapRole(listRole);
                return Ultility.Responses("", Enums.TypeCRUD.Success.ToString(), result);

            }
            catch (Exception e)
            {
                return Ultility.Responses("Có lỗi xảy ra !", Enums.TypeCRUD.Error.ToString(), description: e.Message);

            }
        }

        public Response CreateRole(CreateRoleViewModel input, string emailUser)
        {
            try
            {
                Role role = new Role();
                role = Mapper.MapCreateRole(input);
                string jsonContent = JsonSerializer.Serialize(role);
                _db.Roles.Add(role);
                _db.SaveChanges();

                bool result = _log.AddLog(content: jsonContent, type: "create", emailCreator: emailUser, classContent: "Role");
                if (result)
                {
                    return Ultility.Responses("Thêm thành công !", Enums.TypeCRUD.Success.ToString());
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
        public Response UpdateRole(UpdateRoleViewModel input, string emailUser)
        {
            try
            {
                if (input.NameRole.ToLower() == "admin")
                {
                    return Ultility.Responses("Không thể phân quyền Admin !", Enums.TypeCRUD.Validation.ToString());
                }
                Role role = new Role();
                role = Mapper.MapUpdateRole(input);
                string jsonContent = JsonSerializer.Serialize(role);

                _db.Roles.Update(role);
                _db.SaveChanges();

                bool result = _log.AddLog(content: jsonContent, type: "update", emailCreator: emailUser, classContent: "Role");
                if (result)
                {
                    return Ultility.Responses("Sửa thành công !", Enums.TypeCRUD.Success.ToString());
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




        public Response RestoreRole(int idRole, string emailUser)
        {
            try
            {
                var role = _db.Roles.Find(idRole);
                if (role != null)
                {
                    string jsonContent = JsonSerializer.Serialize(role);

                    role.IsDelete = false;
                    _db.SaveChanges();

                    bool result = _log.AddLog(content: jsonContent, type: "restore", emailCreator: emailUser, classContent: "Role");
                    if (result)
                    {
                        return Ultility.Responses("Khôi phục thành công !", Enums.TypeCRUD.Success.ToString());

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

        public Response DeleteRole(int idRole, string emailUser)
        {
            try
            {
                if (CheckIsAdmin(idRole))
                {
                    return Ultility.Responses("Không thể xóa Admin !", Enums.TypeCRUD.Validation.ToString());
                }
                var role = _db.Roles.Find(idRole);
                if (role != null)
                {
                    string jsonContent = JsonSerializer.Serialize(role);

                    role.IsDelete = true;
                    _db.SaveChanges();

                    bool result = _log.AddLog(content: jsonContent, type: "delete", emailCreator: emailUser, classContent: "Role");
                    if (result)
                    {
                        return Ultility.Responses("Xóa thành công !", Enums.TypeCRUD.Success.ToString());

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

        public Response SearchRole(JObject frmData)
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

                var idRole = PrCommon.GetString("idRole", frmData);
                if (!String.IsNullOrEmpty(isDelete))
                {
                    keywords.KwId = idRole;
                }

                var kwName = PrCommon.GetString("nameRole", frmData).Trim();
                if (!String.IsNullOrEmpty(kwName))
                {
                    keywords.KwName = kwName.Trim().ToLower();
                }
                else
                {
                    keywords.KwName = "";

                }

                var kwDescription = PrCommon.GetString("description", frmData).Trim();
                if (!String.IsNullOrEmpty(kwDescription))
                {
                    keywords.KwDescription = kwDescription.Trim().ToLower();
                }
                else
                {
                    keywords.KwDescription = "";

                }

                var kwIdRole = PrCommon.GetString("idRole", frmData);
                keywords.KwIdRole = PrCommon.getListInt(kwIdRole, ',', false);



                var listRole = new List<Role>();
                if (keywords.KwIdRole.Count > 0)
                {
                    var queryListRole = (from x in _db.Roles.AsNoTracking()
                                         where x.IsDelete == keywords.IsDelete &&
                                                         x.NameRole.ToLower().Contains(keywords.KwName) &&
                                                         x.Description.ToLower().Contains(keywords.KwDescription)
                                         select x);
                    totalResult = queryListRole.Count();
                    listRole = queryListRole.Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();
                }
                else
                {
                    var queryListRole = (from x in _db.Roles.AsNoTracking()
                                         where x.IsDelete == keywords.IsDelete &&
                                                        x.NameRole.ToLower().Contains(keywords.KwName) &&
                                                        x.Description.ToLower().Contains(keywords.KwDescription)
                                         select x);
                    totalResult = queryListRole.Count();
                    listRole = queryListRole.Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();
                }
                var result = Mapper.MapRole(listRole);
                if (listRole.Count() > 0)
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


        private bool CheckIsAdmin(int idRole)
        {
            var currentRole = (from x in _db.Roles.AsNoTracking()
                               where x.IdRole == idRole
                               select x).FirstOrDefault();
            bool isAdmin = currentRole.NameRole.ToLower() == "admin";
            if (isAdmin)
            {
                return true;
            }
            return false;
        }
        private Response CheckSameRole(string input, string description, string idRole = null)
        {
            try
            {
                string oriRoleInput = Ultility.removeVietnameseSign(input.ToLower().Replace(" ", ""));


                if (!string.IsNullOrEmpty(idRole)) // update
                {
                    int id = int.Parse(idRole);
                    var oldRole = (from x in _db.Roles.AsNoTracking()
                                   where x.IdRole == id
                                   select x).FirstOrDefault();
                    string oldRoleName = oldRole.NameRole;
                    string oldRoleDescription = oldRole.Description;

                    if (oldRoleName != input || oldRoleDescription !=input) // có thay đổi  
                    {
                        var obj = _db.Roles.Where(delegate (Role role)
                        {
                            if (Ultility.removeVietnameseSign(role.NameRole.ToLower().Replace(" ", "")).Contains(oriRoleInput) && role.NameRole != oldRoleName)
                            {
                                return true;
                            }
                            if (Ultility.removeVietnameseSign(role.Description.ToLower().Replace(" ", "")).Contains(oriRoleInput) && role.Description != oldRoleDescription)
                            {
                                return true;
                            }
                            return false;
                        });
                        if (obj.FirstOrDefault() != null)
                        {
                            return Ultility.Responses("[" + input + "] này đã tồn tại !", Enums.TypeCRUD.Validation.ToString(), description: description);
                        }
                    }
                }
                else // create
                {
                    var obj = _db.Roles.Where(delegate (Role role)
                    {
                        if (Ultility.removeVietnameseSign(role.NameRole.ToLower().Replace(" ", "")).Contains(oriRoleInput))
                        {
                            return true;
                        }
                        if (Ultility.removeVietnameseSign(role.Description.ToLower().Replace(" ", "")).Contains(oriRoleInput))
                        {
                            return true;
                        }
                        return false;
                    });
                    if (obj.FirstOrDefault() != null)
                    {
                        return Ultility.Responses("[" + input + "] này đã tồn tại !", Enums.TypeCRUD.Validation.ToString(), description: description);
                    }
                }
                return res;




            }
            catch (Exception e)
            {
                return Ultility.Responses("Có lỗi xảy ra !", Enums.TypeCRUD.Error.ToString(), description: e.Message);
            }
        }
    }
}
