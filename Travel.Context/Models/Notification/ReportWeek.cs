using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Travel.Context.Models.Notification
{
    public class ReportWeek
    {
        public Guid IdWeek { get; set; }
        public int Week { get; set; }
        public int Year { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }

    }
}
