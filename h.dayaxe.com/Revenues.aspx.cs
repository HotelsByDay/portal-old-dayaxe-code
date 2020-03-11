using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using DayaxeDal;
using DayaxeDal.Custom;
using DayaxeDal.Extensions;
using DayaxeDal.Parameters;
using DayaxeDal.Repositories;

namespace h.dayaxe.com
{
    public partial class Revenues : BasePageProduct
    {
        private readonly BookingRepository _bookingRepository = new BookingRepository();
        private readonly HotelRepository _hotelRepository = new HotelRepository();
        private Hotels _hotels;
        protected void Page_Init(object sender, EventArgs e)
        {
            try
            {
                Session["Active"] = "Revenue";
                if (Request.Params["id"] != null)
                {
                    int hotelId;
                    int.TryParse(Request.Params["id"], out hotelId);
                    _hotels = _hotelRepository.GetHotel(hotelId, PublicCustomerInfos != null ? PublicCustomerInfos.EmailAddress : string.Empty);
                    Session["Hotel"] = _hotels.HotelId;
                }
                else
                {
                    string sessionHotel = Session["Hotel"] != null ? Session["Hotel"].ToString() : string.Empty;
                    _hotels = _hotelRepository.GetById(int.Parse(sessionHotel));
                }
                if (_hotels == null)
                {
                    Response.Redirect(Constant.HotelList);
                }
        
                DateFrom.Visible = false;
                DateTo.Visible = false;
                Search.Visible = false;
                CustomForm.Visible = false;
                if (!IsPostBack)
                {
                    SelectedFilterBy.Text = SelectedFilterDdl.Text;
                    ProductTypeLabel.Text = ProductTypeDdl.Text;
                    var searchAllParam = new SearchAllBookingsParams
                    {
                        HotelId = _hotels.HotelId,
                        IsForRevenue = true
                    };
                    var bookings = _bookingRepository.GetAllBookingsOfHotel(searchAllParam);
                    FilterBookingByProductType(ref bookings);
                    BindRevenue(bookings);
                }
            }
            catch (Exception ex)
            {
                var logs = new Logs
                {
                    LogKey = "Admin_RevenuesPage_Error",
                    UpdatedBy = PublicCustomerInfos != null ? PublicCustomerInfos.CustomerId : 1,
                    UpdatedDate = DateTime.UtcNow,
                    UpdatedContent = string.Format("{0} - {1} - {2}", ex.Message, ex.StackTrace, ex.Source)
                };
                _bookingRepository.AddLog(logs);
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
        
        }

        protected RevenueItem GetRevenue(List<Bookings> bookings)
        {
            var revenue = new RevenueItem();

            switch (ProductTypeDdl.SelectedValue)
            {
                case "DayPasses":
                    revenue.AvgGuestPerBooking = bookings.Count > 0 ? (int)Math.Round(bookings.Average(b => b.Quantity)) : 0;
                    break;
                case "Cabanas":
                case "Daybeds":
                case "SpaPasses":
                    var firstBooking = bookings.FirstOrDefault();
                    int productId = firstBooking != null ? firstBooking.ProductId : 0;
                    var firstProduct = _bookingRepository.ProductList.FirstOrDefault(p => p.ProductId == productId);
                    revenue.AvgGuestPerBooking = firstProduct != null ? firstProduct.MaxGuest : 0;
                    break;
            }

            // All Pass Redeemed
            revenue.TicketRedeemed = bookings.Sum(b => b.Quantity);
            revenue.TicketRevenue = bookings.Sum(x => x.PassRevenue);

            // Finish Survey
            var surveyFinish = bookings
                .Where(x => _bookingRepository.SurveyList.Where(sl => sl.BookingId == x.BookingId).Any(s => s.IsFinish))
                .Select(x => _bookingRepository.SurveyList.FirstOrDefault(sl => sl.BookingId == x.BookingId))
                .ToList();

            if (surveyFinish.Any())
            {
                surveyFinish.ForEach(item =>
                {
                    double spend = 0;
                    if (item.IsBuyFoodAndDrink && item.FoodAndDrinkPrice.HasValue)
                    {
                        spend += item.FoodAndDrinkPrice.Value;
                    }

                    if (item.IsPayForParking)
                    {
                        spend += Constant.ParkingPrice;
                    }

                    if (item.IsBuySpaService && item.SpaServicePrice.HasValue)
                    {
                        spend += item.SpaServicePrice.Value;
                    }

                    if (item.IsBuyAdditionalService && item.AdditionalServicePrice.HasValue)
                    {
                        spend += item.AdditionalServicePrice.Value;
                    }

                    item.EstSpend = spend;
                    item.RedeemedDate = item.Bookings.RedeemedDate;
                });
            }

            var totalResponsed = bookings.Where(x => surveyFinish.Select(s => s.BookingId).Contains(x.BookingId))
                .Sum(b => b.Quantity);

            // Breakdown Avg Ticket Spend
            revenue.FoodDrink = surveyFinish.Any() ? surveyFinish.Sum(s => s.FoodAndDrinkPrice ?? 0) / totalResponsed : 0;
            revenue.GiftShop = 0;
            revenue.AvgSpa = surveyFinish.Any() ? surveyFinish.Sum(s => s.SpaServicePrice ?? 0) / totalResponsed : 0;
            revenue.Parking = surveyFinish.Any() ? surveyFinish.Sum(s => s.ParkingPrice ?? 0) / totalResponsed : 0;
            revenue.Other = surveyFinish.Any() ? surveyFinish.Sum(s => s.AdditionalServicePrice ?? 0) / totalResponsed : 0;

            //revenue.AvgPerTicketSpend = revenue.FoodDrink + revenue.GiftShop + revenue.AvgSpa + revenue.Parking + revenue.Other;

            revenue.AvgPerTicketSpend = surveyFinish.Count > 0
                ? surveyFinish.Sum(s => s.EstSpend) / totalResponsed
                : 0;

            var totalBookings = bookings.Count > 0 ? bookings.Count : 0;
            var nonSpender = surveyFinish.Count > 0
                ? surveyFinish.Count(s => s.EstSpend.Equals(0.0))
                : 0;
            var nonSpenderPercent = totalBookings > 0 ? nonSpender * 100 / totalBookings : 0;
            revenue.IncrementalRevenue = revenue.TicketRedeemed * revenue.AvgPerTicketSpend * (100 - nonSpenderPercent) / 100;

            revenue.TotalRevenue = revenue.TicketRevenue + revenue.IncrementalRevenue;
            
            // Calculate base on whose use those service and avg for whose used
            //var passUseFoodDrink = surveyFinish.Where(s => s.IsBuyFoodAndDrink).ToList();
            //var passUseSpa = surveyFinish.Where(s => s.IsBuySpaService).ToList();
            //var passUseParking = surveyFinish.Where(s => s.IsPayForParking).ToList();
            //var passUseOther = surveyFinish.Where(s => s.IsBuyAdditionalService).ToList();
            //revenue.FoodDrink = surveyFinish.Any() ? surveyFinish.Sum(s => s.FoodAndDrinkPrice ?? 0)/ totalResponsed : 0;
            //revenue.GiftShop = 0;
            //revenue.AvgSpa = surveyFinish.Any() ? surveyFinish.Sum(s => s.SpaServicePrice ?? 0) / totalResponsed : 0;
            //revenue.Parking = surveyFinish.Any() ? surveyFinish.Sum(s => s.ParkingPrice ?? 0) / totalResponsed : 0;
            //revenue.Other = surveyFinish.Any() ? surveyFinish.Sum(s => s.AdditionalServicePrice ?? 0) / totalResponsed : 0;

            // total Finish Survey
            var totalFinishSurvey = bookings.Count(x => _bookingRepository.SurveyList.Any(sl => sl.BookingId == x.BookingId && sl.IsFinish));
            var usePool = bookings.Count(x => _bookingRepository.SurveyList.Where(sl => sl.BookingId == x.BookingId && sl.IsFinish).Count(s => s.UsePool) > 0);
            var useGym = bookings.Count(x => _bookingRepository.SurveyList.Where(sl => sl.BookingId == x.BookingId && sl.IsFinish).Count(s => s.UseGym) > 0);
            var useSpa = bookings.Count(x => _bookingRepository.SurveyList.Where(sl => sl.BookingId == x.BookingId && sl.IsFinish).Count(s => s.UseSpa) > 0);
            var userBc = bookings.Count(x => _bookingRepository.SurveyList.Where(sl => sl.BookingId == x.BookingId && sl.IsFinish).Count(s => s.UseBusinessCenter) > 0);
            revenue.PoolPercent = totalFinishSurvey > 0 ? (usePool * 100) / totalFinishSurvey : 0;
            revenue.GymPercent = totalFinishSurvey > 0 ? (useGym * 100) / totalFinishSurvey : 0;
            revenue.SpaPercent = totalFinishSurvey > 0 ? (useSpa * 100) / totalFinishSurvey : 0;
            revenue.BusinessCenterPercent = totalFinishSurvey > 0 ? (userBc * 100) / totalFinishSurvey : 0;
            return revenue;
        }

        protected void BindRevenue(List<Bookings> bookings)
        {
            if (bookings.Any() && bookings.Count >= AppConfiguration.MininumItemToCalculateRevenue)
            {
                var revenue = GetRevenue(bookings);

                TicketRedeemedLit.Text = revenue.TicketRedeemed.ToString("N0");
                TicketRevenueLit.Text = string.Format("${0:N0}", revenue.TicketRevenue);
                AvgPerTicketSpendLit.Text = string.Format("${0:N0}", revenue.AvgPerTicketSpend);

                // If Avg Revenue is greater than zero, show it or show defaut config 
                IncrementalRevenueLit.Text = revenue.IncrementalRevenue > 0 ? string.Format("${0:N0}", revenue.IncrementalRevenue) : string.Format("${0:N0}", AppConfiguration.AvgRevenuePerPass);
                TotalRevenueLit.Text = string.Format("${0:N0}", revenue.TotalRevenue);
                AvgGuestPerBookingLit.Text = string.Format("{0:N0}", revenue.AvgGuestPerBooking);

                FoodDrinkLit.Text = string.Format("${0:N0}", revenue.FoodDrink);
                GiftShopLit.Text = string.Format("${0:N0}", revenue.GiftShop);
                AvgSpaLit.Text = string.Format("${0:N0}", revenue.AvgSpa);
                ParkingLit.Text = string.Format("${0:N0}", revenue.Parking);
                OtherLit.Text = string.Format("${0:N0}", revenue.Other);

                PoolPercentLit.Text = string.Format("{0}%", revenue.PoolPercent);
                GymPercentLit.Text = string.Format("{0}%", revenue.GymPercent);
                SpaPercentLit.Text = string.Format("{0}%", revenue.SpaPercent);
                BusinessCenterPercentLit.Text = string.Format("{0}%", revenue.BusinessCenterPercent);
            }
            else
            {
                // number of bookings is less than that in web config (20)
                var tempRevenue = GetRevenue(bookings);
                TicketRedeemedLit.Text = tempRevenue.TicketRedeemed.ToString("N0");
                TicketRevenueLit.Text = string.Format("${0:N0}", tempRevenue.TicketRevenue);
                AvgPerTicketSpendLit.Text = string.Format("${0:N0}", tempRevenue.AvgPerTicketSpend);

                //PassRedeemedLit.Text = tempRevenue.TotalPass.ToString("N0");
                //SpendPerGuestLit.Text = string.Format("~${0}", tempRevenue.SpendPerGuest);
                //TotalLit.Text = string.Format("${0:N0}", tempRevenue.TotalExtraRevenue);

                if (tempRevenue.TicketRedeemed == 0)
                {
                    AvgGuestPerBookingLit.Text = "0";
                    IncrementalRevenueLit.Text = string.Format("${0:N0}", 0);
                    //AvgGuestPerPassLit2.Text = "0";
                }
                else
                {
                    AvgGuestPerBookingLit.Text = string.Format("{0:N0}", tempRevenue.AvgGuestPerBooking);
                    IncrementalRevenueLit.Text = tempRevenue.IncrementalRevenue > 0 ? string.Format("${0:N0}", tempRevenue.IncrementalRevenue) : string.Format("${0:N0}", AppConfiguration.AvgRevenuePerPass);
                    //AvgGuestPerPassLit2.Text = "2";
                }
                TotalRevenueLit.Text = string.Format("${0:N0}", tempRevenue.TotalRevenue);

                FoodDrinkLit.Text = string.Format("${0:N0}", tempRevenue.FoodDrink);
                GiftShopLit.Text = string.Format("${0:N0}", tempRevenue.GiftShop);
                AvgSpaLit.Text = string.Format("${0:N0}", tempRevenue.AvgSpa);
                ParkingLit.Text = string.Format("${0:N0}", tempRevenue.Parking);
                OtherLit.Text = string.Format("${0:N0}", tempRevenue.Other);

                PoolPercentLit.Text = string.Format("{0}%", tempRevenue.PoolPercent);
                GymPercentLit.Text = string.Format("{0}%", tempRevenue.GymPercent);
                SpaPercentLit.Text = string.Format("{0}%", tempRevenue.SpaPercent);
                BusinessCenterPercentLit.Text = string.Format("{0}%", tempRevenue.BusinessCenterPercent);
            }
        }

        protected void Search_OnClick(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(DateFrom.Text) || string.IsNullOrEmpty(DateTo.Text))
            {
                return;
            }
            List<Bookings> bookings;
            DateTime startDate;
            DateTime.TryParseExact(DateFrom.Text, "MM/dd/yyyy", null, DateTimeStyles.None, out startDate);
            DateTime endDate;
            DateTime.TryParseExact(DateTo.Text, "MM/dd/yyyy", null, DateTimeStyles.None, out endDate);
            var searchBookingsParams = new SearchBookingsParams
            {
                HotelId = _hotels.HotelId,
                StartDate = startDate,
                EndDate = endDate,
                IsBookingForRevenue = true
            };
            bookings = _bookingRepository.GetAllbookingsByRange(searchBookingsParams);
            FilterBookingByProductType(ref bookings);
            BindRevenue(bookings);
        }

