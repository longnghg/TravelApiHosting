using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Travel.Context.Models.Notification;
using Travel.Data.Interfaces.INotify;
using Travel.Shared.ViewModels;
using Travel.Shared.ViewModels.Notify.CommentVM;

namespace TravelApi.Controllers.Notify
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController : Controller
    {
        private IComment _comment;
        private Notification message;
        private Response res;
        public CommentController(IComment comment)
        {
            _comment = comment;
            res = new Response();
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("gets-comment")]
        public async Task<object> GetComment(string idTour)
        {
            res = await _comment.Gets(idTour);
            return Ok(res);
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("get-idCus")]
        public async Task<object> Getid(Guid idCus)
        {
            res = await _comment.GetsId(idCus);
            return Ok(res);
        }

        [HttpPost]
        [Authorize]
        [Route("create-comment")]
        public async Task<object> Create(CreateCommentViewModel comment)
        {
            res = await _comment.Create(comment);
            return Ok(res);
        }


        [HttpDelete]
        [Authorize]
        [Route("delete-comment")]
        public async Task<object> DeleteComment(Guid idComment, Guid idCustomer)
        {
            res = await _comment.Delete(idComment, idCustomer);

            return Ok(res);
        }
    }
}
