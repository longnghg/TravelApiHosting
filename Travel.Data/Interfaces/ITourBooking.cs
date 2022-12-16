using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Travel.Context.Models;
using Travel.Data.Interfaces;
using Travel.Shared.ViewModels;
using Travel.Shared.ViewModels.Travel.TourBookingVM;
namespace Travel.Data.Interfaces
{
   public interface ITourBooking
    {
        string CheckBeforSave(JObject frmData, ref Notification _message, bool isUpdate = false);
        Task<Response> Gets();
        Task<Response> Create(CreateTourBookingViewModel input, string emailUser);
        Task<Response> TourBookingById(string idTourbooking);
        Response GetTourBookingFromDateToDate(DateTime? fromDate, DateTime? toDate);
        Task<Response> DoPayment(string idTourBooking);
        // customer
        Task<Response> CancelBooking(string idTourBooking);
        Task<Response> RestoreBooking(string idTourBooking, string emailUser);
        Task<Response> TourBookingByBookingNo(string bookingNo);
        Response StatisticTourBooking();

        Response CheckCalled(string idTourBooking);

        Response SearchTourBooking(JObject frmData);

        Response UpdateStatus(string pincode, string emailUser);
        Task<TourBooking> GetTourBookingByIdForPayPal (string idTourBooking);
        Task<TourBooking> GetTourBookingByIdForVnPay(string idTourBooking);

        Task<bool> UpdateTourBookingFinished();
        Task<bool> ChangePayment(string idTourBooking, int idPayment);
        Response CheckInBooking(string bookingNo);

        byte[] CreateByteQR(string qrCodeText);
        string AddImg(string qrCodeText, string idService);
        Response StatisticPaidNotCheckedin();

        Response CusSearchBookingNo(string bookingNo);

        Response UpdateStatus(string idTourBooking, int status, string emailUser);
    }
}
