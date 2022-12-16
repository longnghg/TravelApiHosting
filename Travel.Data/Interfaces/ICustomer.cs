using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Travel.Shared.ViewModels;
using Travel.Shared.ViewModels.Travel.CustomerVM;

namespace Travel.Data.Interfaces
{
    public interface ICustomer
    {
        string CheckBeforeSave(JObject frmdata, ref Notification _message, bool isUpdate);
        Response Gets();

        Response Create(CreateCustomerViewModel input);
        Response GetsHistory(Guid idCustomer);
        Task<Response> SendOTP(string email);
        Response GetCustomer(Guid idCustomer);
         Task<Response> UpdateCustomer(UpdateCustomerViewModel input);
        Task<Response> CustomerSendRate(string idTour, int rating);

        Task<bool> UpdateScoreToCustomer(Guid idCustomer, int point );
        Task<Response> UpdateBlockCustomer(Guid idCustomer, bool isBlock);
        Response Search(JObject frmData);
    }
}
