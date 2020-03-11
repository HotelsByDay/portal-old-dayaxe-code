using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.UI;
using DayaxeDal;
using DayaxeDal.Custom;
using DayaxeDal.Extensions;
using DayaxeDal.Parameters;
using DayaxeDal.Repositories;
using Newtonsoft.Json;
using Stripe;

namespace dayaxe.com
{
    public partial class ViewDayPass : BasePage
    {
        protected Bookings PublicBooking { get; set; }
        protected Hotels PublicHotel { get; set; }
        protected Products PublicProduct { get; set; }
        protected Markets PublicMarkets { get; set; }
        protected string ProductTypeTrackString { get; set; }
        protected string PublicTickets { get; set; }
        protected string ProductBlockoutDate { get; set; }
        private DateTime _selectedCheckInDate = DateTime.UtcNow.ToLosAngerlesTime();
        private string StripeCardString { get; set; }

        private readonly BookingRepository _bookingRepository = new BookingRepository();
        private readonly ProductRepository _productRepository = new ProductRepository();
        private readonly HotelRepository _hotelRepository = new HotelRepository();

        protected void Page_Init(object sender, EventArgs e)
        {
            if (Page.RouteData.Values["bookingId"] != null)
            {
                int bookingId;
                if (int.TryParse(Page.RouteData.Values["bookingId"].ToString(), out bookingId))
                {
                    PublicBooking = _bookingRepository.GetById(bookingId);
                }
            }

            if (PublicCustomerInfos == null)
            {
                Response.Redirect(string.Format(Constant.SignIpPage, HttpUtility.UrlEncode(Request.Url.PathAndQuery)));
            }
            else if (PublicBooking != null &&
                     PublicCustomerInfos.CustomerId != PublicBooking.CustomerId)
            {
                Response.Redirect(Constant.InvalidTicketPage);
            }

            if (PublicBooking == null)
            {
                Response.Redirect(!string.IsNullOrEmpty((string)Session["SearchPage"])
                    ? Session["SearchPage"].ToString()
                    : Constant.SearchPageDefault);
            }

            PublicProduct = _productRepository.GetById(PublicBooking != null ? PublicBooking.ProductId : 0);
            PublicHotel = _hotelRepository.GetById(PublicProduct.HotelId);
            PublicMarkets = _productRepository.GetMarketsByHotelId(PublicProduct.HotelId);
            ProductTypeTrackString = Helper.GetStringPassByProductType(PublicProduct.ProductType);

            if (!string.IsNullOrEmpty(PublicHotel.HotelDiscountCode) && PublicHotel.HotelDiscountPercent > 0)
            {
                DiscountRow.Visible = true;
            }
            if (Request.Browser["IsMobileDevice"] == "true")
            {
                containerDesktop.Visible = false;
                ContainerMobile.Visible = true;
            }
            else
            {
                containerDesktop.Visible = true;
                ContainerMobile.Visible = false;
            }

            var tickets = _productRepository.GetTicketsFuture(PublicProduct.ProductId);
            string json = JsonConvert.SerializeObject(tickets, CustomSettings.SerializerSettings());
            PublicTickets = json;

            string tab = string.Empty;
            var amenties = PublicHotel.AmentiesHotels;
            double item = amenties.ActiveFeatures > 0 ? 12 / amenties.ActiveFeatures : 0;
            var itemClass = item.ToString(CultureInfo.InvariantCulture);
            if (12 % amenties.ActiveFeatures != 0)
            {
                itemClass = "2-5";
            }
            string activeClass = "active";
            if (amenties.PoolActive)
            {
                tab += "<li role=\"presentation\" class=\"col-xs-" + itemClass + " " + activeClass + "\">"
                       + "<a href=\"#restaurant\" aria-controls=\"restaurant\" role=\"tab\" data-toggle=\"tab\" aria-expanded=\"true\">"
                       + "<img src=\"/images/pool-icon-white.png\" class=\"img-responsive\" />"
                       + "</a>"
                       + "<div class='tab-mask'></div></li>";
                activeClass = string.Empty;
            }
            if (amenties.GymActive)
            {
                tab += "<li role=\"presentation\" class=\"col-xs-" + itemClass + " " + activeClass + "\">"
                       + "<a href=\"#sports-club\" aria-controls=\"sports-club\" role=\"tab\" data-toggle=\"tab\" aria-expanded=\"false\">"
                       + "<img src=\"/images/gym-white.png\" alt=\"\" class=\"img-responsive\" />"
                       + "</a>"
                       + "<div class='tab-mask'></div></li>";
                activeClass = string.Empty;
            }
            if (amenties.SpaActive)
            {
                tab += "<li role=\"presentation\" class=\"col-xs-" + itemClass + " " + activeClass + "\">"
                       + "<a href=\"#spa-club\" aria-controls=\"spa-club\" role=\"tab\" data-toggle=\"tab\" aria-expanded=\"false\">"
                       + "<img src=\"/images/spa-white.png\" alt=\"\" class=\"img-responsive\" />"
                       + "</a>"
                       + "<div class='tab-mask'></div></li>";
                activeClass = string.Empty;
            }
            if (amenties.BusinessActive)
            {
                tab += "<li role=\"presentation\" class=\"col-xs-" + itemClass + " " + activeClass + "\">"
                       + "<a href=\"#pick-up\" class=\"pick-up\" aria-controls=\"pick-up\" role=\"tab\" data-toggle=\"tab\" aria-expanded=\"false\">"
                       + "<img src=\"/images/handshake-white.png\" alt=\"\" class=\"img-responsive\" />"
                       + "</a>"
                       + "<div class='tab-mask'></div></li>";
            }
            if (amenties.DinningActive)
            {
                tab += "<li role=\"presentation\" class=\"col-xs-" + itemClass + " " + activeClass + "\">"
                       + "<a href=\"#dining\" class=\"dining\" aria-controls=\"dining\" role=\"tab\" data-toggle=\"tab\" aria-expanded=\"false\">"
                       + "<img src=\"/images/icon_dinning.png\" alt=\"\" class=\"img-responsive\" />"
                       + "</a>"
                       + "<div class='tab-mask'></div></li>";
            }
            if (amenties.EventActive)
            {
                tab += "<li role=\"presentation\" class=\"col-xs-" + itemClass + " " + activeClass + "\">"
                       + "<a href=\"#event\" class=\"event\" aria-controls=\"event\" role=\"tab\" data-toggle=\"tab\" aria-expanded=\"false\">"
                       + "<img src=\"/images/icon_event.png\" alt=\"\" class=\"img-responsive\" />"
                       + "</a>"
                       + "<div class='tab-mask'></div></li>";
            }
            LiMainTab.Text = tab;

            KidAllowLabel.Text = PublicProduct.KidAllowedString;
            CheckInPlaceLit.Text = string.Format("AT {0}",
                string.IsNullOrEmpty(PublicHotel.CheckInPlace) ? Constant.FrontDeskString : PublicHotel.CheckInPlace);

            if (!IsPostBack)
            {
                CheckInDateText.Text = PublicBooking.CheckinDate.HasValue && PublicBooking.CheckinDate.Value.Date != DateTime.MinValue ? 
                    PublicBooking.CheckinDate.Value.ToLosAngerlesTimeWithTimeZone(PublicHotel.TimeZoneId).ToString(Constant.DateFormat) : "Not Selected";
            }

            BindExpiredDate();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (PublicBooking.CheckinDate.HasValue && 
                PublicBooking.CheckinDate.Value.Date != DateTime.MinValue)
            {
                CheckInDateLink.Attributes["class"] = "update-checkin-date";
                CheckInDateLink.Text = "UPDATE<br/>CHECK-IN DATE";
                UpdateNotPossibleLit.Text = "We could not update your check-in date to another day.";
                if (PublicBooking.CheckinDate.Value.Date <= DateTime.UtcNow.ToLosAngerlesTimeWithTimeZone(PublicHotel.TimeZoneId).AddDays(2).Date)
                {
                    CheckInDateDiv.Attributes["class"] += " disabled";
                    CheckInDateDiv.Attributes["disabled"] = "disabled";
                }
            }
            DateTime.TryParseExact(CheckInDateText.Text, "MMM dd, yyyy", null, DateTimeStyles.None, out _selectedCheckInDate);
            if ((IsPostBack && PublicBooking.CheckinDate == null) || (PublicBooking.CheckinDate.HasValue && PublicBooking.CheckinDate.Value.Date != _selectedCheckInDate.Date))
            {
                UpdateNotPossibleLit.Text = string.Format("We could not update your check-in date to <b>{0:MMM dd, yyyy}</b>.", _selectedCheckInDate);
                CheckInDateText.Text = _selectedCheckInDate.ToString(Constant.DateFormat);
                CheckInDateLink.Text = "UPDATE<br/>CHECK-IN DATE";
                CheckInDateLink.CssClass = "update-checkin-date";
                CheckInDateLit.Text = _selectedCheckInDate.ToString(Constant.DateFormat);

                var newPrice = _productRepository.GetById(PublicProduct.ProductId, _selectedCheckInDate).ActualPriceWithDate;
                var actualPrice = GetActualPrice(newPrice);
                var diffPrice = actualPrice * PublicBooking.Quantity - PublicBooking.TotalPrice;

                if (diffPrice > 0)
                {
                    PriceDiffLabel.Text = string.Format("{0} (charge)", Helper.FormatPrice(diffPrice));
                    ConfirmLit.Text =
                        string.Format(
                            "Pressing submit will finalize your selection and you will be charged a <span class='price'>{0}</span> difference in price using your credit card on file.",
                            Helper.FormatPrice(diffPrice));
                }
                else if (diffPrice < 0)
                {
                    PriceDiffLabel.Text = string.Format("{0} (refund)",
                        Helper.FormatPrice(Math.Abs(diffPrice)));
                    ConfirmLit.Text =
                        string.Format(
                            "Pressing submit will finalize your selection and you will be refunded a <span class='price'>{0}</span> difference in price using your credit card on file.",
                            Helper.FormatPrice(Math.Abs(diffPrice)));
                }
                else
                {
                    PriceDiffLabel.Text = Helper.FormatPrice(diffPrice);
                    ConfirmLit.Text = "Pressing submit will finalize your selection.";
                }
            }
            AssignBlackOutDate();

            if (!IsPostBack)
            {
                ClientScript.RegisterStartupScript(typeof(ScriptManager), "Update_Not_Available", "", true);
                ClientScript.RegisterStartupScript(typeof(ScriptManager), "Update_Successfully", "", true);
                ClientScript.RegisterStartupScript(typeof(ScriptManager), "ConfirmUpdate", "", true);
            }
        }

