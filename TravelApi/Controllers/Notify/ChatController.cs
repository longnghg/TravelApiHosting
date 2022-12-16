using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Travel.Context.Models.Notification;
using Travel.Data.Interfaces.INotify;
using Travel.Shared.ViewModels;

namespace TravelApi.Controllers.Notify
{
    [Route("api/[controller]")]
    public class ChatController : Controller
    {
        private IMessenger _messenger;
        private Notification message;
        private Response res;
        public ChatController(IMessenger messenger)
        {
            _messenger = messenger;
            res = new Response();
        }
        [HttpPost]
        [AllowAnonymous]
        [Route("create-messenger")]
        public async Task<object> Create([FromBody]Messenger input)
        {
            res = await _messenger.Create(input);
            return Ok(res);
        }
        [HttpPost]
        [AllowAnonymous]
        [Route("reply-messenger")]
        public async Task<object> ReplyMessenger([FromBody] Messenger input)
        {
            res = await _messenger.SupportedReply(input);
            return Ok(res);
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("cus-messenger")]
        public async Task<object> CustomerViewMessenger(Guid idCustomer)
        {
            res = await _messenger.CustomerViewMessenger(idCustomer);
            return Ok(res);
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("support-messenger")]
        public async Task<object> GetComment(Guid IdSuporter)
        {
            res = await _messenger.SupporterViewMessenger(IdSuporter);
            return Ok(res);
        }

        [HttpPut]
        [Authorize]
        [Route("check-seen-messenger")]
        public async Task<object> CheckSeenMessenger(Guid key)
        {
            res = await _messenger.CheckSeenMessenger(key);
            return Ok(res);
        }

        [HttpPut]
        [Authorize]
        [Route("check-seen-messenger-cus")]
        public async Task<object> CheckSeenMessenger(Guid idCus, Guid idSp)
        {
            res = await _messenger.CheckSeenMessenger(idCus, idSp);
            return Ok(res);
        }

    }
}
