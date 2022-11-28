using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Travel.Context.Models
{
    public class Timeline
    {
        public Guid IdTimeline { get; set; }
        public string Description { get; set; }
        public long FromTime { get; set; }
        public long ToTime { get; set; }
        public string ModifyBy { get; set; }
        public long ModifyDate { get; set; }
        public bool IsDelete { get; set; }
        public virtual Schedule Schedule { get; set; }
        public string IdSchedule { get; set; }

    }
}
