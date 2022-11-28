using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
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
    public class CarController : ControllerBase
    {
        private ICars _car;
        private Notification message;
        private Response res;
        public CarController(ICars car)
        {
            _car = car;
            res = new Response();
        }

        [HttpGet]
        [Authorize]
        [Route("list-car")]
        public object Gets(bool isDelete)
        {
            res = _car.Gets(isDelete);
            return Ok(res);
        }
        [HttpGet]
        [Authorize]
        [Route("list-selectbox-car")]
        public object GetsSelectBoxCar(long fromDate, long toDate)
        {
            res = _car.GetsSelectBoxCar(fromDate, toDate);
            return Ok(res);
        }
        [HttpGet]
        [Authorize]
        [Route("list-selectbox-car-update")]
        public object GetsSelectBoxCarpdate(long fromDate, long toDate, string idSchedule)
        {
            res = _car.GetsSelectBoxCarUpdate(fromDate, toDate,idSchedule);
            return Ok(res);
        }
        [HttpGet]
        [Authorize]
        [Route("statistic-car")]
        public object StatisticCar()
        {
            res = _car.StatisticCar();
            return Ok(res);
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("create-car")]
        public object Create([FromBody] JObject frmData)
        {

            message = null;
            var result = _car.CheckBeforeSave(frmData, ref message, false);
            if (message == null)
            {
                var createObj = JsonSerializer.Deserialize<CreateCarViewModel>(result);
                res = _car.Create(createObj);
            }
            else
            {
                res.Notification = message;
            }


            return Ok(res);
        }

        [HttpPut]
        [Authorize]
        [Route("update-car")]
        public object UpdateRestaurant([FromBody] JObject frmData, Guid idCar)
        {
            message = null;
            var result = _car.CheckBeforeSave(frmData, ref message, true);
            if (message == null)
            {
                var updateObj = JsonSerializer.Deserialize<UpdateCarViewModel>(result);
                res = _car.UpdateCar(updateObj);
            }
            else
            {
                res.Notification = message;
            }
            return Ok(res);
        }

        [HttpDelete]
        [Authorize]
        [Route("delete-car")]
        public object DeleteCar(Guid idCar, Guid idUser)
        {
            res = _car.DeleteCar(idCar, idUser);
            return Ok(res);
        }

        [HttpPut]
        [Authorize]
        [Route("restore-car")]
        public object RestoreCar(Guid idCar)
        {
            res = _car.RestoreCar(idCar);
            return Ok(res);
        }

        [HttpPost]
        [Authorize]
        [Route("search-car")]
        public object SearchCar([FromBody] JObject frmData)
        {
            res = _car.SearchCar(frmData);
            return Ok(res);
        }
    }
}
