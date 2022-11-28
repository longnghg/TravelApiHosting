using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Travel.Data.Interfaces;
using Travel.Shared.ViewModels;
using Travel.Shared.ViewModels.Travel.CustomerVM;
using Travel.Shared.Ultilities;
using PrUtility;
using Travel.Context.Models.Travel;
using Travel.Context.Models;
using Microsoft.AspNetCore.Http;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;

namespace Travel.Data.Repositories
{
    public class CustomerRes : ICustomer
    {
        private readonly TravelContext _db;
        private Notification message;
        private readonly IConfiguration _config;
        private Response res;
        public CustomerRes(TravelContext db, IConfiguration config)
        {
            _db = db;
            message = new Notification();
            _config = config;
            res = new Response();
        }
        private void UpdateDatabase<T>(T input)
        {
            _db.Entry(input).State = EntityState.Modified;
            _db.SaveChanges();
        }
        private void DeleteDatabase<T>(T input)
        {
            _db.Entry(input).State = EntityState.Deleted;
            _db.SaveChanges();
        }
        private void CreateDatabase<T>(T input)
        {
            _db.Entry(input).State = EntityState.Added;
            _db.SaveChanges();
        }
        private async Task SaveChangeAsync()
        {
            await _db.SaveChangesAsync();
        }
        public async Task<Response> CustomerSendRate(string idTour, int rating)
        {
            using var transaction = _db.Database.BeginTransaction();

            try
            {
                await transaction.CreateSavepointAsync("BeforeSave");

                var tour = await (from x in _db.Tour.AsNoTracking()
                                  where x.IdTour == idTour
                                  select x).FirstOrDefaultAsync();

                var listReviewByTour = await (from x in _db.reviews.AsNoTracking()
                                              where x.IdTour == idTour
                                              select x).ToListAsync();
                // create review0
                var review = new Review()
                {
                    Id = Guid.NewGuid(),
                    Rating = rating,
                    IdTour = idTour
                };
                listReviewByTour.Add(review);
                CreateDatabase(review);
                tour.Rating = listReviewByTour.Average(x => x.Rating);
                UpdateDatabase(tour);
                await SaveChangeAsync();

                transaction.Commit();
                transaction.Dispose();

                return Ultility.Responses("Cảm ơn bạn đã đánh giá !", Enums.TypeCRUD.Success.ToString());

            }
            catch (Exception e)
            {
                transaction.RollbackToSavepoint("BeforeSave");
                return Ultility.Responses("Có lỗi xảy ra !", Enums.TypeCRUD.Error.ToString(), description: e.Message);
            }
        }

        public string CheckBeforeSave(JObject frmData, ref Notification _message, bool isUpdate)
        {
            try
            {
                if (frmData != null)
                {
                    var idCustomer = PrCommon.GetString("idCustomer", frmData);
                    if (String.IsNullOrEmpty(idCustomer))
                    {
                        //idCustomer = Guid.NewGuid().ToString();
                    }

                    var nameCustomer = PrCommon.GetString("nameCustomer", frmData);
                    if (String.IsNullOrEmpty(nameCustomer))
                    {
                    }

                    var email = PrCommon.GetString("email", frmData);
                    if (!String.IsNullOrEmpty(email) && isUpdate == false)
                    {
                        var check = CheckEmailCustomer(email);
                        if (check.Notification.Type == "Validation" || check.Notification.Type == "Error")
                        {
                            _message = check.Notification;
                            return string.Empty;
                        }
                    }


                    var phone = PrCommon.GetString("phone", frmData);
                    if (String.IsNullOrEmpty(phone))
                    {
                    }
                    var birthday = PrCommon.GetString("birthday", frmData);
                    if (String.IsNullOrEmpty(birthday))
                    {
                      
                    }


                    var address = PrCommon.GetString("address", frmData);
                    if (String.IsNullOrEmpty(address))
                    {
                    }

                    var password = PrCommon.GetString("password", frmData);
                    if (String.IsNullOrEmpty(password))
                    {
                    }

                    var gender = PrCommon.GetString("gender", frmData);
                    if (String.IsNullOrEmpty(gender))
                    {
                    }

                    var modifyBy = PrCommon.GetString("modifyBy", frmData);
                    if (String.IsNullOrEmpty(modifyBy))
                    {
                    }

                    if (isUpdate)
                    {
                        UpdateCustomerViewModel objUpdate = new UpdateCustomerViewModel();
                        objUpdate.IdCustomer = Guid.Parse(idCustomer);
                        objUpdate.NameCustomer = nameCustomer;
                        objUpdate.Phone = phone;
                        objUpdate.Email = email;
                        objUpdate.Address = address;
                        if (birthday != "0" && !string.IsNullOrEmpty(birthday))
                        {
                            objUpdate.Birthday = Ultility.ConvertDatetimeToUnixTimeStampMiliSecond(DateTime.Parse(birthday));
                        }
                        objUpdate.Gender = Convert.ToBoolean(gender);
                        return JsonSerializer.Serialize(objUpdate);
                    }
                        CreateCustomerViewModel objCreate = new CreateCustomerViewModel();
                        objCreate.NameCustomer = nameCustomer;
                        objCreate.Phone = phone;
                        objCreate.Email = email;
                        objCreate.Address = address;
                        //objCreate.Birthday = Ultility.ConvertDatetimeToUnixTimeStampMiliSecond(DateTime.Parse(birthday));
                        objCreate.Password = Ultility.Encryption(password);
                        return JsonSerializer.Serialize(objCreate);
                    }
                    return string.Empty;
                }
            catch (Exception e)
            {
                _message = Ultility.Responses("Có lỗi xảy ra !", Enums.TypeCRUD.Error.ToString(), description: e.Message).Notification;
                return string.Empty;
            }     
        }

