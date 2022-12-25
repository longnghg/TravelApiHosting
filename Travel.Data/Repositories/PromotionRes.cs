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
using Travel.Data.Interfaces.INotify;
using Travel.Shared.Ultilities;
using Travel.Shared.ViewModels;
using Travel.Shared.ViewModels.Travel;
using Travel.Shared.ViewModels.Travel.PromotionVM;
using static Travel.Shared.Ultilities.Enums;

namespace Travel.Data.Repositories
{
    public class PromotionRes : IPromotions
    {
        private readonly TravelContext _db;
        private Notification message;
        private long today = 0;
        private INotification _notification; 
        private readonly ILog _log;
    
        private Employee GetCurrentUser(Guid IdUserModify)
        {
            return (from x in _db.Employees.AsNoTracking()
                    where x.IdEmployee == IdUserModify
                    select x).FirstOrDefault();
        }
   
        public PromotionRes(TravelContext db, INotification notification, ILog log)
        {
            _db = db; _log = log;
            message = new Notification();
            today = Ultility.ConvertDatetimeToUnixTimeStampMiliSecond(DateTime.Now);
            _notification = notification;
        }
        private void CreateDatabase<T>(T input)
        {
            _db.Entry(input).State = EntityState.Added;
        }
        private void DeleteDatabaseNotSave(Promotion input)
        {
            _db.Entry(input).State = EntityState.Deleted;
        }
        private void UpdateDatabase(Promotion input)
        {
            _db.Entry(input).State = EntityState.Modified;
            _db.SaveChanges();
        }
        private void DeleteDatabase(Promotion input)
        {
            _db.Entry(input).State = EntityState.Deleted;
            _db.SaveChanges();
        }
        private void CreateDatabase(Promotion input)
        {
            _db.Entry(input).State = EntityState.Added;
            _db.SaveChanges();
        }
        public string CheckBeforSave(JObject frmData, ref Notification _message, TypeService type, bool isUpdate = false)
        {
            try
            {
                var idPromotion = PrCommon.GetString("idPromotion", frmData) ?? "0";

                 if (String.IsNullOrEmpty(idPromotion))
                {
                }
                var value = PrCommon.GetString("value", frmData);
                if (String.IsNullOrEmpty(value))
                {
                }
                var toDate = PrCommon.GetString("toDate", frmData);
                if (String.IsNullOrEmpty(toDate))
                {
                }
                var fromDate = PrCommon.GetString("fromDate", frmData);
                if (String.IsNullOrEmpty(fromDate))
                {
                }

                var typeAction = PrCommon.GetString("typeAction", frmData);
                if (String.IsNullOrEmpty(typeAction))
                {
                }
                var idUserModify = PrCommon.GetString("IdUserModify", frmData);
                if (String.IsNullOrEmpty(idUserModify))
                {
                }

                if (isUpdate)
                {
                    UpdatePromotionViewModel uPromotionObj = new UpdatePromotionViewModel();
                    uPromotionObj.IdPromotion = int.Parse(idPromotion);
                    uPromotionObj.Value = int.Parse(value);
                    uPromotionObj.ToDate = long.Parse(toDate);
                    uPromotionObj.FromDate = long.Parse(fromDate);
                    uPromotionObj.IdUserModify = Guid.Parse(idUserModify);
                    uPromotionObj.TypeAction = "update";
                    return JsonSerializer.Serialize(uPromotionObj);
                }
                else
                {
                    CreatePromotionViewModel PromotionObj = new CreatePromotionViewModel();
                    PromotionObj.Value = int.Parse(value);
                    PromotionObj.ToDate = long.Parse(toDate);
                    PromotionObj.FromDate = long.Parse(fromDate);
                    PromotionObj.IdUserModify = Guid.Parse(idUserModify);
                    return JsonSerializer.Serialize(PromotionObj);
                }

            }
            catch (Exception e)
            {
                _message = Ultility.Responses("Có lỗi xảy ra !", Enums.TypeCRUD.Error.ToString(), description: e.Message).Notification;
                return string.Empty;
            }
        }

