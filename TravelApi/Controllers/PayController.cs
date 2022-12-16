using BraintreeHttp;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PayPal.Core;
using PayPal.v1.Payments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Travel.Context.Models;
using Travel.Data.Interfaces;
using Travel.Shared.Ultilities;
using Payment = PayPal.v1.Payments.Payment;

namespace TravelApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PayController : Controller
    {
        private readonly string _clientIdPaypal;
        private readonly string _secretKeyPaypal;
        private readonly string _urlSandBoxAPI;
        private readonly ISchedule _schedule;
        private readonly ITour _tour;
        private readonly ITourBooking _tourbooking;
        public double TyGiaUSD = 25000;
        private readonly IVnPay _vnPayRes;
        private readonly IConfiguration _configuration;
        public PayController(IConfiguration config,
            ITourBooking tourBookingRes,
            ISchedule schedule,
            ITourBooking tourbooking,
            ITour tour,
            IVnPay vnPayRes)
        {
            _clientIdPaypal = config["PaypalSettings:ClientId"];
            _secretKeyPaypal = config["PaypalSettings:SecretKey"];
            _urlSandBoxAPI = config["PaypalSettings:UrlAPI"];
            _schedule = schedule;
            _tourbooking = tourbooking;
            _tour = tour;
            _vnPayRes = vnPayRes;
            _configuration = config;

        }
        //private HttpClient GetPaypalHttpClient()
        //{
        //    var http = new HttpClient
        //    {
        //        BaseAddress = new Uri(_urlSandBoxAPI),
        //        Timeout = TimeSpan.FromSeconds(30)
        //    };
        //    return http;
        //}
        //private async Task<PayPalAccessToken> GetPayPalAccessTokenAsync(HttpClient http)
        //{
        //    byte[] bytes = Encoding.GetEncoding("iso-8859-1").GetBytes($"{_clientIdPaypal}:{_secretKeyPaypal}");
        //    HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "/v1/oauth2/token");
        //    request.Headers.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(bytes));

        //    var form = new Dictionary<string, string>
        //    {
        //        ["grant_type"] = "client_credentials"
        //    };
        //    request.Content = new FormUrlEncodedContent(form);
        //    HttpResponseMessage respone = await http.SendAsync(request);

        //    string content = await respone.Content.ReadAsStringAsync();

        //    PayPalAccessToken accessToken = JsonConvert.DeserializeObject<PayPalAccessToken>(content);
        //    return accessToken;

        //}

        //private async Task<PayPalPaymentCreateResponse> CreatePaypalPaymentAsync(HttpClient http, PayPalAccessToken accessToken, double total,string currency)
        //{
        //    HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "v1/payments/payment");
        //    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken.access_token);

        //    var payment = JObject.FromObject(new
        //    {
        //        intent = "sale",
        //        redirectUrls = new RedirectUrls()
        //        {
        //            CancelUrl = "https://localhost:4200/Paypal/CheckoutFailed",
        //            ReturnUrl = "https://youtube.com"
        //        },
        //    });
        //}

        [HttpGet]
        [AllowAnonymous]
        [Route("checkout-paypal")]
        public async Task<object> PaypalCheckout(string idTourBooking)
        {
            var environment = new SandboxEnvironment(_clientIdPaypal, _secretKeyPaypal);
            var client = new PayPalHttpClient(environment);
            #region get schedule
            var tourBooking = await _tourbooking.GetTourBookingByIdForPayPal(idTourBooking);

            var schedule = await _schedule.GetScheduleByIdForPayPal(tourBooking.ScheduleId);

            var tour = await _tour.GetTourByIdForPayPal(schedule.TourId);
            #endregion
            var itemList = new ItemList()
            {
                Items = new List<Item>()
            };

            float total = 0;

            if (tourBooking.ValuePromotion != 0)
            {
                total = (float)((tourBooking.TotalPrice) / TyGiaUSD);
            }
            else
            {
                total = (float)((tourBooking.TotalPrice)/TyGiaUSD);
            }
            total = (float)Math.Round(total, 2);

            var item = new Item()
            {
                Name = tour.NameTour,
                Description = schedule.Description,
                Currency = "USD",
                Price = total.ToString(),
                Quantity = "1",
                Sku = "sku",
                Tax = tourBooking.Vat.ToString()
            };
            itemList.Items.Add(item);


            var payment = new Payment()
            {
                Intent = "sale",
                Transactions = new List<Transaction>()
                {
                    new Transaction()
                    {
                        Amount = new Amount()
                        {
                            Total = total.ToString(),
                            Currency = "USD",
                            Details = new AmountDetails
                            {
                                Tax = "0",
                                Shipping = "0",
                                Subtotal = total.ToString()
                            }
                        },
                        ItemList = itemList,
                        Description = "Hàn hóa xịn",
                        InvoiceNumber = "ko ko ko ko "

                    }
                },
                RedirectUrls = new RedirectUrls()
                {
                    //CancelUrl = _configuration["PaypalSettings:CancelUrl"],
                    CancelUrl = $"{_configuration["UrlClientCustomer"]}bill/{idTourBooking}",
                    ReturnUrl = $"{_configuration["PaypalSettings:ReturnUrl"]}api/pay/check-paypal?idTourBooking={idTourBooking}"
                },
                Payer = new Payer()
                {
                    PaymentMethod = "paypal"
                }
            };
            PaymentCreateRequest request = new PaymentCreateRequest();
            request.RequestBody(payment);
            try
            {
                
                var response = await client.Execute(request);
                var statusCode = response.StatusCode;
                Payment result = response.Result<Payment>();
                var links = result.Links.GetEnumerator();
                string paypalRedirectUrl = null;
              
                while (links.MoveNext())
                {
                    LinkDescriptionObject lnk = links.Current;
                    if (lnk.Rel.ToLower().Trim().Equals("approval_url"))
                    {
                        paypalRedirectUrl = lnk.Href;
                    }
                }

                // update
         
                return new { status = 1, url = paypalRedirectUrl };
            }
            catch (HttpException httpException)
            {
                var statusCode = httpException.StatusCode;
                var debugId = httpException.Headers.GetValues("PayPal-Debug-Id").FirstOrDefault();
                return new { status = 1, url = payment.RedirectUrls.CancelUrl, DebugId = debugId, StatusCode = statusCode };
            }
        }



        [HttpGet]
        [AllowAnonymous]
        [Route("check-paypal")]
        public async Task<object> UpdateStatusTourbooking(string idTourBooking)
        {
        
              await  _tourbooking.DoPayment(idTourBooking);
            
            return Redirect($"{_configuration["UrlClientCustomer"]}/bill/{idTourBooking}");

        }

        [HttpGet]
        [AllowAnonymous]
        [Route("checkout-vnpay")]
        public async Task<object> VnPayCheckout(string idTourBooking)
        {
            var url = await _vnPayRes.CreatePaymentUrl(idTourBooking, HttpContext);
            return new
            {
                status = 1,
                url = url,
            };
        }
        [HttpGet]
        [AllowAnonymous]
        [Route("callback-vnpay")]
        public async Task<object> PaymentCallback(string idTourBooking)
        {
            var response = await _vnPayRes.PaymentExecute(Request.Query  , idTourBooking);
            return Redirect(response.UrlReturnBill);
        }

    }
}
