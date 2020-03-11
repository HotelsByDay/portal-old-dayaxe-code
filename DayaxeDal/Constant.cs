using System.Configuration;

namespace DayaxeDal
{
    public static class Constant
    {
        public static string ConnectionString
        {
            get { return ConfigurationManager.ConnectionStrings["DayaxeConnectionString"].ConnectionString; }
        }

        public const string FullStar = "/images/star_full.png";
        public const string HalfStar = "/images/star_half.png";
        public const string EmptyStar = "/images/star_empty.png";
        public const string ImageDefault = "/images/default-thumb.png";

        public static string FullStarEmail
        {
            get
            {
              return AppConfiguration.CdnImageUrlDefault + "/images/star_f_full.png";
            }
        }

        public static string HalfStarEmail
        {
            get
            {
                return AppConfiguration.CdnImageUrlDefault + "/images/star_f_half.png";
            }
        }

        public static string EmptyStarEmail
        {
            get
            {
                return AppConfiguration.CdnImageUrlDefault + "/images/star_f_empty.png";
            }
        }

        public const string SearchPage = "/day-passes";
        public const string SearchSpapage = "/spa-passes";
        public const string SerachCabanasPage = "/cabanas";
        public const string SerachDaybedsPage = "/daybeds";

        public const string SearchPageDefault = "/s/socal/day-passes";
        public const string DayPassPage = "/s/{0}/day-passes";
        public const string SpaPassPage = "/s/{0}/spa-passes";
        public const string CabanaPassPage = "/s/{0}/cabanas";
        public const string DaybedsPassPage = "/s/{0}/daybeds";

        public const string BookPage = "/book.aspx";
        public const string BookProductPage = "/{0}/{1}/{2}/{3}-book/{4}";

        public const string ConfirmPage = "/Confirm.aspx";
        public const string ConfirmProductPage = "/{0}/{1}/{2}/{3}-confirm-{4}";
        public const string ConfirmPageProduct = "-confirm-";

        public const string ViewDayPassPage = "/ViewDayPass.aspx";
        public const string DefaultPage = "/";
        public const string RefreshHotelUrl = "/Handler/Refresh.ashx";
        public const string SearchPageWithCityUrl = "/s/{0}/{1}";

        public const string UpgradePage = "/{0}/{1}/{2}/{3}-upgrade-{4}";

        public const string HotelList = "/HotelListings.aspx";
        public const string MarketList = "/Markets.aspx";
        public const string UserHotelPage = "/UserHotel.aspx";
        public const string ContentPagePage = "/ContentPage.aspx";
        public const string PromoListPage = "/PromoList.aspx";
        public const string PromoDetailPage = "/PromoDetails.aspx";
        public const string GiftCardListPage = "/GiftCardList.aspx";
        public const string ProductListPage = "/ProductListings.aspx";
        public const string FeedbackList = "/Feedback.aspx";

        // Subscription
        public const string SubscriptionListPage = "/SubscriptionsList.aspx";
        public const string ConfirmPageSubscription = "/confirm/";

        public const string SubscriptionConfirmPage = "/subscription/{0}/confirm/{1}{2}";

        // eGift Card
        public const string GiftCardConfirmPage = "/gift-card/confirm/{0}";

        //public const string SignUpPage = "/signup?ReturnUrl={0}";
        public const string SignIpPage = "/signin?ReturnUrl={0}";

        public const string InvalidTicketPage = "/invalid-ticket";

        public const string DateFormat = "MMM dd, yyyy";
        public const string FullDateFormat = "MMM dd, yyyy hh:mm tt";
        public const string SurveyDateFormat = "ddd, MMM dd, yyyy";
        public const string ReviewsDateFormat = "ddd, MMM dd. yyyy";
        public const string ExpiresDateFormat = "MMM dd, hh tt";
        public const string DiscountDateFormat = "MM/dd/yyyy";
        public const string DiscountDateTimeFormat = "MM/dd/yyyy hh:mm tt";

        public const string GiftCardFormat = "{0}G{1}";
        public const string GiftCardNameFormat = "{0} eGift Card for {1} - {2}";