        private void AssignBlackOutDate()
        {
            var blockedDates = _bookingRepository.BlockedDatesCustomPriceList
                .Where(bd => bd.ProductId == PublicProduct.ProductId &&
                             bd.Capacity == 0 &&
                             bd.Date.Date >= DateTime.UtcNow.ToLosAngerlesTimeWithTimeZone(PublicHotel.TimeZoneId).Date)
                .Select(b => "'" + b.Date.ToString("MM/dd/yyyy") + "'")
                .ToList();

            var date = DateTime.UtcNow.ToLosAngerlesTimeWithTimeZone(PublicHotel.TimeZoneId);
            if (date.Hour > 19)
            {
                blockedDates.Add(string.Format("'{0}'", date.ToString("MM/dd/yyyy")));
            }

            ProductBlockoutDate = string.Format("[{0}]", String.Join(",", blockedDates));
        }

        protected void CheckInDateLink_OnClick(object sender, EventArgs e)
        {
            int ticketAvailable;
            bool availableProduct = PublicBooking.CheckinDate == null ||
                                    DateTime.UtcNow.ToLosAngerlesTimeWithTimeZone(PublicHotel.TimeZoneId).AddDays(2).Date <= PublicBooking.CheckinDate.Value;
            var param = new CheckAvailableProductParams
            {
                ProductId = PublicProduct.ProductId,
                CheckInDate = _selectedCheckInDate,
                TotalTicket = PublicBooking.Quantity,
                BookingId = PublicBooking.BookingId,
                IsAdmin = false,
                TimezoneId = PublicHotel.TimeZoneId
            };
            availableProduct &= _productRepository.CheckAvailableProduct(param, out ticketAvailable);

            if (!availableProduct)
            {
                Session.Remove("CheckInDate" + PublicBooking.BookingId);
                CheckInDateLink.Text = "SELECT<br/>CHECK-IN DATE";
                CheckInDateLink.CssClass = "select-checkin-date";
                ScriptManager.RegisterStartupScript(CheckInDateChangePanel, CheckInDateChangePanel.GetType(), "Update_Not_Available", "$(function(){$('#updateFail').modal('show');});", true);
                return;
            }

            var logs = new CustomerCreditLogs();
            bool isRefund = false;
            try
            {
                if (!PublicBooking.HasInvoice)
                {
                    MaintainOldInvoices();
                }

                var newPrice = _productRepository.GetById(PublicProduct.ProductId, _selectedCheckInDate).ActualPriceWithDate;

                double actualPrice = GetActualPrice(newPrice);

                double diffPrice = actualPrice * PublicBooking.Quantity - PublicBooking.TotalPrice;

                // Charge
                if (diffPrice > 0)
                {
                    Charges(diffPrice, newPrice, actualPrice);

                    PublicBooking.TotalPrice += diffPrice;
                    PublicBooking.HotelPrice = actualPrice;
                    PublicBooking.MerchantPrice = newPrice.Price;
                }
                else if (diffPrice < 0) //Refund
                {
                    double payByCard = Math.Abs(diffPrice) - PublicBooking.PayByCredit;
                    if (PublicBooking.PayByCredit > 0)
                    {
                        var market = (from mh in _productRepository.MarketHotelList
                            join m in _productRepository.MarketList on mh.MarketId equals m.Id
                            where mh.HotelId == PublicHotel.HotelId
                            select m).FirstOrDefault();
                        logs = new CustomerCreditLogs
                        {
                            CustomerId = PublicCustomerInfos != null
                                ? PublicCustomerInfos.CustomerId
                                : PublicBooking.CustomerId,
                            ProductId = PublicBooking.ProductId,
                            Description = string.Format("{0} – {1} – {2} – {3}",
                                PublicProduct.ProductName,
                                PublicHotel.HotelName,
                                market != null ? market.LocationName : "",
                                PublicBooking.BookingIdString),
                            CreatedBy = PublicCustomerInfos != null ? PublicCustomerInfos.CustomerId : 0,
                            CreatedDate = DateTime.UtcNow,
                            CreditType = payByCard >= 0
                                ? (byte) Enums.CreditType.FullPurchaseRefund
                                : (byte) Enums.CreditType.PartialPuchaseRefund,
                            ReferralId = 0,
                            BookingId = PublicBooking.BookingId,
                            Status = true,
                            GiftCardId = 0
                        };
                        PublicBooking.PaymentType = (byte) Enums.PaymentType.DayAxeCredit;

                        // Refund All of Credit Used
                        if (payByCard >= 0)
                        {
                            PublicBooking.PayByCredit = 0;
                        }
                        else
                        {
                            PublicBooking.PayByCredit -= Math.Abs(diffPrice);
                        }
                    }

                    // Refund More with Stripe
                    if (payByCard > 0)
                    {
                        Refund(payByCard, new KeyValuePair<double, double>(PublicBooking.MerchantPrice, 0),
                            PublicBooking.HotelPrice);

                        PublicBooking.PaymentType = (byte) Enums.PaymentType.Stripe;

                        var cardService = new StripeCardService();
                        StripeCard stripeCard = cardService.Get(PublicCustomerInfos.StripeCustomerId,
                            PublicCustomerInfos.StripeCardId);
                        StripeCardString = string.Format("{0} {1}", stripeCard.Brand, stripeCard.Last4);
                    }

                    PublicBooking.TotalPrice += diffPrice;
                    PublicBooking.HotelPrice = actualPrice;
                    PublicBooking.MerchantPrice = newPrice.Price;
                    isRefund = true;

                    PublicBooking.StripeCardString = StripeCardString;
                    _bookingRepository.RefundBooking(PublicBooking, logs);
                }
                else
                {
                    PublicBooking.TotalPrice += diffPrice;
                    PublicBooking.HotelPrice = actualPrice;
                    PublicBooking.MerchantPrice = newPrice.Price;
                }
            }
            catch (Exception ex)
            {
                UpdateNotPossibleLit.Text = ex.Message;
                ScriptManager.RegisterStartupScript(CheckInDateChangePanel, CheckInDateChangePanel.GetType(), "UpdateFail", "$(function(){$('#updateFail').modal('show');});", true);
                return;
            }

            PublicBooking.CheckinDate = _selectedCheckInDate.AddHours(Math.Abs(PublicHotel.TimeZoneOffset));
            PublicBooking.ExpiredDate = _selectedCheckInDate.AddDays(1).AddHours(Math.Abs(PublicHotel.TimeZoneOffset));
            if (isRefund)
            {
                _bookingRepository.Update(PublicBooking);
            }
            else
            {
                _bookingRepository.Update(PublicBooking, 0, logs);
            }

            Session.Remove("CheckInDate" + PublicBooking.BookingId);
            BindExpiredDate();
            CacheLayer.ClearAll();
            updateSuccess.Visible = true;
            CheckInDateLink.Text = "SELECT<br/>CHECK-IN DATE";
            CheckInDateLink.CssClass = "select-checkin-date";
            ScriptManager.RegisterStartupScript(CheckInDateChangePanel, CheckInDateChangePanel.GetType(), "ConfirmUpdate", "$(function(){$('#updateSuccess').modal('show');});", true);
        }

