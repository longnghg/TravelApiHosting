using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using PrUtility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        public NewsRes(TravelContext db)
        {
            _db = db;
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
                var result = (from x in _db.Banners.AsNoTracking() where x.IsDelete == false select x).ToList();
                if (result.Count > 0)
                {
                    res.Content = result;
                }

                return res;
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

                if (img != null)
                {
                    foreach (var item in img)
                    {

                    }
                }

                if (result != null)
                {
                    banner.IsDelete = true;
                    banner.IsActive = false;
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
            catch (Exception)
            {
                return Ultility.Responses("Lỗi !", Enums.TypeCRUD.Success.ToString());
            }
        }
    }
}
