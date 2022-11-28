using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Travel.Shared.ViewModels.Travel
{
    public class CreateTimeLineViewModel
    {
        public class UpdateTimeLineViewModel : CreateTimeLineViewModel
        {
            private Guid idTimeLine;

            public Guid IdTimeLine { get => idTimeLine; set => idTimeLine = value; }
        }
        private string description;
        private long fromTime;
        private long toTime;
        private string idSchedule;

        public string Description { get => description; set => description = value; }
        public long FromTime { get => fromTime; set => fromTime = value; }
        public long ToTime { get => toTime; set => toTime = value; }
        public string IdSchedule { get => idSchedule; set => idSchedule = value; }
    }
}
