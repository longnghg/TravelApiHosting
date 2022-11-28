using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Travel.Context.Models
{
    public class Place
    {
        public Guid IdPlace {get; set;}
        public Guid ContractId { get; set; }
        public string NamePlace { get; set; }
        public string Address { get; set; }
  
        public string Phone { get; set; }
        public float PriceTicket { get; set; }
        public Guid IdUserModify { get; set; }
        public string ModifyBy { get; set; }
        public long ModifyDate { get; set; }
        public int Approve { get; set; }
        public string IdAction { get; set; }
        public string TypeAction { get; set; }
        public bool IsTempdata { get; set; }
        public bool IsDelete { get; set; }
        public virtual ICollection<CostTour> CostTours { get; set; }
        public virtual ICollection<TourBookingDetails> TourBookingDetails { get; set; }

    }
}
