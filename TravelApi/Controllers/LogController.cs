using Microsoft.AspNetCore.Authorization;
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
    public class LogController : Controller
    {
        private ILog _log;
        private Notification message;
        private Response res;
        public LogController(ILog log)
        {
            _log = log;
            res = new Response();

        }
        [HttpPost]
        [Authorize]
        [Route("search-log-by-type")]
        public async Task<object> Gets(JObject frmData)
        {
           res =  await _log.SearchLogByType(frmData);
            return Ok(res);
        }
        [HttpGet]
        [Authorize]
        [Route("detail-log")]
        public async Task<object> Get(Guid id)
        {
            res = await _log.GetDetail(id);
            return Ok(res);
        }
    }
}
