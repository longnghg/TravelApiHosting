using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Travel.Context.Models;
using Travel.Shared.ViewModels;
using Travel.Shared.ViewModels.Travel;
using static Travel.Shared.ViewModels.Travel.CreateTimeLineViewModel;

namespace Travel.Data.Interfaces
{
    public interface ITimeLine
    {
        string CheckBeforSave(JObject frmData, ref Notification _message, bool isUpdate = false);
        Response Create(ICollection<CreateTimeLineViewModel> input);
        Response Update(ICollection<UpdateTimeLineViewModel> input);
        Response Delete(ICollection<Timeline> timelines);
        Response Get();
        Response GetTimelineByIdSchedule(string IdSchedule);
    }
}