        private void BindExpiredDate()
        {
            switch (PublicBooking.PassStatus)
            {
                case (int)Enums.BookingStatus.Active:
                    StatusLabel.Attributes["class"] += " active";
                    DateLabel.Text = PublicBooking.ExpiredDate.HasValue ?
                        string.Format("Expires {0} {1}",
                            PublicBooking.ExpiredDate.Value.AddHours(PublicHotel.TimeZoneOffset).ToString(Constant.ExpiresDateFormat),
                            PublicHotel.TimeZoneId.GetTimeZoneInfo()) : string.Empty;
                    break;
                case (int)Enums.BookingStatus.Expired:
                    StatusLabel.Attributes["class"] += " expired";
                    DateLabel.Text = PublicBooking.ExpiredDate.HasValue ?
                        string.Format("{0} {1}", 
                            PublicBooking.ExpiredDate.Value.AddHours(PublicHotel.TimeZoneOffset).ToString(Constant.ExpiresDateFormat),
                            PublicHotel.TimeZoneId.GetTimeZoneInfo()) : string.Empty;
                    CheckInDateDiv.Attributes["class"] += " disabled";
                    break;
                case (int)Enums.BookingStatus.Redeemed:
                    StatusLabel.Attributes["class"] += " redeemed";
                    DateLabel.Text = PublicBooking.RedeemedDate.HasValue ?
                        string.Format("{0} {1}",
                            PublicBooking.RedeemedDate.Value.AddHours(PublicHotel.TimeZoneOffset).ToString(Constant.ExpiresDateFormat),
                            PublicHotel.TimeZoneId.GetTimeZoneInfo()) : string.Empty;
                    CheckInDateDiv.Attributes["class"] += " disabled";
                    break;
                default:
                    StatusLabel.Attributes["class"] = " redeemed";
                    DateLabel.Text = PublicBooking.CancelDated.HasValue ?
                        string.Format("{0} {1}",
                            PublicBooking.CancelDated.Value.AddHours(PublicHotel.TimeZoneOffset).ToString(Constant.ExpiresDateFormat),
                            PublicHotel.TimeZoneId.GetTimeZoneInfo()) : string.Empty;
                    CheckInDateDiv.Attributes["class"] += " disabled";
                    break;
            }
        }

