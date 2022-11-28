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
        private Employee GetCurrentUser(Guid IdUserModify)
        {
            return (from x in _db.Employees.AsNoTracking()
                    where x.IdEmployee == IdUserModify
                    select x).FirstOrDefault();
        }
   
        public PromotionRes(TravelContext db, INotification notification)
        {
            _db = db;
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
                var idUserModify = PrCommon.GetString("idUserModify", frmData);
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

        public Response GetsPromotion(bool isDelete)
        {
            try
            {
                var list = (from x in _db.Promotions.AsNoTracking()
                            where
                            x.IsDelete == isDelete &&
                            x.IsTempdata == false &&
                            x.Approve == Convert.ToInt16(Enums.ApproveStatus.Approved)

                            select x).ToList();
                var result = Mapper.MapPromotion(list);
                return Ultility.Responses("", Enums.TypeCRUD.Success.ToString(), result);
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

        public Response GetsWaitingPromotion(Guid idUser)
        {
            try
            {
                var userLogin = (from x in _db.Employees.AsNoTracking()
                                 where x.IdEmployee == idUser
                                 select x).FirstOrDefault();
                var listWaiting = new List<Promotion>();
                if (userLogin.RoleId == (int)Enums.TitleRole.Admin)
                {
                    listWaiting = (from x in _db.Promotions.AsNoTracking()
                                   where x.Approve == Convert.ToInt16(ApproveStatus.Waiting) select x).ToList();
                }
                else
                {
                    listWaiting = (from x in _db.Promotions.AsNoTracking()
                                   where x.IdUserModify == idUser
                                   && x.Approve == Convert.ToInt16(ApproveStatus.Waiting)
                                   select x).ToList();
                }

                var result = Mapper.MapPromotion(listWaiting);

                return Ultility.Responses("", Enums.TypeCRUD.Success.ToString(), result);
            }
            catch (Exception e)
            {
                return Ultility.Responses("Có lỗi xảy ra !", Enums.TypeCRUD.Error.ToString(), description: e.Message);
            }
        } 
         public Response CreatePromotion(CreatePromotionViewModel input)
        {
            Promotion promotion
                        = Mapper.MapCreatePromotion(input);
            var user = GetCurrentUser(input.IdUserModify);
            input.ModifyBy = user.NameEmployee;
            promotion.TypeAction = "insert";
            promotion.ModifyDate = Ultility.ConvertDatetimeToUnixTimeStampMiliSecond(DateTime.Now);
            CreateDatabase(promotion);

            var listRole = Ultility.ConvertListInt(new int[] { Convert.ToInt16(Enums.TitleRole.Admin), Convert.ToInt16(Enums.TitleRole.LocalManager) });
            _notification.CreateNotification(user.IdEmployee, Convert.ToInt16(Enums.TypeNotification.Promotion), promotion.Value.ToString(), listRole, "");

            return Ultility.Responses("Thêm thành công !", Enums.TypeCRUD.Success.ToString());
        }

        public Response DeletePromotion(int id, Guid idUser)
        {
            try
            {
                var promotion = (from x in _db.Promotions.AsNoTracking()
                                 where x.IdPromotion == id
                             select x).FirstOrDefault();

                var userLogin = (from x in _db.Employees.AsNoTracking()
                                 where x.IdEmployee == idUser
                                 select x).FirstOrDefault();
                if (promotion.Approve == (int)ApproveStatus.Approved)
                {
                    promotion.ModifyBy = userLogin.NameEmployee;
                    promotion.TypeAction = "delete";
                    promotion.IdUserModify = userLogin.IdEmployee;
                    promotion.ModifyDate = Ultility.ConvertDatetimeToUnixTimeStampMiliSecond(DateTime.Now);
                    promotion.Approve = (int)ApproveStatus.Waiting;
                    promotion.IsDelete = true;
                    UpdateDatabase(promotion);

                    var listRole = Ultility.ConvertListInt(new int[] { Convert.ToInt16(Enums.TitleRole.Admin), Convert.ToInt16(Enums.TitleRole.LocalManager) });
                    _notification.CreateNotification(userLogin.IdEmployee, Convert.ToInt16(Enums.TypeNotification.Promotion), promotion.Value.ToString(), listRole, "");

                    return Ultility.Responses("Đã gửi yêu cầu xóa !", Enums.TypeCRUD.Success.ToString());
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

        public Response UpdatePromotion(UpdatePromotionViewModel input)
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

                UpdateDatabase(promotion);

                var listRole = Ultility.ConvertListInt(new int[] { Convert.ToInt16(Enums.TitleRole.Admin), Convert.ToInt16(Enums.TitleRole.LocalManager) });
                _notification.CreateNotification(userLogin.IdEmployee, Convert.ToInt16(Enums.TypeNotification.Promotion), promotion.Value.ToString(), listRole, "");

                return Ultility.Responses("Đã gửi yêu cầu sửa !", Enums.TypeCRUD.Success.ToString());

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
                    _notification.CreateNotification(userModify.IdEmployee, Convert.ToInt16(Enums.TypeNotification.Promotion), promotion.Value.ToString(), userModify.RoleId.ToString(), "Thành công");
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
                    _notification.CreateNotification(userModify.IdEmployee, Convert.ToInt16(Enums.TypeNotification.Promotion), promotion.Value.ToString(), userModify.RoleId.ToString(), "Từ chối");

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

        public Response RestorePromotion(int id, Guid idUser)
        {
            try
            {
                var promotion = (from x in _db.Promotions.AsNoTracking()
                             where x.IdPromotion == id
                             select x).FirstOrDefault();

                var userLogin = (from x in _db.Employees.AsNoTracking()
                                 where x.IdEmployee == idUser
                                 select x).FirstOrDefault();
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

                var listRole = Ultility.ConvertListInt(new int[] { Convert.ToInt16(Enums.TitleRole.Admin), Convert.ToInt16(Enums.TitleRole.LocalManager) });
                _notification.CreateNotification(userLogin.IdEmployee, Convert.ToInt16(Enums.TypeNotification.Promotion), promotion.Value.ToString(), listRole, "");

                return Ultility.Responses("Đã gửi yêu cầu khôi phục !", Enums.TypeCRUD.Success.ToString());

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
    }
}
