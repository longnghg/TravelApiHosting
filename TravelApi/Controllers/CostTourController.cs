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
using Travel.Shared.ViewModels.Travel.CostTourVM;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TravelApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CostTourController : ControllerBase
    {
        private readonly ICostTour _costTourRes;
        private Notification message;
        private Response res;
        public CostTourController(ICostTour costTourRes , ILog log)
        {
            _costTourRes = costTourRes;
            res = new Response();

        }

        [NonAction]
        private Claim GetEmailUserLogin()
        {
            return (User.Identity as ClaimsIdentity).Claims.Where(c => c.Type == ClaimTypes.Email).FirstOrDefault();
        }

        [HttpGet]
        [Authorize]
        [Route("list-cost-tour")]
        public object Get()
        {
            res = _costTourRes.Get();
            return Ok(res);
        }
        [HttpGet]
        [Authorize]
        [Route("detail-cost-tour-idSchedule")] 
        public object GetGetCostByIdTourDetail(string idSchedule)
        {
            res = _costTourRes.GetCostByIdSchedule(idSchedule);
            return Ok(res);
        }


        [HttpPost]
        [Authorize]
        [Route("create-cost-tour")]
        public object Create([FromBody] JObject frmData)
        {
            message = null;
            var result = _costTourRes.CheckBeforSave(frmData, ref message, false);
            if (message == null)
            {
                var createObj = JsonSerializer.Deserialize<CreateCostViewModel>(result);
                var emailUser = GetEmailUserLogin().Value;
                res = _costTourRes.Create(createObj, emailUser);
            }
            else
            {
                res.Notification = message;
            }
            return Ok(res);
        }

        [HttpPut]
        [Authorize]
        [Route("update-cost-tour")]
        public object Update([FromBody] JObject frmData, string IdSchedule)
        {
            message = null;
            var result = _costTourRes.CheckBeforSave(frmData, ref message, true);
            if (message == null)
            {
                var updateObj = JsonSerializer.Deserialize<UpdateCostViewModel>(result);
                var emailUser = GetEmailUserLogin().Value;
                res = _costTourRes.Update(updateObj, emailUser);
            }
            else
            {
                res.Notification = message;
            }
            return Ok(res);
        }

    
    }
}
