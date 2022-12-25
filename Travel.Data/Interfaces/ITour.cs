using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Travel.Context.Models;
using Travel.Shared.ViewModels;
using Travel.Shared.ViewModels.Travel.TourVM;
using static Travel.Shared.Ultilities.Enums;

namespace Travel.Data.Interfaces
{
   public  interface ITour
    {
        string CheckBeforSave(IFormCollection frmdata, IFormFile file, ref Notification _message, bool isUpdate = false);
        Response Create(CreateTourViewModel input, string emailUser);
        Response Update(UpdateTourViewModel input, string emailUser);
        Response Get(bool isDelete);
        Response GetWaiting(Guid idUser, int pageIndex, int pageSize);
        Response GetTour(string idTour);
        Response Delete(string idTour,Guid idUser, string emailUser);
        Response RestoreTour(string idTour, Guid idUser, string emailUser);
        Response Approve(string idTour );
        Response Refused(string idTour);
        Task<Response> GetsTourWithSchedule();
        Task<Response> GetTourById(string idTour);
        Task<Response> GetsTourByRating(int pageIndex, int pageSize);
        Task<Response> SearchAutoComplete(string key);
        Response UpdateRating(int rating , string idTour, string emailUser);
        Response SearchTour(JObject frmData);
        Response SearchTourWaiting(JObject frmData);
        Task<Tour> GetTourByIdForPayPal(string idTour);

        void DeleteTourImme(string id);





        Task<Tour> ServiceGetNameTourrByIdSchedule(string idSchedule);
    }
}
