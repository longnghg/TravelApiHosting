using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
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

namespace Travel.Data.Repositories
{
    public class ImageRes : IImage
    {
        private readonly TravelContext _db;
        private Notification _message;
        private Response res;
        private readonly ILog _log;


        public ImageRes(TravelContext db, ILog log)
        {
            _db = db;
            _log = log;
            _message = new Notification();
            res = new Response();
        }

        public Response GetImageByIdTour(string idTour)
        {
            try
            {
                var image = (from x in _db.Images
                                where x.IdService == idTour &&
                                      x.IsDelete == false  
                                select x).ToList();
               
                if (image != null)
                {
                    res = Ultility.Responses("", Enums.TypeCRUD.Success.ToString(), image);
                }
                return res;
            }
            catch (Exception e)
            {
                return Ultility.Responses("Có lỗi xảy ra !", Enums.TypeCRUD.Error.ToString(), description: e.Message);
            }
        }

        public Response GetImageByBanner(Guid idBanner)
        {
            try
            {
                var image = (from x in _db.Images
                             where x.IdService == idBanner.ToString() &&
                                   x.IsDelete == false
                             select x).ToList();

                if (image != null)
                {
                    res = Ultility.Responses("", Enums.TypeCRUD.Success.ToString(), image);
                }
                return res;
            }
            catch (Exception e)
            {
                return Ultility.Responses("Có lỗi xảy ra !", Enums.TypeCRUD.Error.ToString(), description: e.Message);
            }
        }

        public Response CreateImageTourDetail(ICollection<IFormFile> files, string idTour, string emailUser)
        {
            try
            {
                List<Image> imageDetail = new List<Image>();
                if (files != null)
                {
                    foreach (var item in files)
                    {
                        if (item.Length > 2030346)
                        {
                            return Ultility.Responses("File ảnh quá lớn ! Ảnh không vượt quá 2mb ! ", Enums.TypeCRUD.Error.ToString());

                        }
                    }

                    imageDetail = Ultility.WriteFiles(files, "Tour", idTour, ref _message);
                    if (imageDetail.Count > 0)
                    {
                        foreach (var image in imageDetail)
                        {
                            image.TypeAction = "insert";
                        }
                        _db.Images.AddRange(imageDetail.AsEnumerable());
                        _db.SaveChanges();
                        res = Ultility.Responses("Đã gửi yêu cầu !", Enums.TypeCRUD.Success.ToString());
                    }
                    else
                    {
                        res = Ultility.Responses("", Enums.TypeCRUD.Error.ToString());
                    }
                }
                return res;
                
            }
            catch (Exception e)
            {
                return Ultility.Responses("Có lỗi xảy ra !", Enums.TypeCRUD.Error.ToString(), description: e.Message);
            }
        }

        public Response DeleteImageTourDetail(ICollection<Image> images, string emailUser)
        {
            try
            {
                if (images.Count > 0)
                {
                    foreach (var image in images)
                    {
                        image.IsDelete = true;
                        image.TypeAction = "delete";
                    }
                    _db.Images.UpdateRange(images.AsEnumerable());
                    _db.SaveChanges();
                    res = Ultility.Responses("", Enums.TypeCRUD.Success.ToString());
                }
                else
                {
                    res = Ultility.Responses("", Enums.TypeCRUD.Error.ToString());
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
