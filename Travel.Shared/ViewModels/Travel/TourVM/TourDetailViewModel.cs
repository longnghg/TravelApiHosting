using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Travel.Shared.ViewModels.Travel.TourVM
{
    public class TourDetailViewModel
    {
        private string idTourDetail ;
        private string tourId ;
        private string description ;
        private int    quantityBooked ;

        public string IdTourDetail { get => idTourDetail; set => idTourDetail = value; }
        public string TourId { get => tourId; set => tourId = value; }
        public string Description { get => description; set => description = value; }
        public int QuantityBooked { get => quantityBooked; set => quantityBooked = value; }

    }
}
