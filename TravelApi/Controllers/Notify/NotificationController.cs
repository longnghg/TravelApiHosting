using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Travel.Data.Interfaces.INotify;
using Travel.Shared.ViewModels;

namespace TravelApi.Controllers.Notify
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private INotification _notification;
        private Notification message;
        private Response res;
        public NotificationController(INotification notification)
        {
            _notification = notification;
            res = new Response();
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("list-notification")]
        public async Task<object> Get(string idRole, Guid idEmp, bool IsSeen, int pageSize)
        {
            res = await _notification.Get(idRole, idEmp, IsSeen, pageSize);
            return Ok(res);
        }

        [HttpPut]
        [AllowAnonymous]
        [Route("update-isSeen-notification")]
        public async Task<object> UpdateIsSeen(Guid idNotification) 
        {
            res = await _notification.UpdateIsSeen(idNotification);
            return Ok(res);
        }

        [HttpDelete]
        [AllowAnonymous]
        [Route("delete-notification")]
        public async Task<object> Delete(Guid idNotification)
        {
            res = await _notification.Delete(idNotification);
            return Ok(res);
        }
    }
}
