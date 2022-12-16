using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Travel.Context.Models;
using Travel.Shared.ViewModels.Travel;
using Travel.Shared.ViewModels.Travel.TourVM;
using Travel.Shared.Ultilities;
using Travel.Shared.ViewModels.Travel.CustomerVM;
using Travel.Shared.ViewModels;
using Travel.Shared.ViewModels.Travel.DistrictVM;
using Travel.Shared.ViewModels.Travel.WardVM;
using Travel.Shared.ViewModels.Travel.CostTourVM;
using Travel.Shared.ViewModels.Travel.TourBookingVM;
using Travel.Shared.ViewModels.Travel.ContractVM;
using static Travel.Shared.ViewModels.Travel.ServiceVM.ServiceViewModel;
using static Travel.Shared.Ultilities.Enums;
using Travel.Shared.ViewModels.Travel.VoucherVM;
using Travel.Shared.ViewModels.Travel.PromotionVM;
using Travel.Shared.ViewModels.Travel.ReviewVM;
using static Travel.Shared.ViewModels.Travel.CreateTimeLineViewModel;
using Travel.Context.Models.Notification;
using Travel.Shared.ViewModels.Notify.CommentVM;

namespace Travel.Shared.Ultilities
{
    public static class Mapper
    {
        private static IMapper _mapper;
        public static void RegisterMappings()
        {
            var mapperConfiguration = new MapperConfiguration(cfg =>
            {



                // create contract 
                cfg.CreateMap<CreateContractViewModel, Contract>()
             .ForMember(dto => dto.IdContract, opt => opt.MapFrom(src => Guid.NewGuid()))
             .ForMember(dto => dto.CreateBy, opt => opt.MapFrom(src => src.CreateBy))
             .ForMember(dto => dto.CreateDate, opt => opt.MapFrom(src => Ultility.ConvertDatetimeToUnixTimeStampMiliSecond(DateTime.Now)))
             .ForMember(dto => dto.ExpDate, opt => opt.MapFrom(src => src.ExpDate))
             .ForMember(dto => dto.SignDate, opt => opt.MapFrom(src => src.SignDate))
             .ForMember(dto => dto.FileId, opt => opt.MapFrom(src => src.FileId))
             .ForMember(dto => dto.NameContract, opt => opt.MapFrom(src => src.NameContract))
             .ForMember(dto => dto.ServiceId, opt => opt.MapFrom(src => src.ServiceId))
             .ForMember(dto => dto.TypeService, opt => opt.MapFrom(src => src.TypeService))
             ;

                // create tourbooking details

                cfg.CreateMap<CreateBookingDetailViewModel, TourBookingDetails>()
                   .ForMember(dto => dto.IsCalled, opt => opt.MapFrom(src => false))
                   .ForMember(dto => dto.Baby, opt => opt.MapFrom(src => src.Baby))
                   .ForMember(dto => dto.Child, opt => opt.MapFrom(src => src.Child))
                   .ForMember(dto => dto.Adult, opt => opt.MapFrom(src => src.Adult == 0 ? 1 : src.Adult))
                   .ForMember(dto => dto.Status, opt => opt.MapFrom(src => src.Status))
                   .ForMember(dto => dto.HotelId, opt => opt.MapFrom(src => src.HotelId))
                   .ForMember(dto => dto.RestaurantId, opt => opt.MapFrom(src => src.RestaurantId))
                   .ForMember(dto => dto.PlaceId, opt => opt.MapFrom(src => src.PlaceId))
                   ;
                // create tourbooking
                cfg.CreateMap<CreateTourBookingViewModel, TourBooking>()
                   .ForMember(dto => dto.IdTourBooking, opt => opt.MapFrom(src => $"TRB-{Ultility.ConvertDatetimeToUnixTimeStampMiliSecond(DateTime.Now)}"))
                   .ForMember(dto => dto.PaymentId, opt => opt.MapFrom(src => src.PaymentId))
                   .ForMember(dto => dto.TotalPrice, opt => opt.MapFrom(src => src.TotalPrice))
                   .ForMember(dto => dto.TotalPricePromotion, opt => opt.MapFrom(src => src.TotalPricePromotion))
                   .ForMember(dto => dto.ScheduleId, opt => opt.MapFrom(src => src.ScheduleId))
                    .ForMember(dto => dto.CustomerId, opt => opt.MapFrom(src => src.CustomerId))
                   .ForMember(dto => dto.NameCustomer, opt => opt.MapFrom(src => src.NameCustomer))
                   .ForMember(dto => dto.Address, opt => opt.MapFrom(src => src.Address))
                   .ForMember(dto => dto.Email, opt => opt.MapFrom(src => src.Email))
                   .ForMember(dto => dto.Phone, opt => opt.MapFrom(src => src.Phone))
                   .ForMember(dto => dto.NameContact, opt => opt.MapFrom(src => src.NameContact))
                   .ForMember(dto => dto.DateBooking, opt => opt.MapFrom(src => Ultility.ConvertDatetimeToUnixTimeStampMiliSecond(DateTime.Now)))
                   .ForMember(dto => dto.LastDate, opt => opt.MapFrom(src => Ultility.ConvertDatetimeToUnixTimeStampMiliSecond(DateTime.Now.AddDays(1))))
                   .ForMember(dto => dto.Vat, opt => opt.MapFrom(src => src.Vat))
                   .ForMember(dto => dto.Pincode, opt => opt.MapFrom(src => src.Pincode))
                   .ForMember(dto => dto.VoucherCode, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.VoucherCode) ? src.VoucherCode : ""))
                   .ForMember(dto => dto.BookingNo, opt => opt.MapFrom(src => ""))
                      .ForMember(dto => dto.ValuePromotion, opt => opt.MapFrom(src => src.ValuePromotion == null ? 0 : src.ValuePromotion))
                   .ForMember(dto => dto.IsCalled, opt => opt.MapFrom(src => false))
                   .ForMember(dto => dto.Deposit, opt => opt.MapFrom(src => 0))
                   .ForMember(dto => dto.RemainPrice, opt => opt.MapFrom(src => 0))
                   .ForMember(dto => dto.Status, opt => opt.MapFrom(src => Enums.StatusBooking.Paying))
                   .ForMember(dto => dto.ModifyBy, opt => opt.MapFrom(src => ""))
                   .ForMember(dto => dto.ModifyDate, opt => opt.MapFrom(src => 0))
                                  .ForMember(dto => dto.CheckIn, opt => opt.MapFrom(src => 0))
                   .ForMember(dto => dto.CheckOut, opt => opt.MapFrom(src => 0));

                // view tourbooking
                cfg.CreateMap<TourBooking, TourBookingViewModel>()
                    .ForMember(dto => dto.IdTourBooking, opt => opt.MapFrom(src => src.IdTourBooking))
                    .ForMember(dto => dto.NameCustomer, opt => opt.MapFrom(src => src.NameCustomer))
                    .ForMember(dto => dto.TotalPrice, opt => opt.MapFrom(src => src.TotalPrice))
                     .ForMember(dto => dto.TotalPricePromotion, opt => opt.MapFrom(src => src.TotalPricePromotion))
                    .ForMember(dto => dto.Address, opt => opt.MapFrom(src => src.Address))
                    .ForMember(dto => dto.Email, opt => opt.MapFrom(src => src.Email))
                    .ForMember(dto => dto.Phone, opt => opt.MapFrom(src => src.Phone))
                    .ForMember(dto => dto.NameContact, opt => opt.MapFrom(src => src.NameContact))
                    .ForMember(dto => dto.DateBooking, opt => opt.MapFrom(src => src.DateBooking))
                    .ForMember(dto => dto.LastDate, opt => opt.MapFrom(src => src.LastDate))
                    .ForMember(dto => dto.Vat, opt => opt.MapFrom(src => src.Vat))
                    .ForMember(dto => dto.Pincode, opt => opt.MapFrom(src => src.Pincode))
                    .ForMember(dto => dto.VoucherCode, opt => opt.MapFrom(src => src.VoucherCode))
                    .ForMember(dto => dto.BookingNo, opt => opt.MapFrom(src => src.BookingNo))
                    .ForMember(dto => dto.IsCalled, opt => opt.MapFrom(src => src.IsCalled))
                    .ForMember(dto => dto.Deposit, opt => opt.MapFrom(src => src.Deposit))
                    .ForMember(dto => dto.RemainPrice, opt => opt.MapFrom(src => src.RemainPrice))
                    .ForMember(dto => dto.TotalPrice, opt => opt.MapFrom(src => src.TotalPrice))
                    .ForMember(dto => dto.ModifyBy, opt => opt.MapFrom(src => src.ModifyBy))
                    .ForMember(dto => dto.ModifyDate, opt => opt.MapFrom(src => src.ModifyDate))
                    .ForMember(dto => dto.ModifyDate, opt => opt.MapFrom(src => src.ModifyDate))
                    .ForMember(dto => dto.PaymentId, opt => opt.MapFrom(src => src.PaymentId))
                     .ForMember(dto => dto.Status, opt => opt.MapFrom(src => src.Status))
                     .ForMember(dto => dto.ToPlace, opt => opt.MapFrom(src => src.Schedule.Tour.ToPlace))
                    //.ForMember(dto => dto.Note, opt => opt.MapFrom(src => src.TourbookingDetails.Note))



                    ;

                // create schedule
                cfg.CreateMap<CreateScheduleViewModel, Schedule>()
               .ForMember(dto => dto.IdSchedule, opt => opt.MapFrom(src => src.IdSchedule))
               .ForMember(dto => dto.Alias, opt => opt.MapFrom(src => src.Alias))
               .ForMember(dto => dto.Description, opt => opt.MapFrom(src => src.Description))
               .ForMember(dto => dto.DeparturePlace, opt => opt.MapFrom(src => src.DeparturePlace))
               .ForMember(dto => dto.BeginDate, opt => opt.MapFrom(src => src.BeginDate))
               .ForMember(dto => dto.EndDate, opt => opt.MapFrom(src => src.EndDate))
               .ForMember(dto => dto.TimePromotion, opt => opt.MapFrom(src => src.TimePromotion))
               .ForMember(dto => dto.QuantityAdult, opt => opt.MapFrom(src => 0))
               .ForMember(dto => dto.QuantityBaby, opt => opt.MapFrom(src => 0))
               .ForMember(dto => dto.QuantityChild, opt => opt.MapFrom(src => 0))
               .ForMember(dto => dto.QuantityCustomer, opt => opt.MapFrom(src => 0))
               .ForMember(dto => dto.PriceAdult, opt => opt.MapFrom(src => 0))
               .ForMember(dto => dto.PriceChild, opt => opt.MapFrom(src => 0))
               .ForMember(dto => dto.PriceBaby, opt => opt.MapFrom(src => 0))
               .ForMember(dto => dto.PriceAdultHoliday, opt => opt.MapFrom(src => 0))
               .ForMember(dto => dto.PriceChildHoliday, opt => opt.MapFrom(src => 0))
               .ForMember(dto => dto.PriceBabyHoliday, opt => opt.MapFrom(src => 0))
               .ForMember(dto => dto.MinCapacity, opt => opt.MapFrom(src => src.MinCapacity))
               .ForMember(dto => dto.MaxCapacity, opt => opt.MapFrom(src => src.MaxCapacity))
               .ForMember(dto => dto.IsHoliday, opt => opt.MapFrom(src => false))
               .ForMember(dto => dto.Status, opt => opt.MapFrom(src => Enums.StatusSchedule.Free))
               .ForMember(dto => dto.TourId, opt => opt.MapFrom(src => src.TourId))
               .ForMember(dto => dto.CarId, opt => opt.MapFrom(src => src.CarId))
               .ForMember(dto => dto.EmployeeId, opt => opt.MapFrom(src => src.EmployeeId))
               .ForMember(dto => dto.PromotionId, opt => opt.MapFrom(src => src.PromotionId))
               .ForMember(dto => dto.Description, opt => opt.MapFrom(src => src.Description))
               .ForMember(dto => dto.Vat, opt => opt.MapFrom(src => src.Vat))
                              .ForMember(dto => dto.IsTempData, opt => opt.MapFrom(src => false))
               .ForMember(dto => dto.IdUserModify, opt => opt.MapFrom(src => src.IdUserModify))
               .ForMember(dto => dto.TypeAction, opt => opt.MapFrom(src => "insert"))
               .ForMember(dto => dto.Approve, opt => opt.MapFrom(src => Enums.ApproveStatus.Waiting))
                .ForMember(dto => dto.ModifyDate, opt => opt.MapFrom(src => Ultility.ConvertDatetimeToUnixTimeStampMiliSecond(DateTime.Now)))
               ;

                cfg.CreateMap<UpdateScheduleViewModel, Schedule>()
              .ForMember(dto => dto.IdSchedule, opt => opt.MapFrom(src => src.IdSchedule))
              //.ForMember(dto => dto.Alias, opt => opt.MapFrom(src => src.Alias))
              .ForMember(dto => dto.Description, opt => opt.MapFrom(src => src.Description))
              .ForMember(dto => dto.DeparturePlace, opt => opt.MapFrom(src => src.DeparturePlace))
              .ForMember(dto => dto.BeginDate, opt => opt.MapFrom(src => src.BeginDate))
               .ForMember(dto => dto.EndDate, opt => opt.MapFrom(src => src.EndDate))
              .ForMember(dto => dto.TimePromotion, opt => opt.MapFrom(src => src.TimePromotion))
              //.ForMember(dto => dto.QuantityAdult, opt => opt.MapFrom(src => src.QuantityAdult))
              //.ForMember(dto => dto.QuantityBaby, opt => opt.MapFrom(src => src.QuantityBaby))
              //.ForMember(dto => dto.QuantityChild, opt => opt.MapFrom(src => src.QuantityChild))
              //.ForMember(dto => dto.QuantityCustomer, opt => opt.MapFrom(src => src.QuantityCustomer))
              //.ForMember(dto => dto.PriceAdult, opt => opt.MapFrom(src => src.PriceAdult))
              //.ForMember(dto => dto.PriceChild, opt => opt.MapFrom(src => src.PriceChild))
              //.ForMember(dto => dto.PriceBaby, opt => opt.MapFrom(src => src.PriceBaby))
              //.ForMember(dto => dto.PriceAdultHoliday, opt => opt.MapFrom(src => src.PriceAdultHoliday))
              //.ForMember(dto => dto.PriceChildHoliday, opt => opt.MapFrom(src => src.PriceChildHoliday))
              //.ForMember(dto => dto.PriceBabyHoliday, opt => opt.MapFrom(src => src.PriceBabyHoliday))
              .ForMember(dto => dto.MinCapacity, opt => opt.MapFrom(src => src.MinCapacity))
              .ForMember(dto => dto.MaxCapacity, opt => opt.MapFrom(src => src.MaxCapacity))
              //.ForMember(dto => dto.IsHoliday, opt => opt.MapFrom(src => src.IsHoliday))
              //.ForMember(dto => dto.Status, opt => opt.MapFrom(src => Enums.StatusSchedule.Free))
              .ForMember(dto => dto.TourId, opt => opt.MapFrom(src => src.TourId))
              .ForMember(dto => dto.CarId, opt => opt.MapFrom(src => src.CarId))
              .ForMember(dto => dto.EmployeeId, opt => opt.MapFrom(src => src.EmployeeId))
              .ForMember(dto => dto.PromotionId, opt => opt.MapFrom(src => src.PromotionId))
              .ForMember(dto => dto.Description, opt => opt.MapFrom(src => src.Description))
              .ForMember(dto => dto.Vat, opt => opt.MapFrom(src => src.Vat))
                .ForMember(dto => dto.ModifyDate, opt => opt.MapFrom(src => Ultility.ConvertDatetimeToUnixTimeStampMiliSecond(DateTime.Now)))

              ;

                //View
                cfg.CreateMap<Schedule, ScheduleViewModel>()
                .ForMember(dto => dto.IdSchedule, opt => opt.MapFrom(src => src.IdSchedule))
                .ForMember(dto => dto.Alias, opt => opt.MapFrom(src => src.Alias))
                .ForMember(dto => dto.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dto => dto.DeparturePlace, opt => opt.MapFrom(src => src.DeparturePlace))
               .ForMember(dto => dto.DepartureDate, opt => opt.MapFrom(src => src.DepartureDate))
               .ForMember(dto => dto.BeginDate, opt => opt.MapFrom(src => src.BeginDate))
               .ForMember(dto => dto.EndDate, opt => opt.MapFrom(src => src.EndDate))
               .ForMember(dto => dto.TimePromotion, opt => opt.MapFrom(src => src.TimePromotion))
               .ForMember(dto => dto.QuantityAdult, opt => opt.MapFrom(src => src.QuantityAdult))
               .ForMember(dto => dto.QuantityBaby, opt => opt.MapFrom(src => src.QuantityBaby))
               .ForMember(dto => dto.QuantityChild, opt => opt.MapFrom(src => src.QuantityChild))
               .ForMember(dto => dto.MinCapacity, opt => opt.MapFrom(src => src.MinCapacity))
               .ForMember(dto => dto.MaxCapacity, opt => opt.MapFrom(src => src.MaxCapacity))
               .ForMember(dto => dto.PriceAdult, opt => opt.MapFrom(src => src.PriceAdult))
               .ForMember(dto => dto.PriceBaby, opt => opt.MapFrom(src => src.PriceBaby))
               .ForMember(dto => dto.PriceChild, opt => opt.MapFrom(src => src.PriceChild))
               .ForMember(dto => dto.PriceAdultHoliday, opt => opt.MapFrom(src => src.PriceAdultHoliday))
               .ForMember(dto => dto.PriceBabyHoliday, opt => opt.MapFrom(src => src.PriceBabyHoliday))
               .ForMember(dto => dto.PriceChildHoliday, opt => opt.MapFrom(src => src.PriceChildHoliday))
               .ForMember(dto => dto.AdditionalPriceHoliday, opt => opt.MapFrom(src => src.MaxCapacity))
                .ForMember(dto => dto.AdditionalPrice, opt => opt.MapFrom(src => src.AdditionalPrice))
               .ForMember(dto => dto.AdditionalPriceHoliday, opt => opt.MapFrom(src => src.AdditionalPriceHoliday))
                .ForMember(dto => dto.IsHoliday, opt => opt.MapFrom(src => src.IsHoliday))
               .ForMember(dto => dto.Status, opt => opt.MapFrom(src => src.Status))
               .ForMember(dto => dto.TourId, opt => opt.MapFrom(src => src.TourId))
               .ForMember(dto => dto.CarId, opt => opt.MapFrom(src => src.CarId))
               .ForMember(dto => dto.EmployeeId, opt => opt.MapFrom(src => src.EmployeeId))
               .ForMember(dto => dto.PromotionId, opt => opt.MapFrom(src => src.PromotionId))
               .ForMember(dto => dto.NameTour, opt => opt.MapFrom(src => src.Tour.NameTour))
               .ForMember(dto => dto.LiscensePlate, opt => opt.MapFrom(src => src.Car.LiscensePlate))
               .ForMember(dto => dto.NameDriver, opt => opt.MapFrom(src => src.Car.NameDriver))
               .ForMember(dto => dto.NameEmployee, opt => opt.MapFrom(src => src.Employee.NameEmployee))
               .ForMember(dto => dto.ValuePromotion, opt => opt.MapFrom(src => src.Promotions.Value))
               .ForMember(dto => dto.TotalCostTourNotService, opt => opt.MapFrom(src => src.TotalCostTourNotService))
               .ForMember(dto => dto.IsDelete, opt => opt.MapFrom(src => src.Isdelete))
               .ForMember(dto => dto.TypeAction, opt => opt.MapFrom(src => src.TypeAction))
               .ForMember(dto => dto.IdUserModify, opt => opt.MapFrom(src => src.IdUserModify))
               .ForMember(dto => dto.Approve, opt => opt.MapFrom(src => src.Approve))
                .ForMember(dto => dto.ModifyBy, opt => opt.MapFrom(src => src.ModifyBy))
                .ForMember(dto => dto.ModifyDate, opt => opt.MapFrom(src => src.ModifyDate))

               //          private float totalCostTour;
               //private int profit;
               //private float vat;



               //private float finalPrice;
               // private float finalPriceHoliday;
               ;

                //create
                cfg.CreateMap<CreateTourViewModel, Tour>()
                .ForMember(dto => dto.IdTour, opt => opt.MapFrom(src => src.IdTour))
                .ForMember(dto => dto.NameTour, opt => opt.MapFrom(src => src.NameTour))
                .ForMember(dto => dto.Thumbnail, opt => opt.MapFrom(src => src.Thumbnail))
                .ForMember(dto => dto.ToPlace, opt => opt.MapFrom(src => src.ToPlace))
                .ForMember(dto => dto.Rating, opt => opt.MapFrom(src => src.Rating))
                .ForMember(dto => dto.ApproveStatus, opt => opt.MapFrom(src => Enums.ApproveStatus.Waiting))
                .ForMember(dto => dto.Status, opt => opt.MapFrom(src =>
                 Enums.TourStatus.Normal))
                .ForMember(dto => dto.CreateDate, opt => opt.MapFrom(src =>
                Ultility.ConvertDatetimeToUnixTimeStampMiliSecond(DateTime.Now)))
                .ForMember(dto => dto.IsDelete, opt => opt.MapFrom(src => false))
                .ForMember(dto => dto.IsActive, opt => opt.MapFrom(src => false))
                .ForMember(dto => dto.IsTempdata, opt => opt.MapFrom(src => false))
                ;

                cfg.CreateMap<UpdateTourViewModel, Tour>()
                .ForMember(dto => dto.IdTour, opt => opt.MapFrom(src => src.IdTour))
                .ForMember(dto => dto.NameTour, opt => opt.MapFrom(src => src.NameTour))
                .ForMember(dto => dto.Thumbnail, opt => opt.MapFrom(src => src.Thumbnail))
                .ForMember(dto => dto.ToPlace, opt => opt.MapFrom(src => src.ToPlace))
                .ForMember(dto => dto.ModifyDate, opt => opt.MapFrom(src =>
                Ultility.ConvertDatetimeToUnixTimeStampMiliSecond(DateTime.Now)))
                ;

                //view
                cfg.CreateMap<Tour, TourViewModel>()
                   .ForMember(dto => dto.IdTour, opt => opt.MapFrom(src => src.IdTour))
                   .ForMember(dto => dto.Rating, opt => opt.MapFrom(src => src.Rating))
                   .ForMember(dto => dto.NameTour, opt => opt.MapFrom(src => src.NameTour))
                   .ForMember(dto => dto.Thumbnail, opt => opt.MapFrom(src => src.Thumbnail))
                   .ForMember(dto => dto.ToPlace, opt => opt.MapFrom(src => src.ToPlace))
                   .ForMember(dto => dto.ApproveStatus, opt => opt.MapFrom(src => src.ApproveStatus))
                   .ForMember(dto => dto.Status, opt => opt.MapFrom(src => src.Status))
                    .ForMember(dto => dto.CreateDate, opt => opt.MapFrom(src => src.CreateDate))
                   .ForMember(dto => dto.IsActive, opt => opt.MapFrom(src => src.IsActive))
                   .ForMember(dto => dto.IsDelete, opt => opt.MapFrom(src => src.IsDelete))
                   .ForMember(dto => dto.ModifyBy, opt => opt.MapFrom(src => src.ModifyBy))
                   .ForMember(dto => dto.ModifyDate, opt => opt.MapFrom(src => src.ModifyDate))
                   .ForMember(dto => dto.QuantityBooked, opt => opt.MapFrom(src => src.QuantityBooked))
                   .ForMember(dto => dto.TypeAction, opt => opt.MapFrom(src => src.TypeAction))
                   .ForMember(dto => dto.IdUserModify, opt => opt.MapFrom(src => src.IdUserModify))
                   ;




                //   //create
                //   cfg.CreateMap<CreateTourViewModel, TourDetail>()
                //   .ForMember(dto => dto.IdTourDetail, opt => opt.MapFrom(src => $"{src.IdTour}-Details"))
                //   .ForMember(dto => dto.TourId, opt => opt.MapFrom(src => src.IdTour))
                //   .ForMember(dto => dto.Description, opt => opt.MapFrom(src => src.Description))
                //;

                cfg.CreateMap<Payment, PaymentViewModel>()
                           .ForMember(dto => dto.IdPayment, otp => otp.MapFrom(src => src.IdPayment))
                           .ForMember(dto => dto.NamePayment, otp => otp.MapFrom(src => src.NamePayment))
                           .ForMember(dto => dto.Type, otp => otp.MapFrom(src => src.Type));


                cfg.CreateMap<CreatePaymentViewModel, Payment>()
                           .ForMember(dto => dto.IdPayment, otp => otp.MapFrom(src => src.IdPayment))
                           .ForMember(dto => dto.NamePayment, otp => otp.MapFrom(src => src.NamePayment))
                           .ForMember(dto => dto.Type, otp => otp.MapFrom(src => src.Type));

                cfg.CreateMap<UpdatePaymentViewModel, Payment>()
                           .ForMember(dto => dto.IdPayment, otp => otp.MapFrom(src => src.IdPayment))
                           .ForMember(dto => dto.NamePayment, otp => otp.MapFrom(src => src.NamePayment))
                           .ForMember(dto => dto.Type, otp => otp.MapFrom(src => src.Type));

                cfg.CreateMap<Employee, EmployeeViewModel>()
                                       .ForMember(dto => dto.IdEmployee, opt => opt.MapFrom(src => src.IdEmployee))
                                       .ForMember(dto => dto.NameEmployee, opt => opt.MapFrom(src => src.NameEmployee))
                                       .ForMember(dto => dto.Birthday, opt => opt.MapFrom(src => src.Birthday))
                                       .ForMember(dto => dto.CreateDate, opt => opt.MapFrom(src => src.CreateDate))
                                       .ForMember(dto => dto.Email, opt => opt.MapFrom(src => src.Email))
                                       .ForMember(dto => dto.Image, opt => opt.MapFrom(src => src.Image))
                                       .ForMember(dto => dto.IsActive, opt => opt.MapFrom(src => src.IsActive))
                                       .ForMember(dto => dto.IsDelete, opt => opt.MapFrom(src => src.IsDelete))
                                       .ForMember(dto => dto.ModifyBy, opt => opt.MapFrom(src => src.ModifyBy))
                                       .ForMember(dto => dto.ModifyDate, opt => opt.MapFrom(src => src.ModifyDate))
                                       .ForMember(dto => dto.Phone, opt => opt.MapFrom(src => src.Phone))
                                       .ForMember(dto => dto.RoleDescription, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.Role.Description) ? "" : src.Role.Description))
                                       .ForMember(dto => dto.RoleId, opt => opt.MapFrom(src => src.RoleId))
                                       .ForMember(dto => dto.RoleName, opt => opt.MapFrom(src => src.Role.NameRole))
                                       .ForMember(dto => dto.Address, opt => opt.MapFrom(src => src.Address))
                                       .ForMember(dto => dto.Gender, opt => opt.MapFrom(src => src.Gender));

