using System;
using System.Configuration;
using System.Globalization;

namespace DayaxeDal
{
    /// <summary>
    /// Summary description for AppConfiguration
    /// </summary>
    public static class AppConfiguration
    {
        #region Client Side
        public static string MixpanelKey
        {
            get
            {
                return ConfigurationManager.AppSettings["mixpanel"];
            }
        }

        public static string FacebookKey
        {
            get
            {
                return ConfigurationManager.AppSettings["facebook"];
            }
        }

        public static string GoogleKey
        {
            get
            {
                return ConfigurationManager.AppSettings["google"];
            }
        }

        public static bool EnableMixpanel
        {
            get
            {
                bool enable;
                bool.TryParse(ConfigurationManager.AppSettings["enableMixpanel"], out enable);
                return enable;
            }
        }
        public static bool EnableTracking
        {
            get
            {
                bool enable;
                bool.TryParse(ConfigurationManager.AppSettings["enableTracking"], out enable);
                return enable;
            }
        }

        public static string StripeApiKey
        {
            get
            {
                return ConfigurationManager.AppSettings["stripeApiKey"];
            }
        }

        public static string DefaultImageUrlSendEmail
        {
            get
            {
                return ConfigurationManager.AppSettings["defaultImageUrlSendEmail"];
            }
        }

        public static string AdminUrlDefault
        {
            get { return ConfigurationManager.AppSettings["adminUrlDefault"]; }
        }

        public static string ForPartnerLink
        {
            get { return ConfigurationManager.AppSettings["forPartnerLink"]; }
        }

        public static string HostLink
        {
            get { return ConfigurationManager.AppSettings["hostLink"]; }
        }

        public static string ClientUrlDefault
        {
            get { return ConfigurationManager.AppSettings["clientUrlDefault"]; }
        }

        public static string CdnImageUrlDefault
        {
            get
            {
                return ConfigurationManager.AppSettings["cdnImageUrlDefault"];
            }
        }

        public static string KlaviyoApiKey
        {
            get
            {
                return ConfigurationManager.AppSettings["klaviyoApiKey"];
            }
        }

        public static string KlaviyoPrivateApiKey
        {
            get
            {
                return ConfigurationManager.AppSettings["klaviyoPrivateApiKey"];
            }
        }

        public static string KlaviyoListId
        {
            get
            {
                return ConfigurationManager.AppSettings["klaviyoListId"];
            }
        }

        public static string KlaviyoNewsletterListId
        {
            get
            {
                return ConfigurationManager.AppSettings["klaviyoNewsletterListId"];
            }
        }

        public static string KlaviyoWaittingListId
        {
            get
            {
                return ConfigurationManager.AppSettings["klaviyoWaittingListId"];
            }
        }

        public static bool IsStageSite
        {
            get
            {
                return ConfigurationManager.AppSettings["IsStageSite"] == "true";
            }
        }

        public static DateTime RestrictBookingSameDayAtHour
        {
            get
            {
                DateTime val;
                DateTime.TryParseExact(ConfigurationManager.AppSettings["restrictBookingSameDayAtHour"], "H:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out val);
                return val;
            }
        }

        public static string LandingPageSubscription
        {
            get
            {
                return ConfigurationManager.AppSettings["landingPageSubscription"];
            }
        }

        public static int ReserveSubscriptionPassInHours
        {
            get
            {
                int val;
                int.TryParse(ConfigurationManager.AppSettings["reserveSubscriptionPassInHours"], out val);
                return val;
            }
        }

        public static int MaxReserveSubscriptionPass
        {
            get
            {
                int val;
                int.TryParse(ConfigurationManager.AppSettings["maxReserveSubscriptionPass"], out val);
                return val;
            }
        }

        public static double MaxRadius
        {
            get
            {
                double val;
                double.TryParse(ConfigurationManager.AppSettings["maxRadius"], out val);
                return val;
            }
        }

        public static int SubscriptionUpgradeId
        {
            get
            {
                int val;
                int.TryParse(ConfigurationManager.AppSettings["subscriptionUpgradeId"], out val);
                return val;
            }
        }

        public static string PromoCodeUpgrade
        {
            get
            {
                return ConfigurationManager.AppSettings["promoCodeUpgrade"];
            }
        }

        #endregion Client Side

        #region Server Side

        public static string DayaxeClientUrl
        {
            get { return ConfigurationManager.AppSettings["DayaxeClientUrl"]; }
        }

        public static double AvgRevenuePerPass
        {
            get
            {
                double val;
                double.TryParse(ConfigurationManager.AppSettings["AvgRevenuePerPass"], out val);
                return val;
            }
        }

        public static int MininumItemToCalculateRevenue
        {
            get
            {
                int val;
                int.TryParse(ConfigurationManager.AppSettings["MininumItemToCalculateRevenue"], out val);
                return val;
            }
        }

        public static string GoogleApiKey
        {
            get
            {
                return ConfigurationManager.AppSettings["googleApiKey"];
            }
        }

        public static long SendEmailSurveyAfterMinutes
        {
            get
            {
                long val = 0;
                long.TryParse(ConfigurationManager.AppSettings["sendEmailSurveyAfterMinutes"], out val);
                return val;
            }
        }

        #endregion Server Side
    }
}