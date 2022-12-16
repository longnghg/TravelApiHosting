using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Travel.Context.Models;

namespace Travel.Shared.ViewModels.Travel
{
    public class TimeLineViewModel
    {
        private string title;
        private string idTimeLine;
        private string description;
        private long fromTime;
        private long toTime;
        private string modifyBy;
        private long modifyDate;
        private string idSchedule;
        private Schedule schedule;
      

        public string IdTimeLine { get => idTimeLine; set => idTimeLine = value; }
        public string Title { get => title; set => title = value; }
        public string Description { get => description; set => description = value; }
        public long FromTime { get => fromTime; set => fromTime = value; }
        public long ToTime { get => toTime; set => toTime = value; }
        public string ModifyBy { get => modifyBy; set => modifyBy = value; }
        public long ModifyDate { get => modifyDate; set => modifyDate = value; }
        public Schedule Schedule { get => schedule; set => schedule = value; }
        public string IdSchedule { get => idSchedule; set => idSchedule = value; }
    }
}
