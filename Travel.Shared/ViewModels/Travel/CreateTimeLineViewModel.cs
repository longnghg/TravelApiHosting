using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Travel.Shared.ViewModels.Travel.ContractVM;

namespace Travel.Shared.ViewModels.Travel
{
    public class UpdateTimeLineViewModel : CreateTimeLineViewModel
    {
        private Guid idTimeLine;

        public Guid IdTimeLine { get => idTimeLine; set => idTimeLine = value; }
    }
    public class CreateTimeLineViewModel:UpdateApproveData
    {
        private string title;
        private string description;
        private long fromTime;
        private long toTime;
        private string idSchedule;
        private string idScheduleTmp;


        public string Title { get => title; set => title = value; }
        public string Description { get => description; set => description = value; }
        public long FromTime { get => fromTime; set => fromTime = value; }
        public long ToTime { get => toTime; set => toTime = value; }
        public string IdSchedule { get => idSchedule; set => idSchedule = value; }
        public string IdScheduleTmp { get => idScheduleTmp; set => idScheduleTmp = value; }
    }
}