        protected void CheckInDateText_OnTextChanged(object sender, EventArgs e)
        {
            CheckInDateLink.CssClass += " update";

            int ticketAvailable;
            bool availableProduct = PublicBooking.CheckinDate == null ||
                                    DateTime.UtcNow.ToLosAngerlesTimeWithTimeZone(PublicHotel.TimeZoneId).AddDays(2).Date <= PublicBooking.CheckinDate.Value;

            var param = new CheckAvailableProductParams
            {
                ProductId = PublicProduct.ProductId,
                CheckInDate = _selectedCheckInDate,
                TotalTicket = PublicBooking.Quantity,
                BookingId = PublicBooking.BookingId,
                IsAdmin = false,
                TimezoneId = PublicHotel.TimeZoneId
            };
            availableProduct &= _productRepository.CheckAvailableProduct(param, out ticketAvailable);

            if (!availableProduct)
            {
                Session.Remove("CheckInDate" + PublicBooking.BookingId);
                CheckInDateLink.Text = "SELECT<br/>CHECK-IN DATE";
                CheckInDateLink.CssClass = "select-checkin-date";
                ScriptManager.RegisterStartupScript(CheckInDateChangePanel, CheckInDateChangePanel.GetType(), "Update_Not_Available", "$(function(){$('#updateFail').modal('show');});", true);
                return;
            }

            ScriptManager.RegisterStartupScript(CheckInDateChangePanel, CheckInDateChangePanel.GetType(), "ConfirmUpdate", "$(function(){$('#confirmUpdate').modal('show');});", true);
        }

