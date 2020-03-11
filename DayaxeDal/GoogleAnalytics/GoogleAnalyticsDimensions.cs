using Google.Apis.AnalyticsReporting.v4.Data;

namespace DayaxeDal.GoogleAnalytics
{
    public static class GoogleAnalyticsDimensions
    {
        public static Dimension AgeDimension
        {
            get
            {
                return new Dimension {Name = "ga:userAgeBracket"};
            }
        }

        public static Dimension GenderDimension
        {
            get
            {
                return new Dimension { Name = "ga:userGender" };
            }
        }

        public static Dimension InterestOtherCategoryDimension
        {
            get
            {
                return new Dimension { Name = "ga:interestOtherCategory" };
            }
        }

        public static Dimension InterestAffinityCategoryDimension
        {
            get
            {
                return new Dimension { Name = "ga:interestAffinityCategory" };
            }
        }

        public static Dimension InterestInMarketCategoryDimension
        {
            get
            {
                return new Dimension { Name = "ga:interestInMarketCategory" };
            }
        }
    }
}
