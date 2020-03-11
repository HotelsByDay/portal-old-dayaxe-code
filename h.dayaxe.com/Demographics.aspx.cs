using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using DayaxeDal;
using DayaxeDal.Extensions;
using DayaxeDal.GoogleAnalytics;
using DayaxeDal.Parameters;
using DayaxeDal.Repositories;
using Google.Apis.AnalyticsReporting.v4.Data;

namespace h.dayaxe.com
{
    public partial class Demographics : BasePageProduct
    {
        private readonly BookingRepository _bookingRepository = new BookingRepository();

        protected string AgeAndGenderCategories { get; set; }

        protected string DemographicsMale { get; set; }

        protected string DemographicsFemale { get; set; }

        protected void Page_Init(object sender, EventArgs e)
        {
            Session["Active"] = "CustomerInsight";
            if (!IsPostBack)
            {
                var searchAllParam = new SearchAllBookingsParams
                {
                    HotelId = PublicHotel.HotelId,
                    IsForRevenue = true
                };
                var bookings = _bookingRepository.GetAllBookingsOfHotel(searchAllParam);
                if (bookings.Count >= AppConfiguration.MininumItemToCalculateRevenue)
                {
                    MVDemographics.ActiveViewIndex = 1;

                    DateTime startDate = new DateTime(DateTime.UtcNow.ToLosAngerlesTimeWithTimeZone(PublicHotel.TimeZoneId).Year,
                        DateTime.UtcNow.ToLosAngerlesTimeWithTimeZone(PublicHotel.TimeZoneId).Month, 1);
                    DateTime endDate = startDate.AddDays(DateTime.DaysInMonth(startDate.Year, startDate.Month) - 1);
                    BindAgeAndGender(startDate, endDate);
                }
                else
                {
                    MVDemographics.ActiveViewIndex = 0;
                }
                //MVDemographics.ActiveViewIndex = 0;
                SelectedFilterBy.Text = SelectedFilterDdl.SelectedItem.Text;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
        
        }

        protected void SelectedFilterDdl_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            DateTime startDate;
            DateTime endDate;
            switch (SelectedFilterDdl.SelectedValue)
            {
                case "All":
                    startDate = new DateTime(2016, 01, 01);
                    endDate = DateTime.UtcNow.ToLosAngerlesTimeWithTimeZone(PublicHotel.TimeZoneId);
                    BindAgeAndGender(startDate, endDate);
                    break;
                case "Today":
                    startDate = endDate = DateTime.UtcNow.ToLosAngerlesTimeWithTimeZone(PublicHotel.TimeZoneId);
                    BindAgeAndGender(startDate, endDate);
                    break;
                case "ThisWeek":
                    startDate = DateTime.UtcNow.ToLosAngerlesTimeWithTimeZone(PublicHotel.TimeZoneId).StartOfWeek(DayOfWeek.Monday);
                    endDate = startDate.AddDays(6);
                    BindAgeAndGender(startDate, endDate);
                    break;
                case "ThisMonth":
                    startDate = new DateTime(DateTime.UtcNow.ToLosAngerlesTimeWithTimeZone(PublicHotel.TimeZoneId).Year,
                        DateTime.UtcNow.ToLosAngerlesTimeWithTimeZone(PublicHotel.TimeZoneId).Month, 1);
                    endDate = startDate.AddDays(DateTime.DaysInMonth(startDate.Year, startDate.Month) - 1);
                    BindAgeAndGender(startDate, endDate);
                    break;
                case "Custom":
                    DateFrom.Visible = true;
                    DateTo.Visible = true;
                    Search.Visible = true;
                    CustomForm.Visible = true;
                    break;
            }
            SelectedFilterBy.Text = SelectedFilterDdl.SelectedItem.Text;
        }

        protected void Search_OnClick(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(DateFrom.Text) || string.IsNullOrEmpty(DateTo.Text))
            {
                return;
            }
            DateTime startDate;
            DateTime.TryParseExact(DateFrom.Text, "MM/dd/yyyy", null, DateTimeStyles.None, out startDate);
            DateTime endDate;
            DateTime.TryParseExact(DateTo.Text, "MM/dd/yyyy", null, DateTimeStyles.None, out endDate);

            BindAgeAndGender(startDate, endDate);
        }

        private void BindAgeAndGender(DateTime startDate, DateTime endDate)
        {
            var param = new GoogleAnalyticsSearchParams
            {
                IsStageSite = true,
                FromDate = startDate,
                ToDate = endDate,
                GoogleAnalyticsDimensions = new List<Dimension>
                {
                    GoogleAnalyticsDimensions.AgeDimension,
                    GoogleAnalyticsDimensions.GenderDimension
                },
                ViewId = GoogleAnalyticsViewId.DayAxeAllWebsiteDataStage,
                LoginEmailAddress = "stage.bookings@dayaxe.com"
            };

            var ageAndGenderResult = GoogleAnalyticsHelper.GetData(param);

            var female = ageAndGenderResult.Where(x => string.Join(",", x.Dimensions).Contains("female")).ToList();
            var male = ageAndGenderResult.Except(female).ToList();

            DemographicsMale = string.Format("[{0}]", string.Join(",", male.Select(x => x.Values.First())));
            DemographicsFemale = string.Format("[{0}]", string.Join(",", female.Select(x => string.Format("-{0}", x.Values.First()))));

            AgeAndGenderCategories = string.Format("[{0}]", string.Join(",", male.Select(x => string.Format("'{0}'", x.Dimensions[0]))));
        }
    }
}