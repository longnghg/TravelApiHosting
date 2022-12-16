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
        Response CreateProvince(CreateProvinceViewModel province, string emailUser);
        Response CreateDistrict(CreateDistrictViewModel district, string emailUser);
        Response CreateWard(CreateWardViewModel ward, string emailUser);
        Response UpdateProvince(UpdateProvinceViewModel province, string emailUser);
        Response UpdateDistrict(UpdateDistrictViewModel district, string emailUser);
        Response UpdateWard(UpdateWardViewModel ward, string emailUser);
        Response DeleteProvince(Guid idProvince, string emailUser);
        Response DeleteDistrict(Guid idDistrict, string emailUser);
        Response DeleteWard(Guid idWard, string emailUser);
    }
}
