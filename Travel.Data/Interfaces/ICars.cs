using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Travel.Shared.ViewModels;
using Travel.Shared.ViewModels.Travel;

namespace Travel.Data.Interfaces
{
    public interface ICars
    {
        string CheckBeforeSave(JObject frmData, ref Notification _message, bool isUpdate);
        Response Gets(bool isDelete);
        Response UpdateCar(UpdateCarViewModel input);
        Response Create(CreateCarViewModel input);
        Response StatisticCar();
        Response GetsSelectBoxCar(long fromDate, long toDate);
        Response ViewSelectBoxCar(string idSchedule);
        Response DeleteCar(Guid id, Guid idUser);
        Response GetsSelectBoxCarUpdate(long fromDate, long toDate, string idSchedule);

        Response RestoreCar(Guid id);
        Response SearchCar(JObject frmData);
    }
}
