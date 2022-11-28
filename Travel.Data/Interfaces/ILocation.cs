using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Travel.Context.Models;
using Travel.Shared.ViewModels;
using Travel.Shared.ViewModels.Travel;
using Travel.Shared.ViewModels.Travel.DistrictVM;
using Travel.Shared.ViewModels.Travel.WardVM;

namespace Travel.Data.Interfaces
{
    public interface ILocation
    {
        string CheckBeforeSaveProvince(JObject frmData, ref Notification _message, bool isUpdate);
        string CheckBeforeSaveDistrict(JObject frmData, ref Notification _message, bool isUpdate);
        string CheckBeforeSaveWard(JObject frmData, ref Notification _message, bool isUpdate);
        Response GetsProvince();
        Response GetsDistrict();
        Response GetsWard();
        Response SearchProvince(JObject frmData);
        Response SearchDistrict(JObject frmData);
        Response SearchWard(JObject frmData);
        Response CreateProvince(CreateProvinceViewModel province);
        Response CreateDistrict(CreateDistrictViewModel district);
        Response CreateWard(CreateWardViewModel ward);
        Response UpdateProvince(UpdateProvinceViewModel province);
        Response UpdateDistrict(UpdateDistrictViewModel district);
        Response UpdateWard(UpdateWardViewModel ward);
        Response DeleteProvince(Guid idProvince);
        Response DeleteDistrict(Guid idDistrict);
        Response DeleteWard(Guid idWard);
    }
}
