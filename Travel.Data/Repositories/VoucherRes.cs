
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using PrUtility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Travel.Context.Models;
using Travel.Context.Models.Travel;
using Travel.Data.Interfaces;
using Travel.Shared.Ultilities;
using Travel.Shared.ViewModels;
using Travel.Shared.ViewModels.Travel.VoucherVM;

namespace Travel.Data.Repositories
{
    public class VoucherRes : IVoucher
    {
        private readonly TravelContext _db;
        private Notification message;
        private Response res;
        private readonly ILog _log;
        public VoucherRes(TravelContext db , ILog log)
        {
            _db = db;
            _log = log;
            message = new Notification();
            res = new Response();
        }
        public string CheckBeforSave(JObject frmData, ref Notification _message, bool isUpdate)
        {
            try
            {            
                var code = PrCommon.GetString("code", frmData);
                if (String.IsNullOrEmpty(code))
                {
                }
             
                var value = PrCommon.GetString("value", frmData);
                if (String.IsNullOrEmpty(value))
                {
                }
                var startDate = PrCommon.GetString("startDate", frmData);
                if (String.IsNullOrEmpty(startDate))
                {
                }
                var endDate = PrCommon.GetString("endDate", frmData);
                if (String.IsNullOrEmpty(endDate))
                {
                }
               
              
                if (isUpdate)
                {
                    // map data
                    UpdateVoucherViewModel objUpdate = new UpdateVoucherViewModel();
                     objUpdate.Code = code;
                    objUpdate.Value = int.Parse(value);
                    objUpdate.StartDate = long.Parse(startDate);
                    objUpdate.EndDate = long.Parse(endDate);
                   
                    // generate ID

                    return JsonSerializer.Serialize(objUpdate);
                }
                // map data
                CreateVoucherViewModel obj = new CreateVoucherViewModel();
                
                obj.Code = Ultility.RandomString(8, false);
                obj.Value = int.Parse(value);
                obj.StartDate = long.Parse(startDate);
                obj.EndDate = long.Parse(endDate);
             
                return JsonSerializer.Serialize(obj);
            }
            catch (Exception e)
            {
                message.DateTime = DateTime.Now;
                message.Description = e.Message;
                message.Messenge = "Có lỗi xảy ra !";
                message.Type = "Error";
                _message = message;
                return null;
            }
        }

        public Response CreateTiket(Guid idVoucher, Guid idCus)
        {
           
            try
            {
                
                var cus = _db.Customers.Find(idCus);
                var vou = _db.Vouchers.Find(idVoucher);
                var voucher = new Customer_Voucher();

                if (cus.Point > vou.Value)
                {
                    var value = cus.Point - vou.Value;
                    cus.Point = value;
                    voucher.VoucherId = idVoucher;
                    voucher.CustomerId = idCus;              
                    _db.Customer_Vouchers.Add(voucher);
                    _db.SaveChanges();
                    return Ultility.Responses("Mua thành công !", Enums.TypeCRUD.Success.ToString());
                }
                else
                {
                    return Ultility.Responses("Bạn không đủ điểm  !", Enums.TypeCRUD.Success.ToString());
                }
                
            }
            catch (Exception e)
            {
                return Ultility.Responses("Có lỗi xảy ra !", Enums.TypeCRUD.Error.ToString(), description: e.Message);
            }
        }

        public Response CreateVoucher(CreateVoucherViewModel input, string emailUser)
        {
            try
            {
                Voucher voucher = new Voucher();
                voucher = Mapper.MapCreateVoucher(input);
                string jsonContent = JsonSerializer.Serialize(voucher);

                _db.Vouchers.Add(voucher);
                _db.SaveChanges();
                bool result = _log.AddLog(content: jsonContent, type: "create", emailCreator: emailUser, classContent: "Voucher");
                if (result)
                {
                    return Ultility.Responses($"Thêm thành công !", Enums.TypeCRUD.Success.ToString());
                }
                else
                {
                    return Ultility.Responses("Lỗi log!", Enums.TypeCRUD.Error.ToString());
                }
            }
            catch (Exception e)
            {

                res.Notification.DateTime = DateTime.Now;
                res.Notification.Description = e.Message;
                res.Notification.Messenge = "Có lỗi xảy ra !";
                res.Notification.Type = "Error";
                return res;
            }
        }

