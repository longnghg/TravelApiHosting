using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Travel.Context.Models;
using Travel.Context.Models.Notification;
using Travel.Context.Models.Travel;
using Travel.Data.Interfaces.INotify;
using Travel.Shared.Ultilities;
using Travel.Shared.ViewModels;
using Travel.Shared.ViewModels.Notify.CommentVM;
namespace Travel.Data.Repositories.NotifyRes
{
    public class CommentRes : IComment
    {
        private readonly TravelContext _db;
        private readonly NotificationContext _notifyContext;
        public CommentRes(NotificationContext notifyContext, TravelContext db)
        {
            _db = db;
            _notifyContext = notifyContext;
        }
        public async Task<Response> Create(CreateCommentViewModel input)
        {
            try
            {
                var customer = await (from x in _db.Customers.AsNoTracking()
                               where x.IdCustomer == input.IdCustomer
                               select x).FirstOrDefaultAsync();

                var schedule = await (from x in _db.Schedules.AsNoTracking()
                                      where x.IdSchedule == input.IdSchedule
                                      select x).FirstOrDefaultAsync();


                if (customer != null)
                {
                    Comment cmt = new Comment();
                    cmt.IdComment = Guid.NewGuid();
                    cmt.NameCustomer = customer.NameCustomer;
                    cmt.CommentText = input.CommentText;
                    cmt.CommentTime = Ultility.ConvertDatetimeToUnixTimeStampMiliSecond(DateTime.Now);

                    cmt.IdCustomer = input.IdCustomer;
                    cmt.IdTour = schedule.TourId;
                    cmt.ReviewId = Guid.NewGuid();

                    ChangeFeedback(input.IdTourBooking);
                    ChangeRating(schedule.TourId, input.Rating, customer.IdCustomer, cmt.ReviewId);


                    await _notifyContext.AddAsync(cmt);
                    await _notifyContext.SaveChangesAsync();
                    return Ultility.Responses("Bình luận thành công !", Enums.TypeCRUD.Success.ToString());
                }
                else
                {
                    return Ultility.Responses("Cần đăng nhập để thực hiện chức năng !", Enums.TypeCRUD.Success.ToString());
                }
            }
            catch (Exception e)
            {
                return Ultility.Responses("Có lỗi xảy ra !", Enums.TypeCRUD.Error.ToString(), description: e.Message);
            }
        }

        public async Task<Response> Delete(Guid id, Guid idUser)
        {
            try
            {
                var customer = await (from x in _db.Customers.AsNoTracking()
                                where x.IdCustomer == idUser
                                select x).FirstOrDefaultAsync();

                var cmt = await (from x in _notifyContext.Comment.AsNoTracking()
                                 where x.IdComment == id
                                 select x).FirstOrDefaultAsync();

                if (customer.IdCustomer == idUser)
                {
                    _notifyContext.Remove(cmt);
                    _notifyContext.SaveChanges();
                    return Ultility.Responses("Xóa thành công !", Enums.TypeCRUD.Success.ToString());
                }
                else
                {
                    return Ultility.Responses("Bạn không thể thực hiện hành động này !", Enums.TypeCRUD.Error.ToString());
                }
            }
            catch (Exception e)
            {
                return Ultility.Responses("Có lỗi xảy ra !", Enums.TypeCRUD.Error.ToString(), description: e.Message);
            }
        }


        
        public async Task<Response> Gets(string idTour)
        {
            try
            {
                var listCmtView = await (from x in _notifyContext.Comment.AsNoTracking()
                                 where x.IdTour == idTour
                                 orderby x.CommentTime descending
                                 select new CommentViewModel
                                 {
                                     CommentText = x.CommentText,
                                     CommentTime = x.CommentTime,
                                     IdComment = x.IdComment,
                                     IdCustomer = x.IdCustomer,
                                     NameCustomer = x.NameCustomer,
                                     ReviewId = x.ReviewId,
                        
                                 }).ToListAsync();
                foreach (var item in listCmtView)
                {
                    item.Rating = (from r in _db.reviews.AsNoTracking()
                                   where r.Id == item.ReviewId
                                   select r.Rating).FirstOrDefault();
                }

           
                
                return Ultility.Responses("", Enums.TypeCRUD.Success.ToString(), listCmtView);                   
            }
            catch (Exception e) 
            {
                return Ultility.Responses("Có lỗi xảy ra !", Enums.TypeCRUD.Error.ToString(), description: e.Message);
            }
        }

        public async Task<Response> GetsId(Guid idCustomer)
        {
            try
            {
                var id = await (from x in _db.TourBookings
                                where x.CustomerId == idCustomer
                                select x).ToListAsync();
                return Ultility.Responses("", Enums.TypeCRUD.Success.ToString(), id);
            }
            catch (Exception e)
            {
                return Ultility.Responses("Có lỗi xảy ra !", Enums.TypeCRUD.Error.ToString(), description: e.Message);
            }
        }

        public void ChangeFeedback(string idTourBooking)
        {
            var tourBooking = (from x in _db.TourBookings.AsNoTracking()
                                    where x.IdTourBooking == idTourBooking
                                    select x).FirstOrDefault();

            if (tourBooking != null)
            {
                tourBooking.IsSendFeedBack = true;
                _db.TourBookings.Update(tourBooking);
                _db.SaveChanges();
            }
        }

        public void ChangeRating(string idTour, double rating, Guid idCustomer, Guid idReview)
        {

            Review review = new Review();
            review.Id = idReview;
            review.Rating = rating;
            review.IdTour = idTour;
            review.IdCustomer = idCustomer;
            _db.reviews.Add(review);
            _db.SaveChanges();

            var tourRating = (from t in _db.reviews.AsNoTracking()
                              where t.IdTour == idTour
                              select t);

            var tour = (from t in _db.Tour.AsNoTracking()
                        where t.IdTour == idTour
                        select t).FirstOrDefault();

            var count = tourRating.Count();

            var sumRating = tourRating.Sum(r => r.Rating);

            var averge = Math.Round((sumRating / count));

            if (tour != null)
            {
                tour.Rating = averge;
                _db.Tour.Update(tour);
                _db.SaveChanges();
            }
            
        }
    }
}
