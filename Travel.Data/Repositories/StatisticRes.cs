using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Travel.Context.Models.Notification;
using Travel.Context.Models.Travel;
using Travel.Data.Interfaces;
using Travel.Shared.Ultilities;
using Travel.Shared.ViewModels;

namespace Travel.Data.Repositories
{
    public class StatisticRes : IStatistic
    {
        private readonly TravelContext _db;
        private readonly NotificationContext _dbNotyf;
        public StatisticRes(TravelContext db, NotificationContext dbNotyf)
        {
            _db = db;
            _dbNotyf = dbNotyf;
        }
        private void UpdateDatabase<T>(T input)
        {
            _dbNotyf.Entry(input).State = EntityState.Modified;
        }
        private void CreateDatabase<T>(T input)
        {
            _dbNotyf.Entry(input).State = EntityState.Added;
        }
        private async Task SaveChangeAsync()
        {
            await _dbNotyf.SaveChangesAsync();
        }
        public async Task<bool> SaveReportTourBookingEveryDay(DateTime dateInput)
        {
            var dateTimeYesterday = DateTime.Now.AddDays(-1);
            var day = dateTimeYesterday.Day;
            var month = dateTimeYesterday.Month;
            var year = dateTimeYesterday.Year;
            var input = Ultility.ConvertDatetimeToUnixTimeStampMiliSecond(dateInput);
            var yesterday = DateTime.Parse($"{year}/{month}/{day}");
            var unixEndOfYesterday = Ultility.ConvertDatetimeToUnixTimeStampMiliSecond(yesterday.AddDays(1).AddMinutes(-1));
            var unixYesterday = Ultility.ConvertDatetimeToUnixTimeStampMiliSecond(yesterday);
            using var transaction = _dbNotyf.Database.BeginTransaction();
            try
            {
                await transaction.CreateSavepointAsync("BeforeSave");
                if (input == unixYesterday)
                {
                    if (!await CheckReportByDateIsExist(unixYesterday))
                    {
                        var listTourBookingFinished =await (from tbk in _db.TourBookings.AsNoTracking()
                                                       join s in _db.Schedules.AsNoTracking()
                                                       on tbk.ScheduleId equals s.IdSchedule
                                                       where tbk.Status == (int)Enums.StatusBooking.Finished
                                                       && (s.ReturnDate >= unixYesterday && s.ReturnDate <= unixEndOfYesterday)
                                                       select tbk).ToListAsync() ;

                        var listGroupingTourbooking = listTourBookingFinished.GroupBy(x => x.ScheduleId);
                        foreach (var item in listGroupingTourbooking)
                        {
                            var schedule = await (from s in _db.Schedules.AsNoTracking()
                                                  join t in _db.Tour.AsNoTracking()
                                                  on s.TourId equals t.IdTour
                                                  where s.IdSchedule == item.Key
                                                  select new { t.NameTour, t.IdTour }).FirstOrDefaultAsync();
                            var costTour = await (from s in _db.Schedules.AsNoTracking()
                                                  where s.IdSchedule == item.Key
                                                  select s.TotalCostTour).FirstOrDefaultAsync();
                            long sumCostTour = (int)costTour * item.Count();
                            var sumNormalPrice = (long)item.Sum(x => x.TotalPrice);
                            var sumNormalPricePromotion = (long)item.Sum(x => x.TotalPricePromotion);
                            ReportTourBooking obj = new ReportTourBooking
                            {
                                DateSave = unixYesterday,
                                IdReportTourBooking = Guid.NewGuid(),
                                IdTour = schedule.IdTour,
                                NameTour = schedule.NameTour,
                                QuantityBooked = item.Count(),
                                TotalRevenue = (sumNormalPrice + sumNormalPricePromotion),
                                TotalCost = sumCostTour
                            };
                            CreateDatabase(obj);


                        }

                        await SaveChangeAsync();
                    }

                }
                transaction.Commit();
                transaction.Dispose();
                return true;
            }
            catch (Exception e)
            {
                transaction.RollbackToSavepoint("BeforeSave");
                return false;
            }

        }
        private async Task<bool> CheckReportByDateIsExist(long dateSave)
        {
            var obj = await (from x in _dbNotyf.ReportTourBooking.AsNoTracking()
                             where x.DateSave == dateSave
                             select x).FirstOrDefaultAsync();
            if (obj != null)
            {
                return true;
            }
            return false;
        }
        public Response StatisticTourBookingFromDateToDate(long fromDate, long toDate)
        {
            try
            {
                var lsReportTourBooking = (from x in _dbNotyf.ReportTourBooking.AsNoTracking()
                                           where x.DateSave >= fromDate
                                           && x.DateSave <= toDate
                                           select x).ToList();
                return Ultility.Responses("", Enums.TypeCRUD.Success.ToString(), lsReportTourBooking);
            }
            catch (Exception e)
            {
                return Ultility.Responses("Có lỗi xảy ra !", Enums.TypeCRUD.Error.ToString(), description: e.Message);
            }
        }

