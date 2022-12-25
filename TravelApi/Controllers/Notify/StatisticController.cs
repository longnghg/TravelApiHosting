using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Travel.Data.Interfaces;
using Travel.Shared.Ultilities;
using Travel.Shared.ViewModels;

namespace TravelApi.Controllers.Notify
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatisticController : ControllerBase
    {
        private IStatistic _statistic;
        private ITourBooking _tourbooking;
        private Response res;

        public StatisticController(IStatistic statistic, ITourBooking tourbooking)
        {
            _statistic = statistic;
            res = new Response();
            _tourbooking = tourbooking;
        }

        [HttpGet]
        [Authorize]
        [Route("list-statistic-tourbooking-by-date")]
        public object GetStatisticTourbookingFromDateToDate(long fromDate, long toDate)
        {
            res = _statistic.StatisticTourBookingFromDateToDate(fromDate, toDate);
            return Ok(res);
        }
        [HttpGet]
        [Authorize]
        [Route("list-statistic-total-tourbooking-by-date")]
        public object GetStatisticTotalTourBooking(long fromDate, long toDate)
        {
            res = _statistic.GetStatisticTotalTourBooking(fromDate, toDate);
            return Ok(res);
        }
        //[HttpGet]
        //[AllowAnonymous]
        //[Route("list-statistic-total-cus")]
        //public object GetStatisticTotalCus()
        //{
        //    res = _statistic.GetStatisticTotalCus();
        //    return Ok(res);
        //}
        //[HttpGet]
        //[AllowAnonymous]
        //[Route("list-statistic-total-emp")]
        //public object GetStatisticTotalEmp()
        //{
        //    res = _statistic.GetStatisticTotalEmp();
        //    return Ok(res);
        //}
        [HttpGet]
        [Authorize]
        [Route("list-statistic-tourbooking-by-year")]
        public object GetStatisticTourbookingByYear(int year)
        {
            res = _statistic.GetStatisticTourbookingByYear(year);
            return Ok(res);
        }

        [HttpGet]
        [Authorize]
        [Route("list-statistic-tourbooking-by-month")]
        public object GetStatisticTourbookingByMonth(int month , int year)
        {
            res = _statistic.GetStatisticTourbookingByMonth(month, year);
            return Ok(res);
        }
        [HttpGet]
        [Authorize]
        [Route("list-statistic-tourbooking-by-week")]
        public object StatisticTourBookingInThisWeek(long fromDate, long toDate)
        {
            res = _statistic.StatisticTourBookingInThisWeek(fromDate,toDate);
            return Ok(res);
        }
        [HttpGet]
        [Authorize]
        [Route("list-week-by-year")]
        public object ListWeekByYear(int year)
        {
            res = _statistic.GetListWeekOfYear(year);
            return Ok(res);
        }
        [HttpPost]
        [Authorize]
        [Route("saving-current-week")]
        public async Task SavingCurrentWeek()
        {
            await _statistic.SaveReportWeek();
        }
        [HttpPost]
        [Authorize]
        [Route("saving-tourbooking-finished")]
        public async Task<object> SaveReportTourBookingEveryDay(DateTime date)
        {
            var flag = await _statistic.SaveReportTourBookingEveryDay(date);
            if (flag)
            {
                return Ok(new
                {
                    Notification = new
                    {
                        Type = Enums.TypeCRUD.Success,
                        Messenge = "Thành công"
                    }
                });
            }
            else
            {
                return BadRequest(new
                {
                    Notification = new
                    {
                        Type = Enums.TypeCRUD.Error,
                        Messenge = "Thất bại"
                    }
                });
            }
        }

      
    }
}
