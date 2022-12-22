using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using PrUtility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Travel.Context.Models;
using Travel.Context.Models.Travel;
using Travel.Data.Interfaces;
using Travel.Shared.Ultilities;
using Travel.Shared.ViewModels;
using Travel.Shared.ViewModels.Travel;
using Travel.Shared.ViewModels.Travel.DistrictVM;
using Travel.Shared.ViewModels.Travel.WardVM;

namespace Travel.Data.Repositories
{
    public class LocationRes : ILocation
    {
        private readonly TravelContext _db;
        private Notification message; private readonly ILog _log;
        public LocationRes(TravelContext db , ILog log)
        {
            _db = db;
            _log = log;
            message = new Notification();
        }


        public string CheckBeforeSaveProvince(JObject frmData, ref Notification _message, bool isUpdate)
        {
            try
            {
                var idProvince = PrCommon.GetString("idProvince", frmData);
                if (String.IsNullOrEmpty(idProvince))
                {
                }
                var nameProvince = PrCommon.GetString("nameProvince", frmData);
                #region validate
                if (isUpdate)
                {
                    if (!String.IsNullOrEmpty(nameProvince))
                    {
                        bool check = CheckSameNameProvince(nameProvince,idProvince);
                        if (check)
                        {
                            _message = Ultility.Responses($"{nameProvince} đã tồn tại !", Enums.TypeCRUD.Validation.ToString()).Notification;
                            return string.Empty;
                        }
                    }
                }
                else
                {
                    if (!String.IsNullOrEmpty(nameProvince))
                    {
                        bool check = CheckSameNameProvince(nameProvince);
                        if (check)
                        {
                            _message = Ultility.Responses($"{nameProvince} đã tồn tại !", Enums.TypeCRUD.Validation.ToString()).Notification;
                            return string.Empty;
                        }
                    }
                }
                

                #endregion


                if (isUpdate)
                {
                    UpdateProvinceViewModel objUpdate = new UpdateProvinceViewModel();
                    objUpdate.IdProvince = Guid.Parse(idProvince);
                    objUpdate.NameProvince = nameProvince;
                    return JsonSerializer.Serialize(objUpdate);
                }

                CreateProvinceViewModel objCreate = new CreateProvinceViewModel();
                objCreate.NameProvince = nameProvince;
                return JsonSerializer.Serialize(objCreate);
            }
            catch (Exception e)
            {
                _message = Ultility.Responses("Có lỗi xảy ra !", Enums.TypeCRUD.Error.ToString(), description: e.Message).Notification;
                return string.Empty;
            }
        }

        public string CheckBeforeSaveDistrict(JObject frmData, ref Notification _message, bool isUpdate)
        {
            try
            {
                var idDistrict = PrCommon.GetString("idDistrict", frmData);
                if (String.IsNullOrEmpty(idDistrict))
                {
                }

                var nameDistrict = PrCommon.GetString("nameDistrict", frmData);

                #region validate
                if (isUpdate)
                {
                    if (!String.IsNullOrEmpty(nameDistrict))
                    {
                        bool check = CheckSameNameDistrict(nameDistrict, idDistrict);
                        if (check)
                        {
                            _message = Ultility.Responses($"{nameDistrict} đã tồn tại !", Enums.TypeCRUD.Validation.ToString()).Notification;
                            return string.Empty;
                        }
                    }
                }
                else
                {

                    if (!String.IsNullOrEmpty(nameDistrict))
                    {
                        bool check = CheckSameNameDistrict(nameDistrict);
                        if (check)
                        {
                            _message = Ultility.Responses($"{nameDistrict} đã tồn tại !", Enums.TypeCRUD.Validation.ToString()).Notification;
                            return string.Empty;
                        }
                    }
                }


                #endregion



                var provinceId = PrCommon.GetString("provinceId", frmData);
                if (String.IsNullOrEmpty(provinceId))
                {
                }
                if (isUpdate)
                {
                    UpdateDistrictViewModel objUpdate = new UpdateDistrictViewModel();
                    objUpdate.IdDistrict = Guid.Parse(idDistrict);
                    objUpdate.NameDistrict = nameDistrict;
                    objUpdate.IdProvince = Guid.Parse(provinceId);
                    return JsonSerializer.Serialize(objUpdate);
                }

                CreateDistrictViewModel objCreate = new CreateDistrictViewModel();
                objCreate.NameDistrict = nameDistrict;
                objCreate.IdProvince = Guid.Parse(provinceId);
                return JsonSerializer.Serialize(objCreate);
            }
            catch (Exception e)
            {
                _message = Ultility.Responses("Có lỗi xảy ra !", Enums.TypeCRUD.Error.ToString(), description: e.Message).Notification;
                return string.Empty;
            }
        }

