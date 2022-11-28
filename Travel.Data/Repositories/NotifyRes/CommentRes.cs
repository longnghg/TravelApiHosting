using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                if (customer != null)
                {
                    Comment cmt = new Comment();
                    cmt.IdComment = new Guid();
                    cmt.NameCustomer = customer.NameCustomer;
                    cmt.CommentText = input.CommentText;
                    cmt.CommentTime = Ultility.ConvertDatetimeToUnixTimeStampMiliSecond(DateTime.Now);

                    cmt.IdCustomer = input.IdCustomer;
                    cmt.IdTour = input.IdTour;
                    await _notifyContext.AddAsync(cmt);
                    await _notifyContext.SaveChangesAsync();
                    return Ultility.Responses("Tạo mới thành công !", Enums.TypeCRUD.Success.ToString());
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
                var cmt = await (from x in _notifyContext.Comment.AsNoTracking()
                                 where x.IdTour == idTour
                                 orderby x.CommentTime descending
                                 select x).ToListAsync();
                return Ultility.Responses("", Enums.TypeCRUD.Success.ToString(), cmt);                   
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
    }
}