        private void Charges(double diffPrice, ActualPriceObject newPrice, double actualPrice)
        {
            string description = string.Format("Update Check-In Date {0} - {1} - {2} - {3:MMM dd, yyyy}",
                        ProductTypeTrackString,
                        PublicProduct.ProductName,
                        PublicHotel.HotelName, DateTime.UtcNow.ToLosAngerlesTimeWithTimeZone(PublicHotel.TimeZoneId));
            StripeCharge stripeCharge = CreateCharges(diffPrice, PublicCustomerInfos.StripeCustomerId,
                description);

            var chargeInvoice = new Invoices
            {
                BookingId = PublicBooking.BookingId,
                PassStatus = PublicBooking.PassStatus,
                StripeChargeId = stripeCharge.Id,
                ChargeAmount = diffPrice,
                CreatedDate = DateTime.UtcNow,
                CreatedBy = PublicCustomerInfos.CustomerId,
                InvoiceStatus = (int)Enums.InvoiceStatus.Charge,
                HotelPrice = actualPrice,
                MerchantPrice = newPrice.Price,
                Quantity = PublicBooking.Quantity,
                RefundAmount = 0,
                StripeRefundId = string.Empty,
                StripeRefundStatus = string.Empty,
                TotalPrice = PublicBooking.TotalPrice + diffPrice
            };
            _bookingRepository.AddInvoices(chargeInvoice, string.Empty);
        }