        public string CheckBeforeSaveWard(JObject frmData, ref Notification _message, bool isUpdate)
        {
            try
            {
                var idWard = PrCommon.GetString("idWard", frmData);
                if (String.IsNullOrEmpty(idWard))
                {
                }

                var nameWard = PrCommon.GetString("nameWard", frmData);
                #region validate
                if (isUpdate)
                {
                    if (!String.IsNullOrEmpty(nameWard))
                    {
                        bool check = CheckSameNameWard(nameWard, idWard);
                        if (check)
                        {
                            _message = Ultility.Responses($"{nameWard} đã tồn tại !", Enums.TypeCRUD.Validation.ToString()).Notification;
                            return string.Empty;
                        }
                    }
                }
                else
                {

                    if (!String.IsNullOrEmpty(nameWard))
                    {
                        bool check = CheckSameNameDistrict(nameWard);
                        if (check)
                        {
                            _message = Ultility.Responses($"{nameWard} đã tồn tại !", Enums.TypeCRUD.Validation.ToString()).Notification;
                            return string.Empty;
                        }
                    }
                }


                #endregion



                var districtId = PrCommon.GetString("districtId", frmData);
                if (String.IsNullOrEmpty(districtId))
                {
                }
                if (isUpdate)
                {
                    UpdateWardViewModel objUpdate = new UpdateWardViewModel();
                    objUpdate.IdWard = Guid.Parse(idWard);
                    objUpdate.NameWard = nameWard;
                    objUpdate.IdDistrict = Guid.Parse(districtId);
                    return JsonSerializer.Serialize(objUpdate);
                }

                CreateWardViewModel objCreate = new CreateWardViewModel();
                objCreate.NameWard = nameWard;
                objCreate.IdDistrict = Guid.Parse(districtId);
                return JsonSerializer.Serialize(objCreate);
            }
            catch (Exception e)
            {
                _message = Ultility.Responses("Có lỗi xảy ra !", Enums.TypeCRUD.Error.ToString(), description: e.Message).Notification;
                return string.Empty;
            }
        }

        public Response GetsProvince()
        {
            try
            {
                var queryListProvince = (from x in _db.Provinces.AsNoTracking()
                                    orderby x.NameProvince 
                                    select x);
                int totalResult = queryListProvince.Count();
                var listProvince = queryListProvince.ToList();
                var result = Mapper.MapProvince(listProvince);
                var res = Ultility.Responses("", Enums.TypeCRUD.Success.ToString(), result);
                res.TotalResult = totalResult;
                return res;
            }
            catch (Exception e)
            {
                return Ultility.Responses("Có lỗi xảy ra !", Enums.TypeCRUD.Error.ToString(), description: e.Message);

            }
        }

        public Response GetsDistrict()
        {
            try
            {
                var queryListDistrict = (from x in _db.Districts.AsNoTracking() 
                                         orderby x.Province.NameProvince, x.NameDistrict 
                                         select x);



                int totalResult = queryListDistrict.Count();
                var list = queryListDistrict.ToList();
                var result = Mapper.MapDistrict(list);
                var res = Ultility.Responses("", Enums.TypeCRUD.Success.ToString(), result);
                res.TotalResult = totalResult;
                return res;


            

            }
            catch (Exception e)
            {
                return Ultility.Responses("Có lỗi xảy ra !", Enums.TypeCRUD.Error.ToString(), description: e.Message);

            }
        }

