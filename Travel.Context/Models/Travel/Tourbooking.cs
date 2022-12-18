using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Travel.Context.Models
{
    [NotMapped]
    public class TourBooking
    {
        public string IdTourBooking{get;set;}
        public int PaymentId { get; set; }
        public Guid CustomerId { get; set; }

        public string ScheduleId { get; set; }


        public string NameCustomer { get;set;}
        public string NameContact { get;set;}
        public string Phone{get;set;}
        public string BookingNo{get;set;}
        public string Pincode{get;set;}       
        public long   DateBooking{get;set;}
        public long   LastDate{get;set;}
        public double Vat{get;set;}
        public string Address{get;set;}
        public string Email{get;set;}
        public string VoucherCode{get;set;}
        public int ValuePromotion { get; set; }
        public bool   IsCalled{get;set;}
        public float  Deposit{get;set;}
        public float  RemainPrice{get;set;}
        public float TotalPrice { get; set; }
        public float AdditionalPrice{get;set;}
        public float TotalPricePromotion { get; set;}
        public string ModifyBy{get;set;}
        public int Status {get;set; }
        public long CheckIn { get; set; }
        public long CheckOut { get; set; }
        public long ModifyDate{get;set;}
        public string UrlQR { get;set;}
        public bool IsSendFeedBack { get; set; }

        public Schedule Schedule { get; set; }
        public  Payment Payment { get; set; }
        public TourBookingDetails TourBookingDetails { get; set; }

    }
}
