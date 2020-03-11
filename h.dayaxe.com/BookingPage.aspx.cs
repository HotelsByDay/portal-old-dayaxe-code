using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using AutoMapper;
using CsvHelper;
using DayaxeDal;
using DayaxeDal.Custom;
using DayaxeDal.Extensions;
using DayaxeDal.Parameters;
using DayaxeDal.Repositories;
using Newtonsoft.Json;

namespace h.dayaxe.com
{
    public partial class BookingPage : BasePageProduct
    {
        protected CustomerInfos PublicUser { get; set; }

        private readonly BookingRepository _bookingRepository = new BookingRepository();
        private readonly HotelRepository _hotelRepository = new HotelRepository();
        private Hotels _hotels;
        private List<Bookings> _bookings;

        protected void Page_Init(object sender, EventArgs e)
        {
            try
            {
                string sessionUser = Session["CurrentUser"] != null ? Session["CurrentUser"].ToString() : string.Empty;
                PublicUser = JsonConvert.DeserializeObject<CustomerInfos>(sessionUser);
                Session["Active"] = "Booking";
                if (PublicUser == null)
                {
                    Response.Redirect(Constant.DefaultPage);
                }

                string sessionHotel = Session["Hotel"] != null ? Session["Hotel"].ToString() : string.Empty;
                _hotels = _hotelRepository.GetById(int.Parse(sessionHotel));

                if (!IsPostBack)
                {
                    InitSearch();
                }
            }
            catch (Exception ex)
            {
                var logs = new Logs
                {
                    LogKey = "Admin_BookingPage_Error",
                    UpdatedBy = PublicUser != null ? PublicUser.CustomerId : 1,
                    UpdatedDate = DateTime.UtcNow,
                    UpdatedContent = string.Format("{0} - {1} - {2}", ex.Message, ex.StackTrace, ex.Source)
                };
                _bookingRepository.AddLog(logs);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
        
        }

        private void BindRepeater(List<Bookings> bookings)
        {
            HistoricalRepeater.DataSource = bookings;
            HistoricalRepeater.DataBind();
        }

        protected void HistoricalRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var rowHistory = (HtmlTableRow)e.Item.FindControl("rowHistory");
                rowHistory.Attributes.Add("class", "alternative");
            }

            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var booking = (Bookings) e.Item.DataItem;
                var checkInDateLabel = (Label) e.Item.FindControl("CheckInDateLabel");

                if (booking.PassStatus != (int)Enums.BookingStatus.Redeemed)
                {
                    checkInDateLabel.Text = booking.CheckinDate.HasValue
                        ? booking.CheckinDate.Value.ToLosAngerlesTimeWithTimeZone(_hotels.TimeZoneId).ToString("M/d/yy")
                        : "";
                }
                else
                {
                    checkInDateLabel.Text = booking.RedeemedDate.HasValue
                        ? booking.RedeemedDate.Value.ToLosAngerlesTimeWithTimeZone(PublicHotel.TimeZoneId).ToString("M/d/yy hh:mm tt")
                        : "";
                }
                
                if (booking.PassStatus == (int) Enums.BookingStatus.Active)
                {
                    var redeemButton = (Button)e.Item.FindControl("RedeemButton");
                    redeemButton.Visible = true;
                }
            }

