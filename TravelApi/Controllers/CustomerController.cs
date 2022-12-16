using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using Travel.Context.Models;
using Travel.Data.Interfaces;
using Travel.Shared.ViewModels;
using Travel.Shared.ViewModels.Travel.CustomerVM;

namespace TravelApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private ICustomer customer;
        private Notification message;
        private Response res;
        private readonly ILog _log;
        public CustomerController(ICustomer _customer)
        {
            customer = _customer;
            res = new Response();
        }

        [NonAction]
        private Claim GetEmailUserLogin()
        {
            return (User.Identity as ClaimsIdentity).Claims.Where(c => c.Type == ClaimTypes.Email).FirstOrDefault();
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("create-customer")]
        public object Create([FromBody] JObject frmdata)
        {
            message = null;
            var result = customer.CheckBeforeSave(frmdata, ref message, false);
            if (message == null)
            {
                var createObj = JsonSerializer.Deserialize<CreateCustomerViewModel>(result);
                res = customer.Create(createObj);

            }
            else
            {
                res.Notification = message;
            }
            return Ok(res);
        }

        [HttpGet]
        [Authorize]
        [Route("list-customer")]
        public object GetCustomer()
        {
            res = customer.Gets();
            return Ok(res);
        }
        [HttpGet]
        [Authorize]
        [Route("list-history-booking-bycustomer")]
        public object GetHistoryByIdCustomer(Guid idCustomer)
        {
            res = customer.GetsHistory(idCustomer);
            return Ok(res);
        }
        [HttpGet]
        [AllowAnonymous]
        [Route("send-otp")]
        public async Task<object> SendOTP(string email)
        {
            res = await customer.SendOTP(email);
            return Ok(res);
        }

        [HttpGet]
        [Authorize]
        [Route("detail-customer")]
        public object GetCustomer(Guid idCustomer)
        {
            res = customer.GetCustomer(idCustomer);
            //_messageHub.Clients.All.Init();
            return Ok(res);
        }

        [HttpPut]
        [Authorize]
        [Route("update-customer")]
        public async Task<object> UpdateCustomer([FromBody] JObject frmdata, Guid idCustomer)
        {
            message = null;
            //frmdata.Add(new JProperty("idCustomer", idCustomer));
            var result = customer.CheckBeforeSave(frmdata, ref message, true);
            if (message == null)
            {
                var updateObj = JsonSerializer.Deserialize<UpdateCustomerViewModel>(result);
                
                res = await customer.UpdateCustomer(updateObj);
                //_messageHub.Clients.All.Init();
            }
            else
            {
                res.Notification = message;
            }
            return Ok(res);
        }
        [HttpPost]
        [AllowAnonymous]
        [Route("cus-vote-rating")]
        public async Task<object> CustomerVoteRateting(string idTour, int rating)
        {
            res = await customer.CustomerSendRate(idTour, rating);
            return Ok(res);
        }

        [HttpPost]
        [Authorize]
        [Route("search-customer")]
        public object SearchCustomer([FromBody] JObject frmData)
        {
            res = customer.Search(frmData);
            return Ok(res);
        }

        [HttpPut]
        [Authorize]
        [Route("block-customer")]
        public async Task<object> BlockCustomer(Guid idCustomer, bool isBlock)
        {
            res = await customer.UpdateBlockCustomer(idCustomer, isBlock);
            return Ok(res);
        }
    }
}
