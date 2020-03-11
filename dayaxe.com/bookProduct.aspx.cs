using System;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using AutoMapper;
using Dayaxe.SendEmail;
using DayaxeDal;
using DayaxeDal.Extensions;
using DayaxeDal.Parameters;
using DayaxeDal.Repositories;
using DayaxeDal.Result;
using Newtonsoft.Json;
using Stripe;
using Taxjar;

namespace dayaxe.com
{
    public partial class BookProduct : BasePage
    {
        protected bool ShowAuth { get; set; }
        protected Hotels PublicHotel { get; set; }
        protected Products PublicProduct { get; set; }
        protected Markets PublicMarkets { get; set; }
        protected string ProductTypeString { get; set; }
        protected string ProductTypeTrackString { get; set; }
        protected double DiscountPrice { get; set; }

        protected Discounts PublicDiscountUsed { get; set; }

        private string SearchUrl { get; set; }

        protected double NormalPrice { get; set; }

        protected string PublicTickets { get; set; }

        private int TotalTickets { get; set; }

        protected string ProductBlockoutDate { get; set; }

        private readonly BookingRepository _bookingRepository = new BookingRepository();
        private readonly ProductRepository _productRepository = new ProductRepository();
        private readonly CustomerInfoRepository _customerInfoRepository = new CustomerInfoRepository();
        private readonly DiscountRepository _discountRepository = new DiscountRepository();
        private readonly CustomerCreditRepository _customerCreditRepository = new CustomerCreditRepository();
        private readonly SubscriptionBookingRepository _subscriptionBookingRepository = new SubscriptionBookingRepository();

        private string _discountKey = string.Empty;
        private BookingsTemps _bookingsTemps;
        private DateTime _checkinDate = DateTime.UtcNow.ToLosAngerlesTime();
        private bool _selectDateNow;
        private int _maxTicket;
        private DateTime RestrictDate { get; set; }
        protected string RestrictDateStr { get; set; }
        private CustomerCredits PublicCustomerCredits { get; set; }
        private bool IsHaveBookingWithSelectedDate { get; set; }
        protected int TotalActiveBookingWithSubscription { get; set; }

        private int TotalBookingInCurrentRecycle { get; set; }
        protected DateTime ActiveDateSubscription { get; set; }

        private TaxjarApi _taxJarClient
        {
            get
            {
                return new TaxjarApi(ConfigurationManager.AppSettings["TaxjarApiKey"]);
            }
        }

        private double TaxChargeAmount = 0;

        private int ChargeTicket = 0;

