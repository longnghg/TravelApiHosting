using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Travel.Context.Models;
using Travel.Shared.ViewModels;
using Travel.Shared.ViewModels.Travel.VoucherVM;

namespace Travel.Data.Interfaces
{
   public interface IVoucher
    {
        string CheckBeforSave(JObject frmData, ref Notification _message, bool isUpdate);
        Response GetsVoucher(bool isDelete);
       // Response SearchRole(JObject frmData);
        Response CreateVoucher(CreateVoucherViewModel input, string emailUser);
        Response UpdateVoucher(UpdateVoucherViewModel input, string emailUser);
        Response DeleteVoucher(Guid id, string emailUser);
        Response RestoreVoucher(Guid id, string emailUser);
        Response CreateTiket(Guid idVoucher, Guid idCus);

        Response GetsVoucherHistory(Guid idCustomer);


        #region service call
        Task<Voucher> CheckIsVoucherValid(string code, Guid customerId);
        Task DeleteVourcherCustomer(Guid idVoucher);
        #endregion
    }
}
