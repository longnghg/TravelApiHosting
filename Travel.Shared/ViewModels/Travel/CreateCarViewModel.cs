using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Travel.Shared.ViewModels.Travel
{
    public class UpdateCarViewModel : CreateCarViewModel
    {


    }
    public class CreateCarViewModel
    {
        private Guid idCar;
        private string liscensePlate;
        private int status;
        private int amountSeat;
        private string nameDriver;
        private string phone;

        private string modifyBy;
        private long modifyDate;
        private bool isDelete;
        private Guid idUserModify;
        public Guid IdCar { get => idCar; set => idCar = value; }
        public string LiscensePlate { get => liscensePlate; set => liscensePlate = value; }
        public int Status { get => status; set => status = value; }
        public int AmountSeat { get => amountSeat; set => amountSeat = value; }
        public string NameDriver { get => nameDriver; set => nameDriver = value; }
        public string Phone { get => phone; set => phone = value; }
        public string ModifyBy { get => modifyBy; set => modifyBy = value; }
        public long ModifyDate { get => modifyDate; set => modifyDate = value; }
        public bool IsDelete { get => isDelete; set => isDelete = value; }
        public Guid IdUserModify { get => idUserModify; set => idUserModify = value; }
    }
}
