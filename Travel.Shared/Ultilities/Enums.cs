using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Travel.Shared.Ultilities
{
    public  class Enums
    {
        public enum StatusBooking
        {
            Pending = -2, // đã huỷu và đang chờ hoàn tiền
            Refunded = -1, // đã hoàn tiền

            Paying = 1, // Đã đặt tour nhưng chưa thanh toán
            Deposit = 2, // Đã đặt tour và có đặt cọc

            Paid = 3, //  Đã thanh toán hết
            Cancel = 4, // Hủy tour
            Finished = 5, // Tour đã hoàn thành

        }
        public enum TypeService
        {
            Hotel = 1,
            Restaurant = 2,
            Place = 3,
        }
        public enum StatusSchedule
        {
            Free = 0, // tour đang rảnh
            Busy = 1, // hết chỗ
            Going = 2, // tour đang đi
            Finished = 3, // tour đã hoàn thành tour
        }
        public enum StatusCar
        {
            
            Free = 0, // xe đang rảnh
            Busy = 1, // xe đã có tour
            Full = 2, // xe đang đầy
        }
        public enum TitleRole
        {
            Admin = -1,
            LocalManager = 1,
            ServiceManager = 2,
            TourManager = 3,
            TourBookingManager = 4,
        }
        public enum StatusContract
        {
            Expired = 0,
            Valid = 1,
        }
        public enum ApproveStatus
        {
            Waiting = 0,
            Approved = 1,
            Refused = 2,
            CancelRequired =3
        }
        public enum TourStatus
        {
            Normal = 0,
            Promotion = 1,
            Refused = 2
        }
        public enum TypeCRUD
        {
            Success = 1,
            Error=  2,
            Warning = 3,
            Info = 4,
            Validation = 5,
            Block = 6
        }

        public enum TypeNotification
        {
            Tour = 0,
            Hotel = 1,
            Place = 2,
            Restaurant = 3,
            Promotion = 4
        }

    }
}