        protected void Page_Init(object sender, EventArgs e)
        {
            ProductBlockoutDate = "[]";
            if (Page.RouteData.Values["hotelName"] != null && Page.RouteData.Values["productName"] != null)
            {
                PublicProduct = _productRepository.GetProductsByName((string)Page.RouteData.Values["hotelName"],
                    (string)Page.RouteData.Values["productName"],
                    (string)Session["UserSession"]);

                if (PublicProduct == null && Page.RouteData.Values["productId"] != null)
                {
                    int productId;
                    if (int.TryParse(Page.RouteData.Values["productId"].ToString(), out productId))
                    {
                        PublicProduct = _productRepository.GetById(productId);
                    }
                }
            }

            if (PublicProduct != null)
            {
                PublicHotel = _bookingRepository.HotelList.FirstOrDefault(h => h.HotelId == PublicProduct.HotelId);

                PublicMarkets = _productRepository.GetMarketsByHotelId(PublicProduct.HotelId);

                // Do not have permission to view this page
                if (PublicHotel != null &&
                    !PublicHotel.IsPublished &&
                    (PublicCustomerInfos == null || !PublicCustomerInfos.IsAdmin))
                {
                    Response.Redirect(Constant.SearchPageDefault);
                }

                _discountKey = string.Format("Discount_{0}_{1:ddMMyyyy}", PublicProduct.ProductId, _checkinDate);

                BindTicketCapacity();

                //NormalPrice = PublicProduct.LowestPrice;
                //DiscountPrice = PublicProduct.LowestUpgradeDiscount;
                goBack.HRef = string.Format("/{0}/{1}/{2}/{3}/{4}",
                    Page.RouteData.Values["market"] ?? "socal",
                    PublicHotel.CityUrl,
                    PublicHotel.HotelNameUrl,
                    PublicProduct.ProductNameUrl,
                    PublicProduct.ProductId);

                if (!IsPostBack)
                {
                    if (Session["TotalTickets"] != null)
                    {
                        HidTicket.Value = Session["TotalTickets"].ToString();
                    }
                    else
                    {
                        HidTicket.Value = PublicProduct.ProductType == (int)Enums.ProductType.DayPass ? "2" : "1";
                    }

                    var selectDate = Session["Selected_" + PublicProduct.ProductId];

                    if (!PublicProduct.IsCheckedInRequired && selectDate != null && selectDate.ToString() == "1")
                    {
                        selectLater.Attributes["class"] += " selected";
                        CheckInDateText.Attributes["disabled"] = "disabled";
                        CheckInDateText.Text = string.Empty;
                        HidSelectedDate.Value = "1";
                    }
                    else
                    {
                        HidSelectedDate.Value = "0";
                        selectNow.Attributes["class"] += " selected";

                        var selectedDateBefore = Session["CheckInDateSearch"];
                        if (selectedDateBefore != null)
                        {
                            CheckInDateText.Text = selectedDateBefore.ToString();
                        }
                    }

                    hotelname.Text = Helper.GetHotelName(PublicProduct);
                    Neighborhood.Text = string.Format("{0}, {1}", PublicHotel.Neighborhood, PublicHotel.City);

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

                    var multiView = (MultiView)AuthControl.FindControl("AuthMultiView");
                    if (Request.Params["sp"] != null &&
                        (Session["UserSession"] == null || (Session["UserSession"] != null && string.Equals(Request.Params["sp"], (string)Session["UserSession"], StringComparison.OrdinalIgnoreCase))) &&
                        multiView != null)
                    {
                        ShowAuth = true;
                        multiView.ActiveViewIndex = 3;
                    }

                    if (Request.Params["reg"] != null
                        && string.Equals(Request.Params["reg"], "true", StringComparison.OrdinalIgnoreCase)
                        && Session["UserSession"] == null &&
                        multiView != null)
                    {
                        multiView.ActiveViewIndex = 0;
                        ShowAuth = true;
                    }
                }

                var tickets = _productRepository.GetTicketsFuture(PublicProduct.ProductId);
                string json = JsonConvert.SerializeObject(tickets, CustomSettings.SerializerSettings());
                PublicTickets = json;
            }

            if (PublicCustomerInfos != null)
            {
                PublicCustomerCredits = _customerCreditRepository.GetById(PublicCustomerInfos.CustomerId);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (PublicProduct == null || PublicHotel == null)
            {
                if (PublicCustomerInfos != null && !string.IsNullOrEmpty(PublicCustomerInfos.BrowsePassUrl))
                {
                    Response.Redirect(PublicCustomerInfos.BrowsePassUrl, true);
                }
                Response.Redirect(!string.IsNullOrEmpty((string)Session["SearchPage"])
                    ? Session["SearchPage"].ToString()
                    : Constant.SearchPageDefault, true);
            }

            //var client = new TaxjarApi();
            //var rates = TaxJarClient.TaxForOrder(new
            //{
            //    from_country = "US",
            //    from_zip = "90403",
            //    from_state = "CA",
            //    to_country = "US",
            //    to_zip = "33304",
            //    to_state = "FL",
            //    amount = 50,
            //    shipping = 0
            //});
            //RateLabel.Text = Helper.FormatPriceWithFixed((double) rates.AmountToCollect);

            if (!IsPostBack)
            {
                Session.Remove("CurrentBookingUpgradeSubscription");
                Session.Remove("ShowGoldPassConfirmation");

                ShowCustomerInfos();
            }

            var selectedDateBefore = CheckInDateText.Text;
            if (!string.IsNullOrEmpty(selectedDateBefore))
            {
                if (DateTime.TryParseExact(selectedDateBefore, "MM/dd/yyyy", null,
                    DateTimeStyles.None, out _checkinDate))
                {
                    var customPrice = _productRepository.GetById(PublicProduct.ProductId, _checkinDate).ActualPriceWithDate;
                    NormalPrice = customPrice.Price;
                    DiscountPrice = customPrice.DiscountPrice;
                }
            }

            RestrictDate = DateTime.UtcNow.ToLosAngerlesTimeWithTimeZone(PublicHotel != null ? PublicHotel.TimeZoneId : string.Empty);

            if (_checkinDate.Date == RestrictDate.Date &&
                RestrictDate > DateTime.UtcNow.ToLosAngerlesTimeWithTimeZone(PublicHotel != null ? PublicHotel.TimeZoneId : string.Empty).Date.AddHours(19))
            {
                RestrictDate = RestrictDate.AddDays(1);
                _checkinDate = _checkinDate.AddDays(1);
            }
            RestrictDateStr = RestrictDate.ToString(Constant.DiscountDateFormat);

            ShowHideControl();

            BindProductInfos();

            AssignPassString();
        }

        protected void HtmlAnchor_Click(object sender, EventArgs e)
        {
            if ((Session["UserSession"] == null || PublicCustomerInfos == null || string.IsNullOrEmpty(PublicCustomerInfos.EmailAddress)) &&
                string.IsNullOrEmpty(email.Text.Trim()) && !Helper.IsValidEmail(email.Text.Trim()))
            {
                email.CssClass = "form-control errorborder";
                return;
            }

            if (string.IsNullOrEmpty(FirstName.Text.Trim()))
            {
                FirstName.CssClass = "form-control errorborder";
                return;
            }
            if (string.IsNullOrEmpty(LastName.Text.Trim()))
            {
                LastName.CssClass = "form-control errorborder";
                return;
            }

            // Select Date Now with SelectedDate.Value == "0"
            _selectDateNow = !PublicProduct.IsCheckedInRequired && HidSelectedDate.Value == "0";

            if ((PublicProduct.IsCheckedInRequired || _selectDateNow) && string.IsNullOrEmpty(CheckInDateText.Text))
            {
                CheckInDateText.CssClass = "form-control errorborder";
                return;
            }

            if (!string.IsNullOrEmpty(CheckInDateText.Text))
            {
                DateTime.TryParseExact(CheckInDateText.Text, Constant.DiscountDateFormat, null, DateTimeStyles.None, out _checkinDate);
            }

            try
            {
                // Check Available Product on that Check-In Date
                try
                {
                    if (PublicProduct.IsCheckedInRequired || _selectDateNow)
                    {
                        var currentDate = DateTime.UtcNow.ToLosAngerlesTimeWithTimeZone(PublicHotel.TimeZoneId);
                        var dateWithHour = DateTime.UtcNow.ToLosAngerlesTimeWithTimeZone(PublicHotel.TimeZoneId).Date
                            .AddHours(AppConfiguration.RestrictBookingSameDayAtHour.Hour)
                            .AddMinutes(AppConfiguration.RestrictBookingSameDayAtHour.Minute);
                        if (_checkinDate.Date == currentDate.Date &&
                            currentDate >= dateWithHour)
                        {
                            ErrorMessageLit.Text = string.Format(ErrorMessage.RestrictBookingAfterHour, AppConfiguration.RestrictBookingSameDayAtHour.ToString("h:mm tt"));
                            CheckInDateText.CssClass = "form-control errorborder";
                            return;
                        }

                        int ticketAvailable;
                        var param = new CheckAvailableProductParams
                        {
                            ProductId = PublicProduct.ProductId,
                            CheckInDate = _checkinDate,
                            TotalTicket = TotalTickets,
                            BookingId = 0,
                            IsAdmin = false,
                            TimezoneId = PublicHotel.TimeZoneId
                        };
                        var availableProduct = _productRepository.CheckAvailableProduct(param, out ticketAvailable);
                        if (!availableProduct)
                        {
                            soldOutModal.Visible = true;
                            if (ticketAvailable > 0)
                            {
                                ProductAvailableLit.Text = string.Format(Constant.TicketAvailable, ticketAvailable,
                                    _checkinDate);
                                ProductAvailableTitleLit.Text = "Select Less Tickets";
                            }
                            else
                            {
                                ProductAvailableLit.Text = string.Format(Constant.TicketNotAvailable, _checkinDate);
                                ProductAvailableTitleLit.Text = "Sold Out";
                            }
                            ScriptManager.RegisterClientScriptBlock(BookProductUpdatePanel, typeof(string), "openConfirmAvailableProductPopup", "window.confirmAvailable = 'true';", true);
                            return;
                        }
                    }
                }
                catch (Exception ex)
                {
                    var logs = new Logs
                    {
                        LogKey = "Booking-Error-On-Submit Available Product",
                        UpdatedDate = DateTime.UtcNow,
                        UpdatedContent = string.Format("Booking Error - ProductId: {0} - Check-In Date: {1} - Total Tickets: {2} - {3} - {4} - {5}",
                            PublicProduct.ProductId,
                            _checkinDate,
                            TotalTickets,
                            ex.Message,
                            ex.StackTrace,
                            ex.Source),
                        UpdatedBy = PublicCustomerInfos != null ? PublicCustomerInfos.CustomerId : 0
                    };
                    _productRepository.AddLog(logs);
                }

                // If Buy Add-Ons have buy core product first
                if (PublicProduct.ProductType == (int)Enums.ProductType.AddOns)
                {
                    if (PublicCustomerInfos == null)
                    {
                        PublicCustomerInfos = _productRepository.CustomerInfoList
                            .FirstOrDefault(ci => ci.EmailAddress == email.Text.Replace(" ", "").Trim());
                    }

                    var productHaveBooking = (from p in _productRepository.ProductList
                                              join ap in _productRepository.ProductAddOnList on p.ProductId equals ap.AddOnId
                                              where ap.AddOnId == PublicProduct.ProductId
                                              select ap.ProductId).FirstOrDefault();
                    var bookings =
                        _productRepository.BookingList.FirstOrDefault(
                            b => b.ProductId == productHaveBooking && b.CheckinDate.HasValue &&
                                 b.CheckinDate.Value.ToLosAngerlesTimeWithTimeZone(PublicHotel.TimeZoneId).Date == _checkinDate.Date &&
                                 b.CustomerId == (PublicCustomerInfos != null ? PublicCustomerInfos.CustomerId : 0));

                    if (bookings == null)
                    {
                        CheckInDateText.CssClass = "form-control errorborder";
                        throw new Exception(ErrorMessage.DoNotHaveAccessOnThisDate);
                    }
                }

                var result = PurchaseProduct();
                if (result.IsSuccess)
                {
                    Session["BookingSuccess"] = true;
                    Response.Redirect(string.Format(Constant.ConfirmProductPage,
                        Page.RouteData.Values["market"],
                        Page.RouteData.Values["city"],
                        Page.RouteData.Values["hotelName"],
                        Page.RouteData.Values["productName"],
                        result.BookingId), false);
                }
                else if (result.IsUpgrade) // Upgrade from Day Pass to Subscription
                {
                    upgradeModal.Visible = true;
                    //ExtraLit.Text = string.Format(Message.SubscriptionExtraCost, Helper.FormatPriceWithFixed(result.ExtraFund));
                    var confirmUrl = string.Format(Constant.ConfirmProductPage,
                        Page.RouteData.Values["market"],
                        Page.RouteData.Values["city"],
                        Page.RouteData.Values["hotelName"],
                        Page.RouteData.Values["productName"],
                        result.BookingId);
                    ScriptManager.RegisterClientScriptBlock(BookProductUpdatePanel,
                        typeof(string),
                        "openUpgradeSubscription",
                        string.Format("window.upgradeSubscription = 'true'; window.confirmPageUrl = '{0}';",
                        confirmUrl),
                        true);
                }
                else
                {
                    ScriptManager.RegisterClientScriptBlock(BookProductUpdatePanel,
                        typeof(string),
                        "openConfirmPopup",
                        "window.showConfirmM = 'true';",
                        true);
                }
            }
            catch (Exception ex)
            {
                ErrorMessageLit.Text = ex.Message;

                var logs = new Logs
                {
                    LogKey = "Booking-Error-On-Submit",
                    UpdatedDate = DateTime.UtcNow,
                    UpdatedContent = string.Format("Booking Error - {0} - {1} - {2}", ex.Message, ex.StackTrace, ex.Source),
                    UpdatedBy = PublicCustomerInfos != null ? PublicCustomerInfos.CustomerId : 0
                };
                _productRepository.AddLog(logs);
            }
        }

        private PurchaseTicketResult PurchaseProduct()
        {
            // Check customer change their email
            var result = new PurchaseTicketResult
            {
                IsSuccess = true
            };

            var bookingExists = PublicCustomerInfos != null ? _bookingRepository.GetBookingInLast3Minutes(PublicProduct.ProductId, PublicCustomerInfos.EmailAddress) : null;
            var isNewBooking = NewBookingHidden.Value == "true";
            if (bookingExists != null && !isNewBooking)
            {
                result.IsSuccess = false;
                newBookingModal.Visible = true;
                result.BookingId = bookingExists.BookingId;
                return result;
            }

            CreateStripeCustomer();

            bool isPayByCredit;
            Regex regexMatchMonthYear = new Regex(@"([\d]+)(\s)?/(\s)?([\d]+)");
            int month;
            int.TryParse(regexMatchMonthYear.Match(txtexpdat.Value).Groups[1].Value, out month);
            int year;
            int.TryParse("20" + regexMatchMonthYear.Match(txtexpdat.Value).Groups[4].Value, out year);
            string fullName = string.Format("{0} {1}", FirstName.Text, LastName.Text);

            // Check customer has exists with email
            int customerId = PublicCustomerInfos.CustomerId;
            StripeCustomer stripeCustomer = null;
            var discounts = new Discounts();
            bool upgradeAvailable = false;
            var actualPrice = NormalPrice;
            double totalPrice;

            // update user info with exists
            if (!string.IsNullOrEmpty(PublicCustomerInfos.StripeCustomerId))
            {
                stripeCustomer = GetCustomerById(PublicCustomerInfos.StripeCustomerId);
            }

            // Use New Card
            if (MVCardInfo.ActiveViewIndex == 0)
            {
                StripeToken stripeToken = CreateToken(cctextbox.Text.Replace(" ", ""), year, month, txtzipcode.Text,
                    fullName,
                    txtseccode.Value);

                // update new card for customer
                PublicCustomerInfos.StripeTokenId = stripeToken.Id;
                PublicCustomerInfos.StripeCardId = stripeToken.StripeCard.Id;
                PublicCustomerInfos.BankAccountLast4 = stripeToken.StripeCard.Last4;
                PublicCustomerInfos.CardType = Helper.GetCreditCardType(cctextbox.Text.Replace(" ", ""));
                PublicCustomerInfos.ZipCode = txtzipcode.Text;

                if (stripeCustomer == null)
                {
                    stripeCustomer = CreateCustomer(PublicCustomerInfos.EmailAddress, fullName, stripeToken.Id,
                        string.Empty, string.Empty);
                    PublicCustomerInfos.StripeCustomerId = stripeCustomer.Id;
                }
                else
                {
                    var card = CreateCard(stripeToken.Id, PublicCustomerInfos.StripeCustomerId);
                    stripeCustomer = UpdateCustomer(PublicCustomerInfos.EmailAddress, fullName,
                        PublicCustomerInfos.StripeCustomerId, card.Id);
                }
                _customerInfoRepository.Update(PublicCustomerInfos);

                isPayByCredit = IsPayByCreditCheckBox.Checked;
            }
            else
            {
                isPayByCredit = DCreditCheckBox.Checked;
            }

            if (Session[Constant.UpgradeKey] == null)
            {
                // Check have Upgrade available
                upgradeAvailable = _bookingRepository.IsAvailableUpgrade(PublicProduct.ProductId);

                if (string.IsNullOrWhiteSpace(PromoText.Text))
                {
                    Session.Remove(_discountKey);
                }

                // Not available upgrade so checkout with this hotel
                if (Session[_discountKey] != null)
                {
                    discounts = JsonConvert.DeserializeObject<Discounts>(Session[_discountKey].ToString());
                }

                // Have Discount
                if (discounts != null)
                {
                    discounts = _discountRepository.VerifyDiscounts(discounts, customerId);

                    // Discount Invalid
                    if (discounts != null)
                    {
                        actualPrice = Helper.CalculateDiscount(discounts, actualPrice, TotalTickets);
                    }
                    else
                    {
                        InitDefaultPromo(Message.ExceedLimit, false);
                        Session[_discountKey] = null;
                        result.IsSuccess = false;
                        result.BookingId = 0;
                        return result;
                    }
                }
            }
            else
            {
                actualPrice = NormalPrice - DiscountPrice - _bookingsTemps.HotelPrice;
                if (actualPrice <= 0)
                {
                    actualPrice = 0;
                }
            }

            if (PublicCustomerInfos.FirstName != FirstName.Text.Trim() || PublicCustomerInfos.LastName != LastName.Text.Trim())
            {
                PublicCustomerInfos.FirstName = FirstName.Text.Trim();
                PublicCustomerInfos.LastName = LastName.Text.Trim();
                _customerInfoRepository.Update(PublicCustomerInfos);

                // Update Stripe exists customer
                stripeCustomer = UpdateCustomer(PublicCustomerInfos.EmailAddress, fullName, PublicCustomerInfos.StripeCustomerId, string.Empty);
            }

            double chargePrice = totalPrice = actualPrice * ChargeTicket + TaxChargeAmount;
            string creditLogDescription = string.Empty;

            // Use DayAxe Credit
            if (isPayByCredit && PublicCustomerCredits != null && PublicCustomerCredits.Amount > 0)
            {
                chargePrice = totalPrice - PublicCustomerCredits.Amount;
                if (chargePrice <= 0)
                {
                    chargePrice = 0;
                }
                var markets = _productRepository.MarketList.FirstOrDefault(
                        m => m.Permalink == (string)Page.RouteData.Values["market"] && m.IsActive);
                creditLogDescription = string.Format("{0} – {1} – {2} – ",
                    PublicProduct.ProductName,
                    PublicHotel.HotelName,
                    markets != null ? markets.LocationName : "");
            }

            string descriptionCharge = string.Format("{0} - {1} - {2} - {3}",
                                                      ProductTypeString,
                                                      PublicProduct.ProductName,
                                                      PublicHotel.HotelName,
                                                      DateTime.UtcNow.ToLosAngerlesTimeWithTimeZone(PublicHotel.TimeZoneId).ToString(Constant.DateFormat));

            bool isUpgradeToSubscription = false;
            Subscriptions upgradeSubscription;

            // Check Upgrade from Day Pass to Subscription
            if (PublicProduct.ProductType == (int)Enums.ProductType.DayPass)
            {
                upgradeSubscription = _bookingRepository.SubscriptionsList.FirstOrDefault(s => s.Id == AppConfiguration.SubscriptionUpgradeId && s.IsActive);
                if (upgradeSubscription != null)
                {
                    // Avoid case if customer not log in before purchase tickets
                    if (PublicDiscounts == null)
                    {
                        PublicDiscounts = _customerInfoRepository.GetSubscriptionDiscount(PublicCustomerInfos.CustomerId);
                    }

                    var paramDiscount = new GetDiscountValidByCodeParams
                    {
                        Code = AppConfiguration.PromoCodeUpgrade,
                        ProductId = 0,
                        CustomerId = PublicCustomerInfos != null ? PublicCustomerInfos.CustomerId : 0,
                        IsAdmin = false,
                        BookingId = 0,
                        SubscriptionId = upgradeSubscription.Id,
                        TimezoneId = PublicHotel.TimeZoneId
                    };

                    if (PublicDiscounts == null)
                    {
                        bool isLimit = false;
                        double subscriptionActualPrice = upgradeSubscription.Price;
                        var discountSubscription = _discountRepository.GetDiscountValidByCode(paramDiscount, out isLimit);


                        // Have Discount
                        if (discountSubscription != null && discountSubscription.Id > 0)
                        {
                            discountSubscription = _discountRepository.VerifyDiscountsSubscription(discountSubscription, PublicCustomerInfos.CustomerId, upgradeSubscription.Id);

                            // Discount Invalid
                            if (discountSubscription != null)
                            {
                                subscriptionActualPrice = Helper.CalculateDiscount(discountSubscription, subscriptionActualPrice, 1);
                            }
                        }
                        SubscriptionPriceLit.Text = string.Format("{0}/month", Helper.FormatPrice(upgradeSubscription.Price));
                        SubscriptionPrice2Lit.Text = string.Format("{0}/month", Helper.FormatPrice(upgradeSubscription.Price));
                        ChargePriceLit.Text = Helper.FormatPrice(subscriptionActualPrice);

                        //double extraFund = upgradeSubscription.Price - chargePrice / TotalTickets;
                        //var discountSubscription = _discountRepository.GetDiscountUsedWithSubscription(
                        //    PromoText.Text.Trim(), upgradeSubscription != null ? upgradeSubscription.Id : 0,
                        //    PublicHotel.TimeZoneId);

                        //if (discountSubscription != null)
                        //{
                        //    extraFund = discountSubscription.PromoType == (int) Enums.PromoType.Fixed
                        //        ? upgradeSubscription.Price - discountSubscription.PercentOff
                        //        : upgradeSubscription.Price - upgradeSubscription.Price * discountSubscription.PercentOff / 100;
                        //    extraFund = extraFund - chargePrice / TotalTickets;
                        //}

                        if (_subscriptionBookingRepository.CanApplySubscriptionPromo(PublicCustomerInfos.CustomerId))
                        {
                            result.IsSuccess = false;
                            result.IsUpgrade = true;
                            isUpgradeToSubscription = true;
                        }
                        else
                        {
                            result.IsSuccess = true;
                        }
                        //result.ExtraFund = extraFund;
                    }
                }
            }

            var bookingCode = Helper.RandomString(Constant.BookingCodeLength);
            while (_bookingRepository.IsBookingCodeExists(bookingCode))
            {
                bookingCode = Helper.RandomString(Constant.BookingCodeLength);
            }

            //PassStatus = isUpgradeToSubscription ? (int)Enums.BookingStatus.Capture : (int)Enums.BookingStatus.Active,
            var booking = new Bookings
            {
                CustomerId = customerId,
                StripeChargeId = null,//stripeChargeId,
                ProductId = PublicProduct.ProductId,
                MerchantPrice = NormalPrice,
                HotelPrice = actualPrice,
                BookedDate = DateTime.UtcNow,
                PassStatus = (int)Enums.BookingStatus.Active,
                TransactionId = "",
                TotalPrice = totalPrice,
                Quantity = TotalTickets,
                BookingCode = bookingCode,
                CheckinDate = null,
                IsUpgrade = Session[Constant.UpgradeKey] != null,
                BrowsePassUrl = GetProductUrl().Replace(EmailConfig.DefaultImageUrlSendEmail, ""),
                PayByCredit = totalPrice.Equals(chargePrice) ? 0 : totalPrice - chargePrice,
                PaymentType = chargePrice > 0 ? (byte)Enums.PaymentType.Stripe : (byte)Enums.PaymentType.DayAxeCredit,
                TaxPrice = TaxChargeAmount,
            };

            if (CheckIsActiveSubscription())
            {
                booking.IsActiveSubscription = true;
            }

            if (PublicProduct.IsCheckedInRequired || _selectDateNow)
            {
                booking.CheckinDate = _checkinDate.AddHours(Math.Abs(PublicHotel.TimeZoneOffset));
                booking.ExpiredDate = _checkinDate.AddDays(1).AddHours(Math.Abs(PublicHotel.TimeZoneOffset));
            }
            else
            {
                // Can be add blackout days on near 3 days
                var dateNow = DateTime.UtcNow.ToLosAngerlesTimeWithTimeZone(PublicHotel.TimeZoneId).Date;
                var blockOutDateContains = PublicProduct.GetNearBlockedDates.Count(b => b.Date.Date >= dateNow.Date);
                int redemptionPeriod = PublicProduct.RedemptionPeriod >= 0 ? PublicProduct.RedemptionPeriod : Constant.DefaultRedemptionPeriod;

                DateTime expiredDate = DateTime.UtcNow.ToLosAngerlesTimeWithTimeZone(PublicHotel.TimeZoneId)
                    .AddDays(redemptionPeriod)
                    .AddDays(blockOutDateContains)
                    .ToLosAngerlesTimeWithTimeZone(PublicHotel.TimeZoneId);
                if (PublicProduct.IsOnBlackOutDay)
                {
                    var nextAvailableDay =
                        DateTime.ParseExact(
                            string.Format("{0} {1:yyyy}",
                            PublicProduct.NextAvailableDay.Replace("Available ", ""),
                            DateTime.UtcNow.ToLosAngerlesTimeWithTimeZone(PublicHotel.TimeZoneId)), "MMM dd yyyy", null);

                    expiredDate = nextAvailableDay
                        .AddDays(redemptionPeriod)
                        .AddDays(blockOutDateContains)
                        .ToLosAngerlesTimeWithTimeZone(PublicHotel.TimeZoneId);
                }
                booking.ExpiredDate = expiredDate.Date.AddHours(Math.Abs(PublicHotel.TimeZoneOffset));
            }

            int discountId = discounts != null ? discounts.Id : 0;
            // If Upgrade Available Add Data to BookingsTemp/DiscountBookingsTemp
            if (upgradeAvailable)
            {
                var bookingsTemp = Mapper.Map<Bookings, BookingsTemps>(booking);
                bookingsTemp.CustomerId = PublicCustomerInfos.CustomerId;
                bookingsTemp.Status = (int)Enums.BookingsTempStatus.Upgrade;
                bookingsTemp.IsUpgrade = true;

                int bookingTempId = _bookingRepository.AddBookingsTemp(bookingsTemp, discountId);

                Session[Constant.UpgradeKey] = bookingTempId;
                Response.Redirect(string.Format(Constant.UpgradePage, Page.RouteData.Values["market"],
                        Page.RouteData.Values["city"],
                        Page.RouteData.Values["hotelName"],
                        Page.RouteData.Values["productName"],
                        bookingTempId), true);
            }

            creditLogDescription += booking.BookingIdString;

            var addParam = new AddBookingParams
            {
                BookingObject = booking,
                DiscountId = discountId,
                CustomerCreditObject = PublicCustomerCredits,
                Description = creditLogDescription,
                IsUpgradeToSubscription = isUpgradeToSubscription
            };

            if (CheckIsActiveSubscription())
            {
                addParam.SubscriptionDiscountId = PublicDiscounts != null ? PublicDiscounts.Id : 0;
            }

            int bookingId = _bookingRepository.Add(addParam, false);

            string stripeChargeId = string.Empty;
            try
            {
                result.IsSuccess = false;

                if (chargePrice > 0)
                {
                    StripeCharge stripeCharge = CreateCharges(chargePrice, stripeCustomer.Id, descriptionCharge, true);
                    stripeChargeId = stripeCharge.Id;
                }

                // Charge With Credit
                if (chargePrice > 0 && Session[Constant.UpgradeKey] == null && !upgradeAvailable && !isUpgradeToSubscription)
                {
                    StripeCharge stripeCharge = CreateCharges(chargePrice, stripeCustomer.Id, descriptionCharge, true);
                    stripeChargeId = stripeCharge.Id;
                }

                booking.StripeChargeId = stripeChargeId;

                _bookingRepository.UpdateStripeChargeId(booking, true);

                result.IsSuccess = true;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.IsUpgrade = false;
                result.BookingId = bookingId = 0;

                var log = new Logs
                {
                    LogKey = "Error-StripeCharge-Booking",
                    UpdatedContent = string.Format("{0} - {1} - {2}", ex.Message, ex.InnerException, ex.StackTrace),
                    UpdatedDate = DateTime.UtcNow,
                    UpdatedBy = PublicCustomerInfos != null ? PublicCustomerInfos.CustomerId : 0
                };
                _productRepository.AddLog(log);

                throw new Exception("An error occurred while making a payment. Please contact our customer service for more details at <a href='mailto:help@dayaxe.com'>help@dayaxe.com</a>");
            }

            Session.Remove("CheckInDateSearch");
            Session.Remove("TotalTickets");
            Session.Remove(Constant.UpgradeKey);
            Session.Remove(_discountKey);

            CacheLayer.Clear(CacheKeys.BookingsCacheKey);
            CacheLayer.Clear(CacheKeys.CustomerInfosCacheKey);
            CacheLayer.Clear(CacheKeys.CustomerCreditsCacheKey);
            CacheLayer.Clear(CacheKeys.CustomerCreditLogsCacheKey);
            CacheLayer.Clear(CacheKeys.DiscountsCacheKey);
            CacheLayer.Clear(CacheKeys.DiscountBookingsCacheKey);
            CacheLayer.Clear(CacheKeys.SurveysCacheKey);

            if (isUpgradeToSubscription)
            {
                Session["CurrentBookingUpgradeSubscription"] = bookingId;
            }

            result.BookingId = bookingId;
            return result;
        }

        private void CreateStripeCustomer()
        {
            if (PublicCustomerInfos == null)
            {
                PublicCustomerInfos = new CustomerInfos
                {
                    EmailAddress = email.Text.Replace(" ", ""),
                    FirstName = FirstName.Text.Trim(),
                    LastName = LastName.Text.Trim(),
                    ZipCode = txtzipcode.Text.Trim(),
                    BrowsePassUrl = SearchUrl
                };

                // Create new account with email
                var response = _customerInfoRepository.GetVipAccess(email.Text.Replace(" ", ""), SearchUrl, FirstName.Text.Trim(), LastName.Text.Trim());
                PublicCustomerInfos.CustomerId = response.CustomerId;
                PublicCustomerInfos.StripeCustomerId = response.StripeCustomerId;

                CacheLayer.Clear(CacheKeys.CustomerInfosCacheKey);
                CacheLayer.Clear(CacheKeys.CustomerCreditsCacheKey);

                Session["ReferralCode"] = response.ReferralCode;

                string searchPage = !string.IsNullOrEmpty((string)Session["SearchPage"])
                    ? Session["SearchPage"].ToString()
                    : Constant.SearchPageDefault;

                // Send email new account
                var responseEmail = EmailHelper.EmailNewAccount(PublicCustomerInfos.EmailAddress,
                    PublicCustomerInfos.FirstName,
                    response.Password,
                    Helper.ResolveRelativeToAbsoluteUrl(Request.Url,
                        string.Format("{0}?sp={1}",
                            searchPage,
                            response.PasswordKey)), // Reset Password Url
                    Helper.ResolveRelativeToAbsoluteUrl(Request.Url,
                        string.Format("{0}?c={1}",
                            searchPage,
                            response.AccessKey))); // Browse Day Pass

                PublicCustomerCredits = _bookingRepository.GetCustomerCredits(response.CustomerId);

                Session["UserSession"] = response.AccessKey;
                Session["IsRegister"] = true;
                Session["ReferralCode"] = response.ReferralCode;
            }
        }

        private StripeCustomer CreateCustomer(string emailStr, string name, string tokenId, string planId, string couponId)
        {
            var myCustomer = new StripeCustomerCreateOptions
            {
                Email = emailStr,
                Description = string.Format("{0} ({1})", name, emailStr),
                SourceToken = tokenId
            };

            //myCustomer.PlanId = planId;                          // only if you have a plan
            //myCustomer.TaxPercent = 20;                            // only if you are passing a plan, this tax percent will be added to the price.
            //myCustomer.CouponId = couponId;                        // only if you have a coupon
            //myCustomer.TrialEnd = productPublic.ExpiredDate;    // when the customers trial ends (overrides the plan if applicable)
            //myCustomer.Quantity = 1;                               // optional, defaults to 1

            var customerService = new StripeCustomerService();
            StripeCustomer stripeCustomer = customerService.Create(myCustomer);

            return stripeCustomer;
        }

        private StripeCustomer GetCustomerById(string customerId)
        {
            var customerService = new StripeCustomerService();
            StripeCustomer stripeCustomer = customerService.Get(customerId);

            return stripeCustomer;
        }

        private StripeCustomer UpdateCustomer(string emailStr, string fullName, string customerId, string sourceToken)
        {
            var myCustomer = new StripeCustomerUpdateOptions();
            myCustomer.Email = emailStr;
            myCustomer.Description = string.Format("{0} ({1})", fullName, emailStr);
            if (!string.IsNullOrEmpty(sourceToken))
            {
                myCustomer.DefaultSource = sourceToken;
            }

            var customerService = new StripeCustomerService();
            StripeCustomer stripeCustomer = customerService.Update(customerId, myCustomer);
            return stripeCustomer;
        }

        #region Stripe Function Use

        private StripeToken CreateToken(string number, int expireYear, int expireMonth, string zipCode, string name, string cvc)
        {
            var myToken = new StripeTokenCreateOptions();

            // if you need this...
            myToken.Card = new StripeCreditCardOptions()
            {
                // set these properties if passing full card details (do not
                // set these properties if you set TokenId)
                Number = number,
                ExpirationYear = expireYear,
                ExpirationMonth = expireMonth,
                //AddressCountry = "US",                // optional
                //AddressLine1 = "24 Beef Flank St",    // optional
                //AddressLine2 = "Apt 24",              // optional
                //AddressCity = "Biggie Smalls",        // optional
                //AddressState = state,                  // optional
                AddressZip = zipCode,                 // optional
                Name = name,               // optional
                Cvc = cvc                          // optional
            };

            // set this property if using a customer (stripe connect only)
            //if (!string.IsNullOrEmpty(customerId))
            //{
            //    myToken.CustomerId = customerId;
            //}

            var tokenService = new StripeTokenService();
            StripeToken stripeToken = tokenService.Create(myToken);

            return stripeToken;
        }

        private StripeCard CreateCard(string tokenId, string customerId)
        {
            var myCard = new StripeCardCreateOptions
            {
                SourceToken = tokenId
            };

            var cardService = new StripeCardService();
            StripeCard stripeCard = cardService.Create(customerId, myCard); // optional isRecipient
            return stripeCard;
        }

        private StripeCharge CreateCharges(double amount, string customerId, string description, bool capture = true)
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
            myCharge.Capture = capture;

            var chargeService = new StripeChargeService();
            StripeCharge stripeCharge = chargeService.Create(myCharge);

            return stripeCharge;
        }

