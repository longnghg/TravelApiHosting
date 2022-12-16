using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Travel.Context.Models.Notification
{
    public class ReportTourBooking
    {
        public Guid IdReportTourBooking { get; set; }
        public string NameTour { get; set; }
        public string IdTour { get; set; }
        public long DateSave { get; set; }
        public long QuantityBooked { get; set; }
        public long TotalRevenue { get; set; }
        public long  QuantityCancel { get; set; }
        public long TotalCost { get; set; }

    }
}
