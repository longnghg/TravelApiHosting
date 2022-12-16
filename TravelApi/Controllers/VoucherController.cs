using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using Travel.Data.Interfaces;
using Travel.Shared.ViewModels;
using Travel.Shared.ViewModels.Travel.VoucherVM;

namespace TravelApi.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class VoucherController : ControllerBase
    {
        private readonly IVoucher _voucher;
        private Notification message;
        private Response res;

        public VoucherController(IVoucher voucher , ILog log)
        {
            _voucher = voucher;  
            res = new Response();
        }

        [NonAction]
        private Claim GetEmailUserLogin()
        {
            return (User.Identity as ClaimsIdentity).Claims.Where(c => c.Type == ClaimTypes.Email).FirstOrDefault();
        }
        [HttpGet]
        [AllowAnonymous]
        [Route("list-voucher")]
        public object GetVoucher(bool isDelete)
        {
            res = _voucher.GetsVoucher(isDelete);
            return Ok(res);
        }
        [HttpPost]
        [AllowAnonymous]
        [Route("create-voucher")]
        public object CreateVoucher([FromBody] JObject frmData)
        {

            message = null;
            var result = _voucher.CheckBeforSave(frmData, ref message, false);
            if (message == null)
            {
                var createObj = JsonSerializer.Deserialize<CreateVoucherViewModel>(result);
              
                var emailUser = GetEmailUserLogin().Value;
                res = _voucher.CreateVoucher(createObj, emailUser);
            }
            else
            {
                res.Notification = message;
            }
            return Ok(res);
        }
        [HttpPut]
        [AllowAnonymous]
        [Route("update-voucher")]
        public object UpdateVoucher([FromBody] JObject frmData, int id)
        {

            message = null;
            var result = _voucher.CheckBeforSave(frmData, ref message, true);
            if (message == null)
            {
                var updateObj = JsonSerializer.Deserialize<UpdateVoucherViewModel>(result);
                
                var emailUser = GetEmailUserLogin().Value;
                res = _voucher.UpdateVoucher(updateObj, emailUser);
            }
            else
            {
                res.Notification = message;
            }
            return Ok(res);
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("voucher-tiket")]
        public object VoucherTiket(Guid idVoucher, Guid idCus)
        {
            res = _voucher.CreateTiket( idVoucher,  idCus);
            return Ok(res);
        }

        [HttpDelete]
        [Authorize]
        [Route("delete-voucher")]
        public object DeleteVoucher(Guid idVoucher)
        {
          
            var emailUser = GetEmailUserLogin().Value;
            res = _voucher.DeleteVoucher(idVoucher, emailUser);
            return Ok(res);
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("vouchers-history")]
        public object GetsVoucherHistory(Guid idCustomer)
        {
            res = _voucher.GetsVoucherHistory(idCustomer);
            return Ok(res);
        }
    }
}