        private StripeCard GetCardById(string customerId, string cardId)
        {
            var cardService = new StripeCardService();
            StripeCard stripeCard = cardService.Get(customerId, cardId);
            return stripeCard;
        }

        #endregion

        protected void UseNewCardClick(object sender, EventArgs e)
        {
            AnchorButton.Attributes["class"] = "AnchorButton btngray";
            MVCardInfo.ActiveViewIndex = 0;
        }

        protected void AddPromoClick(object sender, EventArgs e)
        {
            AddPromo();
        }

        protected void ApplyPromoClick(object sender, EventArgs e)
        {
            rowMessage.Visible = false;
            MessageLabel.Text = string.Empty;
            discountInfoRow.Visible = false;
            promoRow.Visible = false;
            perMoneyPrice.Attributes["class"] = "total-price";
            bool isLimit;
            if (string.IsNullOrEmpty(PromoText.Text))
            {
                InitDefaultPromo(Message.InvalidOrExpiredPromo, false);
                return;
            }

            var giftCard = _customerCreditRepository.GetGiftCardByCode(PromoText.Text.Trim());
            bool isGiftCard = giftCard != null;
            bool isReferral = false;

            if (giftCard != null)
            {
                if (PublicCustomerCredits == null)
                {
                    InitDefaultPromo(ErrorMessage.YouMustLogIn, false);
                    return;
                }

                if (giftCard.Status == (short)Enums.GiftCardType.Used)
                {
                    InitDefaultPromo(ErrorMessage.GiftCardHasBeenUsed, false);
                    return;
                }

                if (!string.IsNullOrEmpty(giftCard.EmailAddress) &&
                    !PublicCustomerInfos.EmailAddress.Equals(giftCard.EmailAddress, StringComparison.OrdinalIgnoreCase))
                {
                    InitDefaultPromo(ErrorMessage.PleaseCheckYourGiftCardWithCurrentAccount, false);
                    return;
                }

                _customerCreditRepository.AddGiftCard(PublicCustomerCredits, PromoText.Text.Trim());
                PublicCustomerCredits = _customerCreditRepository.Refresh(PublicCustomerCredits);
                CacheLayer.Clear(CacheKeys.GiftCardCacheKey);
                CacheLayer.Clear(CacheKeys.CustomerCreditsCacheKey);
                CacheLayer.Clear(CacheKeys.CustomerCreditLogsCacheKey);
                BindProductInfos();
                InitDefaultPromo(ErrorMessage.GiftCardHasBeenApplied);
                BookProductUpdatePanel.Update();
            }

            var referralCustomer = _customerCreditRepository.GetByReferCode(PromoText.Text.Trim());
            if (referralCustomer != null)
            {
                if (PublicCustomerCredits == null)
                {
                    InitDefaultPromo(ErrorMessage.YouMustLogIn, false);
                    return;
                }

                if (PublicCustomerCredits.ReferralCustomerId != 0)
                {
                    InitDefaultPromo(ErrorMessage.CanNotReferralTwice, false);
                    return;
                }

                if (_customerCreditRepository.BookingList.Count(b => b.CustomerId == referralCustomer.CustomerId) == 0)
                {
                    InitDefaultPromo(ErrorMessage.DoNotHavePurchase, false);
                    return;
                }

                if (_customerCreditRepository.BookingList.Count(b => b.CustomerId == PublicCustomerCredits.CustomerId) >
                    0)
                {
                    InitDefaultPromo(ErrorMessage.YouAlreadyHaveBookings, false);
                    return;
                }

                var total = _customerCreditRepository.AddReferral(PublicCustomerCredits, referralCustomer);
                PublicCustomerCredits = _customerCreditRepository.Refresh(PublicCustomerCredits);
                isReferral = true;
                CacheLayer.Clear(CacheKeys.GiftCardCacheKey);
                CacheLayer.Clear(CacheKeys.CustomerCreditsCacheKey);
                CacheLayer.Clear(CacheKeys.CustomerCreditLogsCacheKey);
                BindProductInfos();
                InitDefaultPromo(ErrorMessage.CreditHasBeenApplied);
                BookProductUpdatePanel.Update();
            }

            if (!isGiftCard && !isReferral)
            {
                var param = new GetDiscountValidByCodeParams
                {
                    Code = PromoText.Text.Trim(),
                    ProductId = PublicProduct.ProductId,
                    CustomerId = PublicCustomerInfos != null ? PublicCustomerInfos.CustomerId : 0,
                    IsAdmin = false,
                    BookingId = 0,
                    SubscriptionId = PublicSubscriptionId,
                    TimezoneId = PublicHotel.TimeZoneId
                };

                Discounts discounts = _discountRepository.GetDiscountValidByCode(param, out isLimit);
                if (discounts == null)
                {
                    InitDefaultPromo(isLimit ? Message.ExceedLimit : Message.InvalidOrExpiredPromo, false);
                    return;
                }

                if (discounts.MinAmount > NormalPrice * TotalTickets)
                {
                    InitDefaultPromo(ErrorMessage.MinAmountNotMatch, false);
                    return;
                }

                double actualPrice = NormalPrice;
                actualPrice = Helper.CalculateDiscount(discounts, actualPrice, TotalTickets);
                if (!actualPrice.Equals(NormalPrice))
                {
                    msrp.Visible = true;
                    perMoneyPrice.Attributes["class"] += " has-discount";
                }

                if (CheckIsActiveSubscription())
                {
                    moneyPrice.Text = Helper.FormatPriceWithFixed(actualPrice * (TotalTickets - 1));
                }
                else
                {
                    moneyPrice.Text = Helper.FormatPriceWithFixed(actualPrice * TotalTickets);
                }

                perMoneyPrice.Text = Helper.FormatPriceWithFixed(actualPrice);

                string json = JsonConvert.SerializeObject(discounts, CustomSettings.SerializerSettings());
                Session[_discountKey] = json;
                BindProductInfos();
            }
        }

