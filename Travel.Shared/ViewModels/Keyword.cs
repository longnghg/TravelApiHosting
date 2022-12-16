using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Travel.Shared.ViewModels
{
    public class Keywords: Pagination
    {
        public bool IsDelete { get; set; }
        public bool IsBlock { get; set; }
        public string Keyword { get; set; }
        public string KwName { get; set; }
        public string KwId { get; set; }
        public string KwPhone { get; set; }
        public string KwEmail { get; set; }
        public List<int> KwIdRole { get; set; }
        public List<string> KwIdProvince { get; set; }
        public List<string> KwIdDistrict { get; set; }
        public List<string> KwIdWard { get; set; }
        public string KwStatus { get; set; }
        public bool KwIsActive { get; set; }
        public string KwDescription { get; set; }
        public List<int> KwPayment { get; set; }
        public List<int> KwStar{ get; set; }
        public List<int> KwStatusList { get; set; }
        public List<string> KwTypeActions { get; set; }
        public string KwAddress { get; set; }
        public string KwPriceTicket { get; set; }
        public string KwComboPrice{ get; set; }
        public string KwPincode { get; set; }
        public string KwBookingNo { get; set; }
        public bool kwIsCalled { get; set; }
        public long KwDate { get; set; }
        public long KwFromDate { get; set; }
        public long KwToDate { get; set; }
        public long KwDepartureDate { get; set; }
        public long KwReturnDate { get; set; }
        public string KwToPlace { get; set; }
        public List<int> KwRating { get; set; }
        public string KwTypeAction { get; set; }
        public int KwAprroveStatus { get; set; }
        public long KwBeginDate { get; set; }
        public long KwEndDate { get; set; }
        public float KwTotalCostTourNotService { get; set; }
        public float KwFinalPrice { get; set; }
        public float KwFinalPriceHoliday { get; set; }

        public string KwIdTour { get; set; }
        public string KwModifyBy { get; set; }
        public int KwAmount { get; set; }
        public string KwLiscensePlate { get; set; }
        public string KwFrom { get; set; }
        public string KwTo { get; set; }
        public float KwPriceFrom { get; set; }
        public float KwPriceTo { get; set; }

        public int KwPromotion { get; set; }

        public bool KwIsHoliday { get; set; }

        public bool KwIsAllOption { get; set; }
        public int KwValue { get; set; }

        public int KwPoint { get; set; }
    }
    public class Pagination
    {
        public int pageSize { get; set; }
        public int pageIndex { get; set; }
    }
}
