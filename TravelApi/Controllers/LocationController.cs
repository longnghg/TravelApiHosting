using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json.Linq;
using PrUtility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using Travel.Data.Interfaces;
using Travel.Shared.ViewModels;
using Travel.Shared.ViewModels.Travel;
using Travel.Shared.ViewModels.Travel.DistrictVM;
using Travel.Shared.ViewModels.Travel.WardVM;
using TravelApi.Hubs;

namespace TravelApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LocationController : ControllerBase
    {
        private ILocation location;
        private Notification message;
        private Response res;
        private readonly ILog _log;

        public LocationController(ILocation _location)
        {
            location = _location;
            res = new Response();
        }
        [NonAction]
        private Claim GetEmailUserLogin()
        {
            return (User.Identity as ClaimsIdentity).Claims.Where(c => c.Type == ClaimTypes.Email).FirstOrDefault();
        }
        [HttpGet]
        [AllowAnonymous]
        [Route("list-province")]
        public  object GetsProvince()
        {
       
            res = location.GetsProvince();
            return Ok(res);
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("list-district")]
        public object GetsDistrict()
        {
            res = location.GetsDistrict();
            return Ok(res);
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("list-ward")]
        public object GetsWard()
        {
            res = location.GetsWard();
            return Ok(res);
        }

        [HttpPost]
        [Authorize]
        [Route("search-province")]
        public object SearchProvince([FromBody] JObject frmData)
        {
            res = location.SearchProvince(frmData);
            return Ok(res);
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("search-district")]
        public object SearchDistrict([FromBody] JObject frmData)
        {
            res = location.SearchDistrict(frmData);
            return Ok(res);
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("search-ward")]
        public object SearchWard([FromBody] JObject frmData)
        {
            res = location.SearchWard(frmData);
            return Ok(res);
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("create-province")]
        public object CreateProvince([FromBody] JObject frmData)
        {
            message = null;
            var result = location.CheckBeforeSaveProvince(frmData, ref message, false);
            if (message == null)
            {
                var createObj = JsonSerializer.Deserialize<CreateProvinceViewModel>(result);
                var emailUser = GetEmailUserLogin().Value;
                res = location.CreateProvince(createObj, emailUser);
            }
            else
            {
                res.Notification = message;
            }
            return Ok(res);
        }

        [HttpPost]
        [Authorize]
        [Route("create-district")]
        public object CreateDistrict([FromBody] JObject frmData)
        {
            message = null;
            var result = location.CheckBeforeSaveDistrict(frmData, ref message, false);
            if (message == null)
            {
                var createObj = JsonSerializer.Deserialize<CreateDistrictViewModel>(result);
                var emailUser = GetEmailUserLogin().Value;
                res = location.CreateDistrict(createObj, emailUser);
            }
            else
            {
                res.Notification = message;
            }
            return Ok(res);
        }

        [HttpPost]
        [Authorize]
        [Route("create-ward")]
        public object CreateWard([FromBody] JObject frmData)
        {
            message = null;
            var result = location.CheckBeforeSaveWard(frmData, ref message, false);
            if (message == null)
            {
                var createObj = JsonSerializer.Deserialize<CreateWardViewModel>(result);
                var emailUser = GetEmailUserLogin().Value;
                res = location.CreateWard(createObj, emailUser);
            }
            else
            {
                res.Notification = message;
            }
            return Ok(res);
        }

        [HttpPut]
        [Authorize]
        [Route("update-province")]
        public object UpdateProvince([FromBody] JObject frmData, Guid idProvince)
        {
            message = null;
            var result = location.CheckBeforeSaveProvince(frmData, ref message, true);
            if (message == null)
            {
                var updateObj = JsonSerializer.Deserialize<UpdateProvinceViewModel>(result);
 
                var emailUser = GetEmailUserLogin().Value;
                res = location.UpdateProvince(updateObj, emailUser);
            }
            else
            {
                res.Notification = message;
            }
            return Ok(res);
        }

        [HttpPut]
        [Authorize]
        [Route("update-district")]
        public object UpdateDistrict([FromBody] JObject frmData, Guid idDistrict)
        {
            message = null;
            var result = location.CheckBeforeSaveDistrict(frmData, ref message, true);
            if (message == null)
            {
                var updateObj = JsonSerializer.Deserialize<UpdateDistrictViewModel>(result);
                var emailUser = GetEmailUserLogin().Value;
                res = location.UpdateDistrict(updateObj, emailUser);

            }
            else
            {
                res.Notification = message;
            }
            return Ok(res);
        }

        [HttpPut]
        [Authorize]
        [Route("update-ward")]
        public object UpdateWard([FromBody] JObject frmData, Guid idWard)
        {
            message = null;
            var result = location.CheckBeforeSaveWard(frmData, ref message, true);
            if (message == null)
            {
                var updateObj = JsonSerializer.Deserialize<UpdateWardViewModel>(result);
                var emailUser = GetEmailUserLogin().Value;
                res = location.UpdateWard(updateObj, emailUser);

            }
            else
            {
                res.Notification = message;
            }
            return Ok(res);
        }

        [HttpDelete]
        [Authorize]
        [Route("delete-province")]
        public object DeleteProvince(Guid idProvince)
        {
            var emailUser = GetEmailUserLogin().Value;
            res = location.DeleteProvince(idProvince, emailUser);          
             
            return Ok(res);
        }

        [HttpDelete]
        [Authorize]
        [Route("delete-district")]
        public object DeleteDistrict(Guid idDistrict)
        {
            var emailUser = GetEmailUserLogin().Value;
            res = location.DeleteDistrict(idDistrict, emailUser);
           
            return Ok(res);
        }

        [HttpDelete]
        [Authorize]
        [Route("delete-ward")]
        public object DeleteWard(Guid idWard)
        {
            var emailUser = GetEmailUserLogin().Value;
            res = location.DeleteWard(idWard, emailUser);
        
            return Ok(res);
        }
    }
}