        private void InitDefaultPromo(string message, bool isSuccess = true)
        {
            rowMessage.Visible = true;
            MessageLabel.Text = message;
            MessageLabel.Visible = true;
            Session[_discountKey] = JsonConvert.SerializeObject(null, CustomSettings.SerializerSettings());
            msrp.Visible = false;

            BindPriceInfo();

            if (CheckIsActiveSubscription())
            {
                moneyPrice.Text = Helper.FormatPriceWithFixed(NormalPrice * (TotalTickets - 1));
            }
            else
            {
                moneyPrice.Text = Helper.FormatPriceWithFixed(NormalPrice * TotalTickets);
            }
            perMoneyPrice.Text = Helper.FormatPriceWithFixed(NormalPrice);
            MessageLabel.CssClass = isSuccess ? "success-message" : "error-message";
        }

        private void ShowHideControl()
        {
            if (PublicProduct.IsCheckedInRequired)
            {
                // Replace current text with CheckIn Required
                DateLabelLabel.Text = "Check-In Date:";
                DateLabelLabel.CssClass = "date-label date-label-3";
            }
            else
            {
                wrapCheckInDate.Visible = true;
            }

            var isAllowCheckout = IsAllowCheckout.Value;
            if (String.Equals(isAllowCheckout, "true", StringComparison.OrdinalIgnoreCase))
            {
                AnchorButton.CssClass = "AnchorButton";
            }

            if (Session[Constant.UpgradeKey] != null)
            {
                AnchorButton.Text = "UPGRADE NOW";
            }

            if (PublicDiscounts != null && PublicProduct.ProductType == (int)Enums.ProductType.DayPass)
            {
                var bookings = (from b in _productRepository.BookingList
                                join db in _productRepository.DiscountBookingList on b.BookingId equals db.BookingId
                                join p in _productRepository.ProductList on b.ProductId equals p.ProductId
                                join h in _productRepository.HotelList on p.HotelId equals h.HotelId
                                where b.CustomerId == PublicCustomerInfos.CustomerId &&
                                      b.CheckinDate.HasValue &&
                                      b.CheckinDate.Value.ToLosAngerlesTimeWithTimeZone(h.TimeZoneId).Date == _checkinDate.Date &&
                                      b.PassStatus != (int)Enums.BookingStatus.Refunded &&
                                      db.DiscountId == PublicDiscounts.Id
                                select b);

                IsHaveBookingWithSelectedDate = bookings.Any();

                // Active Booking with Subscription
                var bookingActive = (from b in _productRepository.BookingList
                                     join db in _productRepository.DiscountBookingList on b.BookingId equals db.BookingId
                                     join p in _productRepository.ProductList on b.ProductId equals p.ProductId
                                     join h in _productRepository.HotelList on p.HotelId equals h.HotelId
                                     where b.CustomerId == PublicCustomerInfos.CustomerId &&
                                           b.PassStatus == (int)Enums.BookingStatus.Active &&
                                           db.DiscountId == PublicDiscounts.Id
                                     select b).ToList();

                TotalActiveBookingWithSubscription = bookingActive.Count;
                if (bookingActive.Any())
                {
                    var firstBooking = bookingActive.First();
                    ActiveDateSubscription = firstBooking.CheckinDate.HasValue
                        ? firstBooking.CheckinDate.Value.ToLosAngerlesTimeWithTimeZone(PublicHotel.TimeZoneId)
                        : _checkinDate;
                }

                var totalBooking = (from b in _productRepository.BookingList
                                    join db in _productRepository.DiscountBookingList on b.BookingId equals db.BookingId
                                    join p in _productRepository.ProductList on b.ProductId equals p.ProductId
                                    join h in _productRepository.HotelList on p.HotelId equals h.HotelId
                                    where b.CustomerId == PublicCustomerInfos.CustomerId &&
                                          b.PassStatus != (int)Enums.BookingStatus.Refunded &&
                                          db.DiscountId == PublicDiscounts.Id &&
                                          b.CheckinDate.HasValue &&
                                          PublicDiscounts.EndDate.HasValue &&
                                          PublicDiscounts.StartDate.HasValue &&
                                          b.CheckinDate.Value.ToLosAngerlesTimeWithTimeZone(h.TimeZoneId) >= PublicDiscounts.EndDate.Value.ToLosAngerlesTimeWithTimeZone(h.TimeZoneId).AddMonths(-1) &&
                                          b.CheckinDate.Value.ToLosAngerlesTimeWithTimeZone(h.TimeZoneId) <= PublicDiscounts.EndDate.Value.ToLosAngerlesTimeWithTimeZone(h.TimeZoneId)
                                    select b);
                TotalBookingInCurrentRecycle = totalBooking.Count();
            }
        }