                cfg.CreateMap<CreateEmployeeViewModel, Employee>()
                          .ForMember(dto => dto.IdEmployee, opt => opt.MapFrom(src => src.IdEmployee))
                          .ForMember(dto => dto.NameEmployee, opt => opt.MapFrom(src => src.NameEmployee))
                          .ForMember(dto => dto.Email, opt => opt.MapFrom(src => src.Email))
                          .ForMember(dto => dto.Birthday, opt => opt.MapFrom(src => src.Birthday))
                          .ForMember(dto => dto.Image, opt => opt.MapFrom(src => src.Image))
                          .ForMember(dto => dto.Phone, opt => opt.MapFrom(src => src.Phone))
                          .ForMember(dto => dto.RoleId, opt => opt.MapFrom(src => src.RoleId))
                          .ForMember(dto => dto.Address, opt => opt.MapFrom(src => src.Address))
                          .ForMember(dto => dto.CreateDate, opt => opt.MapFrom(src => Ultility.ConvertDatetimeToUnixTimeStampMiliSecond(DateTime.Now)))
                          .ForMember(dto => dto.ModifyBy, opt => opt.MapFrom(src => src.ModifyBy))
                          .ForMember(dto => dto.ModifyDate, opt => opt.MapFrom(src => Ultility.ConvertDatetimeToUnixTimeStampMiliSecond(DateTime.Now)))
                          .ForMember(dto => dto.Gender, opt => opt.MapFrom(src => src.Gender));

