using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace MyTimesheet
{
    [DataContract(Name = "TSM")]
    public class TimeSheetMonth : INotifyPropertyChanged
    {
        private double totalHoursBillable;
        private double totalHoursWorked;
        private TimeSpan extraHours;
        private int leaves;
        private DateTime? timeOutToday;
        private ObservableCollection<TimeSheetDay> dailyTimeSheets;

        public TimeSheetMonth()
        {

        }
        public TimeSheetMonth(string monthName, int year)
        {
            int month = DateTime.ParseExact(monthName, "MMMM", CultureInfo.CurrentCulture).Month;
            var dates = GetDates(year, month);
            var timesheets = dates.Select(d => new TimeSheetDay(d));
            Initialize(timesheets);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public static List<DateTime> GetDates(int year, int month)
        {
            return Enumerable.Range(1, DateTime.DaysInMonth(year, month))  // Days: 1, 2 ... 31 etc.
                             .Select(day => new DateTime(year, month, day)) // Map each day to a date
                             .ToList(); // Load dates into a list
        }
        public TimeSheetMonth(IEnumerable<TimeSheetDay> dailyReports)
        {
            Initialize(dailyReports);
        }
        public void CalculateSummaryDetails()
        {
            CalculateTotalHrsWorked();
            GetTotalHrsBillable();
            CalculateExtraHours();
            GetTimeOutForToday();
            CalculateLeaves();
        }
        private void Initialize(IEnumerable<TimeSheetDay> dailyReports)
        {
            DailyTimeSheets = new ObservableCollection<TimeSheetDay>(dailyReports);
            CalculateSummaryDetails();
            var firstRow = DailyTimeSheets.FirstOrDefault().Date;
            Year = firstRow.Year;
            Name = firstRow.ToString("yyyy MMMM");
            Month = firstRow.ToString("MMMM");
        }

        private void CalculateLeaves()
        {
            Leaves = DailyTimeSheets.Count(e => e.IsLeave == true);
        }

        private void GetTotalHrsBillable()
        {
            TotalHoursBillable = DailyTimeSheets.Where(s => s.IsWorkingDay).Aggregate(new TimeSpan(), (r, e) => r + new TimeSpan(8, 0, 0)).TotalHours;
        }

        private void GetTimeOutForToday()
        {
            var today = DailyTimeSheets.LastOrDefault(s => s.TimeIn.HasValue);

            if (today != null && today.TimeIn.HasValue && today.Date == DateTime.Now.Date)
            {
                var hrsToWorkToday = today.HoursWorked;
                TimeOutToday = DateTime.Now.Date + (today.TimeIn.Value + new TimeSpan(8, 0, 0) + today.LunchBreakTime - ExtraHours);
            }
            else
                TimeOutToday = null;
        }

        private void CalculateExtraHours()
        {
            IEnumerable<TimeSheetDay> query = GetWorkingDays();
            var h = query.Select(s => s.ExtraHours.GetValueOrDefault());
            ExtraHours = TimeSpan.FromHours(h.Sum(s => s.TotalHours));
        }

        private void CalculateTotalHrsWorked()
        {
            IEnumerable<TimeSheetDay> query = GetWorkingDays();
            var totalHrs = query.Aggregate(new TimeSpan(), (r, e) => r + e.HoursWorked.GetValueOrDefault()).TotalHours;
            TotalHoursWorked = Math.Round(totalHrs * 4, MidpointRounding.ToEven) / 4;
        }

        private IEnumerable<TimeSheetDay> GetWorkingDays()
        {
            return DailyTimeSheets.Where(s => s.IsWorkingDay && s.Date < DateTime.Now.Date);
        }

        [JsonProperty("DTS")]
        public ObservableCollection<TimeSheetDay> DailyTimeSheets
        {
            get
            {
                return dailyTimeSheets;
            }
            set
            {
                dailyTimeSheets = value;
                NotifyPropertyChanged();
            }
         }
        [JsonProperty("THB")]
        public double TotalHoursBillable
        {
            get
            {
                return totalHoursBillable;
            }
            set
            {
                totalHoursBillable = value;
                NotifyPropertyChanged();
            }
        }
        [JsonProperty("THW")]
        public double TotalHoursWorked
        {
            get
            {
                return totalHoursWorked;
            }
            set
            {
                totalHoursWorked = value;
                NotifyPropertyChanged();
            }
        }
        [JsonProperty("EH")]
        public TimeSpan ExtraHours
        {
            get
            {
                return extraHours;
            }
            set
            {
                extraHours = value;
                NotifyPropertyChanged();
            }
        }

        [JsonProperty("Lea")]
        public int Leaves
        {
            get
            {
                return leaves;
            }
            set
            {
                leaves = value;
                NotifyPropertyChanged();
            }
        }
        [JsonProperty("TOT")]
        public DateTime? TimeOutToday
        {
            get
            {
                return timeOutToday;
            }
            set
            {
                timeOutToday = value;
                NotifyPropertyChanged();
            }
        }
        [JsonProperty("YR")]
        public int Year { get; set; }
        [JsonProperty("MM")]
        public string Month { get; set; }
        [JsonProperty("Name")]
        public string Name { get; set; }
    }
}