        private void Refund(double diffPrice, KeyValuePair<double, double> newPrice, double actualPrice)
        {
            var bookingInvoices = _bookingRepository.GetInvoicesByBookingId(PublicBooking.BookingId).ToList();
            var chargeInvoices = bookingInvoices
                .Where(x => x.InvoiceStatus == (int)Enums.InvoiceStatus.Charge)
                .OrderBy(x => x.TotalPrice)
                .ToList();

            while (diffPrice > 0)
            {
                var currentCharge = chargeInvoices.FirstOrDefault();
                if (currentCharge != null)
                {
                    var refundOfCurrentCharge = bookingInvoices
                        .Where(x => (x.InvoiceStatus == (int)Enums.InvoiceStatus.PartialRefund || x.InvoiceStatus == (int)Enums.InvoiceStatus.FullRefund) &&
                                    x.StripeChargeId == currentCharge.StripeChargeId)
                        .ToList();

                    var oldRefund = refundOfCurrentCharge.Sum(x => x.RefundAmount/100);
                    var newDiffPrice = currentCharge.ChargeAmount - oldRefund;

                    if (newDiffPrice > 0)
                    {
                        double newRefundPrice = diffPrice;
                        diffPrice -= newDiffPrice;
                        var chargeInvoice = new Invoices
                        {
                            BookingId = PublicBooking.BookingId,
                            PassStatus = PublicBooking.PassStatus,
                            StripeChargeId = currentCharge.StripeChargeId,
                            ChargeAmount = currentCharge.ChargeAmount,
                            CreatedDate = DateTime.UtcNow,
                            CreatedBy = PublicCustomerInfos.CustomerId,
                            HotelPrice = actualPrice,
                            MerchantPrice = newPrice.Key,
                            Quantity = PublicBooking.Quantity,
                            TotalPrice = PublicBooking.TotalPrice
                        };

                        if (diffPrice >= 0)
                        {
                            newRefundPrice = newDiffPrice;
                            chargeInvoice.InvoiceStatus = (int) Enums.InvoiceStatus.FullRefund;
                        }
                        else
                        {
                            chargeInvoice.InvoiceStatus = (int) Enums.InvoiceStatus.PartialRefund;
                        }
                        var refund = CallStripeRefund(currentCharge.StripeChargeId, newRefundPrice);
                        chargeInvoice.RefundAmount = (double)refund.Amount/100;
                        chargeInvoice.StripeRefundId = refund.Id;
                        chargeInvoice.StripeRefundStatus = refund.Status;

                        string requestObj = JsonConvert.SerializeObject(refund, CustomSettings.SerializerSettings());
                        string message = string.Format("{0} Refund Response: {1}, Refund Post Data: {2}",
                            PublicBooking.BookingCode,
                            refund.StripeResponse.ResponseJson,
                            requestObj);
                        _bookingRepository.AddInvoices(chargeInvoice, message);
                    }
                    chargeInvoices.Remove(currentCharge);
                }
            }
        }

