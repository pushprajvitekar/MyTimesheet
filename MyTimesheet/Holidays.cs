using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace MyTimesheet
{
    public static class Holidays
    {
        const int year_2022 = 2022;
        const int year_2023 = 2023;

        public static List<OfficialHoliday> OfficialHolidays => new List<OfficialHoliday>
        {
            new OfficialHoliday() { Name = "New Year's Day", Description = "in lieu of 1 January", Date = new DateTime(year_2022,1,3) },
            new OfficialHoliday() { Name = "Good Friday", Description = null, Date = new DateTime(year_2022,4,15) },
            new OfficialHoliday() { Name = "Easter Monday", Description = null, Date = new DateTime(year_2022,4,18) },
            new OfficialHoliday() { Name = "May Day", Description = "in lieu of 1 May", Date = new DateTime(year_2022,5,2) },
            new OfficialHoliday() { Name = "Eid al-Fitr", Description = null, Date = new DateTime(year_2022,5,3) },
            new OfficialHoliday() { Name = "Eid al-Adha", Description = "in lieu of 10 July", Date = new DateTime(year_2022,7,11) },
            new OfficialHoliday() { Name = "Austrian National Day", Description = null, Date = new DateTime(year_2022,10,26) },
            new OfficialHoliday() { Name = "Christmas Day", Description = "in lieu of 25 December", Date = new DateTime(year_2022,12,26) },
            new OfficialHoliday() { Name = "St. Stephen's Day", Description = "in lieu of 26 December", Date = new DateTime(year_2022,12,27) },

            new OfficialHoliday() { Name = "New Year's Day", Description = "in lieu of 1 January", Date = new DateTime(year_2023,1,2) },
            new OfficialHoliday() { Name = "Good Friday", Description = null, Date = new DateTime(year_2023,4,7) },
            new OfficialHoliday() { Name = "Easter Monday", Description = null, Date = new DateTime(year_2023,4,10) },
            new OfficialHoliday() { Name = "Eid al-Fitr", Description = "in lieu of 22 April", Date = new DateTime(year_2023,4,24) },
            new OfficialHoliday() { Name = "May Day", Description = null, Date = new DateTime(year_2023,5,1) },

            new OfficialHoliday() { Name = "Eid al-Adha", Description = null, Date = new DateTime(year_2023,6,29) },
            new OfficialHoliday() { Name = "Austrian National Day", Description = null, Date = new DateTime(year_2023,10,26) },
            new OfficialHoliday() { Name = "Christmas Day", Description = null, Date = new DateTime(year_2023,12,25) },
            new OfficialHoliday() { Name = "St. Stephen's Day", Description = null, Date = new DateTime(year_2023,12,26) },
        };

        public static List<DateTime> GetDates => OfficialHolidays.Select(x => x.Date).ToList();

        public static OfficialHoliday GetHoliday(DateTime date) { return OfficialHolidays.FirstOrDefault(s=>s.Date.Date == date.Date); }
        public static bool IsPublicHoliday(DateTime date) { return GetDates.Contains(date); }
    }
}
