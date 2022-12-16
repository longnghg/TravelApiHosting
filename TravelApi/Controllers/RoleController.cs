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
using Travel.Shared.ViewModels;
using Travel.Shared.ViewModels.Travel;
using TravelApi.Hubs;

namespace TravelApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : Controller
    {
        private IRole role;
        private Notification message;
        private Response res;

        public RoleController(IRole _role ,ILog log)
        {
            role = _role;
            res = new Response();
        }

        [NonAction]
        private Claim GetEmailUserLogin()
        {
            return (User.Identity as ClaimsIdentity).Claims.Where(c => c.Type == ClaimTypes.Email).FirstOrDefault();
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("list-role")]
        public object GetsRole(bool isDelete)
        {
            res = role.GetsRole(isDelete);
            return Ok(res);
        }

        [HttpPost]
        [Authorize]
        [Route("search-role")]
        public object SearchRole([FromBody] JObject frmData)
        {
            res = role.SearchRole(frmData);
            return Ok(res);
        }


        [HttpPost]
        [Authorize]
        [Route("create-role")]
        public object CreateRole([FromBody] JObject frmData)
        {
            message = null;
            var result = role.CheckBeforSave(frmData, ref message, false);
            if (message == null)
            {
                var createObj = JsonSerializer.Deserialize<CreateRoleViewModel>(result);

                var emailUser =GetEmailUserLogin().Value;
                res = role.CreateRole(createObj, emailUser);
            }
            else
            {
                res.Notification = message;
            }
            
            return Ok(res);
        }

        [HttpPut]
        [Authorize]
        [Route("update-role")]
        public object UpdateRole([FromBody] JObject frmData, int idRole)
        {

            message = null;
            var result = role.CheckBeforSave(frmData, ref message, true);
            if (message == null)
            {
                var updateObj = JsonSerializer.Deserialize<UpdateRoleViewModel>(result);
                var emailUser = GetEmailUserLogin().Value;
                res = role.UpdateRole(updateObj, emailUser);
            }
            else
            {
                res.Notification = message;
            }

            return Ok(res);
        }

        [HttpPut]
        [Authorize]
        [Route("restore-role")]
        public object RestoreRole(int idRole)
        {
       
            var emailUser = GetEmailUserLogin().Value;
            res = role.RestoreRole(idRole, emailUser);
            return Ok(res);
        }
        [HttpDelete]
        [Authorize]
        [Route("delete-role")]
        public object DeleteRole(int idRole)
        {
   
            var emailUser = GetEmailUserLogin().Value;
            res = role.DeleteRole(idRole, emailUser);
            return Ok(res);
        }
    }
}
