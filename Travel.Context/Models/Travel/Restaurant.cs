using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Travel.Context.Models
{
    public class Restaurant
    {
        public Guid IdRestaurant { get; set; }
        public Guid ContractId { get; set; }
        public string NameRestaurant { get; set; }
        public float ComboPrice { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public int Approve { get; set; }
        public bool IsTempdata { get; set; }
        public string IdAction { get; set; }
        public string TypeAction { get; set; }
        public Guid IdUserModify { get; set; }
        public string ModifyBy { get; set; }
        public long ModifyDate { get; set; }
        public bool IsDelete { get; set; }
        public Guid ProvinceId { get; set; }
        public Guid DistrictId { get; set; }
        public Guid WardId { get; set; }
        public virtual ICollection<CostTour> CostTours { get; set; }
        [NotMapped]

        public virtual ICollection<TourBookingDetails> TourBookingDetails { get; set; }

    }
}
