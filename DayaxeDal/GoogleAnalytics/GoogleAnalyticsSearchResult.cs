using System.Collections.Generic;

namespace DayaxeDal.GoogleAnalytics
{
    public class GoogleAnalyticsSearchResult
    {
        public IList<string> Dimensions { get; set; }

        public IList<string> Values { get; set; }
    }
}
