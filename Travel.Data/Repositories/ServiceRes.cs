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
using Travel.Shared.ViewModels.Travel.ContractVM;
using static Travel.Shared.Ultilities.Enums;

namespace Travel.Data.Repositories
{
    public class ServiceRes : IService
    {
        private readonly TravelContext _db;
        private Notification message;
        private INotification _notification;
        private readonly ILog _log;
        private Response res;
        public ServiceRes(TravelContext db, INotification notification, ILog log)
        {
            _db = db;
            _log = log;
            message = new Notification();
            _notification = notification;
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
        public string CheckBeforSave(JObject frmData, ref Notification _message, TypeService type, bool isUpdate = false)
        {
            try
            {
                var idHotel = PrCommon.GetString("idHotel", frmData);
                var idPlace = PrCommon.GetString("idPlace", frmData);

                var idRestaurant = PrCommon.GetString("idRestaurant", frmData) ?? "0";

                if (String.IsNullOrEmpty(idHotel))
                {
                }
                var star = PrCommon.GetString("star", frmData);
                if (String.IsNullOrEmpty(star))
                {
                }
                var quantitySR = PrCommon.GetString("quantitySR", frmData);
                if (String.IsNullOrEmpty(quantitySR))
                {
                }
                var quantityDBR = PrCommon.GetString("quantityDBR", frmData);
                if (String.IsNullOrEmpty(quantityDBR))
                {
                }
                var singleRoomPrice = PrCommon.GetString("singleRoomPrice", frmData);
                if (String.IsNullOrEmpty(singleRoomPrice))
                {
                }

                var doubleRoomPrice = PrCommon.GetString("doubleRoomPrice", frmData);
                if (String.IsNullOrEmpty(doubleRoomPrice))
                {
                }
                var comboPrice = PrCommon.GetString("comboPrice", frmData);
                if (String.IsNullOrEmpty(comboPrice))
                {
                }
                var phone = PrCommon.GetString("phone", frmData);
                if (String.IsNullOrEmpty(phone))
                {
                }
                var name = PrCommon.GetString("name", frmData);
                if (String.IsNullOrEmpty(name))
                {
                }
                var nameContract = PrCommon.GetString("nameContract", frmData);
                if (String.IsNullOrEmpty(nameContract))
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
                var priceTicket = PrCommon.GetString("priceTicket", frmData);
                if (String.IsNullOrEmpty(priceTicket))
                {
                }
                var provinceId = PrCommon.GetString("provinceId", frmData);
                if (String.IsNullOrEmpty(provinceId))
                {
                }
                var districtId = PrCommon.GetString("districtId", frmData);
                if (String.IsNullOrEmpty(districtId))
                {
                }
                var wardId = PrCommon.GetString("wardId", frmData);
                if (String.IsNullOrEmpty(wardId))
                {
                }
                var address = PrCommon.GetString("address", frmData);
                if (!String.IsNullOrEmpty(address))
                {
                    if (type == TypeService.Hotel)
                    {
                        var check = CheckAddressHotel(address, provinceId, districtId, wardId);
                        if (check.Notification.Type == Enums.TypeCRUD.Validation.ToString() || check.Notification.Type == Enums.TypeCRUD.Error.ToString())
                        {
                            _message = check.Notification;
                            return string.Empty;
                        }
                    }
                    else if (type == TypeService.Restaurant)
                    {
                        var check = CheckAddressRestaurant(address, provinceId, districtId, wardId);
                        if (check.Notification.Type == Enums.TypeCRUD.Validation.ToString() || check.Notification.Type == Enums.TypeCRUD.Error.ToString())
                        {
                            _message = check.Notification;
                            return string.Empty;
                        }
                    }
                    else
                    {
                        var check = CheckAddressPlace(address, provinceId, districtId, wardId);
                        if (check.Notification.Type == Enums.TypeCRUD.Validation.ToString() || check.Notification.Type == Enums.TypeCRUD.Error.ToString())
                        {
                            _message = check.Notification;
                            return string.Empty;
                        }
                    }

                }
                if (isUpdate)
                {
                    if (type == TypeService.Hotel)
                    {
                        UpdateHotelViewModel uHotelObj = new UpdateHotelViewModel();
                        uHotelObj.IdHotel = Guid.Parse(idHotel);
                        uHotelObj.Address = address;
                        uHotelObj.DoubleRoomPrice = float.Parse(doubleRoomPrice);
                        uHotelObj.Name = name;
                        uHotelObj.Phone = phone;
                        uHotelObj.QuantityDBR = Convert.ToInt16(quantityDBR);
                        uHotelObj.QuantitySR = Convert.ToInt16(quantitySR);
                        uHotelObj.SingleRoomPrice = float.Parse(singleRoomPrice);
                        uHotelObj.Star = Convert.ToInt16(star);
                        uHotelObj.IdUserModify = Guid.Parse(idUserModify);
                        uHotelObj.TypeAction = "update";
                        uHotelObj.ModifyBy = GetCurrentUser(uHotelObj.IdUserModify).NameEmployee;
                        uHotelObj.ModifyDate = Ultility.ConvertDatetimeToUnixTimeStampMiliSecond(DateTime.Now);
                        uHotelObj.ProvinceId = Guid.Parse(provinceId);
                        uHotelObj.DistrictId = Guid.Parse(districtId);
                        uHotelObj.WardId = Guid.Parse(wardId);
                        return JsonSerializer.Serialize(uHotelObj);

                    }
                    else if (type == TypeService.Restaurant)
                    {
                        UpdateRestaurantViewModel uRestaurantObj = new UpdateRestaurantViewModel();
                        uRestaurantObj.IdRestaurant = Guid.Parse(idRestaurant);
                        uRestaurantObj.Address = address;
                        uRestaurantObj.Name = name;
                        uRestaurantObj.Phone = phone;
                        uRestaurantObj.IdUserModify = Guid.Parse(idUserModify);
                        uRestaurantObj.ComboPrice = float.Parse(comboPrice);
                        uRestaurantObj.TypeAction = "update";
                        uRestaurantObj.ModifyBy = GetCurrentUser(uRestaurantObj.IdUserModify).NameEmployee;
                        uRestaurantObj.ModifyDate = Ultility.ConvertDatetimeToUnixTimeStampMiliSecond(DateTime.Now);
                        uRestaurantObj.ProvinceId = Guid.Parse(provinceId);
                        uRestaurantObj.DistrictId = Guid.Parse(districtId);
                        uRestaurantObj.WardId = Guid.Parse(wardId);
                        return JsonSerializer.Serialize(uRestaurantObj);
                    }
                    else
                    {
                        UpdatePlaceViewModel uPlaceObj = new UpdatePlaceViewModel();
                        uPlaceObj.IdPlace = Guid.Parse(idPlace);
                        uPlaceObj.PriceTicket = float.Parse(priceTicket);
                        uPlaceObj.Address = address;
                        uPlaceObj.Name = name;
                        uPlaceObj.Phone = phone;
                        uPlaceObj.IdUserModify = Guid.Parse(idUserModify);
                        uPlaceObj.TypeAction = "update";
                        uPlaceObj.ModifyBy = GetCurrentUser(uPlaceObj.IdUserModify).NameEmployee;
                        uPlaceObj.ModifyDate = Ultility.ConvertDatetimeToUnixTimeStampMiliSecond(DateTime.Now);
                        uPlaceObj.ProvinceId = Guid.Parse(provinceId);
                        uPlaceObj.DistrictId = Guid.Parse(districtId);
                        uPlaceObj.WardId = Guid.Parse(wardId);
                        return JsonSerializer.Serialize(uPlaceObj);
                    }
                }
                else
                {
                    if (type == TypeService.Hotel)
                    {
                        CreateHotelViewModel hotelObj = new CreateHotelViewModel();
                        hotelObj.Address = address;
                        hotelObj.DoubleRoomPrice = float.Parse(doubleRoomPrice);
                        hotelObj.Name = name;
                        hotelObj.Phone = phone;
                        hotelObj.QuantityDBR = Convert.ToInt16(quantityDBR);
                        hotelObj.QuantitySR = Convert.ToInt16(quantitySR);
                        hotelObj.SingleRoomPrice = float.Parse(singleRoomPrice);
                        hotelObj.Star = Convert.ToInt16(star);
                        hotelObj.NameContract = nameContract;
                        hotelObj.IdUserModify = Guid.Parse(idUserModify);
                        hotelObj.ModifyBy = GetCurrentUser(hotelObj.IdUserModify).NameEmployee;
                        hotelObj.ModifyDate = Ultility.ConvertDatetimeToUnixTimeStampMiliSecond(DateTime.Now);
                        hotelObj.TypeAction = "insert";
                        hotelObj.ProvinceId = Guid.Parse(provinceId);
                        hotelObj.DistrictId = Guid.Parse(districtId);
                        hotelObj.WardId = Guid.Parse(wardId);
                        return JsonSerializer.Serialize(hotelObj);

                    }
                    else if (type == TypeService.Restaurant)
                    {
                        CreateRestaurantViewModel restaurantObj = new CreateRestaurantViewModel();
                        restaurantObj.Address = address;
                        restaurantObj.Name = name;
                        restaurantObj.Phone = phone;
                        restaurantObj.NameContract = nameContract;
                        restaurantObj.ComboPrice = float.Parse(comboPrice);
                        restaurantObj.IdUserModify = Guid.Parse(idUserModify);
                        restaurantObj.ModifyBy = GetCurrentUser(restaurantObj.IdUserModify).NameEmployee;
                        restaurantObj.ModifyDate = Ultility.ConvertDatetimeToUnixTimeStampMiliSecond(DateTime.Now);
                        restaurantObj.TypeAction = "insert";
                        restaurantObj.ProvinceId = Guid.Parse(provinceId);
                        restaurantObj.DistrictId = Guid.Parse(districtId);
                        restaurantObj.WardId = Guid.Parse(wardId);
                        return JsonSerializer.Serialize(restaurantObj);
                    }
                    else
                    {
                        CreatePlaceViewModel placeObj = new CreatePlaceViewModel();
                        placeObj.PriceTicket = float.Parse(priceTicket);
                        placeObj.Address = address;
                        placeObj.Name = name;
                        placeObj.Phone = phone;
                        placeObj.NameContract = nameContract;
                        placeObj.IdUserModify = Guid.Parse(idUserModify);
                        placeObj.ModifyBy = GetCurrentUser(placeObj.IdUserModify).NameEmployee;
                        placeObj.ModifyDate = Ultility.ConvertDatetimeToUnixTimeStampMiliSecond(DateTime.Now);
                        placeObj.TypeAction = "insert";
                        placeObj.ProvinceId = Guid.Parse(provinceId);
                        placeObj.DistrictId = Guid.Parse(districtId);
                        placeObj.WardId = Guid.Parse(wardId);
                        return JsonSerializer.Serialize(placeObj);
                    }
                }

            }
            catch (Exception e)
            {
                _message = Ultility.Responses("Có lỗi xảy ra !", Enums.TypeCRUD.Error.ToString(), description: e.Message).Notification;
                return string.Empty;
            }
        }
        #region Hotel
        public Response GetsWaitingHotel(Guid idUser, int pageIndex, int pageSize)
        {
            try
            {
                var totalResult = 0;
                var userLogin = (from x in _db.Employees.AsNoTracking()
                                 where x.IdEmployee == idUser
                                 select x).FirstOrDefault();
                var listWaiting = new List<Hotel>();
                if (userLogin.RoleId == (int)Enums.TitleRole.Admin)
                {
                    var querylistWaiting = (from x in _db.Hotels.AsNoTracking()
                                            where x.Approve == Convert.ToInt16(ApproveStatus.Waiting)
                                            orderby x.ModifyDate descending
                                            select x);
                    totalResult = querylistWaiting.Count();
                    listWaiting = querylistWaiting.Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();
                }
                else
                {
                    var querylistWaiting = (from x in _db.Hotels.AsNoTracking()
                                            where x.IdUserModify == idUser
                                            && x.Approve == Convert.ToInt16(ApproveStatus.Waiting)
                                            orderby x.ModifyDate descending
                                            select x);
                    totalResult = querylistWaiting.Count();
                    listWaiting = querylistWaiting.Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();
                }
                var result = Mapper.MapHotel(listWaiting);
                var res = Ultility.Responses("", Enums.TypeCRUD.Success.ToString(), result);
                res.TotalResult = totalResult;
                return res;
            }
            catch (Exception e)
            {
                return Ultility.Responses("Có lỗi xảy ra !", Enums.TypeCRUD.Error.ToString(), description: e.Message);
            }
        }
        public Response GetsHotel(bool isDelete)
        {
            try
            {
                var list = (from x in _db.Hotels.AsNoTracking()
                            where
                            x.IsDelete == isDelete &&
                            x.IsTempdata == false &&
                            x.Approve == Convert.ToInt16(Enums.ApproveStatus.Approved)
                            orderby x.Star descending
                            select x).ToList();
                var result = Mapper.MapHotel(list);
                return Ultility.Responses("", Enums.TypeCRUD.Success.ToString(), result);
            }
            catch (Exception e)
            {
                return Ultility.Responses("Có lỗi xảy ra !", Enums.TypeCRUD.Error.ToString(), description: e.Message);
            }
        }

        private Response CheckAddressHotel(string Address, string ProvinceId, string DistricId, string WardId)
        {
            try
            {
                var oriAddress = Address.Replace(" ", "");

                var obj = (from x in _db.Hotels.AsNoTracking()
                           where x.Address.Replace(" ", "") == oriAddress
                           && x.ProvinceId == Guid.Parse(ProvinceId)
                           && x.DistrictId == Guid.Parse(DistricId)
                           && x.WardId == Guid.Parse(WardId)
                           select x).FirstOrDefault();
                if (obj != null)
                {
                    return Ultility.Responses("Địa chỉ [" + Address + "] này đã được đăng ký !", Enums.TypeCRUD.Validation.ToString(), description: "address");
                }
                return res;

            }
            catch (Exception e)
            {

                return Ultility.Responses("Có lỗi xảy ra !", Enums.TypeCRUD.Error.ToString(), description: e.Message);

            }
        }
        private Response CheckAddressPlace(string Address, string ProvinceId, string DistricId, string WardId)
        {
            try
            {
                var oriAddress = Address.Replace(" ", "");

                var obj = (from x in _db.Places.AsNoTracking()
                           where x.Address.Replace(" ", "") == oriAddress
                           && x.ProvinceId == Guid.Parse(ProvinceId)
                           && x.DistrictId == Guid.Parse(DistricId)
                           && x.WardId == Guid.Parse(WardId)
                           select x).FirstOrDefault();
                if (obj != null)
                {
                    return Ultility.Responses("Địa chỉ [" + Address + "] này đã được đăng ký !", Enums.TypeCRUD.Validation.ToString(), description: "address");
                }
                return res;

            }
            catch (Exception e)
            {

                return Ultility.Responses("Có lỗi xảy ra !", Enums.TypeCRUD.Error.ToString(), description: e.Message);

            }
        }
        private Response CheckAddressRestaurant(string Address, string ProvinceId, string DistricId, string WardId)
        {
            try
            {
                var oriAddress = Address.Replace(" ", "");

                var obj = (from x in _db.Restaurants.AsNoTracking()
                           where x.Address.Replace(" ", "") == oriAddress
                           && x.ProvinceId == Guid.Parse(ProvinceId)
                           && x.DistrictId == Guid.Parse(DistricId)
                           && x.WardId == Guid.Parse(WardId)
                           select x).FirstOrDefault();
                if (obj != null)
                {
                    return Ultility.Responses("Địa chỉ [" + Address + "] này đã được đăng ký !", Enums.TypeCRUD.Validation.ToString(), description: "address");
                }
                return res;

            }
            catch (Exception e)
            {

                return Ultility.Responses("Có lỗi xảy ra !", Enums.TypeCRUD.Error.ToString(), description: e.Message);

            }
        }

        public Response CreateHotel(CreateHotelViewModel input, string emailUser)
        {
            try
            {
                var user = GetCurrentUser(input.IdUserModify);
                input.ModifyBy = user.NameEmployee;
                Hotel hotel = Mapper.MapCreateHotel(input);

                string jsonContent = JsonSerializer.Serialize(hotel);
                CreateDatabase<Hotel>(hotel);
                SaveChange();

                var listRole = new int[] { Convert.ToInt16(Enums.TitleRole.Admin), Convert.ToInt16(Enums.TitleRole.LocalManager) };
                _notification.CreateNotification(user.IdEmployee, Convert.ToInt16(Enums.TypeNotification.Hotel), hotel.NameHotel, listRole, "");

                bool result = _log.AddLog(content: jsonContent, type: "create", emailCreator: emailUser, classContent: "Hotel");
                if (result)
                {
                    return Ultility.Responses("Đã gửi yêu cầu thêm !", Enums.TypeCRUD.Success.ToString());

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
        public Response DeleteHotel(Guid id, Guid idUser, string emailUser)
        {
            try
            {
                var hotel = (from x in _db.Hotels.AsNoTracking()
                             where x.IdHotel == id
                             select x).FirstOrDefault();

                var userLogin = (from x in _db.Employees.AsNoTracking()
                                 where x.IdEmployee == idUser
                                 select x).FirstOrDefault();
                if (hotel.Approve == (int)ApproveStatus.Approved)
                {
                    hotel.ModifyBy = userLogin.NameEmployee;
                    hotel.TypeAction = "delete";
                    hotel.IdUserModify = userLogin.IdEmployee;
                    hotel.ModifyDate = Ultility.ConvertDatetimeToUnixTimeStampMiliSecond(DateTime.Now);
                    hotel.Approve = (int)ApproveStatus.Waiting;

                    string jsonContent = JsonSerializer.Serialize(hotel);
                    // bổ sung isdelete
                    hotel.IsDelete = true;

                    UpdateDatabase<Hotel>(hotel);
                    SaveChange();

                    var listRole = new int[] { Convert.ToInt16(Enums.TitleRole.Admin), Convert.ToInt16(Enums.TitleRole.LocalManager) };
                    _notification.CreateNotification(userLogin.IdEmployee, Convert.ToInt16(Enums.TypeNotification.Hotel), hotel.NameHotel, listRole, "");
                    bool result = _log.AddLog(content: jsonContent, type: "delete", emailCreator: emailUser, classContent: "Hotel");
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
                    if (hotel.IdUserModify == idUser)
                    {
                        if (hotel.TypeAction == "insert")
                        {
                            DeleteDatabase<Hotel>(hotel);
                            SaveChange();
                            return Ultility.Responses("Đã hủy yêu cầu thêm !", Enums.TypeCRUD.Success.ToString());
                        }
                        else if (hotel.TypeAction == "update")
                        {
                            var idHotelTemp = hotel.IdAction;
                            // old hotel
                            var hotelTemp = (from x in _db.Hotels.AsNoTracking()
                                             where x.IdHotel == Guid.Parse(idHotelTemp)
                                             select x).FirstOrDefault();
                            hotel.Approve = (int)ApproveStatus.Approved;

                            hotel.IdAction = null;
                            hotel.TypeAction = null;

                            #region restore old data
                            hotel.Approve = (int)ApproveStatus.Approved;


                            hotel.Address = hotelTemp.Address;
                            hotel.DoubleRoomPrice = hotelTemp.DoubleRoomPrice;
                            hotel.SingleRoomPrice = hotelTemp.SingleRoomPrice;
                            hotel.NameHotel = hotelTemp.NameHotel;
                            hotel.Phone = hotelTemp.Phone;
                            hotel.QuantityDBR = hotelTemp.QuantityDBR;
                            hotel.QuantitySR = hotelTemp.QuantitySR;
                            hotel.Star = hotelTemp.Star;
                            #endregion
                            DeleteDatabase<Hotel>(hotelTemp);
                            SaveChange();
                            return Ultility.Responses("Đã hủy yêu cầu chỉnh sửa !", Enums.TypeCRUD.Success.ToString());

                        }

                        else if (hotel.TypeAction == "restore")
                        {
                            hotel.IdAction = null;
                            hotel.TypeAction = null;
                            hotel.IsDelete = true;
                            hotel.Approve = (int)ApproveStatus.Approved;
                            UpdateDatabase<Hotel>(hotel);
                            SaveChange();
                            return Ultility.Responses("Đã hủy yêu cầu khôi phục !", Enums.TypeCRUD.Success.ToString());

                        }
                        else // delete
                        {
                            hotel.IdAction = null;
                            hotel.TypeAction = null;
                            hotel.IsDelete = false;
                            hotel.Approve = (int)ApproveStatus.Approved;
                            UpdateDatabase<Hotel>(hotel);
                            SaveChange();
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
        public Response RestoreHotel(Guid id, Guid idUser, string emailUser)
        {
            try
            {
                var hotel = (from x in _db.Hotels.AsNoTracking()
                             where x.IdHotel == id
                             select x).FirstOrDefault();

                var userLogin = (from x in _db.Employees.AsNoTracking()
                                 where x.IdEmployee == idUser
                                 select x).FirstOrDefault();
                if (hotel.Approve == (int)ApproveStatus.Approved)
                {
                    hotel.ModifyBy = userLogin.NameEmployee;
                    hotel.TypeAction = "restore";
                    hotel.IdUserModify = userLogin.IdEmployee;
                    hotel.ModifyDate = Ultility.ConvertDatetimeToUnixTimeStampMiliSecond(DateTime.Now);
                    hotel.Approve = (int)ApproveStatus.Waiting;
                    // bổ sung isdelete
                    hotel.IsDelete = false;
                }

                string jsonContent = JsonSerializer.Serialize(hotel);
                UpdateDatabase<Hotel>(hotel);
                SaveChange();
                var listRole = new int[] { Convert.ToInt16(Enums.TitleRole.Admin), Convert.ToInt16(Enums.TitleRole.LocalManager) };
                _notification.CreateNotification(userLogin.IdEmployee, Convert.ToInt16(Enums.TypeNotification.Hotel), hotel.NameHotel, listRole, "");

                bool result = _log.AddLog(content: jsonContent, type: "restore", emailCreator: emailUser, classContent: "Hotel");
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

        public Response UpdateHotel(UpdateHotelViewModel input, string emailUser)
        {
            try
            {
                var userLogin = GetCurrentUser(input.IdUserModify);

                var hotel = (from x in _db.Hotels.AsNoTracking()
                             where x.IdHotel == input.IdHotel
                             select x).FirstOrDefault();

                // clone new object
                var hotelOld = new Hotel();
                hotelOld = Ultility.DeepCopy<Hotel>(hotel);
                hotelOld.IdAction = hotelOld.IdHotel.ToString();
                hotelOld.IdHotel = Guid.NewGuid();
                hotelOld.IsTempdata = true;

                string jsonContent = JsonSerializer.Serialize(hotel);
                CreateDatabase<Hotel>(hotelOld);

                #region setdata
                hotel.IdAction = hotelOld.IdHotel.ToString();
                hotel.IdUserModify = input.IdUserModify;
                hotel.TypeAction = input.TypeAction;
                hotel.Approve = (int)ApproveStatus.Waiting;
                hotel.ModifyBy = userLogin.NameEmployee;
                hotel.ModifyDate = Ultility.ConvertDatetimeToUnixTimeStampMiliSecond(DateTime.Now);


                hotel.Address = input.Address;
                hotel.DoubleRoomPrice = input.DoubleRoomPrice;
                hotel.SingleRoomPrice = input.SingleRoomPrice;
                hotel.NameHotel = input.Name;
                hotel.Phone = input.Phone;
                hotel.QuantityDBR = input.QuantityDBR;
                hotel.QuantitySR = input.QuantitySR;
                hotel.Star = input.Star;
                hotel.ProvinceId = input.ProvinceId;
                hotel.WardId = input.WardId;
                hotel.DistrictId = input.DistrictId;
                #endregion
                UpdateDatabase<Hotel>(hotel);
                SaveChange();

                var listRole = new int[] { Convert.ToInt16(Enums.TitleRole.Admin), Convert.ToInt16(Enums.TitleRole.LocalManager) };
                _notification.CreateNotification(userLogin.IdEmployee, Convert.ToInt16(Enums.TypeNotification.Hotel), hotel.NameHotel, listRole, "");

                bool result = _log.AddLog(content: jsonContent, type: "update", emailCreator: emailUser, classContent: "Hotel");
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
        public Response ApproveHotel(Guid id)
        {
            try
            {
                var hotel = (from x in _db.Hotels.AsNoTracking()
                             where x.IdHotel == id
                             && x.Approve == (int)ApproveStatus.Waiting
                             select x).FirstOrDefault();
                if (hotel != null)
                {

                    if (hotel.TypeAction == "update")
                    {
                        var idHotelTemp = hotel.IdAction;
                        hotel.Approve = (int)ApproveStatus.Approved;
                        hotel.IdAction = null;
                        hotel.TypeAction = null;


                        // delete tempdata
                        var hotelTemp = (from x in _db.Hotels.AsNoTracking()
                                         where x.IdHotel == Guid.Parse(idHotelTemp)
                                         select x).FirstOrDefault();
                        DeleteDatabase<Hotel>(hotelTemp);
                    }
                    else if (hotel.TypeAction == "insert")
                    {
                        hotel.IdAction = null;
                        hotel.TypeAction = null;
                        hotel.Approve = (int)ApproveStatus.Approved;
                    }
                    else if (hotel.TypeAction == "restore")
                    {
                        hotel.IdAction = null;
                        hotel.TypeAction = null;
                        hotel.Approve = (int)ApproveStatus.Approved;
                        hotel.IsDelete = false;

                    }
                    else
                    {
                        hotel.IdAction = null;
                        hotel.TypeAction = null;
                        hotel.Approve = (int)ApproveStatus.Approved;
                        hotel.IsDelete = true;
                    }


                    UpdateDatabase<Hotel>(hotel);
                    SaveChange();

                    var userModify = GetCurrentUser(hotel.IdUserModify);
                    _notification.CreateNotification(userModify.IdEmployee, Convert.ToInt16(Enums.TypeNotification.Hotel), hotel.NameHotel, new int[] { userModify.RoleId }, "Thành công");
                    return Ultility.Responses($"Phê duyệt thành công !", Enums.TypeCRUD.Success.ToString());
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

        public Response RefusedHotel(Guid id)
        {
            try
            {
                var hotel = (from x in _db.Hotels.AsNoTracking()
                             where x.IdHotel == id
                             && x.Approve == (int)ApproveStatus.Waiting
                             select x).FirstOrDefault();
                if (hotel != null)
                {
                    if (hotel.TypeAction == "update")
                    {
                        var idHotelTemp = hotel.IdAction;
                        // old hotel
                        var hotelTemp = (from x in _db.Hotels
                                         where x.IdHotel == Guid.Parse(idHotelTemp)
                                         select x).FirstOrDefault();
                        hotel.Approve = (int)ApproveStatus.Approved;

                        hotel.IdAction = null;
                        hotel.TypeAction = null;

                        #region restore old data
                        hotel.Approve = (int)ApproveStatus.Approved;


                        hotel.Address = hotelTemp.Address;
                        hotel.DoubleRoomPrice = hotelTemp.DoubleRoomPrice;
                        hotel.SingleRoomPrice = hotelTemp.SingleRoomPrice;
                        hotel.NameHotel = hotelTemp.NameHotel;
                        hotel.Phone = hotelTemp.Phone;
                        hotel.QuantityDBR = hotelTemp.QuantityDBR;
                        hotel.QuantitySR = hotelTemp.QuantitySR;
                        hotel.Star = hotelTemp.Star;
                        #endregion

                        DeleteDatabase<Hotel>(hotelTemp);
                    }
                    else if (hotel.TypeAction == "insert")
                    {
                        hotel.IdAction = null;
                        hotel.TypeAction = null;
                        hotel.Approve = (int)ApproveStatus.Refused;
                    }
                    else if (hotel.TypeAction == "restore")
                    {
                        hotel.IdAction = null;
                        hotel.TypeAction = null;
                        hotel.IsDelete = true;
                        hotel.Approve = (int)ApproveStatus.Approved;
                    }
                    else // delete
                    {
                        hotel.IdAction = null;
                        hotel.TypeAction = null;
                        hotel.IsDelete = false;
                        hotel.Approve = (int)ApproveStatus.Approved;
                    }
                    UpdateDatabase<Hotel>(hotel);
                    SaveChange();
                    var userModify = GetCurrentUser(hotel.IdUserModify);
                    _notification.CreateNotification(userModify.IdEmployee, Convert.ToInt16(Enums.TypeNotification.Hotel), hotel.NameHotel, new int[] { userModify.RoleId }, "Từ chối");
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
        #endregion
        #region Restaurant
        public Response GetsRestaurant(bool isDelete)
        {
            try
            {
                var list = (from x in _db.Restaurants
                            where
                            x.IsDelete == isDelete &&
                            x.IsTempdata == false &&
                            x.Approve == Convert.ToInt16(ApproveStatus.Approved)
                            select x).ToList();
                var result = Mapper.MapRestaurant(list);
                return Ultility.Responses("", Enums.TypeCRUD.Success.ToString(), result);

            }
            catch (Exception e)
            {
                return Ultility.Responses("Có lỗi xảy ra !", Enums.TypeCRUD.Error.ToString(), description: e.Message);
            }
        }

        public Response GetsWaitingRestaurant(Guid idUser, int pageIndex, int pageSize)
        {
            try
            {
                var totalResult = 0;
                var userLogin = (from x in _db.Employees
                                 where x.IdEmployee == idUser
                                 select x).FirstOrDefault();
                var listWaiting = new List<Restaurant>();
                if (userLogin.RoleId == (int)Enums.TitleRole.Admin)
                {
                    var querylistWaiting = (from x in _db.Restaurants
                                            where x.Approve == Convert.ToInt16(ApproveStatus.Waiting)
                                            select x);
                    totalResult = querylistWaiting.Count();
                    listWaiting = querylistWaiting.Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();
                }
                else
                {
                    var querylistWaiting = (from x in _db.Restaurants
                                            where x.IdUserModify == idUser
                                            && x.Approve == Convert.ToInt16(ApproveStatus.Waiting)
                                            select x);

                    totalResult = querylistWaiting.Count();
                    listWaiting = querylistWaiting.Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();
                }

                var result = Mapper.MapRestaurant(listWaiting);

                var res = Ultility.Responses("", Enums.TypeCRUD.Success.ToString(), result);
                res.TotalResult = totalResult;
                return res;
            }
            catch (Exception e)
            {
                return Ultility.Responses("Có lỗi xảy ra !", Enums.TypeCRUD.Error.ToString(), description: e.Message);
            }
        }
        public Response DeleteRestaurant(Guid id, Guid idUser, string emailUser)
        {
            try
            {
                var restaurant = (from x in _db.Restaurants.AsNoTracking()
                                  where x.IdRestaurant == id
                                  select x).FirstOrDefault();
                var userLogin = (from x in _db.Employees
                                 where x.IdEmployee == idUser
                                 select x).FirstOrDefault();
                if (restaurant.Approve == (int)ApproveStatus.Approved)
                {
                    restaurant.ModifyBy = userLogin.NameEmployee;
                    restaurant.IdUserModify = userLogin.IdEmployee;
                    restaurant.ModifyDate = Ultility.ConvertDatetimeToUnixTimeStampMiliSecond(DateTime.Now);
                    restaurant.Approve = (int)ApproveStatus.Waiting;
                    restaurant.TypeAction = "delete";
                    string jsonContent = JsonSerializer.Serialize(restaurant);

                    UpdateDatabase<Restaurant>(restaurant);
                    SaveChange();
                    var listRole = new int[] { Convert.ToInt16(Enums.TitleRole.Admin), Convert.ToInt16(Enums.TitleRole.LocalManager) };
                    _notification.CreateNotification(userLogin.IdEmployee, Convert.ToInt16(Enums.TypeNotification.Restaurant), restaurant.NameRestaurant, listRole, "");
                    return Ultility.Responses("Đã gửi yêu cầu xóa !", Enums.TypeCRUD.Success.ToString());
                    bool result = _log.AddLog(content: jsonContent, type: "delete", emailCreator: emailUser, classContent: "Restaurant");
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
                    if (restaurant.IdUserModify == idUser)
                    {
                        if (restaurant.TypeAction == "insert")
                        {
                            _db.Restaurants.Remove(restaurant);
                            return Ultility.Responses("Đã xóa!", Enums.TypeCRUD.Success.ToString());
                        }
                        else if (restaurant.TypeAction == "update")
                        {
                            var idRestaurantTemp = restaurant.IdAction;
                            var restaurantTemp = (from x in _db.Restaurants
                                                  where x.IdRestaurant == Guid.Parse(idRestaurantTemp)
                                                  select x).FirstOrDefault();
                            restaurant.Approve = (int)ApproveStatus.Approved;

                            restaurant.IdAction = null;
                            restaurant.TypeAction = null;

                            #region restore old data
                            restaurant.Approve = (int)ApproveStatus.Approved;


                            restaurant.Address = restaurantTemp.Address;

                            restaurant.Phone = restaurantTemp.Phone;
                            restaurant.NameRestaurant = restaurantTemp.NameRestaurant;
                            restaurant.ComboPrice = restaurantTemp.ComboPrice;
                            #endregion

                            _db.Restaurants.Remove(restaurantTemp);

                            UpdateDatabase<Restaurant>(restaurant);
                            SaveChange();


                            return Ultility.Responses("Đã hủy yêu cầu chỉnh sửa !", Enums.TypeCRUD.Success.ToString());

                        }
                        else if (restaurant.TypeAction == "restore")
                        {
                            restaurant.IdAction = null;
                            restaurant.TypeAction = null;
                            restaurant.IsDelete = true;
                            restaurant.Approve = (int)ApproveStatus.Approved;


                            UpdateDatabase<Restaurant>(restaurant);
                            SaveChange();

                            return Ultility.Responses("Đã hủy yêu cầu khôi phục !", Enums.TypeCRUD.Success.ToString());

                        }
                        else //delete
                        {
                            restaurant.IdAction = null;
                            restaurant.TypeAction = null;
                            restaurant.IsDelete = false;
                            restaurant.Approve = (int)ApproveStatus.Approved;


                            UpdateDatabase<Restaurant>(restaurant);
                            SaveChange();

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


        public Response UpdateRestaurant(UpdateRestaurantViewModel input, string emailUser)
        {
            try
            {
                var userLogin = GetCurrentUser(input.IdUserModify);

                var restaurant = (from x in _db.Restaurants
                                  where x.IdRestaurant == input.IdRestaurant
                                  select x).FirstOrDefault();

                // clone new object
                var restaurantOld = new Restaurant();
                restaurantOld = Ultility.DeepCopy<Restaurant>(restaurant);
                restaurantOld.IdAction = restaurantOld.IdRestaurant.ToString(); // gán ID cũ cho IdAction để có thể truy cập qua lại
                restaurantOld.IdRestaurant = Guid.NewGuid();
                restaurantOld.IsTempdata = true;
                _db.Restaurants.Add(restaurantOld);

                #region setdata
                restaurant.IdAction = restaurantOld.IdRestaurant.ToString();
                restaurant.IdUserModify = input.IdUserModify;
                restaurant.TypeAction = input.TypeAction;
                restaurant.Approve = (int)ApproveStatus.Waiting;
                restaurant.ModifyBy = userLogin.NameEmployee;
                restaurant.ModifyDate = Ultility.ConvertDatetimeToUnixTimeStampMiliSecond(DateTime.Now);

                restaurant.Address = input.Address;
                restaurant.NameRestaurant = input.Name;
                restaurant.Phone = input.Phone;
                restaurant.ComboPrice = input.ComboPrice;
                restaurant.ProvinceId = input.ProvinceId;
                restaurant.DistrictId = input.DistrictId;
                restaurant.WardId = input.WardId;
                #endregion

                string jsonContent = JsonSerializer.Serialize(restaurant);

                UpdateDatabase<Restaurant>(restaurant);
                SaveChange();
                var listRole = new int[] { Convert.ToInt16(Enums.TitleRole.Admin), Convert.ToInt16(Enums.TitleRole.LocalManager) };
                _notification.CreateNotification(userLogin.IdEmployee, Convert.ToInt16(Enums.TypeNotification.Restaurant), restaurant.NameRestaurant, listRole, "");
                bool result = _log.AddLog(content: jsonContent, type: "update", emailCreator: emailUser, classContent: "Restaurant");
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
        public Response CreateRestaurant(CreateRestaurantViewModel input, string emailUser)
        {
            var user = GetCurrentUser(input.IdUserModify);
            input.ModifyBy = user.NameEmployee;
            Restaurant restaurant
                         = Mapper.MapCreateRestaurant(input);
            string jsonContent = JsonSerializer.Serialize(restaurant);
            restaurant.ContractId = Guid.Empty;
            _db.Restaurants.Add(restaurant);

            _db.SaveChanges();
            var listRole = new int[] { Convert.ToInt16(Enums.TitleRole.Admin), Convert.ToInt16(Enums.TitleRole.LocalManager) };
            _notification.CreateNotification(user.IdEmployee, Convert.ToInt16(Enums.TypeNotification.Restaurant), restaurant.NameRestaurant, listRole, "");
            return Ultility.Responses("Đã gửi yêu cầu thêm !", Enums.TypeCRUD.Success.ToString());
            bool result = _log.AddLog(content: jsonContent, type: "create", emailCreator: emailUser, classContent: "Restaurant");
            if (result)
            {
                return Ultility.Responses("Đã gửi yêu cầu thêm !", Enums.TypeCRUD.Success.ToString());

            }
            else
            {
                return Ultility.Responses("Lỗi log!", Enums.TypeCRUD.Error.ToString());
            }
        }

        public Response ApproveRestaurant(Guid id)
        {
            try
            {
                var restaurant = (from x in _db.Restaurants
                                  where x.IdRestaurant == id
                                  && x.Approve == (int)ApproveStatus.Waiting
                                  select x).FirstOrDefault();
                if (restaurant != null)
                {
                    if (restaurant.TypeAction == "update")
                    {
                        var idRestaurantTemp = restaurant.IdAction;
                        restaurant.Approve = (int)ApproveStatus.Approved;
                        restaurant.IdAction = null;
                        restaurant.TypeAction = null;
                        // delete tempdata
                        var restaurantTemp = (from x in _db.Restaurants
                                              where x.IdRestaurant == Guid.Parse(idRestaurantTemp)
                                              select x).FirstOrDefault();
                        _db.Restaurants.Remove(restaurantTemp);
                    }
                    else if (restaurant.TypeAction == "insert")
                    {
                        restaurant.IdAction = null;
                        restaurant.TypeAction = null;
                        restaurant.Approve = (int)ApproveStatus.Approved;
                    }
                    else if (restaurant.TypeAction == "restore")
                    {
                        restaurant.IdAction = null;
                        restaurant.TypeAction = null;
                        restaurant.Approve = (int)ApproveStatus.Approved;
                        restaurant.IsDelete = false;
                    }
                    else // delete
                    {
                        restaurant.IdAction = null;
                        restaurant.TypeAction = null;
                        restaurant.Approve = (int)ApproveStatus.Approved;
                        restaurant.IsDelete = true;
                    }
                    UpdateDatabase<Restaurant>(restaurant);
                    SaveChange();
                    var userModify = GetCurrentUser(restaurant.IdUserModify);
                    _notification.CreateNotification(userModify.IdEmployee, Convert.ToInt16(Enums.TypeNotification.Restaurant), restaurant.NameRestaurant, new int[] { userModify.RoleId }, "Thành công");
                    return Ultility.Responses($"Duyệt thành công !", Enums.TypeCRUD.Success.ToString());
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

        public Response RefusedRestaurant(Guid id)
        {
            try
            {
                var restaurant = (from x in _db.Restaurants
                                  where x.IdRestaurant == id
                                  && x.Approve == (int)ApproveStatus.Waiting
                                  select x).FirstOrDefault();
                if (restaurant != null)
                {
                    if (restaurant.TypeAction == "update")
                    {
                        var idRestaurantTemp = restaurant.IdAction;
                        // old hotel
                        var restaurantTemp = (from x in _db.Restaurants
                                              where x.IdRestaurant == Guid.Parse(idRestaurantTemp)
                                              select x).FirstOrDefault();
                        restaurant.Approve = (int)ApproveStatus.Approved;
                        restaurant.IdAction = null;
                        restaurant.TypeAction = null;
                        #region restore old data

                        restaurant.Approve = (int)ApproveStatus.Approved;
                        restaurant.Address = restaurantTemp.Address;

                        restaurant.Phone = restaurantTemp.Phone;
                        restaurant.NameRestaurant = restaurantTemp.NameRestaurant;
                        restaurant.ComboPrice = restaurantTemp.ComboPrice;
                        #endregion

                        _db.Restaurants.Remove(restaurantTemp);
                    }
                    else if (restaurant.TypeAction == "insert")
                    {
                        restaurant.IdAction = null;
                        restaurant.TypeAction = null;
                        restaurant.Approve = (int)ApproveStatus.Refused;
                    }
                    else if (restaurant.TypeAction == "restore")
                    {
                        restaurant.IdAction = null;
                        restaurant.TypeAction = null;
                        restaurant.IsDelete = true;
                        restaurant.Approve = (int)ApproveStatus.Approved;
                    }
                    else // delete
                    {
                        restaurant.IdAction = null;
                        restaurant.TypeAction = null;
                        restaurant.IsDelete = false;
                        restaurant.Approve = (int)ApproveStatus.Approved;
                    }

                    UpdateDatabase<Restaurant>(restaurant);
                    SaveChange();
                    var userModify = GetCurrentUser(restaurant.IdUserModify);
                    _notification.CreateNotification(userModify.IdEmployee, Convert.ToInt16(Enums.TypeNotification.Restaurant), restaurant.NameRestaurant, new int[] { userModify.RoleId }, "Từ chối");
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


        #endregion
        #region Place
        public Response CreatePlace(CreatePlaceViewModel input, string emailUser)

        {
            try
            {
                var userLogin = GetCurrentUser(input.IdUserModify);
                input.ModifyBy = userLogin.NameEmployee;
                Place place = Mapper.MapCreatePlace(input);
                string jsonContent = JsonSerializer.Serialize(place);
                CreateDatabase<Place>(place);
                SaveChange();
                var listRole = new int[] { Convert.ToInt16(Enums.TitleRole.Admin), Convert.ToInt16(Enums.TitleRole.LocalManager) };
                _notification.CreateNotification(userLogin.IdEmployee, Convert.ToInt16(Enums.TypeNotification.Place), place.NamePlace, listRole, "");
                bool result = _log.AddLog(content: jsonContent, type: "create", emailCreator: emailUser, classContent: "Place");
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
        public Response GetsPlace(bool isDelete)
        {
            try
            {
                var list = (from x in _db.Places
                            where
                            x.IsDelete == isDelete &&
                            x.IsTempdata == false &&
                            x.Approve == Convert.ToInt16(ApproveStatus.Approved)
                            select x).ToList();
                var result = Mapper.MapPlace(list);
                return Ultility.Responses("", Enums.TypeCRUD.Success.ToString(), result);
            }
            catch (Exception e)
            {
                return Ultility.Responses("Có lỗi xảy ra !", Enums.TypeCRUD.Error.ToString(), description: e.Message);
            }
        }
        public Response GetsWaitingPlace(Guid idUser, int pageIndex, int pageSize)
        {
            try
            {
                var totalResult = 0;
                var userLogin = (from x in _db.Employees.AsNoTracking()
                                 where x.IdEmployee == idUser
                                 select x).FirstOrDefault();
                var listWaiting = new List<Place>();
                if (userLogin.RoleId == (int)Enums.TitleRole.Admin)
                {
                    var querylistWaiting = (from x in _db.Places.AsNoTracking()
                                            where x.Approve == Convert.ToInt16(ApproveStatus.Waiting)
                                            orderby x.ModifyDate descending
                                            select x);
                    totalResult = querylistWaiting.Count();
                    listWaiting = querylistWaiting.Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();
                }
                else
                {
                    var querylistWaiting = (from x in _db.Places.AsNoTracking()
                                            where x.IdUserModify == idUser
                                            && x.Approve == Convert.ToInt16(ApproveStatus.Waiting)
                                            orderby x.ModifyDate descending
                                            select x);
                    totalResult = querylistWaiting.Count();
                    listWaiting = querylistWaiting.Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();
                }
                var result = Mapper.MapPlace(listWaiting);
                var res = Ultility.Responses("", Enums.TypeCRUD.Success.ToString(), result);
                res.TotalResult = totalResult;
                return res;
            }
            catch (Exception e)
            {
                return Ultility.Responses("Có lỗi xảy ra !", Enums.TypeCRUD.Error.ToString(), description: e.Message);
            }
        }
        public Response DeletePlace(Guid id, Guid idUser, string emailUser)
        {
            try
            {
                var place = (from x in _db.Places.AsNoTracking()
                             where x.IdPlace == id
                             select x).FirstOrDefault();

                var userLogin = (from x in _db.Employees
                                 where x.IdEmployee == idUser
                                 select x).FirstOrDefault();
                if (place.Approve == (int)ApproveStatus.Approved)
                {
                    place.ModifyBy = userLogin.NameEmployee;
                    place.TypeAction = "delete";
                    place.IdUserModify = userLogin.IdEmployee;
                    place.ModifyDate = Ultility.ConvertDatetimeToUnixTimeStampMiliSecond(DateTime.Now);
                    place.Approve = (int)ApproveStatus.Waiting;
                    place.IsDelete = true;
                    string jsonContent = JsonSerializer.Serialize(place);
                    UpdateDatabase<Place>(place);
                    SaveChange();
                    var listRole = new int[] { Convert.ToInt16(Enums.TitleRole.Admin), Convert.ToInt16(Enums.TitleRole.LocalManager) };
                    _notification.CreateNotification(userLogin.IdEmployee, Convert.ToInt16(Enums.TypeNotification.Place), place.NamePlace, listRole, "");
                    bool result = _log.AddLog(content: jsonContent, type: "delete", emailCreator: emailUser, classContent: "Place");
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
                    if (place.IdUserModify == idUser)
                    {
                        if (place.TypeAction == "insert")
                        {
                            _db.Places.Remove(place);

                            UpdateDatabase<Place>(place);
                            SaveChange();

                            return Ultility.Responses("Đã xóa!", Enums.TypeCRUD.Success.ToString());
                        }
                        else if (place.TypeAction == "update")
                        {
                            var idPlaceTemp = place.IdAction;
                            // old hotel
                            var placeTemp = (from x in _db.Places.AsNoTracking()
                                             where x.IdPlace == Guid.Parse(idPlaceTemp)
                                             select x).FirstOrDefault();
                            place.Approve = (int)ApproveStatus.Approved;
                            place.IdAction = null;
                            place.TypeAction = null;
                            #region restore old data

                            place.Approve = (int)ApproveStatus.Approved;
                            place.Address = placeTemp.Address;

                            place.Phone = placeTemp.Phone;
                            place.NamePlace = placeTemp.NamePlace;
                            place.PriceTicket = placeTemp.PriceTicket;
                            #endregion

                            DeleteDatabase<Place>(placeTemp);

                            UpdateDatabase<Place>(placeTemp);
                            SaveChange();
                            return Ultility.Responses("Đã hủy yêu cầu chỉnh sửa !", Enums.TypeCRUD.Success.ToString());
                        }
                        else if (place.TypeAction == "restore")
                        {
                            place.IdAction = null;
                            place.TypeAction = null;
                            place.IsDelete = true;
                            place.Approve = (int)ApproveStatus.Approved;

                            UpdateDatabase<Place>(place);
                            SaveChange();

                            return Ultility.Responses("Đã hủy yêu cầu khôi phục!", Enums.TypeCRUD.Success.ToString());

                        }
                        else // delete
                        {
                            place.IdAction = null;
                            place.TypeAction = null;
                            place.IsDelete = false;
                            place.Approve = (int)ApproveStatus.Approved;

                            UpdateDatabase<Place>(place);
                            SaveChange();

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
        public Response UpdatePlace(UpdatePlaceViewModel input, string emailUser)
        {
            try
            {
                var userLogin = GetCurrentUser(input.IdUserModify);

                var place = (from x in _db.Places.AsNoTracking()
                             where x.IdPlace == input.IdPlace
                             select x).FirstOrDefault();

                // clone new object
                var placeOld = new Place();
                placeOld = Ultility.DeepCopy<Place>(place);
                placeOld.IdAction = placeOld.IdPlace.ToString();
                placeOld.IdPlace = Guid.NewGuid();
                placeOld.IsTempdata = true;
                CreateDatabase<Place>(placeOld);
                string jsonContent = JsonSerializer.Serialize(place);
                #region setdata
                place.IdAction = placeOld.IdPlace.ToString();
                place.IdUserModify = input.IdUserModify;
                place.TypeAction = input.TypeAction;
                place.Approve = (int)ApproveStatus.Waiting;
                place.ModifyBy = userLogin.NameEmployee;
                place.ModifyDate = Ultility.ConvertDatetimeToUnixTimeStampMiliSecond(DateTime.Now);


                place.Address = input.Address;
                place.NamePlace = input.Name;
                place.Phone = input.Phone;
                place.PriceTicket = input.PriceTicket;
                place.ProvinceId = input.ProvinceId;
                place.DistrictId = input.DistrictId;

                place.WardId = input.WardId;
                #endregion
                UpdateDatabase<Place>(place);
                SaveChange();
                var listRole = new int[] { Convert.ToInt16(Enums.TitleRole.Admin), Convert.ToInt16(Enums.TitleRole.LocalManager) };
                _notification.CreateNotification(userLogin.IdEmployee, Convert.ToInt16(Enums.TypeNotification.Place), place.NamePlace, listRole, "");
                bool result = _log.AddLog(content: jsonContent, type: "update", emailCreator: emailUser, classContent: "Place");
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
        public Response ApprovePlace(Guid id)
        {
            try
            {
                var place = (from x in _db.Places.AsNoTracking()
                             where x.IdPlace == id
                             && x.Approve == (int)ApproveStatus.Waiting
                             select x).FirstOrDefault();
                if (place != null)
                {


                    if (place.TypeAction == "update")
                    {
                        var idPlaceTemp = place.IdAction;
                        place.Approve = (int)ApproveStatus.Approved;
                        place.IdAction = null;
                        place.TypeAction = null;


                        // delete tempdata
                        var placeTemp = (from x in _db.Places
                                         where x.IdPlace == Guid.Parse(idPlaceTemp)
                                         select x).FirstOrDefault();
                        DeleteDatabase<Place>(place);
                    }
                    else if (place.TypeAction == "insert")
                    {
                        place.IdAction = null;
                        place.TypeAction = null;
                        place.Approve = (int)ApproveStatus.Approved;
                    }
                    else if (place.TypeAction == "restore")
                    {
                        place.IdAction = null;
                        place.TypeAction = null;
                        place.Approve = (int)ApproveStatus.Approved;
                        place.IsDelete = false;

                    }
                    else
                    {
                        place.IdAction = null;
                        place.TypeAction = null;
                        place.Approve = (int)ApproveStatus.Approved;
                        place.IsDelete = true;
                    }



                    UpdateDatabase<Place>(place);
                    SaveChange();
                    var userModify = GetCurrentUser(place.IdUserModify);
                    _notification.CreateNotification(userModify.IdEmployee, Convert.ToInt16(Enums.TypeNotification.Place), place.NamePlace, new int[] { userModify.RoleId }, "Thành công");
                    return Ultility.Responses($"Duyệt thành công !", Enums.TypeCRUD.Success.ToString());
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
        public Response RefusedPlace(Guid id)
        {
            try
            {
                var place = (from x in _db.Places.AsNoTracking()

                             where x.IdPlace == id
                             && x.Approve == (int)ApproveStatus.Waiting
                             select x).FirstOrDefault();
                if (place != null)
                {
                    if (place.TypeAction == "update")
                    {
                        var idPlaceTemp = place.IdAction;
                        // old hotel
                        var placeTemp = (from x in _db.Places.AsNoTracking()
                                         where x.IdPlace == Guid.Parse(idPlaceTemp)
                                         select x).FirstOrDefault();
                        place.Approve = (int)ApproveStatus.Approved;
                        place.IdAction = null;
                        place.TypeAction = null;
                        #region restore old data

                        place.Approve = (int)ApproveStatus.Approved;
                        place.Address = placeTemp.Address;

                        place.Phone = placeTemp.Phone;
                        place.NamePlace = placeTemp.NamePlace;
                        place.PriceTicket = placeTemp.PriceTicket;
                        #endregion
                        DeleteDatabase<Place>(placeTemp);

                    }
                    else if (place.TypeAction == "insert")
                    {
                        place.IdAction = null;
                        place.TypeAction = null;
                        place.Approve = (int)ApproveStatus.Refused;
                    }
                    else if (place.TypeAction == "restore")
                    {
                        place.IdAction = null;
                        place.TypeAction = null;
                        place.IsDelete = true;
                        place.Approve = (int)ApproveStatus.Approved;
                    }
                    else // delete
                    {
                        place.IdAction = null;
                        place.TypeAction = null;
                        place.IsDelete = false;
                        place.Approve = (int)ApproveStatus.Approved;
                    }
                    UpdateDatabase<Place>(place);
                    SaveChange();
                    var userModify = GetCurrentUser(place.IdUserModify);
                    _notification.CreateNotification(userModify.IdEmployee, Convert.ToInt16(Enums.TypeNotification.Place), place.NamePlace, new int[] { userModify.RoleId }, "Từ chối");
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
        #endregion
        public Response CreateContract(CreateContractViewModel input, string emailUser)
        {
            try
            {
                Contract contract = Mapper.MapCreateContract(input);
                string jsonContent = JsonSerializer.Serialize(contract);
                CreateDatabase<Contract>(contract);

                UpdateDatabase<Contract>(contract);
                SaveChange();
                bool result = _log.AddLog(content: jsonContent, type: "create", emailCreator: emailUser, classContent: "Contract");
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
        public Response RestorePlace(Guid id, Guid idUser, string emailUser)
        {
            try
            {
                var place = (from x in _db.Places.AsNoTracking()
                             where x.IdPlace == id
                             select x).FirstOrDefault();
                string jsonContent = JsonSerializer.Serialize(place);
                var userLogin = (from x in _db.Employees
                                 where x.IdEmployee == idUser
                                 select x).FirstOrDefault();
                if (place.Approve == (int)ApproveStatus.Approved)
                {
                    place.ModifyBy = userLogin.NameEmployee;
                    place.TypeAction = "restore";
                    place.IdUserModify = userLogin.IdEmployee;
                    place.ModifyDate = Ultility.ConvertDatetimeToUnixTimeStampMiliSecond(DateTime.Now);
                    place.Approve = (int)ApproveStatus.Waiting;
                    // bổ sung isdelete
                    place.IsDelete = false;
                }
                UpdateDatabase<Place>(place);
                SaveChange();
                var listRole = new int[] { Convert.ToInt16(Enums.TitleRole.Admin), Convert.ToInt16(Enums.TitleRole.LocalManager) };
                _notification.CreateNotification(userLogin.IdEmployee, Convert.ToInt16(Enums.TypeNotification.Place), place.NamePlace, listRole, "");
                bool result = _log.AddLog(content: jsonContent, type: "restore", emailCreator: emailUser, classContent: "Place");
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

        public Response RestoreRestaurant(Guid id, Guid idUser, string emailUser)
        {
            try
            {
                var restaurant = (from x in _db.Restaurants.AsNoTracking()
                                  where x.IdRestaurant == id
                                  select x).FirstOrDefault();
                string jsonContent = JsonSerializer.Serialize(restaurant);
                var userLogin = (from x in _db.Employees
                                 where x.IdEmployee == idUser
                                 select x).FirstOrDefault();
                if (restaurant.Approve == (int)ApproveStatus.Approved)
                {
                    restaurant.ModifyBy = userLogin.NameEmployee;
                    restaurant.TypeAction = "restore";
                    restaurant.IdUserModify = userLogin.IdEmployee;
                    restaurant.ModifyDate = Ultility.ConvertDatetimeToUnixTimeStampMiliSecond(DateTime.Now);
                    restaurant.Approve = (int)ApproveStatus.Waiting;
                    restaurant.IsDelete = false;
                }
                UpdateDatabase<Restaurant>(restaurant);
                SaveChange();
                var listRole = new int[] { Convert.ToInt16(Enums.TitleRole.Admin), Convert.ToInt16(Enums.TitleRole.LocalManager) };
                _notification.CreateNotification(userLogin.IdEmployee, Convert.ToInt16(Enums.TypeNotification.Restaurant), restaurant.NameRestaurant, listRole, "");
                bool result = _log.AddLog(content: jsonContent, type: "restore", emailCreator: emailUser, classContent: "Restaurant");
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

        public Response SearchHotel(JObject frmData)
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
                var kwName = PrCommon.GetString("name", frmData).Trim();
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
                var KwAddress = PrCommon.GetString("address", frmData).Trim();
                if (!String.IsNullOrEmpty(KwAddress))
                {
                    keywords.KwAddress = KwAddress.Trim().ToLower();
                }
                else
                {
                    keywords.KwAddress = "";

                }
                var kwStar = PrCommon.GetString("star", frmData);
                keywords.KwStar = PrCommon.getListInt(kwStar, ',', false);
                var listHotel = new List<Hotel>();

                if (keywords.KwStar.Count > 0)
                {
                    var querylistHotel = (from x in _db.Hotels
                                          where x.IsDelete == keywords.IsDelete &&
                                                          x.NameHotel.ToLower().Contains(keywords.KwName) &&
                                                          x.Address.ToLower().Contains(keywords.KwAddress) &&
                                                          x.Phone.ToLower().Contains(keywords.KwPhone) &&
                                                          x.IsTempdata == false &&
                                                          x.Approve == Convert.ToInt16(Enums.ApproveStatus.Approved) &&
                                                          keywords.KwStar.Contains(x.Star)
                                          orderby x.ModifyDate descending
                                          select x);
                    totalResult = querylistHotel.Count();
                    listHotel = querylistHotel.Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();
                }
                else
                {
                    var querylistHotel = (from x in _db.Hotels
                                          where x.IsDelete == keywords.IsDelete &&
                                                          x.NameHotel.ToLower().Contains(keywords.KwName) &&
                                                          x.Address.ToLower().Contains(keywords.KwAddress) &&
                                                          x.Phone.ToLower().Contains(keywords.KwPhone) &&
                                                          x.IsTempdata == false &&
                                                          x.Approve == Convert.ToInt16(Enums.ApproveStatus.Approved)

                                          orderby x.ModifyDate descending
                                          select x);
                    totalResult = querylistHotel.Count();
                    listHotel = querylistHotel.Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();
                }
                var result = Mapper.MapHotel(listHotel);
                if (result.Count > 0)
                {
                    var res = Ultility.Responses("", Enums.TypeCRUD.Success.ToString(), result);
                    res.TotalResult = totalResult;
                    return res;
                }
                else
                {
                    return Ultility.Responses("Không có dữ liệu trả về !", Enums.TypeCRUD.Warning.ToString(), result);
                }
            }
            catch (Exception e)
            {
                return Ultility.Responses("Có lỗi xảy ra !", Enums.TypeCRUD.Error.ToString(), description: e.Message);
            }
        }
        public Response SearchPlace(JObject frmData)
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
                var kwName = PrCommon.GetString("name", frmData).Trim();
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
                var KwAddress = PrCommon.GetString("address", frmData).Trim();
                if (!String.IsNullOrEmpty(KwAddress))
                {
                    keywords.KwAddress = KwAddress.Trim().ToLower();
                }
                else
                {
                    keywords.KwAddress = "";

                }
                var KwPriceTicket = PrCommon.GetString("priceTicket", frmData).Trim();
                if (!String.IsNullOrEmpty(KwPriceTicket))
                {
                    keywords.KwPriceTicket = KwPriceTicket.Trim().ToLower();
                }
                else
                {
                    keywords.KwPriceTicket = "";

                }
                var listPlace = new List<Place>();

                if (!string.IsNullOrEmpty(isDelete))
                {
                    var querylistPlace = (from x in _db.Places
                                          where x.IsDelete == keywords.IsDelete &&
                                                          x.NamePlace.ToLower().Contains(keywords.KwName) &&
                                                          x.Address.ToLower().Contains(keywords.KwAddress) &&
                                                          x.Phone.ToLower().Contains(keywords.KwPhone) &&
                                                          x.IsTempdata == false &&
                                                          x.Approve == Convert.ToInt16(Enums.ApproveStatus.Approved)

                                          select x);
                    totalResult = querylistPlace.Count();
                    listPlace = querylistPlace.Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();
                }
                else
                {
                    var querylistPlace = (from x in _db.Places
                                          where x.IsDelete == keywords.IsDelete &&
                                                          x.NamePlace.ToLower().Contains(keywords.KwName) &&
                                                          x.Address.ToLower().Contains(keywords.KwAddress) &&
                                                          x.Phone.ToLower().Contains(keywords.KwPhone) &&
                                                          x.IsTempdata == false &&
                                                          x.Approve == Convert.ToInt16(Enums.ApproveStatus.Approved)
                                          orderby x.PriceTicket
                                          select x);
                    totalResult = querylistPlace.Count();
                    listPlace = querylistPlace.Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();
                }
                var result = Mapper.MapPlace(listPlace);
                if (result.Count > 0)
                {
                    var res = Ultility.Responses("", Enums.TypeCRUD.Success.ToString(), result);
                    res.TotalResult = totalResult;
                    return res;
                }
                else
                {
                    return Ultility.Responses("Không có dữ liệu trả về !", Enums.TypeCRUD.Warning.ToString(), result);
                }
            }
            catch (Exception e)
            {
                return Ultility.Responses("Có lỗi xảy ra !", Enums.TypeCRUD.Error.ToString(), description: e.Message);
            }
        }

        public Response SearchRestaurant(JObject frmData)
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
                var kwName = PrCommon.GetString("name", frmData).Trim();
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
                var KwAddress = PrCommon.GetString("address", frmData).Trim();
                if (!String.IsNullOrEmpty(KwAddress))
                {
                    keywords.KwAddress = KwAddress.Trim().ToLower();
                }
                else
                {
                    keywords.KwAddress = "";

                }
                var kwComboPrice = PrCommon.GetString("comboPrice", frmData).Trim();
                if (!String.IsNullOrEmpty(kwComboPrice))
                {
                    keywords.KwComboPrice = kwComboPrice.Trim().ToLower();
                }
                else
                {
                    keywords.KwComboPrice = "";

                }
                var listRestaurant = new List<Restaurant>();


                if (!string.IsNullOrEmpty(isDelete))
                {
                    var querylistRestaurant = (from x in _db.Restaurants
                                               where x.IsDelete == keywords.IsDelete &&
                                                               x.NameRestaurant.ToLower().Contains(keywords.KwName) &&
                                                               x.Address.ToLower().Contains(keywords.KwAddress) &&
                                                               x.Phone.ToLower().Contains(keywords.KwPhone) &&
                                                               x.IsTempdata == false &&
                                                               x.Approve == Convert.ToInt16(Enums.ApproveStatus.Approved)
                                               select x);
                    totalResult = querylistRestaurant.Count();
                    listRestaurant = querylistRestaurant.Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();
                }
                else
                {
                    var querylistRestaurant = (from x in _db.Restaurants
                                               where x.IsDelete == keywords.IsDelete &&
                                                               x.NameRestaurant.ToLower().Contains(keywords.KwName) &&
                                                               x.Address.ToLower().Contains(keywords.KwAddress) &&
                                                               x.Phone.ToLower().Contains(keywords.KwPhone) &&
                                                               x.IsTempdata == false &&
                                                               x.Approve == Convert.ToInt16(Enums.ApproveStatus.Approved)

                                               select x);
                    totalResult = querylistRestaurant.Count();
                    listRestaurant = querylistRestaurant.Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();
                }
                var result = Mapper.MapRestaurant(listRestaurant);
                if (result.Count > 0)
                {
                    var res = Ultility.Responses("", Enums.TypeCRUD.Success.ToString(), result);
                    res.TotalResult = totalResult;
                    return res;
                }
                else
                {
                    return Ultility.Responses("Không có dữ liệu trả về !", Enums.TypeCRUD.Warning.ToString(), result);
                }
            }
            catch (Exception e)
            {
                return Ultility.Responses("Có lỗi xảy ra !", Enums.TypeCRUD.Error.ToString(), description: e.Message);
            }
        }


        public Response GetListHotelByProvince(string toPlace)
        {
            try
            {
 
                var listHotel = new List<Hotel>();
                var idProvince = (from x in _db.Provinces
                                     where x.NameProvince == toPlace
                                     select x.IdProvince).FirstOrDefault();

                var querylistHotel = (from x in _db.Hotels
                                      where x.IsDelete == false &&
                                      x.ProvinceId == idProvince
                                      orderby x.ModifyDate descending
                                      select x);
                var result = Mapper.MapHotel(querylistHotel.ToList());
                if (result.Count > 0)
                {
                    var res = Ultility.Responses("", Enums.TypeCRUD.Success.ToString(), result);
                    return res;
                }
                else
                {
                    return Ultility.Responses("Không có dữ liệu trả về !", Enums.TypeCRUD.Warning.ToString(), result);
                }
            }
            catch (Exception e)
            {
                return Ultility.Responses("Có lỗi xảy ra !", Enums.TypeCRUD.Error.ToString(), description: e.Message);
            }
        }

        public Response GetListPlaceByProvince(string toPlace)
        {
            try
            {

                var listHotel = new List<Hotel>();
                var idProvince = (from x in _db.Provinces
                                  where x.NameProvince == toPlace
                                  select x.IdProvince).FirstOrDefault();

                var querylistPlace = (from x in _db.Places
                                      where x.IsDelete == false &&
                                      x.ProvinceId == idProvince
                                      orderby x.ModifyDate descending
                                      select x);
                var result = Mapper.MapPlace(querylistPlace.ToList());
                if (result.Count > 0)
                {
                    var res = Ultility.Responses("", Enums.TypeCRUD.Success.ToString(), result);
                    return res;
                }
                else
                {
                    return Ultility.Responses("Không có dữ liệu trả về !", Enums.TypeCRUD.Warning.ToString(), result);
                }
            }
            catch (Exception e)
            {
                return Ultility.Responses("Có lỗi xảy ra !", Enums.TypeCRUD.Error.ToString(), description: e.Message);
            }
        }

        public Response GetListRestaurantByProvince(string toPlace)
        {
            try
            {

                var listHotel = new List<Hotel>();
                var idProvince = (from x in _db.Provinces
                                  where x.NameProvince == toPlace
                                  select x.IdProvince).FirstOrDefault();

                var querylistRestaurant = (from x in _db.Restaurants
                                      where x.IsDelete == false &&
                                      x.ProvinceId == idProvince
                                      orderby x.ModifyDate descending
                                      select x);
                var result = Mapper.MapRestaurant(querylistRestaurant.ToList());
                if (result.Count > 0)
                {
                    var res = Ultility.Responses("", Enums.TypeCRUD.Success.ToString(), result);
                    return res;
                }
                else
                {
                    return Ultility.Responses("Không có dữ liệu trả về !", Enums.TypeCRUD.Warning.ToString(), result);
                }
            }
            catch (Exception e)
            {
                return Ultility.Responses("Có lỗi xảy ra !", Enums.TypeCRUD.Error.ToString(), description: e.Message);
            }
        }
    }
    
}
