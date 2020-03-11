using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using DayaxeDal;
using DayaxeDal.Extensions;
using DayaxeDal.Parameters;
using DayaxeDal.Repositories;

namespace h.dayaxe.com
{
    public partial class Engagement : BasePageProduct
    {
        protected string MostRedeemedDays { get; set; }

        protected string CustomerSatisfaction { get; set; }

        protected string HotelVisited { get; set; }
        protected string HotelVisitedCateogries { get; set; }

        protected string NewData { get; set; }
        protected string ReturningData { get; set; }
        protected string NewAndReturningCategory { get; set; }
        protected string PercentNew { get; set; }
        protected string PercentReturning { get; set; }

        protected string FrequencyCategory { get; set; }
        protected string Frequency { get; set; }

        protected string DaysRedeemedCategories { get; set; }
        protected string DaysRedeemed { get; set; }

        protected string HoursRedeemedCategories { get; set; }
        protected string HoursRedeemed { get; set; }
        private List<DateTime> ListDates { get; set; }

        private readonly BookingRepository _bookingRepository = new BookingRepository();
        private readonly SurveyRepository _surveyRepository = new SurveyRepository();
        protected void Page_Init(object sender, EventArgs e)
        {
            try
            {
                Session["Active"] = "CustomerInsight";
                var searchAllParam = new SearchAllBookingsParams
                {
                    HotelId = PublicHotel.HotelId,
                    IsForRevenue = true
                };
                var result = _bookingRepository.GetAllBookingsOfHotel(searchAllParam);
                if (result.Count >= AppConfiguration.MininumItemToCalculateRevenue)
                {
                    MVEngagement.ActiveViewIndex = 1;
                }
                else
                {
                    MVEngagement.ActiveViewIndex = 0;
                }
                if (!IsPostBack)
                {
                    SelectedFilterBy.Text = SelectedFilterDdl.SelectedItem.Text;
                    ProductTypeLabel.Text = ProductTypeDdl.Text;

                    DateTime endDate = DateTime.UtcNow.ToLosAngerlesTimeWithTimeZone(PublicHotel.TimeZoneId);
                    var startDate = new DateTime(DateTime.UtcNow.ToLosAngerlesTimeWithTimeZone(PublicHotel.TimeZoneId).Year,
                        DateTime.UtcNow.ToLosAngerlesTimeWithTimeZone(PublicHotel.TimeZoneId).Month, 1);
                    var searchBookingsParams = new SearchBookingsParams
                    {
                        HotelId = PublicHotel.HotelId,
                        StartDate = startDate,
                        EndDate = endDate,
                        IsBookingForRevenue = false
                    };
                    var bookings = _bookingRepository.GetAllbookingsByRange(searchBookingsParams);

                    ListDates = Helper.GetDateRanges(startDate, endDate);

                    BindEngagement(bookings);
                }
            }
            catch (Exception ex)
            {
                var logs = new Logs
                {
                    LogKey = "Admin_EngagementPage_Error",
                    UpdatedBy = 1,
                    UpdatedDate = DateTime.UtcNow,
                    UpdatedContent = string.Format("{0} - {1} - {2}", ex.Message, ex.StackTrace, ex.Source)
                };
                _bookingRepository.AddLog(logs);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
        
        }

        protected void SelectedFilterDdl_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            ReCalculateEngagement();
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
                HotelId = PublicHotel.HotelId,
                StartDate = startDate,
                EndDate = endDate,
                IsBookingForRevenue = false
            };
            bookings = _bookingRepository.GetAllbookingsByRange(searchBookingsParams);

            ListDates = Helper.GetDateRanges(startDate, endDate);
            BindEngagement(bookings);
        }

