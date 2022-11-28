using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Travel.Context.Models.Travel;
using Travel.Shared;
using Travel.Shared.Ultilities;
using TravelApi.Hubs;

namespace TravelApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        
        private readonly TravelContext _db;

        public WeatherForecastController(TravelContext db)
        {
            _db = db;
        }
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        [HttpGet]
        [Route("get-data")]
        public void  Get()
        {
            List<string> offers = new List<string>();
            offers.Add("20% Off on IPhone 12");
            offers.Add("15% Off on HP Pavillion");
            offers.Add("25% Off on Samsung Smart TV");
        }
    }
}
