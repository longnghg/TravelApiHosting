using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using Travel.Context.Models;
using Travel.Context.Models.Travel;
using Travel.Data.Interfaces;
using Travel.Shared.Ultilities;
using Travel.Shared.ViewModels;
using Travel.Shared.ViewModels.Travel;
using TravelApi.Helpers;
using TravelApi.Hubs;
using TravelApi.Hubs.HubServices;

namespace TravelApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {

        private IEmployee employee;
        private Notification message;
        private Response res;

        public EmployeeController(IEmployee _employee)
        {
            employee = _employee;
            res = new Response();
        }

        [HttpGet]
        //[ClaimRequirement(ClaimTypes.Name, "Admin")]
        [Authorize]
        [Route("list-employee")]
        public object GetsEmployee(bool isDelete)
        {
            var userId1 = this.User.FindFirstValue(ClaimTypes.Role);
            //var userId = this.User.Claims.Where(x=> x.Type == ClaimTypes.NameIdentifier);
            //var userId1 = this.User.FindFirstValue(ClaimTypes.Role);
            res = employee.GetsEmployee(isDelete);
            return Ok(res);
        }

        [HttpPost]
        [Authorize]
        [Route("search-employee")]
        public object SearchEmployee([FromBody] JObject frmData)
        {
            res = employee.SearchEmployee(frmData);
            return Ok(res);
        }

        [HttpPost]
        [Authorize]
        [Route("create-employee")]
        public object CreateEmployee(IFormCollection frmdata, IFormFile file)
        {
            var result = employee.CheckBeforeSave(frmdata, file, ref message, false);
            if (message == null)
            {
                var createObj = JsonSerializer.Deserialize<CreateEmployeeViewModel>(result);
                res = employee.CreateEmployee(createObj);
            }
            else
            {
                res.Notification = message;
            }
            return Ok(res);
        }



        [HttpPut]
        [Authorize]
        [Route("update-employee")]
        public object UpdateEmployee(IFormCollection frmdata, IFormFile file, Guid idEmployee)
        {
            message = null;
            var result = employee.CheckBeforeSave(frmdata, file, ref message, true);
            if (message == null)
            {
                var updateObj = JsonSerializer.Deserialize<UpdateEmployeeViewModel>(result);
                res = employee.UpdateEmployee(updateObj);
            }
            else
            {
                res.Notification = message;
            }
            return Ok(res);
        }
        [HttpDelete]
        [Authorize]
        [Route("delete-employee")]
        public object DeleteEmployee(Guid idEmployee)
        {
            res = employee.DeleteEmployee(idEmployee);
            return Ok(res);
        }

        [HttpPut]
        [AllowAnonymous]
        [Route("restore-employee")]
        public object RestoreEmployee(Guid idEmployee)
        {
            res = employee.RestoreEmployee(idEmployee);
            return Ok(res);
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("detail-employee")]
        public object GetEmployee(Guid idEmployee)
        {
            res = employee.GetEmployee(idEmployee);
            return Ok(res);
        }


        [HttpGet]
        [AllowAnonymous]
        [Route("statistic-employee")]
        public object StatisticEmployee(bool isDelete , bool isActive)
        {
            res = employee.StatisticEmployee();
            return Ok(res);
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("send-otp-emp")]
        public async Task<object> SendOTP(string email)
        {
            res = await employee.SendOTP(email);
            return Ok(res);
        }
    }
}
