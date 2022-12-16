using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Travel.Context.Models;
using Travel.Data.Interfaces;
using Travel.Shared.ViewModels;

namespace TravelApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        private readonly IImage _imageRes;
        private Notification message;
        private Response res;

        public ImageController(IImage imageRes, ILog log)
        {
            _imageRes = imageRes;
            res = new Response();
        }

        [NonAction]
        private Claim GetEmailUserLogin()
        {
            return (User.Identity as ClaimsIdentity).Claims.Where(c => c.Type == ClaimTypes.Email).FirstOrDefault();
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("list-image-idTour")]
        public object GetImageByIdTour(string idTour)
        {
            res = _imageRes.GetImageByIdTour(idTour);
            return Ok(res);
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("list-image-banner")]
        public object GetImageByBanner(Guid idBanner)
        {
            res = _imageRes.GetImageByBanner(idBanner);
            return Ok(res);
        }

        [HttpPost]
        [Authorize]
        [Route("create-image-idTour")]
        public object Create(ICollection<IFormFile> files, string idTour)
        {
            message = null;
            if (message == null)
            {
                var createObj = files;
                if(files.Count > 0)
                {
                    var emailUser = GetEmailUserLogin().Value;
                    res = _imageRes.CreateImageTourDetail(createObj, idTour, emailUser);
                }
            }
            else
            {
                res.Notification = message;
            }
            return Ok(res);
        }

        [HttpPost]
        [Authorize]
        [Route("delete-image-idTour")]
        public object Delete(ICollection<Image> images)
        {
            message = null;
            if (message == null)
            {
                var updateObj = images;
                var emailUser = GetEmailUserLogin().Value;
                res = _imageRes.DeleteImageTourDetail(updateObj, emailUser);

            }
            else
            {
                res.Notification = message;
            }
            return Ok(res);
        }
    }
}