                cfg.CreateMap<UpdateEmployeeViewModel, Employee>()
                          .ForMember(dto => dto.IdEmployee, opt => opt.MapFrom(src => src.IdEmployee))
                          .ForMember(dto => dto.NameEmployee, opt => opt.MapFrom(src => src.NameEmployee))
                          .ForMember(dto => dto.Email, opt => opt.MapFrom(src => src.Email))
                          .ForMember(dto => dto.Birthday, opt => opt.MapFrom(src => src.Birthday))
                          .ForMember(dto => dto.Image, opt => opt.MapFrom(src => src.Image))
                          .ForMember(dto => dto.Phone, opt => opt.MapFrom(src => src.Phone))
                          .ForMember(dto => dto.RoleId, opt => opt.MapFrom(src => src.RoleId))
                          .ForMember(dto => dto.Address, opt => opt.MapFrom(src => src.Address))
                          .ForMember(dto => dto.Gender, opt => opt.MapFrom(src => src.Gender))
                          .ForMember(dto => dto.ModifyBy, opt => opt.MapFrom(src => src.ModifyBy))
                          .ForMember(dto => dto.ModifyDate, opt => opt.MapFrom(src => Ultility.ConvertDatetimeToUnixTimeStampMiliSecond(DateTime.Now)));

