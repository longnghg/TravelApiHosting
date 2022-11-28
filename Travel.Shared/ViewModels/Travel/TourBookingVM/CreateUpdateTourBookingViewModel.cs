using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Travel.Shared.ViewModels.Travel.TourBookingVM
{
   public class   UpdateTourBookingViewModel : CreateTourBookingViewModel
    {

    }

   public class CreateTourBookingViewModel
    {
        private string idTourBooking;
        private string scheduleId;
        private int paymentId;

        private Guid? customerId;
        private string nameCustomer;
        private string address;
        private string email;
        private string phone;
        private string nameContact;
        private double vat;
        private string pincode;
        private string voucherCode;
        private int? valuePromotion;
        private float totalPrice;
        private float totalPricePromotion;
        private CreateBookingDetailViewModel bookingDetails;



        public string IdTourBooking { get => idTourBooking; set => idTourBooking = value; }
        public string NameCustomer { get => nameCustomer; set => nameCustomer = value; }
        public string Address { get => address; set => address = value; }
        public string Email { get => email; set => email = value; }
        public string Phone { get => phone; set => phone = value; }
        public string NameContact { get => nameContact; set => nameContact = value; }
        public double Vat { get => vat; set => vat = value; }
        public string Pincode { get => pincode; set => pincode = value; }
        public string VoucherCode { get => voucherCode; set => voucherCode = value; }
        public string ScheduleId { get => scheduleId; set => scheduleId = value; }
        public int PaymentId { get => paymentId; set => paymentId = value; }
        public CreateBookingDetailViewModel BookingDetails { get => bookingDetails; set => bookingDetails = value; }
        public int? ValuePromotion { get => valuePromotion; set => valuePromotion = value; }
        public Guid? CustomerId { get => customerId; set => customerId = value; }
        public float TotalPrice { get => totalPrice; set => totalPrice = value; }
        public float TotalPricePromotion { get => totalPricePromotion; set => totalPricePromotion = value; }
    }
}
