using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
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


        public NewsController(INews _news)
        {
            news = _news;
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
    }
}
