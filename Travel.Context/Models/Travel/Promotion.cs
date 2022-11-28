using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Travel.Context.Models
{
    public class Promotion
    {

        public int IdPromotion { get; set; }
        public int Value { get; set; }
        public long ToDate { get; set; }
        public long FromDate { get; set; }
        public Guid IdUserModify { get; set; }
        public string ModifyBy { get; set; }
        public long ModifyDate { get; set; }
        public int Approve { get; set; }
        public string IdAction { get; set; }
        public string TypeAction { get; set; }
        public bool IsTempdata { get; set; }
        public bool IsDelete { get; set; }
        public  ICollection<Schedule> Schedules { get; set; }
    }
}
