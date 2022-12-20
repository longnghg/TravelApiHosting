using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using Travel.Context.Models;
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

        public ServiceController(IService service, ILog log)
        {
            _serviceRes = service;
            res = new Response();
        }
        [NonAction]
        private Claim GetEmailUserLogin()
        {
            return (User.Identity as ClaimsIdentity).Claims.Where(c => c.Type == ClaimTypes.Email).FirstOrDefault();
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
                
                var emailUser = GetEmailUserLogin().Value;
                res = _serviceRes.CreateHotel(createObj, emailUser);
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
                
                var emailUser = GetEmailUserLogin().Value;
                res = _serviceRes.UpdateHotel(updateObj, emailUser);
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
          
            var emailUser = GetEmailUserLogin().Value;
            res = _serviceRes.DeleteHotel(idHotel, idUser, emailUser);
            return Ok(res);
        }
        [HttpPut]
        [Authorize]
        [Route("restore-hotel")]
        public object RestoreHotel(Guid idHotel, Guid idUser)
        {
          
            var emailUser = GetEmailUserLogin().Value;
            res = _serviceRes.RestoreHotel(idHotel, idUser, emailUser);
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
                var emailUser = GetEmailUserLogin().Value;
                res = _serviceRes.CreateRestaurant(createObj, emailUser);
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
            var emailUser = GetEmailUserLogin().Value;
             res = _serviceRes.DeleteRestaurant(idRestaurant, idUser, emailUser);
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
                  var emailUser = GetEmailUserLogin().Value;
                res = _serviceRes.UpdateRestaurant(updateObj, emailUser);
                
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
           
            var emailUser = GetEmailUserLogin().Value;
            res = _serviceRes.RestoreRestaurant(idRestaurant, idUser, emailUser);
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
              
                var emailUser = GetEmailUserLogin().Value;
                res = _serviceRes.CreatePlace(createObj, emailUser);
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
              
                var emailUser = GetEmailUserLogin().Value;
                res = _serviceRes.UpdatePlace(updateObj, emailUser);
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
          
            var emailUser = GetEmailUserLogin().Value;
            res = _serviceRes.DeletePlace(idPlace, idUser, emailUser);
            return Ok(res); 
        }

        [HttpPut]
        [Authorize]
        [Route("restore-place")]
        public object RestorePlace(Guid idPlace, Guid idUser)
        {
           
            var emailUser = GetEmailUserLogin().Value;
            res = _serviceRes.RestorePlace(idPlace, idUser, emailUser);
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

        //[HttpPost]
        //[Authorize]
        //[Route("search-hotel-waiting")]
        //public object SearchHotelWaiting([FromBody] JObject frmData)
        //{
        //    res = _serviceRes.SearchHotelWaiting(frmData);
        //    return Ok(res);
        //}

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
        [Route("search-restaurant")]
        public object SearchRestaurant([FromBody] JObject frmData)
        {
            res = _serviceRes.SearchRestaurant(frmData);
            return Ok(res);
        }

        //[HttpPost]
        //[Authorize]
        //[Route("search-place-waiting")]
        //public object SearchPlaceWaiting([FromBody] JObject frmData)
        //{
        //    res = _serviceRes.SearchPlaceWaiting(frmData);
        //    return Ok(res);
        //}

        //[HttpPost]
        //[Authorize]
        //[Route("search-restaurant-waiting")]
        //public object SearchRestaurantWaiting([FromBody] JObject frmData)
        //{
        //    res = _serviceRes.SearchRestaurantWaiting(frmData);
        //    return Ok(res);
        //}



        [HttpGet]
        [Authorize]
        [Route("list-hotel-by-province")]
        public object GetListHotelByProvince(string toPlace)
        {
            res = _serviceRes.GetListHotelByProvince(toPlace);
            return Ok(res);
        }


        [HttpGet]
        [Authorize]
        [Route("list-place-by-province")]
        public object GetListPlaceByProvince(string toPlace)
        {
            res = _serviceRes.GetListPlaceByProvince(toPlace);
            return Ok(res);
        }


        [HttpGet]
        [Authorize]
        [Route("list-restaurant-by-province")]
        public object GetListRestaurantByProvince(string toPlace)
        {
            res = _serviceRes.GetListRestaurantByProvince(toPlace);
            return Ok(res);
        }
    }
}
