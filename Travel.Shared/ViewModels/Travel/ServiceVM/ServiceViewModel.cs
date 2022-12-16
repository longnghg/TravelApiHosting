using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Travel.Shared.ViewModels.Travel.ContractVM;

namespace Travel.Shared.ViewModels.Travel.ServiceVM
{
    public class ServiceViewModel
    {
        public class RestaurantViewModel : ParentProperty
        {
            private Guid idRestaurant;
            public Guid IdRestaurant { get => idRestaurant; set => idRestaurant = value; }
            public float ComboPrice { get => comboPrice; set => comboPrice = value; }

            private float comboPrice;
    
        }
        public class HotelViewModel : ParentProperty
        {
            private Guid idHotel;
            private int star;
            private int quantitySR;
            private int quantityDBR;
            private float singleRoomPrice;
            private float doubleRoomPrice;

            public Guid IdHotel { get => idHotel; set => idHotel = value; }
            public int Star { get => star; set => star = value; }
            public int QuantitySR { get => quantitySR; set => quantitySR = value; }
            public int QuantityDBR { get => quantityDBR; set => quantityDBR = value; }
            public float SingleRoomPrice { get => singleRoomPrice; set => singleRoomPrice = value; }
            public float DoubleRoomPrice { get => doubleRoomPrice; set => doubleRoomPrice = value; }
        }
        public class PlaceViewModel : ParentProperty
        {
            private Guid idPlace;        
          //  private string namePlace;    
            private float priceTicket;

            public Guid IdPlace { get => idPlace; set => idPlace = value; }
           // public string NamePlace { get => namePlace; set => namePlace = value; }
            public float PriceTicket { get => priceTicket; set => priceTicket = value; }
        }
        public class ParentProperty : UpdateApproveData
        {
            private Guid contractId;
            private string modifyBy;
            private long modifyDate;
            private string phone;
            private string address;
            private string name;
            private int approve;
            private bool isDelete;
            private Guid provinceId;
            private Guid districtId;
            private Guid wardId;
            public string ModifyBy { get => modifyBy; set => modifyBy = value; }
            public long ModifyDate { get => modifyDate; set => modifyDate = value; }
            public string Phone { get => phone; set => phone = value; }
            public string Address { get => address; set => address = value; }
            public string Name { get => name; set => name = value; }
            public Guid ContractId { get => contractId; set => contractId = value; }
            public int Approve { get => approve; set => approve = value; }
            public bool IsDelete { get => isDelete; set => isDelete = value; }
            public Guid ProvinceId { get => provinceId; set => provinceId = value; }
            public Guid DistrictId { get => districtId; set => districtId = value; }
            public Guid WardId { get => wardId; set => wardId = value; }
        }
    }
}
