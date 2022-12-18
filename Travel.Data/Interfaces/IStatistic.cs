using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Travel.Shared.ViewModels;

namespace Travel.Data.Interfaces
{
    public interface IStatistic
    {
        Response StatisticTourBookingFromDateToDate(long fromDate, long toDate);
        Task<bool> SaveReportTourBookingEveryDay(DateTime dateInput); // hạm tự động
        Response StatisticTourBookingInThisWeek(long fromDate, long toDate);
        Response GetStatisticTourbookingByYear(int year);

        Response GetStatisticTourbookingByMonth(int Month, int year);
        Response GetListWeekOfYear(int year);
        Response GetStatisticTotalTourBooking(long fromDate, long toDate);
        Task SaveReportWeek();
    }
}
