using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using Travel.Data.Interfaces;
using Travel.Data.Repositories;
using Travel.Shared.Ultilities;
using Travel.Shared.ViewModels;
using Travel.Shared.ViewModels.Travel;
using TravelApi.Helpers;
using TravelApi.Hubs;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TravelApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScheduleController : ControllerBase
    {
        private readonly ISchedule _schedule;
        private Notification message;
        private Response res;

        public ScheduleController(ISchedule schedule , ILog log)
        {
            _schedule = schedule;
            res = new Response();

        }

        [NonAction]
        private Claim GetEmailUserLogin()
        {
            return (User.Identity as ClaimsIdentity).Claims.Where(c => c.Type == ClaimTypes.Email).FirstOrDefault();
        }

        [HttpPost]
        [Authorize]
        [Route("create-schedule")]
        public object Create([FromBody] JObject frmData)
        {
            message = null;
            var result = _schedule.CheckBeforSave(frmData, ref message, false);
            if (message == null)
            {
                var createObj = JsonSerializer.Deserialize<CreateScheduleViewModel>(result);
          
                var emailUser = GetEmailUserLogin().Value;
                res = _schedule.Create(createObj, emailUser);
            }
            else
            {
                res.Notification = message;
            }
            return Ok(res);
        }

        [HttpPut]
        [Authorize]
        [Route("update-schedule")]
        public object Update([FromBody] JObject frmData, string idSchedule)
        {
            message = null;
            var result = _schedule.CheckBeforSave(frmData, ref message, true);
            if (message == null)
            {
                var updateObj = JsonSerializer.Deserialize<UpdateScheduleViewModel>(result);
             var emailUser = GetEmailUserLogin().Value;
                res = _schedule.Update(updateObj, emailUser);
            }
            else
            {
                res.Notification = message;
            }
            return Ok(res);
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("sia-sia")]
        public object Updatecâc(DateTime a, DateTime b)
        {
            var caon = Ultility.CountDay(a, b);

            var cut = 1;
            return 0;
        }

        [HttpDelete]
        [Authorize]
        [Route("delete-schedule")]
        public object DeleteTour(string idSchedule, Guid idUser)
        {
            var emailUser = GetEmailUserLogin().Value;
            res = _schedule.Delete(idSchedule, idUser, emailUser);
           
            return Ok(res);
        }
        [HttpPut]
        [Authorize]
        [Route("restore-schedule")]
        public object RestoreSchedule(string idSchedule, Guid idUser)
        {
            
            var emailUser = GetEmailUserLogin().Value;
            res = _schedule.RestoreShedule(idSchedule, idUser, emailUser);
            return Ok(res);
        }

        [HttpPut]
        [Authorize]
        [Route("approve-schedule")]
        public object ApproveSchedule(string idSchedule)
        {
            res = _schedule.Approve(idSchedule);
            return Ok(res);
        }
        [HttpPut]
        [Authorize]
        [Route("refused-schedule")]
        public object RefusedSchedule(string idSchedule)
        {
            res = _schedule.Refused(idSchedule);
            return Ok(res);
        }
        [HttpGet]
        [AllowAnonymous]
        [Route("list-schedule")]
        public object Gets()
        {
            res = _schedule.Gets();
            return Ok(res);
        }
        [HttpGet]
        [AllowAnonymous]
        [Route("detail-schedule")]
        public async Task<object> Get(string idSchedule)
        {
            res = await _schedule.Get(idSchedule);
            return Ok(res);
        }
        [HttpPut]
        [ClaimRequirement(ClaimTypes.Name, "Admin")]
        [Route("update-promotion")]
        public object UpdatePromotion(string idSchedule, int idPromotion)
        {
            var emailUser = GetEmailUserLogin().Value;
            res = _schedule.UpdatePromotion(idSchedule, idPromotion, emailUser);
           
            return Ok(res);
        }
        [HttpPost]
        [ClaimRequirement(ClaimTypes.Name, "Admin")]
        [Route("automatic-promotion-for-schedule")]
        public async Task<object> AutomaticUpdatePromotionForSchedule()
        {
            res = await _schedule.AutomaticUpdatePromotionForSchedule();
            return Ok(res);
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("list-schedule-idtour")]
        public object GetsSchedulebyIdTour(string idtour, bool isDelete)
        {
            res = _schedule.GetsSchedulebyIdTour(idtour, isDelete);
            return Ok(res);
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("list-schedule-idtour-waiting")]
        public object GetsSchedulebyIdTourWaiting(string idtour, Guid idUser,int pageIndex, int pageSize)
        {
            res = _schedule.GetSchedulebyIdTourWaiting(idtour, idUser,pageIndex, pageSize);
            return Ok(res);
        }



        [HttpGet]
        [AllowAnonymous]
        [Route("cus-search-schedule")]
        public async Task<object> SearchSchedule(string from = null, string to = null,DateTime? departureDate = null, DateTime? returnDate = null)
        {
            res = await _schedule.SearchTour(from,to,departureDate,returnDate);
            return Ok(res);
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("cus-list-schedule-idtour")]
        public object CusGetsSchedulebyIdTour(string idtour)
        {
            res = _schedule.CusGetsSchedulebyIdTour(idtour);
            return Ok(res);
        }
        [HttpGet]
        [AllowAnonymous]
        [Route("cus-list-schedule")]
        public async Task<object> GetsSchedule(int pageIndex, int pageSize)
        {
            res = await _schedule.GetsSchedule(pageIndex, pageSize);
            return Ok(res);
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("list-schedule-promotion")]
        public async Task<object> GetsSchedulePromotion(int pageIndex, int pageSize)
        {
            res = await _schedule.GetsSchedulePromotion(pageIndex, pageSize);
            return Ok(res);
        }
        [HttpGet]
        [AllowAnonymous]
        [Route("list-schedule-flash-sale")]
        public async Task<object> GetsScheduleFlashSale(int pageIndex, int pageSize)
        {
            res = await _schedule.GetsScheduleFlashSale(pageIndex, pageSize);
            return Ok(res);
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("list-schedule-relate")]
        public async Task<object> GetsScheduleRelate(string idSchedule, int pageIndex, int pageSize)
        {
            res = await _schedule.GetsRelatedSchedule(idSchedule, pageIndex, pageSize);
            return Ok(res);
        }

        [HttpPost]
        [Authorize]
        [Route("search-schedule")]
        public object SearchSchedule([FromBody] JObject frmData, string idTour)
        {
            res = _schedule.SearchSchedule(frmData, idTour);
            return Ok(res);
        }

        [HttpPost]
        [Authorize]
        [Route("search-schedule-waiting")]
        public object SearchScheduleWaiting([FromBody] JObject frmData, string idTour)
        {
            res = _schedule.SearchScheduleWaiting(frmData, idTour);
            return Ok(res);
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("cus-search-schedule-filter")]
        public async Task<object> SearchSchedule([FromBody] JObject frmData)
        {
            res = await _schedule.SearchTourFilter(frmData);
          
            return Ok(res);
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("update-promotion-last-hour")]
        public object UpdatePromotionTourLastHour(DateTime date)
        {
            res = _schedule.UpdatePromotionTourLastHour(date);
            return Ok(res);
        }
    }
}
