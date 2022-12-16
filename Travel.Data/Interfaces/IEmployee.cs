using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Travel.Context.Models;
using Travel.Shared.ViewModels;
using Travel.Shared.ViewModels.Travel;

namespace Travel.Data.Interfaces
{
    public interface IEmployee
    {
        string CheckBeforeSave(IFormCollection frmdata, IFormFile file, ref Notification _message, bool isUpdate);
        Response GetsEmployee(bool isDelete);
        Response GetEmployee(Guid idEmployee);
        Response UpdateEmployee(UpdateEmployeeViewModel input, string emailUser);
        Response CreateEmployee(CreateEmployeeViewModel input, string emailUser);
        Response SearchEmployee(JObject frmData);
        Response RestoreEmployee(Guid idEmployee, string emailUser);
        Response DeleteEmployee(Guid idEmployee, string emailUser);
        Response StatisticEmployee();

        Task<Response> SendOTP(string email);
        Task<Response> SendFile(string email);


        Response GetsSelectBoxEmployee(long fromDate, long toDate);
        Response GetsSelectBoxEmployeeUpdate(long fromDate, long toDate, string idSchedule);
    }
}