        public Response GetsWard()
        {
            try
            {
                var queryListWard = (from x in _db.Wards.AsNoTracking() orderby x.District.NameDistrict, x.NameWard select x);


                int totalResult = queryListWard.Count();
                var list = queryListWard.ToList();
                var result = Mapper.MapWard(list);
                var res = Ultility.Responses("", Enums.TypeCRUD.Success.ToString(), result);
                res.TotalResult = totalResult;
                return res;

            }
            catch (Exception e)
            {
                return Ultility.Responses("Có lỗi xảy ra !", Enums.TypeCRUD.Error.ToString(), description: e.Message);

            }
        }

        public Response SearchProvince(JObject frmData)
        {
            try
            {
                var totalResult = 0;
                Keywords keywords = new Keywords();
                var pageSize = PrCommon.GetString("pageSize", frmData) == null ? 10 : Convert.ToInt16(PrCommon.GetString("pageSize", frmData));
                var pageIndex = PrCommon.GetString("pageIndex", frmData) == null ? 1 : Convert.ToInt16(PrCommon.GetString("pageIndex", frmData));

                var kwId = PrCommon.GetString("idProvince", frmData);
                if (!String.IsNullOrEmpty(kwId))
                {
                    keywords.KwId = kwId.Trim().ToLower();
                }
                else
                {
                    keywords.KwId = "";

                }

                var kwName = PrCommon.GetString("nameProvince", frmData).Trim();
                if (!String.IsNullOrEmpty(kwName))
                {
                    keywords.KwName = kwName.Trim().ToLower();
                }
                else
                {
                    keywords.KwName = "";

                }

                var listProvince = new List<Province>();
                var queryListProvince = (from x in _db.Provinces.AsNoTracking()
                                where x.IdProvince.ToString().ToLower().Contains(keywords.KwId) &&
                                      x.NameProvince.ToLower().Contains(keywords.KwName)
                                orderby x.NameProvince
                                select x);
                totalResult = queryListProvince.Count();
                listProvince = queryListProvince.Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();
                var result = Mapper.MapProvince(listProvince);
                if (result.Count() > 0)
                {
                    var res = Ultility.Responses("", Enums.TypeCRUD.Success.ToString(), result);
                    res.TotalResult = totalResult;
                    return res;
                }
                else
                {
                    var res = Ultility.Responses("Không có dữ liệu !", Enums.TypeCRUD.Success.ToString(), result);
                    res.TotalResult = totalResult;
                    return res;
                }
            }
            catch (Exception e)
            {

                return Ultility.Responses("Có lỗi xảy ra !", Enums.TypeCRUD.Error.ToString(), description: e.Message);

            }
        }

        public Response SearchDistrict(JObject frmData)
        {
            try
            {
                var totalResult = 0;
                Keywords keywords = new Keywords();
                var pageSize = PrCommon.GetString("pageSize", frmData) == null ? 10 : Convert.ToInt16(PrCommon.GetString("pageSize", frmData));
                var pageIndex = PrCommon.GetString("pageIndex", frmData) == null ? 1 : Convert.ToInt16(PrCommon.GetString("pageIndex", frmData));


                var kwId = PrCommon.GetString("idDistrict", frmData);
                if (!String.IsNullOrEmpty(kwId))
                {
                    keywords.KwId = kwId.Trim().ToLower();
                }
                else
                {
                    keywords.KwId = "";

                }

                var kwName = PrCommon.GetString("nameDistrict", frmData).Trim();
                if (!String.IsNullOrEmpty(kwName))
                {
                    keywords.KwName = kwName.Trim().ToLower();
                }
                else
                {
                    keywords.KwName = "";

                }


                var kwIdProvince = PrCommon.GetString("provinceId", frmData);

                keywords.KwIdProvince = PrCommon.getListString(kwIdProvince, ',', false);

                var listDistrict = new List<District>();
                if (keywords.KwIdProvince.Count > 0)
                {
                    var queryListDistrict = (from x in _db.Districts.AsNoTracking()
                                    where x.IdDistrict.ToString().ToLower().Contains(keywords.KwId) &&
                                           x.NameDistrict.ToLower().Contains(keywords.KwName) &&
                                            keywords.KwIdProvince.Contains(x.ProvinceId.ToString())
                                    orderby x.Province.NameProvince, x.NameDistrict
                                    select x);
                    totalResult = queryListDistrict.Count();
                    listDistrict = queryListDistrict.Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();
                }
                else
                {
                 var   queryListDistrict = (from x in _db.Districts.AsNoTracking()
                                    where x.IdDistrict.ToString().ToLower().Contains(keywords.KwId) &&
                                           x.NameDistrict.ToLower().Contains(keywords.KwName)
                                    orderby x.Province.NameProvince, x.NameDistrict
                                    select x);
                    totalResult = queryListDistrict.Count();
                    listDistrict = queryListDistrict.Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();
                }
                var result = Mapper.MapDistrict(listDistrict);
                if (result.Count() > 0)
                {
                    var res = Ultility.Responses("", Enums.TypeCRUD.Success.ToString(), result);
                    res.TotalResult = totalResult;
                    return res;
                }
                else
                {
                    var res = Ultility.Responses("Không có dữ liệu !", Enums.TypeCRUD.Warning.ToString(), result);
                    res.TotalResult = totalResult;
                    return res;
                }
            }
            catch (Exception e)
            {

                return Ultility.Responses("Có lỗi xảy ra !", Enums.TypeCRUD.Error.ToString(), description: e.Message);

            }
        }

