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
using Travel.Shared.ViewModels.Travel;

namespace Travel.Data.Repositories
{
    public class PaymentRes : IPayment
    {
        private readonly TravelContext _db;
        private Notification message;
        private Response res;
        public PaymentRes(TravelContext db)
        {
            _db = db;
            message = new Notification();
            res = new Response();
        }
      

        public string CheckBeforSave(JObject frmData, ref Notification _message, bool isUpdate = false)
        {
            try
            {
                var idPay = PrCommon.GetString("idPayment", frmData);
                if (!String.IsNullOrEmpty(idPay))
                {
                 //   payment.IdPayment = idPay;
                }
                var namePay = PrCommon.GetString("namePayment", frmData);
                if (!String.IsNullOrEmpty(namePay))
                {
                   // payment.IdPayment = namePay;
                }
                var type = PrCommon.GetString("type", frmData);
                if (!String.IsNullOrEmpty(type))
                {
                   // payment.IdPayment = type;
                }
                if (isUpdate)
                {
                    CreatePaymentViewModel updateObj = new CreatePaymentViewModel();
                    updateObj.IdPayment = idPay;
                    updateObj.NamePayment = namePay;
                    updateObj.Type = type;
                    return JsonSerializer.Serialize(updateObj);
                }
                CreatePaymentViewModel createObj = new CreatePaymentViewModel();
                createObj.IdPayment = idPay;
                createObj.NamePayment = namePay;
                createObj.Type = type;
                return JsonSerializer.Serialize(createObj);
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

        public Response Create(CreatePaymentViewModel input)
        {
            try
            {
                Payment pay = new Payment();
                pay = Mapper.MapCreatePayment(input);

                _db.Payment.Add(pay);
                _db.SaveChanges();
                return Ultility.Responses($"Thêm mới thành công !", Enums.TypeCRUD.Success.ToString());

            }
            catch (Exception e)
            {

                return Ultility.Responses("Có lỗi xảy ra !", Enums.TypeCRUD.Error.ToString(), description: e.Message);

            }
        }




        public Response Gets()
        {
            try
            {
                var listPayment = (from x in _db.Payment.AsNoTracking()
                                   select x).ToList();
                var result = Mapper.MapPayment(listPayment);
                if (result.Count() > 0)
                {
                    res = Ultility.Responses("", Enums.TypeCRUD.Success.ToString(), result);
                }
                return res;
            }
            catch (Exception e)
            {
                return Ultility.Responses("Có lỗi xảy ra !", Enums.TypeCRUD.Error.ToString(), description: e.Message);

            };
        }

        //public Response Update(CreateUpdatePaymentViewModel input)
        //{
        //    try
        //    {
        //        Payment pay = new Payment();
        //        pay = Mapper.MapCreatePayment(input);

        //        _db.Payment.Update(pay);
        //        _db.SaveChanges();

        //        res.Notification.DateTime = DateTime.Now;
        //        res.Notification.Messenge = "Sửa thành công !";
        //        res.Notification.Type = "Success";
        //        return res;
        //    }
        //    catch (Exception e)
        //    {

        //        res.Notification.DateTime = DateTime.Now;
        //        res.Notification.Description = e.Message;
        //        res.Notification.Messenge = "Có lỗi xảy ra !";
        //        res.Notification.Type = "Error";
        //        return res;
        //    }
        //}
    }
}
