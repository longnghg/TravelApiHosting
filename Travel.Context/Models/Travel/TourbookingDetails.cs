using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Travel.Context.Models
{
    public class TourBookingDetails
    {
        public string IdTourBookingDetails{get;set;}
        public int    Baby{get;set;}
        public int    Child{get;set;}
        public int    Adult{get;set;}
        public int    Status{get;set;}
       public bool   IsCalled{get;set;}
        public string Note { get; set; }
        public Guid HotelId { get; set; }
        public Guid RestaurantId { get; set; }
        public Guid PlaceId { get; set; }
        public virtual Hotel Hotel { get; set; }
        public virtual Restaurant Restaurant { get; set; }
        public virtual Place Place { get; set; }
        public virtual TourBooking TourBooking { get; set; }

    }
}