        public Response GetsPromotion(bool isDelete )
        {
            try
            {
                var queryListPromotion = (from x in _db.Promotions.AsNoTracking()
                            where
                            x.IsDelete == isDelete &&
                            x.IsTempdata == false &&
                            x.Approve == Convert.ToInt16(Enums.ApproveStatus.Approved)
                            select x);
                int totalResult = queryListPromotion.Count();
                var list = queryListPromotion.ToList();
                var result = Mapper.MapPromotion(list);

                var res = Ultility.Responses("", Enums.TypeCRUD.Success.ToString(), result);
                res.TotalResult = totalResult;
                return res;
            }
            catch (Exception e)
            {
                return Ultility.Responses("Có lỗi xảy ra !", Enums.TypeCRUD.Error.ToString(), description: e.Message);
            }
        }

        public Response GetsPromotionExists()
        {
            try
            {
                var list = (from x in _db.Promotions.AsNoTracking()
                            where
                            x.Value == 0 ||
                            (x.ToDate > today &&
                            x.IsDelete == false &&
                            x.IsTempdata == false &&
                            x.Approve == Convert.ToInt16(Enums.ApproveStatus.Approved))
                            select x).ToList();

                var result = Mapper.MapPromotion(list);
                return Ultility.Responses("", Enums.TypeCRUD.Success.ToString(), result);
            }
            catch (Exception e)
            {
                return Ultility.Responses("Có lỗi xảy ra !", Enums.TypeCRUD.Error.ToString(), description: e.Message);
            }
        }

        public Response GetsWaitingPromotion(Guid idUser, int pageIndex, int pageSize)
        {
            try
            {
                var totalResult = 0;
                var userLogin = (from x in _db.Employees.AsNoTracking()
                                 where x.IdEmployee == idUser
                                 select x).FirstOrDefault();
                var listWaiting = new List<Promotion>();
                if (userLogin.RoleId == (int)Enums.TitleRole.Admin)
                {
                    var querylistWaiting = (from x in _db.Promotions.AsNoTracking()
                                   where x.Approve == Convert.ToInt16(ApproveStatus.Waiting) select x).ToList();
                    totalResult = querylistWaiting.Count();
                    listWaiting = querylistWaiting.Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();
                }
                else
                {
                    var querylistWaiting = (from x in _db.Promotions.AsNoTracking()
                                   where x.IdUserModify == idUser
                                   && x.Approve == Convert.ToInt16(ApproveStatus.Waiting)
                                   select x).ToList();
                    totalResult = querylistWaiting.Count();
                    listWaiting = querylistWaiting.Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();
                }

                var result = Mapper.MapPromotion(listWaiting);
                var res =  Ultility.Responses("", Enums.TypeCRUD.Success.ToString(), result);
                res.TotalResult = totalResult;
                return res;
            }
            catch (Exception e)
            {
                return Ultility.Responses("Có lỗi xảy ra !", Enums.TypeCRUD.Error.ToString(), description: e.Message);
            }
        } 
         public Response CreatePromotion(CreatePromotionViewModel input, string emailUser)
        {
            Promotion promotion
                        = Mapper.MapCreatePromotion(input);
            var user = GetCurrentUser(input.IdUserModify);
            promotion.ModifyBy = user.NameEmployee;
            //promotion.IdUserModify = user.IdEmployee;
            promotion.TypeAction = "insert";
            string jsonContent = JsonSerializer.Serialize(promotion);
            promotion.ModifyDate = Ultility.ConvertDatetimeToUnixTimeStampMiliSecond(DateTime.Now);
            CreateDatabase(promotion);

            var listRole = new int[] { Convert.ToInt16(Enums.TitleRole.Admin), Convert.ToInt16(Enums.TitleRole.LocalManager) };
            _notification.CreateNotification(user.IdEmployee, Convert.ToInt16(Enums.TypeNotification.Promotion), promotion.Value.ToString(), listRole, "");
       
            bool result = _log.AddLog(content: jsonContent, type: "create", emailCreator: emailUser, classContent: "Promotion");
            if (result)
            {
                return Ultility.Responses("Thêm thành công !", Enums.TypeCRUD.Success.ToString());
            }
            else
            {
                return Ultility.Responses("Lỗi log!", Enums.TypeCRUD.Error.ToString());
            }
        }

