using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Travel.Context.Models
{
    
    public class Car
    {
      
        public Guid IdCar { get; set; }
        public string LiscensePlate { get; set; }
        public int Status { get; set; }
        public int AmountSeat { get; set; }
        public string NameDriver { get; set; }
        public string Phone { get; set; }

        public Guid IdUserModify { get; set; }
        public string ModifyBy { get; set; }
        public long ModifyDate { get; set; }
        public bool IsDelete { get; set; }
        public  ICollection<Schedule> Schedules { get; set; }

    }
}
