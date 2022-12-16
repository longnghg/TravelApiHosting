using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using Travel.Context.Models;
using Travel.Data.Interfaces;
using Travel.Data.Repositories;
using Travel.Shared.ViewModels;
using Travel.Shared.ViewModels.Travel;
using static Travel.Shared.ViewModels.Travel.CreateTimeLineViewModel;

namespace TravelApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TimelineController : ControllerBase
    {
        private readonly ITimeLine _timelineRes;
        private Notification message;
        private Response res;
        
        public TimelineController(ITimeLine timelineRes , ILog log)
        {
            _timelineRes = timelineRes;
            res = new Response();
        }
        [NonAction]
        private Claim GetEmailUserLogin()
        {
            return (User.Identity as ClaimsIdentity).Claims.Where(c => c.Type == ClaimTypes.Email).FirstOrDefault();
        }
        [HttpPost]
        [Authorize]
        [Route("create-timeline")]
        public object Create(ICollection<CreateTimeLineViewModel> timelinee)
        {
            message = null;
            //var result = _timelineRes.CheckBeforSave(frmData, ref message, false);
            if (message == null)
            {
                var createObj = timelinee;
                var emailUser = GetEmailUserLogin().Value;
                res = _timelineRes.Create(createObj, emailUser);
             
            }
            else
            {
                res.Notification = message;
            }
            return Ok(res);
        }

        [HttpPost]
        [Authorize]
        [Route("update-timeline")]
        public object update(ICollection<UpdateTimeLineViewModel> timeline)
        {
            message = null;
            //var result = _timelineRes.CheckBeforSave(frmData, ref message, false);
            if (message == null)
            {
                var updateObj = timeline;
                
                var emailUser = GetEmailUserLogin().Value;
                res = _timelineRes.Update(updateObj, emailUser);
            }
            else
            {
                res.Notification = message;
            }
            return Ok(res);
        }

        [HttpPost]
        [Authorize]
        [Route("delete-timeline")]
        public object Delete(ICollection<Timeline> timeline)
        {
          
            var emailUser = GetEmailUserLogin().Value;
            res = _timelineRes.Delete(timeline, emailUser);
            return Ok(res);
        }

        [HttpGet]
        [Authorize]
        [Route("list-timeline")]
        public object GetTimeline()
        {
            res = _timelineRes.Get();
            return Ok(res);
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("list-timeline-idSchedule")]
        public object GetGetCostByIdTourDetail(string idSchedule)
        {
            res = _timelineRes.GetTimelineByIdSchedule(idSchedule);
            return Ok(res);
        }
    }
}
