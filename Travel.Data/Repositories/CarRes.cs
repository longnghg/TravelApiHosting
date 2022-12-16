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

    public class CarRes : ICars
    {
        private readonly TravelContext _db;
        private Notification message;
        private readonly ILog _log;
        private Response res;
        private ICache _cache;
        public CarRes(TravelContext db, ILog log, ICache cache)
        {
            _db = db;
            _log = log;
            _cache = cache;
            message = new Notification();
            res = new Response();
        }

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
        private Employee GetCurrentUser(Guid IdUserModify)
        {
            return (from x in _db.Employees.AsNoTracking()
                    where x.IdEmployee == IdUserModify
                    select x).FirstOrDefault();
        }
        public string CheckBeforeSave(JObject frmData, ref Notification _message, bool isUpdate) // hàm đăng nhập  
        {
            try
            {
                var idCar = PrCommon.GetString("idCar", frmData);
                if (!String.IsNullOrEmpty(idCar))
                {
                }

                var nameDriver = PrCommon.GetString("nameDriver", frmData);
                if (!String.IsNullOrEmpty(nameDriver))
                {
                }

                var amountSeat = PrCommon.GetString("amountSeat", frmData);
                if (!String.IsNullOrEmpty(amountSeat))
                {
                }

                var liscenseplate = PrCommon.GetString("liscenseplate", frmData);
                if (!String.IsNullOrEmpty(liscenseplate))
                {
                    var check = CheckLiscensePlate(liscenseplate);
                    if (check.Notification.Type == "Validation" || check.Notification.Type == "Error")
                    {
                        _message = check.Notification;
                        return string.Empty;
                    }
                }


                var phone = PrCommon.GetString("Phone", frmData);
                if (!String.IsNullOrEmpty(phone))
                {
                }

                var status = PrCommon.GetString("status", frmData);
                if (!String.IsNullOrEmpty(status))
                {
                }
                var idUserModify = PrCommon.GetString("idUserModify", frmData);
                if (String.IsNullOrEmpty(idUserModify))
                {
                    idUserModify = Guid.Empty.ToString() ;
                }

                if (isUpdate)
                {
                    UpdateCarViewModel objUpdate = new UpdateCarViewModel();
                    objUpdate.IdCar = Guid.Parse(idCar);
                    objUpdate.NameDriver = nameDriver;
                    objUpdate.AmountSeat = int.Parse(amountSeat);
                    objUpdate.Status = int.Parse(status);
                    objUpdate.LiscensePlate = liscenseplate;
                    objUpdate.Phone = phone;
                    objUpdate.IdUserModify = Guid.Parse(idUserModify);
                    objUpdate.ModifyBy = GetCurrentUser(objUpdate.IdUserModify).NameEmployee;
                    objUpdate.ModifyDate = Ultility.ConvertDatetimeToUnixTimeStampMiliSecond(DateTime.Now);
                    return JsonSerializer.Serialize(objUpdate);
                }

                CreateCarViewModel objCreate = new CreateCarViewModel();
                //objCreate.IdCar = Guid.Parse(idCar);
                objCreate.NameDriver = nameDriver;
                objCreate.AmountSeat = Convert.ToInt16(amountSeat);
                objCreate.Status = Convert.ToInt16(status);
                objCreate.LiscensePlate = liscenseplate.ToString();
                objCreate.Phone = phone;
                if (idUserModify == Guid.Empty.ToString())
                {
                    objCreate.IdUserModify = Guid.Parse(idUserModify);
                    objCreate.ModifyBy = "Vô danh";
                    objCreate.ModifyDate = Ultility.ConvertDatetimeToUnixTimeStampMiliSecond(DateTime.Now);
                    return JsonSerializer.Serialize(objCreate);
                }
                objCreate.IdUserModify = Guid.Parse(idUserModify);
                objCreate.ModifyBy = GetCurrentUser(objCreate.IdUserModify).NameEmployee;
                objCreate.ModifyDate = Ultility.ConvertDatetimeToUnixTimeStampMiliSecond(DateTime.Now);
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

        public Response ViewSelectBoxCar(string idSchedule)
        {
            try
            {
                var carOfSchedule = (from x in _db.Schedules.AsNoTracking()
                                     join
                                        c in _db.Cars.AsNoTracking() on x.CarId equals c.IdCar
                                     where x.IdSchedule == idSchedule
                                     select new
                                     {
                                         LiscensePlate = c.LiscensePlate,
                                         CarId = x.CarId
                                     }).FirstOrDefault();
                return Ultility.Responses("", Enums.TypeCRUD.Success.ToString(), carOfSchedule);

            }
            catch (Exception e)
            {

                return Ultility.Responses("Có lỗi xảy ra !", Enums.TypeCRUD.Error.ToString(), description: e.Message);
            }
        }

        public Response GetsSelectBoxCar(long fromDate, long toDate)
        {
            try
            {
                var unixTimeOneDay = 86400000;

                var listCarShouldRemove1 = (from x in _db.Schedules.AsNoTracking()
                                            where (fromDate >= x.DepartureDate && fromDate < (x.ReturnDate + unixTimeOneDay))
                                            && x.Isdelete == false
                                            orderby x.ReturnDate ascending
                                            select x.CarId);

                var scheduleDepartDateLargerToDate = (from x in _db.Schedules.AsNoTracking()
                                                      where x.DepartureDate >= fromDate
                                                        && x.Isdelete == false
                                                      orderby x.DepartureDate ascending
                                                      select x);
                var listCarShouldRemove2 = (from x in scheduleDepartDateLargerToDate
                                            where !(from s in listCarShouldRemove1 select s).Contains(x.CarId)
                                              && x.Isdelete == false
                                            && (toDate + unixTimeOneDay) > x.DepartureDate
                                            select x.CarId).Distinct();

                var listShouldRemove = listCarShouldRemove1.Concat(listCarShouldRemove2);

                var listCar = (from x in _db.Cars.AsNoTracking()
                               where !listShouldRemove.Any(c => c == x.IdCar)
                                 && x.IsDelete == false
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

        public Response GetsSelectBoxCarUpdate(long fromDate, long toDate, string idSchedule)
        {
            try
            {
                var unixTimeOneDay = 86400000;
                var carOfSchedule = (from x in _db.Schedules.AsNoTracking()
                                     where x.IdSchedule == idSchedule
                                     && x.Isdelete == false
                                     select x).FirstOrDefault();
                var fromDateCurrentUpdate = carOfSchedule.DepartureDate;
                var toDateCurrentUpdate = carOfSchedule.ReturnDate;
                IQueryable<Guid> listCarShouldRemove1;
                IQueryable<Schedule> scheduleDepartDateLargerToDate;
                if (fromDate == fromDateCurrentUpdate && toDate == toDateCurrentUpdate)
                {
                    listCarShouldRemove1 = (from x in _db.Schedules.AsNoTracking()
                                            where x.CarId != carOfSchedule.CarId
                                             && x.Isdelete == false
                                            && (fromDate >= x.DepartureDate && fromDate < (x.ReturnDate + unixTimeOneDay))
                                            orderby x.ReturnDate ascending
                                            select x.CarId);
                    scheduleDepartDateLargerToDate = (from x in _db.Schedules.AsNoTracking()
                                                      where x.CarId != carOfSchedule.CarId
                                                       && x.Isdelete == false
                                                      && x.DepartureDate >= fromDate
                                                      orderby x.DepartureDate ascending
                                                      select x);
                }
                else
                {
                    if ((fromDate >= fromDateCurrentUpdate && fromDate <= toDateCurrentUpdate) || toDate >= fromDateCurrentUpdate && toDate <= toDateCurrentUpdate)
                    {
                        listCarShouldRemove1 = (from x in _db.Schedules.AsNoTracking()
                                                where (fromDate >= x.DepartureDate && fromDate < (x.ReturnDate + unixTimeOneDay))
                                                 && x.Isdelete == false
                                                && x.IdSchedule != idSchedule
                                                orderby x.ReturnDate ascending
                                                select x.CarId);

                        scheduleDepartDateLargerToDate = (from x in _db.Schedules.AsNoTracking()
                                                          where x.DepartureDate >= fromDate
                                                           && x.Isdelete == false
                                                                && x.IdSchedule != idSchedule
                                                          orderby x.DepartureDate ascending
                                                          select x);
                    }
                    else
                    {
                        listCarShouldRemove1 = (from x in _db.Schedules.AsNoTracking()
                                                where (fromDate >= x.DepartureDate && fromDate < (x.ReturnDate + unixTimeOneDay))
                                                 && x.Isdelete == false
                                                orderby x.ReturnDate ascending
                                                select x.CarId);

                        scheduleDepartDateLargerToDate = (from x in _db.Schedules.AsNoTracking()
                                                          where x.DepartureDate >= fromDate
                                                           && x.Isdelete == false
                                                          orderby x.DepartureDate ascending
                                                          select x);
                    }

                }





                var listCarShouldRemove2 = (from x in scheduleDepartDateLargerToDate
                                            where !(from s in listCarShouldRemove1 select s).Contains(x.CarId)
                                            && (toDate + unixTimeOneDay) > x.DepartureDate
                                             && x.Isdelete == false
                                            select x.CarId).Distinct();

                var listShouldRemove = listCarShouldRemove1.Concat(listCarShouldRemove2);


                var listCarCanChoose = (from x in _db.Cars.AsNoTracking()
                                        where !listShouldRemove.Any(c => c == x.IdCar)
                                         && x.IsDelete == false
                                        select x).ToList();
                if (listCarCanChoose.Count() == 0)
                {
                    return Ultility.Responses("Ngày bạn chọn hiện tại không có xe !", Enums.TypeCRUD.Warning.ToString());
                }
                var result = Mapper.MapCar(listCarCanChoose);
                return Ultility.Responses("", Enums.TypeCRUD.Success.ToString(), result);
            }
            catch (Exception e)
            {
                return Ultility.Responses("Có lỗi xảy ra !", Enums.TypeCRUD.Error.ToString(), description: e.Message);
            }
        }

        public Response Gets(bool isDelete)
        {
            try
            {

                var queryListCar = (from x in _db.Cars.AsNoTracking()
                                    where x.IsDelete == isDelete
                                    select x);
                int totalResult = queryListCar.Count();
                var listCar = queryListCar.ToList();
                var result = Mapper.MapCar(listCar);
                var res = Ultility.Responses("", Enums.TypeCRUD.Success.ToString(), result);
                res.TotalResult = totalResult;
                return res;
            }
            catch (Exception e)
            {
                return Ultility.Responses("Có lỗi xảy ra !", Enums.TypeCRUD.Error.ToString(), description: e.Message);
            }
        }
        public Response Create(CreateCarViewModel input, string emailUser)
        {
            try
            {
                Car car = new Car();
                car = Mapper.MapCreateCar(input);
                CreateDatabase<Car>(car);
                string jsonContent = JsonSerializer.Serialize(car);
                SaveChange();
                bool result = _log.AddLog(content: jsonContent, type: "create", emailCreator: emailUser, classContent: "Car");
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

        public Response StatisticCar()
        {
            try
            {
                var lsCarFree = (from x in _db.Cars.AsNoTracking()
                                 where x.Status == (int)Enums.StatusCar.Free
                                 select x).ToList();
                var lsCarBusy = (from x in _db.Cars.AsNoTracking()
                                 where x.Status == (int)Enums.StatusCar.Busy
                                 select x).ToList();

                var lsCarFull = (from x in _db.Cars.AsNoTracking()
                                 where x.Status == (int)Enums.StatusCar.Busy
                                 select x).ToList();

                var lsResult = lsCarFree.Concat(lsCarBusy).Concat(lsCarFull);
                return Ultility.Responses("", Enums.TypeCRUD.Success.ToString(), lsResult);

            }
            catch (Exception e)
            {
                return Ultility.Responses("Có lỗi xảy ra !", Enums.TypeCRUD.Error.ToString(), description: e.Message);

            }
        }

        public Response UpdateCar(UpdateCarViewModel input, string emailUser)
        {
            try
            {
                var userLogin = GetCurrentUser(input.IdUserModify);

                var car = (from x in _db.Cars.AsNoTracking()
                           where x.IdCar == input.IdCar
                           select x).FirstOrDefault();
                car.Status = input.Status;
                car.LiscensePlate = input.LiscensePlate;
                car.NameDriver = input.NameDriver;
                car.Phone = input.Phone;
                car.AmountSeat = input.AmountSeat;
                car.Status = input.Status;
                string jsonContent = JsonSerializer.Serialize(car);
                UpdateDatabase<Car>(car);
                SaveChange();

                bool result = _log.AddLog(content: jsonContent, type: "update", emailCreator: emailUser, classContent: "Car");
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

        public Response DeleteCar(Guid id, Guid idUser, string emailUser)
        {
            try
            {
                var dateTimeNow = Ultility.ConvertDatetimeToUnixTimeStampMiliSecond(DateTime.Now);
                var car = (from x in _db.Cars.AsNoTracking()
                           where x.IdCar == id
                           select x).FirstOrDefault();
                var userLogin = (from x in _db.Employees.AsNoTracking()
                                 where x.IdEmployee == idUser
                                 select x).FirstOrDefault();
                var amountCarInSchedule = (from x in _db.Schedules.AsNoTracking()
                                     where x.CarId == id
                                     && x.ReturnDate > dateTimeNow
                                     select x).Count();
                if (amountCarInSchedule > 0)
                {
                    return Ultility.Responses("Xe đang có lịch trình !", Enums.TypeCRUD.Warning.ToString());
                }
                if (car != null)
                {
                    car.ModifyBy = userLogin.NameEmployee;
                    car.IdUserModify = userLogin.IdEmployee;
                    car.ModifyDate = Ultility.ConvertDatetimeToUnixTimeStampMiliSecond(DateTime.Now);
                    car.IsDelete = true;
                    string jsonContent = JsonSerializer.Serialize(car);
                    UpdateDatabase<Car>(car);
                    SaveChange();

                    bool result = _log.AddLog(content: jsonContent, type: "delete", emailCreator: emailUser, classContent: "Car");
                    if (result)
                    {
                        return Ultility.Responses("Đã xóa !", Enums.TypeCRUD.Success.ToString());
                    }
                    else
                    {
                        return Ultility.Responses("Lỗi log!", Enums.TypeCRUD.Error.ToString());
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

        public Response RestoreCar(Guid id, string emailUser)
        {
            try
            {
                var car = (from x in _db.Cars.AsNoTracking()
                           where x.IdCar == id
                           select x).FirstOrDefault();
                if (car != null)
                {
                    string jsonContent = JsonSerializer.Serialize(car);
                    car.IsDelete = false;
                    UpdateDatabase<Car>(car);
                    SaveChange();

                    bool result = _log.AddLog(content: jsonContent, type: "restore", emailCreator: emailUser, classContent: "Car");
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

        public Response SearchCar(JObject frmData)
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

                var kwName = PrCommon.GetString("nameDriver", frmData);
                if (!String.IsNullOrEmpty(kwName))
                {
                    keywords.KwName = kwName.Trim().ToLower();
                }
                else
                {
                    keywords.KwName = "";
                }

                var kwAmountSeat = PrCommon.GetString("amountSeat", frmData);
                if (!String.IsNullOrEmpty(kwAmountSeat))
                {
                    keywords.KwAmount = int.Parse(kwAmountSeat);
                }
                else
                {
                    keywords.KwAmount = 0;
                }

                var kwLiscensePlate = PrCommon.GetString("liscenseplate", frmData);
                if (!String.IsNullOrEmpty(kwLiscensePlate))
                {
                    keywords.KwLiscensePlate = kwLiscensePlate.Trim().ToLower();
                }
                else
                {
                    keywords.KwLiscensePlate = "";
                }


                var kwPhone = PrCommon.GetString("phone", frmData);
                if (!String.IsNullOrEmpty(kwPhone))
                {
                    keywords.KwPhone = kwPhone.Trim().ToLower();
                }
                else
                {
                    keywords.KwPhone = "";
                }


                var status = PrCommon.GetString("status", frmData);
                keywords.KwStatusList = PrCommon.getListInt(status, ',', false);

                var listCar = new List<Car>();

                if (keywords.KwStatusList.Count > 0)
                {
                    if (!string.IsNullOrEmpty(kwAmountSeat))
                    {
                        var querylistCar = (from x in _db.Cars.AsNoTracking()
                                            where x.IsDelete == keywords.IsDelete &&
                                                            x.AmountSeat.Equals(keywords.KwAmount) &&
                                                             x.NameDriver.ToLower().Contains(keywords.KwName) &&
                                                  x.LiscensePlate.ToLower().Contains(keywords.KwLiscensePlate) &&
                                                  x.Phone.ToLower().Contains(keywords.KwPhone) &&
                                                  keywords.KwStatusList.Contains(x.Status)
                                            orderby x.ModifyDate descending
                                            select x);
                        totalResult = querylistCar.Count();
                        listCar = querylistCar.Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();
                    }
                    else
                    {
                        var querylistCar = (from x in _db.Cars
                                            where x.IsDelete == keywords.IsDelete &&
                                                               x.NameDriver.ToLower().Contains(keywords.KwName) &&
                                                    x.LiscensePlate.ToLower().Contains(keywords.KwLiscensePlate) &&
                                                    x.Phone.ToLower().Contains(keywords.KwPhone) &&
                                                    keywords.KwStatusList.Contains(x.Status)
                                            orderby x.ModifyDate descending
                                            select x);
                        totalResult = querylistCar.Count();
                        listCar = querylistCar.Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();
                    }

                }
                else
                {
                    if (!string.IsNullOrEmpty(kwAmountSeat))
                    {
                        var querylistCar = (from x in _db.Cars.AsNoTracking()
                                            where x.IsDelete == keywords.IsDelete &&
                                                            x.AmountSeat.Equals(keywords.KwAmount) &&
                                                             x.NameDriver.ToLower().Contains(keywords.KwName) &&
                                                  x.LiscensePlate.ToLower().Contains(keywords.KwLiscensePlate) &&
                                                  x.Phone.ToLower().Contains(keywords.KwPhone)
                                            orderby x.ModifyDate descending
                                            select x);
                        totalResult = querylistCar.Count();
                        listCar = querylistCar.Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();
                    }
                    else
                    {
                        var querylistCar = (from x in _db.Cars.AsNoTracking()
                                            where x.IsDelete == keywords.IsDelete &&
                                                  x.NameDriver.ToLower().Contains(keywords.KwName) &&
                                                  x.LiscensePlate.ToLower().Contains(keywords.KwLiscensePlate) &&
                                                  x.Phone.ToLower().Contains(keywords.KwPhone)
                                            orderby x.ModifyDate descending
                                            select x);
                        totalResult = querylistCar.Count();
                        listCar = querylistCar.Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();
                    }
                }


                var result = Mapper.MapCar(listCar);
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

        public Response GetListCarHaveSchedule(Guid idCar, int pageIndex, int pageSize)
        {
            try
            {
                var dateTimeNow = Ultility.ConvertDatetimeToUnixTimeStampMiliSecond(DateTime.Now);
                var lsResult = (from x in _db.Schedules.AsNoTracking()
                                where x.CarId == idCar
                                && x.ReturnDate >= dateTimeNow
                                orderby x.DepartureDate ascending
                                select new Schedule
                                {
                                    BeginDate = x.ReturnDate,
                                    Car = (from c in _db.Cars.AsNoTracking()
                                           where c.IdCar == idCar
                                           select c).FirstOrDefault(),
                                    QuantityCustomer = x.QuantityCustomer,
                                    DepartureDate = x.DepartureDate,
                                    DeparturePlace = x.DeparturePlace,
                                    ReturnDate = x.ReturnDate,
                                    Tour = (from t in _db.Tour.AsNoTracking()
                                            where t.IdTour == x.TourId
                                            select t).FirstOrDefault(),
                                    Employee = (from e in _db.Employees.AsNoTracking()
                                                where e.IdEmployee == x.EmployeeId
                                                select e).FirstOrDefault(),
                                    Status = x.Status
                                });
                var result = lsResult.Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();
                res = Ultility.Responses("", Enums.TypeCRUD.Success.ToString(), result);
                res.TotalResult = lsResult.Count();
                return res;
            }
            catch (Exception e)
            {
                return Ultility.Responses("Có lỗi xảy ra !", Enums.TypeCRUD.Error.ToString(), description: e.Message);
            }
        }
        private Response CheckLiscensePlate(string LiscensePlate)
        {
            try
            {
                    var oriPlateNumber = LiscensePlate.Replace("-", "");
             
                    var obj = (from x in _db.Cars.AsNoTracking()
                               where x.LiscensePlate.Replace("-", "") == oriPlateNumber
                               select x).FirstOrDefault();
                    if (obj != null)
                    {
                        return Ultility.Responses("[" + LiscensePlate + "] này đã được đăng ký !", Enums.TypeCRUD.Validation.ToString());
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
