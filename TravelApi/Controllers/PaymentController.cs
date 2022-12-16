using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using Travel.Data.Interfaces;
using Travel.Shared.ViewModels;
using Travel.Shared.ViewModels.Travel;

namespace TravelApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : Controller
    {
        private IPayment pay;
        private Notification message;
        private Response res;
        private readonly ILog _log;
        public PaymentController(IPayment _pay)
        {
            pay = _pay;
            res = new Response();
        }
        [NonAction]
        private Claim GetEmailUserLogin()
        {
            return (User.Identity as ClaimsIdentity).Claims.Where(c => c.Type == ClaimTypes.Email).FirstOrDefault();
        }
        [HttpGet]
        [AllowAnonymous]
        [Route("list-payment")]
        public object GetPayment()
        {
            res = pay.Gets();
            return Ok(res);
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("create-payment")]
        public object Create([FromBody] JObject frmData)
        {

            message = null;
            var result = pay.CheckBeforSave(frmData, ref message, false);
            if (message == null)
            {
                var createObj = JsonSerializer.Deserialize<CreatePaymentViewModel>(result);            
                var emailUser = GetEmailUserLogin().Value;
                res = pay.Create(createObj, emailUser);
            }
            else
            {
                res.Notification = message;
            }
            return Ok(res);
        }
    }
}