        public Response Create(CreateCustomerViewModel input)
        {
            try
            {
                Customer customer = Mapper.MapCreateCustomer(input);
                customer.IdCustomer = Guid.NewGuid();
                customer.Point = 0;
                customer.IsDelete = false;
                CreateDatabase(customer);

                return Ultility.Responses("Đăng ký thành công !", Enums.TypeCRUD.Success.ToString());
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
                var listCus = (from x in _db.Customers.AsNoTracking()
                               where x.IsDelete == false select x).ToList();
                var result = Mapper.MapCustomer(listCus);
                if (result.Count() > 0)
                {
                    return Ultility.Responses("", Enums.TypeCRUD.Success.ToString(), result);
                }
                else
                {
                    return Ultility.Responses("", Enums.TypeCRUD.Warning.ToString(), null);
                }
            }
            catch (Exception e)
            {
                return Ultility.Responses("Có lỗi xảy ra !", Enums.TypeCRUD.Error.ToString(), description: e.Message);
            }
        }
        public Response GetsHistory(Guid idCustomer)
        {
            try
            {
                var list = (from x in _db.TourBookings.AsNoTracking()
                            where x.CustomerId == idCustomer
                            orderby x.DateBooking descending    
                            select new TourBooking
                            {
                                IdTourBooking = x.IdTourBooking,
                                Status = x.Status,
                                TotalPrice = x.TotalPrice,
                                TotalPricePromotion = x.TotalPricePromotion,
                                ScheduleId = x.ScheduleId,
                                DateBooking = x.DateBooking,
                                BookingNo = x.BookingNo,
                                ValuePromotion = x.ValuePromotion,
                                TourBookingDetails = (from tbd in _db.tourBookingDetails.AsNoTracking()
                                                      where tbd.IdTourBookingDetails == x.IdTourBooking 
                                                      select tbd).First(),
                                Schedule = (from s in _db.Schedules.AsNoTracking()
                                            where x.ScheduleId == s.IdSchedule
                                            select new Schedule {
                                                Description = s.Description,
                                                DepartureDate = s.DepartureDate,
                                                DeparturePlace = s.DeparturePlace,
                                                ReturnDate = s.ReturnDate,
                                                Tour = (from t in _db.Tour.AsNoTracking()
                                                        where s.TourId == t.IdTour
                                                        select new Tour { 
                                                        Thumbnail = t.Thumbnail,
                                                        NameTour = t.NameTour,
                                                        ToPlace = t.ToPlace
                                                        }).First()
                                            }).First()
                            }).ToList();

                var result = Mapper.MapHistoryCustomerViewModel(list);

                if (result.Count() > 0)
                {
                    return Ultility.Responses("", Enums.TypeCRUD.Success.ToString(), result);
                }
                else
                {
                    return Ultility.Responses("", Enums.TypeCRUD.Warning.ToString(), null);
                }
                //var list = (from x in _db.Tourbookings
                //            where x.CustomerId == idCustomer
                //            select new Tourbooking {
                //                ValuePromotion = x.ValuePromotion,
                //                CustomerId = x.CustomerId,
                //                IsCalled = x.IsCalled,
                //                NameContact = x.NameContact,
                //                NameCustomer = x.NameCustomer,
                //                DateBooking = x.DateBooking,
                //                Deposit = x.Deposit,
                //                VoucherCode = x.VoucherCode,
                //                Address = x.Address,
                //                LastDate = x.LastDate,
                //                ModifyDate = x.ModifyDate,
                //                BookingNo = x.BookingNo,
                //                Email = x.Email,
                //                IdTourbooking = x.IdTourbooking,
                //                ModifyBy = x.ModifyBy,

                //                PaymentId = x.PaymentId,
                //                Phone = x.Phone,
                //                Pincode = x.Pincode,
                //                RemainPrice = x.RemainPrice,
                //                ScheduleId = x.ScheduleId,
                //                TotalPrice = x.TotalPrice,
                //                TotalPricePromotion = x.TotalPricePromotion,
                //                Vat = x.Vat,
                //                Payment = (from p in _db.Payment where p.IdPayment == x.PaymentId select p).First(),
                //                Schedule = (from s in _db.Schedules where s.IdSchedule == x.ScheduleId
                //                            select new Schedule{ 
                //                DepartureDate = s.DepartureDate,
                //                Tour = (from t  in _db.Tour where t.IdTour == s.TourId select t).First()
                //                }).First(),
                //                TourbookingDetails = (from td in _db.tourbookingDetails where td.IdTourbookingDetails == x.IdTourbooking select td).First()
                //            }).ToList();
                //var result = Mapper.MapTourBooking(list);

                //if (list.Count() > 0)
                //{
                //    res.Content = result;
                //}
                //return res;
            }
            catch (Exception e)
            {
                return Ultility.Responses("Có lỗi xảy ra !", Enums.TypeCRUD.Error.ToString(), description: e.Message);
            }
        }