        public Response DeletePromotion(int id, Guid idUser,string emailUser)
        {
            try
            {
                var promotion = (from x in _db.Promotions.AsNoTracking()
                                 where x.IdPromotion == id
                             select x).FirstOrDefault();

                var userLogin = (from x in _db.Employees.AsNoTracking()
                                 where x.IdEmployee == idUser
                                 select x).FirstOrDefault();
                string jsonContent = JsonSerializer.Serialize(promotion);

                if (promotion.Approve == (int)ApproveStatus.Approved)
                {
                    promotion.ModifyBy = userLogin.NameEmployee;
                    promotion.TypeAction = "delete";
                    promotion.IdUserModify = userLogin.IdEmployee;
                    promotion.ModifyDate = Ultility.ConvertDatetimeToUnixTimeStampMiliSecond(DateTime.Now);
                    promotion.Approve = (int)ApproveStatus.Waiting;
                    promotion.IsDelete = true;
                    UpdateDatabase(promotion);

                    var listRole = new int[] { Convert.ToInt16(Enums.TitleRole.Admin), Convert.ToInt16(Enums.TitleRole.LocalManager) };
                    _notification.CreateNotification(userLogin.IdEmployee, Convert.ToInt16(Enums.TypeNotification.Promotion), promotion.Value.ToString(), listRole, "");
                  
                    bool result = _log.AddLog(content: jsonContent, type: "detele", emailCreator: emailUser, classContent: "Promotion");
                    if (result)
                    {
                        return Ultility.Responses("Đã gửi yêu cầu xóa !", Enums.TypeCRUD.Success.ToString());

                    }
                    else
                    {
                        return Ultility.Responses("Lỗi log!", Enums.TypeCRUD.Error.ToString());
                    }
                }
                else
                {
                    if (promotion.IdUserModify == idUser)
                    {
                        if (promotion.TypeAction == "insert")
                        {
                            DeleteDatabase(promotion);

                            return Ultility.Responses("Đã xóa!", Enums.TypeCRUD.Success.ToString());
                        }
                        else if (promotion.TypeAction == "update")
                        {
                            var idPromotionTemp = promotion.IdAction;
                            // old hotel
                            var promotionTemp = (from x in _db.Promotions.AsNoTracking()
                                                 where x.IdPromotion == int.Parse(idPromotionTemp)
                                             select x).FirstOrDefault();
                            promotion.Approve = (int)ApproveStatus.Approved;
                            promotion.IdAction = null;
                            promotion.TypeAction = null;
                            #region restore old data

                            promotion.Approve = (int)ApproveStatus.Approved;
                            promotion.Value = promotionTemp.Value;

                            promotion.ToDate = promotionTemp.ToDate;
                            promotion.FromDate = promotionTemp.FromDate;
                            #endregion

                            DeleteDatabase(promotionTemp);

                            return Ultility.Responses("Đã hủy yêu cầu chỉnh sửa !", Enums.TypeCRUD.Success.ToString());
                        }
                        else if (promotion.TypeAction == "restore")
                        {
                            promotion.IdAction = null;
                            promotion.TypeAction = null;
                            promotion.IsDelete = true;
                            promotion.Approve = (int)ApproveStatus.Approved;

                                UpdateDatabase(promotion);

                            return Ultility.Responses("Đã hủy yêu cầu khôi phục!", Enums.TypeCRUD.Success.ToString());

                        }
                        else // delete
                        {
                            promotion.IdAction = null;
                            promotion.TypeAction = null;
                            promotion.IsDelete = false;
                            promotion.Approve = (int)ApproveStatus.Approved;
                            UpdateDatabase(promotion);
                            return Ultility.Responses("Đã hủy yêu cầu xóa !", Enums.TypeCRUD.Success.ToString());
                        }
                    }
                    else
                    {
                        return Ultility.Responses("Bạn không thể thực thi hành động này !", Enums.TypeCRUD.Info.ToString());
                    }
                }
            }
            catch (Exception e)
            {
                return Ultility.Responses("Có lỗi xảy ra !", Enums.TypeCRUD.Error.ToString(), description: e.Message);
            }
        }

