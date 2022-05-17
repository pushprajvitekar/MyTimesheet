using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;

namespace MyTimesheet
{
    [DataContract(Name="PB")]
    public class PersonalBreak
    {
        //TimeSpan lunchBreak = new TimeSpan(1, 0, 0);
        TimeSpan lunchBreakHalf = new TimeSpan(0, 30, 0);
        TimeSpan lunchTimeStart = new TimeSpan(11, 30, 0);
        TimeSpan lunchTimeEnd = new TimeSpan(14, 0, 0);
        public PersonalBreak(object timeIn=null, object timeOut=null)
        {
            var tin = Convert.ToString(timeIn);
            var tout = Convert.ToString(timeOut);
            if (!string.IsNullOrEmpty(Convert.ToString(tout)))
            {
                tout = tout.Replace("PB - ", "");
            }
            if (TimeSpan.TryParse(tin.Trim(), out TimeSpan res))
            {
                BreakEnd = res;
            }

            if (TimeSpan.TryParse(tout.Trim(), out TimeSpan res1))
            {
                BreakStart = res1;
            }
        }
        [JsonProperty("BS")]
        public TimeSpan? BreakStart { get;  set; }

        [JsonProperty("BE")]
        public TimeSpan? BreakEnd { get;  set; }

        [JsonIgnore]
        public bool IsLunchBreak { get { return BreakStart >= lunchTimeStart && BreakEnd <= lunchTimeEnd; } }

        [JsonIgnore]
        public bool ExcludeLunchBreak { 
            get
            {
                TimeSpan breakEnd = BreakEnd.GetValueOrDefault();
                TimeSpan breakStart = BreakStart.GetValueOrDefault();
                TimeSpan breakTime = breakEnd - breakStart;
                var brkHrs = breakTime.TotalHours;
                if (IsLunchBreak)
                {
                    if (brkHrs <= 0.5)
                    {
                        return false;
                    }
                    if (brkHrs > 0.5)
                    {
                        return true;
                    }
                }
                if ((breakStart >= lunchTimeStart || breakStart <= lunchTimeEnd) || (breakEnd >= lunchTimeStart || breakEnd <= lunchTimeEnd))
                {
                    if (brkHrs >= 1)
                        return true;
                }
                return false;
            } }

        [JsonProperty("BT")]
        [JsonIgnore]
        public TimeSpan BreakTime
        {
            get
            {
                TimeSpan breakEnd = BreakEnd.GetValueOrDefault();
                TimeSpan breakStart = BreakStart.GetValueOrDefault();
                TimeSpan breakTime = breakEnd - breakStart;
                if ((breakEnd > lunchTimeEnd && breakStart >= lunchTimeEnd) 
                    || (breakEnd <= lunchTimeStart && breakStart < lunchTimeStart)
                   )

                {
                    return breakTime;
                }
                else
                {
                    breakTime = breakEnd - breakStart;
                    var brkHrs = breakTime.TotalHours;
                    if (IsLunchBreak)
                    {
                        if (brkHrs <= 0.5)
                        {
                            return lunchBreakHalf;
                        }
                        if (brkHrs > 0.5)
                        {
                            return breakTime;
                        }
                    }
                    if ((breakStart < lunchTimeStart && breakEnd < lunchTimeEnd)
                    || (breakStart > lunchTimeStart && breakEnd > lunchTimeEnd))
                        
                    {
                        if(brkHrs >= 1)
                            return breakTime;
                    }
                }
                return breakTime;
            }
        }
    }
}
