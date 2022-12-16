using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Travel.Context.Models
{
    public class Tour
    {     
        public string IdTour { get; set; }
        public string NameTour { get; set; }
        public string NameTour_EN { get; set; }
        public string Alias { get; set; }
        public double Rating { get; set; }
        public string ToPlace { get; set; }
        public int ApproveStatus { get; set; }
        public string IdAction { get; set; }
        public string TypeAction { get; set; }
        public bool IsTempdata { get; set; }
        public int Status { get; set; }
        public long CreateDate { get; set; }
        public Guid IdUserModify { get; set; }
        public string ModifyBy { get; set; }
        public long ModifyDate { get; set; }
        public bool IsDelete { get; set; }
        public bool IsActive { get; set; }
        public string Thumbnail { get; set; }
        public int QuantityBooked { get; set; }
        public ICollection<Schedule> Schedules { get; set; }

    }
}
