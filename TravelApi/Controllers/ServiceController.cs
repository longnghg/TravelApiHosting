using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Travel.Data.Interfaces;
using Travel.Shared.ViewModels;
using Travel.Shared.ViewModels.Travel.ContractVM;

namespace TravelApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceController : ControllerBase
    {
        private readonly IService _serviceRes;
        private Notification message;
        private Response res;

        public ServiceController(IService service)
        {
            _serviceRes = service;
            res = new Response();
        }
        #region hotel
        [HttpPost]
        [Authorize]
        [Route("create-hotel")]
        public object CreateHotel([FromBody] JObject frmData)
        {
            message = null;
            var result = _serviceRes.CheckBeforSave(frmData, ref message, Travel.Shared.Ultilities.Enums.TypeService.Hotel, false);
            if (message == null)
            {
                var createObj = JsonSerializer.Deserialize<CreateHotelViewModel>(result);
                res = _serviceRes.CreateHotel(createObj);
            }
            else
            {
                res.Notification = message;
            }
            return Ok(res);
        }
        [HttpGet()]
        [Authorize]
        [Route("list-hotel")]
        public object GetHotel(bool isDelete)
        {
            res = _serviceRes.GetsHotel(isDelete);
            return Ok(res);
        }
        [HttpGet()]
        [Authorize]
        [Route("list-hotel-waiting")]
        public object GetHotelWaiting(Guid idUser, int pageIndex, int pageSize)
        {
            res = _serviceRes.GetsWaitingHotel(idUser,pageIndex,pageSize);
            return Ok(res);
        }

        [HttpPut]
        [Authorize]
        [Route("update-hotel")]
        public object UpdateHotel([FromBody] JObject frmData, Guid idHotel)
        {
            message = null;
            var result = _serviceRes.CheckBeforSave(frmData, ref message, Travel.Shared.Ultilities.Enums.TypeService.Hotel, true);
            if (message == null)
            {
                var updateObj = JsonSerializer.Deserialize<UpdateHotelViewModel>(result);
                res = _serviceRes.UpdateHotel(updateObj);
            }
            else
            {
                res.Notification = message;
            }
            return Ok(res);
        }
        [HttpPut]
        [Authorize]
        [Route("approve-hotel")]
        public object ApproveHotel(Guid idHotel)
        {
            res = _serviceRes.ApproveHotel(idHotel);
            return Ok(res);
        }

        [HttpPut]
        [Authorize]
        [Route("refuse-hotel")]
        public object RefuseHotel(Guid idHotel)
        {
            res = _serviceRes.RefusedHotel(idHotel);
            return Ok(res);
        }
        [HttpDelete]
        [Authorize]
        [Route("delete-hotel")]
        public object DeleteHotel(Guid idHotel, Guid idUser)
        {
            res = _serviceRes.DeleteHotel(idHotel, idUser);
            return Ok(res);
        }
        [HttpPut]
        [Authorize]
        [Route("restore-hotel")]
        public object RestoreHotel(Guid idHotel, Guid idUser)
        {
            res = _serviceRes.RestoreHotel(idHotel, idUser);
            return Ok(res);
        }
        #endregion

        #region restaurant
        [HttpGet()]
        [Authorize]
        [Route("list-restaurant-waiting")]
        public object GetWaitingRestaurant(Guid idUser, int pageIndex, int pageSize)
        {
            res = _serviceRes.GetsWaitingRestaurant(idUser,pageIndex,pageSize);
            return Ok(res);
        }

        [HttpGet()]
        [Authorize]
        [Route("list-restaurant")]
        public object GetRestaurant(bool isDelete)
        {
            res = _serviceRes.GetsRestaurant(isDelete);
            return Ok(res);
        }
        [HttpPost]
        [Authorize]
        [Route("create-restaurant")]
        public object CreateRestaurant([FromBody] JObject frmData)
        {
            message = null;
            var result = _serviceRes.CheckBeforSave(frmData, ref message, Travel.Shared.Ultilities.Enums.TypeService.Restaurant, false);
            if (message == null)
            {
                var createObj = JsonSerializer.Deserialize<CreateRestaurantViewModel>(result);
                res = _serviceRes.CreateRestaurant(createObj);
            }
            else
            {
                res.Notification = message;
            }
            return Ok(res);
        }

        [HttpPut]
        [Authorize]
        [Route("approve-restaurant")]
        public object ApproveRestaurant(Guid idRestaurant)
        {
            res = _serviceRes.ApproveRestaurant(idRestaurant);
            return Ok(res);
        }

        [HttpPut]
        [Authorize]
        [Route("refuse-restaurant")]
        public object RefuseRestaurant(Guid idRestaurant)
        {
            res = _serviceRes.RefusedRestaurant(idRestaurant);
            return Ok(res);
        }
        [HttpDelete]
        [Authorize]
        [Route("delete-restaurant")]
        public object DeleteRestaurant(Guid idRestaurant, Guid idUser)
        {
            res = _serviceRes.DeleteRestaurant(idRestaurant, idUser);
            return Ok(res);
        }

        [HttpPut]
        [Authorize]
        [Route("update-restaurant")]
        public object UpdateRestaurant([FromBody] JObject frmData, Guid idRestaurant)
        {

            message = null;
            var result = _serviceRes.CheckBeforSave(frmData, ref message, Travel.Shared.Ultilities.Enums.TypeService.Restaurant, true);
            if (message == null)
            {
                var updateObj = JsonSerializer.Deserialize<UpdateRestaurantViewModel>(result);
                res = _serviceRes.UpdateRestaurant(updateObj);
            }
            else
            {
                res.Notification = message;
            }
            return Ok(res);
        }
        [HttpPut]
        [Authorize]
        [Route("restore-restaurant")]
        public object RestoreRestaurant(Guid idRestaurant, Guid idUser)
        {
            res = _serviceRes.RestoreRestaurant(idRestaurant, idUser);
            return Ok(res);
        }
        #endregion

        #region place
        [HttpGet()]
        [Authorize]
        [Route("list-place-waiting")]
        public object GetPlaceWaiting(Guid idUser, int pageIndex, int pageSize)
        {
            res = _serviceRes.GetsWaitingPlace(idUser,  pageIndex,  pageSize);
            return Ok(res);
        }

        [HttpGet()]
        [AllowAnonymous]
        [Route("list-place")]
        public object GetPlace(bool isDelete)
        {
            res = _serviceRes.GetsPlace(isDelete);
            return Ok(res);
        }
        [HttpPost]
        [Authorize]
        [Route("create-place")]
        public object CreatePlace([FromBody] JObject frmData)
        {

            message = null;
            var result = _serviceRes.CheckBeforSave(frmData, ref message, Travel.Shared.Ultilities.Enums.TypeService.Place, false);
            if (message == null)
            {
                var createObj = JsonSerializer.Deserialize<CreatePlaceViewModel>(result);
                res = _serviceRes.CreatePlace(createObj);
            }
            else
            {
                res.Notification = message;
            }
            return Ok(res);
        }
        [HttpPut]
        [Authorize]
        [Route("update-place")]
        public object UpdatePlace([FromBody] JObject frmData, Guid idPlace)
        {
            message = null;
            var result = _serviceRes.CheckBeforSave(frmData, ref message, Travel.Shared.Ultilities.Enums.TypeService.Place, true);
            if (message == null)
            {
                var updateObj = JsonSerializer.Deserialize<UpdatePlaceViewModel>(result);
                res = _serviceRes.UpdatePlace(updateObj);
            }
            else
            {
                res.Notification = message;
            }
            return Ok(res);
        }
        [HttpPut]
        [Authorize]
        [Route("approve-place")]
        public object ApprovePlace(Guid idPlace)
        {
            res = _serviceRes.ApprovePlace(idPlace);
            return Ok(res);
        }

        [HttpPut]
        [Authorize]
        [Route("refuse-place")]
        public object RefusePlace(Guid idPlace)
        {
            res = _serviceRes.RefusedPlace(idPlace);
            return Ok(res);
        }
        [HttpDelete]
        [Authorize]
        [Route("delete-place")]
        public object DeletePlace(Guid idPlace, Guid idUser)
        {
            res = _serviceRes.DeletePlace(idPlace, idUser);
            return Ok(res); 
        }

        [HttpPut]
        [Authorize]
        [Route("restore-place")]
        public object RestorePlace(Guid idPlace, Guid idUser)
        {
            res = _serviceRes.RestorePlace(idPlace, idUser);
            return Ok(res);
        }
        #endregion

        [HttpPost]
        [Authorize]
        [Route("search-hotel")]
        public object SearchHotel([FromBody] JObject frmData)
        {
            res = _serviceRes.SearchHotel(frmData);
            return Ok(res);
        }

        [HttpPost]
        [Authorize]
        [Route("search-hotel-waiting")]
        public object SearchHotelWaiting([FromBody] JObject frmData)
        {
            res = _serviceRes.SearchHotelWaiting(frmData);
            return Ok(res);
        }

        [HttpPost]
        [Authorize]
        [Route("search-place")]
        public object SearchPlace([FromBody] JObject frmData)
        {
            res = _serviceRes.SearchPlace(frmData);
            return Ok(res);
        }
        [HttpPost]
        [Authorize]
        [Route("search-Restaurant")]
        public object SearchRestaurant([FromBody] JObject frmData)
        {
            res = _serviceRes.SearchRestaurant(frmData);
            return Ok(res); 
        }
        [HttpPost]
        [Authorize]
        [Route("search-place-waiting")]
        public object SearchPlaceWaiting([FromBody] JObject frmData)
        {
            res = _serviceRes.SearchPlaceWaiting(frmData);
            return Ok(res);
        }

        [HttpPost]
        [Authorize]
        [Route("search-restaurant-waiting")]
        public object SearchRestaurantWaiting([FromBody] JObject frmData)
        {
            res = _serviceRes.SearchRestaurantWaiting(frmData);
            return Ok(res);
        }
    }
}
