using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using PrUtility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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
    public class NewsRes : INews
    {
        private readonly TravelContext _db;
        private Banner banner;
        private Notification message, _message;
        private Response res;
        private ICache _cache;
        private readonly IConfiguration _config;
        public NewsRes(TravelContext db, ICache cache, IConfiguration config)
        {
            _db = db;
            _cache = cache;
            _config = config;
            banner = new Banner();
            message = new Notification();
            res = new Response();
        }
        private void UpdateDatabase<T>(T input)
        {
            _db.Entry(input).State = EntityState.Modified;
        }
        private void SaveChange( )
        {
            _db.SaveChanges();
        }
        public Response UploadBanner(string name, IFormCollection frmdata, ICollection<IFormFile> files)
        {
            
            try
             {
                #region kiểm tra size ảnh
                foreach (var item in files)
                {
                    if (item.Length > 2030346)
                    {
                        return Ultility.Responses("File ảnh quá lớn ! Ảnh không vượt quá 2mb ! ", Enums.TypeCRUD.Error.ToString());

                    }
                }
                #endregion

                if (files.Count > 0)
                {

                    var result = (from x in _db.Banners.AsNoTracking()
                                  where x.IsActive == true
                                  select x).ToList();

                     if(result != null)
                    {
                        foreach (var banner in result)
                        {
                            banner.IsActive = false;
                            UpdateDatabase(banner);
                        }
                        SaveChange();
                    }
                  
                    var Id = Guid.NewGuid();
                    banner.NameBanner = name;
                    banner.IdBanner = Id;
                    banner.IsActive = true;
                    banner.IsDelete = false;
                    banner.Total = files.Count;
                    _db.Banners.Add(banner);
                    _db.SaveChanges();
                    int err = 0;
                    var orderby = 1;
                    foreach (var file in files)
                    {
                        var image = Ultility.WriteFile(file, "Banners", Id.ToString(), ref _message, orderby);
                        if (_message != null)
                        {
                            err++;
                            _message = null;
                        }
                        else
                        {
                            _db.Images.Add(image);
                            _db.SaveChanges();
                        }
                        orderby++;
                    }

                    if (err > 0)
                    {
                        if (err == files.Count)
                        {
                            return Ultility.Responses("Có (" + err + ") lỗi xảy ra khi lưu file !", Enums.TypeCRUD.Error.ToString());
                        }
                        else
                        {
                            return Ultility.Responses("Thêm thành công  (" + (files.Count - err) + "), Thất bại (" + err + ") !", Enums.TypeCRUD.Warning.ToString());
                        }
                    }
                    else
                    {
                        return Ultility.Responses("Thêm thành công  (" + files.Count + ") !", Enums.TypeCRUD.Success.ToString());
                    }
                }
                return Ultility.Responses(" ", Enums.TypeCRUD.Success.ToString());
            }
            catch (Exception e)
            {
                return Ultility.Responses("Có lỗi xảy ra !", Enums.TypeCRUD.Error.ToString(), description: e.Message);
            }
        }

        public Response GetBanner()
        {
            try
            {
                var queryresult = (from x in _db.Banners.AsNoTracking() 
                              where x.IsDelete == false 
                              select x);


                int totalResult = queryresult.Count();
                var list = queryresult.ToList();
                var res = Ultility.Responses("", Enums.TypeCRUD.Success.ToString(), list);
                res.TotalResult = totalResult;
                return res;

            }
            catch (Exception e)
            {
                return Ultility.Responses("Có lỗi xảy ra !", Enums.TypeCRUD.Error.ToString(), description: e.Message);
            }
        }

        public Response SearchBanner(JObject frmData)
        {
            try
            {
                var totalResult = 0;
                Keywords keywords = new Keywords();
                var pageSize = PrCommon.GetString("pageSize", frmData) == null ? 10 : Convert.ToInt16(PrCommon.GetString("pageSize", frmData));
                var pageIndex = PrCommon.GetString("pageIndex", frmData) == null ? 1 : Convert.ToInt16(PrCommon.GetString("pageIndex", frmData));

                var kwName = PrCommon.GetString("nameBanner", frmData);
                if (!String.IsNullOrEmpty(kwName))
                {
                    keywords.KwName = kwName.Trim().ToLower();
                }
                else
                {
                    keywords.KwName = "";
                }


                var listBanner = new List<Banner>();

                var queryresultBanner = (from x in _db.Banners.AsNoTracking()
                                         where x.NameBanner.ToLower().Contains(keywords.KwName)
                                         select x);
                totalResult = queryresultBanner.Count();
                listBanner = queryresultBanner.Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();


                if (listBanner.Count() > 0)
                {
                    var res = Ultility.Responses("", Enums.TypeCRUD.Success.ToString(), listBanner);
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


        public Response DeleteBanner(Guid idBanner)
        {
            try
            {
                 var result = (from x in _db.Banners
                                 where x.IdBanner == idBanner
                                 select x).SingleOrDefault();

                var img = (from x in _db.Images.AsNoTracking()
                           where x.IdService == idBanner.ToString()
                           select x).ToList();

                

                if (result != null)
                {
                    if (img.Count > 0) {
                        _db.Images.RemoveRange(img);
                        _db.SaveChanges();
                    }
                    _db.Banners.Remove(result);
                    _db.SaveChanges();
                    return Ultility.Responses("Xóa thành công !", Enums.TypeCRUD.Success.ToString());
                }
                else
                {
                    return Ultility.Responses("Không tìm thấy !", Enums.TypeCRUD.Success.ToString());
                }

            }
            catch (Exception)
            {
                return Ultility.Responses("Có lỗi xảy ra !", Enums.TypeCRUD.Success.ToString());
            }
        }

        public Response GetBannerAll()
        {
            try
            {
                var result = (from x in _db.Banners.AsNoTracking()
                              where x.IsActive == true
                              select x.IdBanner).SingleOrDefault();

                var img = (from x in _db.Images.AsNoTracking()
                           where x.IdService == result.ToString()
                           select x).ToList() ;
                if (img.Count > 0)
                {
                    res.Content = img;
                }
                return res;
            }
            catch (Exception e)
            {
                return Ultility.Responses("Lỗi !", Enums.TypeCRUD.Error.ToString(), e.Message);
            }
        }

        public async Task<Response> GetApiWeather(string location)
        {
            try
            {
                #region check cache
                if (_cache.Get<Response>($"GetApiWeather{location}") != null) // có cache
                {
                    return _cache.Get<Response>($"GetApiWeather{location}");
                }
                #endregion
                #region get longtitude attitude
                var lat = "10.8333";
                var lon = "106.6667";
                var locationMap = await GetGoogleMapLocation(location);
                if (locationMap != null)
                {
                    lat = locationMap.latitude.ToString();
                    lon = locationMap.longitude.ToString();
                }
                #endregion
                string appId = _config["AppIdWeather"].ToString();
                string urlApiWeather = _config["UrlApiWeather"].ToString();
                WeatherResponse weatherRes = new();
                using (var client = new HttpClient())
                {
                    string uri = $"{urlApiWeather}?lat={lat}&lon={lon}&units=metric&appid={appId}";
                    var request = new HttpRequestMessage
                    {
                        Method = HttpMethod.Get,
                        RequestUri = new Uri(uri),
                    };
                    using (var response = await client.SendAsync(request))
                    {
                        response.EnsureSuccessStatusCode();
                        var body = await response.Content.ReadAsStringAsync();
                        weatherRes = JsonSerializer.Deserialize<WeatherResponse>(body);
                    }

                }
                foreach (var item in weatherRes.alerts)
                {
                    item.description_vi = await Translate(item.description,"en","vi");
                    item.event_vi = await Translate(item.@event, "en", "vi");
                }
                var res = Ultility.Responses("", Enums.TypeCRUD.Success.ToString(), weatherRes);
                #region set cache
                _cache.Set<Response>(res, $"GetApiWeather{location}");
                #endregion
                return res;
            }
            catch (Exception e)
            {
                return Ultility.Responses("Lỗi !", Enums.TypeCRUD.Error.ToString(), e.Message);
            }
        }

        public async Task<string> Translate(string input,string fromLang, string toLang)
        {
            string url = String.Format
             ("https://translate.googleapis.com/translate_a/single?client=gtx&sl={0}&tl={1}&dt=t&q={2}",
              fromLang, toLang, Uri.EscapeUriString(input));
            using (HttpClient httpClient = new HttpClient())
            {
                string result = await httpClient.GetStringAsync(url);
                var jsonData = Newtonsoft.Json.JsonConvert.DeserializeObject<List<dynamic>>(result);

                var translationItems = jsonData[0];
                string translation = "";
                foreach (object item in translationItems)
                {
                    IEnumerable translationLineObject = item as IEnumerable;
                    IEnumerator translationLineString = translationLineObject.GetEnumerator();
                    translationLineString.MoveNext();
                    translation += string.Format(" {0}", Convert.ToString(translationLineString.Current));
                }
                if (translation.Length > 1) { translation = translation.Substring(1); };
                return translation;
            }


        }

        public async Task<Datum> GetGoogleMapLocation(string address)
        {
            try
            {
                string urlLocation = _config["UrlLocationMap:Url"].ToString();
                string accessKey = _config["UrlLocationMap:AccessKey"].ToString();
                using (var client = new HttpClient())
                {
                    string uri = $"{urlLocation}?access_key={accessKey}&query={address}";
                    var request = new HttpRequestMessage
                    {
                        Method = HttpMethod.Get,
                        RequestUri = new Uri(uri),
                    };
                    using (var response = await client.SendAsync(request))
                    {
                        response.EnsureSuccessStatusCode();
                        var body = await response.Content.ReadAsStringAsync();
                        var listLocation =  JsonSerializer.Deserialize<GoogleMap>(body);
                        return listLocation.data.FirstOrDefault();
                    }
                }

            }
            catch (Exception e)
            {
                return null;
            }
        }
    }
}
