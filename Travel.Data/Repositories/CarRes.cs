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
        private Response res;
        public CarRes(TravelContext db)
        {
            _db = db;
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
                objCreate.AmountSeat = int.Parse(amountSeat);
                objCreate.Status = int.Parse(status);
                objCreate.LiscensePlate = liscenseplate;
                objCreate.Phone = phone;
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
                var listCar = (from x in _db.Cars.AsNoTracking()
                               where x.IsDelete == isDelete
                               select x).ToList();
                var result = Mapper.MapCar(listCar);
                return Ultility.Responses("", Enums.TypeCRUD.Success.ToString(), result);
            }
            catch (Exception e)
            {
                return Ultility.Responses("Có lỗi xảy ra !", Enums.TypeCRUD.Error.ToString(), description: e.Message);
            }
        }
        public Response Create(CreateCarViewModel input)
        {
            try
            {
                Car car = new Car();
                car = Mapper.MapCreateCar(input);
                CreateDatabase<Car>(car);
                SaveChange();
                return Ultility.Responses("Thêm thành công !", Enums.TypeCRUD.Success.ToString());
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

        public Response UpdateCar(UpdateCarViewModel input)
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
                UpdateDatabase<Car>(car);
                SaveChange();
                return Ultility.Responses("Sửa thành công !", Enums.TypeCRUD.Success.ToString());
            }
            catch (Exception e)
            {
                return Ultility.Responses("Có lỗi xảy ra !", Enums.TypeCRUD.Error.ToString(), description: e.Message);
            }
        }

        public Response DeleteCar(Guid id, Guid idUser)
        {
            try
            {
                var car = (from x in _db.Cars.AsNoTracking()
                           where x.IdCar == id
                           select x).FirstOrDefault();
                var userLogin = (from x in _db.Employees.AsNoTracking()
                                 where x.IdEmployee == idUser
                                 select x).FirstOrDefault();
                if (car != null)
                {
                    car.ModifyBy = userLogin.NameEmployee;
                    car.IdUserModify = userLogin.IdEmployee;
                    car.ModifyDate = Ultility.ConvertDatetimeToUnixTimeStampMiliSecond(DateTime.Now);
                    car.IsDelete = true;
                    UpdateDatabase<Car>(car);
                    SaveChange();
                    return Ultility.Responses("Đã xóa !", Enums.TypeCRUD.Success.ToString());
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

        public Response RestoreCar(Guid id)
        {
            try
            {
                var car = (from x in _db.Cars.AsNoTracking()
                           where x.IdCar == id
                           select x).FirstOrDefault();
                if (car != null)
                {
                    car.IsDelete = false;
                    UpdateDatabase<Car>(car);
                    SaveChange();

                    return Ultility.Responses("Khôi phục thành công !", Enums.TypeCRUD.Success.ToString());

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
                Keywords keywords = new Keywords();

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

                var kwAmountSeat = PrCommon.GetString("amountSeat", frmData);
                if (!String.IsNullOrEmpty(kwAmountSeat))
                {
                    keywords.KwAmount = int.Parse(kwAmountSeat);
                }

                var kwLiscensePlate = PrCommon.GetString("liscenseplate", frmData);
                if (!String.IsNullOrEmpty(kwLiscensePlate))
                {
                    keywords.KwLiscensePlate = kwLiscensePlate.Trim().ToLower();
                }


                var kwPhone = PrCommon.GetString("phone", frmData);
                if (!String.IsNullOrEmpty(kwPhone))
                {
                    keywords.KwPhone = kwPhone.Trim().ToLower();
                }

                var status = PrCommon.GetString("status", frmData);
                if (!String.IsNullOrEmpty(status))
                {
                }

                var listCar = new List<Car>();
                if (!string.IsNullOrEmpty(isDelete))
                {
                    listCar = (from x in _db.Cars.AsNoTracking()
                               where x.IsDelete == keywords.IsDelete &&
                                               x.NameDriver.ToLower().Contains(keywords.KwName) &&
                                               x.LiscensePlate.ToLower().Contains(keywords.KwLiscensePlate) &&
                                               x.Phone.ToLower().Contains(keywords.KwPhone) &&
                                               x.AmountSeat == keywords.KwAmount
                               select x).ToList();
                }
                else
                {
                    if (!string.IsNullOrEmpty(kwAmountSeat))
                    {
                        listCar = (from x in _db.Cars.AsNoTracking()
                                   where x.IsDelete == keywords.IsDelete &&
                                                   x.AmountSeat.Equals(keywords.KwAmount)
                                   select x).ToList();
                    }
                    else
                    {
                        listCar = (from x in _db.Cars.AsNoTracking()
                                   where x.IsDelete == keywords.IsDelete
                                   select x).ToList();
                    }
                }
                var result = Mapper.MapCar(listCar);
                if (listCar.Count() > 0)
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


    }
}
