using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Travel.Shared.ViewModels.Travel.TourBookingVM
{
    public class UpdateBookingDetailViewModel : CreateBookingDetailViewModel
    {

    }
    public class CreateBookingDetailViewModel
    {
        private string idTourbookingDetails ;
        private int    baby ;
        private int    child ;
        private int    adult ;
        private Ultilities.Enums.StatusBooking status ;
        private string note;
        private Guid hotelId;
        private Guid restaurantId;
        private Guid placeId;

        public string IdTourbookingDetails { get => idTourbookingDetails; set => idTourbookingDetails = value; }
       
        public int Baby { get => baby; set => baby = value; }
        public int Child { get => child; set => child = value; }
        public int Adult { get => adult; set => adult = value; }
        public Ultilities.Enums.StatusBooking Status { get => status; set => status = value; }
        public Guid HotelId { get => hotelId; set => hotelId = value; }
        public Guid RestaurantId { get => restaurantId; set => restaurantId = value; }
        public Guid PlaceId { get => placeId; set => placeId = value; }
        public string Note { get => note; set => note = value; }
    }
}