        private void MaintainOldInvoices()
        {
            var chargeInvoice = new Invoices
            {
                BookingId = PublicBooking.BookingId,
                PassStatus = PublicBooking.PassStatus,
                StripeChargeId = PublicBooking.StripeChargeId,
                ChargeAmount = PublicBooking.TotalPrice,
                CreatedDate = DateTime.UtcNow,
                CreatedBy = PublicCustomerInfos.CustomerId,
                HotelPrice = PublicBooking.HotelPrice,
                InvoiceStatus = (int)Enums.InvoiceStatus.Charge,
                MerchantPrice = PublicBooking.MerchantPrice,
                Quantity = PublicBooking.Quantity,
                RefundAmount = 0,
                StripeRefundId = string.Empty,
                StripeRefundStatus = string.Empty,
                TotalPrice = PublicBooking.TotalPrice
            };
            _bookingRepository.AddInvoices(chargeInvoice, string.Empty);

            if (!string.IsNullOrEmpty(PublicBooking.StripeRefundTransactionId))
            {
                double refundAmount = PublicBooking.StripeRefundAmount.HasValue
                    ? PublicBooking.StripeRefundAmount.Value
                    : 0;
                var refundInvoice = new Invoices
                {
                    BookingId = PublicBooking.BookingId,
                    PassStatus = PublicBooking.PassStatus,
                    StripeChargeId = string.Empty,
                    ChargeAmount = 0,
                    CreatedDate = DateTime.UtcNow,
                    CreatedBy = PublicCustomerInfos.CustomerId,
                    HotelPrice = PublicBooking.HotelPrice,
                    InvoiceStatus = refundAmount >= PublicBooking.TotalPrice * 100 ? (int)Enums.InvoiceStatus.FullRefund : (int)Enums.InvoiceStatus.PartialRefund,
                    MerchantPrice = PublicBooking.MerchantPrice,
                    Quantity = PublicBooking.Quantity,
                    RefundAmount = (double)refundAmount / 100,
                    StripeRefundId = PublicBooking.StripeRefundTransactionId,
                    StripeRefundStatus = PublicBooking.StripeRefundStatus,
                    TotalPrice = PublicBooking.TotalPrice
                };
                _bookingRepository.AddInvoices(refundInvoice, string.Empty);
            }

            PublicBooking.StripeChargeId = string.Empty;
            PublicBooking.StripeRefundAmount = 0;
            PublicBooking.StripeRefundStatus = string.Empty;
            PublicBooking.StripeRefundTransactionId = string.Empty;
            PublicBooking.HasInvoice = true;
            PublicBooking.IsMaintainInvoices = true;
            _bookingRepository.Update(PublicBooking);
        }

        private StripeCharge CreateCharges(double amount, string customerId, string description)
        {
            var myCharge = new StripeChargeCreateOptions();

            // always set these properties
            myCharge.Amount = Convert.ToInt32(amount * 100); // cents
            myCharge.Currency = "USD";

            // set this if you want to
            myCharge.Description = description;

            // set this property if using a customer - this MUST be set if you are using an existing source!
            myCharge.CustomerId = customerId;

            // (not required) set this to false if you don't want to capture the charge yet - requires you call capture later
            myCharge.Capture = true;

            var chargeService = new StripeChargeService();
            StripeCharge stripeCharge = chargeService.Create(myCharge);

            return stripeCharge;
        }

        private StripeRefund CallStripeRefund(string stripeChargeId, double diffPrice)
        {
            var refundService = new StripeRefundService();

            StripeRefund refund = refundService.Create(stripeChargeId, new StripeRefundCreateOptions()
            {
                Amount = Convert.ToInt32(diffPrice * 100),
                Reason = StripeRefundReasons.RequestedByCustomer
            });
            return refund;
        }

        private double GetActualPrice(ActualPriceObject newPrice)
        {
            var discounts = (from dp in _bookingRepository.DiscountBookingList
                             join d in _bookingRepository.DiscountList on dp.DiscountId equals d.Id
                             where dp.ProductId == PublicProduct.ProductId && dp.BookingId == PublicBooking.BookingId
                             select d).FirstOrDefault();

            double actualPrice = newPrice.Price;
            if (discounts != null)
            {
                return Helper.CalculateDiscount(discounts, actualPrice, PublicBooking.Quantity);
            }
            return actualPrice;
        }
    }
}