        protected void SelectedFilterDdl_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            ReCalculateRevenues();
        }

        protected void ProductTypeDdl_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            ReCalculateRevenues();
        }

        private void ReCalculateRevenues()
        {
            var bookings = new List<Bookings>();
            DateFrom.Visible = false;
            DateTo.Visible = false;
            Search.Visible = false;
            CustomForm.Visible = false;
            DateTime startDate;
            var searchBookingsParams = new SearchBookingsParams
            {
                HotelId = _hotels.HotelId,
                IsBookingForRevenue = true
            };
            switch (SelectedFilterDdl.SelectedValue)
            {
                case "All":
                    var searchAllParam = new SearchAllBookingsParams
                    {
                        HotelId = _hotels.HotelId,
                        IsForRevenue = true
                    };
                    bookings = _bookingRepository.GetAllBookingsOfHotel(searchAllParam);
                    break;
                case "Today":
                    var searchTodayParam = new SearchAllBookingsTodayParams
                    {
                        HotelId = _hotels.HotelId,
                        IsForRevenue = true
                    };
                    bookings = _bookingRepository.GetAllBookingsToday(searchTodayParam);
                    break;
                case "ThisWeek":
                    startDate = DateTime.UtcNow.ToLosAngerlesTimeWithTimeZone(PublicHotel.TimeZoneId).StartOfWeek(DayOfWeek.Monday);
                    searchBookingsParams.StartDate = startDate;
                    searchBookingsParams.EndDate = startDate.AddDays(6);
                    bookings = _bookingRepository.GetAllbookingsByRange(searchBookingsParams);
                    break;
                case "ThisMonth":
                    startDate = new DateTime(DateTime.UtcNow.ToLosAngerlesTimeWithTimeZone(PublicHotel.TimeZoneId).Year,
                        DateTime.UtcNow.ToLosAngerlesTimeWithTimeZone(PublicHotel.TimeZoneId).Month, 1);
                    searchBookingsParams.StartDate = startDate;
                    searchBookingsParams.EndDate = startDate.AddDays(DateTime.DaysInMonth(startDate.Year, startDate.Month) - 1);
                    bookings = _bookingRepository.GetAllbookingsByRange(searchBookingsParams);
                    break;
                case "Custom":
                    DateFrom.Visible = true;
                    DateTo.Visible = true;
                    Search.Visible = true;
                    CustomForm.Visible = true;
                    break;
            }
            SelectedFilterBy.Text = SelectedFilterDdl.SelectedItem.Text;
            ProductTypeLabel.Text = ProductTypeDdl.SelectedItem.Text;
            FilterBookingByProductType(ref bookings);
            BindRevenue(bookings);
        }

        private void FilterBookingByProductType(ref List<Bookings> bookings)
        {
            switch (ProductTypeDdl.SelectedValue)
            {
                case "DayPasses":
                    bookings = bookings
                        .Where(p => _bookingRepository.ProductList.First(pchild => pchild.ProductId == p.ProductId)
                                        .ProductType == (int)Enums.ProductType.DayPass).ToList();
                    break;
                case "Cabanas":
                    bookings = bookings
                        .Where(p => _bookingRepository.ProductList.First(pchild => pchild.ProductId == p.ProductId)
                                        .ProductType == (int)Enums.ProductType.Cabana).ToList();
                    break;
                case "Daybeds":
                    bookings = bookings
                        .Where(p => _bookingRepository.ProductList.First(pchild => pchild.ProductId == p.ProductId)
                                        .ProductType == (int)Enums.ProductType.Daybed).ToList();
                    break;
                case "SpaPasses":
                    bookings = bookings
                        .Where(p => _bookingRepository.ProductList.First(pchild => pchild.ProductId == p.ProductId)
                                        .ProductType == (int)Enums.ProductType.SpaPass).ToList();
                    break;
            }
        }
    }
}