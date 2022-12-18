using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using PrUtility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using static Travel.Shared.Ultilities.Enums;

namespace Travel.Data.Repositories
{

    public class ScheduleRes : ISchedule
    {
        private readonly TravelContext _db;
        private Notification message;
        private readonly ILog _log;
        private INotification _notification;
        private readonly ICache _cache;

        public ScheduleRes(TravelContext db, INotification notification, ILog log, ICache cache)
        {
            _db = db;
            _log = log;
            message = new Notification();
            _notification = notification;
            _cache = cache;
        }

        private Employee GetCurrentUser(Guid IdUserModify)
        {
            return (from x in _db.Employees.AsNoTracking()
                    where x.IdEmployee == IdUserModify
                    select x).FirstOrDefault();
        }

        private long GetDateTimeNow(int addMinutes = 0)
        {
            return Ultility.ConvertDatetimeToUnixTimeStampMiliSecond(DateTime.Now.AddMinutes(addMinutes));
        }
        private long GetDateTimeDayConfig(int addDay = 0)
        {
            return Ultility.ConvertDatetimeToUnixTimeStampMiliSecond(DateTime.Now.AddDays(addDay));
        }
        private void UpdateDatabase(Schedule input)
        {
            _db.Entry(input).State = EntityState.Modified;
        }

        private void UpdateDatabaseCostTour(CostTour input)
        {
            _db.Entry(input).State = EntityState.Modified;
        }

        private void DeleteDatabase(Schedule input)
        {
            _db.Entry(input).State = EntityState.Deleted;
        }
        private void DeleteDatabaseCostTour(CostTour input)
        {
            _db.Entry(input).State = EntityState.Deleted;
        }
        private void CreateDatabase(Schedule input)
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
        public string CheckBeforSave(JObject frmData, ref Notification _message, bool isUpdate)
        {
            try
            {
                var idSchedule = PrCommon.GetString("idSchedule", frmData);
                if (String.IsNullOrEmpty(idSchedule))
                {
                }
                #region check update when having tour booking
                if (isUpdate)
                {
                    if (CheckAnyBookingInSchedule(idSchedule))
                    {
                        _message = Ultility.Responses("Chuyến đi này đang có booking !", Enums.TypeCRUD.Warning.ToString()).Notification;
                        return null;
                    }
                }

                #endregion
                var tourId = PrCommon.GetString("tourId", frmData);

                if (String.IsNullOrEmpty(tourId))
                {
                }
                var carId = PrCommon.GetString("carId", frmData);
                if (String.IsNullOrEmpty(carId))
                {
                }
                var employeeId = PrCommon.GetString("employeeId", frmData);
                if (String.IsNullOrEmpty(employeeId))
                {
                }
                var promotionId = PrCommon.GetString("promotionId", frmData);
                if (String.IsNullOrEmpty(promotionId))
                {
                }
                var departurePlace = PrCommon.GetString("departurePlace", frmData);
                if (String.IsNullOrEmpty(departurePlace))
                {
                }
                var departureDate = PrCommon.GetString("departureDate", frmData);
                if (String.IsNullOrEmpty(departureDate))
                {
                }
                var returnDate = PrCommon.GetString("returnDate", frmData);
                if (String.IsNullOrEmpty(returnDate))
                {
                }
                var beginDate = PrCommon.GetString("beginDate", frmData);
                if (String.IsNullOrEmpty(beginDate))
                {
                }
                var endDate = PrCommon.GetString("endDate", frmData);
                if (String.IsNullOrEmpty(endDate))
                {
                }
                var timePromotion = PrCommon.GetString("timePromotion", frmData);
                if (String.IsNullOrEmpty(timePromotion))
                {
                }
                var description = PrCommon.GetString("description", frmData);
                if (String.IsNullOrEmpty(description))
                {
                }
                var vat = PrCommon.GetString("vat", frmData);
                if (String.IsNullOrEmpty(vat))
                {
                }
                var profit = PrCommon.GetString("profit", frmData);
                if (String.IsNullOrEmpty(vat))
                {
                }
                var minCapacity = PrCommon.GetString("minCapacity", frmData);
                if (String.IsNullOrEmpty(minCapacity))
                {
                }
                var maxCapacity = PrCommon.GetString("maxCapacity", frmData);
                if (String.IsNullOrEmpty(maxCapacity))
                {
                }

                var idUserModify = PrCommon.GetString("idUserModify", frmData);
                if (String.IsNullOrEmpty(idUserModify))
                {
                }
                var typeAction = PrCommon.GetString("typeAction", frmData);

                if (isUpdate)
                {
                    UpdateScheduleViewModel updateObj = new UpdateScheduleViewModel();
                    updateObj.TourId = tourId;
                    updateObj.CarId = Guid.Parse(carId);
                    updateObj.EmployeeId = Guid.Parse(employeeId);
                    updateObj.PromotionId = Convert.ToInt32(promotionId);
                    updateObj.Description = description;
                    updateObj.DeparturePlace = departurePlace;

                    updateObj.DepartureDate = long.Parse(departureDate);
                    updateObj.ReturnDate = long.Parse(returnDate);
                    updateObj.BeginDate = long.Parse(beginDate);
                    updateObj.EndDate = long.Parse(endDate);
                    updateObj.TimePromotion = long.Parse(timePromotion);

                    updateObj.MinCapacity = Convert.ToInt16(minCapacity);
                    updateObj.MaxCapacity = Convert.ToInt16(maxCapacity);
                    updateObj.IdSchedule = idSchedule;
                    updateObj.TypeAction = typeAction;
                    updateObj.Profit = int.Parse(profit);
                    updateObj.IdUserModify = Guid.Parse(idUserModify);
                    updateObj.PromotionId = Convert.ToInt32(promotionId);
                    // price 
                    updateObj.Vat = float.Parse(vat);
                    updateObj.Profit = int.Parse(profit);
                    return JsonSerializer.Serialize(updateObj);
                }
                CreateScheduleViewModel createObj = new CreateScheduleViewModel();
                createObj.TourId = tourId;
                createObj.CarId = Guid.Parse(carId);
                createObj.EmployeeId = Guid.Parse(employeeId);
                createObj.PromotionId = Convert.ToInt32(promotionId);
                createObj.Description = description;
                createObj.Vat = float.Parse(vat);
                createObj.Profit = int.Parse(profit);
                createObj.DeparturePlace = departurePlace;
                createObj.DepartureDate = long.Parse(departureDate);
                createObj.ReturnDate = long.Parse(returnDate);
                createObj.BeginDate = long.Parse(beginDate);
                createObj.EndDate = long.Parse(endDate);
                createObj.TimePromotion = long.Parse(timePromotion);
                createObj.MinCapacity = Convert.ToInt16(minCapacity);
                createObj.MaxCapacity = Convert.ToInt16(maxCapacity);
                createObj.Profit = int.Parse(profit);
                createObj.IdSchedule = $"{tourId}-S{Ultility.ConvertDatetimeToUnixTimeStampMiliSecond(DateTime.Now)}";
                createObj.IdUserModify = Guid.Parse(idUserModify);

                return JsonSerializer.Serialize(createObj);
            }
            catch (Exception e)
            {
                message.DateTime = DateTime.Now;
                message.Description = e.Message;
                message.Messenge = "Có lỗi xảy ra !";
                message.Type = "Error";
                _message = message;
                return null;
            }
        }

        public Response Gets()
        {
            try
            {
                var stopWatchEntitya1 = Stopwatch.StartNew();
                var schedule = (from x in _db.Schedules.AsNoTracking()
                                where x.IdSchedule == "1667699141567-x459298"
                                select x).FirstOrDefault();
                var b2 = schedule.Alias;

                schedule.Alias = "Ngay hom nay";
                _db.Entry(schedule).State = EntityState.Deleted;
                _db.SaveChanges();
                var dont1 = stopWatchEntitya1.Elapsed;

                var b = schedule.Alias;
                var c = 0;
                //var bab = _db.ChangeTracker.LazyLoadingEnabled;

                //_db.ChangeTracker.LazyLoadingEnabled = false;
                //var b32 = _db.ChangeTracker.LazyLoadingEnabled;






                //var ls11 = _db.Tour.ToList();
                //var ls121 = _db.Tour.AsNoTracking().ToList();
                //var ls1221 = (from x in _db.Tour select x).ToList();
                //var ls2 = (from x in _db.Schedules select x).AsNoTracking().OrderByDescending(x=> x.IdSchedule).ToList();

                //var kls2 = (from x in _db.Schedules select x).OrderByDescending(x => x.IdSchedule).ToList();


                //var kl3s2 = _db.Schedules.OrderByDescending(x => x.IdSchedule).ToList();


                //var ls = (from x in _db.Tour select x).ToList();

                //var kut = _db.Schedules;
                //var b = 1;






                //var ls31 = _db.Schedules.AsSingleQuery().ToList();
                //var ls3 = (from x in _db.Schedules select x).AsSingleQuery().ToList();
                //var stopWatch1 = Stopwatch.StartNew();
                //var ls331 = _db.Schedules.AsSplitQuery().ToList();
                //var b1 = stopWatch1.Elapsed;

                //var stopWatch2 = Stopwatch.StartNew();
                //var ls33 = (from x in _db.Schedules select x).AsSplitQuery().ToList();
                //var b2 = stopWatch2.Elapsed;
                //for (int i = 0; i < 10000; i++)
                //{
                //    var schedule = new Schedule();
                //    schedule.IdSchedule = $"{ Ultility.ConvertDatetimeToUnixTimeStampMiliSecond(DateTime.Now)}-{Ultility.GenerateRandomCode()}";
                //    schedule.AdditionalPrice = 0;
                //    schedule.IdUserModify = Guid.Empty;
                //    schedule.TourId = "DLNT-1666353913295";
                //    schedule.CarId = Guid.Parse("F9EB1352-3649-4AC3-A5AA-FE4FCFF0118A");
                //    schedule.EmployeeId = Guid.Parse("6E6C3C53-DFB9-4B69-B66E-2BE93C677DA7");
                //    schedule.PromotionId = 1;
                //    schedule.DepartureDate = 1;
                //    schedule.BeginDate = 1;
                //    schedule.EndDate = 1;
                //    schedule.Approve = 1;
                //    schedule.ReturnDate = 1;
                //    schedule.TimePromotion = 1;
                //    schedule.MinCapacity = 0;
                //    schedule.MaxCapacity = 0;
                //    schedule.QuantityAdult = 1;
                //    schedule.QuantityBaby = 1;
                //    schedule.QuantityChild = 1;
                //    _db.Schedules.Add(schedule);
                //}
                //_db.SaveChanges();


                //var stopWatchEntity1 = Stopwatch.StartNew();
                //var list42444 = _db.Schedules.Where(x => x.Isdelete == false &&
                //x.Approve == (int)Enums.ApproveStatus.Approved).Include(x => x.CostTour).Include(x => x.Timelines).Include(x => x.Promotions).Include(x => x.Tour).OrderBy(x => x.DepartureDate).AsNoTracking().ToList();
                //var entity1 = stopWatchEntity1.Elapsed;

                //var stopWatchLinq = Stopwatch.StartNew();
                var list = (from s in _db.Schedules.AsNoTracking()
                            where s.Isdelete == false &&
                            s.Approve == (int)Enums.ApproveStatus.Approved
                            select new Schedule
                            {
                                Alias = s.Alias,
                                Approve = s.Approve,
                                BeginDate = s.BeginDate,
                                QuantityAdult = s.QuantityAdult,
                                QuantityBaby = s.QuantityBaby,
                                QuantityChild = s.QuantityChild,
                                CarId = s.CarId,
                                Description = s.Description,
                                DepartureDate = s.DepartureDate,
                                ReturnDate = s.ReturnDate,
                                EndDate = s.EndDate,
                                Isdelete = s.Isdelete,
                                EmployeeId = s.EmployeeId,
                                IdSchedule = s.IdSchedule,
                                MaxCapacity = s.MaxCapacity,
                                MinCapacity = s.MinCapacity,
                                PromotionId = s.PromotionId,
                                DeparturePlace = s.DeparturePlace,
                                Status = s.Status,
                                TourId = s.TourId,
                                FinalPrice = s.FinalPrice,
                                FinalPriceHoliday = s.FinalPriceHoliday,
                                AdditionalPrice = s.AdditionalPrice,
                                AdditionalPriceHoliday = s.AdditionalPriceHoliday,
                                IsHoliday = s.IsHoliday,
                                Profit = s.Profit,
                                QuantityCustomer = s.QuantityCustomer,
                                TimePromotion = s.TimePromotion,
                                Vat = s.Vat,
                                TotalCostTourNotService = s.TotalCostTourNotService,
                                CostTour = (from c in _db.CostTours where c.IdSchedule == s.IdSchedule select c).FirstOrDefault(),
                                Timelines = (from t in _db.Timelines where t.IdSchedule == s.IdSchedule select t).ToList(),
                                Promotions = (from p in _db.Promotions where p.IdPromotion == s.PromotionId select p).FirstOrDefault(),
                                Tour = (from t in _db.Tour
                                        where s.TourId == t.IdTour
                                        select new Tour
                                        {
                                            Thumbnail = t.Thumbnail,
                                            ToPlace = t.ToPlace,
                                            IdTour = t.IdTour,
                                            NameTour = t.NameTour,
                                            Alias = t.Alias,
                                            ApproveStatus = t.ApproveStatus,
                                            CreateDate = t.CreateDate,
                                            IsActive = t.IsActive,
                                            IsDelete = t.IsDelete,
                                            ModifyBy = t.ModifyBy,
                                            ModifyDate = t.ModifyDate,
                                            Rating = t.Rating,
                                            Status = t.Status
                                        }).First(),

                            }).OrderBy(x => x.DepartureDate).ToList();
                //var linq = stopWatchLinq.Elapsed;

                //var stopWatchLinq1 = Stopwatch.StartNew();
                //var list2 = (from s in _db.Schedules
                //            where s.Isdelete == false &&
                //            s.Approve == (int)Enums.ApproveStatus.Approved
                //            select new Schedule
                //            {
                //                Alias = s.Alias,
                //                Approve = s.Approve,
                //                BeginDate = s.BeginDate,
                //                QuantityAdult = s.QuantityAdult,
                //                QuantityBaby = s.QuantityBaby,
                //                QuantityChild = s.QuantityChild,
                //                CarId = s.CarId,
                //                Description = s.Description,
                //                DepartureDate = s.DepartureDate,
                //                ReturnDate = s.ReturnDate,
                //                EndDate = s.EndDate,
                //                Isdelete = s.Isdelete,
                //                EmployeeId = s.EmployeeId,
                //                IdSchedule = s.IdSchedule,
                //                MaxCapacity = s.MaxCapacity,
                //                MinCapacity = s.MinCapacity,
                //                PromotionId = s.PromotionId,
                //                DeparturePlace = s.DeparturePlace,
                //                Status = s.Status,
                //                TourId = s.TourId,
                //                FinalPrice = s.FinalPrice,
                //                FinalPriceHoliday = s.FinalPriceHoliday,
                //                AdditionalPrice = s.AdditionalPrice,
                //                AdditionalPriceHoliday = s.AdditionalPriceHoliday,
                //                IsHoliday = s.IsHoliday,
                //                Profit = s.Profit,
                //                QuantityCustomer = s.QuantityCustomer,
                //                TimePromotion = s.TimePromotion,
                //                Vat = s.Vat,
                //                TotalCostTourNotService = s.TotalCostTourNotService,
                //                CostTour = (from c in _db.CostTours where c.IdSchedule == s.IdSchedule select c).FirstOrDefault(),
                //                Timelines = (from t in _db.Timelines where t.IdSchedule == s.IdSchedule select t).ToList(),
                //                Promotions = (from p in _db.Promotions where p.IdPromotion == s.PromotionId select p).FirstOrDefault(),
                //                Tour = (from t in _db.Tour
                //                        where s.TourId == t.IdTour
                //                        select new Tour
                //                        {
                //                            Thumbnail = t.Thumbnail,
                //                            ToPlace = t.ToPlace,
                //                            IdTour = t.IdTour,
                //                            NameTour = t.NameTour,
                //                            Alias = t.Alias,
                //                            ApproveStatus = t.ApproveStatus,
                //                            CreateDate = t.CreateDate,
                //                            IsActive = t.IsActive,
                //                            IsDelete = t.IsDelete,
                //                            ModifyBy = t.ModifyBy,
                //                            ModifyDate = t.ModifyDate,
                //                            Rating = t.Rating,
                //                            Status = t.Status
                //                        }).First(),

                //            }).OrderBy(x => x.DepartureDate).AsNoTracking().ToList();
                //var linq1 = stopWatchLinq1.Elapsed;

                //var stopWatchEntity = Stopwatch.StartNew();
                //var list4444 = _db.Schedules.Where(x => x.Isdelete == false &&
                //x.Approve == (int)Enums.ApproveStatus.Approved).Include(x => x.CostTour).Include(x => x.Timelines).Include(x => x.Promotions).Include(x => x.Tour).OrderBy(x => x.DepartureDate).AsNoTracking().ToList();
                //var entity = stopWatchEntity.Elapsed;



                // tét


                var stopWatchEntitya2 = Stopwatch.StartNew();
                var schedule1 = (from x in _db.Schedules.AsNoTracking()
                                 where x.IdSchedule == "DLNT-1666353913295-S1666417503679"
                                 select x).FirstOrDefault();
                var dont2 = stopWatchEntitya2.Elapsed;


                //var stopWatchEntitya1b = Stopwatch.StartNew();
                //var schedule1 = (from x in _db.Schedules
                //                where x.IdSchedule == "DLNT-1666353913295-S1666417503679"
                //                select x).FirstOrDefault();
                //var dont2 = stopWatchEntitya1b.Elapsed;


                // end tét
                return Ultility.Responses("", Enums.TypeCRUD.Success.ToString(), list);
            }
            catch (Exception e)
            {
                return Ultility.Responses("Có lỗi xảy ra !", Enums.TypeCRUD.Error.ToString(), description: e.Message);
            }
        }