        public Response SearchWard(JObject frmData)
        {
            try
            {
                var totalResult = 0;
                Keywords keywords = new Keywords();
                var pageSize = PrCommon.GetString("pageSize", frmData) == null ? 10 : Convert.ToInt16(PrCommon.GetString("pageSize", frmData));
                var pageIndex = PrCommon.GetString("pageIndex", frmData) == null ? 1 : Convert.ToInt16(PrCommon.GetString("pageIndex", frmData));

                var kwId = PrCommon.GetString("idWard", frmData);
                if (!String.IsNullOrEmpty(kwId))
                {
                    keywords.KwId = kwId.Trim().ToLower();
                }
                else
                {
                    keywords.KwId = "";

                }

                var kwName = PrCommon.GetString("nameWard", frmData).Trim();
                if (!String.IsNullOrEmpty(kwName))
                {
                    keywords.KwName = kwName.Trim().ToLower();
                }
                else
                {
                    keywords.KwName = "";

                }


                var kwIdDistrict = PrCommon.GetString("districtId", frmData);

                keywords.KwIdDistrict = PrCommon.getListString(kwIdDistrict, ',', false);

                var listWard = new List<Ward>();
                if (keywords.KwIdDistrict.Count > 0)
                {
                    var queryListWard = (from x in _db.Wards.AsNoTracking()
                                where x.IdWard.ToString().ToLower().Contains(keywords.KwId) &&
                                      x.NameWard.ToLower().Contains(keywords.KwName) &&
                                      keywords.KwIdDistrict.Contains(x.DistrictId.ToString())
                                orderby x.District.NameDistrict, x.NameWard
                                select x);
                    totalResult = queryListWard.Count();
                    listWard = queryListWard.Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();
                }
                else
                {
                    var queryListWard = (from x in _db.Wards.AsNoTracking()
                                where x.IdWard.ToString().ToLower().Contains(keywords.KwId) &&
                                      x.NameWard.ToLower().Contains(keywords.KwName)
                                orderby x.District.NameDistrict, x.NameWard
                                select x);

                    totalResult = queryListWard.Count();
                    listWard = queryListWard.Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();
                }
                var result = Mapper.MapWard(listWard);
                if (result.Count() > 0)
                {
                    var res = Ultility.Responses("", Enums.TypeCRUD.Success.ToString(), result);
                    res.TotalResult = totalResult;
                    return res;
                }
                else
                {
                    var res = Ultility.Responses("Không tìm thấy dữ liệu !", Enums.TypeCRUD.Warning.ToString(), result);
                    res.TotalResult = totalResult;
                    return res;
                }
            }
            catch (Exception e)
            {

                return Ultility.Responses("Có lỗi xảy ra !", Enums.TypeCRUD.Error.ToString(), description: e.Message);

            }
        }