        public async Task<Response> SendOTP(string email)
        {
            try
            {
                var account = (from x in _db.Customers.AsNoTracking()
                               where x.Email.ToLower() == email.ToLower()
                               select x).FirstOrDefault();
                if (account != null)
                {
                    string otpCode = Ultility.RandomString(8, false);
                    OTP obj = new OTP();
                    var dateTime = DateTime.Now;
                    var begin = dateTime;
                    var end = dateTime.AddMinutes(2);
                    obj.BeginTime = Ultility.ConvertDatetimeToUnixTimeStampMiliSecond(begin);
                    obj.EndTime = Ultility.ConvertDatetimeToUnixTimeStampMiliSecond(end);
                    obj.OTPCode = otpCode;
                    await _db.OTPs.AddAsync(obj);
                    await _db.SaveChangesAsync();

                    var subjectOTP = _config["OTPSubject"];
                    var emailSend = _config["emailSend"];
                    var keySecurity = _config["keySecurity"]; 
                     var stringHtml = Ultility.getHtml(otpCode, subjectOTP, "OTP");

                    Ultility.sendEmail(stringHtml, email, "Yêu cầu quên mật khẩu", emailSend,keySecurity);
                    return Ultility.Responses($"Mã OTP đã gửi vào email {email}!", Enums.TypeCRUD.Success.ToString(), obj);

                }
                else
                {
                    return Ultility.Responses($"{email} không tồn tại!", Enums.TypeCRUD.Error.ToString());
                }
            }
            catch (Exception e)
            {
                return Ultility.Responses("Có lỗi xảy ra !", Enums.TypeCRUD.Error.ToString(), description: e.Message);
            }
        }



        public Response GetCustomer(Guid idCustomer)
        {
            try
            {
                var customer = (from x in _db.Customers.AsNoTracking()
                                where x.IdCustomer == idCustomer
                                select x).First();
                var result = Mapper.MapCustomer(customer);

                if (result != null)
                {
                    return Ultility.Responses("", Enums.TypeCRUD.Success.ToString(), result);
                }
                else
                {
                    return Ultility.Responses("", Enums.TypeCRUD.Warning.ToString());
                }
            }
            catch (Exception e)
            {
                return Ultility.Responses("Có lỗi xảy ra !", Enums.TypeCRUD.Error.ToString(), description: e.Message);
            }
        }

        public async Task<Response> UpdateCustomer(UpdateCustomerViewModel input)
        {
            try
            {
                var customer = await (from x in _db.Customers.AsNoTracking()
                                      where x.IdCustomer == input.IdCustomer
                                      select x).FirstOrDefaultAsync();
                customer.NameCustomer = input.NameCustomer;
                customer.Phone = input.Phone;
                customer.Email = input.Email;
                customer.Address = input.Address;
                customer.Gender = input.Gender;
                customer.Birthday = input.Birthday;
                UpdateDatabase(customer);
                return Ultility.Responses("Cập nhật thành công !", Enums.TypeCRUD.Success.ToString());
            }
            catch (Exception e)
            {
                return Ultility.Responses("Có lỗi xảy ra !", Enums.TypeCRUD.Error.ToString(), description: e.Message);
            }
        }

        public Response CheckEmailCustomer(string email)
        {
            try
            {
                var cus = (from x in _db.Customers where x.IsDelete == false && x.Email == email select x).Count();
                if (cus > 0)
                {
                    return Ultility.Responses("[" + email + "] này đã được đăng ký !", Enums.TypeCRUD.Validation.ToString(), description: "Email");
                }
                else
                {
                    return Ultility.Responses("", Enums.TypeCRUD.Error.ToString());
                }
            }
            catch (Exception e)
            {
                return Ultility.Responses("Có lỗi xảy ra !", Enums.TypeCRUD.Error.ToString(), description: e.Message);

            }
        }

        public async Task<bool> UpdateScoreToCustomer(Guid idCustomer, int point)
        {
            try
            {
                var customer = await (from x in _db.Customers.AsNoTracking()
                                      where x.IdCustomer == idCustomer
                                      select x).FirstOrDefaultAsync();
                if (customer != null )
                {
                    customer.Point += point;
                    
                    customer.Legit += 10;
                    if (customer.Legit > 100)
                    {
                        customer.Legit = 100;
                    }
                    UpdateDatabase(customer);
                    await SaveChangeAsync();
                    return true;

                }
                return false;
            }
            catch 
            {
                return false;

            }
        }
    }


}