            if (e.Item.ItemType == ListItemType.Footer)
            {
                var totalPass = (Literal)e.Item.FindControl("TotalPass");
                var spendAvg = (Literal)e.Item.FindControl("SpendAvg");
                var ratingAvg = (Literal)e.Item.FindControl("RatingAvg");
                var totalRedeemed = (Literal)e.Item.FindControl("TotalRedeemed");
                if (_bookings.Any())
                {
                    totalPass.Text = _bookings.Count + " PASSES";
                    spendAvg.Text = string.Format("AVG ${0:N0}", _bookings.Where(x => x.EstSpend.HasValue).Average(x => x.EstSpend));
                    var ratingBooking = _bookings.Where(x => x.UserRating > 0).ToList();
                    if (ratingBooking.Any())
                    {
                        ratingAvg.Text = "AVG " + ratingBooking.Average(x => x.UserRating).ToString("###.#");
                    }

                    totalRedeemed.Text = _bookings.Count(x => x.PassStatus == (int)Enums.PassStatus.Redeemed) + " REDEEMED";
                }
                else
                {
                    totalPass.Text = "0 PASSES";
                    spendAvg.Text = "AVG $";
                    ratingAvg.Text = "AVG ";
                    totalRedeemed.Text = "0 REDEEMED";
                }

                var litPage = (Literal)e.Item.FindControl("LitPage");
                var totalHotel = _bookings.Count;
                var totalPage = totalHotel / Constant.ItemPerPage + (totalHotel % Constant.ItemPerPage != 0 ? 1 : 0);
                litPage.Text = string.Format("Page {0} of {1}", Session["CurrentPage"], totalPage);
            }
        }

        protected void SelectedFilterDdl_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            DateFrom.Visible = false;
            DateTo.Visible = false;
            Search.Visible = false;
            CustomForm.Visible = false;
            DateTime startDate;
            var searchBookingsParams = new SearchBookingsParams
            {
                HotelId = _hotels.HotelId,
                IsBookingForRevenue = false,
                FilterText = SearchText.Text.Trim()
            };
            switch (SelectedFilterDdl.SelectedValue)
            {
                case "All":
                    var searchAllParam = new SearchAllBookingsParams
                    {
                        HotelId = _hotels.HotelId,
                        IsForRevenue = false
                    };
                    _bookings = _bookingRepository.GetAllBookingsOfHotel(searchAllParam);
                    break;
                case "Today":
                    var searchTodayParam = new SearchAllBookingsTodayParams
                    {
                        HotelId = _hotels.HotelId,
                        IsForRevenue = false
                    };
                    _bookings = _bookingRepository.GetAllBookingsToday(searchTodayParam);
                    break;
                case "ThisWeek":
                    startDate = DateTime.UtcNow.ToLosAngerlesTimeWithTimeZone(PublicHotel.TimeZoneId).StartOfWeek(DayOfWeek.Monday);
                    searchBookingsParams.StartDate = startDate;
                    searchBookingsParams.EndDate = startDate.AddDays(6);
                    _bookings = _bookingRepository.GetAllbookingsByRange(searchBookingsParams);
                    break;
                case "ThisMonth":
                    startDate = new DateTime(DateTime.UtcNow.ToLosAngerlesTimeWithTimeZone(PublicHotel.TimeZoneId).Year,
                        DateTime.UtcNow.ToLosAngerlesTimeWithTimeZone(PublicHotel.TimeZoneId).Month, 1);
                    searchBookingsParams.StartDate = startDate;
                    searchBookingsParams.EndDate = startDate.AddDays(DateTime.DaysInMonth(startDate.Year, startDate.Month) - 1);
                    _bookings = _bookingRepository.GetAllbookingsByRange(searchBookingsParams);
                    break;
                case "Custom":
                    DateFrom.Visible = true;
                    DateTo.Visible = true;
                    Search.Visible = true;
                    CustomForm.Visible = true;
                    _bookings = new List<Bookings>();
                    break;
            }
            SelectedFilterBy.Text = SelectedFilterDdl.SelectedItem.Text;

            Session["CurrentPage"] = 1;
            BindRepeater(_bookings.Take(Constant.ItemPerPage).ToList());
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
            var searchBookingsParams = new SearchBookingsParams
            {
                HotelId = _hotels.HotelId,
                StartDate = startDate,
                EndDate = endDate,
                IsBookingForRevenue = false,
                FilterText = SearchText.Text.Trim()
            };
            _bookings = _bookingRepository.GetAllbookingsByRange(searchBookingsParams);

            Session["CurrentPage"] = 1;
            BindRepeater(_bookings.Take(Constant.ItemPerPage).ToList());
        }

        protected void SearchBooking(object sender, EventArgs e)
        {
            DateTime startDate;
            var searchBookingsParams = new SearchBookingsParams
            {
                HotelId = _hotels.HotelId,
                IsBookingForRevenue = false,
                FilterText = SearchText.Text.Trim()
            };
            switch (SelectedFilterDdl.SelectedValue)
            {
                case "All":
                    var searchAllParam = new SearchAllBookingsParams
                    {
                        HotelId = _hotels.HotelId,
                        IsForRevenue = false,
                        FilterText = SearchText.Text.Trim()
                    };
                    _bookings = _bookingRepository.GetAllBookingsOfHotel(searchAllParam);
                    break;
                case "Today":
                    var searchTodayParam = new SearchAllBookingsTodayParams
                    {
                        HotelId = _hotels.HotelId,
                        IsForRevenue = false
                    };
                    _bookings = _bookingRepository.GetAllBookingsToday(searchTodayParam);
                    break;
                case "ThisWeek":
                    startDate = DateTime.UtcNow.ToLosAngerlesTimeWithTimeZone(PublicHotel.TimeZoneId).StartOfWeek(DayOfWeek.Monday);
                    searchBookingsParams.StartDate = startDate;
                    searchBookingsParams.EndDate = startDate.AddDays(6);
                    _bookings = _bookingRepository.GetAllbookingsByRange(searchBookingsParams);
                    break;
                case "ThisMonth":
                    startDate = new DateTime(DateTime.UtcNow.ToLosAngerlesTimeWithTimeZone(PublicHotel.TimeZoneId).Year,
                        DateTime.UtcNow.ToLosAngerlesTimeWithTimeZone(PublicHotel.TimeZoneId).Month, 1);
                    searchBookingsParams.StartDate = startDate;
                    searchBookingsParams.EndDate = startDate.AddDays(DateTime.DaysInMonth(startDate.Year, startDate.Month) - 1);
                    _bookings = _bookingRepository.GetAllbookingsByRange(searchBookingsParams);
                    break;
                case "Custom":
                    DateTime.TryParseExact(DateFrom.Text, "MM/dd/yyyy", null, DateTimeStyles.None, out startDate);
                    DateTime endDate;
                    DateTime.TryParseExact(DateTo.Text, "MM/dd/yyyy", null, DateTimeStyles.None, out endDate);
                    searchBookingsParams.StartDate = startDate;
                    searchBookingsParams.EndDate = endDate;
                    searchBookingsParams.FilterText = SearchText.Text.Trim();
                    _bookings = _bookingRepository.GetAllbookingsByRange(searchBookingsParams);
                    break;
            }

            Session["CurrentPage"] = 1;
            BindRepeater(_bookings.Take(Constant.ItemPerPage).ToList());
        }

        protected void Previous_OnClick(object sender, EventArgs e)
        {
            int currentPage = int.Parse(Session["CurrentPage"].ToString());
            _bookings = GetBookingByCurrentCondition();
            var bookings = _bookings.Skip((currentPage - 2) * Constant.ItemPerPage).Take(Constant.ItemPerPage).ToList();
            if (bookings.Any() && currentPage - 2 >= 0)
            {
                Session["CurrentPage"] = currentPage - 1;
                BindRepeater(bookings);
            }
        }

        protected void Next_OnClick(object sender, EventArgs e)
        {
            int currentPage = int.Parse(Session["CurrentPage"].ToString());
            _bookings = GetBookingByCurrentCondition();
            var bookings = _bookings.Skip(currentPage * Constant.ItemPerPage).Take(Constant.ItemPerPage).ToList();
            if (bookings.Any())
            {
                Session["CurrentPage"] = currentPage + 1;
                BindRepeater(bookings);
            }
        }

        protected void ExportToExcelButtonOnClick(object sender, EventArgs e)
        {
            var bookings = GetBookingByCurrentCondition();

            if (bookings.Any())
            {
                var exports = Mapper.Map<List<Bookings>, List<ExportBookingObject>>(bookings);
                exports.Insert(0, new ExportBookingObject
                {
                    BookingId = "Booking ID",
                    BookingCode = "Booking Code",
                    PassStatus = "Pass Status",
                    ProductName = "Product Name",
                    GuestName = "Guest Name",
                    BookedDate = "Booked Date",
                    CheckInDate = "Check-in Date",
                    RedeemedDate = "Redeemed",
                    TicketQuantity = "Ticket Quantity",
                    PerTicketPrice = "Per Ticket Price",
                    GrossEarnings = "Gross Earnings",
                    Rating = "Rating",
                    Feedback = "Feedback",
                    EstSpend = "Est Spend",
                    Paid = "Paid"
                });

                var fileName = string.Format("Bookings_{0}_{1:yyyyMMddhhmm}.csv", _hotels.HotelName.Replace(' ', '_'), 
                    DateTime.UtcNow.ToLosAngerlesTimeWithTimeZone(PublicHotel.TimeZoneId));
                using (TextWriter textWriter = File.CreateText(Server.MapPath(string.Format("/Helper/{0}", fileName))))
                {
                    var csv = new CsvWriter(textWriter);
                    csv.Configuration.Encoding = Encoding.UTF8;
                    csv.Configuration.Delimiter = ",";
                    csv.Configuration.Quote = '"';
                    csv.Configuration.QuoteAllFields = true;
                    csv.Configuration.HasHeaderRecord = false;

                    csv.WriteRecords(exports);
                }

                Response.ContentType = "Application/vnd.ms-excel";
                Response.AppendHeader("Content-Disposition", "attachment; filename=" + fileName);
                Response.TransmitFile(Server.MapPath("~/Helper/" + fileName));
                Response.End();
            }
        }

        private List<Bookings> GetBookingByCurrentCondition()
        {
            var bookings = new List<Bookings>();
            DateTime startDate;
            var param = new SearchBookingsParams
            {
                HotelId = _hotels.HotelId,
                FilterText = SearchText.Text.Trim()
            };
            switch (SelectedFilterDdl.SelectedValue)
            {
                case "All":
                    var searchAllParam = new SearchAllBookingsParams
                    {
                        HotelId = _hotels.HotelId,
                        IsForRevenue = false,
                        FilterText = SearchText.Text.Trim()
                    };
                    bookings = _bookingRepository.GetAllBookingsOfHotel(searchAllParam);
                    break;
                case "Today":
                    var searchTodayParam = new SearchAllBookingsTodayParams
                    {
                        HotelId = _hotels.HotelId,
                        IsForRevenue = false
                    };
                    bookings = _bookingRepository.GetAllBookingsToday(searchTodayParam);
                    break;
                case "ThisWeek":
                    startDate = DateTime.UtcNow.ToLosAngerlesTimeWithTimeZone(PublicHotel.TimeZoneId).StartOfWeek(DayOfWeek.Monday);
                    param.StartDate = startDate;
                    param.EndDate = startDate.AddDays(6);
                    param.IsBookingForRevenue = false;
                    bookings = _bookingRepository.GetAllbookingsByRange(param);
                    break;
                case "ThisMonth":
                    startDate = new DateTime(DateTime.UtcNow.ToLosAngerlesTimeWithTimeZone(PublicHotel.TimeZoneId).Year,
                        DateTime.UtcNow.ToLosAngerlesTimeWithTimeZone(PublicHotel.TimeZoneId).Month, 1);
                    param.StartDate = startDate;
                    param.EndDate = startDate.AddDays(DateTime.DaysInMonth(startDate.Year, startDate.Month) - 1);
                    param.IsBookingForRevenue = false;
                    bookings = _bookingRepository.GetAllbookingsByRange(param);
                    break;
                case "Custom":
                    DateTime endDate;
                    if (string.IsNullOrEmpty(DateFrom.Text) || !DateTime.TryParseExact(DateFrom.Text, "MM/dd/yyyy", null, DateTimeStyles.None, out startDate))
                    {
                        MessageLabel.Text = "Please enter From and To date.";
                        return bookings;
                    }
                    if (string.IsNullOrEmpty(DateTo.Text) || !DateTime.TryParseExact(DateTo.Text, "MM/dd/yyyy", null, DateTimeStyles.None, out endDate))
                    {
                        MessageLabel.Text = "Please enter From and To date.";
                        return bookings;
                    }
                    param.StartDate = startDate;
                    param.EndDate = endDate;
                    param.IsBookingForRevenue = false;
                    bookings = _bookingRepository.GetAllbookingsByRange(param);
                    break;
            }

            return bookings;
        }

        protected void RedeemButton_OnCommand(object sender, CommandEventArgs e)
        {
            long bookingId = Convert.ToInt64(e.CommandArgument);

            var bookings = _bookingRepository.GetById(bookingId);
            var products = _bookingRepository.ProductList.First(p => p.ProductId == bookings.ProductId);
            var customerInfos = _bookingRepository.CustomerInfoList.First(ci => ci.CustomerId == bookings.CustomerId);
            FullNameLabel.Text = string.Format("{0} {1}", customerInfos.FirstName, customerInfos.LastName);
            CheckInDateLabel.Text = bookings.CheckinDate.HasValue
                ? bookings.CheckinDate.Value.ToLosAngerlesTimeWithTimeZone(PublicHotel.TimeZoneId).ToString(Constant.DateFormat)
                : string.Empty;
            TicketsLabel.Text = bookings.Quantity.ToString();
            EachTicketLabel.Text = string.Format("{0} guest(s)", products.MaxGuest);
            BookingCodeLabel.Text = bookings.BookingIdString;
            BookingIdHidden.Value = bookings.BookingId.ToString();

            ValidateMessage.Text = string.Empty;

            ScriptManager.RegisterClientScriptBlock(ValidateUpdatePanel, typeof(string), "ValidateBookings", "$(function(){$('#redeemModal').modal('show');});", true);
        }

        protected void ValidateButton_OnClick(object sender, EventArgs e)
        {
            var bookingId = int.Parse(BookingIdHidden.Value);
            var response = _bookingRepository.ValidatePinCode(bookingId, string.Empty, true);

            var javaScriptSerializer = new JavaScriptSerializer();
            var responseData = javaScriptSerializer.Deserialize<Response>(response);
            string script = string.Empty;

            if (responseData.IsSuccess)
            {
                var bookings = _bookingRepository.GetById(bookingId);
                var products = _bookingRepository.ProductList.First(p => p.ProductId == bookings.ProductId);
                var hotels = _bookingRepository.HotelList.First(h => h.HotelId == products.HotelId);
                var customerInfo =
                    _bookingRepository.CustomerInfoList.First(ci => ci.CustomerId == bookings.CustomerId);
                var schedules = new Schedules
                {
                    BookingId = bookingId,
                    Name = "EmailSurvey",
                    ScheduleSendType = (int) Enums.ScheduleSendType.IsEmailSurvey,
                    Status = (int) Enums.ScheduleType.NotRun,
                    SendAt = DateTime.UtcNow.AddMinutes(AppConfiguration.SendEmailSurveyAfterMinutes)
                };
                _bookingRepository.AddSchedule(schedules);

                // Insert Schedule Send Add On Notification
                var schedulesAddOn = new Schedules
                {
                    ScheduleSendType = (int) Enums.ScheduleSendType.IsAddOnNotificationAfterRedemption,
                    Name = "Send Add-On Notification Redemption",
                    Status = (int) Enums.ScheduleType.NotRun,
                    BookingId = bookingId,
                    SendAt = DateTime.UtcNow.AddMinutes(AppConfiguration.SendEmailSurveyAfterMinutes)
                };
                _bookingRepository.AddSchedule(schedulesAddOn);

                script = string.Format(@"_learnq.push(['track', 'Booking Status Changed', {{
                    '$email': '{0}',
                    'email': '{0}',
                    'customer_id': '{1}',
                    'item_type': '{2}',
                    'item_name': '{3}',
                    'item_hotel_name': '{4}',
                    'item_id': '{5}',
                    'item_location': '{6}',
                    'item_photo': '{7}',
                    'booking_id': '{8}',
                    'booking_code': '{9}',
                    'booking_url': '{10}',
                    'booking_status': '{11}'
                }}]);",
                    customerInfo.EmailAddress,
                    customerInfo.CustomerId,
                    Helper.GetStringPassByProductType(products.ProductType),
                    products.ProductName,
                    hotels.HotelName,
                    products.ProductId,
                    string.Format("{0}, {1}", hotels.Neighborhood, hotels.City),
                    string.Format("{0}",
                        new Uri(new Uri(AppConfiguration.CdnImageUrlDefault), products.ImageUrl).AbsoluteUri),
                    bookings.BookingId,
                    bookings.BookingIdString,
                    AppConfiguration.DayaxeClientUrl + "/" + bookings.BookingId + Constant.ViewDayPassPage,
                    ((Enums.BookingStatus) bookings.PassStatus).ToString()
                );
                ScriptManager.RegisterClientScriptBlock(ValidateUpdatePanel,typeof(string), "HideRedeemEvent", "$(function(){$('#redeemModal').on('hidden.bs.modal', function (e) {window.location.reload();});});", true);
                ValidateMessage.Text = "Success! This ticket is valid! You may issue this customer an access card";
                ValidateMessage.CssClass = "success-message";
                ValidateButton.Attributes["data-dismiss"] = "modal";
                ValidateButton.Text = "DONE";
            }
            else
            {
                ValidateMessage.Text = responseData.Message;
                ValidateMessage.CssClass = "error-message";
            }

            ScriptManager.RegisterClientScriptBlock(ValidateUpdatePanel, typeof(string), "ValidateBookings", string.Format("$(function(){{ {0} $('.modal-backdrop').remove(); $('#redeemModal').modal('show');}});", script), true);
        }

        protected void ClearSearchButton_OnClick(object sender, EventArgs e)
        {
            SelectedFilterDdl.SelectedIndex = 0;
            CustomForm.Visible = false;
            SearchText.Text = string.Empty;
            SelectedFilterBy.Text = SelectedFilterDdl.Text;
            InitSearch();
        }

        private void InitSearch()
        {
            var searchAllParam = new SearchAllBookingsParams
            {
                HotelId = _hotels.HotelId,
                IsForRevenue = false
            };
            _bookings = _bookingRepository.GetAllBookingsOfHotel(searchAllParam)
                .ToList();
            Session["CurrentPage"] = 1;
            SelectedFilterBy.Text = SelectedFilterDdl.Text;
            BindRepeater(_bookings.Take(Constant.ItemPerPage).ToList());
        }
    }
}