using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Travel.Data.Interfaces;
using Travel.Shared.ViewModels;

namespace TravelApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NewsController : Controller
    {
        private INews news;
        private Response res;
        private ICache cache;


        public NewsController(INews _news, ICache _cache)
        {
            news = _news;
            cache = _cache;
            res = new Response();
        }

        [HttpPost]
        [Authorize]
        [Route("UploadBanner")]
        public object UploadBanner(string name, IFormCollection frmdata, ICollection<IFormFile> files)
        {
            try
            {
                res = news.UploadBanner(name, frmdata, files);
                return Ok(res);
            }
            catch (Exception)
            {
                return Ok(res);
            }
        }

        [HttpGet]
        [Authorize]
        [Route("list-banner")]
        public object GetBanner()
        {
            res = news.GetBanner();
            return Ok(res);
        }

        [HttpDelete]
        [Authorize]
        [Route("delete-banner")]
        public object DeleteEmployee(Guid idBanner)
        {
            res = news.DeleteBanner(idBanner);
            return Ok(res);
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("gets-banner-all")]
        public object GetBannerAll()
        {
            res = news.GetBannerAll();
            return Ok(res);
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("weather-forecast")]
        public async Task<object> WeatherLoading(string location)
        {
            res = await news.GetApiWeather(location);
            return Ok(res);
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("translate-language")]
        public async Task<object> Translate(string input, string fromLang,string toLang) 
        {
            var resultString = await news.Translate(input,fromLang,toLang);
            return Ok(resultString);

        }
        [HttpGet]
        [AllowAnonymous]
        [Route("map-location")]
        public async Task<object> GetGoogleMapLocation(string locationString)
        {
            var map = await news.GetGoogleMapLocation(locationString);
           
            return Ok(map);

        }

        [HttpPost]
        [Authorize]
        [Route("search-banner")]
        public object SearchBanner(JObject frmData)
        {
            res = news.SearchBanner(frmData);
            return Ok(res);
        }
    }
}