        public Response StatisticTourBookingInThisWeek(long fromDate,long toDate)
        {
            try
            {
                var lsStatisticByWeek = (from x in _dbNotyf.ReportTourBooking.AsNoTracking()
                                         where x.DateSave >= fromDate
                                         && x.DateSave <= toDate
                                         select x).ToList();
                return Ultility.Responses("", Enums.TypeCRUD.Success.ToString(),lsStatisticByWeek);
            }
            catch (Exception e)
            {

                return Ultility.Responses("Có lỗi xảy ra !", Enums.TypeCRUD.Error.ToString(), description: e.Message);
            }
        }
        private int GetWeekNumber(DateTime now)
        {
            CultureInfo ciCurr = CultureInfo.CurrentCulture;
            int weekNum = ciCurr.Calendar.GetWeekOfYear(now, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
            return weekNum;
        }
        public  DateTime FirstDateOfWeekISO8601(int year, int weekOfYear)
        {
            DateTime jan1 = new DateTime(year, 1, 1);
            int daysOffset = DayOfWeek.Thursday - jan1.DayOfWeek;

            // Use first Thursday in January to get first week of the year as
            // it will never be in Week 52/53
            DateTime firstThursday = jan1.AddDays(daysOffset);
            var cal = CultureInfo.CurrentCulture.Calendar;
            int firstWeek = cal.GetWeekOfYear(firstThursday, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);

            var weekNum = weekOfYear;
            // As we're adding days to a date in Week 1,
            // we need to subtract 1 in order to get the right date for week #1
            if (firstWeek == 1)
            {
                weekNum -= 1;
            }

            // Using the first Thursday as starting week ensures that we are starting in the right year
            // then we add number of weeks multiplied with days
            var result = firstThursday.AddDays(weekNum * 7);

            // Subtract 3 days from Thursday to get Monday, which is the first weekday in ISO8601
            return result.AddDays(-3);
        }
        private async Task<bool> IsWeekExists(int week, int year)
        {
            try
            {
                var weekNumber = await (from x in _dbNotyf.ReportWeek.AsNoTracking()
                                  where x.Week == week
                                  && x.Year == year
                                  select x).CountAsync();
                if (weekNumber >0)
                {
                    return true;
                }
                return false;
            }
            catch 
            {
                return false;
            }
        }
        public async Task SaveReportWeek()
        {
            var now = DateTime.Now;
            var year = now.Year;
            int weekNumber = GetWeekNumber(now);
            if (!await IsWeekExists(weekNumber,year))
            {
                var firstDateOfWeek = FirstDateOfWeekISO8601(year, weekNumber);
                var endDateOfWeek = firstDateOfWeek.AddDays(6);

                ReportWeek obj = new ReportWeek
                {
                    FromDate = firstDateOfWeek,
                    ToDate = endDateOfWeek,
                    IdWeek = Guid.NewGuid(),
                    Week = weekNumber,
                    Year = year
                };
                CreateDatabase(obj);
                await SaveChangeAsync();
            }
        }

        public Response GetListWeekOfYear(int year)
        {

            try
            {
                var lsWeek = (from x in _dbNotyf.ReportWeek.AsNoTracking()
                              where x.Year == year
                              select x).ToList();
                return Ultility.Responses("", Enums.TypeCRUD.Success.ToString(), lsWeek);

            }
            catch (Exception e)
            {
                return Ultility.Responses("Có lỗi xảy ra !", Enums.TypeCRUD.Error.ToString(), description: e.Message);

            }
        }

        public Response GetStatisticTourbookingByYear(int year)
        {
            try
            {
                var lsWeekInYear = (from x in _dbNotyf.ReportWeek.AsNoTracking()
                                           where x.Year == year
                              select x).ToList();
                var firstDateInYear = lsWeekInYear.Min(x => x.FromDate);
                var lastDateInYear = lsWeekInYear.Max(x => x.ToDate);
                var firstDateInYearUnix = Ultility.ConvertDatetimeToUnixTimeStampMiliSecond(firstDateInYear);
                var lastDateInYearUnix = Ultility.ConvertDatetimeToUnixTimeStampMiliSecond(lastDateInYear);

                  var lsReportTourBooking = StatisticTourBookingFromDateToDate(firstDateInYearUnix, lastDateInYearUnix);
                return Ultility.Responses("", Enums.TypeCRUD.Success.ToString(), lsReportTourBooking);

            }

            catch (Exception e)
            {
                return Ultility.Responses("Có lỗi xảy ra !", Enums.TypeCRUD.Error.ToString(), description: e.Message);
            }
        }
    }
}
