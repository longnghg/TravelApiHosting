using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TravelApi.Calendar
{
    public class GoogleCalendar
    {
        public string EmailNhan { get; set; }
        public string Summary { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
    }
}