        public Response Create(CreateScheduleViewModel input, string emailUser)
        {
            try
            {
                Schedule schedule =
                schedule = Mapper.MapCreateSchedule(input);
                string jsonContent = JsonSerializer.Serialize(schedule);
                var tour = (from x in _db.Tour.AsNoTracking()
                            where x.IdTour == input.TourId
                            select x).FirstOrDefault();
                schedule.Alias = $"S{tour.Alias}";
                schedule.TypeAction = "insert";
                var userLogin = (from x in _db.Employees.AsNoTracking()
                                 where x.IdEmployee == input.IdUserModify
                                 select x).FirstOrDefault();
                schedule.ModifyBy = userLogin.NameEmployee;
                schedule.ModifyDate = Ultility.ConvertDatetimeToUnixTimeStampMiliSecond(DateTime.Now);
                CreateDatabase(schedule);
                SaveChange();
                _cache.Remove("schedule");
                _cache.Remove("scheduleflashsale");
                var listRole = new int[] { Convert.ToInt16(Enums.TitleRole.Admin), Convert.ToInt16(Enums.TitleRole.LocalManager) };
                _notification.CreateNotification(userLogin.IdEmployee, Convert.ToInt16(Enums.TypeNotification.Schedule), schedule.IdSchedule, listRole, "");
                bool result = _log.AddLog(content: jsonContent, type: "create", emailCreator: emailUser, classContent: "Schedule");
                if (result)
                {
                    return Ultility.Responses("Thêm thành công !", Enums.TypeCRUD.Success.ToString(), schedule.IdSchedule);
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
        public Response GetsSchedulebyIdTour(string idTour, bool isDelete)
        {
            try
            {
                var list = (from s in _db.Schedules.AsNoTracking()
                            where s.TourId == idTour
                            && s.Isdelete == isDelete
                            && s.Approve == (int)Enums.ApproveStatus.Approved
                            && s.IsTempData == false
                            select new Schedule
                            {
                                Alias = s.Alias,
                                Approve = s.Approve,
                                BeginDate = s.BeginDate,
                                QuantityAdult = s.QuantityAdult,
                                QuantityBaby = s.QuantityBaby,
                                QuantityChild = s.QuantityChild,
                                CarId = s.CarId,
                                DepartureDate = s.DepartureDate,
                                ReturnDate = s.ReturnDate,
                                EndDate = s.EndDate,
                                DeparturePlace = s.DeparturePlace,
                                Description = s.Description,
                                MetaDesc = s.MetaDesc,
                                MetaKey = s.MetaKey,
                                Isdelete = s.Isdelete,
                                EmployeeId = s.EmployeeId,
                                IdSchedule = s.IdSchedule,
                                MaxCapacity = s.MaxCapacity,
                                MinCapacity = s.MinCapacity,
                                PromotionId = s.PromotionId,
                                Status = s.Status,
                                TourId = s.TourId,
                                FinalPrice = s.FinalPrice,
                                FinalPriceHoliday = s.FinalPriceHoliday,
                                AdditionalPrice = s.AdditionalPrice,
                                AdditionalPriceHoliday = s.AdditionalPriceHoliday,
                                IsHoliday = s.IsHoliday,
                                Profit = s.Profit,
                                QuantityCustomer = s.QuantityCustomer,
                                TimePromotion = s.TimePromotion,
                                Vat = s.Vat,
                                IdUserModify = s.IdUserModify,
                                TotalCostTourNotService = s.TotalCostTourNotService,
                                CostTour = (from c in _db.CostTours.AsNoTracking() where c.IdSchedule == s.IdSchedule select c).FirstOrDefault(),
                                Timelines = (from t in _db.Timelines.AsNoTracking() where t.IdSchedule == s.IdSchedule select t).ToList(),
                                Tour = (from t in _db.Tour.AsNoTracking()
                                        where s.TourId == t.IdTour
                                        select new Tour
                                        {
                                            Thumbnail = t.Thumbnail,
                                            ToPlace = t.ToPlace,
                                            IdTour = t.IdTour,
                                            NameTour = t.NameTour,
                                            Alias = t.Alias,
                                            ApproveStatus = t.ApproveStatus,
                                            CreateDate = t.CreateDate,
                                            IsActive = t.IsActive,
                                            IsDelete = t.IsDelete,
                                            ModifyBy = t.ModifyBy,
                                            ModifyDate = t.ModifyDate,
                                            Rating = t.Rating,
                                            Status = t.Status,
                                            QuantityBooked = t.QuantityBooked,
                                        }).FirstOrDefault(),

                            }).ToList();


                var result = Mapper.MapSchedule(list);
                return Ultility.Responses("", Enums.TypeCRUD.Success.ToString(), result);
            }
            catch (Exception e)
            {
                return Ultility.Responses("Có lỗi xảy ra !", Enums.TypeCRUD.Error.ToString(), description: e.Message);
            }
        }

        public Response CusGetsSchedulebyIdTour(string idTour   )
        {
            try
            {
                var dateTimeNow = GetDateTimeNow();
                var list = (from s in _db.Schedules.AsNoTracking()
                            where s.TourId == idTour
                            && s.Isdelete == false
                            && s.EndDate > dateTimeNow
                            && s.Status == (int)Enums.StatusSchedule.Free
                            && s.MaxCapacity > s.QuantityCustomer
                            && s.IsTempData == false
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
                            }).ToList();


                //var result = Mapper.MapSchedule(list);
                var result = list;
                return Ultility.Responses("", Enums.TypeCRUD.Success.ToString(), result);
            }
            catch (Exception e)
            {
                return Ultility.Responses("Có lỗi xảy ra !", Enums.TypeCRUD.Error.ToString(), description: e.Message);
            }
        }

        public Response GetSchedulebyIdTourWaiting(string idTour, Guid idUser, int pageIndex, int pageSize)
        {
            try
            {
                var totalResult = 0;
                var userLogin = (from x in _db.Employees.AsNoTracking()
                                 where x.IdEmployee == idUser
                                 select x).FirstOrDefault();
                var listWaiting = new List<Schedule>();
                if (userLogin.RoleId == (int)Enums.TitleRole.Admin)
                {
                    var querylistWaiting = (from s in _db.Schedules.AsNoTracking()
                                            where s.TourId == idTour
                                            && s.Isdelete == false &&
                                            s.Approve == (int)Enums.ApproveStatus.Waiting
                                            orderby s.BeginDate descending, s.DepartureDate descending

                                            select new Schedule
                                            {
                                                Alias = s.Alias,
                                                Approve = s.Approve,
                                                BeginDate = s.BeginDate,
                                                QuantityAdult = s.QuantityAdult,
                                                QuantityBaby = s.QuantityBaby,
                                                QuantityChild = s.QuantityChild,
                                                CarId = s.CarId,
                                                DepartureDate = s.DepartureDate,
                                                ReturnDate = s.ReturnDate,
                                                DeparturePlace = s.DeparturePlace,
                                                Description = s.Description,
                                                MetaDesc = s.MetaDesc,
                                                MetaKey = s.MetaKey,
                                                EndDate = s.EndDate,
                                                Isdelete = s.Isdelete,
                                                EmployeeId = s.EmployeeId,
                                                IdSchedule = s.IdSchedule,
                                                MaxCapacity = s.MaxCapacity,
                                                MinCapacity = s.MinCapacity,
                                                PromotionId = s.PromotionId,
                                                Status = s.Status,
                                                TourId = s.TourId,
                                                FinalPrice = s.FinalPrice,
                                                FinalPriceHoliday = s.FinalPriceHoliday,
                                                AdditionalPrice = s.AdditionalPrice,
                                                AdditionalPriceHoliday = s.AdditionalPriceHoliday,
                                                IsHoliday = s.IsHoliday,
                                                Profit = s.Profit,
                                                QuantityCustomer = s.QuantityCustomer,
                                                TimePromotion = s.TimePromotion,
                                                Vat = s.Vat,
                                                TotalCostTourNotService = s.TotalCostTourNotService,
                                                TypeAction = s.TypeAction,
                                                IdUserModify = s.IdUserModify,
                                                ModifyBy = s.ModifyBy,
                                                ModifyDate = s.ModifyDate,
                                                CostTour = (from c in _db.CostTours.AsNoTracking()
                                                            where c.IdSchedule == s.IdSchedule
                                                            select c).First(),
                                                Timelines = (from t in _db.Timelines.AsNoTracking()
                                                             where t.IdSchedule == s.IdSchedule
                                                             select t).ToList(),
                                                Promotions = (from p in _db.Promotions.AsNoTracking()
                                                              where p.IdPromotion == s.PromotionId
                                                              select p).First(),
                                                Tour = (from t in _db.Tour.AsNoTracking()
                                                        where s.TourId == t.IdTour
                                                        select new Tour
                                                        {
                                                            Thumbnail = t.Thumbnail,
                                                            ToPlace = t.ToPlace,
                                                            IdTour = t.IdTour,
                                                            NameTour = t.NameTour,
                                                            Alias = t.Alias,
                                                            ApproveStatus = t.ApproveStatus,
                                                            CreateDate = t.CreateDate,
                                                            IsActive = t.IsActive,
                                                            IsDelete = t.IsDelete,
                                                            ModifyBy = t.ModifyBy,
                                                            ModifyDate = t.ModifyDate,
                                                            Rating = t.Rating,
                                                            Status = t.Status,
                                                            QuantityBooked = t.QuantityBooked,
                                                        }).First(),

                                            });
                    totalResult = querylistWaiting.Count();
                    listWaiting = querylistWaiting.ToList();
                }
                else
                {
                    var querylistWaiting = (from s in _db.Schedules.AsNoTracking()
                                            where s.TourId == idTour && s.IdUserModify == idUser
                                            where s.Isdelete == false &&
                                            s.Approve == (int)Enums.ApproveStatus.Waiting
                                            orderby s.BeginDate descending, s.DepartureDate descending

                                            select new Schedule
                                            {
                                                Alias = s.Alias,
                                                Approve = s.Approve,
                                                BeginDate = s.BeginDate,
                                                QuantityAdult = s.QuantityAdult,
                                                QuantityBaby = s.QuantityBaby,
                                                QuantityChild = s.QuantityChild,
                                                CarId = s.CarId,
                                                DepartureDate = s.DepartureDate,
                                                ReturnDate = s.ReturnDate,
                                                DeparturePlace = s.DeparturePlace,
                                                Description = s.Description,
                                                MetaDesc = s.MetaDesc,
                                                MetaKey = s.MetaKey,
                                                EndDate = s.EndDate,
                                                Isdelete = s.Isdelete,
                                                EmployeeId = s.EmployeeId,
                                                IdSchedule = s.IdSchedule,
                                                MaxCapacity = s.MaxCapacity,
                                                MinCapacity = s.MinCapacity,
                                                PromotionId = s.PromotionId,
                                                Status = s.Status,
                                                TourId = s.TourId,
                                                FinalPrice = s.FinalPrice,
                                                FinalPriceHoliday = s.FinalPriceHoliday,
                                                AdditionalPrice = s.AdditionalPrice,
                                                AdditionalPriceHoliday = s.AdditionalPriceHoliday,
                                                IsHoliday = s.IsHoliday,
                                                Profit = s.Profit,
                                                QuantityCustomer = s.QuantityCustomer,
                                                TimePromotion = s.TimePromotion,
                                                Vat = s.Vat,
                                                TotalCostTourNotService = s.TotalCostTourNotService,
                                                TypeAction = s.TypeAction,
                                                IdUserModify = s.IdUserModify,
                                                ModifyBy = s.ModifyBy,
                                                ModifyDate = s.ModifyDate,
                                                CostTour = (from c in _db.CostTours.AsNoTracking()
                                                            where c.IdSchedule == s.IdSchedule
                                                            select c).First(),
                                                Timelines = (from t in _db.Timelines.AsNoTracking()
                                                             where t.IdSchedule == s.IdSchedule
                                                             select t).ToList(),
                                                Promotions = (from p in _db.Promotions.AsNoTracking()
                                                              where p.IdPromotion == s.PromotionId
                                                              select p).First(),
                                                Tour = (from t in _db.Tour.AsNoTracking()
                                                        where s.TourId == t.IdTour
                                                        select new Tour
                                                        {
                                                            Thumbnail = t.Thumbnail,
                                                            ToPlace = t.ToPlace,
                                                            IdTour = t.IdTour,
                                                            NameTour = t.NameTour,
                                                            Alias = t.Alias,
                                                            ApproveStatus = t.ApproveStatus,
                                                            CreateDate = t.CreateDate,
                                                            IsActive = t.IsActive,
                                                            IsDelete = t.IsDelete,
                                                            ModifyBy = t.ModifyBy,
                                                            ModifyDate = t.ModifyDate,
                                                            Rating = t.Rating,
                                                            Status = t.Status,
                                                            QuantityBooked = t.QuantityBooked,
                                                        }).First(),

                                            });
                    totalResult = querylistWaiting.Count();
                    listWaiting = querylistWaiting.ToList();
                }



                var result = listWaiting.Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();
                var res = Ultility.Responses("", Enums.TypeCRUD.Success.ToString(), result);
                res.TotalResult = totalResult;
                return res;
            }
            catch (Exception e)
            {
                return Ultility.Responses("Có lỗi xảy ra !", Enums.TypeCRUD.Error.ToString(), description: e.Message);
            }
        }

        public Response RestoreShedule(string idSchedule, Guid idUser, string emailUser)
        {
            try
            {
                var schedule = (from x in _db.Schedules.AsNoTracking()
                                where x.IdSchedule == idSchedule
                            && x.Isdelete == true
                                select x).FirstOrDefault();

                var userLogin = (from x in _db.Employees.AsNoTracking()
                                 where x.IdEmployee == idUser
                                 select x).FirstOrDefault();
                if (schedule != null)
                {
                    schedule.Isdelete = false;
                    schedule.IdUserModify = userLogin.IdEmployee;
                    schedule.Approve = (int)ApproveStatus.Waiting;
                    schedule.TypeAction = "restore";
                    string jsonContent = JsonSerializer.Serialize(schedule);

                    schedule.ModifyDate = Ultility.ConvertDatetimeToUnixTimeStampMiliSecond(DateTime.Now);
                    UpdateDatabase(schedule);
                    SaveChange();
                    _cache.Remove("schedule");
                    _cache.Remove("scheduleflashsale");
                    var listRole = new int[] { Convert.ToInt16(Enums.TitleRole.Admin), Convert.ToInt16(Enums.TitleRole.LocalManager) };
                    _notification.CreateNotification(userLogin.IdEmployee, Convert.ToInt16(Enums.TypeNotification.Schedule), schedule.IdSchedule, listRole, "");

                    bool result = _log.AddLog(content: jsonContent, type: "restore", emailCreator: emailUser, classContent: "promotion");
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


        public Response UpdatePromotion(string idSchedule, int idPromotion, string emailUser)
        {
            try
            {
                var schedule = (from x in _db.Schedules.AsNoTracking()
                                where x.IdSchedule == idSchedule
                                select x).FirstOrDefault();
                if (schedule != null)
                {

                    var promotion = (from x in _db.Promotions.AsNoTracking()
                                     where x.IdPromotion == idPromotion
                                     select x).FirstOrDefault();
                    if (promotion != null)
                    {


                        schedule.PromotionId = promotion.IdPromotion;
                        schedule.TimePromotion = promotion.ToDate;

                        UpdateDatabase(schedule);
                        SaveChange();
                        _cache.Remove("schedule");
                        _cache.Remove("scheduleflashsale");
                    }
                    string jsonContent = JsonSerializer.Serialize(promotion);
                    bool result = _log.AddLog(content: jsonContent, type: "update", emailCreator: emailUser, classContent: "promotion");
                    if (result)
                    {
                        return Ultility.Responses("Cập nhật thành công !", Enums.TypeCRUD.Success.ToString());

                    }
                    else
                    {
                        return Ultility.Responses("Lỗi log!", Enums.TypeCRUD.Error.ToString());
                    }
                }
                else
                {
                    return Ultility.Responses($"Không tìm thấy Id [{idSchedule}] !", Enums.TypeCRUD.Warning.ToString());
                }
            }
            catch (Exception e)
            {
                return Ultility.Responses("Có lỗi xảy ra !", Enums.TypeCRUD.Error.ToString(), description: e.Message);
            }
        }


        public async Task UpdateCapacity(string idSchedule, int adult = 1, int child = 0, int baby = 0)
        {
            try
            {
                var schedule = await (from x in _db.Schedules.AsNoTracking()
                                      where x.IdSchedule == idSchedule
                                      select x).FirstOrDefaultAsync();
                int availableQuantity = schedule.QuantityCustomer;
                int quantity = availableQuantity + (adult + child);

                schedule.QuantityAdult = adult;
                schedule.QuantityBaby = baby;
                schedule.QuantityChild = child;
                schedule.QuantityCustomer = quantity;
                UpdateDatabase(schedule);
                await SaveChangeAsync();

            }
            catch (Exception e)
            {
            }
        }

        public async Task<bool> CheckEmptyCapacity(string idSchedule, int adult, int child, int baby)
        {
            int cusRemain = 0;
            try
            {
                var schedule = await (from x in _db.Schedules.AsNoTracking()
                                where x.IdSchedule == idSchedule
                                select x).FirstOrDefaultAsync();
                int availableQuantity = schedule.QuantityCustomer;

                cusRemain = schedule.MaxCapacity - schedule.QuantityCustomer;
                int quantityCus = adult + child;

                if (quantityCus <= cusRemain)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                return false;
            }
        }


        public async Task<Response> Get(string idSchedule)
        {
            try
            {
                var dateTimeNow = GetDateTimeNow();
                int approve = Convert.ToInt16(Enums.ApproveStatus.Approved);
                var schedule = await (from x in _db.Schedules.AsNoTracking()
                                      where x.EndDate > dateTimeNow
                                      && x.Isdelete == false
                                      && x.Approve == approve
                                      && x.IdSchedule == idSchedule
                                      && x.IsTempData == false
                                      && x.MaxCapacity > x.QuantityCustomer

                                      select new Schedule
                                      {
                                          IdSchedule = x.IdSchedule,
                                          MinCapacity = x.MinCapacity,
                                          MaxCapacity = x.MaxCapacity,
                                          QuantityCustomer = x.QuantityCustomer,
                                          AdditionalPrice = x.AdditionalPrice,
                                          AdditionalPriceHoliday = x.AdditionalPriceHoliday,
                                          Alias = x.Alias,
                                          FinalPrice = x.FinalPrice,
                                          FinalPriceHoliday = x.FinalPriceHoliday,
                                          PriceAdult = x.PriceAdult,
                                          PriceBaby = x.PriceBaby,
                                          PriceChild = x.PriceChild,
                                          PriceAdultHoliday = x.PriceAdultHoliday,
                                          PriceBabyHoliday = x.PriceBabyHoliday,
                                          PriceChildHoliday = x.PriceChildHoliday,
                                          QuantityAdult = x.QuantityAdult,
                                          QuantityBaby = x.QuantityBaby,
                                          QuantityChild = x.QuantityChild,
                                          BeginDate = x.BeginDate,
                                          EndDate = x.EndDate,
                                          DepartureDate = x.DepartureDate,
                                          DeparturePlace = x.DeparturePlace,
                                          ReturnDate = x.ReturnDate,
                                          Description = x.Description,
                                          IsHoliday = x.IsHoliday,
                                          Promotions = (from p in _db.Promotions.AsNoTracking()
                                                        where p.IdPromotion == x.PromotionId
                                                        select p).FirstOrDefault(),
                                          CostTour = (from c in _db.CostTours.AsNoTracking()
                                                      where c.IdSchedule == x.IdSchedule
                                                      select c).FirstOrDefault(),
                                          Timelines = (from t in _db.Timelines.AsNoTracking()
                                                       where t.IdSchedule == x.IdSchedule
                                                       select t).ToList(),
                                          Tour = (from tour in _db.Tour.AsNoTracking()
                                                  where x.TourId == tour.IdTour
                                                  select tour).FirstOrDefault()
                                      }).FirstAsync();
                return Ultility.Responses("", Enums.TypeCRUD.Success.ToString(), schedule);
            }
            catch (Exception e)
            {
                return Ultility.Responses("Có lỗi xảy ra !", Enums.TypeCRUD.Error.ToString(), description: e.Message);
            }
        }

        public async Task<Response> SearchTour(string from, string to, DateTime? departureDate, DateTime? returnDate)
        {
            try
            {
                var dateTimeNow = GetDateTimeNow();
                if (departureDate != null && returnDate != null)
                {
                    var list = await (from x in _db.Schedules
                                      where x.EndDate > dateTimeNow
                                      && x.Isdelete == false
                                       && x.MaxCapacity > x.QuantityCustomer

                                      && x.Approve == (int)Enums.ApproveStatus.Approved
                                      select x
                                      ).ToListAsync();
                    if (departureDate != null)
                    {
                        long unixDepartureDate = Ultility.ConvertDatetimeToUnixTimeStampMiliSecond(departureDate.Value);
                        list = (from x in list
                                where x.DepartureDate >= unixDepartureDate
                                select x).ToList();
                    }
                    if (returnDate != null)
                    {
                        long unixReturnDate = Ultility.ConvertDatetimeToUnixTimeStampMiliSecond(returnDate.Value);
                        list = (from x in list
                                where x.DepartureDate <= unixReturnDate
                                select x).ToList();
                    }
                    if (!string.IsNullOrEmpty(from))
                    {
                        string keyFrom = Ultility.removeVietnameseSign(from.ToLower());
                        list = (from x in list
                                where Ultility.removeVietnameseSign(x.DeparturePlace.ToLower()).Contains(keyFrom)
                                select x).ToList();
                    }
                    list = (from s in list
                            select new Schedule
                            {
                                Alias = s.Alias,
                                Approve = s.Approve,
                                BeginDate = s.BeginDate,
                                QuantityAdult = s.QuantityAdult,
                                QuantityBaby = s.QuantityBaby,
                                QuantityChild = s.QuantityChild,
                                CarId = s.CarId,
                                Description = s.Description,
                                DepartureDate = s.DepartureDate,
                                ReturnDate = s.ReturnDate,
                                EndDate = s.EndDate,
                                Isdelete = s.Isdelete,
                                EmployeeId = s.EmployeeId,
                                IdSchedule = s.IdSchedule,
                                MaxCapacity = s.MaxCapacity,
                                MinCapacity = s.MinCapacity,
                                PromotionId = s.PromotionId,
                                DeparturePlace = s.DeparturePlace,
                                Status = s.Status,
                                TourId = s.TourId,
                                FinalPrice = s.FinalPrice,
                                FinalPriceHoliday = s.FinalPriceHoliday,
                                AdditionalPrice = s.AdditionalPrice,
                                AdditionalPriceHoliday = s.AdditionalPriceHoliday,
                                IsHoliday = s.IsHoliday,
                                Profit = s.Profit,
                                QuantityCustomer = s.QuantityCustomer,
                                TimePromotion = s.TimePromotion,
                                Vat = s.Vat,
                                TotalCostTourNotService = s.TotalCostTourNotService,
                                Promotions = (from p in _db.Promotions where p.IdPromotion == s.PromotionId select p).FirstOrDefault(),
                                CostTour = (from c in _db.CostTours where c.IdSchedule == s.IdSchedule select c).FirstOrDefault(),
                                Timelines = (from t in _db.Timelines where t.IdSchedule == s.IdSchedule select t).ToList(),
                                Tour = (from t in _db.Tour
                                        where s.TourId == t.IdTour
                                        select new Tour
                                        {
                                            Thumbnail = t.Thumbnail,
                                            ToPlace = t.ToPlace,
                                            IdTour = t.IdTour,
                                            NameTour = t.NameTour,
                                            Alias = t.Alias,
                                            ApproveStatus = t.ApproveStatus,
                                            CreateDate = t.CreateDate,
                                            IsActive = t.IsActive,
                                            IsDelete = t.IsDelete,
                                            ModifyBy = t.ModifyBy,
                                            ModifyDate = t.ModifyDate,
                                            Rating = t.Rating,
                                            Status = t.Status
                                        }).FirstOrDefault(),

                            }).ToList();
                    if (!string.IsNullOrEmpty(to))
                    {
                        string keyTo = Ultility.removeVietnameseSign(to.ToLower());
                        list = (from x in list
                                where Ultility.removeVietnameseSign(x.Tour.ToPlace.ToLower()).Contains(keyTo)
                                select x).OrderByDescending(x => x.DepartureDate).ToList();
                    }
                    var result = list;
                    return Ultility.Responses("", Enums.TypeCRUD.Success.ToString(), result);
                }
                else if (departureDate == null && returnDate == null)
                {

                    var list2 = await (from x in _db.Schedules
                                       where x.EndDate > dateTimeNow
                                       && x.Isdelete == false
                                      && x.Approve == (int)Enums.ApproveStatus.Approved
                                       select x
                                     ).ToListAsync();

                    if (!string.IsNullOrEmpty(from))
                    {
                        string keyFrom = Ultility.removeVietnameseSign(from.ToLower());
                        list2 = (from x in list2
                                 where Ultility.removeVietnameseSign(x.DeparturePlace.ToLower()).Contains(keyFrom)
                                 select x).ToList();
                    }
                    list2 = (from s in list2
                             select new Schedule
                             {
                                 Alias = s.Alias,
                                 Approve = s.Approve,
                                 BeginDate = s.BeginDate,
                                 QuantityAdult = s.QuantityAdult,
                                 QuantityBaby = s.QuantityBaby,
                                 QuantityChild = s.QuantityChild,
                                 CarId = s.CarId,
                                 Description = s.Description,
                                 DepartureDate = s.DepartureDate,
                                 ReturnDate = s.ReturnDate,
                                 EndDate = s.EndDate,
                                 Isdelete = s.Isdelete,
                                 EmployeeId = s.EmployeeId,
                                 IdSchedule = s.IdSchedule,
                                 MaxCapacity = s.MaxCapacity,
                                 MinCapacity = s.MinCapacity,
                                 PromotionId = s.PromotionId,
                                 DeparturePlace = s.DeparturePlace,
                                 Status = s.Status,
                                 TourId = s.TourId,
                                 FinalPrice = s.FinalPrice,
                                 FinalPriceHoliday = s.FinalPriceHoliday,
                                 AdditionalPrice = s.AdditionalPrice,
                                 AdditionalPriceHoliday = s.AdditionalPriceHoliday,
                                 IsHoliday = s.IsHoliday,
                                 Profit = s.Profit,
                                 QuantityCustomer = s.QuantityCustomer,
                                 TimePromotion = s.TimePromotion,
                                 Vat = s.Vat,
                                 TotalCostTourNotService = s.TotalCostTourNotService,
                                 Promotions = (from p in _db.Promotions where p.IdPromotion == s.PromotionId select p).FirstOrDefault(),
                                 CostTour = (from c in _db.CostTours where c.IdSchedule == s.IdSchedule select c).FirstOrDefault(),
                                 Timelines = (from t in _db.Timelines where t.IdSchedule == s.IdSchedule select t).ToList(),
                                 Tour = (from t in _db.Tour
                                         where s.TourId == t.IdTour
                                         select new Tour
                                         {
                                             Thumbnail = t.Thumbnail,
                                             ToPlace = t.ToPlace,
                                             IdTour = t.IdTour,
                                             NameTour = t.NameTour,
                                             Alias = t.Alias,
                                             ApproveStatus = t.ApproveStatus,
                                             CreateDate = t.CreateDate,
                                             IsActive = t.IsActive,
                                             IsDelete = t.IsDelete,
                                             ModifyBy = t.ModifyBy,
                                             ModifyDate = t.ModifyDate,
                                             Rating = t.Rating,
                                             Status = t.Status
                                         }).FirstOrDefault(),

                             }).ToList();
                    if (!string.IsNullOrEmpty(to))
                    {
                        string keyTo = Ultility.removeVietnameseSign(to.ToLower());
                        list2 = (from x in list2
                                 where Ultility.removeVietnameseSign(x.Tour.ToPlace.ToLower()).Contains(keyTo)
                                 select x).OrderByDescending(x => x.DepartureDate).ToList();
                    }
                    var result = list2;
                    return Ultility.Responses("", Enums.TypeCRUD.Success.ToString(), result);

                }
                else
                {
                    var list1 = new List<Schedule>();
                    if (departureDate != null)
                    {
                        var fromDepartTureDate1 = Ultility.ConvertDatetimeToUnixTimeStampMiliSecond(departureDate.Value);
                        var toDepartTureDate1 = Ultility.ConvertDatetimeToUnixTimeStampMiliSecond(departureDate.Value.AddDays(1).AddMinutes(-1));
                        // cách 1
                        list1 = await (from x in _db.Schedules
                                       where x.EndDate > dateTimeNow
                                        && x.DepartureDate >= fromDepartTureDate1
                                       && x.DepartureDate <= toDepartTureDate1
                                       && x.Isdelete == false
                                       && x.Approve == (int)Enums.ApproveStatus.Approved
                                       select x
                                      ).ToListAsync();
                        // cách 2 
                        //list1 = await (from x in _db.Schedules
                        //               where x.EndDate <= dateTimeNowUnix1
                        //               && (x.DepartureDate >= fromDepartTureDate1 && x.DepartureDate <= toDepartTureDate1)
                        //               && x.Isdelete == false
                        //               && x.Approve == (int)Enums.ApproveStatus.Approved
                        //               select x
                        //              ).ToListAsync();
                    }
                    else
                    {
                        var fromReturnDate1 = Ultility.ConvertDatetimeToUnixTimeStampMiliSecond(returnDate.Value);
                        var toReturnDate1 = Ultility.ConvertDatetimeToUnixTimeStampMiliSecond(returnDate.Value.AddDays(1).AddMinutes(-1));
                        list1 = await (from x in _db.Schedules
                                       where x.EndDate > dateTimeNow
                                       && x.ReturnDate >= fromReturnDate1
                                       && x.ReturnDate <= toReturnDate1
                                       && x.Isdelete == false
                                       && x.Approve == (int)Enums.ApproveStatus.Approved
                                       select x
                                                             ).ToListAsync();
                    }


                    if (!string.IsNullOrEmpty(from))
                    {
                        string keyFrom = Ultility.removeVietnameseSign(from.ToLower());
                        list1 = (from x in list1
                                 where Ultility.removeVietnameseSign(x.DeparturePlace.ToLower()).Contains(keyFrom)
                                 select x).ToList();
                    }
                    list1 = (from s in list1
                             select new Schedule
                             {
                                 Alias = s.Alias,
                                 Approve = s.Approve,
                                 BeginDate = s.BeginDate,
                                 QuantityAdult = s.QuantityAdult,
                                 QuantityBaby = s.QuantityBaby,
                                 QuantityChild = s.QuantityChild,
                                 CarId = s.CarId,
                                 Description = s.Description,
                                 DepartureDate = s.DepartureDate,
                                 ReturnDate = s.ReturnDate,
                                 EndDate = s.EndDate,
                                 Isdelete = s.Isdelete,
                                 EmployeeId = s.EmployeeId,
                                 IdSchedule = s.IdSchedule,
                                 MaxCapacity = s.MaxCapacity,
                                 MinCapacity = s.MinCapacity,
                                 PromotionId = s.PromotionId,
                                 DeparturePlace = s.DeparturePlace,
                                 Status = s.Status,
                                 TourId = s.TourId,
                                 FinalPrice = s.FinalPrice,
                                 FinalPriceHoliday = s.FinalPriceHoliday,
                                 AdditionalPrice = s.AdditionalPrice,
                                 AdditionalPriceHoliday = s.AdditionalPriceHoliday,
                                 IsHoliday = s.IsHoliday,
                                 Profit = s.Profit,
                                 QuantityCustomer = s.QuantityCustomer,
                                 TimePromotion = s.TimePromotion,
                                 Vat = s.Vat,
                                 TotalCostTourNotService = s.TotalCostTourNotService,
                                 Promotions = (from p in _db.Promotions where p.IdPromotion == s.PromotionId select p).FirstOrDefault(),
                                 CostTour = (from c in _db.CostTours where c.IdSchedule == s.IdSchedule select c).FirstOrDefault(),
                                 Timelines = (from t in _db.Timelines where t.IdSchedule == s.IdSchedule select t).ToList(),
                                 Tour = (from t in _db.Tour
                                         where s.TourId == t.IdTour
                                         select new Tour
                                         {
                                             Thumbnail = t.Thumbnail,
                                             ToPlace = t.ToPlace,
                                             IdTour = t.IdTour,
                                             NameTour = t.NameTour,
                                             Alias = t.Alias,
                                             ApproveStatus = t.ApproveStatus,
                                             CreateDate = t.CreateDate,
                                             IsActive = t.IsActive,
                                             IsDelete = t.IsDelete,
                                             ModifyBy = t.ModifyBy,
                                             ModifyDate = t.ModifyDate,
                                             Rating = t.Rating,
                                             Status = t.Status
                                         }).FirstOrDefault(),

                             }).ToList();
                    if (!string.IsNullOrEmpty(to))
                    {
                        string keyTo = Ultility.removeVietnameseSign(to.ToLower());
                        list1 = (from x in list1
                                 where Ultility.removeVietnameseSign(x.Tour.ToPlace.ToLower()).Contains(keyTo)
                                 select x).OrderByDescending(x => x.DepartureDate).ToList();
                    }
                    var result1 = list1;
                    return Ultility.Responses("", Enums.TypeCRUD.Success.ToString(), result1);
                }
                //var list = await (from s in _db.Schedules
                //            where s.Isdelete == false
                //            && s.Approve == (int)Enums.ApproveStatus.Approved
                //            select new Schedule
                //            {
                //                Alias = s.Alias,
                //                Approve = s.Approve,
                //                BeginDate = s.BeginDate,
                //                QuantityAdult = s.QuantityAdult,
                //                QuantityBaby = s.QuantityBaby,
                //                QuantityChild = s.QuantityChild,
                //                CarId = s.CarId,
                //                DepartureDate = s.DepartureDate,
                //                ReturnDate = s.ReturnDate,
                //                DeparturePlace = s.DeparturePlace,
                //                Description = s.Description,
                //                MetaDesc = s.MetaDesc,
                //                MetaKey = s.MetaKey,
                //                EndDate = s.EndDate,
                //                Isdelete = s.Isdelete,
                //                EmployeeId = s.EmployeeId,
                //                IdSchedule = s.IdSchedule,
                //                MaxCapacity = s.MaxCapacity,
                //                MinCapacity = s.MinCapacity,
                //                PromotionId = s.PromotionId,
                //                Status = s.Status,
                //                TourId = s.TourId,
                //                FinalPrice = s.FinalPrice,
                //                FinalPriceHoliday = s.FinalPriceHoliday,
                //                AdditionalPrice = s.AdditionalPrice,
                //                AdditionalPriceHoliday = s.AdditionalPriceHoliday,
                //                IsHoliday = s.IsHoliday,
                //                Profit = s.Profit,
                //                QuantityCustomer = s.QuantityCustomer,
                //                TimePromotion = s.TimePromotion,
                //                Vat = s.Vat,
                //                TotalCostTourNotService = s.TotalCostTourNotService,
                //                Promotions = (from p in _db.Promotions where p.IdPromotion == s.PromotionId select p).FirstOrDefault(),
                //                CostTour = (from c in _db.CostTours where c.IdSchedule == s.IdSchedule select c).First(),
                //                Timelines = (from t in _db.Timelines where t.IdSchedule == s.IdSchedule select t).ToList(),
                //                Tour = (from t in _db.Tour
                //                        where s.TourId == t.IdTour
                //                        select new Tour
                //                        {
                //                            Thumbsnail = t.Thumbsnail,
                //                            ToPlace = t.ToPlace,
                //                            IdTour = t.IdTour,
                //                            NameTour = t.NameTour,
                //                            Alias = t.Alias,
                //                            ApproveStatus = t.ApproveStatus,
                //                            CreateDate = t.CreateDate,
                //                            IsActive = t.IsActive,
                //                            IsDelete = t.IsDelete,
                //                            ModifyBy = t.ModifyBy,
                //                            ModifyDate = t.ModifyDate,
                //                            Rating = t.Rating,
                //                            Status = t.Status,
                //                            QuantityBooked = t.QuantityBooked,
                //                        }).First(),

                //            }).ToListAsync();

            }
            catch (Exception e)
            {
                return Ultility.Responses("Có lỗi xảy ra !", Enums.TypeCRUD.Error.ToString(), description: e.Message);
            }
        }

        public async Task<Response> GetsSchedule(int pageIndex, int pageSize)
        {
            try
            {
                #region check cache
                //if (_cache.Get<Response>($"schedule") != null) // có cache
                //{
                //    return _cache.Get<Response>($"schedule");
                //}
                #endregion

                var dateTimeNow = GetDateTimeNow();
                var list = (from s in _db.Schedules.AsNoTracking()
                            where s.Isdelete == false &&
                      s.Approve == (int)Enums.ApproveStatus.Approved
                      && s.PromotionId == 1
                      && s.EndDate >= dateTimeNow
                      && s.BeginDate <= dateTimeNow
                                                  && s.MaxCapacity > s.QuantityCustomer

                            select new Schedule
                            {
                                Alias = s.Alias,
                                Approve = s.Approve,
                                BeginDate = s.BeginDate,
                                QuantityAdult = s.QuantityAdult,
                                QuantityBaby = s.QuantityBaby,
                                QuantityChild = s.QuantityChild,
                                CarId = s.CarId,
                                Description = s.Description,
                                DepartureDate = s.DepartureDate,
                                ReturnDate = s.ReturnDate,
                                EndDate = s.EndDate,
                                Isdelete = s.Isdelete,
                                EmployeeId = s.EmployeeId,
                                IdSchedule = s.IdSchedule,
                                MaxCapacity = s.MaxCapacity,
                                MinCapacity = s.MinCapacity,
                                PromotionId = s.PromotionId,
                                DeparturePlace = s.DeparturePlace,
                                Status = s.Status,
                                TourId = s.TourId,
                                FinalPrice = s.FinalPrice,
                                FinalPriceHoliday = s.FinalPriceHoliday,
                                AdditionalPrice = s.AdditionalPrice,
                                AdditionalPriceHoliday = s.AdditionalPriceHoliday,
                                IsHoliday = s.IsHoliday,
                                Profit = s.Profit,
                                QuantityCustomer = s.QuantityCustomer,
                                TimePromotion = s.TimePromotion,
                                Vat = s.Vat,
                                TotalCostTourNotService = s.TotalCostTourNotService,
                                CostTour = (from c in _db.CostTours.AsNoTracking() where c.IdSchedule == s.IdSchedule select c).FirstOrDefault(),
                                Timelines = (from t in _db.Timelines.AsNoTracking() where t.IdSchedule == s.IdSchedule select t).ToList(),
                                Promotions = (from p in _db.Promotions.AsNoTracking() where p.IdPromotion == s.PromotionId select p).FirstOrDefault(),
                                Tour = (from t in _db.Tour.AsNoTracking()
                                        where s.TourId == t.IdTour
                                        select new Tour
                                        {
                                            Thumbnail = t.Thumbnail,
                                            ToPlace = t.ToPlace,
                                            IdTour = t.IdTour,
                                            NameTour = t.NameTour,
                                            Alias = t.Alias,
                                            ApproveStatus = t.ApproveStatus,
                                            CreateDate = t.CreateDate,
                                            IsActive = t.IsActive,
                                            IsDelete = t.IsDelete,
                                            ModifyBy = t.ModifyBy,
                                            ModifyDate = t.ModifyDate,
                                            Rating = t.Rating,
                                            Status = t.Status
                                        }).First(),

                            }).OrderBy(x => x.DepartureDate);

                var lis = await list.ToListAsync();
                var totalREsult = await list.CountAsync();
                var listResult = await list.Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToListAsync();

                var result = Mapper.MapSchedule(listResult);
                var res = Ultility.Responses("", Enums.TypeCRUD.Success.ToString(), result);
                _cache.Set(res, $"schedule");
                res.TotalResult = result.Count();
                return res;
            }
            catch (Exception e)
            {
                return Ultility.Responses("Có lỗi xảy ra !", Enums.TypeCRUD.Error.ToString(), description: e.Message);

            }
        }

        public async Task<Response> GetsScheduleFlashSale(int pageIndex, int pageSize)
        {
            try
            {
                #region check cache
                if (_cache.Get<Response>($"scheduleflashsale") != null) // có cache
                {
                    return _cache.Get<Response>($"scheduleflashsale");
                }
                #endregion
                var dateTimeNow = GetDateTimeNow();
                var flashSaleDay = Ultility.ConvertDatetimeToUnixTimeStampMiliSecond(Ultility.GetDateZeroTime(DateTime.Now.AddDays(3))); // sau này gắn config
                var list = await (from s in _db.Schedules.AsNoTracking()
                                  where s.Isdelete == false
                                  && s.Approve == (int)Enums.ApproveStatus.Approved
                                  && s.EndDate >= dateTimeNow
                                  && s.EndDate <= flashSaleDay
                                  && s.BeginDate <= dateTimeNow
                                                              && s.MaxCapacity > s.QuantityCustomer

                                  select new Schedule
                                  {
                                      Alias = s.Alias,
                                      Approve = s.Approve,
                                      BeginDate = s.BeginDate,
                                      QuantityAdult = s.QuantityAdult,
                                      QuantityBaby = s.QuantityBaby,
                                      QuantityChild = s.QuantityChild,
                                      CarId = s.CarId,
                                      Description = s.Description,
                                      DepartureDate = s.DepartureDate,
                                      ReturnDate = s.ReturnDate,
                                      EndDate = s.EndDate,
                                      Isdelete = s.Isdelete,
                                      EmployeeId = s.EmployeeId,
                                      IdSchedule = s.IdSchedule,
                                      MaxCapacity = s.MaxCapacity,
                                      MinCapacity = s.MinCapacity,
                                      PromotionId = s.PromotionId,
                                      DeparturePlace = s.DeparturePlace,
                                      Status = s.Status,
                                      TourId = s.TourId,
                                      FinalPrice = s.FinalPrice,
                                      FinalPriceHoliday = s.FinalPriceHoliday,
                                      AdditionalPrice = s.AdditionalPrice,
                                      AdditionalPriceHoliday = s.AdditionalPriceHoliday,
                                      IsHoliday = s.IsHoliday,
                                      Profit = s.Profit,
                                      QuantityCustomer = s.QuantityCustomer,
                                      TimePromotion = s.TimePromotion,
                                      Vat = s.Vat,
                                      TotalCostTourNotService = s.TotalCostTourNotService,
                                      CostTour = (from c in _db.CostTours.AsNoTracking() where c.IdSchedule == s.IdSchedule select c).FirstOrDefault(),
                                      Timelines = (from t in _db.Timelines.AsNoTracking() where t.IdSchedule == s.IdSchedule select t).ToList(),
                                      Promotions = (from p in _db.Promotions.AsNoTracking() where p.IdPromotion == s.PromotionId select p).FirstOrDefault(),
                                      Tour = (from t in _db.Tour.AsNoTracking()
                                              where s.TourId == t.IdTour
                                              select new Tour
                                              {
                                                  Thumbnail = t.Thumbnail,
                                                  ToPlace = t.ToPlace,
                                                  IdTour = t.IdTour,
                                                  NameTour = t.NameTour,
                                                  Alias = t.Alias,
                                                  ApproveStatus = t.ApproveStatus,
                                                  CreateDate = t.CreateDate,
                                                  IsActive = t.IsActive,
                                                  IsDelete = t.IsDelete,
                                                  ModifyBy = t.ModifyBy,
                                                  ModifyDate = t.ModifyDate,
                                                  Rating = t.Rating,
                                                  Status = t.Status
                                              }).First(),

                                  }).OrderBy(x => x.DepartureDate).ToListAsync();


                var result = Mapper.MapSchedule(list).Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();
                var res = Ultility.Responses("", Enums.TypeCRUD.Success.ToString(), result);
                _cache.Set(res, $"scheduleflashsale");
                res.TotalResult = result.Count();
                return res;

            }
            catch (Exception e)
            {
                return Ultility.Responses("Có lỗi xảy ra !", Enums.TypeCRUD.Error.ToString(), description: e.Message);
            }
        }

        public async Task<Response> GetsSchedulePromotion(int pageIndex, int pageSize)
        {
            try
            {
                var dateTimeNow = GetDateTimeNow();
                var list = await (from s in _db.Schedules.AsNoTracking()
                                  where s.Isdelete == false &&
                                  s.Approve == (int)Enums.ApproveStatus.Approved
                                  && s.PromotionId > 1
                                  && s.EndDate >= dateTimeNow
                                  && s.BeginDate <= dateTimeNow
                                  && s.MaxCapacity > s.QuantityCustomer

                                  select new Schedule
                                  {
                                      Alias = s.Alias,
                                      Approve = s.Approve,
                                      BeginDate = s.BeginDate,
                                      QuantityAdult = s.QuantityAdult,
                                      QuantityBaby = s.QuantityBaby,
                                      QuantityChild = s.QuantityChild,
                                      CarId = s.CarId,
                                      Description = s.Description,
                                      DepartureDate = s.DepartureDate,
                                      ReturnDate = s.ReturnDate,
                                      EndDate = s.EndDate,
                                      Isdelete = s.Isdelete,
                                      EmployeeId = s.EmployeeId,
                                      IdSchedule = s.IdSchedule,
                                      MaxCapacity = s.MaxCapacity,
                                      MinCapacity = s.MinCapacity,
                                      PromotionId = s.PromotionId,
                                      DeparturePlace = s.DeparturePlace,
                                      Status = s.Status,
                                      TourId = s.TourId,
                                      FinalPrice = s.FinalPrice,
                                      FinalPriceHoliday = s.FinalPriceHoliday,
                                      AdditionalPrice = s.AdditionalPrice,
                                      AdditionalPriceHoliday = s.AdditionalPriceHoliday,
                                      IsHoliday = s.IsHoliday,
                                      Profit = s.Profit,
                                      QuantityCustomer = s.QuantityCustomer,
                                      TimePromotion = s.TimePromotion,
                                      Vat = s.Vat,
                                      TotalCostTourNotService = s.TotalCostTourNotService,
                                      CostTour = (from c in _db.CostTours.AsNoTracking() where c.IdSchedule == s.IdSchedule select c).FirstOrDefault(),
                                      Timelines = (from t in _db.Timelines.AsNoTracking() where t.IdSchedule == s.IdSchedule select t).ToList(),
                                      Promotions = (from p in _db.Promotions.AsNoTracking() where p.IdPromotion == s.PromotionId select p).FirstOrDefault(),
                                      Tour = (from t in _db.Tour.AsNoTracking()
                                              where s.TourId == t.IdTour
                                              select new Tour
                                              {
                                                  Thumbnail = t.Thumbnail,
                                                  ToPlace = t.ToPlace,
                                                  IdTour = t.IdTour,
                                                  NameTour = t.NameTour,
                                                  Alias = t.Alias,
                                                  ApproveStatus = t.ApproveStatus,
                                                  CreateDate = t.CreateDate,
                                                  IsActive = t.IsActive,
                                                  IsDelete = t.IsDelete,
                                                  ModifyBy = t.ModifyBy,
                                                  ModifyDate = t.ModifyDate,
                                                  Rating = t.Rating,
                                                  Status = t.Status
                                              }).First(),

                                  }).OrderBy(x => x.DepartureDate).ToListAsync();


                var result = Mapper.MapSchedule(list).Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();
                var res = Ultility.Responses("", Enums.TypeCRUD.Success.ToString(), result);
                res.TotalResult = result.Count();
                return res;
            }
            catch (Exception e)
            {
                return Ultility.Responses("Có lỗi xảy ra !", Enums.TypeCRUD.Error.ToString(), description: e.Message);

            }

        }
        public async Task<Response> GetsRelatedSchedule(string idSchedule, int pageIndex, int pageSize)
        {
            try
            {
                var dateTimeNow = GetDateTimeNow();
                var schedule = await (from x in _db.Schedules.AsNoTracking()
                                      where x.IdSchedule == idSchedule
                                                                  && x.MaxCapacity > x.QuantityCustomer

                                      select x).FirstOrDefaultAsync();
                var closetPrice1 = (schedule.FinalPrice - 200000);
                var closetPrice2 = (schedule.FinalPrice + 200000);
                var list1 = await (from x in _db.Schedules.AsNoTracking()
                                   where x.IdSchedule != idSchedule
                                   && x.EndDate >= dateTimeNow
                                   && x.BeginDate <= dateTimeNow
                                   && x.DeparturePlace == schedule.DeparturePlace
                                   && (x.FinalPrice >= closetPrice1 && x.FinalPrice <= closetPrice2)
                                   && x.Isdelete == false
                                   && x.Approve == (int)Enums.ApproveStatus.Approved
                                   && x.IsTempData == false
                                   select x).ToListAsync();
                var list2 = await (from x in _db.Schedules.AsNoTracking()
                                   where x.IdSchedule != idSchedule
                                   && !(from s in list1 select s.IdSchedule).Contains(x.IdSchedule)
                                   && x.EndDate >= dateTimeNow
                                   && x.BeginDate <= dateTimeNow
                                   && x.DeparturePlace == schedule.DeparturePlace
                                   && (x.Status == (int)StatusSchedule.Free && x.QuantityCustomer <= x.MinCapacity)
                                   && x.Isdelete == false
                                   && x.Approve == (int)Enums.ApproveStatus.Approved
                                   && x.IsTempData == false
                                   select x).OrderBy(x => x.BeginDate).ToListAsync();
                var rd = new Random();
                var lsFinal = list1.Concat(list2).ToList();
                lsFinal = lsFinal.Shuffle(rd);

                var list = (from s in lsFinal
                            select new Schedule
                            {
                                Alias = s.Alias,
                                Approve = s.Approve,
                                BeginDate = s.BeginDate,
                                QuantityAdult = s.QuantityAdult,
                                QuantityBaby = s.QuantityBaby,
                                QuantityChild = s.QuantityChild,
                                CarId = s.CarId,
                                Description = s.Description,
                                DepartureDate = s.DepartureDate,
                                ReturnDate = s.ReturnDate,
                                EndDate = s.EndDate,
                                Isdelete = s.Isdelete,
                                EmployeeId = s.EmployeeId,
                                IdSchedule = s.IdSchedule,
                                MaxCapacity = s.MaxCapacity,
                                MinCapacity = s.MinCapacity,
                                PromotionId = s.PromotionId,
                                DeparturePlace = s.DeparturePlace,
                                Status = s.Status,
                                TourId = s.TourId,
                                FinalPrice = s.FinalPrice,
                                FinalPriceHoliday = s.FinalPriceHoliday,
                                AdditionalPrice = s.AdditionalPrice,
                                AdditionalPriceHoliday = s.AdditionalPriceHoliday,
                                IsHoliday = s.IsHoliday,
                                Profit = s.Profit,
                                QuantityCustomer = s.QuantityCustomer,
                                TimePromotion = s.TimePromotion,
                                Vat = s.Vat,
                                TotalCostTourNotService = s.TotalCostTourNotService,
                                CostTour = (from c in _db.CostTours.AsNoTracking() where c.IdSchedule == s.IdSchedule select c).FirstOrDefault(),
                                Timelines = (from t in _db.Timelines.AsNoTracking() where t.IdSchedule == s.IdSchedule select t).ToList(),
                                Promotions = (from p in _db.Promotions.AsNoTracking() where p.IdPromotion == s.PromotionId select p).FirstOrDefault(),
                                Tour = (from t in _db.Tour.AsNoTracking()
                                        where s.TourId == t.IdTour
                                        select new Tour
                                        {
                                            Thumbnail = t.Thumbnail,
                                            ToPlace = t.ToPlace,
                                            IdTour = t.IdTour,
                                            NameTour = t.NameTour,
                                            Alias = t.Alias,
                                            ApproveStatus = t.ApproveStatus,
                                            CreateDate = t.CreateDate,
                                            IsActive = t.IsActive,
                                            IsDelete = t.IsDelete,
                                            ModifyBy = t.ModifyBy,
                                            ModifyDate = t.ModifyDate,
                                            Rating = t.Rating,
                                            Status = t.Status
                                        }).First(),

                            }).OrderBy(x => x.DepartureDate).ToList();


                var result = Mapper.MapSchedule(list).Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();
                var res = Ultility.Responses("", Enums.TypeCRUD.Success.ToString(), result);
                res.TotalResult = result.Count();
                return res;

            }
            catch (Exception e)
            {
                return Ultility.Responses("Có lỗi xảy ra !", Enums.TypeCRUD.Error.ToString(), description: e.Message);
            }
        }


        #region dang chỉnh
        public Response Delete(string idSchedule, Guid idUser, string emailUser)
        {
            try
            {
                var schedule = (from x in _db.Schedules.AsNoTracking()
                                where x.IdSchedule == idSchedule
                                select x).FirstOrDefault();

                var costTour = (from x in _db.CostTours.AsNoTracking()
                                where x.IdSchedule == idSchedule
                                select x).FirstOrDefault();

                var timelines = (from x in _db.Timelines
                                 where x.IdSchedule == idSchedule
                                 select x).ToList();

                var userLogin = (from x in _db.Employees.AsNoTracking()
                                 where x.IdEmployee == idUser
                                 select x).FirstOrDefault();

                if (schedule.Approve == (int)ApproveStatus.Approved)
                {
                    schedule.IdUserModify = userLogin.IdEmployee;
                    schedule.Approve = (int)ApproveStatus.Waiting;
                    schedule.ModifyBy = userLogin.NameEmployee;
                    schedule.TypeAction = "delete";
                    string jsonContent = JsonSerializer.Serialize(schedule);
                    schedule.ModifyDate = GetDateTimeNow(0);
                    UpdateDatabase(schedule);
                    SaveChange();

                    var listRole = new int[] { Convert.ToInt16(Enums.TitleRole.Admin), Convert.ToInt16(Enums.TitleRole.LocalManager) };
                    _notification.CreateNotification(userLogin.IdEmployee, Convert.ToInt16(Enums.TypeNotification.Schedule), schedule.IdSchedule, listRole, "");
                    bool result = _log.AddLog(content: jsonContent, type: "delete", emailCreator: emailUser, classContent: "Schedule");
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
                    if (schedule.IdUserModify == idUser)
                    {
                        if (schedule.TypeAction == "insert")
                        {
                            if (timelines.Count > 0)
                            {
                                _db.RemoveRange(timelines);
                                SaveChange();
                            }
                            DeleteDatabase(schedule);
                            SaveChange();
                            return Ultility.Responses("Đã xóa!", Enums.TypeCRUD.Success.ToString());
                        }
                        else if (schedule.TypeAction == "update")
                        {
                            var idScheduleTemp = schedule.IdAction;
                            // old hotel

                            var scheduleTemp = (from x in _db.Schedules
                                                where x.IdSchedule == idScheduleTemp
                                                select x).FirstOrDefault();

                            var CostTourTemp = (from x in _db.CostTours.AsNoTracking()
                                                where x.IdSchedule == idScheduleTemp
                                                select x).FirstOrDefault();

                            var TimelineTemp = (from x in _db.Timelines
                                                where x.IdSchedule == idScheduleTemp
                                                select x).ToList();

                            schedule.Approve = (int)ApproveStatus.Approved;
                            schedule.IdAction = null;
                            schedule.TypeAction = null;

                            costTour.Approve = (int)ApproveStatus.Approved;
                            costTour.TypeAction = null;

                            #region restore data

                            schedule.CarId = scheduleTemp.CarId;
                            schedule.DepartureDate = scheduleTemp.DepartureDate;
                            schedule.DeparturePlace = scheduleTemp.DeparturePlace;
                            schedule.Description = scheduleTemp.Description;
                            schedule.EmployeeId = scheduleTemp.EmployeeId;
                            schedule.BeginDate = scheduleTemp.EndDate;
                            schedule.EndDate = scheduleTemp.EndDate;
                            schedule.IsHoliday = scheduleTemp.IsHoliday;
                            schedule.MaxCapacity = scheduleTemp.MaxCapacity;
                            schedule.MinCapacity = scheduleTemp.MinCapacity;
                            schedule.ModifyBy = userLogin.NameEmployee;
                            schedule.PromotionId = scheduleTemp.PromotionId;
                            schedule.ReturnDate = scheduleTemp.ReturnDate;
                            schedule.Vat = scheduleTemp.Vat;
                            schedule.Profit = scheduleTemp.Profit;
                            schedule.TimePromotion = scheduleTemp.TimePromotion;
                            schedule.FinalPrice = scheduleTemp.FinalPrice;
                            schedule.FinalPriceHoliday = scheduleTemp.FinalPriceHoliday;
                            schedule.PriceAdult = scheduleTemp.PriceAdult;
                            schedule.PriceChild = scheduleTemp.PriceChild;
                            schedule.PriceBaby = scheduleTemp.PriceBaby;
                            schedule.PriceAdultHoliday = scheduleTemp.PriceAdultHoliday;
                            schedule.PriceChildHoliday = scheduleTemp.PriceChildHoliday;
                            schedule.PriceBabyHoliday = scheduleTemp.PriceBabyHoliday;

                            costTour.Breakfast = CostTourTemp.Breakfast;
                            costTour.Water = CostTourTemp.Water;
                            costTour.FeeGas = CostTourTemp.FeeGas;
                            costTour.Distance = CostTourTemp.Distance;
                            costTour.SellCost = CostTourTemp.SellCost;
                            costTour.Depreciation = CostTourTemp.Depreciation;
                            costTour.OtherPrice = CostTourTemp.OtherPrice;
                            costTour.Tolls = CostTourTemp.Tolls;
                            costTour.CusExpected = CostTourTemp.CusExpected;
                            costTour.InsuranceFee = CostTourTemp.InsuranceFee;
                            costTour.IsHoliday = CostTourTemp.IsHoliday;
                            costTour.TotalCostTourNotService = CostTourTemp.TotalCostTourNotService;
                            costTour.HotelId = CostTourTemp.HotelId;
                            costTour.PriceHotelDB = CostTourTemp.PriceHotelDB;
                            costTour.PriceHotelSR = CostTourTemp.PriceHotelSR;
                            costTour.RestaurantId = CostTourTemp.RestaurantId;
                            costTour.PriceRestaurant = CostTourTemp.PriceRestaurant;
                            costTour.PlaceId = CostTourTemp.PlaceId;
                            costTour.PriceTicketPlace = CostTourTemp.PriceTicketPlace;



                            foreach (var item in TimelineTemp)
                            {
                                item.IdSchedule = idSchedule;
                            }

                            _db.Timelines.RemoveRange(timelines);
                            _db.Timelines.UpdateRange(TimelineTemp);

                            #endregion
                            DeleteDatabase(scheduleTemp);
                            UpdateDatabase(schedule);

                            DeleteDatabaseCostTour(CostTourTemp);
                            UpdateDatabaseCostTour(costTour);

                            SaveChange();

                            return Ultility.Responses("Đã hủy yêu cầu chỉnh sửa !", Enums.TypeCRUD.Success.ToString());
                        }
                        else if (schedule.TypeAction == "restore")
                        {
                            schedule.IdAction = null;
                            schedule.TypeAction = null;
                            schedule.Isdelete = true;
                            schedule.Approve = (int)ApproveStatus.Approved;


                            costTour.TypeAction = null;
                            costTour.Approve = (int)ApproveStatus.Refused;

                            UpdateDatabase(schedule);
                            UpdateDatabaseCostTour(costTour);
                            SaveChange();

                            return Ultility.Responses("Đã hủy yêu cầu khôi phục!", Enums.TypeCRUD.Success.ToString());

                        }
                        else // delete
                        {
                            schedule.IdAction = null;
                            schedule.TypeAction = null;
                            schedule.Isdelete = false;
                            schedule.Approve = (int)ApproveStatus.Approved;

                            costTour.TypeAction = null;
                            costTour.Approve = (int)ApproveStatus.Refused;

                            UpdateDatabase(schedule);
                            UpdateDatabaseCostTour(costTour);
                            SaveChange();
                            return Ultility.Responses("Đã hủy yêu cầu xóa !", Enums.TypeCRUD.Success.ToString());
                        }
                    }

                    return Ultility.Responses("", Enums.TypeCRUD.Success.ToString());
                }


            }
            catch (Exception e)
            {
                return Ultility.Responses("Có lỗi xảy ra !", Enums.TypeCRUD.Error.ToString(), description: e.Message);

            }
        }
        public Response Approve(string idSchedule)
        {
            try
            {
                var schedule = (from x in _db.Schedules.AsNoTracking()
                                where x.IdSchedule == idSchedule
                                && x.Approve == (int)ApproveStatus.Waiting
                                select x).FirstOrDefault();

                var costTour = (from x in _db.CostTours.AsNoTracking()
                                where x.IdSchedule == idSchedule
                                && x.Approve == (int)ApproveStatus.Waiting
                                select x).FirstOrDefault();

                if (schedule != null)
                {


                    if (schedule.TypeAction == "update")
                    {
                        var idScheduleTemp = schedule.IdAction;
                        schedule.Approve = (int)ApproveStatus.Approved;
                        schedule.IdAction = null;
                        schedule.TypeAction = null;

                        costTour.Approve = (int)ApproveStatus.Approved;
                        costTour.TypeAction = null;

                        // delete tempdata
                        var scheduleTemp = (from x in _db.Schedules.AsNoTracking()
                                            where x.IdSchedule == idScheduleTemp
                                            select x).FirstOrDefault();
                        DeleteDatabase(scheduleTemp);

                        var CostTourTemp = (from x in _db.CostTours.AsNoTracking()
                                            where x.IdSchedule == idScheduleTemp
                                            select x).FirstOrDefault();
                        DeleteDatabaseCostTour(CostTourTemp);

                        var TimlineTemp = (from x in _db.Timelines.AsNoTracking()
                                           where x.IdSchedule == idScheduleTemp
                                           select x).ToList();
                        _db.Timelines.RemoveRange(TimlineTemp);
                    }
                    else if (schedule.TypeAction == "insert")
                    {
                        schedule.IdAction = null;
                        schedule.TypeAction = null;
                        schedule.Approve = (int)ApproveStatus.Approved;

                        costTour.TypeAction = null;
                        costTour.Approve = (int)ApproveStatus.Approved;
                    }
                    else if (schedule.TypeAction == "restore")
                    {
                        schedule.IdAction = null;
                        schedule.TypeAction = null;
                        schedule.Approve = (int)ApproveStatus.Approved;
                        schedule.Isdelete = false;

                        costTour.TypeAction = null;
                        costTour.Approve = (int)ApproveStatus.Approved;
                    }
                    else
                    {
                        schedule.IdAction = null;
                        schedule.TypeAction = null;
                        schedule.Approve = (int)ApproveStatus.Approved;
                        schedule.Isdelete = true;

                        costTour.TypeAction = null;
                        costTour.Approve = (int)ApproveStatus.Approved;
                    }
                    UpdateDatabase(schedule);
                    UpdateDatabaseCostTour(costTour);
                    SaveChange();
                    _cache.Remove("schedule");
                    _cache.Remove("scheduleflashsale");
                    var userModify = GetCurrentUser(schedule.IdUserModify);
                    _notification.CreateNotification(userModify.IdEmployee, Convert.ToInt16(Enums.TypeNotification.Schedule), schedule.IdSchedule, new int[] { userModify.RoleId }, "Thành công");

                    return Ultility.Responses($"Duyệt thành công !", Enums.TypeCRUD.Success.ToString());
                }
                else
                {
                    return Ultility.Responses("Không tìm thấy dữ liệu !", Enums.TypeCRUD.Warning.ToString());

                }
            }
            catch (Exception e)
            {
                return Ultility.Responses("Có lỗi xảy ra !", Enums.TypeCRUD.Error.ToString(), description: e.Message);
            }
        }
        public Response Refused(string idSchedule)
        {
            try
            {
                var schedule = (from x in _db.Schedules.AsNoTracking()
                                where x.IdSchedule == idSchedule
                                && x.Approve == (int)ApproveStatus.Waiting
                                select x).FirstOrDefault();
                var costTour = (from x in _db.CostTours.AsNoTracking()
                                where x.IdSchedule == idSchedule
                                && x.Approve == (int)ApproveStatus.Waiting
                                select x).FirstOrDefault();

                var timelines = (from x in _db.Timelines
                                 where x.IdSchedule == idSchedule
                                 select x).ToList();

                if (schedule != null)
                {
                    if (schedule.TypeAction == "update")
                    {
                        var idScheduleTemp = schedule.IdAction;
                        // old hotel
                        var scheduleTemp = (from x in _db.Schedules.AsNoTracking()
                                            where x.IdSchedule == idScheduleTemp
                                            && x.IsTempData == true
                                            select x).FirstOrDefault();

                        var CostTourTemp = (from x in _db.CostTours.AsNoTracking()
                                            where x.IdSchedule == idScheduleTemp
                                            select x).FirstOrDefault();

                        var TimelineTemp = (from x in _db.Timelines
                                            where x.IdSchedule == idScheduleTemp
                                            select x).ToList();

                        schedule.Approve = (int)ApproveStatus.Approved;
                        schedule.IdAction = null;
                        schedule.TypeAction = null;

                        costTour.Approve = (int)ApproveStatus.Approved;
                        costTour.TypeAction = null;


                        #region restore data

                        schedule.CarId = scheduleTemp.CarId;
                        schedule.DepartureDate = scheduleTemp.DepartureDate;
                        schedule.DeparturePlace = scheduleTemp.DeparturePlace;
                        schedule.Description = scheduleTemp.Description;
                        schedule.EmployeeId = scheduleTemp.EmployeeId;
                        schedule.BeginDate = scheduleTemp.EndDate;
                        schedule.EndDate = scheduleTemp.EndDate;
                        schedule.IsHoliday = scheduleTemp.IsHoliday;
                        schedule.MaxCapacity = scheduleTemp.MaxCapacity;
                        schedule.MinCapacity = scheduleTemp.MinCapacity;
                        schedule.PromotionId = scheduleTemp.PromotionId;
                        schedule.ReturnDate = scheduleTemp.ReturnDate;
                        schedule.Vat = scheduleTemp.Vat;
                        schedule.Profit = scheduleTemp.Profit;
                        schedule.TimePromotion = scheduleTemp.TimePromotion;
                        schedule.FinalPrice = scheduleTemp.FinalPrice;
                        schedule.FinalPriceHoliday = scheduleTemp.FinalPriceHoliday;
                        schedule.PriceAdult = scheduleTemp.PriceAdult;
                        schedule.PriceChild = scheduleTemp.PriceChild;
                        schedule.PriceBaby = scheduleTemp.PriceBaby;
                        schedule.PriceAdultHoliday = scheduleTemp.PriceAdultHoliday;
                        schedule.PriceChildHoliday = scheduleTemp.PriceChildHoliday;
                        schedule.PriceBabyHoliday = scheduleTemp.PriceBabyHoliday;


                        costTour.Breakfast = CostTourTemp.Breakfast;
                        costTour.Water = CostTourTemp.Water;
                        costTour.FeeGas = CostTourTemp.FeeGas;
                        costTour.Distance = CostTourTemp.Distance;
                        costTour.SellCost = CostTourTemp.SellCost;
                        costTour.Depreciation = CostTourTemp.Depreciation;
                        costTour.OtherPrice = CostTourTemp.OtherPrice;
                        costTour.Tolls = CostTourTemp.Tolls;
                        costTour.CusExpected = CostTourTemp.CusExpected;
                        costTour.InsuranceFee = CostTourTemp.InsuranceFee;
                        costTour.IsHoliday = CostTourTemp.IsHoliday;
                        costTour.TotalCostTourNotService = CostTourTemp.TotalCostTourNotService;
                        costTour.HotelId = CostTourTemp.HotelId;
                        costTour.PriceHotelDB = CostTourTemp.PriceHotelDB;
                        costTour.PriceHotelSR = CostTourTemp.PriceHotelSR;
                        costTour.RestaurantId = CostTourTemp.RestaurantId;
                        costTour.PriceRestaurant = CostTourTemp.PriceRestaurant;
                        costTour.PlaceId = CostTourTemp.PlaceId;
                        costTour.PriceTicketPlace = CostTourTemp.PriceTicketPlace;

                        foreach (var item in TimelineTemp)
                        {
                            item.IdSchedule = idSchedule;
                        }

                        _db.Timelines.RemoveRange(timelines);
                        _db.Timelines.UpdateRange(TimelineTemp);
                        #endregion

                        DeleteDatabase(scheduleTemp);
                        DeleteDatabaseCostTour(CostTourTemp);
                    }
                    else if (schedule.TypeAction == "insert")
                    {
                        _db.RemoveRange(timelines);

                        schedule.IdAction = null;
                        schedule.TypeAction = null;
                        schedule.Approve = (int)ApproveStatus.Refused;

                        costTour.TypeAction = null;
                        costTour.Approve = (int)ApproveStatus.Refused;
                    }
                    else if (schedule.TypeAction == "restore")
                    {
                        schedule.IdAction = null;
                        schedule.TypeAction = null;
                        schedule.Isdelete = true;
                        schedule.Approve = (int)ApproveStatus.Approved;

                        costTour.TypeAction = null;
                        costTour.Approve = (int)ApproveStatus.Approved;
                    }
                    else // delete
                    {
                        schedule.IdAction = null;
                        schedule.TypeAction = null;
                        schedule.Isdelete = false;
                        schedule.Approve = (int)ApproveStatus.Approved;

                        costTour.TypeAction = null;
                        costTour.Approve = (int)ApproveStatus.Approved;
                    }
                    UpdateDatabase(schedule);
                    UpdateDatabaseCostTour(costTour);
                    SaveChange();
                    _cache.Remove("schedule");
                    _cache.Remove("scheduleflashsale");
                    var userModify = GetCurrentUser(schedule.IdUserModify);
                    _notification.CreateNotification(userModify.IdEmployee, Convert.ToInt16(Enums.TypeNotification.Schedule), schedule.IdSchedule, new int[] { userModify.RoleId }, "Từ chối");


                    return Ultility.Responses($"Từ chối thành công !", Enums.TypeCRUD.Success.ToString());
                }
                else
                {
                    return Ultility.Responses("Không tìm thấy dữ liệu !", Enums.TypeCRUD.Warning.ToString());

                }
            }
            catch (Exception e)
            {
                return Ultility.Responses("Có lỗi xảy ra !", Enums.TypeCRUD.Error.ToString(), description: e.Message);
            }
        }
        public Response Update(UpdateScheduleViewModel input, string emailUser)
        {
            try
            {
                var userLogin = (from x in _db.Employees.AsNoTracking()
                                 where x.IdEmployee == input.IdUserModify
                                 select x).FirstOrDefault();

                var schedule = (from x in _db.Schedules.AsNoTracking()
                                where x.IdSchedule == input.IdSchedule
                                select x).FirstOrDefault();

                // clone new object
                var scheduleOld = new Schedule();
                scheduleOld = Ultility.DeepCopy<Schedule>(schedule);
                scheduleOld.IdAction = scheduleOld.IdSchedule.ToString();
                scheduleOld.IdSchedule = $"{Ultility.ConvertDatetimeToUnixTimeStampMiliSecond(DateTime.Now)}Temp";
                scheduleOld.IsTempData = true;
                string jsonContent = JsonSerializer.Serialize(schedule);

                CreateDatabase(scheduleOld);

                #region setdata
                schedule.IdAction = scheduleOld.IdSchedule.ToString();
                schedule.IdUserModify = input.IdUserModify;
                schedule.ModifyBy = userLogin.NameEmployee;

                schedule.Approve = (int)ApproveStatus.Waiting;
                schedule.TypeAction = "update";
                schedule.BeginDate = input.BeginDate;
                schedule.CarId = input.CarId;
                schedule.DepartureDate = input.DepartureDate;
                schedule.DeparturePlace = input.DeparturePlace;
                schedule.Description = input.Description;
                schedule.EmployeeId = input.EmployeeId;
                schedule.PromotionId = input.PromotionId;
                schedule.TimePromotion = input.TimePromotion;
                schedule.EndDate = input.EndDate;
                schedule.IsHoliday = input.IsHoliday;
                schedule.MaxCapacity = input.MaxCapacity;
                schedule.MinCapacity = input.MinCapacity;
                schedule.ReturnDate = input.ReturnDate;
                schedule.Vat = input.Vat;
                schedule.Profit = input.Profit;
                schedule.ModifyBy = userLogin.NameEmployee;
                schedule.ModifyDate = Ultility.ConvertDatetimeToUnixTimeStampMiliSecond(DateTime.Now);
                #endregion

                UpdateDatabase(schedule);
                SaveChange();
                _cache.Remove("schedule");
                _cache.Remove("scheduleflashsale");
                var listRole = new int[] { Convert.ToInt16(Enums.TitleRole.Admin), Convert.ToInt16(Enums.TitleRole.LocalManager) };
                _notification.CreateNotification(userLogin.IdEmployee, Convert.ToInt16(Enums.TypeNotification.Schedule), schedule.IdSchedule, listRole, "");
                bool result = _log.AddLog(content: jsonContent, type: "update", emailCreator: emailUser, classContent: "Schedule");
                if (result)
                {
                    return Ultility.Responses("Đã gửi yêu cầu sửa !", Enums.TypeCRUD.Success.ToString(), scheduleOld.IdSchedule);
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
        #endregion
        private bool CheckAnyBookingInSchedule(string idSchedule) // chỉ dùng khi thay đổi thông tin tour
        {
            // cách 1
            var scheduleInTour = (from x in _db.Schedules.AsNoTracking()
                                  where x.IdSchedule == idSchedule
                                  && x.QuantityCustomer == 0
                                  && x.Isdelete == false
                                  && x.Status == (int)Enums.StatusSchedule.Free
                                  && x.Approve == (int)Enums.ApproveStatus.Approved
                                  select x).FirstOrDefault();
            if (scheduleInTour != null) // có dữ liệu, tức là ko có tour
            {
                return false;
            }
            else
            {
                return true;
            }
        }


        public Response SearchSchedule(JObject frmData, string idTour)
        {
            try
            {
                var totalResult = 0;
                Keywords keywords = new Keywords();
                var pageSize = PrCommon.GetString("pageSize", frmData) == null ? 10 : Convert.ToInt16(PrCommon.GetString("pageSize", frmData));
                var pageIndex = PrCommon.GetString("pageIndex", frmData) == null ? 1 : Convert.ToInt16(PrCommon.GetString("pageIndex", frmData));

                if (!String.IsNullOrEmpty(idTour))
                {
                    keywords.KwIdTour = idTour;
                }

                var isDelete = PrCommon.GetString("isDelete", frmData);
                if (!String.IsNullOrEmpty(isDelete))
                {
                    keywords.IsDelete = Boolean.Parse(isDelete);
                }

                var kwIdSchedule = PrCommon.GetString("idSchedule", frmData);
                if (!String.IsNullOrEmpty(kwIdSchedule))
                {
                    keywords.KwId = kwIdSchedule.Trim().ToLower();
                }
                else
                {
                    keywords.KwId = "";
                }


                var kwBeginDate = PrCommon.GetString("beginDateFrom", frmData);
                if (!String.IsNullOrEmpty(kwBeginDate))
                {
                    keywords.KwBeginDate = Ultility.ConvertDatetimeToUnixTimeStampMiliSecond(DateTime.Parse(kwBeginDate));
                }
                else
                {
                    keywords.KwBeginDate = 0;
                }

                var kwEndDate = PrCommon.GetString("beginDateTo", frmData);
                if (!String.IsNullOrEmpty(kwEndDate))
                {
                    keywords.KwEndDate = Ultility.ConvertDatetimeToUnixTimeStampMiliSecond(DateTime.Parse(kwEndDate).AddDays(1).AddSeconds(-1));
                }
                else
                {
                    keywords.KwEndDate = 0;
                }

                var kwdepartureDate = PrCommon.GetString("departureDateFrom", frmData);
                if (!String.IsNullOrEmpty(kwdepartureDate))
                {
                    keywords.KwDepartureDate = Ultility.ConvertDatetimeToUnixTimeStampMiliSecond(DateTime.Parse(kwdepartureDate));
                }
                else
                {
                    keywords.KwDepartureDate = 0;
                }

                var kwReturnDate = PrCommon.GetString("departureDateTo", frmData);
                if (!String.IsNullOrEmpty(kwReturnDate))
                {
                    keywords.KwReturnDate = Ultility.ConvertDatetimeToUnixTimeStampMiliSecond(DateTime.Parse(kwReturnDate).AddDays(1).AddSeconds(-1));
                }
                else
                {
                    keywords.KwReturnDate = 0;
                }


                var kwTotalCostTourNotSvc = PrCommon.GetString("TotalCostTourNotService", frmData);
                if (!String.IsNullOrEmpty(kwTotalCostTourNotSvc))
                {
                    keywords.KwTotalCostTourNotService = float.Parse(kwTotalCostTourNotSvc);
                }
                else
                {
                    keywords.KwTotalCostTourNotService = 0;
                }

                var kwFinalPrice = PrCommon.GetString("finalPrice", frmData);
                if (!String.IsNullOrEmpty(kwFinalPrice))
                {
                    keywords.KwFinalPrice = float.Parse(kwFinalPrice);
                }
                else
                {
                    keywords.KwFinalPrice = 0;
                }

                var kwFinalPriceHoliday = PrCommon.GetString("finalPriceHoliday", frmData);
                if (!String.IsNullOrEmpty(kwFinalPriceHoliday))
                {
                    keywords.KwFinalPriceHoliday = float.Parse(kwFinalPriceHoliday);
                }
                else
                {
                    keywords.KwFinalPriceHoliday = 0;
                }

                var listSchedule = new List<Schedule>();
                if (!string.IsNullOrEmpty(isDelete))
                {
                    if (!string.IsNullOrEmpty(kwTotalCostTourNotSvc))
                    {
                        var querylistSchedule = (from x in _db.Schedules
                                                 where x.Isdelete == keywords.IsDelete &&
                                                       x.TourId == idTour &&
                                                       x.IdSchedule.ToLower().Contains(keywords.KwId) &&
                                                       x.IsTempData == false &&
                                                       x.Approve == Convert.ToInt16(Enums.ApproveStatus.Approved) &&
                                                       x.TotalCostTourNotService.Equals(keywords.KwTotalCostTourNotService)

                                                 select new Schedule
                                                 {
                                                     IdSchedule = x.IdSchedule,
                                                     BeginDate = x.BeginDate,
                                                     EndDate = x.EndDate,
                                                     TotalCostTourNotService = x.TotalCostTourNotService,
                                                     FinalPrice = x.FinalPrice,
                                                     FinalPriceHoliday = x.FinalPriceHoliday,
                                                     EmployeeId = x.EmployeeId,
                                                     CarId = x.CarId,
                                                     DepartureDate = x.DepartureDate,
                                                     ReturnDate = x.ReturnDate,
                                                     MaxCapacity = x.MaxCapacity,
                                                     MinCapacity = x.MinCapacity,
                                                     DeparturePlace = x.DeparturePlace,
                                                     Description = x.Description,
                                                     Vat = x.Vat,
                                                     PromotionId = x.PromotionId,
                                                     TimePromotion = x.TimePromotion
                                                 });
                        totalResult = querylistSchedule.Count();
                        listSchedule = querylistSchedule.Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(kwFinalPrice))
                        {
                            var querylistSchedule = (from x in _db.Schedules
                                                     where x.Isdelete == keywords.IsDelete &&
                                                           x.TourId == idTour &&
                                                           x.IdSchedule.ToLower().Contains(keywords.KwId) &&
                                                           x.IsTempData == false &&
                                                           x.Approve == Convert.ToInt16(Enums.ApproveStatus.Approved) &&
                                                           x.FinalPrice.Equals(keywords.KwFinalPrice)

                                                     select new Schedule
                                                     {
                                                         IdSchedule = x.IdSchedule,
                                                         BeginDate = x.BeginDate,
                                                         EndDate = x.EndDate,
                                                         TotalCostTourNotService = x.TotalCostTourNotService,
                                                         FinalPrice = x.FinalPrice,
                                                         FinalPriceHoliday = x.FinalPriceHoliday,
                                                         EmployeeId = x.EmployeeId,
                                                         CarId = x.CarId,
                                                         DepartureDate = x.DepartureDate,
                                                         ReturnDate = x.ReturnDate,
                                                         MaxCapacity = x.MaxCapacity,
                                                         MinCapacity = x.MinCapacity,
                                                         DeparturePlace = x.DeparturePlace,
                                                         Description = x.Description,
                                                         Vat = x.Vat,
                                                         PromotionId = x.PromotionId,
                                                         TimePromotion = x.TimePromotion
                                                     });

                            totalResult = querylistSchedule.Count();
                            listSchedule = querylistSchedule.Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();
                        }
                        else
                        {

                            if (!string.IsNullOrEmpty(kwFinalPriceHoliday))
                            {
                                var querylistSchedule = (from x in _db.Schedules
                                                         where x.Isdelete == keywords.IsDelete &&
                                                               x.TourId == idTour &&
                                                               x.IdSchedule.ToLower().Contains(keywords.KwId) &&
                                                               x.IsTempData == false &&
                                                               x.Approve == Convert.ToInt16(Enums.ApproveStatus.Approved) &&
                                                               x.FinalPriceHoliday.Equals(keywords.KwFinalPriceHoliday)

                                                         select new Schedule
                                                         {
                                                             IdSchedule = x.IdSchedule,
                                                             BeginDate = x.BeginDate,
                                                             EndDate = x.EndDate,
                                                             TotalCostTourNotService = x.TotalCostTourNotService,
                                                             FinalPrice = x.FinalPrice,
                                                             FinalPriceHoliday = x.FinalPriceHoliday,
                                                             EmployeeId = x.EmployeeId,
                                                             CarId = x.CarId,
                                                             DepartureDate = x.DepartureDate,
                                                             ReturnDate = x.ReturnDate,
                                                             MaxCapacity = x.MaxCapacity,
                                                             MinCapacity = x.MinCapacity,
                                                             DeparturePlace = x.DeparturePlace,
                                                             Description = x.Description,
                                                             Vat = x.Vat,
                                                             PromotionId = x.PromotionId,
                                                             TimePromotion = x.TimePromotion
                                                         });

                                totalResult = querylistSchedule.Count();
                                listSchedule = querylistSchedule.Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();
                            }
                            else
                            {
                                // ngày bán vé
                                if (keywords.KwBeginDate > 0 && keywords.KwEndDate > 0)
                                {
                                    var querylistSchedule = (from x in _db.Schedules
                                                             where x.TourId == idTour &&
                                                                   x.Isdelete == keywords.IsDelete &&
                                                                   x.IdSchedule.ToLower().Contains(keywords.KwId) &&
                                                                   x.IsTempData == false &&
                                                                   x.Approve == Convert.ToInt16(Enums.ApproveStatus.Approved) &&
                                                                   x.BeginDate >= keywords.KwBeginDate &&
                                                                   x.EndDate <= keywords.KwEndDate
                                                             select x);

                                    totalResult = querylistSchedule.Count();
                                    listSchedule = querylistSchedule.Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();
                                }
                                else
                                {
                                    if (keywords.KwBeginDate == 0 && keywords.KwEndDate > 0)
                                    {
                                        var querylistSchedule = (from x in _db.Schedules
                                                                 where x.TourId == idTour &&
                                                                       x.Isdelete == keywords.IsDelete &&
                                                                       x.IdSchedule.ToLower().Contains(keywords.KwId) &&
                                                                       x.IsTempData == false &&
                                                                       x.Approve == Convert.ToInt16(Enums.ApproveStatus.Approved) &&
                                                                       x.EndDate <= keywords.KwEndDate
                                                                 select x);

                                        totalResult = querylistSchedule.Count();
                                        listSchedule = querylistSchedule.Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();
                                    }
                                    else
                                    {
                                        if (keywords.KwEndDate == 0 && keywords.KwBeginDate > 0)
                                        {
                                            var querylistSchedule = (from x in _db.Schedules
                                                                     where x.TourId == idTour &&
                                                                           x.Isdelete == keywords.IsDelete &&
                                                                           x.IdSchedule.ToLower().Contains(keywords.KwId) &&
                                                                           x.IsTempData == false &&
                                                                           x.Approve == Convert.ToInt16(Enums.ApproveStatus.Approved) &&
                                                                           x.BeginDate >= keywords.KwBeginDate
                                                                     select x);

                                            totalResult = querylistSchedule.Count();
                                            listSchedule = querylistSchedule.Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();
                                        }
                                        else
                                        {
                                            //Ngày khởi hành
                                            if (keywords.KwDepartureDate > 0 && keywords.KwReturnDate > 0)
                                            {
                                                var querylistSchedule = (from x in _db.Schedules
                                                                         where x.TourId == idTour &&
                                                                               x.Isdelete == keywords.IsDelete &&
                                                                               x.IdSchedule.ToLower().Contains(keywords.KwId) &&
                                                                               x.IsTempData == false &&
                                                                               x.Approve == Convert.ToInt16(Enums.ApproveStatus.Approved) &&
                                                                               x.DepartureDate >= keywords.KwDepartureDate &&
                                                                               x.ReturnDate <= keywords.KwReturnDate
                                                                         select x);

                                                totalResult = querylistSchedule.Count();
                                                listSchedule = querylistSchedule.Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();
                                            }
                                            else
                                            {
                                                if (keywords.KwDepartureDate == 0 && keywords.KwReturnDate > 0)
                                                {
                                                    var querylistSchedule = (from x in _db.Schedules
                                                                             where x.TourId == idTour &&
                                                                                   x.Isdelete == keywords.IsDelete &&
                                                                                   x.IdSchedule.ToLower().Contains(keywords.KwId) &&
                                                                                   x.IsTempData == false &&
                                                                                   x.Approve == Convert.ToInt16(Enums.ApproveStatus.Approved) &&
                                                                                   x.ReturnDate <= keywords.KwReturnDate
                                                                             select x);

                                                    totalResult = querylistSchedule.Count();
                                                    listSchedule = querylistSchedule.Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();
                                                }
                                                else
                                                {
                                                    if (keywords.KwReturnDate == 0 && keywords.KwDepartureDate > 0)
                                                    {
                                                        var querylistSchedule = (from x in _db.Schedules
                                                                                 where x.TourId == idTour &&
                                                                                       x.Isdelete == keywords.IsDelete &&
                                                                                       x.IdSchedule.ToLower().Contains(keywords.KwId) &&
                                                                                       x.IsTempData == false &&
                                                                                       x.Approve == Convert.ToInt16(Enums.ApproveStatus.Approved) &&
                                                                                       x.DepartureDate >= keywords.KwDepartureDate
                                                                                 select x);

                                                        totalResult = querylistSchedule.Count();
                                                        listSchedule = querylistSchedule.Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();
                                                    }
                                                    else
                                                    {
                                                        var querylistSchedule = (from x in _db.Schedules
                                                                                 where x.TourId == idTour &&
                                                                                       x.Isdelete == keywords.IsDelete &&
                                                                                       x.IdSchedule.ToLower().Contains(keywords.KwId) &&
                                                                                       x.IsTempData == false &&
                                                                                       x.Approve == Convert.ToInt16(Enums.ApproveStatus.Approved)
                                                                                 select x);

                                                        totalResult = querylistSchedule.Count();
                                                        listSchedule = querylistSchedule.Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(kwTotalCostTourNotSvc))
                    {
                        var querylistSchedule = (from x in _db.Schedules
                                                 where x.Isdelete == keywords.IsDelete &&
                                                       x.TourId == idTour &&
                                                       x.IdSchedule.ToLower().Contains(keywords.KwId) &&
                                                       x.IsTempData == false &&
                                                       x.Approve == Convert.ToInt16(Enums.ApproveStatus.Approved) &&
                                                       x.TotalCostTourNotService.Equals(keywords.KwTotalCostTourNotService)

                                                 select x);

                        totalResult = querylistSchedule.Count();
                        listSchedule = querylistSchedule.Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(kwFinalPrice))
                        {
                            var querylistSchedule = (from x in _db.Schedules
                                                     where x.Isdelete == keywords.IsDelete &&
                                                           x.TourId == idTour &&
                                                           x.IdSchedule.ToLower().Contains(keywords.KwId) &&
                                                           x.IsTempData == false &&
                                                           x.Approve == Convert.ToInt16(Enums.ApproveStatus.Approved) &&
                                                           x.FinalPrice.Equals(keywords.KwFinalPrice)

                                                     select x);

                            totalResult = querylistSchedule.Count();
                            listSchedule = querylistSchedule.Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(kwFinalPriceHoliday))
                            {
                                var querylistSchedule = (from x in _db.Schedules
                                                         where x.Isdelete == keywords.IsDelete &&
                                                               x.TourId == idTour &&
                                                               x.IdSchedule.ToLower().Contains(keywords.KwId) &&
                                                               x.IsTempData == false &&
                                                               x.Approve == Convert.ToInt16(Enums.ApproveStatus.Approved) &&
                                                               x.FinalPriceHoliday.Equals(keywords.KwFinalPriceHoliday)

                                                         select x);

                                totalResult = querylistSchedule.Count();
                                listSchedule = querylistSchedule.Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();
                            }
                            else
                            {
                                // ngày bán vé
                                if (keywords.KwBeginDate > 0 && keywords.KwEndDate > 0)
                                {
                                    var querylistSchedule = (from x in _db.Schedules
                                                             where x.TourId == idTour &&
                                                                   x.Isdelete == keywords.IsDelete &&
                                                                   x.IdSchedule.ToLower().Contains(keywords.KwId) &&
                                                                   x.IsTempData == false &&
                                                                   x.Approve == Convert.ToInt16(Enums.ApproveStatus.Approved) &&
                                                                   x.BeginDate >= keywords.KwBeginDate &&
                                                                   x.EndDate <= keywords.KwEndDate
                                                             select x);

                                    totalResult = querylistSchedule.Count();
                                    listSchedule = querylistSchedule.Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();
                                }
                                else
                                {
                                    if (keywords.KwBeginDate == 0 && keywords.KwEndDate > 0)
                                    {
                                        var querylistSchedule = (from x in _db.Schedules
                                                                 where x.TourId == idTour &&
                                                                       x.Isdelete == keywords.IsDelete &&
                                                                       x.IdSchedule.ToLower().Contains(keywords.KwId) &&
                                                                       x.IsTempData == false &&
                                                                       x.Approve == Convert.ToInt16(Enums.ApproveStatus.Approved) &&
                                                                       x.EndDate <= keywords.KwEndDate
                                                                 select x);

                                        totalResult = querylistSchedule.Count();
                                        listSchedule = querylistSchedule.Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();
                                    }
                                    else
                                    {
                                        if (keywords.KwEndDate == 0 && keywords.KwBeginDate > 0)
                                        {
                                            var querylistSchedule = (from x in _db.Schedules
                                                                     where x.TourId == idTour &&
                                                                           x.Isdelete == keywords.IsDelete &&
                                                                           x.IdSchedule.ToLower().Contains(keywords.KwId) &&
                                                                           x.IsTempData == false &&
                                                                           x.Approve == Convert.ToInt16(Enums.ApproveStatus.Approved) &&
                                                                           x.BeginDate >= keywords.KwBeginDate
                                                                     select x);

                                            totalResult = querylistSchedule.Count();
                                            listSchedule = querylistSchedule.Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();
                                        }
                                        else
                                        {
                                            //ngày khởi hành
                                            if (keywords.KwDepartureDate > 0 && keywords.KwReturnDate > 0)
                                            {
                                                var querylistSchedule = (from x in _db.Schedules
                                                                         where x.TourId == idTour &&
                                                                               x.Isdelete == keywords.IsDelete &&
                                                                               x.IdSchedule.ToLower().Contains(keywords.KwId) &&
                                                                               x.IsTempData == false &&
                                                                               x.Approve == Convert.ToInt16(Enums.ApproveStatus.Approved) &&
                                                                               x.DepartureDate >= keywords.KwDepartureDate &&
                                                                               x.ReturnDate <= keywords.KwReturnDate
                                                                         select x);

                                                totalResult = querylistSchedule.Count();
                                                listSchedule = querylistSchedule.Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();
                                            }
                                            else
                                            {
                                                if (keywords.KwDepartureDate == 0 && keywords.KwReturnDate > 0)
                                                {
                                                    var querylistSchedule = (from x in _db.Schedules
                                                                             where x.TourId == idTour &&
                                                                                   x.Isdelete == keywords.IsDelete &&
                                                                                   x.IdSchedule.ToLower().Contains(keywords.KwId) &&
                                                                                   x.IsTempData == false &&
                                                                                   x.Approve == Convert.ToInt16(Enums.ApproveStatus.Approved) &&
                                                                                   x.ReturnDate <= keywords.KwReturnDate
                                                                             select x);

                                                    totalResult = querylistSchedule.Count();
                                                    listSchedule = querylistSchedule.Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();
                                                }
                                                else
                                                {
                                                    if (keywords.KwReturnDate == 0 && keywords.KwDepartureDate > 0)
                                                    {
                                                        var querylistSchedule = (from x in _db.Schedules
                                                                                 where x.TourId == idTour &&
                                                                                       x.Isdelete == keywords.IsDelete &&
                                                                                       x.IdSchedule.ToLower().Contains(keywords.KwId) &&
                                                                                       x.IsTempData == false &&
                                                                                       x.Approve == Convert.ToInt16(Enums.ApproveStatus.Approved) &&
                                                                                       x.DepartureDate >= keywords.KwDepartureDate
                                                                                 select x);

                                                        totalResult = querylistSchedule.Count();
                                                        listSchedule = querylistSchedule.Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();
                                                    }
                                                    else
                                                    {
                                                        var querylistSchedule = (from x in _db.Schedules
                                                                                 where x.TourId == idTour &&
                                                                                       x.Isdelete == keywords.IsDelete &&
                                                                                       x.IdSchedule.ToLower().Contains(keywords.KwId) &&
                                                                                       x.IsTempData == false &&
                                                                                       x.Approve == Convert.ToInt16(Enums.ApproveStatus.Approved)
                                                                                 select x);

                                                        totalResult = querylistSchedule.Count();
                                                        listSchedule = querylistSchedule.Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();
                                                    }

                                                }

                                            }
                                        }
                                    }
                                }

                            }

                        }

                    }
                }
                var result = listSchedule;
                result = (from s in result
                          orderby s.BeginDate descending, s.DepartureDate descending
                          select new Schedule
                          {
                              Alias = s.Alias,
                              Approve = s.Approve,
                              BeginDate = s.BeginDate,
                              QuantityAdult = s.QuantityAdult,
                              QuantityBaby = s.QuantityBaby,
                              QuantityChild = s.QuantityChild,
                              CarId = s.CarId,
                              DepartureDate = s.DepartureDate,
                              ReturnDate = s.ReturnDate,
                              DeparturePlace = s.DeparturePlace,
                              Description = s.Description,
                              MetaDesc = s.MetaDesc,
                              MetaKey = s.MetaKey,
                              EndDate = s.EndDate,
                              Isdelete = s.Isdelete,
                              EmployeeId = s.EmployeeId,
                              IdSchedule = s.IdSchedule,
                              MaxCapacity = s.MaxCapacity,
                              MinCapacity = s.MinCapacity,
                              PromotionId = s.PromotionId,
                              Status = s.Status,
                              TourId = s.TourId,
                              FinalPrice = s.FinalPrice,
                              FinalPriceHoliday = s.FinalPriceHoliday,
                              AdditionalPrice = s.AdditionalPrice,
                              AdditionalPriceHoliday = s.AdditionalPriceHoliday,
                              IsHoliday = s.IsHoliday,
                              Profit = s.Profit,
                              QuantityCustomer = s.QuantityCustomer,
                              TimePromotion = s.TimePromotion,
                              Vat = s.Vat,
                              TotalCostTourNotService = s.TotalCostTourNotService,
                              TypeAction = s.TypeAction,
                              IdUserModify = s.IdUserModify,
                              ModifyBy = s.ModifyBy,
                              ModifyDate = s.ModifyDate,
                              CostTour = (from c in _db.CostTours.AsNoTracking()
                                          where c.IdSchedule == s.IdSchedule
                                          select c).First(),
                              Timelines = (from t in _db.Timelines.AsNoTracking()
                                           where t.IdSchedule == s.IdSchedule
                                           select t).ToList(),
                              Promotions = (from p in _db.Promotions.AsNoTracking()
                                            where p.IdPromotion == s.PromotionId
                                            select p).First(),
                              Tour = (from t in _db.Tour.AsNoTracking()
                                      where s.TourId == t.IdTour
                                      select new Tour
                                      {
                                          Thumbnail = t.Thumbnail,
                                          ToPlace = t.ToPlace,
                                          IdTour = t.IdTour,
                                          NameTour = t.NameTour,
                                          Alias = t.Alias,
                                          ApproveStatus = t.ApproveStatus,
                                          CreateDate = t.CreateDate,
                                          IsActive = t.IsActive,
                                          IsDelete = t.IsDelete,
                                          ModifyBy = t.ModifyBy,
                                          ModifyDate = t.ModifyDate,
                                          Rating = t.Rating,
                                          Status = t.Status,
                                          QuantityBooked = t.QuantityBooked,
                                      }).First(),

                          }).Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();
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
            catch (Exception e)
            {
                return Ultility.Responses("Có lỗi xảy ra !", Enums.TypeCRUD.Error.ToString(), description: e.Message);
            }
        }

        public Response SearchScheduleWaiting(JObject frmData, string idTour)
        {
            try
            {
                Keywords keywords = new Keywords();

                if (!String.IsNullOrEmpty(idTour))
                {
                    keywords.KwIdTour = idTour;
                }

                var kwIdSchedule = PrCommon.GetString("idSchedule", frmData);
                if (!String.IsNullOrEmpty(kwIdSchedule))
                {
                    keywords.KwId = kwIdSchedule.Trim().ToLower();
                }
                else
                {
                    keywords.KwId = "";
                }

                var kwFinalPrice = PrCommon.GetString("finalPrice", frmData);
                if (!String.IsNullOrEmpty(kwFinalPrice))
                {
                    keywords.KwFinalPrice = float.Parse(kwFinalPrice);
                }
                else
                {
                    keywords.KwFinalPrice = 0;
                }

                var kwFinalPriceHoliday = PrCommon.GetString("finalPriceHoliday", frmData);
                if (!String.IsNullOrEmpty(kwFinalPriceHoliday))
                {
                    keywords.KwFinalPriceHoliday = float.Parse(kwFinalPriceHoliday);
                }
                else
                {
                    keywords.KwFinalPriceHoliday = 0;
                }

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

                var listSchedule = new List<Schedule>();

                if (!string.IsNullOrEmpty(kwFinalPrice))
                {
                    listSchedule = (from x in _db.Schedules
                                    where
                                          x.TourId == idTour &&
                                          x.IdSchedule.ToLower().Contains(keywords.KwId) &&
                                          x.IsTempData == false &&
                                          x.Approve == Convert.ToInt16(Enums.ApproveStatus.Waiting) &&
                                          x.FinalPrice.Equals(keywords.KwFinalPrice) &&
                                          x.ModifyBy.ToLower().Contains(keywords.KwModifyBy)
                                    select x).ToList();
                }
                else
                {
                    if (!string.IsNullOrEmpty(kwFinalPriceHoliday))
                    {
                        listSchedule = (from x in _db.Schedules
                                        where
                                              x.TourId == idTour &&
                                              x.IdSchedule.ToLower().Contains(keywords.KwId) &&
                                              x.IsTempData == false &&
                                              x.Approve == Convert.ToInt16(Enums.ApproveStatus.Waiting) &&
                                              x.ModifyBy.ToLower().Contains(keywords.KwModifyBy) &&
                                              x.FinalPriceHoliday.Equals(keywords.KwFinalPriceHoliday)

                                        select x).ToList();
                    }
                    else
                    {
                        if (keywords.KwTypeActions.Count > 0)
                        {
                            if (keywords.KwFromDate > 0 && keywords.KwToDate > 0)
                            {
                                listSchedule = (from x in _db.Schedules
                                                where
                                                      x.TourId == idTour &&
                                                      x.IdSchedule.ToLower().Contains(keywords.KwId) &&
                                                      x.IsTempData == false &&
                                                      x.Approve == Convert.ToInt16(Enums.ApproveStatus.Waiting) &&
                                                      keywords.KwTypeActions.Contains(x.TypeAction) &&
                                                      x.ModifyBy.ToLower().Contains(keywords.KwModifyBy) &&
                                                      x.ModifyDate >= keywords.KwFromDate &&
                                                       x.ModifyDate <= keywords.KwToDate
                                                orderby x.ModifyDate descending
                                                select x).ToList();
                            }
                            else
                            {
                                if (keywords.KwFromDate == 0 && keywords.KwToDate > 0)
                                {
                                    listSchedule = (from x in _db.Schedules
                                                    where
                                                          x.TourId == idTour &&
                                                          x.ModifyBy.ToLower().Contains(keywords.KwModifyBy) &&
                                                          x.IdSchedule.ToLower().Contains(keywords.KwId) &&
                                                          x.IsTempData == false &&
                                                          x.Approve == Convert.ToInt16(Enums.ApproveStatus.Waiting) &&
                                                          keywords.KwTypeActions.Contains(x.TypeAction) &&
                                                           x.ModifyDate <= keywords.KwToDate
                                                    orderby x.ModifyDate descending
                                                    select x).ToList();
                                }
                                else
                                {
                                    if (keywords.KwToDate == 0 && keywords.KwFromDate > 0)
                                    {
                                        listSchedule = (from x in _db.Schedules
                                                        where
                                                              x.TourId == idTour &&
                                                              x.IdSchedule.ToLower().Contains(keywords.KwId) &&
                                                              x.ModifyBy.ToLower().Contains(keywords.KwModifyBy) &&
                                                              x.IsTempData == false &&
                                                              x.Approve == Convert.ToInt16(Enums.ApproveStatus.Waiting) &&
                                                              keywords.KwTypeActions.Contains(x.TypeAction) &&
                                                              x.ModifyDate >= keywords.KwFromDate
                                                        orderby x.ModifyDate descending
                                                        select x).ToList();
                                    }
                                    else
                                    {
                                        listSchedule = (from x in _db.Schedules
                                                        where
                                                              x.TourId == idTour &&
                                                              x.IdSchedule.ToLower().Contains(keywords.KwId) &&
                                                              x.ModifyBy.ToLower().Contains(keywords.KwModifyBy) &&
                                                              x.IsTempData == false &&
                                                              x.Approve == Convert.ToInt16(Enums.ApproveStatus.Waiting) &&
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
                                listSchedule = (from x in _db.Schedules
                                                where x.TourId == idTour &&
                                                      x.IdSchedule.ToLower().Contains(keywords.KwId) &&
                                                      x.IsTempData == false &&
                                                      x.Approve == Convert.ToInt16(Enums.ApproveStatus.Waiting) &&
                                                      x.ModifyBy.ToLower().Contains(keywords.KwModifyBy) &&
                                                        x.ModifyDate >= keywords.KwFromDate &&
                                                        x.ModifyDate <= keywords.KwToDate
                                                orderby x.ModifyDate descending
                                                select x).ToList();

                            }
                            else
                            {
                                if (keywords.KwFromDate == 0 && keywords.KwToDate > 0)
                                {
                                    listSchedule = (from x in _db.Schedules
                                                    where x.TourId == idTour &&
                                                          x.IdSchedule.ToLower().Contains(keywords.KwId) &&
                                                          x.ModifyBy.ToLower().Contains(keywords.KwModifyBy) &&
                                                          x.IsTempData == false &&
                                                          x.Approve == Convert.ToInt16(Enums.ApproveStatus.Waiting) &&
                                                            x.ModifyDate <= keywords.KwToDate
                                                    orderby x.ModifyDate descending
                                                    select x).ToList();
                                }
                                else
                                {
                                    if (keywords.KwToDate == 0 && keywords.KwFromDate > 0)
                                    {
                                        listSchedule = (from x in _db.Schedules
                                                        where x.TourId == idTour &&
                                                              x.IdSchedule.ToLower().Contains(keywords.KwId) &&
                                                              x.IsTempData == false &&
                                                              x.Approve == Convert.ToInt16(Enums.ApproveStatus.Waiting) &&
                                                              x.ModifyBy.ToLower().Contains(keywords.KwModifyBy) &&
                                                                x.ModifyDate >= keywords.KwFromDate
                                                        orderby x.ModifyDate descending
                                                        select x).ToList();
                                    }
                                    else
                                    {
                                        listSchedule = (from x in _db.Schedules
                                                        where x.TourId == idTour &&
                                                              x.IdSchedule.ToLower().Contains(keywords.KwId) &&
                                                              x.IsTempData == false &&
                                                              x.ModifyBy.ToLower().Contains(keywords.KwModifyBy) &&
                                                              x.Approve == Convert.ToInt16(Enums.ApproveStatus.Waiting)
                                                        orderby x.ModifyDate descending
                                                        select x).ToList();
                                    }
                                }
                            }

                        }

                    }

                }

                var result = listSchedule;
                if (listSchedule.Count() > 0)
                {
                    return Ultility.Responses("", Enums.TypeCRUD.Success.ToString(), result);
                }
                else
                {
                    return Ultility.Responses($"Không có dữ liệu trả về !", Enums.TypeCRUD.Warning.ToString());
                }

            }
            catch (Exception e)
            {
                return Ultility.Responses("Có lỗi xảy ra !", Enums.TypeCRUD.Error.ToString(), description: e.Message);
            }
        }

        public Response GetsSelectBoxCreate(long fromDate, long toDate, string idTour)
        {
            try
            {
                var unixTimeOneDay = 86400000;

                var listCarShouldRemove1 = (from x in _db.Schedules.AsNoTracking()
                                            where x.TourId == idTour
                                            && (fromDate >= x.DepartureDate && fromDate <= (x.ReturnDate + 86400000))
                                            orderby x.ReturnDate ascending
                                            select x.CarId);

                var scheduleDepartDateLargerToDate = (from x in _db.Schedules.AsNoTracking()
                                                      where x.TourId == idTour
                                                      && x.DepartureDate >= fromDate
                                                      orderby x.DepartureDate ascending
                                                      select x);
                var listCarShouldRemove2 = (from x in scheduleDepartDateLargerToDate
                                            where !(from s in listCarShouldRemove1 select s).Contains(x.CarId)
                                            && (toDate + 86400000) > x.ReturnDate
                                            select x.CarId).Distinct();

                var listShouldRemove = listCarShouldRemove1.Concat(listCarShouldRemove2);

                var listCar = (from x in _db.Cars.AsNoTracking()
                               where !listShouldRemove.Any(c => c == x.IdCar)
                               select x).ToList();
                if (listCar.Count() == 0)
                {
                    return Ultility.Responses("Ngày bạn chọn hiện tại không có xe !", Enums.TypeCRUD.Warning.ToString());
                }
                var result = Mapper.MapCar(listCar);
                return Ultility.Responses("", Enums.TypeCRUD.Success.ToString(), result);
            }
            catch (Exception e)
            {
                return Ultility.Responses("Có lỗi xảy ra !", Enums.TypeCRUD.Error.ToString(), description: e.Message);
            }
        }

        public async Task<Schedule> GetScheduleByIdForPayPal(string idSchedule)
        {
            try
            {
                return await _db.Schedules.FindAsync(idSchedule);
            }
            catch (Exception e)
            {

                throw;
            }
        }
        public async Task<Schedule> GetScheduleByIdForVnPay(string idSchedule)
        {
            try
            {
                return await _db.Schedules.FindAsync(idSchedule);
            }
            catch (Exception e)
            {

                throw;
            }
        }
        public async Task<Response> AutomaticUpdatePromotionForSchedule()
        {
            try
            {
                var dateTimeNow = GetDateTimeNow();
                var lsExpiresPromotion = (from x in _db.Promotions.AsNoTracking()
                                          where x.ToDate <= dateTimeNow
                                          && x.IsDelete == false
                                          select x.IdPromotion);
                var lsScheduleHavePromotionExpires = await (from x in _db.Schedules.AsNoTracking()
                                                            where lsExpiresPromotion.Any(p => p == x.PromotionId)
                                                            && x.Isdelete == false
                                                            select x).ToListAsync();
                lsScheduleHavePromotionExpires.ForEach(x => x.PromotionId = -1);
                foreach (var item in lsScheduleHavePromotionExpires)
                {
                    UpdateDatabase(item);
                }
                await SaveChangeAsync();
                return Ultility.Responses("", Enums.TypeCRUD.Success.ToString());
            }
            catch (Exception e)
            {
                return Ultility.Responses("Có lỗi xảy ra !", Enums.TypeCRUD.Error.ToString(), description: e.Message);

            }
        }

        public async Task<Response> SearchTourFilter(JObject frmData)
        {
            try
            {
                Keywords keywords = new Keywords();
                var kwFrom = PrCommon.GetString("kwFrom", frmData);
                if (!String.IsNullOrEmpty(kwFrom))
                {
                    keywords.KwFrom = kwFrom.Trim().ToLower();
                }
                else
                {
                    keywords.KwFrom = "";
                }

                var kwTo = PrCommon.GetString("kwTo", frmData);
                if (!String.IsNullOrEmpty(kwTo))
                {
                    keywords.KwTo = kwTo.Trim().ToLower();
                }
                else
                {
                    keywords.KwTo = "";
                }
                var kwDepartureDate = PrCommon.GetString("kwDepartureDate", frmData);
                if (!String.IsNullOrEmpty(kwDepartureDate))
                {
                    keywords.KwDepartureDate = Ultility.ConvertDatetimeToUnixTimeStampMiliSecond(DateTime.Parse(kwDepartureDate));
                }
                else
                {
                    keywords.KwDepartureDate = 0;
                }

                var kwReturnDate = PrCommon.GetString("kwReturnDate", frmData);
                if (!String.IsNullOrEmpty(kwReturnDate))
                {
                    keywords.KwReturnDate = Ultility.ConvertDatetimeToUnixTimeStampMiliSecond(DateTime.Parse(kwReturnDate).AddDays(1).AddSeconds(-1));
                }
                else
                {
                    keywords.KwReturnDate = 0;
                }

                var kwPriceFrom = PrCommon.GetString("kwPriceFrom", frmData);
                if (!String.IsNullOrEmpty(kwPriceFrom))
                {
                    keywords.KwPriceFrom = float.Parse(kwPriceFrom);
                }
                else
                {
                    keywords.KwPriceFrom = 0;
                }

                var kwPriceTo = PrCommon.GetString("kwPriceTo", frmData);
                if (!String.IsNullOrEmpty(kwPriceTo))
                {
                    keywords.KwPriceTo = float.Parse(kwPriceTo);
                }
                else
                {
                    keywords.KwPriceTo = 0;
                }

                var kwPromotion = PrCommon.GetString("kwPromotion", frmData);
                if (!String.IsNullOrEmpty(kwPromotion))
                {
                    keywords.KwPromotion = int.Parse(kwPromotion);
                }
                else
                {
                    keywords.KwPromotion = 0;
                }

                var kwIsHoliday = PrCommon.GetString("kwIsHoliday", frmData);
                if (!String.IsNullOrEmpty(kwIsHoliday))
                {
                    keywords.KwIsHoliday = bool.Parse(kwIsHoliday);
                }

                var kwIsAllOption = PrCommon.GetString("kwIsAllOption", frmData);
                if (!String.IsNullOrEmpty(kwIsAllOption))
                {
                    keywords.KwIsAllOption = bool.Parse(kwIsAllOption);
                }


                var dateTimeNow = GetDateTimeNow();
                var filterList = (from s in _db.Schedules.AsNoTracking()
                                  where s.Isdelete == false
                                   && s.EndDate > dateTimeNow
                                   && s.BeginDate <= dateTimeNow
                                   && s.Approve == (int)Enums.ApproveStatus.Approved
                                  select new Schedule
                                  {
                                      Alias = s.Alias,
                                      Approve = s.Approve,
                                      BeginDate = s.BeginDate,
                                      QuantityAdult = s.QuantityAdult,
                                      QuantityBaby = s.QuantityBaby,
                                      QuantityChild = s.QuantityChild,
                                      CarId = s.CarId,
                                      Description = s.Description,
                                      DepartureDate = s.DepartureDate,
                                      ReturnDate = s.ReturnDate,
                                      EndDate = s.EndDate,
                                      Isdelete = s.Isdelete,
                                      EmployeeId = s.EmployeeId,
                                      IdSchedule = s.IdSchedule,
                                      MaxCapacity = s.MaxCapacity,
                                      MinCapacity = s.MinCapacity,
                                      PromotionId = s.PromotionId,
                                      DeparturePlace = s.DeparturePlace,
                                      Status = s.Status,
                                      TourId = s.TourId,
                                      FinalPrice = s.FinalPrice,
                                      FinalPriceHoliday = s.FinalPriceHoliday,
                                      AdditionalPrice = s.AdditionalPrice,
                                      AdditionalPriceHoliday = s.AdditionalPriceHoliday,
                                      IsHoliday = s.IsHoliday,
                                      Profit = s.Profit,
                                      QuantityCustomer = s.QuantityCustomer,
                                      TimePromotion = s.TimePromotion,
                                      Vat = s.Vat,
                                      Promotions = (from p in _db.Promotions where p.IdPromotion == s.PromotionId select p).FirstOrDefault(),
                                      TotalCostTourNotService = s.TotalCostTourNotService,
                                      CostTour = (from c in _db.CostTours where c.IdSchedule == s.IdSchedule select c).FirstOrDefault(),
                                      Timelines = (from t in _db.Timelines where t.IdSchedule == s.IdSchedule select t).ToList(),
                                      Tour = (from t in _db.Tour
                                              where t.IdTour == s.TourId
                                              select new Tour
                                              {
                                                  Thumbnail = t.Thumbnail,
                                                  ToPlace = t.ToPlace,
                                                  IdTour = t.IdTour,
                                                  NameTour = t.NameTour,
                                                  Alias = t.Alias,
                                                  ApproveStatus = t.ApproveStatus,
                                                  CreateDate = t.CreateDate,
                                                  IsActive = t.IsActive,
                                                  IsDelete = t.IsDelete,
                                                  ModifyBy = t.ModifyBy,
                                                  ModifyDate = t.ModifyDate,
                                                  Rating = t.Rating,
                                                  Status = t.Status
                                              }).FirstOrDefault(),
                                  });

                #region search filter 
                if (keywords.KwIsAllOption)
                {
                    ///
                    if (keywords.KwDepartureDate > 0 || keywords.KwReturnDate > 0)
                    {
                        if (keywords.KwDepartureDate > 0 && keywords.KwReturnDate > 0)
                        {
                            if (keywords.KwPriceFrom > 0 || keywords.KwPriceTo > 0)
                            {
                                #region price && departureDate => kwReturnDate
                                if (keywords.KwPriceFrom > 0 && keywords.KwPriceTo > 0)
                                {
                                    filterList = from s in filterList
                                                 where s.DeparturePlace.ToLower().Contains(keywords.KwFrom) &&
                                                       s.DepartureDate >= keywords.KwDepartureDate &&
                                                       s.ReturnDate <= keywords.KwReturnDate &&
                                                       s.FinalPrice >= keywords.KwPriceFrom &&
                                                       s.FinalPrice <= keywords.KwPriceTo &&
                                                       s.Tour.ToPlace.ToLower().Contains(keywords.KwTo)
                                                 select s;
                                }
                                else if (keywords.KwPriceFrom > 0)
                                {
                                    filterList = from s in filterList
                                                 where s.DeparturePlace.ToLower().Contains(keywords.KwFrom) &&
                                                       s.DepartureDate >= keywords.KwDepartureDate &&
                                                       s.ReturnDate <= keywords.KwReturnDate &&
                                                       s.FinalPrice <= keywords.KwPriceFrom &&
                                                       s.Tour.ToPlace.ToLower().Contains(keywords.KwTo)
                                                 select s;
                                }
                                else if (keywords.KwPriceTo > 0)
                                {
                                    filterList = from s in filterList
                                                 where s.DeparturePlace.ToLower().Contains(keywords.KwFrom) &&
                                                       s.DepartureDate >= keywords.KwDepartureDate &&
                                                       s.ReturnDate <= keywords.KwReturnDate &&
                                                       s.FinalPrice >= keywords.KwPriceTo &&
                                                       s.Tour.ToPlace.ToLower().Contains(keywords.KwTo)
                                                 select s;
                                }
                                #endregion
                            }
                            else
                            {

                                filterList = from s in filterList
                                             where s.DeparturePlace.ToLower().Contains(keywords.KwFrom) &&
                                                   s.DepartureDate >= keywords.KwDepartureDate &&
                                                   s.ReturnDate <= keywords.KwReturnDate &&
                                                   s.Tour.ToPlace.ToLower().Contains(keywords.KwTo)
                                             select s;
                            }
                        }
                        else if (keywords.KwReturnDate == 0 && keywords.KwDepartureDate > 0)
                        {
                            if (keywords.KwPriceFrom > 0 || keywords.KwPriceTo > 0)
                            {
                                #region price && departureDate 
                                if (keywords.KwPriceFrom > 0 && keywords.KwPriceTo > 0)
                                {
                                    filterList = from s in filterList
                                                 where s.DeparturePlace.ToLower().Contains(keywords.KwFrom) &&
                                                       s.DepartureDate >= keywords.KwDepartureDate &&
                                                       s.FinalPrice >= keywords.KwPriceFrom &&
                                                       s.FinalPrice <= keywords.KwPriceTo &&
                                                       s.Tour.ToPlace.ToLower().Contains(keywords.KwTo)
                                                 select s;
                                }
                                else if (keywords.KwPriceFrom > 0)
                                {
                                    filterList = from s in filterList
                                                 where s.DeparturePlace.ToLower().Contains(keywords.KwFrom) &&
                                                       s.DepartureDate >= keywords.KwDepartureDate &&
                                                       s.FinalPrice <= keywords.KwPriceFrom &&
                                                       s.Tour.ToPlace.ToLower().Contains(keywords.KwTo)
                                                 select s;
                                }
                                else if (keywords.KwPriceTo > 0)
                                {
                                    filterList = from s in filterList
                                                 where s.DeparturePlace.ToLower().Contains(keywords.KwFrom) &&
                                                       s.DepartureDate >= keywords.KwDepartureDate &&
                                                       s.FinalPrice >= keywords.KwPriceTo &&
                                                       s.Tour.ToPlace.ToLower().Contains(keywords.KwTo)
                                                 select s;
                                }
                                #endregion
                            }
                            else
                            {
                                filterList = from s in filterList
                                             where s.DeparturePlace.ToLower().Contains(keywords.KwFrom) &&
                                                   s.DepartureDate >= keywords.KwDepartureDate &&
                                                   s.Tour.ToPlace.ToLower().Contains(keywords.KwTo)
                                             select s;
                            }
                        }
                        else if (keywords.KwReturnDate > 0 && keywords.KwDepartureDate == 0)
                        {
                            if (keywords.KwPriceFrom > 0 || keywords.KwPriceTo > 0)
                            {
                                #region price && kwReturnDate
                                if (keywords.KwPriceFrom > 0 && keywords.KwPriceTo > 0)
                                {
                                    filterList = from s in filterList
                                                 where s.DeparturePlace.ToLower().Contains(keywords.KwFrom) &&
                                                       s.ReturnDate <= keywords.KwReturnDate &&
                                                       s.FinalPrice >= keywords.KwPriceFrom &&
                                                       s.FinalPrice <= keywords.KwPriceTo &&
                                                        s.Tour.ToPlace.ToLower().Contains(keywords.KwTo)
                                                 select s;
                                }
                                else if (keywords.KwPriceFrom > 0)
                                {
                                    filterList = from s in filterList
                                                 where s.DeparturePlace.ToLower().Contains(keywords.KwFrom) &&
                                                       s.ReturnDate <= keywords.KwReturnDate &&
                                                       s.FinalPrice <= keywords.KwPriceFrom &&
                                                        s.Tour.ToPlace.ToLower().Contains(keywords.KwTo)
                                                 select s;
                                }
                                else if (keywords.KwPriceTo > 0)
                                {
                                    filterList = from s in filterList
                                                 where s.DeparturePlace.ToLower().Contains(keywords.KwFrom) &&
                                                       s.ReturnDate <= keywords.KwReturnDate &&
                                                       s.FinalPrice >= keywords.KwPriceTo &&
                                                        s.Tour.ToPlace.ToLower().Contains(keywords.KwTo)
                                                 select s;
                                }
                                #endregion
                            }
                            else
                            {
                                filterList = from s in filterList
                                             where s.DeparturePlace.ToLower().Contains(keywords.KwFrom) &&
                                                   s.ReturnDate <= keywords.KwReturnDate &&
                                                   s.Tour.ToPlace.ToLower().Contains(keywords.KwTo)
                                             select s;
                            }
                            ///
                        }
                    }

                }
                else
                {
                    filterList = from s in filterList
                                 where s.IsHoliday == keywords.KwIsHoliday
                                 select s;

                    if (keywords.KwPromotion == 1)
                    {
                        filterList = from s in filterList
                                     where
                                           s.PromotionId == 1
                                     select s;
                    }
                    else
                    {
                        filterList = from s in filterList
                                     where
                                           s.PromotionId != 1
                                     select s;

                        //foreach (var item in filterList)
                        //{
                        //    if (item.IsHoliday)
                        //    {
                        //        item.FinalPriceHoliday = item.FinalPriceHoliday - (item.FinalPriceHoliday * item.Promotions.Value / 100);
                        //    }
                        //    else
                        //    {
                        //        item.FinalPrice = item.FinalPrice - (item.FinalPrice * item.Promotions.Value / 100);

                        //    }
                        //}
                        var dasasd = filterList.ToList();
                    }
                    if (!keywords.KwIsHoliday)
                    {

                        if (keywords.KwDepartureDate > 0 || keywords.KwReturnDate > 0)
                        {
                            if (keywords.KwDepartureDate > 0 && keywords.KwReturnDate > 0)
                            {
                                if (keywords.KwPriceFrom > 0 || keywords.KwPriceTo > 0)
                                {
                                    #region price && departureDate => kwReturnDate
                                    if (keywords.KwPriceFrom > 0 && keywords.KwPriceTo > 0)
                                    {
                                        filterList = from s in filterList
                                                     where s.DeparturePlace.ToLower().Contains(keywords.KwFrom) &&
                                                           s.DepartureDate >= keywords.KwDepartureDate &&
                                                           s.ReturnDate <= keywords.KwReturnDate &&
                                                           s.FinalPrice >= keywords.KwPriceFrom &&
                                                           s.FinalPrice <= keywords.KwPriceTo &&
                                                           s.Tour.ToPlace.ToLower().Contains(keywords.KwTo)
                                                     select s;
                                    }
                                    else if (keywords.KwPriceFrom > 0)
                                    {
                                        filterList = from s in filterList
                                                     where s.DeparturePlace.ToLower().Contains(keywords.KwFrom) &&
                                                           s.DepartureDate >= keywords.KwDepartureDate &&
                                                           s.ReturnDate <= keywords.KwReturnDate &&
                                                           s.FinalPrice <= keywords.KwPriceFrom &&
                                                           s.Tour.ToPlace.ToLower().Contains(keywords.KwTo)
                                                     select s;
                                    }
                                    else if (keywords.KwPriceTo > 0)
                                    {
                                        filterList = from s in filterList
                                                     where s.DeparturePlace.ToLower().Contains(keywords.KwFrom) &&
                                                           s.DepartureDate >= keywords.KwDepartureDate &&
                                                           s.ReturnDate <= keywords.KwReturnDate &&
                                                           s.FinalPrice >= keywords.KwPriceTo &&
                                                           s.Tour.ToPlace.ToLower().Contains(keywords.KwTo)
                                                     select s;
                                    }
                                    #endregion
                                }
                                else
                                {

                                    filterList = from s in filterList
                                                 where s.DeparturePlace.ToLower().Contains(keywords.KwFrom) &&
                                                       s.DepartureDate >= keywords.KwDepartureDate &&
                                                       s.ReturnDate <= keywords.KwReturnDate &&
                                                       s.Tour.ToPlace.ToLower().Contains(keywords.KwTo)
                                                 select s;
                                }
                            }
                            else if (keywords.KwReturnDate == 0 && keywords.KwDepartureDate > 0)
                            {
                                if (keywords.KwPriceFrom > 0 || keywords.KwPriceTo > 0)
                                {
                                    #region price && departureDate 
                                    if (keywords.KwPriceFrom > 0 && keywords.KwPriceTo > 0)
                                    {
                                        filterList = from s in filterList
                                                     where s.DeparturePlace.ToLower().Contains(keywords.KwFrom) &&
                                                           s.DepartureDate >= keywords.KwDepartureDate &&
                                                           s.FinalPrice >= keywords.KwPriceFrom &&
                                                           s.FinalPrice <= keywords.KwPriceTo &&
                                                           s.Tour.ToPlace.ToLower().Contains(keywords.KwTo)
                                                     select s;
                                    }
                                    else if (keywords.KwPriceFrom > 0)
                                    {
                                        filterList = from s in filterList
                                                     where s.DeparturePlace.ToLower().Contains(keywords.KwFrom) &&
                                                           s.DepartureDate >= keywords.KwDepartureDate &&
                                                           s.FinalPrice <= keywords.KwPriceFrom &&
                                                           s.Tour.ToPlace.ToLower().Contains(keywords.KwTo)
                                                     select s;
                                    }
                                    else if (keywords.KwPriceTo > 0)
                                    {
                                        filterList = from s in filterList
                                                     where s.DeparturePlace.ToLower().Contains(keywords.KwFrom) &&
                                                           s.DepartureDate >= keywords.KwDepartureDate &&
                                                           s.FinalPrice >= keywords.KwPriceTo &&
                                                           s.Tour.ToPlace.ToLower().Contains(keywords.KwTo)
                                                     select s;
                                    }
                                    #endregion
                                }
                                else
                                {
                                    filterList = from s in filterList
                                                 where s.DeparturePlace.ToLower().Contains(keywords.KwFrom) &&
                                                       s.DepartureDate >= keywords.KwDepartureDate &&
                                                       s.Tour.ToPlace.ToLower().Contains(keywords.KwTo)
                                                 select s;
                                }
                            }
                            else if (keywords.KwReturnDate > 0 && keywords.KwDepartureDate == 0)
                            {
                                if (keywords.KwPriceFrom > 0 || keywords.KwPriceTo > 0)
                                {
                                    #region price && kwReturnDate
                                    if (keywords.KwPriceFrom > 0 && keywords.KwPriceTo > 0)
                                    {
                                        filterList = from s in filterList
                                                     where s.DeparturePlace.ToLower().Contains(keywords.KwFrom) &&
                                                           s.ReturnDate <= keywords.KwReturnDate &&
                                                           s.FinalPrice >= keywords.KwPriceFrom &&
                                                           s.FinalPrice <= keywords.KwPriceTo &&
                                                            s.Tour.ToPlace.ToLower().Contains(keywords.KwTo)
                                                     select s;
                                    }
                                    else if (keywords.KwPriceFrom > 0)
                                    {
                                        filterList = from s in filterList
                                                     where s.DeparturePlace.ToLower().Contains(keywords.KwFrom) &&
                                                           s.ReturnDate <= keywords.KwReturnDate &&
                                                           s.FinalPrice <= keywords.KwPriceFrom &&
                                                            s.Tour.ToPlace.ToLower().Contains(keywords.KwTo)
                                                     select s;
                                    }
                                    else if (keywords.KwPriceTo > 0)
                                    {
                                        filterList = from s in filterList
                                                     where s.DeparturePlace.ToLower().Contains(keywords.KwFrom) &&
                                                           s.ReturnDate <= keywords.KwReturnDate &&
                                                           s.FinalPrice >= keywords.KwPriceTo &&
                                                            s.Tour.ToPlace.ToLower().Contains(keywords.KwTo)
                                                     select s;
                                    }
                                    #endregion
                                }
                                else
                                {
                                    filterList = from s in filterList
                                                 where s.DeparturePlace.ToLower().Contains(keywords.KwFrom) &&
                                                       s.ReturnDate <= keywords.KwReturnDate &&
                                                       s.Tour.ToPlace.ToLower().Contains(keywords.KwTo)
                                                 select s;
                                }
                            }

                        }
                        else
                        {
                            if (kwFrom != "" || kwTo != "")
                            {


                                if (keywords.KwPriceFrom > 0 || keywords.KwPriceTo > 0)
                                {
                                    #region from => to && price
                                    if (keywords.KwPriceFrom > 0 && keywords.KwPriceTo > 0)
                                    {
                                        filterList = from s in filterList
                                                     where s.DeparturePlace.ToLower().Contains(keywords.KwFrom) &&
                                                           (s.FinalPrice - (s.FinalPrice * s.Promotions.Value / 100)) >= keywords.KwPriceFrom &&
                                                           (s.FinalPrice - (s.FinalPrice * s.Promotions.Value / 100)) <= keywords.KwPriceTo &&
                                                            s.Tour.ToPlace.ToLower().Contains(keywords.KwTo)
                                                     select s;
                                    }
                                    else if (keywords.KwPriceTo > 0)
                                    {
                                        filterList = from s in filterList
                                                     where s.DeparturePlace.ToLower().Contains(keywords.KwFrom) &&
                                                            (s.FinalPrice - (s.FinalPrice * s.Promotions.Value / 100)) >= keywords.KwPriceTo &&
                                                             s.Tour.ToPlace.ToLower().Contains(keywords.KwTo)
                                                     select s;
                                    }
                                    else if (keywords.KwPriceFrom > 0)
                                    {
                                        filterList = from s in filterList
                                                     where s.DeparturePlace.ToLower().Contains(keywords.KwFrom) &&
                                                            (s.FinalPrice - (s.FinalPrice * s.Promotions.Value / 100)) <= keywords.KwPriceFrom &&
                                                             s.Tour.ToPlace.ToLower().Contains(keywords.KwTo)
                                                     select s;
                                        var das = filterList.ToList();
                                    }
                                    #endregion
                                }
                                else
                                {
                                    filterList = from s in filterList
                                                 where s.DeparturePlace.ToLower().Contains(keywords.KwFrom) &&
                                                  s.Tour.ToPlace.ToLower().Contains(keywords.KwTo)
                                                 select s;
                                }
                            }
                            else if (keywords.KwPriceFrom > 0 || keywords.KwPriceTo > 0)
                            {
                                if (keywords.KwPriceFrom > 0 && keywords.KwPriceTo > 0)
                                {
                                    filterList = from s in filterList
                                                 where (s.FinalPrice - (s.FinalPrice * s.Promotions.Value / 100)) >= keywords.KwPriceFrom
                                                    && (s.FinalPrice - (s.FinalPrice * s.Promotions.Value / 100)) <= keywords.KwPriceTo
                                                 select s;


                                }
                                else if (keywords.KwPriceTo > 0)
                                {
                                    filterList = from s in filterList
                                                 where (s.FinalPrice - (s.FinalPrice * s.Promotions.Value / 100)) >= keywords.KwPriceTo
                                                 select s;
                                }
                                else if (keywords.KwPriceFrom > 0)
                                {
                                    filterList = from s in filterList
                                                 where (s.FinalPrice - (s.FinalPrice * s.Promotions.Value / 100)) <= keywords.KwPriceFrom
                                                 select s;
                                    var asd = filterList.ToList();
                                }
                            }
                        }
                    }
                    else
                    {
                        if (keywords.KwDepartureDate > 0 || keywords.KwReturnDate > 0)
                        {
                            if (keywords.KwDepartureDate > 0 && keywords.KwReturnDate > 0)
                            {
                                if (keywords.KwPriceFrom > 0 || keywords.KwPriceTo > 0)
                                {
                                    #region price && departureDate => kwReturnDate
                                    if (keywords.KwPriceFrom > 0 && keywords.KwPriceTo > 0)
                                    {
                                        filterList = from s in filterList
                                                     where s.DeparturePlace.ToLower().Contains(keywords.KwFrom) &&
                                                           s.DepartureDate >= keywords.KwDepartureDate &&
                                                           s.ReturnDate <= keywords.KwReturnDate &&
                                                           (s.FinalPriceHoliday - (s.FinalPriceHoliday * s.Promotions.Value / 100)) >= keywords.KwPriceFrom &&
                                                           s.FinalPriceHoliday <= keywords.KwPriceTo &&
                                                           s.Tour.ToPlace.ToLower().Contains(keywords.KwTo)
                                                     select s;
                                    }
                                    else if (keywords.KwPriceFrom > 0)
                                    {
                                        filterList = from s in filterList
                                                     where s.DeparturePlace.ToLower().Contains(keywords.KwFrom) &&
                                                           s.DepartureDate >= keywords.KwDepartureDate &&
                                                           s.ReturnDate <= keywords.KwReturnDate &&
                                                           (s.FinalPrice - (s.FinalPrice * s.Promotions.Value / 100)) <= keywords.KwPriceFrom &&
                                                           s.Tour.ToPlace.ToLower().Contains(keywords.KwTo)
                                                     select s;
                                    }
                                    else if (keywords.KwPriceTo > 0)
                                    {
                                        filterList = from s in filterList
                                                     where s.DeparturePlace.ToLower().Contains(keywords.KwFrom) &&
                                                           s.DepartureDate >= keywords.KwDepartureDate &&
                                                           s.ReturnDate <= keywords.KwReturnDate &&
                                                           (s.FinalPrice - (s.FinalPrice * s.Promotions.Value / 100)) >= keywords.KwPriceTo &&
                                                           s.Tour.ToPlace.ToLower().Contains(keywords.KwTo)
                                                     select s;
                                    }
                                    #endregion
                                }
                                else
                                {

                                    filterList = from s in filterList
                                                 where s.DeparturePlace.ToLower().Contains(keywords.KwFrom) &&
                                                       s.DepartureDate >= keywords.KwDepartureDate &&
                                                       s.ReturnDate <= keywords.KwReturnDate &&
                                                       s.Tour.ToPlace.ToLower().Contains(keywords.KwTo)
                                                 select s;
                                }
                            }
                            else if (keywords.KwReturnDate == 0 && keywords.KwDepartureDate > 0)
                            {
                                if (keywords.KwPriceFrom > 0 || keywords.KwPriceTo > 0)
                                {
                                    #region price && departureDate 
                                    if (keywords.KwPriceFrom > 0 && keywords.KwPriceTo > 0)
                                    {
                                        filterList = from s in filterList
                                                     where s.DeparturePlace.ToLower().Contains(keywords.KwFrom) &&
                                                           s.DepartureDate >= keywords.KwDepartureDate &&
                                                           (s.FinalPrice - (s.FinalPrice * s.Promotions.Value / 100)) >= keywords.KwPriceFrom &&
                                                           (s.FinalPrice - (s.FinalPrice * s.Promotions.Value / 100)) <= keywords.KwPriceTo &&
                                                           s.Tour.ToPlace.ToLower().Contains(keywords.KwTo)
                                                     select s;
                                    }
                                    else if (keywords.KwPriceFrom > 0)
                                    {
                                        filterList = from s in filterList
                                                     where s.DeparturePlace.ToLower().Contains(keywords.KwFrom) &&
                                                           s.DepartureDate >= keywords.KwDepartureDate &&
                                                           (s.FinalPrice - (s.FinalPrice * s.Promotions.Value / 100)) <= keywords.KwPriceFrom &&
                                                           s.Tour.ToPlace.ToLower().Contains(keywords.KwTo)
                                                     select s;
                                    }
                                    else if (keywords.KwPriceTo > 0)
                                    {
                                        filterList = from s in filterList
                                                     where s.DeparturePlace.ToLower().Contains(keywords.KwFrom) &&
                                                           s.DepartureDate >= keywords.KwDepartureDate &&
                                                           (s.FinalPrice - (s.FinalPrice * s.Promotions.Value / 100)) >= keywords.KwPriceTo &&
                                                           s.Tour.ToPlace.ToLower().Contains(keywords.KwTo)
                                                     select s;
                                    }
                                    #endregion
                                }
                                else
                                {
                                    filterList = from s in filterList
                                                 where s.DeparturePlace.ToLower().Contains(keywords.KwFrom) &&
                                                       s.DepartureDate >= keywords.KwDepartureDate &&
                                                       s.Tour.ToPlace.ToLower().Contains(keywords.KwTo)
                                                 select s;
                                }
                            }
                            else if (keywords.KwReturnDate > 0 && keywords.KwDepartureDate == 0)
                            {
                                if (keywords.KwPriceFrom > 0 || keywords.KwPriceTo > 0)
                                {
                                    #region price && kwReturnDate
                                    if (keywords.KwPriceFrom > 0 && keywords.KwPriceTo > 0)
                                    {
                                        filterList = from s in filterList
                                                     where s.DeparturePlace.ToLower().Contains(keywords.KwFrom) &&
                                                           s.ReturnDate <= keywords.KwReturnDate &&
                                                           (s.FinalPrice - (s.FinalPrice * s.Promotions.Value / 100)) >= keywords.KwPriceFrom &&
                                                           (s.FinalPrice - (s.FinalPrice * s.Promotions.Value / 100)) <= keywords.KwPriceTo &&
                                                            s.Tour.ToPlace.ToLower().Contains(keywords.KwTo)
                                                     select s;
                                    }
                                    else if (keywords.KwPriceFrom > 0)
                                    {
                                        filterList = from s in filterList
                                                     where s.DeparturePlace.ToLower().Contains(keywords.KwFrom) &&
                                                           s.ReturnDate <= keywords.KwReturnDate &&
                                                           (s.FinalPrice - (s.FinalPrice * s.Promotions.Value / 100)) <= keywords.KwPriceFrom &&
                                                            s.Tour.ToPlace.ToLower().Contains(keywords.KwTo)
                                                     select s;
                                    }
                                    else if (keywords.KwPriceTo > 0)
                                    {
                                        filterList = from s in filterList
                                                     where s.DeparturePlace.ToLower().Contains(keywords.KwFrom) &&
                                                           s.ReturnDate <= keywords.KwReturnDate &&
                                                           (s.FinalPrice - (s.FinalPrice * s.Promotions.Value / 100)) >= keywords.KwPriceTo &&
                                                            s.Tour.ToPlace.ToLower().Contains(keywords.KwTo)
                                                     select s;
                                    }
                                    #endregion
                                }
                                else
                                {
                                    filterList = from s in filterList
                                                 where s.DeparturePlace.ToLower().Contains(keywords.KwFrom) &&
                                                       s.ReturnDate <= keywords.KwReturnDate &&
                                                       s.Tour.ToPlace.ToLower().Contains(keywords.KwTo)
                                                 select s;
                                }
                            }

                        }
                        else
                        {
                            if (kwFrom != "" || kwTo != "")
                            {
                                if (keywords.KwPriceFrom > 0 || keywords.KwPriceTo > 0)
                                {
                                    #region from => to && price
                                    if (keywords.KwPriceFrom > 0 && keywords.KwPriceTo > 0)
                                    {
                                        filterList = from s in filterList
                                                     where s.DeparturePlace.ToLower().Contains(keywords.KwFrom) &&
                                                           (s.FinalPrice - (s.FinalPrice * s.Promotions.Value / 100)) >= keywords.KwPriceFrom &&
                                                           (s.FinalPrice - (s.FinalPrice * s.Promotions.Value / 100)) <= keywords.KwPriceTo &&
                                                            s.Tour.ToPlace.ToLower().Contains(keywords.KwTo)
                                                     select s;
                                    }
                                    else if (keywords.KwPriceTo > 0)
                                    {
                                        filterList = from s in filterList
                                                     where s.DeparturePlace.ToLower().Contains(keywords.KwFrom) &&
                                                            (s.FinalPrice - (s.FinalPrice * s.Promotions.Value / 100)) >= keywords.KwPriceTo &&
                                                             s.Tour.ToPlace.ToLower().Contains(keywords.KwTo)
                                                     select s;
                                    }
                                    else if (keywords.KwPriceFrom > 0)
                                    {
                                        filterList = from s in filterList
                                                     where s.DeparturePlace.ToLower().Contains(keywords.KwFrom) &&
                                                            (s.FinalPrice - (s.FinalPrice * s.Promotions.Value / 100)) <= keywords.KwPriceFrom &&
                                                             s.Tour.ToPlace.ToLower().Contains(keywords.KwTo)
                                                     select s;

                                        var ds = filterList.ToList();
                                    }
                                    #endregion
                                }
                                else
                                {
                                    filterList = from s in filterList
                                                 where s.DeparturePlace.ToLower().Contains(keywords.KwFrom) &&
                                                  s.Tour.ToPlace.ToLower().Contains(keywords.KwTo)
                                                 select s;
                                }
                            }
                            else if (keywords.KwPriceFrom > 0 || keywords.KwPriceTo > 0)
                            {
                                if (keywords.KwPriceFrom > 0 && keywords.KwPriceTo > 0)
                                {
                                    filterList = from s in filterList
                                                 where (s.FinalPrice - (s.FinalPrice * s.Promotions.Value / 100)) >= keywords.KwPriceFrom
                                                    && (s.FinalPrice - (s.FinalPrice * s.Promotions.Value / 100)) <= keywords.KwPriceTo
                                                 select s;
                                }
                                else if (keywords.KwPriceTo > 0)
                                {
                                    filterList = from s in filterList
                                                 where (s.FinalPrice - (s.FinalPrice * s.Promotions.Value / 100)) >= keywords.KwPriceTo
                                                 select s;
                                }
                                else if (keywords.KwPriceFrom > 0)
                                {
                                    filterList = from s in filterList
                                                 where (s.FinalPrice - (s.FinalPrice * s.Promotions.Value / 100)) <= keywords.KwPriceFrom
                                                 select s;

                                }
                            }
                        }
                    }
                }





                #endregion

                if (filterList.Count() > 0)
                {

                    var result = await filterList.ToListAsync();
                    if (result.Count() > 0)
                    {
                        return Ultility.Responses("", Enums.TypeCRUD.Success.ToString(), result);
                    }
                    else
                    {
                        return Ultility.Responses($"Không có dữ liệu trả về !", Enums.TypeCRUD.Warning.ToString());
                    }

                }
                else
                {
                    return Ultility.Responses($"Không có dữ liệu trả về !", Enums.TypeCRUD.Warning.ToString());
                }
            }
            catch (Exception e)
            {
                return Ultility.Responses("Có lỗi xảy ra !", Enums.TypeCRUD.Error.ToString(), description: e.Message);
            }
        }

        //public Response UpdatePromotionTourLastHour(DateTime datetime)
        //{
        //    try
        //    {
        //        var promotion = (from x in _db.Promotions.AsNoTracking()
        //                         where x.IdPromotion == -2
        //                         select x.IdPromotion).FirstOrDefault();

        //        var result = (from x in _db.Schedules.AsNoTracking()
        //                      where x.EndDate <= Ultility.ConvertDatetimeToUnixTimeStampMiliSecond(datetime)
        //                      && x.QuantityCustomer < x.MaxCapacity
        //                      select x).ToList();
        //        foreach (var item in result)
        //        {
        //            item.PromotionId = promotion;
        //        }
        //        SaveChange();
        //        return Ultility.Responses($"Sửa thành công !", Enums.TypeCRUD.Success.ToString());

        //    }
        //    catch (Exception e)
        //    {

        //        return Ultility.Responses("Có lỗi xảy ra !", Enums.TypeCRUD.Error.ToString(), description: e.Message);
        //    }
        //}

        public async Task<bool> IsScheduleInPromotion(string idSchedule)
        {
            var isTourInPromotion = await (from s in _db.Schedules.AsNoTracking()
                                     join p in _db.Promotions.AsNoTracking()
                                     on s.PromotionId equals p.IdPromotion
                                     where s.IdSchedule == idSchedule
                                     && p.Value != 0
                                     select s.IdSchedule).CountAsync();
            if (isTourInPromotion > 0)
                return true;
            return false;
        }

        public async Task<object> ServiceGetSchedule(string idSchedule)
        {

            var schedule = await (from x in _db.Schedules.AsNoTracking()
                                  where x.IdSchedule == idSchedule
                                  select new
                                  {
                                      DepartureDate = x.DepartureDate,
                                      ReturnDate = x.ReturnDate,
                                      DeparturePlace = x.DeparturePlace,
                                      Description = x.Description,
                                      QuantityCustomer = x.QuantityCustomer,    
                                      IdSchedule = x.IdSchedule,
                                      FinalPrice = x.FinalPrice,
                                      FinalPriceHoliday = x.FinalPriceHoliday,
                                      IsHoliday = x.IsHoliday,
                                      ValuePromotion = (from p in _db.Promotions.AsNoTracking()
                                                        where p.IdPromotion == x.PromotionId
                                                        select p.Value).FirstOrDefault(),
                                      PriceChild = x.PriceChild,
                                      PriceChildHoliday = x.PriceChildHoliday,
                                      TourId = x.TourId,
                                      Tour = (from t in _db.Tour.AsNoTracking()
                                              where t.IdTour == x.TourId
                                              select t).FirstOrDefault(),
                                  }).FirstOrDefaultAsync();
            return schedule;
        }

        public async Task<List<string>> ServiceGetListIdScheduleFinished()
        {
            var currentDate = DateTime.Now;
            var day = currentDate.Day;
            var month = currentDate.Month;
            var year = currentDate.Year;

            var dateTimeNow = DateTime.Parse($"{year}/{month}/{day}");
            var unixDateTimeNow = Ultility.ConvertDatetimeToUnixTimeStampMiliSecond(dateTimeNow.AddDays(1).AddMinutes(-1));

            var listIdScheduleFinished = await (from s in _db.Schedules.AsNoTracking()
                                                where s.ReturnDate <= unixDateTimeNow
                                                select s.IdSchedule).ToListAsync();
            return listIdScheduleFinished;
        }


        public async Task<Response> AutomaticAddLastPromotionForSchedule()
        {
            try
            {
                var dateTimeNowConfigMinutes = GetDateTimeNow();
                var dateTimeNowConfigDay = GetDateTimeDayConfig(-3);
                var lsScheduleInLastPromotion = await (from x in _db.Schedules.AsNoTracking()
                                          where x.EndDate <= dateTimeNowConfigDay
                                          && x.ReturnDate >= dateTimeNowConfigMinutes
                                          && x.Isdelete == false
                                          select x).ToListAsync();
                 lsScheduleInLastPromotion.ForEach(x => x.PromotionId = -1);
                foreach (var item in lsScheduleInLastPromotion)
                {
                    UpdateDatabase(item);
                }
                await SaveChangeAsync();
                return Ultility.Responses("", Enums.TypeCRUD.Success.ToString());
            }
            catch (Exception e)
            {
                return Ultility.Responses("Có lỗi xảy ra !", Enums.TypeCRUD.Error.ToString(), description: e.Message);

            }
        }


    }
}