        private void AssignPassString()
        {
            switch (PublicProduct.ProductType)
            {
                case (int)Enums.ProductType.Cabana:
                    SearchUrl = string.Format(Constant.CabanaPassPage, Page.RouteData.Values["market"]);
                    ProductTypeString = "CABANA";
                    break;
                case (int)Enums.ProductType.DayPass:
                    SearchUrl = string.Format(Constant.DayPassPage, Page.RouteData.Values["market"]);
                    ProductTypeString = "DAY PASS";
                    break;
                case (int)Enums.ProductType.Daybed:
                    SearchUrl = string.Format(Constant.DaybedsPassPage, Page.RouteData.Values["market"]);
                    ProductTypeString = "DAYBED";
                    break;
                case (int)Enums.ProductType.SpaPass:
                    SearchUrl = string.Format(Constant.SpaPassPage, Page.RouteData.Values["market"]);
                    ProductTypeString = "SPA PASS";
                    break;
                case (int)Enums.ProductType.AddOns:
                    ProductTypeString = "ADD ONS";
                    break;
            }

            ProductTypeTrackString = Helper.GetStringPassByProductType(PublicProduct.ProductType);
        }

        private void AddPromo()
        {
            PromoText.Visible = true;
            AddPromoButton.Visible = true;
        }

