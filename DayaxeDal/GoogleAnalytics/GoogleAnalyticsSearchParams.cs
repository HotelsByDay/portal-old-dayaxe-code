using System;
using System.Collections.Generic;
using Google.Apis.AnalyticsReporting.v4.Data;

namespace DayaxeDal.GoogleAnalytics
{
    public class GoogleAnalyticsSearchParams
    {
        public string LoginEmailAddress { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public string ViewId { get; set; }

        public List<Dimension> GoogleAnalyticsDimensions { get; set; }

        public bool IsStageSite { get; set; }
    }
}
