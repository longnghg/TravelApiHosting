using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Travel.Context.Models
{
    [NotMapped]
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
        [NotMapped]
        public virtual Hotel Hotel { get; set; }
        [NotMapped]

        public virtual Restaurant Restaurant { get; set; }
        [NotMapped]

        public virtual Place Place { get; set; }
        public virtual TourBooking TourBooking { get; set; }

    }
}
