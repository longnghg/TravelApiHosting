using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Travel.Context.Models;
using Travel.Shared.ViewModels;
using Travel.Shared.ViewModels.Travel.CostTourVM;

namespace Travel.Data.Interfaces
{
    public interface ICostTour
    {
        string CheckBeforSave(JObject frmData, ref Notification _message, bool isUpdate = false);
        Response Create(CreateCostViewModel input);
        Response Get();
        Response GetCostByIdSchedule(string IdSchedule);
        Response Update(UpdateCostViewModel input);
    }
}
