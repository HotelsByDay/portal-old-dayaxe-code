using System;
using System.Collections.Generic;

namespace DayaxeDal
{
    public class CalendarObject
    {
        public DateTime SelectedDate { get; set; }

        public int DailyRedemption { get; set; }

        public int TotalBookings { get; set; }

        public int TotalRedemptions { get; set; }

        public List<string> ProductsDailyPrice { get; set; }

        public int Capacity { get; set; }

        public double RegularPrice { get; set; }

        public double UpgradePrice { get; set; }
    }
}
