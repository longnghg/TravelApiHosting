using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
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
using Travel.Shared.ViewModels.Travel.CostTourVM;
using Travel.Shared.ViewModels.Travel.TourVM;
using static Travel.Shared.Ultilities.Enums;

namespace Travel.Data.Repositories
{
    public class CostTourRes : ICostTour
    {
        private readonly TravelContext _db;
        private Notification message;
        private Response res;
        private IConfiguration _config;
        private readonly ILog _log;
        public CostTourRes(TravelContext db, IConfiguration config , ILog log)
        { 
            _db = db;
            message = new Notification();
            res = new Response();
            _log = log;
            _config = config;
        }
        private void UpdateDatabase(CostTour input)
        {
            _db.Entry(input).State = EntityState.Modified;
            _db.SaveChanges();
        }
        private void UpdateDatabaseSchedule(Schedule input)
        {
            _db.Entry(input).State = EntityState.Modified;
            _db.SaveChanges();
        }
        private void DeleteDatabase(CostTour input)
        {
            _db.Entry(input).State = EntityState.Deleted;
            _db.SaveChanges();
        }
        private void CreateDatabase(CostTour input)
        {
            _db.Entry(input).State = EntityState.Added;
            _db.SaveChanges();
        }
        public string CheckBeforSave(JObject frmData, ref Notification _message, bool isUpdate = false)
        {
            try
            {
                var idSchedule = PrCommon.GetString("idSchedule", frmData);
                if (String.IsNullOrEmpty(idSchedule))
                {
                }

                var idScheduleTmp = PrCommon.GetString("idScheduleTmp", frmData);
                if (String.IsNullOrEmpty(idSchedule))
                {
                }

                var hotelId = PrCommon.GetString("hotelId", frmData);
                if (String.IsNullOrEmpty(hotelId))
                {
                }
                var restaurantId = PrCommon.GetString("restaurantId", frmData);
                if (String.IsNullOrEmpty(restaurantId))
                {
                }
                var placeId = PrCommon.GetString("placeId", frmData);
                if (String.IsNullOrEmpty(placeId))
                {
                }

                var breakfast = PrCommon.GetString("breakfast", frmData);
                if (!breakfast.IsNumber())
                {
                }
                var water = PrCommon.GetString("water", frmData);
                if (!water.IsNumber())
                {

                }
                var feeGas = PrCommon.GetString("feeGas", frmData);
                if (!feeGas.IsNumber())
                {
                }
                var distance = PrCommon.GetString("distance", frmData);
                if (!distance.IsNumber())
                {
                }
                var sellCost = PrCommon.GetString("sellCost", frmData);
                if (!sellCost.IsNumber())
                {
                }
                var depreciation = PrCommon.GetString("depreciation", frmData);
                if (!depreciation.IsNumber())
                {
                }
                var otherPrice = PrCommon.GetString("otherPrice", frmData);
                if (!otherPrice.IsNumber())
                {
                }
                var tolls = PrCommon.GetString("tolls", frmData);
                if (!tolls.IsNumber())
                {
                }
                var cusExpected = PrCommon.GetString("cusExpected", frmData);
                if (!cusExpected.IsNumber())
                {
                }
                var insuranceFee = PrCommon.GetString("insuranceFee", frmData);
                if (!insuranceFee.IsNumber())
                {
                }
                var isHoliday = PrCommon.GetString("isHoliday", frmData);
                if (String.IsNullOrEmpty(isHoliday))
                {
                }
                var returnDate = PrCommon.GetString("returnDate", frmData);
                if (!String.IsNullOrEmpty(isHoliday))
                {
                }
                var departureDate = PrCommon.GetString("departureDate", frmData);
                if (!String.IsNullOrEmpty(isHoliday))
                {
                }

                var typeAction = PrCommon.GetString("typeAction", frmData);
                if (isUpdate)
                {
                    // map data
                    UpdateCostViewModel objUpdate = new UpdateCostViewModel();
                    objUpdate.IdSchedule = idSchedule;
                    objUpdate.IdScheduleTmp = idScheduleTmp;
                    objUpdate.Breakfast = float.Parse(breakfast);
                    objUpdate.Water = float.Parse(water);
                    objUpdate.FeeGas = float.Parse(feeGas);
                    objUpdate.Distance = float.Parse(distance);
                    objUpdate.SellCost = float.Parse(sellCost);
                    objUpdate.Depreciation = float.Parse(depreciation);
                    objUpdate.OtherPrice = float.Parse(otherPrice);
                    objUpdate.Tolls = float.Parse(tolls);
                    objUpdate.CusExpected = Convert.ToInt16(cusExpected);
                    objUpdate.InsuranceFee = float.Parse(insuranceFee);
                    objUpdate.IsHoliday = bool.Parse(isHoliday);
                    objUpdate.HotelId = Guid.Parse(hotelId);
                    objUpdate.RestaurantId = Guid.Parse(restaurantId);
                    objUpdate.PlaceId = Guid.Parse(placeId);
                    objUpdate.DepartureDate = DateTime.Parse(departureDate);
                    objUpdate.ReturnDate = DateTime.Parse(returnDate);
                    objUpdate.TypeAction = typeAction;
                    return JsonSerializer.Serialize(objUpdate);
                }
                // map data
                CreateCostViewModel obj = new CreateCostViewModel();
                obj.IdSchedule = idSchedule;
                obj.Breakfast = float.Parse(breakfast);
                obj.Water = float.Parse(water);
                obj.FeeGas = float.Parse(feeGas);
                obj.Distance = float.Parse(distance);
                obj.SellCost = float.Parse(sellCost);
                obj.Depreciation = float.Parse(depreciation);
                obj.OtherPrice = float.Parse(otherPrice);
                obj.Tolls = float.Parse(tolls);
                obj.CusExpected = Convert.ToInt16(cusExpected);
                obj.InsuranceFee = float.Parse(insuranceFee);
                obj.IsHoliday = bool.Parse(isHoliday);
                obj.HotelId = Guid.Parse(hotelId);
                obj.RestaurantId = Guid.Parse(restaurantId);
                obj.PlaceId = Guid.Parse(placeId);
                obj.DepartureDate = DateTime.Parse(departureDate);
                obj.ReturnDate = DateTime.Parse(returnDate);
                return JsonSerializer.Serialize(obj);
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

        public Response Create(CreateCostViewModel input, string emailUser)
        {
            try
            {
                var hotel = (from x in _db.Hotels.AsNoTracking() 
                             where x.IdHotel == input.HotelId select x).FirstOrDefault();
                var restaurant = (from x in _db.Restaurants.AsNoTracking()
                                  where x.IdRestaurant == input.RestaurantId select x).FirstOrDefault();
                var place = (from x in _db.Places.AsNoTracking()
                             where x.IdPlace == input.PlaceId select x).FirstOrDefault();
                CostTour cost =
                cost = Mapper.MapCreateCost(input);
                cost.Approve = (int)ApproveStatus.Waiting;
                cost.TypeAction = "insert";
                cost.PriceHotelDB = hotel.DoubleRoomPrice; // 1
                cost.PriceHotelSR = hotel.SingleRoomPrice;
                cost.PriceRestaurant = restaurant.ComboPrice; // 2000000
                cost.PriceTicketPlace = place.PriceTicket; // 10000
                string jsonContent = JsonSerializer.Serialize(cost);
                CreateDatabase(cost);
                // thêm schedule update giá
                // update price
                float holidayPercent = Convert.ToInt16(_config["PercentHoliday"]);
             
                var schedule = (from x in _db.Schedules.AsNoTracking()
                                where x.IdSchedule == input.IdSchedule select x).First();
                schedule.AdditionalPrice = cost.PriceHotelSR;
                schedule.AdditionalPriceHoliday = (cost.PriceHotelSR + (cost.PriceHotelSR * (holidayPercent / 100)));
                schedule.TotalCostTourNotService = cost.TotalCostTourNotService;

                // số ngày đi của tour
                int countDay = Convert.ToInt16(Ultility.CountDay(input.DepartureDate, input.ReturnDate));
                #region test
                var PriceTicketPlace = cost.PriceTicketPlace;

                var PriceRestaurant = cost.PriceRestaurant;

                var PriceHotelDB = cost.PriceHotelDB;
                #endregion


                float CostService = ((PriceHotelDB + PriceRestaurant + PriceTicketPlace) * countDay);

                float VAT = (schedule.Vat);
                float Profit = (schedule.Profit);
                float totalPriceNotVatAndProfit = cost.TotalCostTourNotService + CostService ;


                float totalPriceIncludeVAT = (totalPriceNotVatAndProfit + (totalPriceNotVatAndProfit * (VAT / 100)));
                float FinalPrice = totalPriceIncludeVAT + (totalPriceIncludeVAT * (Profit / 100));
                schedule.TotalCostTour = totalPriceNotVatAndProfit;
                schedule.FinalPrice = FinalPrice;
                schedule.FinalPriceHoliday = FinalPrice + (FinalPrice * (holidayPercent / 100));

                schedule.PriceAdult = schedule.FinalPrice;
                schedule.PriceChild = schedule.PriceAdult - (schedule.PriceAdult*50/100);
                schedule.PriceBaby = 0;


                schedule.PriceAdultHoliday = schedule.FinalPriceHoliday;
                schedule.PriceChildHoliday = schedule.PriceAdultHoliday - (schedule.PriceAdultHoliday * 50 / 100);
                schedule.PriceBabyHoliday = 0;

                UpdateDatabaseSchedule(schedule);
                bool result = _log.AddLog(content: jsonContent, type: "create", emailCreator: emailUser, classContent: "CostTour");
                if (result)
                {
                    return Ultility.Responses($"Cập nhật giá cho {schedule.IdSchedule} thành công !", Enums.TypeCRUD.Success.ToString());
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

        public Response Get()
        {
            try
            {
                var list = (from x in _db.CostTours select x).ToList();
                var result = Mapper.MapCost(list);
                if (list.Count() > 0)
                {
                   res =      Ultility.Responses("", Enums.TypeCRUD.Success.ToString(), result);

                }
                else
                {
                    res =  Ultility.Responses("Không có dữ liệu trả về !", Enums.TypeCRUD.Warning.ToString());
                }
                return res;
            }
            catch(Exception e)
            {

                return Ultility.Responses("Có lỗi xảy ra !", Enums.TypeCRUD.Error.ToString(), description: e.Message);
            }
        }

        // hàm cần coi lại
        public Response GetCostByIdSchedule(string IdSchedule)
        {
            try
            {
                var cost = (from x in _db.CostTours where x.IdSchedule == IdSchedule select x).First();
                var result = Mapper.MapCost(cost);
                if (result != null)
                {
                    res = Ultility.Responses("", Enums.TypeCRUD.Success.ToString(), result);
                }
                else
                {
                    res = Ultility.Responses("Không có dữ liệu trả về !", Enums.TypeCRUD.Warning.ToString());
                }
                return res;
            }
            catch (Exception e)
            {
                return Ultility.Responses("Có lỗi xảy ra !", Enums.TypeCRUD.Error.ToString(), description: e.Message);
            }
        }

        public Response Update(UpdateCostViewModel input, string emailUser)
        {
            try
            {
                var costTour = (from x in _db.CostTours.AsNoTracking()
                                where x.IdSchedule == input.IdSchedule
                                select x).FirstOrDefault();
                var costTourOld = new CostTour();
                costTourOld = Ultility.DeepCopy<CostTour>(costTour);
                costTourOld.IdSchedule = input.IdScheduleTmp;
                costTourOld.IsTempData = true;

                CreateDatabase(costTourOld);


                CostTour cost = Mapper.MapUpdateCost(input);
                cost.Approve = (int)ApproveStatus.Waiting;
                cost.TypeAction = "update";
                var hotel = (from x in _db.Hotels where x.IdHotel == input.HotelId select x).First();
                var restaurant = (from x in _db.Restaurants where x.IdRestaurant == input.RestaurantId select x).First();
                var place = (from x in _db.Places where x.IdPlace == input.PlaceId select x).First();
                cost.PriceHotelDB = hotel.DoubleRoomPrice;
                cost.PriceHotelSR = hotel.SingleRoomPrice;
                cost.PriceRestaurant = restaurant.ComboPrice;
                cost.PriceTicketPlace = place.PriceTicket;
                string jsonContent = JsonSerializer.Serialize(cost);
                UpdateDatabase(cost);
                // update price
                float holidayPercent = Convert.ToInt16(_config["PercentHoliday"]);

                var schedule = (from x in _db.Schedules.AsNoTracking()
                                where x.IdSchedule == input.IdSchedule
                                select x).First();
                schedule.AdditionalPrice = cost.PriceHotelSR;
                schedule.AdditionalPriceHoliday = (cost.PriceHotelSR + (cost.PriceHotelSR * (holidayPercent / 100)));
                schedule.TotalCostTourNotService = cost.TotalCostTourNotService;

                // số ngày đi của tour
                int countDay = Convert.ToInt16(Ultility.CountDay(input.DepartureDate, input.ReturnDate));
                #region test
                var PriceTicketPlace = cost.PriceTicketPlace;

                var PriceRestaurant = cost.PriceRestaurant;

                var PriceHotelDB = cost.PriceHotelDB;
                #endregion


                float CostService = ((PriceHotelDB + PriceRestaurant + PriceTicketPlace) * countDay);

                float VAT = (schedule.Vat);
                float Profit = (schedule.Profit);
                float totalPriceNotVatAndProfit = cost.TotalCostTourNotService + CostService;


                float totalPriceIncludeVAT = (totalPriceNotVatAndProfit + (totalPriceNotVatAndProfit * (VAT / 100)));
                float FinalPrice = totalPriceIncludeVAT + (totalPriceIncludeVAT * (Profit / 100));
                schedule.TotalCostTour = totalPriceNotVatAndProfit;
                schedule.FinalPrice = FinalPrice;
                schedule.FinalPriceHoliday = FinalPrice + (FinalPrice * (holidayPercent / 100));


                schedule.PriceAdult = schedule.FinalPrice;
                schedule.PriceChild = schedule.PriceAdult - (schedule.PriceAdult * 50 / 100);
                schedule.PriceBaby = 0;


                schedule.PriceAdultHoliday = schedule.FinalPriceHoliday;
                schedule.PriceChildHoliday = schedule.PriceAdultHoliday - (schedule.PriceAdultHoliday * 50 / 100);
                schedule.PriceBabyHoliday = 0;

                UpdateDatabaseSchedule(schedule);
                bool result = _log.AddLog(content: jsonContent, type: "update", emailCreator: emailUser, classContent: "CostTour");
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

    }
}
