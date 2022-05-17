using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;

namespace MyTimesheet
{
    [DataContract(Name = "TSD")]
    public class TimeSheetDay
    {
        TimeSpan dailyHours = new TimeSpan(8, 0, 0);
        TimeSpan lunchBreak = new TimeSpan(1, 0, 0);
        TimeSpan lunchBreakHalf = new TimeSpan(0, 30, 0);
        public TimeSheetDay(object date, object timeIn = null, object timeOut = null)
        {
            var dtStr = Convert.ToString(date);
            dtStr = dtStr.TrimEnd(new char[] { '*' });
            dtStr = dtStr.Remove(dtStr.Length - 3);
            Date = Convert.ToDateTime(dtStr);
            var pb = new PersonalBreak(timeIn, timeOut);
            TimeIn = pb.BreakEnd;
            TimeOut = pb.BreakStart;
            Break1 = new PersonalBreak();
            Break2 = new PersonalBreak();
            Break3 = new PersonalBreak();
            Remarks = string.Empty;
        }

        [JsonProperty("HW")]
        [JsonIgnore]
        public TimeSpan? HoursWorked
        {

            get
            {
                if (MissedTimeIn || MissedTimeOut) return new TimeSpan(0, 0, 0);
                var timeOut = !TimeOut.HasValue ? new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, 0) : TimeOut;
                TimeSpan breakTime = GetTotalBreakTime();
                return timeOut - TimeIn - breakTime - LunchDuration;
            }
        }

        [JsonProperty("EH")]
        [JsonIgnore]
        public TimeSpan? ExtraHours => HoursWorked - dailyHours;

        private TimeSpan GetTotalBreakTime()
        {
            return Break1.BreakTime + Break2.BreakTime + Break3.BreakTime;
        }
        [JsonProperty("LD")]
        [JsonIgnore]
        public TimeSpan LunchDuration
        {
            get
            {
                if (TookLunchBreak || ExcludeLunchBreak)
                {
                    return new TimeSpan();
                }
                else
                    return lunchBreak;
            }
        }
        [JsonProperty("LBT")]
        [JsonIgnore]
        public TimeSpan LunchBreakTime
        {
            get { return TookLunchBreak ? lunchBreakHalf : lunchBreak; }
        }
        [JsonProperty("ELB")]
        [JsonIgnore]
        public bool ExcludeLunchBreak
        {
            get { return Break1.ExcludeLunchBreak || Break2.ExcludeLunchBreak || Break3.ExcludeLunchBreak; }
        }
        [JsonProperty("TLB")]
        [JsonIgnore]
        public bool TookLunchBreak
        {
            get { return Break1.IsLunchBreak || Break2.IsLunchBreak || Break3.IsLunchBreak; }
        }
        [JsonProperty("MTI")]
        [JsonIgnore]
        public bool MissedTimeIn { get { return IsWorkingDay && !TimeIn.HasValue && TimeOut.HasValue && Date != DateTime.Now.Date; } }

        [JsonProperty("MTO")]
        [JsonIgnore]
        public bool MissedTimeOut { get { return IsWorkingDay && TimeIn.HasValue && !TimeOut.HasValue && Date != DateTime.Now.Date; } }

        [JsonProperty("IL")]
        [JsonIgnore]
        public bool IsLeave { get { return Date.Date < DateTime.Now.Date && IsWorkingDay && !TimeOut.HasValue && !TimeIn.HasValue; } }
        [JsonProperty("IWD")]
        [JsonIgnore]
        public bool IsWorkingDay => !IsWeekend && !IsOfficialHoliday;

        [JsonProperty("DT")]
        public DateTime Date { get; set; }
        [JsonProperty("TI")]
        public TimeSpan? TimeIn { get; set; }
        [JsonProperty("TO")]
        public TimeSpan? TimeOut { get; set; }
        [JsonProperty("B1")]
        public PersonalBreak Break1 { get; set; }
        [JsonProperty("B2")]
        public PersonalBreak Break2 { get; set; }
        [JsonProperty("B3")]
        public PersonalBreak Break3 { get; set; }
        [JsonProperty("IOH")]
        [JsonIgnore]
        public bool IsOfficialHoliday => Holidays.IsPublicHoliday(Date);
        [JsonProperty("H")]
        [JsonIgnore]
        public OfficialHoliday Holiday => IsOfficialHoliday ? Holidays.GetHoliday(Date) : null;
        [JsonProperty("IWE")]
        [JsonIgnore]
        public bool IsWeekend => Date.DayOfWeek == DayOfWeek.Saturday || Date.DayOfWeek == DayOfWeek.Sunday;

        [JsonProperty("REM")]
        public string Remarks { get; set; }
        public static List<TimeSheetDay> GetTimeSheetRows(IEnumerable<string[]> rows)
        {

            var convertedList = (from rw in rows
                                 select new TimeSheetDay(rw[Column.Date.Index], rw[Column.TimeIn.Index], rw[Column.TimeOut.Index])
                                 {
                                     Break1 = new PersonalBreak(rw[Column.Break1_TimeIn.Index], rw[Column.Break1_TimeOut.Index]),
                                     Break2 = new PersonalBreak(rw[Column.Break2_TimeIn.Index], rw[Column.Break2_TimeOut.Index]),
                                     Break3 = new PersonalBreak(rw[Column.Break3_TimeIn.Index], rw[Column.Break3_TimeOut.Index]),
                                     Remarks = Convert.ToString(rw[Column.Remarks.Index])

                                 }).ToList();

            return convertedList;
        }
    }
}
