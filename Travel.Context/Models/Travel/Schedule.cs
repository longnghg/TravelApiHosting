using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Travel.Context.Models
{
    public class Schedule
    {
        [Key]
        public string IdSchedule { get; set; }
        public string Alias { get; set; }
        public string DeparturePlace { get; set; }
        public long DepartureDate { get; set; }
        public long ReturnDate { get; set; }
        public long BeginDate { get; set; }
        public long EndDate { get; set; }
        public long TimePromotion { get; set; }
        public int Status { get; set; }
        public int Approve { get; set; }
        public string IdAction { get; set; }
        public string TypeAction { get; set; }
        public Guid IdUserModify { get; set; }
        public bool IsTempData { get; set; }
        public int QuantityAdult { get; set; }
        public int QuantityBaby { get; set; }
        public int MinCapacity { get; set; }
        public int MaxCapacity { get; set; }
        public int QuantityCustomer { get; set; }
        public int QuantityChild { get; set; }
        public bool Isdelete { get; set; }
        public bool IsHoliday { get; set; }
        public string Description { get; set; }
        public string ModifyBy { get; set; }
        public long ModifyDate { get; set; }

        public float TotalCostTourNotService { get; set; }
        public float TotalCostTour { get; set; }
        public int Profit { get; set; }
        public float Vat { get; set; }


        public float AdditionalPrice { get; set; }
        public float AdditionalPriceHoliday { get; set; }
        public float FinalPrice { get; set; }
        public float FinalPriceHoliday { get; set; }



        public float PriceAdult { get; set; }
        public float PriceChild { get; set; }
        public float PriceBaby { get; set; }
        public float PriceAdultHoliday { get; set; }
        public float PriceChildHoliday { get; set; }
        public float PriceBabyHoliday { get; set; }


        public string MetaDesc { get; set; }
        public string MetaKey { get; set; }




        public string TourId { get; set; }
        public Guid CarId { get; set; }
        public int PromotionId { get; set; }
        public Guid EmployeeId { get; set; }
        public CostTour CostTour { get; set; }
        public Car Car { get; set; }
        public Promotion Promotions { get; set; }
        public Tour Tour { get; set; }
        public ICollection<Timeline> Timelines { get; set; }
        public ICollection<TourBooking> TourBookings { get; set; }
        public Employee Employee { get; set; }
        
    }
}