        public const string UpgradeKey = "Upgrade";

        public const int ParkingPrice = 20;
        public const int LosAngelesTimeWithUtc = -7;
        public const int LosAngelesSummerTimeWithUtc = -8;
        public const string DefaultLosAngelesTimezoneId = "America/Los_Angeles";

        public const int DefaultRedemptionPeriod = 30;
        public const int DefaultMaxGuest = 2;

        public const int ItemPerPage = 10;
        public const int BookingCodeLength = 12;

        public const string SearchDocumentTitle = "{0} Luxury Hotel{1} | VIP pass at DayAxe.com";
        public const string SearchMetaDescription = "Buy day passes to luxury hotels in {0}! Get all day full vip access to pools, cabanas, spas, gyms, fitness centers, and on site activities like tennis, golf, volleyball and basketball.";
        public const string SearchMetaKeywords = "{0} DayAxe day passes, outdoor pools, indoor pools, hot tub, jacuzzi, whirlpool, pool cabanas, poolside bar, poolside food & drink, pool waterslide, spas, spa treatments, saunas, steam rooms, gyms, fitness centers, shower room, locker room, changing room, fitness classes, tennis, golf, volleyball, basketball, golf lessons, golf driving range, yoga classes/instruction, game rooms, conference rooms, meeting rooms, business services, business lounge, computer station, business center, {1}";
        public const string MaxGuestText = "Max {0} adult{1} per {2}; {3}";
        public const string TicketNotAvailable = "No more tickets available on {0:MMMM dd, yyyy}. Please try a different date or product.";
        public const string TicketAvailable = "Only {0} ticket(s) available on {1:MMMM dd, yyyy}. Decrease your ticket number or try a different date or product.";

        public const string KidsUnder6Free = "Children under 6 are free";
        public const string NoKidAllowed = "Children not allowed";
        public const string KidAllowedAtFullPrice = "Children must have full price ticket";

        public const string ProductKidsUnder6Free = "AGES 0-6 FREE";
        public const string ProductNoKidAllowed = "NOT ALLOWED";
        public const string ProductKidAllowedAtFullPrice = "FULL PRICE";

        public const string DayPassString = " Day Pass";
        public const string SpaPassString = " Spa Pass";
        public const string CabanasPassString = " Cabana Pass";
        public const string AddOnsPassString = " Add-Ons";
        public const string DaybedsString = " Daybeds";

        public const string CommingSoonString = "COMING SOON";
        public const string FrontDeskString = "Front Desk";
        public const string TotalReviews = "{0} reviews";
        public const string MoreAtHotel = "More at {0}";

        public const string SearchBookingsAdminpage = "/SearchBookings.aspx";
        public const string CustomersDetailsAdminPage = "/CustomerDetails.aspx?id={0}";
        public const string StripeCustomerLink = "https://dashboard.stripe.com/customers/{0}";
        public const string StripePaymentLink = "https://dashboard.stripe.com/payments/{0}";
        
        public const string KlaviyoListApiUrl = "https://a.klaviyo.com/api/v1/list/{0}/members";

        public const string TotalReviewsText = "<span>{0}</span> review(s) for \"<b>{1}</b>\"";

        public const string GoldPassLink = "http://l.dayaxe.com/goldpass";

        public const string GoldFriendCode = "GOLDFRIEND";

        public const string SignIpPageAdmin = "/?ReturnUrl={0}";

        public const string GoogleTimeZoneUrl = "https://maps.googleapis.com/maps/api/timezone/json?location={0},{1}&timestamp={2}&key={3}";
        public const string GoogleTimeZoneWithAddress = "https://maps.googleapis.com/maps/api/geocode/json?address={0}&key={1}";
        public const string UsDefaultTime = "America/Los_Angeles";//Pacific Standard Time

        public const string FaqUrl = "https://dayaxe.zendesk.com/api/v2/help_center/en-us/articles.json?label_names=product-faq";

        public const string FaqIndexUrl = "https://dayaxe.zendesk.com/hc/en-us";

        public const string EncryptPassword = "8-8-88";
    }
}