        private void ShowCustomerInfos()
        {
            if (PublicCustomerInfos != null)
            {
                EmailPlaceHolder.Visible = false;
                HasAccount.Visible = false;

                FirstName.Text = PublicCustomerInfos.FirstName;
                LastName.Text = PublicCustomerInfos.LastName;

                // Get Card Info from Stripe
                if (!string.IsNullOrEmpty(PublicCustomerInfos.StripeCustomerId) &&
                    !string.IsNullOrEmpty(PublicCustomerInfos.StripeCardId))
                {
                    try
                    {
                        var stripeCard = GetCardById(PublicCustomerInfos.StripeCustomerId,
                            PublicCustomerInfos.StripeCardId);
                        if (stripeCard != null && !string.IsNullOrEmpty(PublicCustomerInfos.CardType) &&
                            PublicCustomerInfos.CardType != "invalid")
                        {
                            CardInfoLit.Text =
                                string.Format(
                                    "<span class='card-type'>{0}</span>" + "Ending in ***{1} (Expires {2}/{3})",
                                    PublicCustomerInfos.CardType,
                                    stripeCard.Last4,
                                    stripeCard.ExpirationMonth,
                                    stripeCard.ExpirationYear % 100);
                            MVCardInfo.ActiveViewIndex = 1;
                            AnchorButton.Attributes["class"] = "AnchorButton";

                            CreditCardRow.Visible = true;
                            CardTypeLiteral.Text = stripeCard.Brand;
                            CardInfoLiteral.Text = string.Format("Ending in ***{0} (Expires {1}/{2})",
                                stripeCard.Last4,
                                stripeCard.ExpirationMonth,
                                stripeCard.ExpirationYear % 100);
                        }
                    }
                    catch (Exception ex)
                    {
                        string json =
                            JsonConvert.SerializeObject(PublicCustomerInfos, CustomSettings.SerializerSettings());
                        PublicCustomerInfos.StripeCustomerId = string.Empty;
                        PublicCustomerInfos.StripeCardId = string.Empty;
                        PublicCustomerInfos.StripeTokenId = string.Empty;
                        PublicCustomerInfos.BankAccountLast4 = string.Empty;
                        _customerInfoRepository.Update(PublicCustomerInfos);
                        var log = new Logs
                        {
                            LogKey = "book-error",
                            UpdatedContent = string.Format("{0} - {1} - {2}", ex.Message, json, ex.StackTrace),
                            UpdatedDate = DateTime.UtcNow,
                            UpdatedBy = PublicCustomerInfos != null ? PublicCustomerInfos.CustomerId : 0
                        };
                        _productRepository.AddLog(log);
                    }
                }
            }
            else
            {
                if (Session["UserSession"] != null)
                {
                    EmailPlaceHolder.Visible = true;
                    HasAccount.Visible = true;
                    Session.Remove("UserSession");
                    Session.Remove("ReferralCode");
                }
            }
        }

        private void BindProductInfos()
        {
            TotalTickets = int.Parse(HidTicket.Value);
            Session["TotalTickets"] = TotalTickets;
            promoRow.Visible = false;
            msrp.Visible = false;
            CurrentTicket.Text = TotalTickets == 1 ? "1 ticket" : string.Format("{0} tickets", TotalTickets);
            var actualPrice = NormalPrice;
            bool canReserveFreePass = false;
            ChargeTicket = TotalTickets;

            bool isShowPromoRow = false;
            perMoneyPrice.Attributes["class"] = "total-price";
            //var maxGuest = PublicProduct.MaxGuest <= 0 ? Constant.DefaultMaxGuest : PublicProduct.MaxGuest;

            if (Session[Constant.UpgradeKey] != null)
            {
                int bookingTempId = int.Parse(Session[Constant.UpgradeKey].ToString());
                _bookingsTemps = _bookingRepository.GetBookingsTempById(bookingTempId).Item1;

                actualPrice = actualPrice - DiscountPrice - _bookingsTemps.HotelPrice;
                isShowPromoRow = true;
                if (actualPrice <= 0)
                {
                    actualPrice = 0;
                }
            }
            else
            {
                if (Session[_discountKey] == null)
                {
                    PublicDiscountUsed = _bookingRepository.GetAutoPromosByProductId(PublicProduct.ProductId, _checkinDate)
                        .DistinctBy(d => d.Code).FirstOrDefault();

                    string json = JsonConvert.SerializeObject(PublicDiscountUsed, CustomSettings.SerializerSettings());
                    Session[_discountKey] = json;
                }
                else
                {
                    PublicDiscountUsed = JsonConvert.DeserializeObject<Discounts>(Session[_discountKey].ToString());
                }

                if (PublicDiscountUsed != null)
                {
                    if (PublicCustomerInfos != null)
                    {
                        PublicDiscountUsed = _discountRepository.VerifyDiscounts(PublicDiscountUsed, PublicCustomerInfos.CustomerId);
                    }
                    if (PublicDiscountUsed != null)
                    {
                        actualPrice = Helper.CalculateDiscount(PublicDiscountUsed, actualPrice, TotalTickets);
                        if (!actualPrice.Equals(NormalPrice))
                        {
                            msrp.Visible = true;
                            perMoneyPrice.Attributes["class"] += " has-discount";
                            isShowPromoRow = true;

                            // Use Discount so, display Code
                            if (!IsPostBack)
                            {
                                PromoText.Text = PublicDiscountUsed.Code.ToUpper();
                            }
                            discountInfoRow.Visible = true;
                            DiscountOffLit.Text = string.Format("{0} - {1} OFF", PublicDiscountUsed.DiscountName,
                                PublicDiscountUsed.PromoType == (int)Enums.PromoType.Fixed
                                    ? "$" + PublicDiscountUsed.PercentOff
                                    : PublicDiscountUsed.PercentOff + "%");
                        }
                    }
                }
            }

            if (isShowPromoRow)
            {
                promoRow.Visible = true;
            }

            BindPriceInfo();

            // Have active Subscription
            if (CheckIsActiveSubscription())
            {
                ChargeTicket -= 1;

                canReserveFreePass = true;
                FreePassCheckBox.Checked = true;
                FreePassCheckBox.Enabled = false;
            }

            var totalMerchantPrice = PublicProduct.ActualPriceWithDate.Price * ChargeTicket;

            if (totalMerchantPrice > 0)
            {
                rowTicket.Visible = true;
                TicketNumberLit.Text = string.Format("{0} x {1}",
                    Helper.FormatPriceWithFixed(PublicProduct.ActualPriceWithDate.Price),
                    ChargeTicket == 1 ? "1 ticket" : ChargeTicket + " tickets");

                TicketPriceLit.Text = Helper.FormatPriceWithFixed(totalMerchantPrice);
            }

            freePassRow.Visible = PublicDiscounts != null;
            WhyChargeLink.Visible = false;

            if (PublicDiscounts != null)
            {
                freePassModal.Visible = true;
                FreePassCheckBox.Checked = false;
                ExixtsFreePassCheckBox.Checked = false;
                OutsideCheckbox.Checked = false;
                NonDayPassCheckbox.Checked = false;

                WhyChargeLink.Visible = true;
                string tooltip = "<b>You are being charged because:</b>";
                freePassRow.Attributes["class"] += " row-2";

                if (canReserveFreePass)
                {
                    FreePassCheckBox.Checked = true;
                    if (TotalTickets <= 1)
                    {
                        WhyChargeLink.Visible = false;
                        freePassRow.Attributes["class"] = freePassRow.Attributes["class"].Replace(" row-2", "");
                    }
                }

                // Already have 1 active free pass exists
                if (TotalActiveBookingWithSubscription > 0)
                {
                    ExixtsFreePassCheckBox.Checked = true;
                }
                tooltip += string.Format("<div class='checkbox'>" +
                                         "<label>" +
                                         "<span >" +
                                         "<input id='ExixtsFreePassCheckBox' type='checkbox' {0} disabled='disabled'>" +
                                         "</span>You already have 1 active free pass{1}</label>" +
                                         "</div>",
                                        TotalActiveBookingWithSubscription > 0 ? "checked='checked'" : string.Empty,
                                        TotalActiveBookingWithSubscription > 0 ? " on " + ActiveDateSubscription.ToString("MM/dd") : string.Empty);

                // Outside of 24 hours
                if (_checkinDate.Date > DateTime.UtcNow.AddHours(AppConfiguration.ReserveSubscriptionPassInHours)
                        .ToLosAngerlesTimeWithTimeZone(PublicHotel.TimeZoneId).Date)
                {
                    OutsideCheckbox.Checked = true;
                }
                tooltip += string.Format("<div class='checkbox'>" +
                                       "<label>" +
                                       "<span >" +
                                       "<input type='checkbox' {0} disabled='disabled'>" +
                                       "</span>Check-in date is outside of a 24 hour window</label>" +
                                       "</div>",
                                       _checkinDate.Date > DateTime.UtcNow.AddHours(AppConfiguration.ReserveSubscriptionPassInHours)
                                            .ToLosAngerlesTimeWithTimeZone(PublicHotel.TimeZoneId).Date ? "checked='checked'" : string.Empty);

                tooltip += string.Format("<div class='checkbox'>" +
                                        "<label>" +
                                        "<span >" +
                                        "<input type='checkbox' {0} disabled='disabled'>" +
                                        "</span>More than 1 ticket is selected</label>" +
                                        "</div>",
                                        TotalTickets > 1 ? "checked='checked'" : string.Empty);

                // Non reserve day pass product
                if (PublicProduct.ProductType != (int)Enums.ProductType.DayPass)
                {
                    NonDayPassCheckbox.Checked = true;
                }

                tooltip += string.Format("<div class='checkbox'>" +
                                         "<label>" +
                                         "<span >" +
                                         "<input type='checkbox' {0} disabled='disabled'>" +
                                         "</span>You are reserving a non-day pass product</label>" +
                                         "</div>",
                                        PublicProduct.ProductType != (int)Enums.ProductType.DayPass ? "checked='checked'" : string.Empty);

                // Max
                if (TotalBookingInCurrentRecycle >= PublicDiscounts.MaxPurchases)
                {
                    MaxNumberPassesCheckbox.Checked = true;
                }

                tooltip += string.Format("<div class='checkbox'>" +
                                         "<label>" +
                                         "<span >" +
                                         "<input type='checkbox' {0} disabled='disabled'>" +
                                         "</span>You've exceeded your max number of passes per month</label>" +
                                         "</div>",
                    TotalBookingInCurrentRecycle >= PublicDiscounts.MaxPurchases ? "checked='checked'" : string.Empty);

                // Why Charge Link tooltip
                WhyChargeLink.Attributes["title"] = tooltip;
            }

            DiscountPrice = actualPrice;
            perMoneyPrice.Text = Helper.FormatPriceWithFixed(actualPrice);

            PromoLit.Text = string.Format("-{0}", Helper.FormatPriceWithFixed((NormalPrice - actualPrice) * TotalTickets));
            msrpPrice.Text = Helper.FormatPriceWithFixed(NormalPrice);

            //UpToGuestLit.Text = Helper.GetStringMaxGuest(maxGuest);

            // Tax Jar
            if (PublicMarkets.IsCalculateTax)
            {
                taxRow.Visible = true;
                var tax = _bookingRepository.GetTaxByDate(_checkinDate.Date, PublicHotel.HotelId);
                if (tax == null)
                {
                    try
                    {
                        var rates = _taxJarClient.TaxForOrder(new
                        {
                            from_country = "US",
                            from_zip = PublicHotel.ZipCode,
                            from_state = PublicHotel.State,
                            to_country = "US",
                            to_zip = PublicHotel.ZipCode,
                            to_state = PublicHotel.State,
                            amount = 50,
                            shipping = 0
                        });

                        tax = new Taxes
                        {
                            Date = _checkinDate.Date,
                            HotelId = PublicHotel.HotelId,
                            MerchantPrice = PublicProduct.ActualPriceWithDate.Price,
                            Quantity = TotalTickets,
                            TaxValue = (double)rates.Rate
                        };

                        _bookingRepository.AddTaxes(tax);
                        CacheLayer.Clear(CacheKeys.TaxesCacheKey);
                    }
                    catch (Exception ex)
                    {
                        ErrorMessageLit.Text =
                            "There was an error processing your request. Please contact our customer service for more details at <a href='mailto:help@dayaxe.com'>help@dayaxe.com</a>";

                        var log = new Logs
                        {
                            LogKey = "Error-TaxJar",
                            UpdatedContent = string.Format("{0} - {1} - {2}", ex.Message, ex.InnerException, ex.StackTrace),
                            UpdatedDate = DateTime.UtcNow,
                            UpdatedBy = PublicCustomerInfos != null ? PublicCustomerInfos.CustomerId : 0
                        };
                        _productRepository.AddLog(log);
                    }
                }

                TaxChargeAmount = tax != null && !tax.TaxValue.Equals(0) ? tax.TaxValue * ChargeTicket * actualPrice : 0;
                TaxLit.Text = Helper.FormatPriceWithFixed(TaxChargeAmount);
            }

            moneyPrice.Text = Helper.FormatPriceWithFixed(actualPrice * ChargeTicket + TaxChargeAmount);

            // Credit
            if (PublicCustomerCredits != null)
            {
                DayaxeCreditCardRow.Visible = true;
                double totalPrice = actualPrice * TotalTickets + TaxChargeAmount;
                if (CheckIsActiveSubscription())
                {
                    totalPrice = actualPrice * ChargeTicket + TaxChargeAmount;
                    PromoLit.Text = string.Format("-{0}", Helper.FormatPriceWithFixed((NormalPrice - actualPrice) * ChargeTicket));
                }
                PayByCreditRow.Visible = true;
                creditRow.Visible = true;

                if (!IsPostBack)
                {
                    IsPayByCreditCheckBox.Checked = true;
                }

                if (PublicCustomerCredits.Amount >= totalPrice)
                {
                    PayByCreditLiteral.Text = string.Format("Use <b>${0:0.00} of ${1:0.00} DayAxe Credit balance</b>",
                        totalPrice, PublicCustomerCredits.Amount);
                    DCreditInfoLit.Text = string.Format("Use <b>${0:0.00} of ${1:0.00} DayAxe Credit balance</b>",
                        totalPrice, PublicCustomerCredits.Amount);
                    DCardInfoLiteral.Text = string.Format("Use your <b>${0:0.00} of ${1:0.00} DayAxe Credit balance</b>",
                        totalPrice, PublicCustomerCredits.Amount);

                    if (DCreditCheckBox.Checked && totalPrice > 0)
                    {
                        CreditLit.Text = string.Format("-{0}", Helper.FormatPriceWithFixed(totalPrice));

                        // Should be 0 here because Credit more than charge amount
                        moneyPrice.Text = Helper.FormatPriceWithFixed(0);
                    }
                    else
                    {
                        creditRow.Visible = false;
                    }
                }
                else if (PublicCustomerCredits.Amount > 0 && totalPrice > 0)
                {
                    PayByCreditLiteral.Text = string.Format("<b>${0:0.00} DayAxe Credit balance</b>",
                        PublicCustomerCredits.Amount);
                    DCreditInfoLit.Text =
                        string.Format("{0:0.00} DayAxe Credit balance", PublicCustomerCredits.Amount);
                    DCardInfoLiteral.Text =
                        string.Format("{0:0.00} DayAxe Credit balance", PublicCustomerCredits.Amount);

                    if (DCreditCheckBox.Checked)
                    {
                        CreditLit.Text = string.Format("-{0}", Helper.FormatPriceWithFixed(PublicCustomerCredits.Amount));

                        moneyPrice.Text = Helper.FormatPriceWithFixed(totalPrice - PublicCustomerCredits.Amount);
                    }
                    else
                    {
                        creditRow.Visible = false;
                    }
                }
                else
                {
                    PayByCreditRow.Visible = false;
                    DayaxeCreditCardRow.Visible = false;
                    creditRow.Visible = false;
                }
            }
        }