        public Response CreateProvince(CreateProvinceViewModel input, string emailUser)
        {
            try
            {
                var province = Mapper.MapCreateProvince(input);
                string jsonContent = JsonSerializer.Serialize(province);
                _db.Provinces.Add(province);
                _db.SaveChanges();

                bool result = _log.AddLog(content: jsonContent, type: "create", emailCreator: emailUser, classContent: "Province");
                if (result)
                {      
                    return Ultility.Responses($"Tạo mới thành công !", Enums.TypeCRUD.Success.ToString());
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

        public Response CreateDistrict(CreateDistrictViewModel input, string emailUser)
        {
            try
            {
                var district = Mapper.MapCreateDistrict(input);
                string jsonContent = JsonSerializer.Serialize(district);
                _db.Districts.Add(district);
                _db.SaveChanges();

                bool result = _log.AddLog(content: jsonContent, type: "create", emailCreator: emailUser, classContent: "District");
                if (result)
                {
                    return Ultility.Responses($"Tạo mới thành công !", Enums.TypeCRUD.Success.ToString());
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

        public Response CreateWard(CreateWardViewModel input, string emailUser)
        {
            try
            {
                var ward = Mapper.MapCreateWard(input);
                string jsonContent = JsonSerializer.Serialize(ward);
                _db.Wards.Add(ward);
                _db.SaveChanges();

                bool result = _log.AddLog(content: jsonContent, type: "create", emailCreator: emailUser, classContent: "Ward");
                if (result)
                {
                    return Ultility.Responses($"Tạo mới thành công !", Enums.TypeCRUD.Success.ToString());
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

        public Response UpdateProvince(UpdateProvinceViewModel input, string emailUser)
        {
            try
            {
                var province = Mapper.MapUpdateProvince(input);
                string jsonContent = JsonSerializer.Serialize(province);
                _db.Provinces.Update(province);
                _db.SaveChanges();

                bool result = _log.AddLog(content: jsonContent, type: "update", emailCreator: emailUser, classContent: "Province");
                if (result)
                {
                    return Ultility.Responses($"Sửa thành công !", Enums.TypeCRUD.Success.ToString());
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

        public Response UpdateDistrict(UpdateDistrictViewModel input, string emailUser)
        {
            try
            {
                var district = Mapper.MapUpdateDistrict(input);
                string jsonContent = JsonSerializer.Serialize(district);
                _db.Districts.Update(district);
                _db.SaveChanges();

                bool result = _log.AddLog(content: jsonContent, type: "update", emailCreator: emailUser, classContent: "District");
                if (result)
                {
                    return Ultility.Responses($"Sửa thành công !", Enums.TypeCRUD.Success.ToString());
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

        public Response UpdateWard(UpdateWardViewModel input, string emailUser)
        {
            try
            {
                var ward = Mapper.MapUpdateWard(input);
                string jsonContent = JsonSerializer.Serialize(ward);
                _db.Wards.Update(ward);
                _db.SaveChanges();

                bool result = _log.AddLog(content: jsonContent, type: "update", emailCreator: emailUser, classContent: "Ward");
                if (result)
                {
                    return Ultility.Responses($"Sửa thành công !", Enums.TypeCRUD.Success.ToString());
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
        public Response DeleteProvince(Guid idProvince, string emailUser)
        {
            try
            {
                var province = _db.Provinces.Find(idProvince);
                if (province != null)
                {
                    string jsonContent = JsonSerializer.Serialize(province);
                    _db.Provinces.Remove(province);
                    _db.SaveChanges();

                    bool result = _log.AddLog(content: jsonContent, type: "delete", emailCreator: emailUser, classContent: "Province");
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
                    return Ultility.Responses("Không tìm thấy !", Enums.TypeCRUD.Warning.ToString());

                }
            }
            catch (Exception e)
            {
                return Ultility.Responses("Có lỗi xảy ra !", Enums.TypeCRUD.Error.ToString(), description: e.Message);

            }
        }

        public Response DeleteDistrict(Guid idDistrict, string emailUser)
        {
            try
            {
                var district = _db.Districts.Find(idDistrict);
                if (district != null)
                {
                    string jsonContent = JsonSerializer.Serialize(district);
                    _db.Districts.Remove(district);
                    _db.SaveChanges();
                    bool result = _log.AddLog(content: jsonContent, type: "delete", emailCreator: emailUser, classContent: "District");
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
                    return Ultility.Responses("Không tìm thấy !", Enums.TypeCRUD.Warning.ToString());

                }
            }
            catch (Exception e)
            {
                return Ultility.Responses("Có lỗi xảy ra !", Enums.TypeCRUD.Error.ToString(), description: e.Message);

            }
        }

        public Response DeleteWard(Guid idWard, string emailUser)
        {
            try
            {
                var ward = _db.Wards.Find(idWard);
                if (ward != null)
                {
                    string jsonContent = JsonSerializer.Serialize(ward);
                    _db.Wards.Remove(ward);
                    _db.SaveChanges();
                    bool result = _log.AddLog(content: jsonContent, type: "delete", emailCreator: emailUser, classContent: "Ward");
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
                    return Ultility.Responses("Không tìm thấy !", Enums.TypeCRUD.Warning.ToString());

                }
            }
            catch (Exception e)
            {
                return Ultility.Responses("Có lỗi xảy ra !", Enums.TypeCRUD.Error.ToString(), description: e.Message);

            }
        }


        private bool CheckSameNameProvince(string name, string idProvince = null)
        {
            try
            {
                name = name.Replace(" ", "").ToLower();
                if (!string.IsNullOrEmpty(idProvince)) // update
                {
                    Guid id = Guid.Parse(idProvince);
                    string oldData = (from x in _db.Provinces.AsNoTracking()
                                       where x.IdProvince == id
                                       select x).FirstOrDefault().NameProvince;


                    oldData = oldData.Replace(" ", "").ToLower();
              


                    if (name != oldData) // có thay đổi  
                    {
                        var obj = (from x in _db.Provinces where x.NameProvince.Replace(" ", "").ToLower() != oldData && 
                                   x.NameProvince.Replace(" ", "").ToLower() == name select x).Count();
                        if (obj > 0)
                        {
                            return true;
                        }
                    }
                }
                else // create
                {
                    return (from x in _db.Provinces.AsNoTracking()
                            where x.NameProvince.Replace(" ", "").ToLower() == name
                            select x).Count() > 0;
                }
                return false;

            }
            catch (Exception e)
            {

                return true;
            }



           
        }
        private bool CheckSameNameDistrict(string name, string idProvince= null)
        {
            try
            {
                name = name.Replace(" ", "").ToLower();

                if (!string.IsNullOrEmpty(idProvince)) // update
                {
                    Guid id = Guid.Parse(idProvince);
                    string oldData = (from x in _db.Districts.AsNoTracking()
                                      where x.IdDistrict == id
                                      select x).FirstOrDefault().NameDistrict;


                    oldData = oldData.Replace(" ", "").ToLower();



                    if (name != oldData) // có thay đổi  
                    {
                        var obj = (from x in _db.Districts
                                   where x.NameDistrict.Replace(" ", "").ToLower() != oldData &&
           x.NameDistrict.Replace(" ", "").ToLower() == name
                                   select x).Count();
                        if (obj > 0)
                        {
                            return true;
                        }
                    }
                }
                else // create
                {
                    return (from x in _db.Districts.AsNoTracking()
                            where x.NameDistrict.Replace(" ", "").ToLower() == name
                            select x).Count() > 0;
                }
                return false;

            }
            catch (Exception e)
            {

                return true;
            }

        }
        private bool CheckSameNameWard(string name, string idProvince = null)
        {


            try
            {
                name = name.Replace(" ", "").ToLower();

                if (!string.IsNullOrEmpty(idProvince)) // update
                {
                    Guid id = Guid.Parse(idProvince);
                    string oldData = (from x in _db.Wards.AsNoTracking()
                                      where x.IdWard == id
                                      select x).FirstOrDefault().NameWard;


                    oldData = oldData.Replace(" ", "").ToLower();



                    if (name != oldData) // có thay đổi  
                    {
                        var obj = (from x in _db.Wards
                                   where x.NameWard.Replace(" ", "").ToLower() != oldData &&
           x.NameWard.Replace(" ", "").ToLower() == name
                                   select x).Count();
                        if (obj > 0)
                        {
                            return true;
                        }
                    }
                }
                else // create
                {
                    return (from x in _db.Wards.AsNoTracking()
                            where x.NameWard.Replace(" ", "").ToLower() == name
                            select x).Count() > 0;
                }
                return false;

            }
            catch (Exception e)
            {

                return true;
            }
        }
    }
}
