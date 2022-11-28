using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Travel.Shared.ViewModels.Travel
{
    public class CostTourViewModel
    {
        private Guid idCostTour;
        private string tourDetailId;
        private float breakfast;
        private float water;
        private float feeGas;
        private float distance;
        private float sellCost;
        private float depreciation;
        private float otherPrice;
        private float tolls;
        private int cusExpected;
        private float insuranceFee;
        private bool isHoliday;
        private float totalCostTourNotService;

        private Guid hotelId;
        private string nameHotel;
        private float priceSRHotel;
        private float priceDBHotel;

        private Guid restaurantId;
        private string nameRestaurant;
        private float priceComboRestaurant;

        private Guid placeId;
        private string namePlace;
        private float priceTicketPlace;


        public Guid IdCostTour { get => idCostTour; set => idCostTour = value; }
        public string TourDetailId { get => tourDetailId; set => tourDetailId = value; }
        public float Breakfast { get => breakfast; set => breakfast = value; }
        public float Water { get => water; set => water = value; }
        public float FeeGas { get => feeGas; set => feeGas = value; }
        public float Distance { get => distance; set => distance = value; }
        public float SellCost { get => sellCost; set => sellCost = value; }
        public float Depreciation { get => depreciation; set => depreciation = value; }
        public float OtherPrice { get => otherPrice; set => otherPrice = value; }
        public float Tolls { get => tolls; set => tolls = value; }
        public int CusExpected { get => cusExpected; set => cusExpected = value; }
        public float InsuranceFee { get => insuranceFee; set => insuranceFee = value; }
        public bool IsHoliday { get => isHoliday; set => isHoliday = value; }
        public float TotalCostTourNotService { get => totalCostTourNotService; set => totalCostTourNotService = value; }
        public Guid HotelId { get => hotelId; set => hotelId = value; }
        public string NameHotel { get => nameHotel; set => nameHotel = value; }
        public float PriceSRHotel { get => priceSRHotel; set => priceSRHotel = value; }
        public Guid RestaurantId { get => restaurantId; set => restaurantId = value; }
        public string NameRestaurant { get => nameRestaurant; set => nameRestaurant = value; }
        public float PriceComboRestaurant { get => priceComboRestaurant; set => priceComboRestaurant = value; }
        public Guid PlaceId { get => placeId; set => placeId = value; }
        public string NamePlace { get => namePlace; set => namePlace = value; }
        public float PriceTicketPlace { get => priceTicketPlace; set => priceTicketPlace = value; }
        public float PriceDBHotel { get => priceDBHotel; set => priceDBHotel = value; }
    }
}