                cfg.CreateMap<CreateRoleViewModel, Role>()
                       .ForMember(dto => dto.NameRole, opt => opt.MapFrom(src => src.NameRole))
                       .ForMember(dto => dto.Description, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.Description) ? "" : src.Description));

                cfg.CreateMap<UpdateRoleViewModel, Role>()
                      .ForMember(dto => dto.IdRole, opt => opt.MapFrom(src => src.IdRole))
                      .ForMember(dto => dto.NameRole, opt => opt.MapFrom(src => src.NameRole))
                      .ForMember(dto => dto.Description, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.Description) ? "" : src.Description));

                cfg.CreateMap<Role, RoleViewModel>()
                          .ForMember(dto => dto.IdRole, opt => opt.MapFrom(src => src.IdRole))
                          .ForMember(dto => dto.NameRole, opt => opt.MapFrom(src => src.NameRole))
                          .ForMember(dto => dto.Description, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.Description) ? "" : src.Description));


                cfg.CreateMap<Customer, CustomerViewModel>()
                         .ForMember(dto => dto.IdCustomer, opt => opt.MapFrom(src => src.IdCustomer))
                         .ForMember(dto => dto.NameCustomer, otp => otp.MapFrom(src => src.NameCustomer))
                         .ForMember(dto => dto.Phone, otp => otp.MapFrom(src => src.Phone))
                         .ForMember(dto => dto.Email, otp => otp.MapFrom(src => src.Email))
                         .ForMember(dto => dto.Address, otp => otp.MapFrom(src => src.Address))
                         .ForMember(dto => dto.Gender, otp => otp.MapFrom(src => src.Gender))
                         .ForMember(dto => dto.Birthday, otp => otp.MapFrom(src => src.Birthday))
                        .ForMember(dto => dto.CreateDate, opt => opt.MapFrom(src => src.CreateDate))
                         .ForMember(dto => dto.Point, otp => otp.MapFrom(src => src.Point))
                          .ForMember(dto => dto.IsBlock, otp => otp.MapFrom(src => src.IsBlock));

                cfg.CreateMap<CreateCustomerViewModel, Customer>()
                        .ForMember(dto => dto.IdCustomer, otp => otp.MapFrom(src => src.IdCustomer))
                        .ForMember(dto => dto.NameCustomer, otp => otp.MapFrom(src => src.NameCustomer))
                        .ForMember(dto => dto.Phone, otp => otp.MapFrom(src => src.Phone))
                        .ForMember(dto => dto.Email, otp => otp.MapFrom(src => src.Email))
                        .ForMember(dto => dto.Gender, otp => otp.MapFrom(src => src.Gender))
                        .ForMember(dto => dto.Address, otp => otp.MapFrom(src => src.Address))
                        .ForMember(dto => dto.Birthday, opt => opt.MapFrom(src => src.Birthday))
                        .ForMember(dto => dto.CreateDate, opt => opt.MapFrom(src => Ultility.ConvertDatetimeToUnixTimeStampMiliSecond(DateTime.Now)))
                        .ForMember(dto => dto.Password, otp => otp.MapFrom(src => src.Password));

                cfg.CreateMap<UpdateCustomerViewModel, Customer>()
                        .ForMember(dto => dto.IdCustomer, otp => otp.MapFrom(src => src.IdCustomer))
                        .ForMember(dto => dto.NameCustomer, otp => otp.MapFrom(src => src.NameCustomer))
                        .ForMember(dto => dto.Phone, otp => otp.MapFrom(src => src.Phone))
                        .ForMember(dto => dto.Email, otp => otp.MapFrom(src => src.Email))
                        .ForMember(dto => dto.Gender, otp => otp.MapFrom(src => src.Gender))
                        .ForMember(dto => dto.Address, otp => otp.MapFrom(src => src.Address))
                        .ForMember(dto => dto.Birthday, opt => opt.MapFrom(src => src.Birthday));



                cfg.CreateMap<Province, ProvinceViewModel>()
                       .ForMember(dto => dto.IdProvince, opt => opt.MapFrom(src => src.IdProvince))
                       .ForMember(dto => dto.NameProvince, opt => opt.MapFrom(src => src.NameProvince));

                cfg.CreateMap<CreateProvinceViewModel, Province>()
                      .ForMember(dto => dto.IdProvince, opt => opt.MapFrom(src => Guid.NewGuid()))
                      .ForMember(dto => dto.NameProvince, opt => opt.MapFrom(src => src.NameProvince));

                cfg.CreateMap<UpdateProvinceViewModel, Province>()
                    .ForMember(dto => dto.IdProvince, opt => opt.MapFrom(src => src.IdProvince))
                    .ForMember(dto => dto.NameProvince, opt => opt.MapFrom(src => src.NameProvince));

                cfg.CreateMap<District, DistrictViewModel>()
                      .ForMember(dto => dto.IdDistrict, opt => opt.MapFrom(src => src.IdDistrict))
                      .ForMember(dto => dto.NameDistrict, opt => opt.MapFrom(src => src.NameDistrict))
                      .ForMember(dto => dto.ProvinceId, opt => opt.MapFrom(src => src.ProvinceId))
                      .ForMember(dto => dto.ProvinceName, opt => opt.MapFrom(src => src.Province.NameProvince));

                cfg.CreateMap<CreateDistrictViewModel, District>()
                .ForMember(dto => dto.IdDistrict, opt => opt.MapFrom(src => Guid.NewGuid()))
                .ForMember(dto => dto.NameDistrict, opt => opt.MapFrom(src => src.NameDistrict))
                .ForMember(dto => dto.ProvinceId, opt => opt.MapFrom(src => src.IdProvince));

                cfg.CreateMap<UpdateDistrictViewModel, District>()
                    .ForMember(dto => dto.IdDistrict, opt => opt.MapFrom(src => src.IdDistrict))
                    .ForMember(dto => dto.NameDistrict, opt => opt.MapFrom(src => src.NameDistrict))
                    .ForMember(dto => dto.ProvinceId, opt => opt.MapFrom(src => src.IdProvince));

                cfg.CreateMap<Ward, WardViewModel>()
                     .ForMember(dto => dto.IdWard, opt => opt.MapFrom(src => src.IdWard))
                     .ForMember(dto => dto.NameWard, opt => opt.MapFrom(src => src.NameWard))
                     .ForMember(dto => dto.DistrictId, opt => opt.MapFrom(src => src.DistrictId))
                     .ForMember(dto => dto.DistrictName, opt => opt.MapFrom(src => src.District.NameDistrict));

                cfg.CreateMap<CreateWardViewModel, Ward>()
                .ForMember(dto => dto.IdWard, opt => opt.MapFrom(src => Guid.NewGuid()))
                .ForMember(dto => dto.NameWard, opt => opt.MapFrom(src => src.NameWard))
                .ForMember(dto => dto.DistrictId, opt => opt.MapFrom(src => src.IdDistrict));

                cfg.CreateMap<UpdateWardViewModel, Ward>()
                    .ForMember(dto => dto.IdWard, opt => opt.MapFrom(src => src.IdWard))
                    .ForMember(dto => dto.NameWard, opt => opt.MapFrom(src => src.NameWard))
                    .ForMember(dto => dto.DistrictId, opt => opt.MapFrom(src => src.IdDistrict));

                cfg.CreateMap<CreateCarViewModel, Car>()
                       .ForMember(dto => dto.IdCar, opt => opt.MapFrom(src => src.IdCar))
                       .ForMember(dto => dto.LiscensePlate, opt => opt.MapFrom(src => src.LiscensePlate))
                       .ForMember(dto => dto.Status, opt => opt.MapFrom(src => src.Status))
                       .ForMember(dto => dto.NameDriver, opt => opt.MapFrom(src => src.NameDriver))
                       .ForMember(dto => dto.Phone, opt => opt.MapFrom(src => src.Phone))
                       .ForMember(dto => dto.IdUserModify, opt => opt.MapFrom(src => src.IdUserModify));

                cfg.CreateMap<UpdateCarViewModel, Car>()
                   .ForMember(dto => dto.IdCar, opt => opt.MapFrom(src => src.IdCar))
                   .ForMember(dto => dto.LiscensePlate, opt => opt.MapFrom(src => src.LiscensePlate))
                   .ForMember(dto => dto.Status, opt => opt.MapFrom(src => src.Status))
                   .ForMember(dto => dto.NameDriver, opt => opt.MapFrom(src => src.NameDriver))
                   .ForMember(dto => dto.Phone, opt => opt.MapFrom(src => src.Phone))
                   .ForMember(dto => dto.ModifyDate, opt => opt.MapFrom(src => src.ModifyDate))
                   .ForMember(dto => dto.IdUserModify, opt => opt.MapFrom(src => src.IdUserModify));
                cfg.CreateMap<Car, CarViewModel>()
                   .ForMember(dto => dto.IdCar, opt => opt.MapFrom(src => src.IdCar))
                   .ForMember(dto => dto.LiscensePlate, opt => opt.MapFrom(src => src.LiscensePlate))
                   .ForMember(dto => dto.Status, opt => opt.MapFrom(src => src.Status))
                   .ForMember(dto => dto.NameDriver, opt => opt.MapFrom(src => src.NameDriver))
                   .ForMember(dto => dto.Phone, opt => opt.MapFrom(src => src.Phone))
                   .ForMember(dto => dto.IsDelete, opt => opt.MapFrom(src => src.IsDelete));

                cfg.CreateMap<CreatePromotionViewModel, Promotion>()
                       .ForMember(dto => dto.IdPromotion, opt => opt.MapFrom(src => src.IdPromotion))
                       .ForMember(dto => dto.Value, opt => opt.MapFrom(src => src.Value))

                       .ForMember(dto => dto.ToDate, opt => opt.MapFrom(src => src.ToDate))
                       .ForMember(dto => dto.FromDate, opt => opt.MapFrom(src => src.FromDate));

                cfg.CreateMap<Promotion, PromotionViewModel>()
                       .ForMember(dto => dto.IdPromotion, opt => opt.MapFrom(src => src.IdPromotion))
                       .ForMember(dto => dto.Value, opt => opt.MapFrom(src => src.Value))

                       .ForMember(dto => dto.ToDate, opt => opt.MapFrom(src => src.ToDate))
                        .ForMember(dto => dto.FromDate, opt => opt.MapFrom(src => src.FromDate));

                cfg.CreateMap<UpdatePromotionViewModel, Promotion>()
                      .ForMember(dto => dto.Value, opt => opt.MapFrom(src => src.Value))
                      .ForMember(dto => dto.ToDate, opt => opt.MapFrom(src => src.ToDate))
                      .ForMember(dto => dto.FromDate, opt => opt.MapFrom(src => src.FromDate));


                cfg.CreateMap<CreateTimeLineViewModel, Timeline>()
                                                  .ForMember(dto => dto.IdTimeline, otp => otp.MapFrom(src => Guid.NewGuid()))
                                                  .ForMember(dto => dto.IsDelete, otp => otp.MapFrom(src => false))
                                                  .ForMember(dto => dto.ModifyDate, otp => otp.MapFrom(src => Ultility.ConvertDatetimeToUnixTimeStampMiliSecond(DateTime.Now)))

                                  .ForMember(dto => dto.Description, otp => otp.MapFrom(src => src.Description))
                                  .ForMember(dto => dto.FromTime, otp => otp.MapFrom(src => src.FromTime))
                                   .ForMember(dto => dto.ToTime, opt => opt.MapFrom(src => src.ToTime))
                                   .ForMember(dto => dto.IdSchedule, opt => opt.MapFrom(src => src.IdSchedule));


                cfg.CreateMap<UpdateTimeLineViewModel, Timeline>()
                                .ForMember(dto => dto.IdTimeline, otp => otp.MapFrom(src => src.IdTimeLine))
                             .ForMember(dto => dto.Description, otp => otp.MapFrom(src => src.Description))
                             .ForMember(dto => dto.FromTime, otp => otp.MapFrom(src => src.FromTime))
                              .ForMember(dto => dto.ToTime, opt => opt.MapFrom(src => src.ToTime))
                              .ForMember(dto => dto.IdSchedule, opt => opt.MapFrom(src => src.IdSchedule));

                cfg.CreateMap<Timeline, TimeLineViewModel>()
                    .ForMember(dto => dto.IdTimeLine, otp => otp.MapFrom(src => src.IdTimeline))
                    .ForMember(dto => dto.Description, otp => otp.MapFrom(src => src.Description))
                    .ForMember(dto => dto.FromTime, otp => otp.MapFrom(src => src.FromTime))
                    .ForMember(dto => dto.ToTime, opt => opt.MapFrom(src => src.ToTime))
                    .ForMember(dto => dto.ModifyBy, opt => opt.MapFrom(src => src.ModifyBy))
                    .ForMember(dto => dto.ModifyDate, opt => opt.MapFrom(src => src.ModifyDate))
                    .ForMember(dto => dto.IdSchedule, opt => opt.MapFrom(src => src.IdSchedule));






                cfg.CreateMap<Place, PlaceViewModel>()
                       .ForMember(dto => dto.IdPlace, opt => opt.MapFrom(src => src.IdPlace))
                          .ForMember(dto => dto.PriceTicket, opt => opt.MapFrom(src => src.PriceTicket))
                               .ForMember(dto => dto.ContractId, opt => opt.MapFrom(src => src.ContractId))
                          .ForMember(dto => dto.ModifyBy, opt => opt.MapFrom(src => src.ModifyBy))
                         .ForMember(dto => dto.ModifyDate, opt => opt.MapFrom(src => src.ModifyDate))
                         .ForMember(dto => dto.Phone, opt => opt.MapFrom(src => src.Phone))
                         .ForMember(dto => dto.Address, opt => opt.MapFrom(src => src.Address))
                         .ForMember(dto => dto.Name, opt => opt.MapFrom(src => src.NamePlace))
                  .ForMember(dto => dto.TypeAction, opt => opt.MapFrom(src => src.TypeAction))
                                         .ForMember(dto => dto.ProvinceId, opt => opt.MapFrom(src => src.ProvinceId))
                               .ForMember(dto => dto.WardId, opt => opt.MapFrom(src => src.WardId))
                              .ForMember(dto => dto.DistrictId, opt => opt.MapFrom(src => src.DistrictId))
                // create 
                ;
                // create Place
                cfg.CreateMap<CreatePlaceViewModel, Place>()
           .ForMember(dto => dto.IdPlace, opt => opt.MapFrom(src => Guid.NewGuid()))
           .ForMember(dto => dto.PriceTicket, opt => opt.MapFrom(src => src.PriceTicket))
                              .ForMember(dto => dto.ModifyBy, opt => opt.MapFrom(src => src.ModifyBy))
                              .ForMember(dto => dto.Phone, opt => opt.MapFrom(src => src.Phone))
                              .ForMember(dto => dto.Approve, opt => opt.MapFrom(src => ApproveStatus.Waiting))
                              .ForMember(dto => dto.Address, opt => opt.MapFrom(src => src.Address))
                              .ForMember(dto => dto.NamePlace, opt => opt.MapFrom(src => src.Name))
                                         .ForMember(dto => dto.IsTempdata, opt => opt.MapFrom(src => false))
                                         .ForMember(dto => dto.IdUserModify, opt => opt.MapFrom(src => src.IdUserModify))
                                         .ForMember(dto => dto.TypeAction, opt => opt.MapFrom(src => src.TypeAction))
                                         .ForMember(dto => dto.ModifyDate, opt => opt.MapFrom(src => src.ModifyDate))
                                         .ForMember(dto => dto.IsDelete, opt => opt.MapFrom(src => false))

                                               .ForMember(dto => dto.ProvinceId, opt => opt.MapFrom(src => src.ProvinceId))
                               .ForMember(dto => dto.WardId, opt => opt.MapFrom(src => src.WardId))
                              .ForMember(dto => dto.DistrictId, opt => opt.MapFrom(src => src.DistrictId))
           ;
                // create restaurant
                cfg.CreateMap<CreateRestaurantViewModel, Restaurant>()
           .ForMember(dto => dto.IdRestaurant, opt => opt.MapFrom(src => Guid.NewGuid()))
                              .ForMember(dto => dto.ModifyBy, opt => opt.MapFrom(src => src.ModifyBy))
                              .ForMember(dto => dto.ModifyDate, opt => opt.MapFrom(src => 0))
                              .ForMember(dto => dto.Phone, opt => opt.MapFrom(src => src.Phone))
                              .ForMember(dto => dto.Address, opt => opt.MapFrom(src => src.Address))
                              .ForMember(dto => dto.NameRestaurant, opt => opt.MapFrom(src => src.Name))
                              .ForMember(dto => dto.ComboPrice, opt => opt.MapFrom(src => src.ComboPrice))
                              .ForMember(dto => dto.Approve, opt => opt.MapFrom(src => ApproveStatus.Waiting))
                              .ForMember(dto => dto.IsTempdata, opt => opt.MapFrom(src => false))
                              .ForMember(dto => dto.IsDelete, opt => opt.MapFrom(src => false))
                              .ForMember(dto => dto.IdUserModify, opt => opt.MapFrom(src => src.IdUserModify))
                              .ForMember(dto => dto.TypeAction, opt => opt.MapFrom(src => src.TypeAction))
                              .ForMember(dto => dto.ModifyDate, opt => opt.MapFrom(src => src.ModifyDate))
                                 .ForMember(dto => dto.ProvinceId, opt => opt.MapFrom(src => src.ProvinceId))
                               .ForMember(dto => dto.WardId, opt => opt.MapFrom(src => src.WardId))
                              .ForMember(dto => dto.DistrictId, opt => opt.MapFrom(src => src.DistrictId))
               ;
                //view restaurant
                cfg.CreateMap<Restaurant, RestaurantViewModel>()
           .ForMember(dto => dto.IdRestaurant, opt => opt.MapFrom(src => src.IdRestaurant))
                              .ForMember(dto => dto.ModifyBy, opt => opt.MapFrom(src => src.ModifyBy))
                              .ForMember(dto => dto.ModifyDate, opt => opt.MapFrom(src => src.ModifyDate))
                              .ForMember(dto => dto.Phone, opt => opt.MapFrom(src => src.Phone))
                              .ForMember(dto => dto.Address, opt => opt.MapFrom(src => src.Address))
                              .ForMember(dto => dto.Name, opt => opt.MapFrom(src => src.NameRestaurant))
                                                       .ForMember(dto => dto.ProvinceId, opt => opt.MapFrom(src => src.ProvinceId))
                               .ForMember(dto => dto.WardId, opt => opt.MapFrom(src => src.WardId))
                              .ForMember(dto => dto.DistrictId, opt => opt.MapFrom(src => src.DistrictId))
               ;
                // create hotel
                cfg.CreateMap<CreateHotelViewModel, Hotel>()
           .ForMember(dto => dto.IdHotel, opt => opt.MapFrom(src => Guid.NewGuid()))
               .ForMember(dto => dto.Star, opt => opt.MapFrom(src => src.Star))
               .ForMember(dto => dto.QuantitySR, opt => opt.MapFrom(src => src.QuantitySR))
               .ForMember(dto => dto.QuantityDBR, opt => opt.MapFrom(src => src.QuantityDBR))
               .ForMember(dto => dto.SingleRoomPrice, opt => opt.MapFrom(src => src.SingleRoomPrice))
               .ForMember(dto => dto.DoubleRoomPrice, opt => opt.MapFrom(src => src.DoubleRoomPrice))
               .ForMember(dto => dto.Approve, opt => opt.MapFrom(src => ApproveStatus.Waiting))
               .ForMember(dto => dto.IsTempdata, opt => opt.MapFrom(src => false))
               .ForMember(dto => dto.IsDelete, opt => opt.MapFrom(src => false))
               .ForMember(dto => dto.IdUserModify, opt => opt.MapFrom(src => src.IdUserModify))
                              .ForMember(dto => dto.ModifyBy, opt => opt.MapFrom(src => src.ModifyBy))
                                .ForMember(dto => dto.TypeAction, opt => opt.MapFrom(src => src.TypeAction))
                              .ForMember(dto => dto.ModifyDate, opt => opt.MapFrom(src => src.ModifyDate))
                              .ForMember(dto => dto.Phone, opt => opt.MapFrom(src => src.Phone))
                              .ForMember(dto => dto.Address, opt => opt.MapFrom(src => src.Address))
                              .ForMember(dto => dto.NameHotel, opt => opt.MapFrom(src => src.Name))
                              .ForMember(dto => dto.ProvinceId, opt => opt.MapFrom(src => src.ProvinceId))
                                                            .ForMember(dto => dto.ProvinceId, opt => opt.MapFrom(src => src.ProvinceId))
                              .ForMember(dto => dto.ProvinceId, opt => opt.MapFrom(src => src.ProvinceId))
                               .ForMember(dto => dto.WardId, opt => opt.MapFrom(src => src.WardId))
                              .ForMember(dto => dto.DistrictId, opt => opt.MapFrom(src => src.DistrictId))


                    ;

                cfg.CreateMap<UpdateHotelViewModel, Hotel>()
                   .ForMember(dto => dto.Star, opt => opt.MapFrom(src => src.Star))
                .ForMember(dto => dto.QuantitySR, opt => opt.MapFrom(src => src.QuantitySR))
                .ForMember(dto => dto.QuantityDBR, opt => opt.MapFrom(src => src.QuantityDBR))
                .ForMember(dto => dto.SingleRoomPrice, opt => opt.MapFrom(src => src.SingleRoomPrice))
                .ForMember(dto => dto.DoubleRoomPrice, opt => opt.MapFrom(src => src.DoubleRoomPrice))
                 .ForMember(dto => dto.Phone, opt => opt.MapFrom(src => src.Phone))
                              .ForMember(dto => dto.Address, opt => opt.MapFrom(src => src.Address))
                              .ForMember(dto => dto.NameHotel, opt => opt.MapFrom(src => src.Name))
                                .ForMember(dto => dto.TypeAction, opt => opt.MapFrom(src => src.TypeAction))
                              .ForMember(dto => dto.ModifyDate, opt => opt.MapFrom(src => src.ModifyDate));

                //view hotel
                cfg.CreateMap<Hotel, HotelViewModel>()
           .ForMember(dto => dto.IdHotel, opt => opt.MapFrom(src => src.IdHotel))
               .ForMember(dto => dto.Star, opt => opt.MapFrom(src => src.Star))
               .ForMember(dto => dto.QuantitySR, opt => opt.MapFrom(src => src.QuantitySR))
               .ForMember(dto => dto.QuantityDBR, opt => opt.MapFrom(src => src.QuantityDBR))
               .ForMember(dto => dto.SingleRoomPrice, opt => opt.MapFrom(src => src.SingleRoomPrice))
               .ForMember(dto => dto.DoubleRoomPrice, opt => opt.MapFrom(src => src.DoubleRoomPrice))

                              .ForMember(dto => dto.ModifyBy, opt => opt.MapFrom(src => src.ModifyBy))
                              .ForMember(dto => dto.ModifyDate, opt => opt.MapFrom(src => src.ModifyDate))
                              .ForMember(dto => dto.Phone, opt => opt.MapFrom(src => src.Phone))
                              .ForMember(dto => dto.Address, opt => opt.MapFrom(src => src.Address))
                              .ForMember(dto => dto.Name, opt => opt.MapFrom(src => src.NameHotel))
                              .ForMember(dto => dto.TypeAction, opt => opt.MapFrom(src => src.TypeAction))
                              .ForMember(dto => dto.Approve, opt => opt.MapFrom(src => src.Approve))
                                                       .ForMember(dto => dto.ProvinceId, opt => opt.MapFrom(src => src.ProvinceId))
                               .ForMember(dto => dto.WardId, opt => opt.MapFrom(src => src.WardId))
                              .ForMember(dto => dto.DistrictId, opt => opt.MapFrom(src => src.DistrictId))
               ;
                // create costtour
                cfg.CreateMap<CreateCostViewModel, CostTour>()
               .ForMember(dto => dto.Breakfast, opt => opt.MapFrom(src => src.Breakfast))
               .ForMember(dto => dto.Water, opt => opt.MapFrom(src => src.Water))
               .ForMember(dto => dto.FeeGas, opt => opt.MapFrom(src => src.FeeGas))
               .ForMember(dto => dto.Distance, opt => opt.MapFrom(src => src.Distance))
               .ForMember(dto => dto.SellCost, opt => opt.MapFrom(src => src.SellCost))
               .ForMember(dto => dto.Depreciation, opt => opt.MapFrom(src => src.Depreciation))
               .ForMember(dto => dto.OtherPrice, opt => opt.MapFrom(src => src.OtherPrice))
               .ForMember(dto => dto.Tolls, opt => opt.MapFrom(src => src.Tolls))
               .ForMember(dto => dto.CusExpected, opt => opt.MapFrom(src => src.CusExpected))
               .ForMember(dto => dto.InsuranceFee, opt => opt.MapFrom(src => src.InsuranceFee))
                    .ForMember(dto => dto.HotelId, opt => opt.MapFrom(src => src.HotelId))
               .ForMember(dto => dto.RestaurantId, opt => opt.MapFrom(src => src.RestaurantId))
               .ForMember(dto => dto.PlaceId, opt => opt.MapFrom(src => src.PlaceId))
               .ForMember(dto => dto.TotalCostTourNotService, opt => opt.MapFrom(src =>
                       (src.Breakfast + src.Water + src.SellCost + src.OtherPrice + src.InsuranceFee) + (src.Depreciation + src.Tolls + (src.FeeGas * (src.Distance / 10)))
               ))
               ;

                // view history customer
                cfg.CreateMap<TourBooking, HistoryBookedViewModel>()
           .ForMember(dto => dto.IdSchedule, opt => opt.MapFrom(src => src.ScheduleId))
           .ForMember(dto => dto.IdTourBooking, opt => opt.MapFrom(src => src.IdTourBooking))
           .ForMember(dto => dto.DepartureDate, opt => opt.MapFrom(src => src.Schedule.DepartureDate))
           .ForMember(dto => dto.ReturnDate, opt => opt.MapFrom(src => src.Schedule.ReturnDate))
           .ForMember(dto => dto.Thumbnail, opt => opt.MapFrom(src => src.Schedule.Tour.Thumbnail))
           .ForMember(dto => dto.TotalPrice, opt => opt.MapFrom(src => src.ValuePromotion == 0 ? src.TotalPrice : src.TotalPricePromotion))
           .ForMember(dto => dto.ValuePromotion, opt => opt.MapFrom(src => src.ValuePromotion))
           .ForMember(dto => dto.DateBooking, opt => opt.MapFrom(src => src.DateBooking))
           .ForMember(dto => dto.BookingNo, opt => opt.MapFrom(src => src.BookingNo))
           .ForMember(dto => dto.Status, opt => opt.MapFrom(src => src.Status))
           .ForMember(dto => dto.Description, opt => opt.MapFrom(src => src.Schedule.Description))
           .ForMember(dto => dto.NameTour, opt => opt.MapFrom(src => src.Schedule.Tour.NameTour))
           .ForMember(dto => dto.Adult, opt => opt.MapFrom(src => src.TourBookingDetails.Adult))
           .ForMember(dto => dto.Child, opt => opt.MapFrom(src => src.TourBookingDetails.Child))
           .ForMember(dto => dto.Baby, opt => opt.MapFrom(src => src.TourBookingDetails.Baby))
           .ForMember(dto => dto.FromPlace, opt => opt.MapFrom(src => src.Schedule.DeparturePlace))
           .ForMember(dto => dto.ToPlace, opt => opt.MapFrom(src => src.Schedule.Tour.ToPlace))
           .ForMember(dto => dto.IsSendFeedBack, opt => opt.MapFrom(src => src.IsSendFeedBack))
           ;

                //view costtour
                cfg.CreateMap<CostTour, CostTourViewModel>()
            .ForMember(dto => dto.Breakfast, opt => opt.MapFrom(src => src.Breakfast))
            .ForMember(dto => dto.Water, opt => opt.MapFrom(src => src.Water))
            .ForMember(dto => dto.FeeGas, opt => opt.MapFrom(src => src.FeeGas))
            .ForMember(dto => dto.Distance, opt => opt.MapFrom(src => src.Distance))
            .ForMember(dto => dto.SellCost, opt => opt.MapFrom(src => src.SellCost))
            .ForMember(dto => dto.Depreciation, opt => opt.MapFrom(src => src.Depreciation))
            .ForMember(dto => dto.OtherPrice, opt => opt.MapFrom(src => src.OtherPrice))
            .ForMember(dto => dto.Tolls, opt => opt.MapFrom(src => src.Tolls))
            .ForMember(dto => dto.CusExpected, opt => opt.MapFrom(src => src.CusExpected))
            .ForMember(dto => dto.InsuranceFee, opt => opt.MapFrom(src => src.InsuranceFee))
            .ForMember(dto => dto.IsHoliday, opt => opt.MapFrom(src => src.IsHoliday))
                 .ForMember(dto => dto.HotelId, opt => opt.MapFrom(src => src.HotelId))
            .ForMember(dto => dto.RestaurantId, opt => opt.MapFrom(src => src.RestaurantId))
            .ForMember(dto => dto.PlaceId, opt => opt.MapFrom(src => src.PlaceId))
            .ForMember(dto => dto.TotalCostTourNotService, opt => opt.MapFrom(src => src.TotalCostTourNotService))
            .ForMember(dto => dto.NameHotel, opt => opt.MapFrom(src => src.Hotel.NameHotel))
            .ForMember(dto => dto.PriceSRHotel, opt => opt.MapFrom(src => src.Hotel.SingleRoomPrice))
            .ForMember(dto => dto.PriceDBHotel, opt => opt.MapFrom(src => src.Hotel.DoubleRoomPrice))
            .ForMember(dto => dto.NameRestaurant, opt => opt.MapFrom(src => src.Restaurant.NameRestaurant))
            .ForMember(dto => dto.PriceComboRestaurant, opt => opt.MapFrom(src => src.Restaurant.ComboPrice))
            .ForMember(dto => dto.NamePlace, opt => opt.MapFrom(src => src.Place.NamePlace))
            .ForMember(dto => dto.PriceTicketPlace, opt => opt.MapFrom(src => src.Place.PriceTicket))
            ;

                cfg.CreateMap<UpdateCostViewModel, CostTour>()
                .ForMember(dto => dto.IdSchedule, opt => opt.MapFrom(src => src.IdSchedule))
                .ForMember(dto => dto.Breakfast, opt => opt.MapFrom(src => src.Breakfast))
                .ForMember(dto => dto.Water, opt => opt.MapFrom(src => src.Water))
                .ForMember(dto => dto.FeeGas, opt => opt.MapFrom(src => src.FeeGas))
                .ForMember(dto => dto.Distance, opt => opt.MapFrom(src => src.Distance))
                .ForMember(dto => dto.SellCost, opt => opt.MapFrom(src => src.SellCost))
                .ForMember(dto => dto.Depreciation, opt => opt.MapFrom(src => src.Depreciation))
                .ForMember(dto => dto.OtherPrice, opt => opt.MapFrom(src => src.OtherPrice))
                .ForMember(dto => dto.Tolls, opt => opt.MapFrom(src => src.Tolls))
                .ForMember(dto => dto.CusExpected, opt => opt.MapFrom(src => src.CusExpected))
                .ForMember(dto => dto.InsuranceFee, opt => opt.MapFrom(src => src.InsuranceFee))
                .ForMember(dto => dto.HotelId, opt => opt.MapFrom(src => src.HotelId))
                .ForMember(dto => dto.RestaurantId, opt => opt.MapFrom(src => src.RestaurantId))
                .ForMember(dto => dto.PlaceId, opt => opt.MapFrom(src => src.PlaceId))
                .ForMember(dto => dto.IsHoliday, opt => opt.MapFrom(src => src.IsHoliday))

                .ForMember(dto => dto.TotalCostTourNotService, opt => opt.MapFrom(src =>
                       (src.FeeGas * (src.Distance / 10)) + (src.Breakfast + src.Water + src.SellCost + src.OtherPrice + src.InsuranceFee + src.Depreciation + src.Tolls)
                ))
                ;
                // review
                cfg.CreateMap<CreateReviewModel, Review>()
                             .ForMember(dto => dto.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
                                   .ForMember(dto => dto.Rating, opt => opt.MapFrom(src => src.Rating));
                cfg.CreateMap<Review, ReviewViewModel>()
                  .ForMember(dto => dto.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
                                   .ForMember(dto => dto.Rating, opt => opt.MapFrom(src => src.Rating))
                                   ;
                cfg.CreateMap<CreateVoucherViewModel, Voucher>()
                                .ForMember(dto => dto.IdVoucher, opt => opt.MapFrom(src => Guid.NewGuid()))
                                   .ForMember(dto => dto.Code, opt => opt.MapFrom(src => src.Code))
                                   .ForMember(dto => dto.Value, opt => opt.MapFrom(src => src.Value))
                                   .ForMember(dto => dto.StartDate, opt => opt.MapFrom(src => src.StartDate))
                                   .ForMember(dto => dto.EndDate, opt => opt.MapFrom(src => src.EndDate));


                cfg.CreateMap<Voucher, VoucherViewModel>()
                                    .ForMember(dto => dto.IdVoucher, opt => opt.MapFrom(src => src.IdVoucher))
                                    .ForMember(dto => dto.Code, opt => opt.MapFrom(src => src.Code))
                                    .ForMember(dto => dto.Value, opt => opt.MapFrom(src => src.Value))
                                    .ForMember(dto => dto.StartDate, opt => opt.MapFrom(src => src.StartDate))
                                     .ForMember(dto => dto.EndDate, opt => opt.MapFrom(src => src.EndDate));

                cfg.CreateMap<UpdateVoucherViewModel, Voucher>()
                                  .ForMember(dto => dto.Value, opt => opt.MapFrom(src => src.Value))
                                  .ForMember(dto => dto.Code, opt => opt.MapFrom(src => src.Code))
                                  .ForMember(dto => dto.StartDate, opt => opt.MapFrom(src => src.StartDate))
                                  .ForMember(dto => dto.EndDate, opt => opt.MapFrom(src => src.EndDate))
                                  ;

            });
            _mapper = mapperConfiguration.CreateMapper();
        }
        public static Voucher MapCreateVoucher(CreateVoucherViewModel data)
        {
            return _mapper.Map<CreateVoucherViewModel, Voucher>(data);
        }
        public static VoucherViewModel MapVoucher(Voucher data)
        {
            return _mapper.Map<Voucher, VoucherViewModel>(data);
        }
        public static List<VoucherViewModel> MapVoucher(List<Voucher> data)
        {
            return _mapper.Map<List<Voucher>, List<VoucherViewModel>>(data);
        }
        public static Voucher MapUpdateVoucher(UpdateVoucherViewModel data)
        {
            return _mapper.Map<UpdateVoucherViewModel, Voucher>(data);
        }
        public static RoleViewModel MapRole(Role data)
        {
            return _mapper.Map<Role, RoleViewModel>(data);
        }

        // view history customer
        public static List<HistoryBookedViewModel> MapHistoryCustomerViewModel(List<TourBooking> data)
        {
            return _mapper.Map<List<TourBooking>, List<HistoryBookedViewModel>>(data);
        }

        public static List<PlaceViewModel> MapPlace(List<Place> data)
        {
            return _mapper.Map<List<Place>, List<PlaceViewModel>>(data);
        }
        public static PaymentViewModel MapPayment(Payment data)
        {
            return _mapper.Map<Payment, PaymentViewModel>(data);
        }
        public static List<PaymentViewModel> MapPayment(List<Payment> data)
        {
            return _mapper.Map<List<Payment>, List<PaymentViewModel>>(data);
        }
        public static Payment MapCreatePayment(CreatePaymentViewModel data)
        {
            return _mapper.Map<CreatePaymentViewModel, Payment>(data);
        }
        public static EmployeeViewModel MapEmployee(Employee data)
        {
            return _mapper.Map<Employee, EmployeeViewModel>(data);
        }

        //map role
        public static List<RoleViewModel> MapRole(List<Role> data)
        {
            return _mapper.Map<List<Role>, List<RoleViewModel>>(data);
        }

        public static Role MapCreateRole(CreateRoleViewModel data)
        {
            return _mapper.Map<CreateRoleViewModel, Role>(data);
        }

        public static Role MapUpdateRole(UpdateRoleViewModel data)
        {
            return _mapper.Map<UpdateRoleViewModel, Role>(data);
        }

        //map promotion
        public static Promotion MapCreatePromotion(CreatePromotionViewModel data)
        {
            return _mapper.Map<CreatePromotionViewModel, Promotion>(data);
        }
        public static List<PromotionViewModel> MapPromotion(List<Promotion> data)
        {
            return _mapper.Map<List<Promotion>, List<PromotionViewModel>>(data);
        }
        public static PromotionViewModel MapPromotion(Promotion data)
        {
            return _mapper.Map<Promotion, PromotionViewModel>(data);
        }
        public static Promotion MapUpdatePromotion(UpdatePromotionViewModel data)
        {
            return _mapper.Map<UpdatePromotionViewModel, Promotion>(data);
        }



        public static Employee MapCreateEmployee(CreateEmployeeViewModel data)
        {
            return _mapper.Map<CreateEmployeeViewModel, Employee>(data);
        }

        public static List<EmployeeViewModel> MapEmployee(List<Employee> data)
        {
            return _mapper.Map<List<Employee>, List<EmployeeViewModel>>(data);
        }
        public static Employee MapUpdateEmployee(UpdateEmployeeViewModel data)
        {
            return _mapper.Map<UpdateEmployeeViewModel, Employee>(data);
        }
        public static Tour MapCreateTour(CreateTourViewModel data)
        {
            return _mapper.Map<CreateTourViewModel, Tour>(data);
        }
        public static Tour MapUpdateTour(UpdateTourViewModel data)
        {
            return _mapper.Map<UpdateTourViewModel, Tour>(data);
        }
        public static List<TourViewModel> MapTour(List<Tour> data)
        {
            return _mapper.Map<List<Tour>, List<TourViewModel>>(data);
        }
        public static TourViewModel MapTour(Tour data)
        {
            return _mapper.Map<Tour, TourViewModel>(data);
        }
        public static Schedule MapCreateSchedule(CreateScheduleViewModel data)
        {
            return _mapper.Map<CreateScheduleViewModel, Schedule>(data);
        }

        public static Schedule MapUpdateSchedule(UpdateScheduleViewModel data)
        {
            return _mapper.Map<UpdateScheduleViewModel, Schedule>(data);
        }

        public static List<ScheduleViewModel> MapSchedule(List<Schedule> data)
        {
            return _mapper.Map<List<Schedule>, List<ScheduleViewModel>>(data);
        }


        public static Customer MapCreateCustomer(CreateCustomerViewModel data)
        {
            return _mapper.Map<CreateCustomerViewModel, Customer>(data);
        }
        public static Customer MapUpdateCustomer(UpdateCustomerViewModel data)
        {
            return _mapper.Map<UpdateCustomerViewModel, Customer>(data);
        }
        public static List<CustomerViewModel> MapCustomer(List<Customer> data)
        {
            return _mapper.Map<List<Customer>, List<CustomerViewModel>>(data);
        }
        public static CustomerViewModel MapCustomer(Customer data)
        {
            return _mapper.Map<Customer, CustomerViewModel>(data);
        }

        public static List<ProvinceViewModel> MapProvince(List<Province> data)
        {
            return _mapper.Map<List<Province>, List<ProvinceViewModel>>(data);
        }
        public static Province MapCreateProvince(CreateProvinceViewModel data)
        {
            return _mapper.Map<CreateProvinceViewModel, Province>(data);
        }
        public static Province MapUpdateProvince(UpdateProvinceViewModel data)
        {
            return _mapper.Map<UpdateProvinceViewModel, Province>(data);
        }
        public static List<DistrictViewModel> MapDistrict(List<District> data)
        {
            return _mapper.Map<List<District>, List<DistrictViewModel>>(data);
        }

        public static District MapCreateDistrict(CreateDistrictViewModel data)
        {
            return _mapper.Map<CreateDistrictViewModel, District>(data);
        }
        public static District MapUpdateDistrict(UpdateDistrictViewModel data)
        {
            return _mapper.Map<UpdateDistrictViewModel, District>(data);
        }

        public static List<WardViewModel> MapWard(List<Ward> data)
        {
            return _mapper.Map<List<Ward>, List<WardViewModel>>(data);
        }

        public static Ward MapCreateWard(CreateWardViewModel data)
        {
            return _mapper.Map<CreateWardViewModel, Ward>(data);
        }
        public static Ward MapUpdateWard(UpdateWardViewModel data)
        {
            return _mapper.Map<UpdateWardViewModel, Ward>(data);
        }

        public static Car MapCreateCar(CreateCarViewModel data)
        {
            return _mapper.Map<CreateCarViewModel, Car>(data);
        }
        public static List<CarViewModel> MapCar(List<Car> data)
        {
            return _mapper.Map<List<Car>, List<CarViewModel>>(data);
        }
        public static CarViewModel MapCar(Car data)
        {
            return _mapper.Map<Car, CarViewModel>(data);
        }


        public static List<TimeLineViewModel> MapTimeLine(List<Timeline> data)
        {
            return _mapper.Map<List<Timeline>, List<TimeLineViewModel>>(data);
        }
        public static TimeLineViewModel MapTimeLine(Timeline data)
        {
            return _mapper.Map<Timeline, TimeLineViewModel>(data);
        }
        public static Timeline MapCreateTimeline(CreateTimeLineViewModel data)
        {
            return _mapper.Map<CreateTimeLineViewModel, Timeline>(data);
        }
        public static ICollection<Timeline> MapCreateTimeline(ICollection<CreateTimeLineViewModel> data)
        {
            return _mapper.Map<ICollection<CreateTimeLineViewModel>, ICollection<Timeline>>(data);
        }

        public static ICollection<Timeline> MapUpdateTimeline(ICollection<UpdateTimeLineViewModel> data)
        {
            return _mapper.Map<ICollection<UpdateTimeLineViewModel>, ICollection<Timeline>>(data);
        }

        public static List<TourBookingViewModel> MapTourBooking(List<TourBooking> data)
        {
            return _mapper.Map<List<TourBooking>, List<TourBookingViewModel>>(data);
        }

        // create restaurant

        public static Restaurant MapCreateRestaurant(CreateRestaurantViewModel data)
        {
            return _mapper.Map<CreateRestaurantViewModel, Restaurant>(data);
        }

        public static List<RestaurantViewModel> MapRestaurant(List<Restaurant> data)
        {
            return _mapper.Map<List<Restaurant>, List<RestaurantViewModel>>(data);
        }
        // review
        public static Review MapCreateReview(CreateReviewModel data)
        {
            return _mapper.Map<CreateReviewModel, Review>(data);
        }
        public static Review MapUpdateReview(UpdateReviewModel data)
        {
            return _mapper.Map<UpdateReviewModel, Review>(data);
        }
        public static List<ReviewViewModel> MapReview(List<Review> data)
        {
            return _mapper.Map<List<Review>, List<ReviewViewModel>>(data);
        }
        public static ReviewViewModel MapReview(Review data)
        {
            return _mapper.Map<Review, ReviewViewModel>(data);
        }
        // create hotel

        public static Hotel MapCreateHotel(CreateHotelViewModel data)
        {
            return _mapper.Map<CreateHotelViewModel, Hotel>(data);
        }
        public static Hotel MapUpdateHotel(UpdateHotelViewModel data)
        {
            return _mapper.Map<UpdateHotelViewModel, Hotel>(data);
        }
        public static List<HotelViewModel> MapHotel(List<Hotel> data)
        {
            return _mapper.Map<List<Hotel>, List<HotelViewModel>>(data);
        }
        public static HotelViewModel MapHotel(Hotel data)
        {
            return _mapper.Map<Hotel, HotelViewModel>(data);
        }
        // create place

        public static Place MapCreatePlace(CreatePlaceViewModel data)
        {
            return _mapper.Map<CreatePlaceViewModel, Place>(data);
        }

        // create tourbooking
        public static TourBooking MapCreateTourBooking(CreateTourBookingViewModel data)
        {
            return _mapper.Map<CreateTourBookingViewModel, TourBooking>(data);
        }
        // create tourbookingDetail
        public static TourBookingDetails MapCreateTourBookingDetail(CreateBookingDetailViewModel data)
        {
            return _mapper.Map<CreateBookingDetailViewModel, TourBookingDetails>(data);
        }

        // Create  cost
        public static CostTour MapCreateCost(CreateCostViewModel data)
        {
            return _mapper.Map<CreateCostViewModel, CostTour>(data);
        }
        public static List<CostTourViewModel> MapCost(List<CostTour> data)
        {
            return _mapper.Map<List<CostTour>, List<CostTourViewModel>>(data);
        }
        public static CostTourViewModel MapCost(CostTour data)
        {
            return _mapper.Map<CostTour, CostTourViewModel>(data);
        }
        public static CostTour MapUpdateCost(UpdateCostViewModel data)
        {
            return _mapper.Map<UpdateCostViewModel, CostTour>(data);
        }
        public static Contract MapCreateContract(CreateContractViewModel data)
        {
            return _mapper.Map<CreateContractViewModel, Contract>(data);
        }

    }
}