        protected void ProductTypeDdl_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            ReCalculateEngagement();
        }

        private void ReCalculateEngagement()
        {
            var bookings = new List<Bookings>();
            DateFrom.Visible = false;
            DateTo.Visible = false;
            Search.Visible = false;
            CustomForm.Visible = false;
            DateTime startDate;
            DateTime endDate = DateTime.UtcNow.ToLosAngerlesTimeWithTimeZone(PublicHotel.TimeZoneId);
            var searchBookingsParams = new SearchBookingsParams
            {
                HotelId = PublicHotel.HotelId,
                IsBookingForRevenue = false
            };
            switch (SelectedFilterDdl.SelectedValue)
            {
                case "All":
                    var searchAllParam = new SearchAllBookingsParams
                    {
                        HotelId = PublicHotel.HotelId,
                        IsForRevenue = false
                    };
                    bookings = _bookingRepository.GetAllBookingsOfHotel(searchAllParam);
                    var firstDate = bookings.Where(x => x.RedeemedDate.HasValue).Min(x => x.RedeemedDate.Value).ToLosAngerlesTimeWithTimeZone(PublicHotel.TimeZoneId);
                    var lastDate = bookings.Where(x => x.RedeemedDate.HasValue).Max(x => x.RedeemedDate.Value).ToLosAngerlesTimeWithTimeZone(PublicHotel.TimeZoneId);

                    ListDates = Helper.GetDateRanges(firstDate, lastDate);
                    break;
                case "Today":
                    ListDates = new List<DateTime>
                    {
                        DateTime.UtcNow.ToLosAngerlesTimeWithTimeZone(PublicHotel.TimeZoneId)
                    };
                    var searchTodayParam = new SearchAllBookingsTodayParams
                    {
                        HotelId = PublicHotel.HotelId,
                        IsForRevenue = false
                    };
                    bookings = _bookingRepository.GetAllBookingsToday(searchTodayParam);
                    break;
                case "ThisWeek":
                    startDate = DateTime.UtcNow.ToLosAngerlesTimeWithTimeZone(PublicHotel.TimeZoneId).StartOfWeek(DayOfWeek.Monday);
                    searchBookingsParams.StartDate = startDate;
                    searchBookingsParams.EndDate = endDate;
                    bookings = _bookingRepository.GetAllbookingsByRange(searchBookingsParams);
                    ListDates = Helper.GetDateRanges(startDate, endDate);
                    break;
                case "ThisMonth":
                    startDate = new DateTime(DateTime.UtcNow.ToLosAngerlesTimeWithTimeZone(PublicHotel.TimeZoneId).Year,
                        DateTime.UtcNow.ToLosAngerlesTimeWithTimeZone(PublicHotel.TimeZoneId).Month, 1);
                    searchBookingsParams.StartDate = startDate;
                    searchBookingsParams.EndDate = endDate;
                    bookings = _bookingRepository.GetAllbookingsByRange(searchBookingsParams);
                    ListDates = Helper.GetDateRanges(startDate, endDate);
                    break;
                case "Custom":
                    DateFrom.Visible = true;
                    DateTo.Visible = true;
                    Search.Visible = true;
                    CustomForm.Visible = true;
                    ListDates = new List<DateTime>();
                    break;
            }
            SelectedFilterBy.Text = SelectedFilterDdl.SelectedItem.Text;
            ProductTypeLabel.Text = ProductTypeDdl.SelectedItem.Text;
            FilterBookingByProductType(ref bookings);
            BindEngagement(bookings);
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

        private void BindEngagement(List<Bookings> bookings)
        {
            // Most Redeemed Days
            BindMostRedeemedDays(bookings);

            //HotelVisited
            BindHotelVisited();

            //New and Returning
            BindNewAndReturning(bookings);

            // Redemtion Frequency
            BindRedemtionFrequency(bookings);

            //Redeemed Days
            BindDaysRedeemed(bookings);

            //Most Redeemed Hours
            BindHoursRedeemed(bookings);

            // Customer Satisfaction
            var surveys = _surveyRepository.GetByHotelId(PublicHotel.HotelId)
                .Where(s => s.IsFinish).ToList();
            BindCustomerSatisfaction(surveys);
        }

        private void BindMostRedeemedDays(List<Bookings> bookings)
        {
            var redeemed = bookings.Where(b => b.RedeemedDate.HasValue)
                .GroupBy(p => p.RedeemedDate.Value.ToLosAngerlesTimeWithTimeZone(PublicHotel.TimeZoneId).DayOfWeek,
                    p => p,
                    (key, g) => new
                    {
                        Date = key,
                        Count = g.Count(),
                        DateString = g.First().RedeemedDate.Value.ToLosAngerlesTimeWithTimeZone(PublicHotel.TimeZoneId).ToString("ddd")
                    })
                .OrderBy(x => ((int)x.Date + 6) % 7)
                .ToList();
            if (redeemed.Count < 7)
            {
                var allWeekDays = new[] { "Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun" };
                var dictionary = new Dictionary<string, string>
                {
                    {"Mon", "Monday"},
                    {"Tue", "Tuesday"},
                    {"Wed", "Wednesday"},
                    {"Thu", "Thursday"},
                    {"Fri", "Friday"},
                    {"Sat", "Saturday"},
                    {"Sun", "Sunday"}
                };
                var exclude = allWeekDays.Except(redeemed.Select(x => x.DateString).ToList()).ToList();
                exclude.ForEach(day =>
                {
                    DayOfWeek dayofWeek;
                    if (Enum.TryParse<DayOfWeek>(dictionary[day], out dayofWeek))
                    {
                        redeemed.Add(new
                        {
                            Date = dayofWeek,
                            Count = 0,
                            DateString = day
                        });
                    }
                });
                redeemed = redeemed.OrderBy(x => ((int)x.Date + 6) % 7)
                    .ToList();
            }
            var totalRedeemed = redeemed.Sum(x => x.Count) > 0 ? redeemed.Sum(x => x.Count) : 1;

            MostRedeemedDays = string.Format("{0}", String.Join(",", redeemed.Select(x => (int) Math.Round((double) (x.Count * 100) / totalRedeemed))));
        }

        private void BindCustomerSatisfaction(List<Surveys> surveys)
        {
            var rates = surveys.GroupBy(r => r.Rating,
                    r => r,
                    (key, g) => new
                    {
                        Value = key,
                        Count = g.Count()
                    }).OrderByDescending(x => x.Value).ToList();
            var maxRate = rates.Sum(x => x.Count) > 0 ? rates.Sum(x => x.Count) : 0;

            if (rates.Count < 5)
            {
                for (int i = 1; i <= 5; i++)
                {
                    if (rates.FirstOrDefault(x => x.Value.HasValue && (int)x.Value.Value == i) == null)
                    {
                        rates.Add(new
                        {
                            Value = (double?)i,
                            Count = 0
                        });
                    }
                }

                rates = rates.OrderByDescending(x => x.Value).ToList();
            }

            CustomerSatisfaction = string.Format("{0}", String.Join(",", rates.Select(x => (int)Math.Round((double)(x.Count * 100) / maxRate))));
        }

        private void BindHotelVisited()
        {
            var visited = _bookingRepository.GetHotelVisited(PublicHotel.HotelId).Take(10).ToList();
            var maxVisited = visited.Any() ? visited.Sum(x => x.Count) : 0;
            HotelVisited = string.Format("{0}", String.Join(",", visited.Select(x => (int)Math.Round((double)(x.Count * 100) / maxVisited))));
            HotelVisitedCateogries = String.Join(",", visited.Select(x => string.Format("'{0}'", x.Name)));
        }

        private void BindNewAndReturning(List<Bookings> bookings)
        {
            var newAndReturning = new List<NewAndReturnObject>();
            ListDates.ForEach(date =>
            {
                var totalBookings = bookings.Where(x => x.BookedDate.ToLosAngerlesTimeWithTimeZone(PublicHotel.TimeZoneId).Date == date.Date).ToList();
                var customerIds = totalBookings.Select(b => b.CustomerId).Distinct().ToList();
                var allBookings = (from ci in _bookingRepository.CustomerInfoList
                    join b in _bookingRepository.BookingList on ci.CustomerId equals b.CustomerId
                    where customerIds.Contains(b.CustomerId)
                    group b by b.CustomerId into g
                    select new
                    {
                        BookingId = g.Key,
                        Count = g.Count()
                    }).ToList();
                newAndReturning.Add(new NewAndReturnObject
                {
                    BookedDate = date,
                    Count = totalBookings.Count,
                    NewCustomer = allBookings.Count(x => x.Count == 1),
                    ReturnCustomer = totalBookings.Count - allBookings.Count(x => x.Count == 1)
                });
            });

            var totalNewAndReturn = newAndReturning.Any() ? newAndReturning.Sum(x => x.Count) : 0;
            int percentNew = totalNewAndReturn > 0
                ? (int) Math.Round((double) (newAndReturning.Sum(x => x.NewCustomer) * 100) / totalNewAndReturn)
                : 0;
            int percentReturning = totalNewAndReturn > 0 
                ? (int) Math.Round((double) (newAndReturning.Sum(x => x.ReturnCustomer) * 100) / totalNewAndReturn)
                : 0;

            NewAndReturningCategory = String.Join(",", newAndReturning.Select(x => string.Format("'{0:MM/dd/yy}'", x.BookedDate)));
            PercentNew = string.Format("{0}% New", percentNew);
            PercentReturning = string.Format("{0}% Returning", percentReturning);
            NewData = string.Format("{0}", String.Join(",", newAndReturning.Select(x => x.NewCustomer)));
            ReturningData = string.Format("{0}", String.Join(",", newAndReturning.Select(x => x.ReturnCustomer)));
        }

        private void BindRedemtionFrequency(List<Bookings> bookings)
        {
            var frequencyData = bookings.GroupBy(b => b.CustomerId,
                        b => b,
                        (key, g) => new
                        {
                            CustomerId = key,
                            Count = g.Count()
                        }).GroupBy(x => x.Count,
                        x => x,
                        (key, g) => new DataObject
                        {
                            RedeemedFrequency = key,
                            Count = g.Count()
                        })
                        .OrderBy(x => x.RedeemedFrequency)
                        .ToList();

            var totalRedeemed = frequencyData.Sum(x => x.Count) > 0 ? frequencyData.Sum(x => x.Count) : 1;
            var max = frequencyData.Any() ? frequencyData.Max(x => x.RedeemedFrequency) : 0;

            if (frequencyData.Count < max)
            {
                for (int i = 1; i <= max; i++)
                {
                    var data = frequencyData.FirstOrDefault(x => x.RedeemedFrequency == i);
                    if (data == null)
                    {
                        frequencyData.Add(new DataObject
                        {
                            RedeemedFrequency = i,
                            Count = 0
                        });
                    }
                }

                frequencyData = frequencyData.OrderBy(x => x.RedeemedFrequency).ToList();
            }

            FrequencyCategory = String.Join(",", frequencyData.Select(x => string.Format("'{0} visits'", x.RedeemedFrequency)));
            Frequency = String.Join(",", frequencyData.Select(x => (int)Math.Round((double)(x.Count * 100) / totalRedeemed)));
        }

        private void BindDaysRedeemed(List<Bookings> bookings)
        {
            var redeemedDays = bookings.Where(b => b.RedeemedDate.HasValue)
                .Select(x => (x.RedeemedDate.Value.ToLosAngerlesTimeWithTimeZone(PublicHotel.TimeZoneId).Date - x.BookedDate.ToLosAngerlesTimeWithTimeZone(PublicHotel.TimeZoneId).Date).Days)
                .GroupBy(x => x,
                    (key, g) => new DataObject
                    {
                        Days = key,
                        Count = g.Count()
                    })
                    .OrderBy(x => x.Days)
                    .ToList();

            var totalRedeemed = redeemedDays.Sum(x => x.Count) > 0 ? redeemedDays.Sum(x => x.Count) : 1;
            var maxDays = redeemedDays.Any() ? redeemedDays.Max(x => x.Days) : 0;

            if (redeemedDays.Count < maxDays)
            {
                for (int i = 0; i < maxDays; i++)
                {
                    var days = redeemedDays.FirstOrDefault(x => x.Days == i);
                    if (days == null)
                    {
                        redeemedDays.Add(new DataObject
                        {
                            Days = i,
                            Count = 0
                        });
                    }
                }
                redeemedDays = redeemedDays.OrderBy(x => x.Days).ToList();
            }

            DaysRedeemedCategories = String.Join(",", redeemedDays.Select(x => string.Format("'{0} days'", x.Days)));
            DaysRedeemed = String.Join(",", redeemedDays.Select(x => (int)Math.Round((double)(x.Count * 100) / totalRedeemed)));
        }

        private void BindHoursRedeemed(List<Bookings> bookings)
        {
            var mostRedeemedHours = bookings.Where(b => b.RedeemedDate.HasValue)
                .GroupBy(x => x.RedeemedDate.Value.ToLosAngerlesTimeWithTimeZone(PublicHotel.TimeZoneId).Hour,
                    (key, g) => new DataObject
                    {
                        Hours = key,
                        Count = g.Count()
                    })
                    .OrderBy(x => x.Hours)
                    .ToList();

            var totalRedeemed = mostRedeemedHours.Sum(x => x.Count) > 0 ? mostRedeemedHours.Sum(x => x.Count) : 1;

            if (mostRedeemedHours.Count < 24)
            {
                for (var i = 0; i <= 23; i++)
                {
                    var hours = mostRedeemedHours.FirstOrDefault(x => x.Hours == i);
                    if (hours == null)
                    {
                        mostRedeemedHours.Add(new DataObject
                        {
                            Hours = i,
                            Count = 0
                        });
                    }
                }
                mostRedeemedHours = mostRedeemedHours.OrderBy(x => x.Hours).ToList();
            }

            HoursRedeemedCategories = String.Join(",", mostRedeemedHours.Select(x => string.Format("'{0} hour'", x.Hours)));
            HoursRedeemed = String.Join(",", mostRedeemedHours.Select(x => (int)Math.Round((double)(x.Count * 100) / totalRedeemed)));
        }
    }
}