        public Response DeleteVoucher(Guid id, string emailUser)
        {
            try
            {
                var voucher = _db.Vouchers.Find(id);
                if (voucher != null)
                {
                    string jsonContent = JsonSerializer.Serialize(voucher);
                    _db.Vouchers.Remove(voucher);
                    _db.SaveChanges();

                    bool result = _log.AddLog(content: jsonContent, type: "delete", emailCreator: emailUser, classContent: "Voucher");
                    if (result)
                    {
                        return Ultility.Responses($"Xóa thành công !", Enums.TypeCRUD.Success.ToString());
                    }
                    else
                    {
                        return Ultility.Responses("Lỗi log!", Enums.TypeCRUD.Error.ToString());
                    }

                }
                else
                {
                    return Ultility.Responses($"Không tìm thấy !", Enums.TypeCRUD.Warning.ToString());

                }
            }
            catch (Exception e)
            {
                res.Notification.DateTime = DateTime.Now;
                res.Notification.Description = e.Message;
                res.Notification.Messenge = "Có lỗi xảy ra !";
                res.Notification.Type = "Error";
                return res;
            }
        }

        public Response GetsVoucher(bool isDelete)
        {
            try
            {
                var list = (from x in _db.Vouchers.AsNoTracking()
                          
                            select x).ToList();
                var result = Mapper.MapVoucher(list);
                return Ultility.Responses("", Enums.TypeCRUD.Success.ToString(), result);
            }
            catch (Exception e)
            {
                return Ultility.Responses("Có lỗi xảy ra !", Enums.TypeCRUD.Error.ToString(), description: e.Message);
            }
        }

        public Response RestoreVoucher(Guid id, string emailUser)
        {
            try
            {
                var voucher = _db.Vouchers.Find(id);
                if (voucher != null)
                {
                    string jsonContent = JsonSerializer.Serialize(voucher);

                    _db.SaveChanges();

                    bool result = _log.AddLog(content: jsonContent, type: "restore", emailCreator: emailUser, classContent: "Voucher");
                    if (result)
                    {
                        return Ultility.Responses($"Khôi phục thành công !", Enums.TypeCRUD.Success.ToString());
                    }
                    else
                    {
                        return Ultility.Responses("Lỗi log!", Enums.TypeCRUD.Error.ToString());
                    }
                }
                else
                {
                    res.Notification.DateTime = DateTime.Now;
                    res.Notification.Messenge = "Không tìm thấy !";
                    res.Notification.Type = "Warning";
                }
                return res;
            }
            catch (Exception e)
            {
                res.Notification.DateTime = DateTime.Now;
                res.Notification.Description = e.Message;
                res.Notification.Messenge = "Có lỗi xảy ra !";
                res.Notification.Type = "Error";
                return res;
            }
        }

        public Response UpdateVoucher(UpdateVoucherViewModel input,string emailUser)
        {
            try
            {
                var update = (from x in _db.Vouchers where x.IdVoucher == input.IdVoucher select x).FirstOrDefault();
                Voucher voucher = new Voucher();
                string jsonContent = JsonSerializer.Serialize(voucher);

                voucher = Mapper.MapUpdateVoucher(input);
                _db.Vouchers.Update(voucher);
                _db.SaveChanges();

              
                bool result = _log.AddLog(content: jsonContent, type: "update", emailCreator: emailUser, classContent: "Voucher");
                if (result)
                {
                    return Ultility.Responses($"Sửa thành công !", Enums.TypeCRUD.Success.ToString());
                }
                else
                {
                    return Ultility.Responses("Lỗi log!", Enums.TypeCRUD.Error.ToString());
                }
            }
            catch (Exception e)
            {
                return Ultility.Responses("Có lỗi xảy ra !", Enums.TypeCRUD.Error.ToString(), description: e.Message);

            }
        }

        public Response GetsVoucherHistory(Guid idCustomer)
        {
            try
            {

                var list = (from x in _db.Customer_Vouchers
                            join v in _db.Vouchers on x.VoucherId equals v.IdVoucher              
                            select v).ToList();
    
                return Ultility.Responses("", Enums.TypeCRUD.Success.ToString(), list);
            }
            catch (Exception e)
            {
                return Ultility.Responses("Có lỗi xảy ra !", Enums.TypeCRUD.Error.ToString(), description: e.Message);
            }
        }


        #region service call
        public async Task<Voucher> CheckIsVoucherValid(string code,Guid customerId)
        {

            var unixDateTimeNow = Ultility.ConvertDatetimeToUnixTimeStampMiliSecond(DateTime.Now);
            var vourcher = await (from x in _db.Vouchers.AsNoTracking()
                                  join vc in _db.Customer_Vouchers.AsNoTracking()
                                  on x.IdVoucher equals vc.VoucherId
                              where x.Code == code
                              && vc.CustomerId == customerId
                              && x.EndDate >= unixDateTimeNow
                              select x).FirstOrDefaultAsync();
            return vourcher;
        }

        public async Task DeleteVourcherCustomer(Guid idVoucher)
        {
            var voucherCus = await (from x in _db.Customer_Vouchers
                                    where x.VoucherId == idVoucher
                                    select x).FirstOrDefaultAsync();
            _db.Customer_Vouchers.Remove(voucherCus);
            await _db.SaveChangesAsync();
        }
        #endregion
    }
}
