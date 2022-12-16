using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Travel.Context.Models.Travel;
using Travel.Data.Interfaces;
using Travel.Shared.Ultilities;
using Travel.Shared.ViewModels;

namespace Travel.Data.Repositories
{
    public class VnpayRes : IVnPay
    {
        private readonly TravelContext _db;
        private readonly IConfiguration _configuration;
        private readonly ISchedule _schedule;
        private readonly ITour _tour;
        private readonly ITourBooking _tourbooking;
        public VnpayRes(IConfiguration configuration, 
            ITourBooking tourBookingRes, 
            ISchedule schedule, 
            ITourBooking tourbooking, 
            ITour tour, TravelContext db)
        {
            _configuration = configuration;
            _schedule = schedule;
            _tourbooking = tourbooking;
            _tour = tour;
            _db = db;
        }

        public async Task<string> CreatePaymentUrl(string idTourBooking, HttpContext context)
        {
            #region get schedule
            var tourBooking = await _tourbooking.GetTourBookingByIdForPayPal(idTourBooking);

            //var schedule = await _schedule.GetScheduleByIdForPayPal(tourBooking.ScheduleId);

            //var tour = await _tour.GetTourByIdForPayPal(schedule.TourId);
            #endregion

            double total = (double)tourBooking.TotalPrice;
            

            var timeZoneById = TimeZoneInfo.FindSystemTimeZoneById(_configuration["TimeZoneId"]);
            var timeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZoneById);
            var tick = DateTime.Now.Ticks.ToString();
            var pay = new VnPayLibrary();
            var urlCallBack = $"https://localhost:44394/api/pay/callback-vnpay?idTourBooking={idTourBooking}";
            var OrderType = "";
            pay.AddRequestData("vnp_Version", _configuration["VnpaySetting:Version"]);
            pay.AddRequestData("vnp_Command", _configuration["VnpaySetting:Command"]);
            pay.AddRequestData("vnp_TmnCode", _configuration["VnpaySetting:TmnCode"]);
            pay.AddRequestData("vnp_Amount", ((total*100).ToString()));
            pay.AddRequestData("vnp_CreateDate", timeNow.ToString("yyyyMMddHHmmss"));
            pay.AddRequestData("vnp_CurrCode", _configuration["VnpaySetting:CurrCode"]);
            pay.AddRequestData("vnp_IpAddr", pay.GetIpAddress(context));
            pay.AddRequestData("vnp_Locale", _configuration["VnpaySetting:Locale"]);
            pay.AddRequestData("vnp_OrderInfo", $" {tourBooking.NameCustomer} thanh toan tour {tourBooking.IdTourBooking} so tien {tourBooking.TotalPrice}");
            pay.AddRequestData("vnp_OrderType", OrderType);
            pay.AddRequestData("vnp_ReturnUrl", urlCallBack);
            pay.AddRequestData("vnp_TxnRef", tick);


            var paymentUrl =
                pay.CreateRequestUrl(_configuration["VnpaySetting:BaseUrl"], _configuration["VnpaySetting:HashSecret"]);

            var stntntnt = ParseQueryString(paymentUrl);

            return paymentUrl;
        }
        private void UpdateDatabase<T>(T input)
        {
            _db.Entry(input).State = EntityState.Modified;
        }
        private async Task<int> SaveChangeAsync()
        {
           return await _db.SaveChangesAsync();
        }
      
        public static Dictionary<string, string> ParseQueryString(string queryString)
        {
            var nvc = HttpUtility.ParseQueryString(queryString);
            return nvc.AllKeys.ToDictionary(k => k, k => nvc[k]);
        }
        public async Task<PaymentResponse> PaymentExecute(IQueryCollection collections, string idTourBooking)
        {
            var pay = new VnPayLibrary();
            var response = pay.GetFullResponseData(collections, _configuration["VnpaySetting:HashSecret"]);
            if (response.Success == false)
            {
                response.UrlReturnBill = $"{_configuration["UrlClientCustomer"]}/bill/{idTourBooking}";
                return response;
            }
            if (response.Success == true)
            {
                if (response.VnPayResponseCode == "00")
                {
                   await _tourbooking.DoPayment(idTourBooking);
                }
                response.UrlReturnBill = $"{_configuration["UrlClientCustomer"]}/bill/{idTourBooking}";
            }
            return response;
        }

    }
}
