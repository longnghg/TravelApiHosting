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
using Travel.Shared.ViewModels.Travel.ReviewVM;

namespace Travel.Data.Repositories
{
    public class ReviewRes : IReview
    {
        private readonly TravelContext _db;
        private Notification message;
        private Response res;
        public ReviewRes(TravelContext db)
        {
            _db = db;
            message = new Notification();
            res = new Response();
        }
        private void CreateDatabase<T>(T input)
        {
            _db.Entry(input).State = EntityState.Added;
        }
        private void UpdateDatabase<T>(T input)
        {
            _db.Entry(input).State = EntityState.Modified;
            _db.SaveChanges();
        }
        private void DeleteDatabase<T>(T input)
        {
            _db.Entry(input).State = EntityState.Deleted;
            _db.SaveChanges();
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
                var id= PrCommon.GetString("id", frmData);
                if (String.IsNullOrEmpty(id))
                {
                    id = Guid.NewGuid().ToString();
                }
                var rating = PrCommon.GetString("rating", frmData);
                if (String.IsNullOrEmpty(rating))
                {

                }
                var dateTime = PrCommon.GetString("dateTime", frmData);
                if (String.IsNullOrEmpty(dateTime))
                {
                }
                var comment = PrCommon.GetString("comment", frmData);
                if (String.IsNullOrEmpty(comment))
                {
                }
                if (isUpdate)
                {
                    // map data
                    UpdateReviewModel objUpdate = new UpdateReviewModel();              
                    objUpdate.Id = Guid.Parse(id);
                    objUpdate.Rating = long.Parse(rating);
                    objUpdate.DateTime = long.Parse(dateTime);

                    objUpdate.Comment = comment;

                  

                    return JsonSerializer.Serialize(objUpdate);
                }
                // map data
                CreateReviewModel obj = new CreateReviewModel();

                obj.Id = Guid.Parse(id);
                obj.Rating = long.Parse(rating);
                obj.DateTime = long.Parse(dateTime);

                obj.Comment = comment;
                return JsonSerializer.Serialize(obj);
            }
            catch (Exception e)
            {
                _message = Ultility.Responses("Có lỗi xảy ra !", Enums.TypeCRUD.Error.ToString(), description: e.Message).Notification;
                return string.Empty;
            }
        }

        public Response CreateReview(CreateReviewModel input)
        {
            try
            {
                Review review = new Review();
                 review = Mapper.MapCreateReview(input);
                _db.reviews.Add(review);
                _db.SaveChanges();
                return Ultility.Responses("Thêm thành công !", Enums.TypeCRUD.Success.ToString());
            }
            catch (Exception e)
            {
                return Ultility.Responses("Có lỗi xảy ra !", Enums.TypeCRUD.Error.ToString(), description: e.Message);
            }
        }


        public Response GetsReview()
        {
            try
            {
                var list = (from x in _db.reviews.AsNoTracking()
                            select x).ToList();
                var result = Mapper.MapReview(list);
                return Ultility.Responses("", Enums.TypeCRUD.Success.ToString(), result);
            }
            catch (Exception e)
            {
                return Ultility.Responses("Có lỗi xảy ra !", Enums.TypeCRUD.Error.ToString(), description: e.Message);
            }
        }
    }
}
