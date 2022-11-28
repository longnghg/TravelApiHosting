using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json.Linq;
using PrUtility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Travel.Data.Interfaces;
using Travel.Shared.Ultilities;
using Travel.Shared.ViewModels;
using Travel.Shared.ViewModels.Travel;
using Travel.Shared.ViewModels.Travel.TourBookingVM;
using TravelApi.Hubs;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860
namespace TravelApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TourBookingController : Controller
    {
        private ITourBooking _tourbooking;
        private readonly ISchedule _schedule;

        private Notification message;
        private Response res;
        public TourBookingController(ITourBooking tourbooking,
            ISchedule schedule)
        {
            _tourbooking = tourbooking;
            _schedule = schedule;
            res = new Response();
        }
        [HttpGet]
        [Authorize]
        [Route("do-payment")]
        public object DoPayment(string idTourBooking)
        {
            res = _tourbooking.DoPayment(idTourBooking);
            return Ok(res);
        }
        [HttpGet]
        [Authorize]
        [Route("list-tourbooking-by-date")]
        public object GetTourBookingFromDateToDate(DateTime? fromDateInput, DateTime? toDateInput)
        {
            res = _tourbooking.GetTourBookingFromDateToDate(fromDateInput, toDateInput);
            return Ok(res);
        }
        [HttpGet]
        [AllowAnonymous]
        [Route("list-tourbooking")]
        public async Task<object> getsTourBooking()
        {
            res = await _tourbooking.Gets();
            return Ok(res);
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("create-tourbooking")]
        public async Task<object> Create([FromBody] JObject frmData)
        {
            message = null;
            var result = _tourbooking.CheckBeforSave(frmData, ref message, false);
            if (message == null)
            {
                var createObj = JsonSerializer.Deserialize<CreateTourBookingViewModel>(result);
                int adult = createObj.BookingDetails.Adult;
                int child = createObj.BookingDetails.Child;
                int baby = createObj.BookingDetails.Baby;

                var checkEmpty = _schedule.CheckEmptyCapacity(createObj.ScheduleId, adult, child, baby);
                if(checkEmpty == null)
                {
                    //await _schedule.UpdateCapacity(createObj.ScheduleId, adult, child, baby);
                    res = await _tourbooking.Create(createObj);
                }
                else
                {
                    return Ok(checkEmpty);
                }
            }
            else
            {
                res.Notification = message;
            }
            return Ok(res);
        }
        [HttpGet]
        [AllowAnonymous]
        [Route("tour-booking-by-id")]
        public async Task<object> TourBookingById(string idTourBooking)
        {
            res = await _tourbooking.TourBookingById(idTourBooking);
            return Ok(res);
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("tour-booking-by-booking-no")]
        public async Task<object> TourBookingByBookingNo(string bookingNo)
        {
            res = await _tourbooking.TourBookingByBookingNo(bookingNo);
            return Ok(res);
        }


        [HttpPut]
        [AllowAnonymous]
        [Route("cancel-booking")]
        public async Task<object> CancelBooking(string idTourBooking)
        {
            res = await _tourbooking.CancelBooking(idTourBooking);
            return Ok(res);
        }

        [HttpPut]
        [AllowAnonymous]
        [Route("restore-booking")]
        public async Task<object> RestoreBooking(string idTourBooking)
        {
            res = await _tourbooking.RestoreBooking(idTourBooking);
            return Ok(res);
        }
        [HttpGet]
        [Authorize]
        [Route("statistic-tourbooking")]
        public object StatisticTourBooking()
        {
            res = _tourbooking.StatisticTourBooking();
            return Ok(res);
        }
        [HttpGet]
        [Authorize]
        [Route("check-called")]
        public object CheckCalled(string idTourBooking)
        {
            res = _tourbooking.CheckCalled(idTourBooking);
            return Ok(res);
        }
        [HttpPost]
        [Authorize]
        [Route("search-TourBooking")]
        public object SearchTourBooking([FromBody] JObject frmData)
        {
            res = _tourbooking.SearchTourBooking(frmData);
            return Ok(res);
        }

        [HttpPut]
        [Authorize]
        [Route("update-tourBooking-status")]
        public object UpdateStatus(string pincode)
        {
            res = _tourbooking.UpdateStatus(pincode);
            return Ok(res);
        }

        [HttpPost]
        [Authorize]
        [Route("update-tourBooking-finished")]
        public async Task<object> UpdateTourBookingFinished()
        {
            var flag = await _tourbooking.UpdateTourBookingFinished();
            if (flag)
            {
                return Ok(new
                {
                    Notification = new {
                        Type = Enums.TypeCRUD.Success,
                        Messenge = "Thành công"
                    }
                });
            }
            else
            {
                return BadRequest(new
                {
                    Status = -1,
                    Message = "Thất bại"
                });
            }
        }
    }
}