        public Response UpdatePromotion(UpdatePromotionViewModel input,string emailUser)
        {
            try
            {
                var userLogin = (from x in _db.Employees.AsNoTracking()
                                 where x.IdEmployee == input.IdUserModify
                                 select x).FirstOrDefault();

                var promotion = (from x in _db.Promotions.AsNoTracking()
                                 where x.IdPromotion == input.IdPromotion
                             select x).FirstOrDefault();

                // clone new object
                var promotionOld = new Promotion();
                promotionOld = Ultility.DeepCopy<Promotion>(promotion);
                promotionOld.IdAction = promotionOld.IdPromotion.ToString();
                promotionOld.IsTempdata = true;

                CreateDatabase<Promotion>(promotion);
                #region setdata
                promotion.IdAction = promotionOld.IdPromotion.ToString();
                promotion.IdUserModify = input.IdUserModify;
                promotion.TypeAction = input.TypeAction;
                promotion.Approve = (int)ApproveStatus.Waiting;
                promotion.ModifyBy = userLogin.NameEmployee;
                promotion.ModifyDate = Ultility.ConvertDatetimeToUnixTimeStampMiliSecond(DateTime.Now);


                promotion.Value = input.Value;
                promotion.ToDate = input.ToDate;
                promotion.FromDate = input.FromDate;
                #endregion
                string jsonContent = JsonSerializer.Serialize(promotion);
                UpdateDatabase(promotion);

                var listRole = new int[] { Convert.ToInt16(Enums.TitleRole.Admin), Convert.ToInt16(Enums.TitleRole.LocalManager) };
                _notification.CreateNotification(userLogin.IdEmployee, Convert.ToInt16(Enums.TypeNotification.Promotion), promotion.Value.ToString(), listRole, "");

                bool result = _log.AddLog(content: jsonContent, type: "update", emailCreator: emailUser, classContent: "Promotion");
                if (result)
                {
                    return Ultility.Responses("Đã sửa !", Enums.TypeCRUD.Success.ToString());

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

        public Response ApprovePromotion(int id)
        {
            try
            {
                var promotion = (from x in _db.Promotions.AsNoTracking()
                             where x.IdPromotion == id
                             && x.Approve == (int)ApproveStatus.Waiting
                             select x).FirstOrDefault();
                if (promotion != null)
                { 
                    if (promotion.TypeAction == "update")
                    {
                        var idPromotionTemp = promotion.IdAction;
                        promotion.Approve = (int)ApproveStatus.Approved;
                        promotion.IdAction = null;
                        promotion.TypeAction = null;
                        // delete tempdata
                        var promotionTemp = (from x in _db.Promotions.AsNoTracking()
                                         where x.IdPromotion == int.Parse(idPromotionTemp)
                                         select x).FirstOrDefault();
                        DeleteDatabaseNotSave(promotionTemp);
                    }
                    else if (promotion.TypeAction == "insert")
                    {
                        promotion.IdAction = null;
                        promotion.TypeAction = null;
                        promotion.Approve = (int)ApproveStatus.Approved;
                    }
                    else if (promotion.TypeAction == "restore")
                    {
                        promotion.IdAction = null;
                        promotion.TypeAction = null;
                        promotion.Approve = (int)ApproveStatus.Approved;
                        promotion.IsDelete = false;
                    }
                    else
                    {
                        promotion.IdAction = null;
                        promotion.TypeAction = null;
                        promotion.Approve = (int)ApproveStatus.Approved;
                        promotion.IsDelete = true;                                       
                    }
                    UpdateDatabase(promotion);

                    var userModify = GetCurrentUser(promotion.IdUserModify);
                    _notification.CreateNotification(userModify.IdEmployee, Convert.ToInt16(Enums.TypeNotification.Promotion), promotion.Value.ToString(), new int[] { userModify.RoleId }, "Thành công");
                    return Ultility.Responses("Duyệt thành công !", Enums.TypeCRUD.Success.ToString());
                }
                else
                {
                    return Ultility.Responses("Không tim thấy dữ liệu !", Enums.TypeCRUD.Warning.ToString());
                }

            }
            catch (Exception e)
            {
                return Ultility.Responses("Có lỗi xảy ra !", Enums.TypeCRUD.Error.ToString(), description: e.Message);
            }
        }

        public Response RefusedPromotion(int id)
        {
            try
            {
                var promotion = (from x in _db.Promotions.AsNoTracking()
                                  where x.IdPromotion == id
                                  && x.Approve == (int)ApproveStatus.Waiting
                                  select x).FirstOrDefault();
                if (promotion != null)
                {
                    if (promotion.TypeAction == "update")
                    {
                        var idPromotionTemp = promotion.IdAction;
                        // old hotel
                        var promotionTemp = (from x in _db.Promotions.AsNoTracking()
                                             where x.IdPromotion == int.Parse(idPromotionTemp)
                                              select x).FirstOrDefault();
                        promotion.Approve = (int)ApproveStatus.Approved;
                        promotion.IdAction = null;
                        promotion.TypeAction = null;
                        #region restore old data

                        promotion.Approve = (int)ApproveStatus.Approved;
                        promotion.Value = promotionTemp.Value;

                        promotion.ToDate = promotionTemp.ToDate;
                        promotion.FromDate = promotionTemp.FromDate;
                        #endregion
                        DeleteDatabaseNotSave(promotionTemp);
                       }
                    else if (promotion.TypeAction == "insert")
                    {
                        promotion.IdAction = null;
                        promotion.TypeAction = null;
                        promotion.Approve = (int)ApproveStatus.Refused;
                    }
                    else if (promotion.TypeAction == "restore")
                    {
                        promotion.IdAction = null;
                        promotion.TypeAction = null;
                        promotion.IsDelete = true;
                        promotion.Approve = (int)ApproveStatus.Approved;
                    }
                    else // delete
                    {
                        promotion.IdAction = null;
                        promotion.TypeAction = null;
                        promotion.IsDelete = false;
                        promotion.Approve = (int)ApproveStatus.Approved;
                    }
                    UpdateDatabase(promotion);

                    var userModify = GetCurrentUser(promotion.IdUserModify);
                    _notification.CreateNotification(userModify.IdEmployee, Convert.ToInt16(Enums.TypeNotification.Promotion), promotion.Value.ToString(), new int[] { userModify.RoleId }, "Từ chối");

                    return Ultility.Responses("Từ chối thành công !", Enums.TypeCRUD.Success.ToString());
                }
                else
                {
                    return Ultility.Responses("Không tim thấy dữ liệu !", Enums.TypeCRUD.Warning.ToString());
                }
            }
            catch (Exception e)
            {
                return Ultility.Responses("Có lỗi xảy ra !", Enums.TypeCRUD.Error.ToString(), description: e.Message);
            }
        }

        public Response RestorePromotion(int id, Guid idUser ,string emailUser)
        {
            try
            {
                var promotion = (from x in _db.Promotions.AsNoTracking()
                             where x.IdPromotion == id
                             select x).FirstOrDefault();

                var userLogin = (from x in _db.Employees.AsNoTracking()
                                 where x.IdEmployee == idUser
                                 select x).FirstOrDefault();
                string jsonContent = JsonSerializer.Serialize(promotion);
                if (promotion.Approve == (int)ApproveStatus.Approved)
                {
                    promotion.ModifyBy = userLogin.NameEmployee;
                    promotion.TypeAction = "restore";
                    promotion.IdUserModify = userLogin.IdEmployee;
                    promotion.ModifyDate = Ultility.ConvertDatetimeToUnixTimeStampMiliSecond(DateTime.Now);
                    promotion.Approve = (int)ApproveStatus.Waiting;
                    // bổ sung isdelete
                    promotion.IsDelete = false;
                }
                UpdateDatabase(promotion);

                var listRole = new int[] { Convert.ToInt16(Enums.TitleRole.Admin), Convert.ToInt16(Enums.TitleRole.LocalManager) };
                _notification.CreateNotification(userLogin.IdEmployee, Convert.ToInt16(Enums.TypeNotification.Promotion), promotion.Value.ToString(), listRole, "");
                bool result = _log.AddLog(content: jsonContent, type: "resore", emailCreator: emailUser, classContent: "Promotion");
                if (result)
                {
                    return Ultility.Responses("Đã gửi yêu cầu khôi phục !", Enums.TypeCRUD.Success.ToString());


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

        public Response StatisticPromotion()
        {
            try
            {

                var dateTimeNow = DateTime.Now;
                var month = dateTimeNow.Month;
                var year = dateTimeNow.Year;
                var day = 1;
                var firstDayOfMonth = DateTime.Parse($"{year}/{month}/{day}");
                var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);
                var firstDay = Ultility.ConvertDatetimeToUnixTimeStampMiliSecond((firstDayOfMonth));
                var lastDay = Ultility.ConvertDatetimeToUnixTimeStampMiliSecond((lastDayOfMonth));
                var timeNow = Ultility.ConvertDatetimeToUnixTimeStampMiliSecond((dateTimeNow));
                var promotionOfMonth = (from x in _db.Promotions.AsNoTracking()
                               where x.IsTempdata == false && x.IsDelete == false &&
                               x.FromDate >= firstDay && x.FromDate <= lastDay
                               select x).Count();

                var promotion = (from x in _db.Promotions.AsNoTracking()
                                        where x.IsTempdata == false && x.IsDelete == false                                     
                                        select x).Count(); 
                // còn hiệu lực
                var promotionOfTime = (from x in _db.Promotions.AsNoTracking()
                                 where x.IsTempdata == false && x.IsDelete == false &&
                                  x.FromDate >= timeNow && x.ToDate >= timeNow
                                       select x).Count();
                var unPromotionOfTime = promotionOfMonth - promotionOfTime;
                var ab = String.Format("promotion: {0} && promotionOfMonth: {1} && promotionOfTime: {2} && unPromotionOfTime: {3}", promotion, promotionOfMonth , promotionOfTime , unPromotionOfTime);
                return Ultility.Responses("", Enums.TypeCRUD.Success.ToString(), ab);

            }
            catch (Exception e)
            {
                return Ultility.Responses("Có lỗi xảy ra !", Enums.TypeCRUD.Error.ToString(), description: e.Message);
            }



        }
   
        public Response SearchPromotion(JObject frmData)
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

                var kwValue = PrCommon.GetString("value", frmData);
                if (!String.IsNullOrEmpty(kwValue))
                {
                    keywords.KwValue = int.Parse(kwValue);
                }
                else
                {
                    keywords.KwValue = 0;
                }

                var kwFromDate = PrCommon.GetString("fromDate", frmData);
                if (!String.IsNullOrEmpty(kwFromDate))
                {
                    keywords.KwFromDate = Ultility.ConvertDatetimeToUnixTimeStampMiliSecond(DateTime.Parse(kwFromDate));
                }
                else
                {
                    keywords.KwFromDate = 0;
                }

                var kwToDate = PrCommon.GetString("toDate", frmData);
                if (!String.IsNullOrEmpty(kwToDate))
                {
                    keywords.KwToDate = Ultility.ConvertDatetimeToUnixTimeStampMiliSecond(DateTime.Parse(kwToDate).AddDays(1).AddSeconds(-1));
                }
                else
                {
                    keywords.KwToDate = 0;
                }

                var listPromotion = new List<Promotion>();

                if(keywords.KwFromDate > 0 || keywords.KwToDate > 0)
                {
                    if(keywords.KwValue > 0)
                    {
                        if (keywords.KwFromDate > 0 && keywords.KwToDate > 0)
                        {
                            var querylistPromo = (from p in _db.Promotions
                                             where p.IsDelete == keywords.IsDelete &&
                                                   p.FromDate >= keywords.KwFromDate &&
                                                   p.ToDate <= keywords.KwToDate &&
                                                   p.Value == keywords.KwValue
                                             select p).ToList();
                            totalResult = querylistPromo.Count();
                            listPromotion = querylistPromo.Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();
                        }
                        else if (keywords.KwFromDate == 0 && keywords.KwToDate > 0)
                        {
                            var querylistPromo = (from p in _db.Promotions
                                             where p.IsDelete == keywords.IsDelete &&
                                                   p.ToDate <= keywords.KwToDate &&
                                                   p.Value == keywords.KwValue
                                             select p).ToList();
                            totalResult = querylistPromo.Count();
                            listPromotion = querylistPromo.Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();
                        }
                        else if (keywords.KwFromDate > 0 && keywords.KwToDate == 0)
                        {
                            var querylistPromo = (from p in _db.Promotions
                                             where p.IsDelete == keywords.IsDelete &&
                                                   p.FromDate >= keywords.KwFromDate &&
                                                   p.Value == keywords.KwValue
                                             select p).ToList();
                            totalResult = querylistPromo.Count();
                            listPromotion = querylistPromo.Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();
                        }
                    }
                    else
                    {
                        if (keywords.KwFromDate > 0 && keywords.KwToDate > 0)
                        {
                            var querylistPromo = (from p in _db.Promotions
                                             where p.IsDelete == keywords.IsDelete &&
                                                   p.FromDate >= keywords.KwFromDate &&
                                                   p.ToDate <= keywords.KwToDate 
                                                  select p).ToList();
                            totalResult = querylistPromo.Count();
                            listPromotion = querylistPromo.Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();
                        }
                        else if (keywords.KwFromDate == 0 && keywords.KwToDate > 0)
                        {
                            var querylistPromo = (from p in _db.Promotions
                                             where   p.IsDelete == keywords.IsDelete &&
                                                     p.ToDate <= keywords.KwToDate 
                                             select p).ToList();
                            totalResult = querylistPromo.Count();
                            listPromotion = querylistPromo.Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();
                        }
                        else if (keywords.KwFromDate > 0 && keywords.KwToDate == 0)
                        {
                            var querylistPromo = (from p in _db.Promotions
                                             where  p.IsDelete == keywords.IsDelete &&
                                                    p.FromDate >= keywords.KwFromDate 
                                                  select p).ToList();
                            totalResult = querylistPromo.Count();
                            listPromotion = querylistPromo.Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();
                        }
                    }
                }
                else
                {
                    if(keywords.KwValue > 0)
                    {
                        var querylistPromo = (from p in _db.Promotions
                                         where p.IsDelete == keywords.IsDelete &&
                                                p.Value == keywords.KwValue 
                                              select p).ToList();
                        totalResult = querylistPromo.Count();
                        listPromotion = querylistPromo.Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();
                    }
                    else
                    {
                        var querylistPromo = (from p in _db.Promotions
                                              where p.IsDelete == keywords.IsDelete 
                                         select p).ToList();
                        totalResult = querylistPromo.Count();
                        listPromotion = querylistPromo.Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();
                    }
                    
                }


                var result = Mapper.MapPromotion(listPromotion);
                if (result.Count() > 0)
                {
                    var res = Ultility.Responses("", Enums.TypeCRUD.Success.ToString(), result);
                    res.TotalResult = totalResult;
                    return res;
                }
                else
                {
                    return Ultility.Responses($"Không có dữ liệu trả về !", Enums.TypeCRUD.Warning.ToString());
                }
            }
            catch(Exception e)
            {
                return Ultility.Responses("Có lỗi xảy ra !", Enums.TypeCRUD.Error.ToString(), description: e.Message);
            }
        }

        public Response SelectBoxPromotions(long fromDate, long toDate)
        {
            try
            {

                var query = (from x in _db.Promotions.AsNoTracking()
                             where x.IdPromotion == 1
                                   
                             select x);



                if (fromDate == 0 && toDate == 0)
                {
                    return Ultility.Responses("", Enums.TypeCRUD.Success.ToString(), query.ToList());
                }
                var query1 = (from x in _db.Promotions.AsNoTracking()
                         where
                         x.IsDelete == false &&
                         x.IdPromotion != -2 &&
                         x.IsTempdata == false &&
                          x.FromDate >= fromDate &&
                          x.ToDate <= toDate &&
                          x.IdPromotion != 1
                         select x);

                var queryConcat = query1.Concat(query);

                return Ultility.Responses("", Enums.TypeCRUD.Success.ToString(), queryConcat.ToList());


                //var queryvalue0 = (from x in queryListPromotion
                //                   where x.Value == 0
                //                   select x);

                //if (fromDate > 0 && toDate > 0)
                //{
                //    queryListPromotion = (from x in queryListPromotion
                //                              where
                //                              x.FromDate >= fromDate &&
                //                              x.ToDate <= toDate
                //                              select x);
                //    totalResult = queryListPromotion.Count();
                //}


                //var res = Ultility.Responses("", Enums.TypeCRUD.Success.ToString());
                //if (fromDate > 0 && toDate > 0)
                //{
                //   var concatList = queryListPromotion.Concat(queryvalue0);
                //   res.Content = concatList;
                //}
                //else
                //{
                //    res.Content = queryvalue0;
                //}

                //res.TotalResult = totalResult;
            }
            catch (Exception e)
            {
                return Ultility.Responses("Có lỗi xảy ra !", Enums.TypeCRUD.Error.ToString(), description: e.Message);
            }
        }
    }
}
