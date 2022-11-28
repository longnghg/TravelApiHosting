using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Travel.Shared.ViewModels.Travel.CustomerVM
{
    public class HistoryBookedViewModel
    {
        private string bookingNo ;
        private string idSchedule ;
        private string idTourBooking;
        private long dateBooking ;
        private float totalPrice ;
        private int valuePromotion ;
        private string thumbnail ;
        private int status;
        private long departureDate ;
        private long returnDate ;

        private string description;
        private string nameTour;

        private int adult;
        private int child;
        private int baby;

        private string fromPlace;
        private string toPlace;

        public long DepartureDate { get => departureDate; set => departureDate = value; }
        public long ReturnDate { get => returnDate; set => returnDate = value; }
        public int ValuePromotion { get => valuePromotion; set => valuePromotion = value; }
        public string Thumbnail { get => thumbnail; set => thumbnail = value; }
        public string BookingNo { get => bookingNo; set => bookingNo = value; }
        public string IdSchedule { get => idSchedule; set => idSchedule = value; }
        public long DateBooking { get => dateBooking; set => dateBooking = value; }
        public float TotalPrice { get => totalPrice; set => totalPrice = value; }
        public int Status { get => status; set => status = value; }
        public string Description { get => description; set => description = value; }
        public string NameTour { get => nameTour; set => nameTour = value; }
        public int Adult { get => adult; set => adult = value; }
        public int Child { get => child; set => child = value; }
        public int Baby { get => baby; set => baby = value; }
        public string FromPlace { get => fromPlace; set => fromPlace = value; }
        public string ToPlace { get => toPlace; set => toPlace = value; }
        public string IdTourBooking { get => idTourBooking; set => idTourBooking = value; }
    }
}