        protected string GetProductUrl()
        {
            return string.Format("{0}/{1}/{2}/{3}/{4}/{5}",
                EmailConfig.DefaultImageUrlSendEmail,
                Page.RouteData.Values["market"],
                Page.RouteData.Values["city"],
                Page.RouteData.Values["hotelName"],
                Page.RouteData.Values["productName"],
                PublicProduct.ProductId);
        }

        protected void CheckInDateText_OnTextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(CheckInDateText.Text))
            {
                Session["CheckInDateSearch"] = CheckInDateText.Text;
            }
            else
            {
                Session.Remove("CheckInDateSearch");
            }

            BindTicketCapacity();
            BookProductUpdatePanel.Update();

            ScriptManager.RegisterClientScriptBlock(BookProductUpdatePanel, typeof(string), "openUpgradeSubscription", "delete window.upgradeSubscription; delete window.conrirmPage; delete window.subscriptionDetailUrl; delete window.conrirmPageUrl;", true);
            ScriptManager.RegisterClientScriptBlock(BookProductUpdatePanel, typeof(string), "openConfirmPopup", "delete window.showConfirmM;", true);
        }

        private void BindTicketCapacity()
        {
            _maxTicket = _productRepository.GetDefaultPassLimit(PublicProduct.ProductId, _checkinDate);

            TicketCapacity.Controls.Clear();

            for (var i = 1; i <= _maxTicket; i++)
            {
                var liTag = new HtmlGenericControl("li");
                var link = new HtmlAnchor
                {
                    InnerText = string.Format("{0} ticket{1}", i, i == 1 ? string.Empty : "s")
                };
                liTag.Controls.Add(link);
                TicketCapacity.Controls.Add(liTag);
            }
        }

        protected void PurchaseSubscriptionButton_OnClick(object sender, EventArgs e)
        {
            int bookingId;
            if (Session["CurrentBookingUpgradeSubscription"] != null &&
                int.TryParse(Session["CurrentBookingUpgradeSubscription"].ToString(), out bookingId) &&
                bookingId > 0)
            {
                int subscriptionId = AppConfiguration.SubscriptionUpgradeId;
                var upgradeSubscription = _bookingRepository.SubscriptionsList.FirstOrDefault(s => s.Id == subscriptionId && s.IsActive);
                var bookings = _bookingRepository.GetBookings(bookingId);
                var customerInfos = _bookingRepository.GetCustomer(bookings.CustomerId);
                bool isLimit = false;

                if (upgradeSubscription != null)
                {

                    double actualPrice = upgradeSubscription.Price;
                    bool hasCoupon = false;
                    string stripeCouponId = string.Empty;
                    string creditLogDescription = string.Empty;
                    double refundCreditByUpgrade = 0;
                    string refundCreditDescription = string.Empty;
                    double payByCredit = 0;
                    var paramDiscount = new GetDiscountValidByCodeParams
                    {
                        Code = AppConfiguration.PromoCodeUpgrade,
                        ProductId = 0,
                        CustomerId = PublicCustomerInfos != null ? PublicCustomerInfos.CustomerId : 0,
                        IsAdmin = false,
                        BookingId = 0,
                        SubscriptionId = upgradeSubscription.Id,
                        TimezoneId = PublicHotel.TimeZoneId
                    };
                    var discounts = _discountRepository.GetDiscountValidByCode(paramDiscount, out isLimit);

                    var subscriptionOptions = new StripeSubscriptionCreateOptions
                    {
                        PlanId = upgradeSubscription.StripePlanId
                    };

                    // Have Discount
                    if (discounts != null && discounts.Id > 0)
                    {
                        discounts = _discountRepository.VerifyDiscountsSubscription(discounts, PublicCustomerInfos.CustomerId, upgradeSubscription.Id);

                        // Discount Invalid
                        if (discounts != null)
                        {
                            actualPrice = Helper.CalculateDiscount(discounts, actualPrice, TotalTickets);
                            hasCoupon = true;
                        }
                    }

                    //if (bookings.PayByCredit > 0)
                    //{
                    //    switch (bookings.Quantity)
                    //    {
                    //        case 1: // Refund All if purchase only 1 ticket
                    //            refundCreditByUpgrade = bookings.PayByCredit;
                    //            break;
                    //        default:
                    //            refundCreditByUpgrade = bookings.PayByCredit / bookings.Quantity;
                    //            break;
                    //    }

                    //    var market =
                    //        _productRepository.MarketList.FirstOrDefault(
                    //            m => m.Permalink == (string)Page.RouteData.Values["market"] && m.IsActive);

                    //    refundCreditDescription = string.Format("{0} – {1} – {2} – {3}",
                    //        PublicProduct.ProductName,
                    //        PublicHotel.HotelName,
                    //        market != null ? market.LocationName : "",
                    //        bookings.BookingIdString);

                    //    // Add return amount and calculate with DayAxe credit
                    //    payByCredit = actualPrice > (PublicCustomerCredits.Amount + refundCreditByUpgrade)
                    //        ? actualPrice - (PublicCustomerCredits.Amount + refundCreditByUpgrade)
                    //        : actualPrice;

                    //    hasCoupon = true;
                    //}

                    if (hasCoupon)
                    {
                        // Price - Discount - Pay By Credit => charge Price
                        //var couponPrice = upgradeSubscription.Price - (upgradeSubscription.Price - actualPrice) - payByCredit;
                        var couponPrice = upgradeSubscription.Price - actualPrice;
                        // Create Coupon Id because we used DayAxe Credit
                        stripeCouponId = string.Format("{0}C{1}", PublicCustomerInfos.CustomerId, Helper.RandomString(Constant.BookingCodeLength));
                        var couponOptions = new StripeCouponCreateOptions
                        {
                            Id = stripeCouponId,
                            AmountOff = Convert.ToInt32(couponPrice * 100), // USD
                            Duration = "once",
                            Currency = "USD"
                        };

                        // Create Coupon
                        var couponService = new StripeCouponService();
                        var coupon = couponService.Create(couponOptions);

                        subscriptionOptions.CouponId = coupon.Id;

                        creditLogDescription = string.Format("{0} – {1}",
                            upgradeSubscription.Name, stripeCouponId);
                    }

                    // Create Subscription on Stripe
                    var subscriptionService = new StripeSubscriptionService();
                    StripeSubscription subscription = subscriptionService.Create(customerInfos.StripeCustomerId, subscriptionOptions);

                    var subscriptionBookings = new SubscriptionBookings
                    {
                        SubscriptionId = upgradeSubscription.Id,
                        Quantity = 1,
                        StripeCouponId = stripeCouponId,
                        BookedDate = DateTime.UtcNow,
                        ActivedDate = subscription.Start,
                        StartDate = subscription.CurrentPeriodStart,
                        EndDate = subscription.CurrentPeriodEnd,
                        Status = (int)Enums.SubscriptionBookingStatus.Active,
                        CustomerId = customerInfos.CustomerId,
                        LastUpdatedDate = DateTime.UtcNow,
                        LastUpdatedBy = customerInfos.CustomerId,
                        StripeSubscriptionId = subscription.Id
                    };

                    //BookingId = bookingId,
                    var param = new AddSubscriptionBookingParams
                    {
                        RefundCreditByUpgrade = refundCreditByUpgrade,
                        RefundCreditDescription = refundCreditDescription,
                        SubscriptionBookingsObject = subscriptionBookings,
                        CustomerCreditsObject = PublicCustomerCredits,
                        Description = creditLogDescription,
                        SubscriptionName = upgradeSubscription.Name,
                        FirstName = customerInfos.FirstName,
                        LastName = customerInfos.LastName,
                        BookingId = 0,
                        SubscriptionBookingDiscounts = discounts,
                        ActualPrice = actualPrice,
                        MerchantPrice = upgradeSubscription.Price,
                        PayByCredit = payByCredit,
                        TotalPrice = actualPrice,
                        MaxPurchases = upgradeSubscription.MaxPurchases
                    };

                    int subScriptionBookingId = _subscriptionBookingRepository.Add(param);

                    CacheLayer.Clear(CacheKeys.SubscriptionBookingsCacheKey);
                    CacheLayer.Clear(CacheKeys.SubscriptionCyclesCacheKey);
                    CacheLayer.Clear(CacheKeys.CustomerInfosCacheKey);
                    CacheLayer.Clear(CacheKeys.CustomerCreditsCacheKey);
                    CacheLayer.Clear(CacheKeys.CustomerCreditLogsCacheKey);
                    CacheLayer.Clear(CacheKeys.DiscountsCacheKey);
                    CacheLayer.Clear(CacheKeys.SubsciptionDiscountUsedCacheKey);
                    CacheLayer.Clear(CacheKeys.SubscriptionDiscountsCacheKey);
                    CacheLayer.Clear(CacheKeys.SubscriptionBookingDiscountsCacheKey);

                    CacheLayer.Clear(CacheKeys.BookingsCacheKey);

                    //Session.Remove("CurrentBookingUpgradeSubscription");

                    // Redirect to Confirm Subscription
                    //var subscriptionDetailUrl = string.Format(Constant.SubscriptionConfirmPage,
                    //    upgradeSubscription.StripePlanId,
                    //    subScriptionBookingId,
                    //    "/" + bookingId);
                    //Response.Redirect(subscriptionDetailUrl, false);

                    //ScriptManager.RegisterClientScriptBlock(BookProductUpdatePanel,
                    //    typeof(string),
                    //    "redirectToConfirmSubscription",
                    //    string.Format("delete window.upgradeSubscription; window.subscriptionDetailUrl = '{0}';",
                    //        subscriptionDetailUrl),
                    //    true);

                    Session["ShowGoldPassConfirmation"] = true;

                    var confirmUrl = string.Format(Constant.ConfirmProductPage,
                        Page.RouteData.Values["market"],
                        Page.RouteData.Values["city"],
                        Page.RouteData.Values["hotelName"],
                        Page.RouteData.Values["productName"],
                        bookingId);

                    Response.Redirect(confirmUrl, false);

                    ScriptManager.RegisterClientScriptBlock(BookProductUpdatePanel,
                       typeof(string),
                       "openUpgradeSubscription",
                       string.Format("window.upgradeSubscription = 'true'; window.confirmPageUrl = '{0}';",
                       confirmUrl),
                       true);
                }
            }
        }

        protected void ViewBookingOnClick(object sender, EventArgs e)
        {
            try
            {
                //int bookingId;
                //if (Session["CurrentBookingUpgradeSubscription"] != null &&
                //    int.TryParse(Session["CurrentBookingUpgradeSubscription"].ToString(), out bookingId))
                //{
                //    var bookings = _bookingRepository.GetBookings(bookingId);
                //    string responseStripe = string.Empty;
                //    if (!string.IsNullOrEmpty(bookings.StripeChargeId))
                //    {
                //        var chargeService = new StripeChargeService();
                //        StripeCharge charge = chargeService.Capture(bookings.StripeChargeId);
                //        responseStripe = charge.StripeResponse.ResponseJson;
                //    }

                //    var logs = new Logs
                //    {
                //        LogKey = "UpgradeSubscriptionCapture",
                //        UpdatedContent = string.Format("Response: {0}",
                //            responseStripe),
                //        UpdatedBy = PublicCustomerInfos != null ? PublicCustomerInfos.CustomerId : 0,
                //        UpdatedDate = DateTime.UtcNow
                //    };

                //    bookings.PassStatus = (int)Enums.BookingStatus.Active;
                //    bookings.IsActiveSubscription = false;

                //    _bookingRepository.AddLog(logs);
                //    _bookingRepository.UpdateConfirmSend(bookings);
                //}

                var confirmUrl = string.Format(Constant.ConfirmProductPage,
                    Page.RouteData.Values["market"],
                    Page.RouteData.Values["city"],
                    Page.RouteData.Values["hotelName"],
                    Page.RouteData.Values["productName"],
                    Session["CurrentBookingUpgradeSubscription"]);

                ScriptManager.RegisterClientScriptBlock(BookProductUpdatePanel,
                    typeof(string),
                    "redirectPageToConfirm",
                    string.Format("delete window.upgradeSubscription; window.confirmPageUrl = '{0}';",
                        confirmUrl),
                    true);
            }
            catch (Exception ex)
            {
                var log = new Logs
                {
                    LogKey = "BccError",
                    UpdatedContent = string.Format("BookingId: {0} - {1} - {2} - {3}", Session["CurrentBookingUpgradeSubscription"], ex.Message, ex.StackTrace, ex.Source),
                    UpdatedDate = DateTime.UtcNow,
                    UpdatedBy = 1
                };
                _bookingRepository.AddLog(log);
            }
        }

        private void BindPriceInfo()
        {

        }

        private bool CheckIsActiveSubscription()
        {
            return (PublicDiscounts != null &&
                   !IsHaveBookingWithSelectedDate &&
                   TotalActiveBookingWithSubscription < AppConfiguration.MaxReserveSubscriptionPass &&
                   _checkinDate.Date <= DateTime.UtcNow.AddHours(AppConfiguration.ReserveSubscriptionPassInHours).ToLosAngerlesTimeWithTimeZone(PublicHotel.TimeZoneId).Date &&
                   PublicProduct.ProductType == (int)Enums.ProductType.DayPass &&
                   TotalBookingInCurrentRecycle < PublicDiscounts.MaxPurchases);
        }

        protected void DCreditCheckBox_OnCheckedChanged(object sender, EventArgs e)
        {
            BindProductInfos();
        }
    }
}