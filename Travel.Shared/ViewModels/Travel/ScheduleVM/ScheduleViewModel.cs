using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Travel.Context.Models;

namespace Travel.Shared.ViewModels.Travel
{
    public class ScheduleViewModel
    {
        private string idSchedule;
        private string alias;
        private string description;
        private long departureDate;
        private string departurePlace;
        private long returnDate;
        private long beginDate;
        private long endDate;
        private long timePromotion;

        private int status;
        private float quantityAdult;
        private float quantityBaby;
        private int minCapacity;
        private int maxCapacity;
        private float quantityChild;

        private string tourId;
        private string nameTour;

        private Guid carId;
        private string liscensePlate;
        private string nameDriver;

        private Guid employeeId;
        private string nameEmployee;

        private int promotionId;
        private int valuePromotion;
        private float priceAdult;
        private float priceChild;
        private float priceBaby;
        private float priceAdultHoliday;
        private float priceChildHoliday;
        private float priceBabyHoliday;

        private int approve;
        private string idUserModify;
        private string modifyBy;
        private long modifyDate;
        private string typeAction;
        private Boolean isHoliday;
        private Boolean isDelete;
        // costtour
        private float additionalPrice;
        private float additionalPriceHoliday;

        private float totalCostTourNotService;
        private int profit;
        private float vat;

        private float finalPrice;
        private float finalPriceHoliday;

        // service


        //private CostTour costTour;
        private Tour tour;
        private ICollection<Timeline> timelines;

        public string IdSchedule { get => idSchedule; set => idSchedule = value; }
        public long DepartureDate { get => departureDate; set => departureDate = value; }
        public long BeginDate { get => beginDate; set => beginDate = value; }
        public long EndDate { get => endDate; set => endDate = value; }
        public long TimePromotion { get => timePromotion; set => timePromotion = value; }
        public int Status { get => status; set => status = value; }
        public float QuantityAdult { get => quantityAdult; set => quantityAdult = value; }
        public float QuantityBaby { get => quantityBaby; set => quantityBaby = value; }
        public int MinCapacity { get => minCapacity; set => minCapacity = value; }
        public int MaxCapacity { get => maxCapacity; set => maxCapacity = value; }
        public float QuantityChild { get => quantityChild; set => quantityChild = value; }
        public string TourId { get => tourId; set => tourId = value; }
        public string NameTour { get => nameTour; set => nameTour = value; }
        public Guid CarId { get => carId; set => carId = value; }
        public string LiscensePlate { get => liscensePlate; set => liscensePlate = value; }
        public string NameDriver { get => nameDriver; set => nameDriver = value; }
        public Guid EmployeeId { get => employeeId; set => employeeId = value; }
        public string NameEmployee { get => nameEmployee; set => nameEmployee = value; }
        public int PromotionId { get => promotionId; set => promotionId = value; }
        public ICollection<Timeline> Timelines { get => timelines; set => timelines = value; }
        public int ValuePromotion { get => valuePromotion; set => valuePromotion = value; }
        public Tour Tour { get => tour; set => tour = value; }
        public long ReturnDate { get => returnDate; set => returnDate = value; }
        public float FinalPriceHoliday { get => finalPriceHoliday; set => finalPriceHoliday = value; }
        public float FinalPrice { get => finalPrice; set => finalPrice = value; }
        public float Vat { get => vat; set => vat = value; }
        public int Profit { get => profit; set => profit = value; }
        public float TotalCostTourNotService { get => totalCostTourNotService; set => totalCostTourNotService = value; }
        public float AdditionalPrice { get => additionalPrice; set => additionalPrice = value; }
        public float AdditionalPriceHoliday { get => additionalPriceHoliday; set => additionalPriceHoliday = value; }
        //public CostTour CostTour { get => costTour; set => costTour = value; }
        public string Description { get => description; set => description = value; }

        public string Alias { get => alias; set => alias = value; }
        public string DeparturePlace { get => departurePlace; set => departurePlace = value; }
        public float PriceAdult { get => priceAdult; set => priceAdult = value; }
        public float PriceChild { get => priceChild; set => priceChild = value; }
        public float PriceBaby { get => priceBaby; set => priceBaby = value; }
        public float PriceAdultHoliday { get => priceAdultHoliday; set => priceAdultHoliday = value; }
        public float PriceChildHoliday { get => priceChildHoliday; set => priceChildHoliday = value; }
        public float PriceBabyHoliday { get => priceBabyHoliday; set => priceBabyHoliday = value; }
        public bool IsHoliday { get => isHoliday; set => isHoliday = value; }
        public bool IsDelete { get => isDelete; set => isDelete = value; }
        public string TypeAction { get => typeAction; set => typeAction = value; }
        public string IdUserModify { get => idUserModify; set => idUserModify = value; }
        public int Approve { get => approve; set => approve = value; }
        public string ModifyBy { get => modifyBy; set => modifyBy = value; }
        public long ModifyDate { get => modifyDate; set => modifyDate = value; }
    }
}
