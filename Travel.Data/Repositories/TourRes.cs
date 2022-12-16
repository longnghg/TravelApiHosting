using Microsoft.AspNetCore.Http;
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
using Travel.Data.Repositories.NotifyRes;
using Travel.Shared.Ultilities;
using Travel.Shared.ViewModels;
using Travel.Shared.ViewModels.Travel.TourVM;
using static Travel.Shared.Ultilities.Enums;

namespace Travel.Data.Repositories
{
    public class TourRes : ITour
    {
        private long dateTimeNow;
        private readonly TravelContext _db;
        private Notification message;
        private INotification _notification;
        private readonly ILog _log;
        private readonly ICache _cache;

        private void UpdateDatabase<T>(T input)
        {
            _db.Entry(input).State = EntityState.Modified;
        }
        private void DeleteDatabase<T>(T input)
        {
            _db.Entry(input).State = EntityState.Deleted;
        }
        private void CreateDatabase<T>(T input)
        {
            _db.Entry(input).State = EntityState.Added;
        }
        private async Task SaveChangeAsync()
        {
            await _db.SaveChangesAsync();
        }
        private void SaveChange()
        {
            _db.SaveChanges();
        }
        public TourRes(TravelContext db, INotification notification, ILog log, ICache cache)
        {
            _db = db;
            message = new Notification();
            dateTimeNow = Ultility.ConvertDatetimeToUnixTimeStampMiliSecond(DateTime.Now.AddMinutes(-3));
            _notification = notification;
            _log = log;
            _cache = cache;
        }
        private Employee GetCurrentUser(Guid IdUserModify)
        {
            return (from x in _db.Employees
                    where x.IdEmployee == IdUserModify
                    select x).FirstOrDefault();
        }
        public string CheckBeforSave(IFormCollection frmdata, IFormFile file, ref Notification _message, bool isUpdate)
        {
            try
            {
                JObject frmData = JObject.Parse(frmdata["data"]);
                if (frmData != null)
                {
                    var tourName = PrCommon.GetString("nameTour", frmData);
                    if (String.IsNullOrEmpty(tourName))
                    {
                    }
                    if (isExistName(tourName))
                    {
                    }
                    var idTour = PrCommon.GetString("idTour", frmData);
                    if (String.IsNullOrEmpty(idTour))
                    {
                        idTour = Ultility.GenerateId(tourName);
                    }

                    #region check update info tour when having booking

                    if (isUpdate)
                    {
                        idTour = PrCommon.GetString("idTour", frmData);
                        if (scheduleInTourHaveAnyBooking(idTour))
                        {
                            _message = Ultility.Responses("Tour đang có booking !", Enums.TypeCRUD.Warning.ToString()).Notification;
                            return null;
                        }
                    }

                    #endregion

                    var thumbnail = PrCommon.GetString("thumbnail", frmData);
                    if (String.IsNullOrEmpty(thumbnail))
                    {
                    }
                    var toPlace = PrCommon.GetString("toPlace", frmData);
                    if (String.IsNullOrEmpty(toPlace))
                    {
                    }
                    var description = PrCommon.GetString("description", frmData);
                    if (String.IsNullOrEmpty(description))
                    {
                    }
                    var rating = PrCommon.GetString("rating", frmData);
                    if (String.IsNullOrEmpty(rating))
                    {
                    }
                    if (file != null)
                    {
                        thumbnail = Ultility.WriteFile(file, "Tour", idTour, ref _message).FilePath;
                        if (_message != null)
                        {
                            message = _message;
                        }
                    }

                    var typeAction = PrCommon.GetString("typeAction", frmData);
                    var idUserModify = PrCommon.GetString("idUserModify", frmData);
                    if (String.IsNullOrEmpty(idUserModify))
                    {
                    }
                    if (isUpdate)
                    {
                        // map data
                        UpdateTourViewModel objUpdate = new UpdateTourViewModel();
                        objUpdate.NameTour = tourName;
                        objUpdate.NameTour_EN = Ultility.removeVietnameseSign(tourName);
                        objUpdate.Thumbnail = thumbnail;
                        objUpdate.ToPlace = toPlace;
                        objUpdate.TypeAction = typeAction;
                        objUpdate.IdUserModify = Guid.Parse(idUserModify);
                        // generate ID
                        objUpdate.IdTour = idTour;
                        return JsonSerializer.Serialize(objUpdate);
                    }
                    // map data
                    CreateTourViewModel obj = new CreateTourViewModel();
                    obj.NameTour = tourName;
                    obj.NameTour_EN = Ultility.removeVietnameseSign(tourName);
                    obj.Thumbnail = thumbnail;
                    obj.ToPlace = toPlace;
                    obj.TypeAction = typeAction;
                    obj.Rating = 0;
                    obj.IdUserModify = Guid.Parse(idUserModify);
                    // generate ID
                    obj.IdTour = idTour;

                    return JsonSerializer.Serialize(obj);
                }
                return string.Empty;
            }
            catch (Exception e)
            {
                _message = Ultility.Responses("Có lỗi xảy ra !", Enums.TypeCRUD.Error.ToString(), description: e.Message).Notification;
                return string.Empty;
            }
        }
        public Response Create(CreateTourViewModel input, string emailUser)
        {
            try
            {
                Tour tour =
                tour = Mapper.MapCreateTour(input);
                tour.Alias = Ultility.SEOUrl(tour.NameTour.ToLower());
                var userLogin = GetCurrentUser(input.IdUserModify);
                tour.ModifyBy = userLogin.NameEmployee;
                tour.ModifyDate = Ultility.ConvertDatetimeToUnixTimeStampMiliSecond(DateTime.Now);
                tour.TypeAction = "insert";
                string jsonContent = JsonSerializer.Serialize(tour);
                CreateDatabase(tour);
                SaveChange();
                var listRole = new int[] { Convert.ToInt16(Enums.TitleRole.Admin), Convert.ToInt16(Enums.TitleRole.LocalManager) };
                _notification.CreateNotification(userLogin.IdEmployee, Convert.ToInt16(Enums.TypeNotification.Tour), tour.NameTour, listRole, "");

                bool result = _log.AddLog(content: jsonContent, type: "create", emailCreator: emailUser, classContent: "Tour");
                if (result)
                {
                    return Ultility.Responses("Đã gửi yêu cầu thêm !", Enums.TypeCRUD.Success.ToString(), content: tour.IdTour);

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

        public Response Get(bool isDelete)
        {
            try
            {
                if (isDelete == false)
                {
                    #region check cache
                    if (_cache.Get<Response>($"tour") != null) // có cache
                    {
                        return _cache.Get<Response>($"tour");
                    }
                    #endregion
                }

                var list = (from x in _db.Tour.AsNoTracking()
                            where x.IsDelete == isDelete
                            && x.ApproveStatus == Convert.ToInt16(Enums.ApproveStatus.Approved)
                            && x.IsTempdata == false

                            orderby x.Rating descending
                            select x).ToList();
                var result = Mapper.MapTour(list);
                var res = Ultility.Responses("", Enums.TypeCRUD.Success.ToString(), result);
                if (isDelete == false)
                {
                    #region save cache
                    _cache.Set(res, $"tour");
                    #endregion
                }
                return res;
            }
            catch (Exception e)
            {
                return Ultility.Responses("Có lỗi xảy ra !", Enums.TypeCRUD.Error.ToString(), description: e.Message);
            }
        }

        public Response GetTour(string idTour)
        {
            try
            {
                // cách 1
                //Tour tour = _db.Tour.Find(idTour);
                // cashc 2
                Tour tour = (from x in _db.Tour.AsNoTracking()
                             where x.IdTour == idTour
                             select x).FirstOrDefault();
                var result = Mapper.MapTour(tour);
                return Ultility.Responses("", Enums.TypeCRUD.Success.ToString(), result);
            }
            catch (Exception e)
            {
                return Ultility.Responses("Có lỗi xảy ra !", Enums.TypeCRUD.Error.ToString(), description: e.Message);
            }
        }


        private bool isExistName(string input)
        {
            try
            {
                var tour =
                    (from x in _db.Tour.AsNoTracking()
                     where x.NameTour.ToLower() == input.ToLower()
                     select x).FirstOrDefault();
                if (tour != null)
                {
                    return true;
                }
                return false;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public async Task<Response> GetsTourWithSchedule()
        {
            try
            {
                var list = await (from x in _db.Tour.AsNoTracking()
                                  where x.IsDelete == false
                                  && x.ApproveStatus == Convert.ToInt16(Enums.ApproveStatus.Approved)
                                  && x.IsActive == true
                                  && x.IsTempdata == false
                                  select new Tour
                                  {
                                      Thumbnail = x.Thumbnail,
                                      ToPlace = x.ToPlace,
                                      IdTour = x.IdTour,
                                      NameTour = x.NameTour,
                                      Alias = x.Alias,
                                      Rating = x.Rating,
                                      QuantityBooked = x.QuantityBooked,
                                      Schedules = (from s in _db.Schedules
                                                   where s.TourId == x.IdTour
                                                   && s.Isdelete == false
                                                   && s.EndDate >= dateTimeNow
                                                   && s.BeginDate <= dateTimeNow
                                                   && s.Status == (int)Enums.StatusSchedule.Free
                                                   orderby s.DepartureDate
                                                   select new Schedule
                                                   {
                                                       DepartureDate = s.DepartureDate,
                                                       ReturnDate = s.ReturnDate,
                                                       DeparturePlace = s.DeparturePlace,
                                                       Description = s.Description,
                                                       BeginDate = s.BeginDate,
                                                       EndDate = s.EndDate,
                                                       MetaDesc = s.MetaDesc,
                                                       MetaKey = s.MetaKey,
                                                       AdditionalPrice = s.AdditionalPrice,
                                                       AdditionalPriceHoliday = s.AdditionalPriceHoliday,
                                                       Alias = s.Alias,
                                                       Status = s.Status,
                                                       Approve = s.Approve,
                                                       FinalPrice = s.FinalPrice,
                                                       FinalPriceHoliday = s.FinalPriceHoliday,
                                                       IdSchedule = s.IdSchedule,
                                                       IsHoliday = s.IsHoliday,
                                                       MinCapacity = s.MinCapacity,
                                                       MaxCapacity = s.MaxCapacity,
                                                       PriceAdult = s.PriceAdult,
                                                       PriceAdultHoliday = s.PriceAdultHoliday,
                                                       PriceChild = s.PriceChild,
                                                       PriceBabyHoliday = s.PriceChildHoliday,
                                                       PriceBaby = s.PriceBaby,
                                                       PriceChildHoliday = s.PriceChildHoliday,
                                                       QuantityAdult = s.QuantityAdult,
                                                       QuantityBaby = s.QuantityBaby,
                                                       QuantityChild = s.QuantityChild,
                                                       QuantityCustomer = s.QuantityCustomer,
                                                       TotalCostTourNotService = s.TotalCostTourNotService,
                                                       Vat = s.Vat,
                                                       Profit = s.Profit,
                                                       TimePromotion = s.TimePromotion,
                                                       Promotions = (from pro in _db.Promotions
                                                                     where pro.IdPromotion == s.PromotionId
                                                                     select pro).FirstOrDefault(),
                                                       Car = (from car in _db.Cars
                                                              where car.IdCar == s.CarId
                                                              select car).First(),
                                                       Timelines = (from timeline in _db.Timelines
                                                                    where timeline.IdSchedule == s.IdSchedule
                                                                    && timeline.IsDelete == false
                                                                    select new Timeline
                                                                    {
                                                                        Description = timeline.Description,
                                                                        FromTime = timeline.FromTime,
                                                                        ToTime = timeline.ToTime,
                                                                    }).ToList(),
                                                       CostTour = (from c in _db.CostTours
                                                                   where c.IdSchedule == s.IdSchedule
                                                                   select c).First(),
                                                       Employee = (from e in _db.Employees
                                                                   where e.IdEmployee == s.EmployeeId
                                                                   select e).First()
                                                   }).ToList()
                                  }).ToListAsync();
                return Ultility.Responses("", Enums.TypeCRUD.Success.ToString(), list);
            }
            catch (Exception e)
            {
                return Ultility.Responses("Có lỗi xảy ra !", Enums.TypeCRUD.Error.ToString(), description: e.Message);
            }
        }

        public async Task<Response> GetTourById(string idTour)
        {
            try
            {
                var dateTimeNow = Ultility.ConvertDatetimeToUnixTimeStampMiliSecond(DateTime.Now);
                var list = await (from x in _db.Tour.AsNoTracking()
                                  where x.IsDelete == false
                                  && x.IdTour == idTour
                                  && x.ApproveStatus == Convert.ToInt16(Enums.ApproveStatus.Approved)
                                  && x.IsActive == true
                                  && x.IsTempdata == false
                                  select new Tour
                                  {
                                      Thumbnail = x.Thumbnail,
                                      ToPlace = x.ToPlace,
                                      IdTour = x.IdTour,
                                      NameTour = x.NameTour,
                                      Alias = x.Alias,
                                      Rating = x.Rating,
                                      QuantityBooked = x.QuantityBooked,
                                      Schedules = (from s in _db.Schedules
                                                   where s.TourId == x.IdTour
                                                   && s.Status == (int)Enums.StatusSchedule.Free
                                                   && s.EndDate >= dateTimeNow
                                                   && s.BeginDate <= dateTimeNow
                                                   select new Schedule
                                                   {
                                                       DepartureDate = s.DepartureDate,
                                                       ReturnDate = s.ReturnDate,
                                                       DeparturePlace = s.DeparturePlace,
                                                       Description = s.Description,
                                                       BeginDate = s.BeginDate,
                                                       EndDate = s.EndDate,
                                                       MetaDesc = s.MetaDesc,
                                                       MetaKey = s.MetaKey,
                                                       AdditionalPrice = s.AdditionalPrice,
                                                       AdditionalPriceHoliday = s.AdditionalPriceHoliday,
                                                       Alias = s.Alias,
                                                       Status = s.Status,
                                                       Approve = s.Approve,
                                                       FinalPrice = s.FinalPrice,
                                                       FinalPriceHoliday = s.FinalPriceHoliday,
                                                       IdSchedule = s.IdSchedule,
                                                       IsHoliday = s.IsHoliday,
                                                       MinCapacity = s.MinCapacity,
                                                       MaxCapacity = s.MaxCapacity,
                                                       PriceAdult = s.PriceAdult,
                                                       PriceAdultHoliday = s.PriceAdultHoliday,
                                                       PriceChild = s.PriceChild,
                                                       PriceBabyHoliday = s.PriceChildHoliday,
                                                       PriceBaby = s.PriceBaby,
                                                       PriceChildHoliday = s.PriceChildHoliday,
                                                       QuantityAdult = s.QuantityAdult,
                                                       QuantityBaby = s.QuantityBaby,
                                                       QuantityChild = s.QuantityChild,
                                                       QuantityCustomer = s.QuantityCustomer,
                                                       TotalCostTourNotService = s.TotalCostTourNotService,
                                                       Vat = s.Vat,
                                                       Profit = s.Profit,
                                                       TimePromotion = s.TimePromotion,
                                                       Promotions = (from pro in _db.Promotions
                                                                     where pro.IdPromotion == s.PromotionId
                                                                     select pro).FirstOrDefault(),
                                                       Car = (from car in _db.Cars
                                                              where car.IdCar == s.CarId
                                                              select car).First(),
                                                       Timelines = (from timeline in _db.Timelines
                                                                    where timeline.IdSchedule == s.IdSchedule
                                                                    && timeline.IsDelete == false
                                                                    select new Timeline
                                                                    {
                                                                        Description = timeline.Description,
                                                                        FromTime = timeline.FromTime,
                                                                        ToTime = timeline.ToTime,
                                                                    }).ToList(),
                                                       CostTour = (from c in _db.CostTours
                                                                   where c.IdSchedule == s.IdSchedule
                                                                   select c).First(),
                                                       Employee = (from e in _db.Employees
                                                                   where e.IdEmployee == s.EmployeeId
                                                                   select e).First()
                                                   }).ToList()
                                  }).FirstAsync();
                return Ultility.Responses("", Enums.TypeCRUD.Success.ToString(), list);
            }
            catch (Exception e)
            {
                return Ultility.Responses("Có lỗi xảy ra !", Enums.TypeCRUD.Error.ToString(), description: e.Message);
            }
        }


        #region Đang chỉnh
        public Response Update(UpdateTourViewModel input, string emailUser)
        {
            try
            {
                Tour tour = (from x in _db.Tour.AsNoTracking()
                             where x.IdTour == input.IdTour
                             && x.IsDelete == false
                             && x.ApproveStatus == (int)ApproveStatus.Approved
                             select x).FirstOrDefault();
                var userLogin = GetCurrentUser(input.IdUserModify);
                // clone new object
                var tourOld = new Tour();
                tourOld = Ultility.DeepCopy<Tour>(tour);
                tourOld.IdAction = tourOld.IdTour.ToString();
                tourOld.IdTour = $"{Ultility.ConvertDatetimeToUnixTimeStampMiliSecond(DateTime.Now)}TempData";
                tourOld.IsTempdata = true;
                string jsonContent = JsonSerializer.Serialize(tour);

                CreateDatabase<Tour>(tourOld);
                SaveChange();
                #region setdata
                tour.IdAction = tourOld.IdTour.ToString();
                tour.IdUserModify = input.IdUserModify;
                tour.ApproveStatus = (int)ApproveStatus.Waiting;
                tour.ModifyBy = userLogin.NameEmployee;
                tour.ModifyDate = Ultility.ConvertDatetimeToUnixTimeStampMiliSecond(DateTime.Now);

                tour.Alias = Ultility.SEOUrl(input.NameTour);
                tour.NameTour = input.NameTour;
                tour.Thumbnail = input.Thumbnail;
                tour.ToPlace = input.ToPlace;
                tour.TypeAction = "update";
                tour.Status = (int)TourStatus.Normal;
                #endregion


                UpdateDatabase<Tour>(tour);
                SaveChange();





                var listRole = new int[] { Convert.ToInt16(Enums.TitleRole.Admin), Convert.ToInt16(Enums.TitleRole.LocalManager) };
                _notification.CreateNotification(userLogin.IdEmployee, Convert.ToInt16(Enums.TypeNotification.Tour), tour.NameTour, listRole, "");
                bool result = _log.AddLog(content: jsonContent, type: "update", emailCreator: emailUser, classContent: "Tour");
                if (result)
                {
                    return Ultility.Responses("Đã gửi yêu cầu sửa !", Enums.TypeCRUD.Success.ToString());

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

        public Response Delete(string idTour, Guid idUser, string emailUser)
        {
            try
            {
                var tour = (from x in _db.Tour.AsNoTracking()
                            where x.IdTour == idTour
                            select x).FirstOrDefault();
                var userLogin = (from x in _db.Employees
                                 where x.IdEmployee == idUser
                                 select x).FirstOrDefault();
                string jsonContent = JsonSerializer.Serialize(tour);

                var unixNow = Ultility.ConvertDatetimeToUnixTimeStampMiliSecond(DateTime.Now);
                if (tour != null)
                {
                    if (tour.ApproveStatus == (int)ApproveStatus.Approved)
                    {
                        //#region Check if not any schedule in tour
                        //var countScheduleInTour = (from x in _db.Schedules.AsNoTracking()
                        //                           where x.TourId == idTour
                        //                           && x.Isdelete == false
                        //                           select x.IdSchedule).Count();
                        //if (countScheduleInTour == 0) // not any scshedule in tour
                        //{
                        //    tour.IsDelete = true;
                        //    tour.ApproveStatus = (int)ApproveStatus.Waiting;
                        //    tour.TypeAction = "delete";
                        //    tour.IdUserModify = idUser;
                        //    tour.ModifyBy = userLogin.NameEmployee;
                        //    tour.ModifyDate = Ultility.ConvertDatetimeToUnixTimeStampMiliSecond(DateTime.Now);
                        //    UpdateDatabase(tour);
                        //    return Ultility.Responses("Đã gửi yêu cầu xóa !", Enums.TypeCRUD.Success.ToString());

                        //}
                        //#endregion



                        //cách 2
                        //var scheduleInTour = (from x in _db.Tourbookings
                        //           where (x.Status >= (int)Enums.StatusBooking.Paying && x.Status <= (int)Enums.StatusBooking.Paid)
                        //           select new
                        //           {
                        //               Schedule = (from s in _db.Schedules
                        //                           where s.TourId == idTour
                        //                           select s).FirstOrDefault()
                        //           }).ToList();


                        if (scheduleInTourHaveAnyBooking(idTour)) // tour đang thay đổi ko có booking
                        {
                            return Ultility.Responses("Tour đang có booking !", Enums.TypeCRUD.Warning.ToString());
                        }
                        else
                        {
                            tour.IsDelete = true;
                            tour.ApproveStatus = (int)ApproveStatus.Waiting;
                            tour.TypeAction = "delete";
                            tour.IdUserModify = idUser;
                            tour.ModifyBy = userLogin.NameEmployee;
                            tour.ModifyDate = Ultility.ConvertDatetimeToUnixTimeStampMiliSecond(DateTime.Now);
                            tour.Status = (int)TourStatus.Normal;
                            UpdateDatabase(tour);
                            SaveChange();
                            _cache.Remove("tour");
                            var listRole = new int[] { Convert.ToInt16(Enums.TitleRole.Admin), Convert.ToInt16(Enums.TitleRole.LocalManager) };
                            _notification.CreateNotification(userLogin.IdEmployee, Convert.ToInt16(Enums.TypeNotification.Tour), tour.NameTour, listRole, "");
                            bool result = _log.AddLog(content: jsonContent, type: "delete", emailCreator: emailUser, classContent: "Tour");
                            if (result)
                            {
                                return Ultility.Responses("Đã gửi yêu cầu xóa !", Enums.TypeCRUD.Success.ToString());
                            }
                            else
                            {
                                return Ultility.Responses("Lỗi log!", Enums.TypeCRUD.Error.ToString());
                            }

                        }
                    }
                    else
                    {
                        if (tour.IdUserModify == idUser)
                        {
                            if (tour.TypeAction == "insert")
                            {
                                DelImageCreate(tour.IdTour, "");
                                DeleteDatabase(tour);
                                SaveChange();
                                _cache.Remove("tour");
                                _cache.Remove("schedule");
                                return Ultility.Responses("Đã xóa!", Enums.TypeCRUD.Success.ToString());
                            }
                            else if (tour.TypeAction == "update")
                            {

                                DelOrResImageTour(tour.IdTour, "restore");
                                DelImageCreate(tour.IdTour, "");

                                var idTourTemp = tour.IdAction;
                                // old hotel
                                var tourTemp = (from x in _db.Tour.AsNoTracking()
                                                where x.IdTour == idTourTemp
                                                select x).FirstOrDefault();

                                tour.IdAction = null;
                                tour.TypeAction = null;

                                #region restore old data

                                tour.Alias = tourTemp.Alias;
                                tour.NameTour = tourTemp.NameTour;
                                tour.Thumbnail = tourTemp.Thumbnail;
                                tour.ApproveStatus = (int)ApproveStatus.Approved;
                                tour.ToPlace = tourTemp.ToPlace;
                                tour.Status = tourTemp.Status;
                                #endregion

                                _db.Entry(tour).State = EntityState.Modified;
                                DeleteDatabase(tourTemp);
                                SaveChange();
                                _cache.Remove("tour");
                                _cache.Remove("schedule");
                                return Ultility.Responses("Đã hủy yêu cầu chỉnh sửa !", Enums.TypeCRUD.Success.ToString());
                            }
                            else if (tour.TypeAction == "restore")
                            {
                                tour.IdAction = null;
                                tour.TypeAction = null;
                                tour.IsDelete = true;
                                tour.ApproveStatus = (int)ApproveStatus.Approved;
                                UpdateDatabase(tour);
                                SaveChange();
                                _cache.Remove("tour");
                                _cache.Remove("schedule");
                                return Ultility.Responses("Đã hủy yêu cầu khôi phục !", Enums.TypeCRUD.Success.ToString());

                            }
                            else // delete
                            {
                                tour.IdAction = null;
                                tour.TypeAction = null;
                                tour.IsDelete = false;
                                tour.ApproveStatus = (int)ApproveStatus.Approved;
                                tour.Status = (int)TourStatus.Promotion;
                                UpdateDatabase(tour);
                                SaveChange();
                                _cache.Remove("tour");
                                _cache.Remove("schedule");
                                return Ultility.Responses("Đã hủy yêu cầu xóa !", Enums.TypeCRUD.Success.ToString());
                            }
                        }
                        else
                        {
                            return Ultility.Responses("Bạn không thể thực thi hành động này !", Enums.TypeCRUD.Success.ToString());
                        }
                    }
                }
                else
                {
                    return Ultility.Responses("Không tìm thấy !", Enums.TypeCRUD.Warning.ToString());
                }
            }
            catch (Exception e)
            {
                return Ultility.Responses("Có lỗi xảy ra !", Enums.TypeCRUD.Error.ToString(), description: e.Message);
            }
        }
        public Response GetWaiting(Guid idUser, int pageIndex, int pageSize)
        {
            try
            {
                var totalResult = 0;
                var userLogin = (from x in _db.Employees.AsNoTracking()
                                 where x.IdEmployee == idUser
                                 select x).FirstOrDefault();
                var listWaiting = new List<Tour>();
                if (userLogin.RoleId == (int)Enums.TitleRole.Admin)
                {
                    var querylistWaiting = (from x in _db.Tour.AsNoTracking()
                                            where x.ApproveStatus == Convert.ToInt16(ApproveStatus.Waiting)
                                            orderby x.ModifyDate descending

                                            select x);
                    totalResult = querylistWaiting.Count();
                    listWaiting = querylistWaiting.Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();
                }
                else
                {
                    var querylistWaiting = (from x in _db.Tour.AsNoTracking()
                                            where x.IdUserModify == idUser
                                            && x.ApproveStatus == Convert.ToInt16(ApproveStatus.Waiting)
                                            orderby x.ModifyDate descending

                                            select x);
                    totalResult = querylistWaiting.Count();
                    listWaiting = querylistWaiting.Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();
                }
                var result = Mapper.MapTour(listWaiting);
                var res = Ultility.Responses("", Enums.TypeCRUD.Success.ToString(), result);
                res.TotalResult = totalResult;
                return res;
            }
            catch (Exception e)
            {
                return Ultility.Responses("Có lỗi xảy ra !", Enums.TypeCRUD.Error.ToString(), description: e.Message);
            }
        }

        public Response Approve(string idTour)
        {
            try
            {
                var tour = (from x in _db.Tour.AsNoTracking()
                            where x.IdTour == idTour
                            && x.ApproveStatus == (int)ApproveStatus.Waiting
                            select x).FirstOrDefault();

                if (tour != null)
                {
                    if (tour.TypeAction == "update")
                    {
                        DelOrResImageTour(tour.IdTour, "delete");
                        DelImageCreate(tour.IdTour, "approve");
                        var idTourTemp = tour.IdAction;
                        tour.ApproveStatus = (int)ApproveStatus.Approved;
                        tour.IdAction = null;
                        tour.TypeAction = null;
                        tour.Status = (int)TourStatus.Promotion;
                        UpdateDatabase(tour);
                        SaveChange();
                        _cache.Remove("tour");
                        _cache.Remove("schedule");
                        // delete tempdata
                        var tourTemp = (from x in _db.Tour.AsNoTracking()
                                        where x.IdTour == idTourTemp
                                        select x).FirstOrDefault();
                        DeleteDatabase(tourTemp);
                        SaveChange();
                    }
                    else if (tour.TypeAction == "insert")
                    {
                        tour.IdAction = null;
                        tour.IsActive = true;
                        tour.TypeAction = null;
                        tour.ApproveStatus = (int)ApproveStatus.Approved;
                        tour.Status = (int)TourStatus.Promotion;
                        UpdateDatabase(tour);
                        SaveChange();
                        _cache.Remove("tour");
                        _cache.Remove("schedule");
                    }
                    else if (tour.TypeAction == "restore")
                    {
                        tour.IdAction = null;
                        tour.TypeAction = null;
                        tour.IsActive = true;
                        tour.ApproveStatus = (int)ApproveStatus.Approved;
                        tour.IsDelete = false;
                        tour.Status = (int)TourStatus.Promotion;
                        UpdateDatabase(tour);
                        SaveChange();
                        _cache.Remove("tour");
                        _cache.Remove("schedule");
                    }
                    else
                    {

                        tour.IdAction = null;
                        tour.TypeAction = null;
                        tour.ApproveStatus = (int)ApproveStatus.Approved;
                        tour.IsDelete = true;
                        tour.IsActive = false;
                        tour.Status = (int)TourStatus.Normal;
                        UpdateDatabase(tour);
                        SaveChange();
                        _cache.Remove("tour");
                        _cache.Remove("schedule");
                    }
                }
                var userModify = GetCurrentUser(tour.IdUserModify);
                _notification.CreateNotification(userModify.IdEmployee, Convert.ToInt16(Enums.TypeNotification.Tour), tour.NameTour, new int[] { userModify.RoleId }, "Thành công");
                return Ultility.Responses($"Duyệt thành công !", Enums.TypeCRUD.Success.ToString());

            }
            catch (Exception e)
            {
                return Ultility.Responses("Có lỗi xảy ra !", Enums.TypeCRUD.Error.ToString(), description: e.Message);
            }
        }
        public Response Refused(string idTour)
        {
            try
            {
                var tour = (from x in _db.Tour.AsNoTracking()
                            where x.IdTour == idTour
                            && x.ApproveStatus == (int)ApproveStatus.Waiting
                            select x).FirstOrDefault();
                if (tour != null)
                {
                    if (tour.TypeAction == "insert")
                    {
                        tour.ApproveStatus = (int)ApproveStatus.Refused;
                        tour.IdAction = null;
                        tour.TypeAction = null;
                        tour.Status = (int)TourStatus.Refused;
                        UpdateDatabase(tour);
                        SaveChange();
                        _cache.Remove("tour");
                        _cache.Remove("schedule");
                    }
                    else if (tour.TypeAction == "update")
                    {
                        DelOrResImageTour(tour.IdTour, "restore");
                        DelImageCreate(tour.IdTour, "");

                        var idTourTemp = tour.IdAction;
                        // old hotel
                        var tourTemp = (from x in _db.Tour.AsNoTracking()
                                        where x.IdTour == idTourTemp
                                        select x).FirstOrDefault();

                        tour.IdAction = null;
                        tour.TypeAction = null;

                        #region restore old data

                        tour.Alias = tourTemp.Alias;
                        tour.NameTour = tourTemp.NameTour;
                        tour.Thumbnail = tourTemp.Thumbnail;
                        tour.ToPlace = tourTemp.ToPlace;
                        tour.ApproveStatus = (int)ApproveStatus.Approved;
                        tour.Status = tourTemp.Status;
                        #endregion
                        _db.Entry(tour).State = EntityState.Modified;
                        DeleteDatabase(tourTemp);
                        SaveChange();
                        _cache.Remove("tour");
                        _cache.Remove("schedule");
                    }

                    else if (tour.TypeAction == "restore")
                    {
                        tour.IdAction = null;
                        tour.TypeAction = null;
                        tour.IsDelete = true;
                        tour.ApproveStatus = (int)ApproveStatus.Approved;
                        UpdateDatabase(tour);
                        SaveChange();
                        _cache.Remove("tour");
                        _cache.Remove("schedule");
                    }
                    else // delete
                    {
                        tour.IdAction = null;
                        tour.TypeAction = null;
                        tour.IsDelete = false;
                        tour.ApproveStatus = (int)ApproveStatus.Approved;
                        tour.Status = (int)TourStatus.Promotion;
                        UpdateDatabase(tour);
                        SaveChange();
                        _cache.Remove("tour");
                        _cache.Remove("schedule");
                    }

                    var userModify = GetCurrentUser(tour.IdUserModify);
                    _notification.CreateNotification(userModify.IdEmployee, Convert.ToInt16(Enums.TypeNotification.Tour), tour.NameTour, new int[] { userModify.RoleId }, "Từ chối");

                    return Ultility.Responses($"Từ chối thành công !", Enums.TypeCRUD.Success.ToString());
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


        public Response RestoreTour(string idTour, Guid idUser, string emailUser)
        {
            try
            {
                var tour = (from x in _db.Tour.AsNoTracking()
                            where x.IdTour == idTour
                            && x.IsDelete == true
                            select x).FirstOrDefault();
                var userLogin = (from x in _db.Employees.AsNoTracking()
                                 where x.IdEmployee == idUser
                                 select x).FirstOrDefault();
                if (tour != null)
                {
                    tour.IsDelete = false;
                    tour.IdUserModify = userLogin.IdEmployee;
                    tour.ApproveStatus = (int)ApproveStatus.Waiting;
                    tour.TypeAction = "restore";
                    tour.ModifyBy = userLogin.NameEmployee;
                    string jsonContent = JsonSerializer.Serialize(tour);

                    tour.ModifyDate = Ultility.ConvertDatetimeToUnixTimeStampMiliSecond(DateTime.Now);
                    UpdateDatabase(tour);
                    SaveChange();
                    _cache.Remove("tour");
                    _cache.Remove("schedule");
                    var listRole = new int[] { Convert.ToInt16(Enums.TitleRole.Admin), Convert.ToInt16(Enums.TitleRole.LocalManager) };
                    _notification.CreateNotification(userLogin.IdEmployee, Convert.ToInt16(Enums.TypeNotification.Tour), tour.NameTour, listRole, "");
                    bool result = _log.AddLog(content: jsonContent, type: "restore", emailCreator: emailUser, classContent: "Tour");
                    if (result)
                    {
                        return Ultility.Responses($"Đã gửi yêu cầu khôi phục !", Enums.TypeCRUD.Success.ToString());

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

        #endregion
        public async Task<Response> GetsTourByRating(int pageIndex, int pageSize)
        {
            try
            {
                #region check cache
                if (_cache.Get<Response>($"tour") != null) // có cache
                {
                    return _cache.Get<Response>($"tour");
                }
                #endregion
                var list = await (from x in _db.Tour.AsNoTracking()
                                  where x.Rating >= 9
                                  && x.IsDelete == false
                                  && x.ApproveStatus == Convert.ToInt16(Enums.ApproveStatus.Approved)
                                  && x.IsActive == true
                                  && x.IsTempdata == false
                                  select new Tour
                                  {
                                      Thumbnail = x.Thumbnail,
                                      ToPlace = x.ToPlace,
                                      IdTour = x.IdTour,
                                      NameTour = x.NameTour,
                                      Alias = x.Alias,
                                      Rating = x.Rating,
                                      QuantityBooked = x.QuantityBooked,
                                      CreateDate = x.CreateDate,
                                      Schedules = (from s in _db.Schedules.AsNoTracking()
                                                   where s.TourId == x.IdTour
                                                   && s.EndDate >= dateTimeNow
                                                   && s.BeginDate <= dateTimeNow
                                                   && s.Status == (int)Enums.StatusSchedule.Free
                                                   orderby s.DepartureDate
                                                   select new Schedule
                                                   {
                                                       DepartureDate = s.DepartureDate,
                                                       ReturnDate = s.ReturnDate,
                                                       DeparturePlace = s.DeparturePlace,
                                                       Description = s.Description,
                                                       BeginDate = s.BeginDate,
                                                       EndDate = s.EndDate,
                                                       MetaDesc = s.MetaDesc,
                                                       MetaKey = s.MetaKey,
                                                       AdditionalPrice = s.AdditionalPrice,
                                                       AdditionalPriceHoliday = s.AdditionalPriceHoliday,
                                                       Alias = s.Alias,
                                                       Status = s.Status,
                                                       Approve = s.Approve,
                                                       FinalPrice = s.FinalPrice,
                                                       FinalPriceHoliday = s.FinalPriceHoliday,
                                                       IdSchedule = s.IdSchedule,
                                                       IsHoliday = s.IsHoliday,
                                                       MinCapacity = s.MinCapacity,
                                                       MaxCapacity = s.MaxCapacity,
                                                       PriceAdult = s.PriceAdult,
                                                       PriceAdultHoliday = s.PriceAdultHoliday,
                                                       PriceChild = s.PriceChild,
                                                       PriceBabyHoliday = s.PriceChildHoliday,
                                                       PriceBaby = s.PriceBaby,
                                                       PriceChildHoliday = s.PriceChildHoliday,
                                                       QuantityAdult = s.QuantityAdult,
                                                       QuantityBaby = s.QuantityBaby,
                                                       QuantityChild = s.QuantityChild,
                                                       QuantityCustomer = s.QuantityCustomer,
                                                       TotalCostTourNotService = s.TotalCostTourNotService,
                                                       Vat = s.Vat,
                                                       Profit = s.Profit,
                                                       TimePromotion = s.TimePromotion,
                                                       Promotions = (from pro in _db.Promotions.AsNoTracking()
                                                                     where pro.IdPromotion == s.PromotionId
                                                                     select pro).FirstOrDefault(),
                                                       Car = (from car in _db.Cars.AsNoTracking()
                                                              where car.IdCar == s.CarId
                                                              select car).First(),
                                                       Timelines = (from timeline in _db.Timelines.AsNoTracking()
                                                                    where timeline.IdSchedule == s.IdSchedule
                                                                    && timeline.IsDelete == false
                                                                    select new Timeline
                                                                    {
                                                                        Description = timeline.Description,
                                                                        FromTime = timeline.FromTime,
                                                                        ToTime = timeline.ToTime,
                                                                    }).ToList(),
                                                       CostTour = (from c in _db.CostTours.AsNoTracking()
                                                                   where c.IdSchedule == s.IdSchedule
                                                                   select c).First(),
                                                       Employee = (from e in _db.Employees.AsNoTracking()
                                                                   where e.IdEmployee == s.EmployeeId
                                                                   select e).First()
                                                   }).ToList()
                                  }).OrderByDescending(x => x.Rating).ToListAsync();
                var result = list.Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();
                var res = Ultility.Responses("", Enums.TypeCRUD.Success.ToString(), result);
                     #region save cache
                    _cache.Set(res, $"tour");
                    #endregion
                res.TotalResult = result.Count();
                return res;
            }
            catch (Exception e)
            {
                return Ultility.Responses("Có lỗi xảy ra !", Enums.TypeCRUD.Error.ToString(), description: e.Message);
            }
        }
        private bool scheduleInTourHaveAnyBooking(string idTour)
        {
            try
            {
                return (from x in _db.Schedules.AsNoTracking()
                        where x.TourId == idTour
                        && x.Isdelete == false
                        && x.Approve == (int)Enums.ApproveStatus.Approved
                        && (x.Status == (int)Enums.StatusSchedule.Busy || (x.Status == (int)Enums.StatusSchedule.Free && x.QuantityCustomer > 0))
                        select x).Count() > 0 ? true : false;
            }
            catch (Exception)
            {
                return false;
                throw;
            }
        }

        //private bool CheckAnyBookingInTour(string idTour) // chỉ dùng khi thay đổi thông tin tour
        //{
        //    // cách 1
        //    var scheduleInTour = (from x in _db.Schedules
        //                          where x.TourId == idTour
        //                          && x.Isdelete == false
        //                          && x.Approve == (int)Enums.ApproveStatus.Approved
        //                          && (x.Status == (int)Enums.StatusSchedule.Finished || (x.Status == (int)Enums.StatusSchedule.Free && x.QuantityCustomer == 0))
        //                          select x).ToList();
        //    if (scheduleInTour.Count != 0)
        //    {
        //        return false;
        //    }
        //    else
        //    {
        //        return true;
        //    }
        //}

        //private bool CheckTourNoSchedule(string idTour) // chỉ dùng khi thay đổi thông tin tour
        //{
        //    var tourNoSchedule = (from x in _db.Schedules
        //                          where x.TourId == idTour
        //                          select x).ToList();
        //    if(tourNoSchedule.Count == 0)
        //    {
        //        return false;
        //    }
        //    else
        //    {
        //        return true;
        //    }
        //}

        //private bool CheckTourProgess(string idTour)// chỉ dùng khi thay đổi thông tin tour
        //{
        //    var scheduleInTourProgress = (from x in _db.Schedules
        //                                  where x.TourId == idTour
        //                                  && x.Isdelete == false
        //                                  && x.Approve == (int)Enums.ApproveStatus.Approved
        //                                  && (x.Status == (int)Enums.StatusSchedule.Busy || (x.Status == (int)Enums.StatusSchedule.Going))
        //                                  select x).ToList();
        //    if(scheduleInTourProgress.Count != 0)
        //    {
        //        return false;
        //    }
        //    else
        //    {
        //        return true;
        //    }
        //}

        public Response UpdateRating(int rating, string idTour, string emailUser)
        {
            using var transaction = _db.Database.BeginTransaction();

            try
            {

                transaction.CreateSavepoint("BeforeSave");

                var tour = (from x in _db.Tour.AsNoTracking()
                            where x.IdTour == idTour
                            select x).FirstOrDefault();
                string jsonContent = JsonSerializer.Serialize(tour);

                var listReviewByTour = (from x in _db.reviews.AsNoTracking()
                                        where x.IdTour == idTour
                                        select x).ToList();
                var countRating = listReviewByTour.Count();
                // get current rating
                var totalValueRating = listReviewByTour.Sum(x => x.Rating);

                var tempDataToGetValueRating = (countRating + 1) * rating;

                var ValueRatingNeeded = tempDataToGetValueRating - totalValueRating;



                // create review
                var review = new Review()
                {
                    Id = Guid.NewGuid(),
                    Rating = ValueRatingNeeded,
                    IdTour = idTour
                };
                listReviewByTour.Add(review);
                CreateDatabase(review);
                //SaveChange();

                tour.Rating = listReviewByTour.Average(x => x.Rating);
                UpdateDatabase(tour);
                SaveChange();

                transaction.Commit();
                transaction.Dispose();
                bool result = _log.AddLog(content: jsonContent, type: "update", emailCreator: emailUser, classContent: "Tour");
                if (result)
                {
                    return Ultility.Responses("Lượt đánh giá vừa chỉnh sửa !", Enums.TypeCRUD.Success.ToString());

                }
                else
                {
                    return Ultility.Responses("Lỗi log!", Enums.TypeCRUD.Error.ToString());
                }

            }
            catch (Exception e)
            {
                transaction.RollbackToSavepoint("BeforeSave");
                return Ultility.Responses("Có lỗi xảy ra !", Enums.TypeCRUD.Error.ToString(), description: e.Message);
            }
        }

        public async Task<Response> SearchAutoComplete(string key)
        {
            try
            {
                var keyUnSign = Ultility.removeVietnameseSign(key);
                var dateTimeNow = Ultility.ConvertDatetimeToUnixTimeStampMiliSecond(DateTime.Now);
                var result = await (from x in _db.Tour.AsNoTracking()
                                    where x.IsDelete == false
                                    && x.IsActive == true
                                    && x.ApproveStatus == (int)ApproveStatus.Approved
                                    && (from s in _db.Schedules.AsNoTracking()
                                        where s.TourId == x.IdTour
                                        && s.EndDate >= dateTimeNow
                                        select s.IdSchedule).Count() > 0
                                    select x).OrderByDescending(x => x.Rating).ToListAsync();

                return Ultility.Responses($"", Enums.TypeCRUD.Success.ToString(), content: result);

            }
            catch (Exception e)
            {
                return Ultility.Responses("Có lỗi xảy ra !", Enums.TypeCRUD.Error.ToString(), description: e.Message);
            }
        }

        public Response SearchTour(JObject frmData)
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

                var kwIdTour = PrCommon.GetString("idTour", frmData);
                if (!String.IsNullOrEmpty(kwIdTour))
                {
                    keywords.KwId = kwIdTour.Trim().ToLower();
                }
                else
                {
                    keywords.KwId = "";
                }

                var KwTourName = PrCommon.GetString("nameTour", frmData);
                if (!String.IsNullOrEmpty(KwTourName))
                {
                    keywords.KwName = KwTourName.Trim().ToLower();
                }
                else
                {
                    keywords.KwName = "";
                }

                var KwToPlace = PrCommon.GetString("toPlace", frmData);
                if (!String.IsNullOrEmpty(KwToPlace))
                {
                    keywords.KwToPlace = KwToPlace.Trim().ToLower();
                }
                else
                {
                    keywords.KwToPlace = "";
                }

                var kwRating = PrCommon.GetString("rating", frmData);
                keywords.KwRating = PrCommon.getListInt(kwRating, ',', false);

                var listTour = new List<Tour>();
                if (!string.IsNullOrEmpty(isDelete))
                {
                    if (keywords.KwRating.Count > 0)
                    {
                        var querylistTour = (from x in _db.Tour
                                             where x.IsDelete == keywords.IsDelete &&
                                                 x.IdTour.ToLower().Contains(keywords.KwId) &&
                                                 x.NameTour.ToLower().Contains(keywords.KwName) &&
                                                 x.ToPlace.ToLower().Contains(keywords.KwToPlace) &&
                                                 x.IsTempdata == false &&
                                                 x.ApproveStatus == Convert.ToInt16(Enums.ApproveStatus.Approved) &&
                                                 keywords.KwRating.Contains(Convert.ToInt16(x.Rating))
                                             orderby x.Rating

                                             select x);
                        totalResult = querylistTour.Count();
                        listTour = querylistTour.Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();
                    }
                    else
                    {
                        var querylistTour = (from x in _db.Tour
                                             where x.IsDelete == keywords.IsDelete &&
                                                 x.IdTour.ToLower().Contains(keywords.KwId) &&
                                                 x.NameTour.ToLower().Contains(keywords.KwName) &&
                                                 x.ToPlace.ToLower().Contains(keywords.KwToPlace) &&
                                                 x.IsTempdata == false &&
                                                 x.ApproveStatus == Convert.ToInt16(Enums.ApproveStatus.Approved)
                                             orderby x.Rating

                                             select x);
                        totalResult = querylistTour.Count();
                        listTour = querylistTour.Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();
                    }
                }
                else
                {
                    if (keywords.KwRating.Count > 0)
                    {
                        var querylistTour = (from x in _db.Tour
                                             where x.IsDelete == keywords.IsDelete &&
                                                 x.IdTour.ToLower().Contains(keywords.KwId) &&
                                                 x.NameTour.ToLower().Contains(keywords.KwName) &&
                                                 x.ToPlace.ToLower().Contains(keywords.KwToPlace) &&
                                                 x.IsTempdata == false &&
                                                 x.ApproveStatus == Convert.ToInt16(Enums.ApproveStatus.Approved) &&
                                                 keywords.KwRating.Contains(Convert.ToInt16(x.Rating))
                                             orderby x.Rating
                                             select new Tour
                                             {
                                                 IdTour = x.IdTour,
                                                 NameTour = x.NameTour,
                                                 ToPlace = x.ToPlace,
                                                 Rating = x.Rating,
                                                 Status = x.Status
                                             });
                        totalResult = querylistTour.Count();
                        listTour = querylistTour.Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();

                    }
                    else
                    {
                        var querylistTour = (from x in _db.Tour
                                             where x.IsDelete == keywords.IsDelete &&
                                                 x.IdTour.ToLower().Contains(keywords.KwId) &&
                                                 x.NameTour.ToLower().Contains(keywords.KwName) &&
                                                 x.ToPlace.ToLower().Contains(keywords.KwToPlace) &&
                                                 x.IsTempdata == false &&
                                                 x.ApproveStatus == Convert.ToInt16(Enums.ApproveStatus.Approved)
                                             //x.Rating.Equals(keywords.KwRating) &&
                                             orderby x.Rating
                                             select new Tour
                                             {
                                                 IdTour = x.IdTour,
                                                 NameTour = x.NameTour,
                                                 ToPlace = x.ToPlace,
                                                 Rating = x.Rating,
                                                 Status = x.Status
                                             });
                        totalResult = querylistTour.Count();
                        listTour = querylistTour.Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();
                    }

                }
                var result = Mapper.MapTour(listTour);
                if (result.Count > 0)
                {
                    var res = Ultility.Responses("", Enums.TypeCRUD.Success.ToString(), result);
                    res.TotalResult = totalResult;
                    return res;
                }
                else
                {
                    return Ultility.Responses("Không tìm thấy dữ liệu !", Enums.TypeCRUD.Warning.ToString(), result);
                }
            }
            catch (Exception e)
            {
                return Ultility.Responses("Có lỗi xảy ra !", Enums.TypeCRUD.Error.ToString(), description: e.Message);

            }

        }

        public Response SearchTourWaiting(JObject frmData)
        {
            try
            {
                Keywords keywords = new Keywords();

                var kwIdTour = PrCommon.GetString("idTour", frmData);
                if (!String.IsNullOrEmpty(kwIdTour))
                {
                    keywords.KwId = kwIdTour.Trim().ToLower();
                }
                else
                {
                    keywords.KwId = "";
                }

                var KwTourName = PrCommon.GetString("nameTour", frmData);
                if (!String.IsNullOrEmpty(KwTourName))
                {
                    keywords.KwName = KwTourName.Trim().ToLower();
                }
                else
                {
                    keywords.KwName = "";
                }

                var KwToPlace = PrCommon.GetString("toPlace", frmData);
                if (!String.IsNullOrEmpty(KwToPlace))
                {
                    keywords.KwToPlace = KwToPlace.Trim().ToLower();
                }
                else
                {
                    keywords.KwToPlace = "";
                }

                var kwRating = PrCommon.GetString("rating", frmData);
                keywords.KwRating = PrCommon.getListInt(kwRating, ',', false);

                var KwModifyBy = PrCommon.GetString("modifyBy", frmData);
                if (!String.IsNullOrEmpty(KwModifyBy))
                {
                    keywords.KwModifyBy = KwModifyBy.Trim().ToLower();
                }
                else
                {
                    keywords.KwModifyBy = "";
                }

                var fromDate = PrCommon.GetString("modifyDateFrom", frmData);
                if (!String.IsNullOrEmpty(fromDate))
                {
                    keywords.KwFromDate = Ultility.ConvertDatetimeToUnixTimeStampMiliSecond(DateTime.Parse(fromDate));
                }
                else
                {
                    keywords.KwFromDate = 0;
                }

                var toDate = PrCommon.GetString("modifyDateTo", frmData);
                if (!String.IsNullOrEmpty(toDate))
                {
                    keywords.KwToDate = Ultility.ConvertDatetimeToUnixTimeStampMiliSecond(DateTime.Parse(toDate).AddDays(1).AddSeconds(-1));
                }
                else
                {
                    keywords.KwToDate = 0;
                }

                var typeAction = PrCommon.GetString("typeAction", frmData);
                keywords.KwTypeActions = PrCommon.getListString(typeAction, ',', false);


                var listTour = new List<Tour>();
                if (keywords.KwRating.Count > 0)
                {
                    listTour = (from x in _db.Tour
                                where
                                    x.IdTour.ToLower().Contains(keywords.KwId) &&
                                    x.NameTour.ToLower().Contains(keywords.KwName) &&
                                    x.ToPlace.ToLower().Contains(keywords.KwToPlace) &&
                                    x.ModifyBy.ToLower().Contains(keywords.KwModifyBy) &&
                                    x.ApproveStatus == Convert.ToInt16(Enums.ApproveStatus.Waiting) &&
                                    x.Status == Convert.ToInt16(Enums.TourStatus.Normal) &&
                                    keywords.KwRating.Contains(Convert.ToInt16(x.Rating))
                                orderby x.ModifyDate descending
                                select new Tour
                                {
                                    IdTour = x.IdTour,
                                    NameTour = x.NameTour,
                                    ToPlace = x.ToPlace,
                                    Rating = x.Rating,
                                    Status = x.Status,
                                    TypeAction = x.TypeAction,
                                    ModifyDate = x.ModifyDate,
                                    ModifyBy = x.ModifyBy,
                                    ApproveStatus = x.ApproveStatus
                                }).ToList();
                }
                else
                {
                    if (keywords.KwTypeActions.Count > 0)
                    {
                        if (keywords.KwFromDate > 0 && keywords.KwToDate > 0)
                        {
                            listTour = (from x in _db.Tour
                                        where
                                            x.IdTour.ToLower().Contains(keywords.KwId) &&
                                            x.NameTour.ToLower().Contains(keywords.KwName) &&
                                            x.ToPlace.ToLower().Contains(keywords.KwToPlace) &&
                                            x.ModifyBy.ToLower().Contains(keywords.KwModifyBy) &&
                                            x.IsTempdata == false &&
                                            x.ApproveStatus == Convert.ToInt16(Enums.ApproveStatus.Waiting) &&
                                            x.Status == Convert.ToInt16(Enums.TourStatus.Normal) &&
                                             keywords.KwTypeActions.Contains(x.TypeAction) &&
                                            x.ModifyDate >= keywords.KwFromDate &&
                                            x.ModifyDate <= keywords.KwToDate
                                        orderby x.ModifyDate descending
                                        select x).ToList();
                        }
                        else
                        {
                            if (keywords.KwFromDate == 0 && keywords.KwToDate > 0)
                            {
                                listTour = (from x in _db.Tour
                                            where
                                                x.IdTour.ToLower().Contains(keywords.KwId) &&
                                                x.NameTour.ToLower().Contains(keywords.KwName) &&
                                                x.ToPlace.ToLower().Contains(keywords.KwToPlace) &&
                                                x.ModifyBy.ToLower().Contains(keywords.KwModifyBy) &&
                                                x.IsTempdata == false &&
                                                x.ApproveStatus == Convert.ToInt16(Enums.ApproveStatus.Waiting) &&
                                                x.Status == Convert.ToInt16(Enums.TourStatus.Normal) &&
                                                 keywords.KwTypeActions.Contains(x.TypeAction) &&
                                                x.ModifyDate <= keywords.KwToDate
                                            orderby x.ModifyDate descending
                                            select x).ToList();
                            }
                            else
                            {
                                if (keywords.KwToDate == 0 && keywords.KwFromDate > 0)
                                {
                                    listTour = (from x in _db.Tour
                                                where
                                                    x.IdTour.ToLower().Contains(keywords.KwId) &&
                                                    x.NameTour.ToLower().Contains(keywords.KwName) &&
                                                    x.ToPlace.ToLower().Contains(keywords.KwToPlace) &&
                                                    x.ModifyBy.ToLower().Contains(keywords.KwModifyBy) &&
                                                    x.IsTempdata == false &&
                                                    x.ApproveStatus == Convert.ToInt16(Enums.ApproveStatus.Waiting) &&
                                                    x.Status == Convert.ToInt16(Enums.TourStatus.Normal) &&
                                                     keywords.KwTypeActions.Contains(x.TypeAction) &&
                                                    x.ModifyDate >= keywords.KwFromDate
                                                orderby x.ModifyDate descending
                                                select x).ToList();
                                }
                                else
                                {
                                    listTour = (from x in _db.Tour
                                                where
                                                    x.IdTour.ToLower().Contains(keywords.KwId) &&
                                                    x.NameTour.ToLower().Contains(keywords.KwName) &&
                                                    x.ToPlace.ToLower().Contains(keywords.KwToPlace) &&
                                                    x.ModifyBy.ToLower().Contains(keywords.KwModifyBy) &&
                                                    x.IsTempdata == false &&
                                                    x.ApproveStatus == Convert.ToInt16(Enums.ApproveStatus.Waiting) &&
                                                    x.Status == Convert.ToInt16(Enums.TourStatus.Normal) &&
                                                     keywords.KwTypeActions.Contains(x.TypeAction)
                                                orderby x.ModifyDate descending
                                                select x).ToList();
                                }
                            }
                        }
                    }
                    else
                    {
                        if (keywords.KwFromDate > 0 && keywords.KwToDate > 0)
                        {
                            listTour = (from x in _db.Tour
                                        where
                                            x.IdTour.ToLower().Contains(keywords.KwId) &&
                                            x.NameTour.ToLower().Contains(keywords.KwName) &&
                                            x.ToPlace.ToLower().Contains(keywords.KwToPlace) &&
                                            x.ModifyBy.ToLower().Contains(keywords.KwModifyBy) &&
                                            x.IsTempdata == false &&
                                            x.ApproveStatus == Convert.ToInt16(Enums.ApproveStatus.Waiting) &&
                                            x.Status == Convert.ToInt16(Enums.TourStatus.Normal) &&
                                            x.ModifyDate >= keywords.KwFromDate &&
                                           x.ModifyDate <= keywords.KwToDate
                                        orderby x.ModifyDate descending
                                        select x).ToList();
                        }
                        else
                        {
                            if (keywords.KwFromDate == 0 && keywords.KwToDate > 0)
                            {
                                listTour = (from x in _db.Tour
                                            where
                                                x.IdTour.ToLower().Contains(keywords.KwId) &&
                                                x.NameTour.ToLower().Contains(keywords.KwName) &&
                                                x.ToPlace.ToLower().Contains(keywords.KwToPlace) &&
                                                x.ModifyBy.ToLower().Contains(keywords.KwModifyBy) &&
                                                x.IsTempdata == false &&
                                                x.ApproveStatus == Convert.ToInt16(Enums.ApproveStatus.Waiting) &&
                                                x.Status == Convert.ToInt16(Enums.TourStatus.Normal) &&
                                                x.ModifyDate <= keywords.KwToDate
                                            orderby x.ModifyDate descending
                                            select x).ToList();
                            }
                            else
                            {
                                if (keywords.KwToDate == 0 && keywords.KwFromDate > 0)
                                {
                                    listTour = (from x in _db.Tour
                                                where
                                                    x.IdTour.ToLower().Contains(keywords.KwId) &&
                                                    x.NameTour.ToLower().Contains(keywords.KwName) &&
                                                    x.ToPlace.ToLower().Contains(keywords.KwToPlace) &&
                                                    x.ModifyBy.ToLower().Contains(keywords.KwModifyBy) &&
                                                    x.IsTempdata == false &&
                                                    x.ApproveStatus == Convert.ToInt16(Enums.ApproveStatus.Waiting) &&
                                                    x.Status == Convert.ToInt16(Enums.TourStatus.Normal) &&
                                                    x.ModifyDate >= keywords.KwFromDate
                                                orderby x.ModifyDate descending
                                                select x).ToList();
                                }
                                else
                                {
                                    listTour = (from x in _db.Tour
                                                where
                                                    x.IdTour.ToLower().Contains(keywords.KwId) &&
                                                    x.NameTour.ToLower().Contains(keywords.KwName) &&
                                                    x.ToPlace.ToLower().Contains(keywords.KwToPlace) &&
                                                    x.ModifyBy.ToLower().Contains(keywords.KwModifyBy) &&
                                                    x.IsTempdata == false &&
                                                    x.ApproveStatus == Convert.ToInt16(Enums.ApproveStatus.Waiting) &&
                                                    x.Status == Convert.ToInt16(Enums.TourStatus.Normal)
                                                orderby x.ModifyDate descending
                                                select x).ToList();
                                }
                            }
                        }
                    }

                }
                var result = Mapper.MapTour(listTour);
                if (result.Count > 0)
                {
                    return Ultility.Responses("", Enums.TypeCRUD.Success.ToString(), result);
                }
                else
                {
                    return Ultility.Responses("Không tìm thấy dữ liệu !", Enums.TypeCRUD.Warning.ToString(), result);
                }

            }
            catch (Exception e)
            {
                return Ultility.Responses("Có lỗi xảy ra !", Enums.TypeCRUD.Error.ToString(), description: e.Message);

            }

        }

        public async Task<Tour> GetTourByIdForPayPal(string idTour)
        {
            try
            {
                return await _db.Tour.FindAsync(idTour);
            }
            catch (Exception e)
            {

                throw;
            }
        }

        private void DelOrResImageTour(string idTour, string type)
        {
            try
            {
                var imgTour = from s in _db.Images
                              where s.IsDelete == true &&
                                    s.IdService == idTour
                              select s;

                if (type == "delete")
                {
                    _db.RemoveRange(imgTour);
                    _db.SaveChanges();
                }
                else
                {
                    foreach (var img in imgTour)
                    {
                        img.IsDelete = false;
                        img.TypeAction = "";
                    }
                    _db.UpdateRange(imgTour);
                    _db.SaveChanges();
                }
            }
            catch (Exception e)
            { }
        }


        private void DelImageCreate(string idTour, string typeApprove)
        {
            try
            {
                var imgTour = from s in _db.Images
                              where s.TypeAction == "insert" &&
                                    s.IdService == idTour
                              select s;
                if (imgTour != null)
                {
                    if (typeApprove == "approve")
                    {
                        foreach (var img in imgTour)
                        {
                            img.TypeAction = "";
                        }
                        _db.UpdateRange(imgTour);
                        _db.SaveChanges();
                    }
                    else
                    {
                        _db.RemoveRange(imgTour);
                        _db.SaveChanges();
                    }
                }
            }
            catch (Exception e)
            { }
        }
    }
}
