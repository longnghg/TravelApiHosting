using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Travel.Shared.ViewModels.Travel
{
    public class CarViewModel
    {
        private Guid idCar;
        private string liscensePlate;
        private int status;
        private int amountSeat;
        private string nameDriver;
        private string phone;
        private bool isDelete;

        public Guid IdCar { get => idCar; set => idCar = value; }
        public string LiscensePlate { get => liscensePlate; set => liscensePlate = value; }
        public int Status { get => status; set => status = value; }
        public int AmountSeat { get => amountSeat; set => amountSeat = value; }
        public string NameDriver { get => nameDriver; set => nameDriver = value; }
        public string Phone { get => phone; set => phone = value; }
        public bool IsDelete { get => isDelete; set => isDelete = value; }
    }
}
