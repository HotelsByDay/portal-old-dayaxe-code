using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using DayaxeDal;
using DayaxeDal.Custom;
using DayaxeDal.Extensions;
using DayaxeDal.Repositories;
using Newtonsoft.Json;

namespace dayaxe.com
{
    public partial class HotelProduct : BasePage
    {
        protected Products PublicProduct { get; set; }
        protected Hotels PublicHotel { get; set; }
        protected Markets PublicMarkets { get; set; }
        protected bool ShowAuth { get; set; }
        protected string[] PublicImages { get; set; }
        protected string ProductTypeTrackString { get; set; }
        protected double RegularPrice { get; set; }
        protected double DiscountPrice { get; set; }
        protected string PublicTickets { get; set; }
        protected string ProductBlockoutDate { get; set; }
        private DateTime RestrictDate { get; set; }
        protected string RestrictDateStr { get; set; }

        private SurveyRepository _surveyRepository = new SurveyRepository();
        private readonly ProductRepository _productRepository = new ProductRepository();
        private readonly DiscountRepository _discountRepository = new DiscountRepository();

        private DateTime _selectedCheckInDate;
        private int _maxTicket;
        private int _productId;
        private SearchParams _searchParams;
        private int TotalTickets { get; set; }
        private bool IsHaveBookingWithSelectedDate { get; set; }
        private int TotalActiveBookingWithSubscription { get; set; }

        private int TotalBookingInCurrentRecycle { get; set; }

        private List<Surveys> _surveys;

        protected void Page_Init(object sender, EventArgs e)
        {
            if (Master == null)
            {
                throw new HttpException(404, ErrorMessage.MasterNotFound);
            }
            HtmlGenericControl body = (HtmlGenericControl)Master.FindControl("body");
            body.Attributes["class"] += " product-page";

            // Search Production with same reason
            var markets = _productRepository.MarketList.FirstOrDefault(m => m.Permalink == (string)Page.RouteData.Values["market"] && m.IsActive) ??
                          _productRepository.MarketList.First(m => m.IsDefault);
            int totalGuest = 2;
            if (Session["Guest"] != null)
            {
                int.TryParse(Session["Guest"].ToString(), out totalGuest);
            }

            DateTime selectedDate = DateTime.UtcNow.ToLosAngerlesTimeWithTimeZone(PublicHotel != null ? PublicHotel.TimeZoneId : string.Empty);
            if (Session["CheckInDateSearch"] != null)
            {
                DateTime.TryParseExact((string)Session["CheckInDateSearch"], "MM/dd/yyyy", null,
                    DateTimeStyles.None, out selectedDate);
            }

            _searchParams = new SearchParams
            {
                CustomerId = PublicCustomerInfos != null ? PublicCustomerInfos.CustomerId : 0,
                SearchMarkets = markets,
                ProductType = new List<Enums.ProductType> { Enums.ProductType.DayPass },
                AvailableTickets = totalGuest,
                StartDate = selectedDate
            };

            if (Page.RouteData.Values["hotelName"] != null)
            {
                if (Page.RouteData.Values["id"] != null)
                {
                    if (int.TryParse(Page.RouteData.Values["id"].ToString(), out _productId))
                    {
                        PublicProduct = _productRepository.GetById(_productId, _searchParams);
                    }
                }

                if (PublicProduct == null)
                {
                    PublicProduct = _productRepository.GetProductsByName((string)Page.RouteData.Values["hotelName"],
                        (string)Page.RouteData.Values["productName"],
                        (string)Session["UserSession"]);

                    if (IsPostBack && PublicProduct != null && PublicProduct.Similarproduct == null)
                    {
                        PublicProduct = _productRepository.GetById(PublicProduct.ProductId, _searchParams);
                        HotelRatingPanel.Update();
                    }
                }

                if (!IsPostBack)
                {
                    // Maintain old Url of Survey
                    if (PublicProduct == null && Page.RouteData.Values["id"] != null && _productId <= 0)
                    {
                        var reviews = _surveyRepository.GetSurveysByCode(Page.RouteData.Values["id"].ToString());
                        if (reviews != null)
                        {
                            var bookings = _surveyRepository.BookingList
                                .FirstOrDefault(b => b.BookingId == reviews.BookingId);
                            if (bookings != null)
                            {
                                PublicProduct = _productRepository.GetById(bookings.ProductId);
                                var newProductUrl = string.Format("/{0}/{1}/{2}/{3}/{4}",
                                    Page.RouteData.Values["market"],
                                    PublicProduct.Hotels.CityUrl,
                                    PublicProduct.Hotels.HotelNameUrl,
                                    PublicProduct.ProductNameUrl,
                                    reviews.Code);
                                Response.Redirect(Helper.ResolveRelativeToAbsoluteUrl(Request.Url, newProductUrl), true);
                            }
                        }
                    }

                    // Maintain old Url of Product
                    if (PublicProduct == null && Page.RouteData.Values["id"] == null)
                    {
                        PublicProduct = _productRepository.GetProductByHotelName((string) Page.RouteData.Values["hotelName"]);
                        var newProductUrl = string.Format("/{0}/{1}/{2}/{3}/{4}",
                            Page.RouteData.Values["market"],
                            PublicProduct.Hotels.CityUrl,
                            PublicProduct.Hotels.HotelNameUrl,
                            PublicProduct.ProductNameUrl,
                            PublicProduct.ProductId);
                        Response.Redirect(Helper.ResolveRelativeToAbsoluteUrl(Request.Url, newProductUrl), true);
                    }

                    PublicHotel = _productRepository.HotelList.First(h => h.HotelId == PublicProduct.HotelId);

                    // Do not have permission to view this page
                    if (PublicHotel != null && 
                        !PublicHotel.IsPublished &&
                        (PublicCustomerInfos == null || !PublicCustomerInfos.IsAdmin))
                    {
                        Response.Redirect(Constant.SearchPageDefault);
                    }

                    if (Session["CheckInDateSearch"] == null && PublicProduct != null && PublicProduct.ProductType == (int) Enums.ProductType.AddOns)
                    {
                        var productHaveBooking = (from p in _productRepository.ProductList
                            join ap in _productRepository.ProductAddOnList on p.ProductId equals ap.AddOnId
                            where ap.AddOnId == PublicProduct.ProductId
                            select ap.ProductId).FirstOrDefault();
                        var bookings =
                            _productRepository.BookingList.FirstOrDefault(
                                b => b.ProductId == productHaveBooking && b.CheckinDate.HasValue &&
                                     b.CheckinDate.Value.ToLosAngerlesTimeWithTimeZone(PublicHotel.TimeZoneId).Date >= DateTime.UtcNow.ToLosAngerlesTimeWithTimeZone(PublicHotel.TimeZoneId).Date);

                        if (bookings != null && bookings.CheckinDate.HasValue)
                        {
                            Session["CheckInDateSearch"] = bookings.CheckinDate.Value.ToLosAngerlesTimeWithTimeZone(PublicHotel.TimeZoneId).Date
                                .ToString("MM/dd/yyyy");
                        }
                    }

                    var sessionCheckInDate = Session["CheckInDateSearch"];
                    if (sessionCheckInDate != null || PublicProduct.IsOnBlackOutDay)
                    {
                        // On Blackout Day
                        if (sessionCheckInDate != null && !string.IsNullOrEmpty(sessionCheckInDate.ToString()))
                        {
                            DateTime.TryParseExact(sessionCheckInDate.ToString(), "MM/dd/yyyy", null, DateTimeStyles.None, out _selectedCheckInDate);
                        }
                        if (!IsPostBack && PublicProduct.IsOnBlackOutDay)
                        {
                            _selectedCheckInDate = PublicProduct.NextAvailableDate;
                        }
                    }
                    else if (PublicCustomerInfos != null && PublicProduct.ProductType == (int) Enums.ProductType.AddOns)
                    {
                        _selectedCheckInDate = _productRepository.GetMostRecentDate(PublicCustomerInfos.CustomerId);
                    }
                    else
                    {
                        _selectedCheckInDate = DateTime.UtcNow.ToLosAngerlesTime();
                    }

                    if (Session["TotalTickets"] != null)
                    {
                        TotalTicketsText.Text = Session["TotalTickets"].ToString();
                    }
                    else
                    {
                        TotalTicketsText.Text = PublicProduct.ProductType == (int)Enums.ProductType.DayPass ? "2" : "1";
                    }
                    
                    TotalTickets = int.Parse(TotalTicketsText.Text);

                    CheckInDateTextMobile.Text = _selectedCheckInDate.ToString(Constant.DiscountDateFormat);
                    CheckInDateTextDesktop.Text = _selectedCheckInDate.ToString(Constant.DiscountDateFormat);
                    HidSelectedDateBefore.Value = _selectedCheckInDate.ToString(Constant.DiscountDateFormat);
                    Session["CheckInDateSearch"] = _selectedCheckInDate.ToString(Constant.DiscountDateFormat);
                    _searchParams.StartDate = _selectedCheckInDate;
                    CheckHaveBookingOnSelectedDate(_selectedCheckInDate);
                    var price = _productRepository.GetById(PublicProduct.ProductId, _searchParams).ActualPriceWithDate;
                    BindPrice(price);

                    HidSelectedDate.Value = "1";
                    BindSurvey();

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

                    if (Request.Browser["IsMobileDevice"] == "true")
                    {
                        CheckInDateTextMobile.Visible = false;
                    }

                    // Reviews
                    if (Page.RouteData.Values["id"] == null && _productId == 0)
                    {
                        Survey.Visible = false;
                    }

                    if (PublicCustomerInfos != null)
                    {
                        EmailAddressText.Text = PublicCustomerInfos.EmailAddress;
                    }

                    CheckInDateRequestText.Text = _productRepository.GetFirstSoldOutDate(PublicProduct.ProductId)
                        .ToString("MMMM dd, yyyy");
                }
            }

            if (PublicProduct == null)
            {
                if (PublicCustomerInfos != null && !string.IsNullOrEmpty(PublicCustomerInfos.BrowsePassUrl))
                {
                    Response.Redirect(PublicCustomerInfos.BrowsePassUrl);
                }
                Response.Redirect(!string.IsNullOrEmpty((string) Session["SearchPage"])
                    ? Session["SearchPage"].ToString()
                    : Constant.SearchPageDefault);
            }

            PublicHotel = _productRepository.HotelList.First(h => h.HotelId == PublicProduct.HotelId);

            PublicMarkets = _productRepository.GetMarketsByHotelId(PublicProduct.HotelId);

            goBack.HRef = GetUrlSearchPage();

            BindProductInfo();

            SetMetaHeaderInfo();

            BindAmenties();

            var tickets = _productRepository.GetTicketsFuture(PublicProduct.ProductId);
            string json = JsonConvert.SerializeObject(tickets, CustomSettings.SerializerSettings());
            PublicTickets = json;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Helper.BrowserIsMobile())
            {
                DateTime.TryParse(CheckInDateTextDesktop.Text, out _selectedCheckInDate);
            }
            else
            {
                DateTime.TryParse(CheckInDateTextMobile.Text, out _selectedCheckInDate);
            }
            BindTicketCapacity();

            if (IsPostBack)
            {
                CheckHaveBookingOnSelectedDate(_selectedCheckInDate);
            }

            if (Page.RouteData.Values["id"] != null && Session["Survey_Finish_" + Page.RouteData.Values["id"]] != null)
            {
                _surveyRepository = new SurveyRepository();
                BindSurvey();
            }

            TotalTickets = int.Parse(TotalTicketsText.Text);
            CurrentTicket.Text = TotalTickets == 1 ? "1 ticket" : string.Format("{0} tickets", TotalTickets);
            CurrentTicketDesktop.Text = TotalTickets == 1 ? "1 ticket" : string.Format("{0} tickets", TotalTickets);

            RestrictDate = DateTime.UtcNow.ToLosAngerlesTimeWithTimeZone(PublicHotel != null
                    ? PublicHotel.TimeZoneId
                    : string.Empty);

            if (_selectedCheckInDate.Date == RestrictDate.Date && 
                RestrictDate > DateTime.UtcNow.ToLosAngerlesTimeWithTimeZone(PublicHotel != null ? PublicHotel.TimeZoneId : string.Empty).Date.AddHours(19))
            {
                RestrictDate = RestrictDate.AddDays(1);
            }
            RestrictDateStr = RestrictDate.ToString(Constant.DiscountDateFormat);
        }

        protected string GetTitleImage(string imageUrl)
        {
            if (imageUrl.Contains("cover"))
            {
                return "Hotel Cover";
            }
            else if (imageUrl.Contains("pool"))
            {
                return "Hotel Pool";
            }
            else if (imageUrl.Contains("gym"))
            {
                return "Hotel Gym";
            }
            else if (imageUrl.Contains("spa"))
            {
                return "Hotel Spa";
            }
            else if (imageUrl.Contains("office"))
            {
                return "Hotel Office";
            }
            return string.Empty;
        }

        protected void BookNow_Click(object sender, EventArgs e)
        {
            Session["Selected_" + _productId] = HidSelectedDate.Value;
            Session["TotalTickets"] = TotalTicketsText.Text;

            if (Page.RouteData.Values["hotelName"] != null)
            {
                Response.Redirect(string.Format(Constant.BookProductPage, 
                    Page.RouteData.Values["market"],
                    PublicHotel.CityUrl,
                    PublicHotel.HotelNameUrl,
                    PublicProduct.ProductNameUrl,
                    PublicProduct.ProductId));
            }
        }

        protected void ReadMoreButtonClick(object sender, EventArgs e)
        {
            Response.Redirect(string.Format("/{0}/{1}/{2}/{3}/reviews", 
                Page.RouteData.Values["market"], 
                Page.RouteData.Values["city"], 
                Page.RouteData.Values["hotelName"], 
                Page.RouteData.Values["productName"]));
        }

        protected void MarketRepeaterItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var redeemedDateLabel = (Label) e.Item.FindControl("RedeemedDateLabel");
                var survey = (Surveys) e.Item.DataItem;
                if (survey.Bookings.RedeemedDate.HasValue && redeemedDateLabel != null)
                {
                    redeemedDateLabel.Text = string.Format("{0}",
                        survey.Bookings.RedeemedDate.Value.ToString(Constant.ReviewsDateFormat));
                }
            }
            if (e.Item.ItemType == ListItemType.Footer)
            {
                if (_surveys.Count() > 5)
                {
                    var readMoreButton = (Button) e.Item.FindControl("ReadMoreButton");
                    readMoreButton.Visible = true;
                    readMoreButton.Text = string.Format("Read More ({0})", _surveys.Count());
                }
            }
        }

        private string GetUrlSearchPage()
        {
            if (Session[Constant.UpgradeKey] != null)
            {
                return Request.RawUrl;
            }
            return string.Format(Constant.DayPassPage, Page.RouteData.Values["market"]);
        }

        private void SetMetaHeaderInfo()
        {
            TitleLiteral.Text = string.Format("{0} {1} {2} | DayAxe.com", 
                PublicProduct.ProductName, 
                PublicHotel.City, 
                Helper.GetStringPassByProductType(PublicProduct.ProductType));
            string metaStr = string.Format("<meta name=\"description\" content=\"{0}\" />"
                                               + "<meta name=\"keywords\" content=\"{1}\" />"
                                               + "<link rel=\"canonical\" href=\"{2}\" />",
                PublicProduct.MetaDescription,
                PublicProduct.MetaKeyword,
                Request.Url.AbsoluteUri);
            var metaCtrl = Page.ParseControl(metaStr);
            if (metaCtrl != null)
            {
                MetaPlaceHolder.Controls.Add(metaCtrl);
            }

            ProductTypeTrackString = Helper.GetStringPassByProductType(PublicProduct.ProductType);
        }

        private void BindAmenties()
        {
            var amentiesControl = Helper.BindAmenties(PublicHotel);

            var objControl = Page.ParseControl(amentiesControl.PoolAmentyControl);
            if (objControl != null)
            {
                poolAmenties.Controls.Add(objControl);
            }

            objControl = Page.ParseControl(amentiesControl.GymAmentyControl);
            if (objControl != null)
            {
                gymAmenties.Controls.Add(objControl);
            }

            objControl = Page.ParseControl(amentiesControl.SpaAmentyControl);
            if (objControl != null)
            {
                spaAmenties.Controls.Add(objControl);
            }

            objControl = Page.ParseControl(amentiesControl.BusinessCenterControl);
            if (objControl != null)
            {
                officeAmenties.Controls.Add(objControl);
            }

            objControl = Page.ParseControl(amentiesControl.DiningControl);
            if (objControl != null)
            {
                dinningAmenties.Controls.Add(objControl);
            }

            objControl = Page.ParseControl(amentiesControl.EventControl);
            if (objControl != null)
            {
                eventAmenties.Controls.Add(objControl);
            }

            LiMainTab.Text = amentiesControl.MainTabString;
        }

        private void BindProductInfo()
        {
            PublicImages = PublicProduct.PhotoUrls;
            if (!PublicImages.Any())
            {
                PublicImages = new [] { Constant.ImageDefault };
            }
            streetname.Text = PublicHotel.StreetAddress;

            // Related Product With Price on Check-In date selectecd
            LvRelatedProductsRepeater.DataSource = PublicProduct.RelatedProducts;
            LvRelatedProductsRepeater.DataBind();

            SimilarProductLv.DataSource = PublicProduct.Similarproduct;
            SimilarProductLv.DataBind();
            
            if (PublicProduct.FinePrint.Any())
            {
                RptFinePrint.DataSource = PublicProduct.FinePrint;
                RptFinePrint.DataBind();
            }

            if (PublicProduct.RelatedProducts.Any())
            {
                moreAtHotelRow.Visible = true;
                afterMoreAtRow.Visible = true;
            }

            if (!IsPostBack)
            {
                try
                {
                    // Get FAQs from services
                    var faqContent = Helper.Get(Constant.FaqUrl);
                    var faqs = JsonConvert.DeserializeObject<FaqResultObject>(faqContent);
                    if (faqs.Count > 0)
                    {
                        RptFaqs.DataSource = faqs.Articles.Take(10);
                        RptFaqs.DataBind();
                    }
                }
                catch (Exception) { }

                CityStateZip.Text = string.Format("{0}, {1} {2}", PublicHotel.City, PublicHotel.State, PublicHotel.ZipCode);

                WhyWeLikeIt.Text = PublicHotel.Recommendation;

                if (!string.IsNullOrEmpty(PublicProduct.WhatYouGet))
                {
                    wrapWhatYouGet.Visible = true;
                    WhatYouGetLit.Text = PublicProduct.WhatYouGet;
                }

                if (!string.IsNullOrEmpty(PublicProduct.Service))
                {
                    wrapYourService.Visible = true;
                    YourServiceLit.Text = PublicProduct.Service;
                }

                HotelNameLit.Text = Helper.GetHotelName(PublicProduct);

                if (PublicHotel.IsComingSoon.HasValue && PublicHotel.IsComingSoon.Value)
                {
                    priceMobile.Visible = false;

                    //priceDesktop.Visible = false;
                    msrp.Visible = false;
                    msrp2.Visible = false;
                    PerGuestLit2.Visible = true;
                    PerGuestLit2.Text = "<div>Coming Soon</div>";
                    book.Attributes["class"] += " coming-soon";

                    passleftDiv.Visible = true;
                    LitPassleft.Text = Constant.CommingSoonString;
                    AnchorButton.Visible = false;
                    ComingSoonAnchorButton.Visible = true;
                }
                else
                {
                    AnchorButton.Visible = true;
                    ComingSoonAnchorButton.Visible = false;
                }
                
                var blockedDates = _productRepository.BlockedDatesCustomPriceList
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
                
                KidAllowLabel.Text = PublicProduct.ProductKidAllowedString.ToUpper();
            }

            var listProducts = PublicProduct.RelatedProducts ?? new List<Products>();

            if (PublicProduct.Similarproduct != null)
            {
                listProducts.AddRange(PublicProduct.Similarproduct);
            }
            listProducts = listProducts.DistinctBy(x => x.ProductId).ToList();

            const string mixpanelscript = "MixpanelScript";
            string strScript = string.Empty;
            listProducts.ForEach(item =>
            {
                strScript += Helper.GetMixpanelScriptRedirect(item.ProductId,
                    string.Format("/{0}/{1}/{2}/{3}/{4}",
                        Page.RouteData.Values["market"] ?? "socal",
                        Helper.ReplaceSpecialCharacter(item.Hotels.City),
                        Helper.ReplaceSpecialCharacter(item.Hotels.HotelName),
                        Helper.ReplaceSpecialCharacter(item.ProductName),
                        item.ProductId));
            });
            if (!string.IsNullOrEmpty(strScript))
            {
                ClientScript.RegisterClientScriptBlock(GetType(), mixpanelscript, strScript, true);
            }
        }

        private void BindPrice(ActualPriceObject price = null, DateTime? selectedCheckInDate = null)
        {
            double actualPrice = PublicProduct.LowestPrice;
            double msrpPrice = PublicProduct.LowestPrice;
            double discountPrice = PublicProduct.LowestUpgradeDiscount;
            if (price != null)
            {
                actualPrice = price.Price;
                msrpPrice = price.Price;
                discountPrice = price.DiscountPrice;
            }

            RegularPrice = msrpPrice;
            var maxGuest = (PublicProduct.MaxGuest <= 0 ? Constant.DefaultMaxGuest : PublicProduct.MaxGuest);
            bool isShowMsrp = false;

            // Upgrade ?
            if (Session[Constant.UpgradeKey] != null)
            {
                int bookingTempId = int.Parse(Session[Constant.UpgradeKey].ToString());
                var bookingsTemps = _productRepository.GetBookingsTempById(bookingTempId).Item1;
                actualPrice = actualPrice - discountPrice - bookingsTemps.HotelPrice;
                if (actualPrice <= 0)
                {
                    actualPrice = 0;
                }
                msrp.Visible = true;
                msrp2.Visible = true;
                isShowMsrp = true;
                AnchorButton.InnerText = "Upgrade Now";
            }
            else
            {
                var discounts = _productRepository.GetAutoPromosByProductId(PublicProduct.ProductId, selectedCheckInDate).FirstOrDefault();
                if (discounts != null)
                {
                    if (PublicCustomerInfos != null)
                    {
                        discounts = _discountRepository.VerifyDiscounts(discounts, PublicCustomerInfos.CustomerId);
                    }

                    if (discounts != null)
                    {
                        actualPrice = Helper.CalculateDiscount(discounts, actualPrice, 1);
                        if (!actualPrice.Equals(msrpPrice))
                        {
                            msrp.Visible = true;
                            msrp2.Visible = true;
                            isShowMsrp = true;
                        }
                    }
                }
            }

            int index = 0;
            if (PublicDiscounts != null && 
                !IsHaveBookingWithSelectedDate && 
                TotalActiveBookingWithSubscription < AppConfiguration.MaxReserveSubscriptionPass &&
                _selectedCheckInDate.Date <= DateTime.UtcNow.AddHours(AppConfiguration.ReserveSubscriptionPassInHours).ToLosAngerlesTimeWithTimeZone(PublicHotel.TimeZoneId).Date &&
                PublicProduct.ProductType == (int)Enums.ProductType.DayPass &&
                TotalBookingInCurrentRecycle < PublicDiscounts.MaxPurchases)
            {
                index = 1;
                if (!RegularPrice.Equals(actualPrice))
                {
                    msrp.Visible = true;
                    msrp2.Visible = true;
                    isShowMsrp = true;
                }
            }

            if (isShowMsrp)
            {
                priceMobile.Attributes["class"] += " has-promo";
                priceDesktop.Attributes["class"] += " has-promo";

                DiscountPrice = actualPrice;
                PerGuestLitMobile.Text = string.Format("<span class=\"price\">{0}</span><span class=\"upto-guest\">{1}</span>", 
                    Helper.FormatPrice(actualPrice * (TotalTickets - index)),
                    Helper.GetStringMaxGuest(maxGuest * TotalTickets));
                PerGuestLit2.Text = string.Format("<div>{0} <span>per guest</span></div>",
                    Helper.FormatPrice(actualPrice / maxGuest));

                TotalLit.Text = string.Format("<div class=\"total-price\">{0} <span>{1}</span></div>",
                    Helper.FormatPrice(actualPrice * (TotalTickets - index)),
                    Helper.GetStringMaxGuest(maxGuest * TotalTickets));

                PerGuestMsrpLitMobile.Text = string.Format("was <i>{0}</i>", Helper.FormatPrice(msrpPrice * TotalTickets));
                PerGuestMsrpLit2.Text = string.Format("was <i>{0}</i>", Helper.FormatPrice(msrpPrice / maxGuest));
            }
            else
            {
                PerGuestLitMobile.Text = string.Format("<span class=\"price\">{0}</span><span class=\"upto-guest\">{1}</span>", 
                    Helper.FormatPrice(actualPrice * (TotalTickets - index)),
                    Helper.GetStringMaxGuest(maxGuest * TotalTickets));
                PerGuestLit2.Text = string.Format("<div>{0} <span>per guest</span></div>",
                    Helper.FormatPrice(actualPrice / maxGuest));

                TotalLit.Text = string.Format("<div class=\"total-price\">{0} <span>{1}</span></div>",
                    Helper.FormatPrice(actualPrice * (TotalTickets - index)),
                    Helper.GetStringMaxGuest(maxGuest * TotalTickets));
                
                PerGuestMsrpLit2.Text = "per guest";
            }

            GuestLit.Text = string.Format("{0} Guests", maxGuest * TotalTickets == 1 ? "" : (maxGuest * TotalTickets).ToString());

            if (selectedCheckInDate.HasValue)
            {
                _searchParams.StartDate = selectedCheckInDate.Value;
                PublicProduct = PublicProduct = _productRepository.GetById(_productId, _searchParams);

                LvRelatedProductsRepeater.DataSource = PublicProduct.RelatedProducts;
                LvRelatedProductsRepeater.DataBind();

                SimilarProductLv.DataSource = PublicProduct.Similarproduct;
                SimilarProductLv.DataBind();
            }
        }

        protected void CheckInDateTextMobile_OnTextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(CheckInDateTextMobile.Text))
            {
                DateTime.TryParse(CheckInDateTextMobile.Text, out _selectedCheckInDate);
                Session["CheckInDateSearch"] = _selectedCheckInDate.ToString(Constant.DiscountDateFormat);
                CheckInDateTextDesktop.Text = _selectedCheckInDate.ToString(Constant.DiscountDateFormat);
                _searchParams.StartDate = _selectedCheckInDate;
                var price = _productRepository.GetById(PublicProduct.ProductId, _searchParams).ActualPriceWithDate;
                BindPrice(price, _selectedCheckInDate);
            }
            else
            {
                Session.Remove("CheckInDateSearch");
                BindPrice();
            }

            BindTicketCapacity();
        }

        protected void CheckInDateTextDesktop_OnTextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(CheckInDateTextDesktop.Text))
            {
                DateTime.TryParse(CheckInDateTextDesktop.Text, out _selectedCheckInDate);
                Session["CheckInDateSearch"] = _selectedCheckInDate.ToString(Constant.DiscountDateFormat);
                CheckInDateTextMobile.Text = _selectedCheckInDate.ToString(Constant.DiscountDateFormat);
                _searchParams.StartDate = _selectedCheckInDate;
                var price = _productRepository.GetById(PublicProduct.ProductId, _searchParams).ActualPriceWithDate;
                BindPrice(price, _selectedCheckInDate);
            }
            else
            {
                Session.Remove("CheckInDateSearch");
                BindPrice();
            }

            BindTicketCapacity();
        }

        protected void TotalTicketsText_OnTextChanged(object sender, EventArgs e)
        {
            Session["TotalTickets"] = TotalTicketsText.Text;
            if (!string.IsNullOrEmpty(CheckInDateTextMobile.Text))
            {
                DateTime.TryParse(CheckInDateTextMobile.Text, out _selectedCheckInDate);
                Session["CheckInDateSearch"] = _selectedCheckInDate.ToString(Constant.DiscountDateFormat);
                _searchParams.StartDate = _selectedCheckInDate;
                var price = _productRepository.GetById(PublicProduct.ProductId, _searchParams).ActualPriceWithDate;
                BindPrice(price);
            }
            else
            {
                Session.Remove("CheckInDateSearch");
                BindPrice();
            }
        }

        protected void LvRelatedProductsRepeater_OnItemDataBound(object sender, ListViewItemEventArgs e)
        {
            //UpToGuestLabel
            if (e.Item.ItemType == ListViewItemType.DataItem)
            {
                var product = (Products)e.Item.DataItem;
                var aTag = (HtmlAnchor)e.Item.FindControl("HotelItem");
                var productPriceLit = (Literal) e.Item.FindControl("ProductPriceLit");

                double actualPrice = product.ActualPriceWithDate.Price;
                var maxGuest = product.MaxGuest <= 0 ? Constant.DefaultMaxGuest : product.MaxGuest;
                if (maxGuest > 1)
                {
                    actualPrice = actualPrice / maxGuest;
                }
                aTag.Attributes.Add("onclick", string.Format("f{0}(event)", product.ProductId));
                productPriceLit.Text = Helper.FormatPrice(actualPrice);
            }
        }

        private void BindTicketCapacity()
        {
            _maxTicket = _productRepository.GetDefaultPassLimit(PublicProduct.ProductId, _selectedCheckInDate);

            TicketCapacity.Controls.Clear();
            TicketCapacityDesktop.Controls.Clear();
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

            for (var i = 1; i <= _maxTicket; i++)
            {
                var liTag = new HtmlGenericControl("li");
                var link = new HtmlAnchor
                {
                    InnerText = string.Format("{0} ticket{1}", i, i == 1 ? string.Empty : "s")
                };
                liTag.Controls.Add(link);
                TicketCapacityDesktop.Controls.Add(liTag);
            }
        }

        #region search Product

        private void BindSurvey()
        {
            _surveys = _surveyRepository.GetByHotelId(PublicProduct.HotelId).ToList();
            if (_surveys.Any())
            {
                ReviewPanel.Visible = true;
                HotelRatingPanel.Update();
            }
            MarketRepeater.DataSource = _surveys.Take(5);
            MarketRepeater.DataBind();
        }

        #endregion

        protected void SimilarProductLv_OnItemDataBound(object sender, ListViewItemEventArgs e)
        {
            if (e.Item.ItemType == ListViewItemType.DataItem)
            {
                var aTag = (HtmlAnchor)e.Item.FindControl("HotelItem");
                var image = (Image)e.Item.FindControl("ProductImage");
                var priceLit = (Label) e.Item.FindControl("PriceLit");
                var product = (Products)e.Item.DataItem;

                image.Attributes["data-src"] = product.CdnImage;
                image.Attributes["src"] = Constant.ImageDefault;

                aTag.Attributes.Add("onclick", string.Format("f{0}(event)", product.ProductId));

                double actualPrice = product.ActualPriceWithDate.Price;
                var maxGuest = product.MaxGuest <= 0 ? Constant.DefaultMaxGuest : product.MaxGuest;
                if (maxGuest > 1)
                {
                    actualPrice = actualPrice / maxGuest;
                }
                priceLit.Text = Helper.FormatPrice(actualPrice);
            }
        }

        protected void AddToWaitListButtonOnClick(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(CheckInDateRequestText.Text))
            {
                ErrorMessageLabel.Text = "Please enter your Check-In Date.";
                return;
            }

            if (string.IsNullOrEmpty(EmailAddressText.Text))
            {
                ErrorMessageLabel.Text = "Please enter your email address.";
                return;
            }

            DateTime checkInDate;
            DateTime.TryParseExact(CheckInDateRequestText.Text, "MMMM dd, yyyy", null,
            DateTimeStyles.None, out checkInDate);

            var item = new ProductWaittingLists
            {
                CheckInDate = checkInDate,
                EmailAddress = EmailAddressText.Text.Trim(),
                ProductId = PublicProduct.ProductId
            };

            _productRepository.AddWaitList(item);
            ScriptManager.RegisterStartupScript(AddToWaitListPanel, 
                AddToWaitListPanel.GetType(), 
                "Update_Not_Available",
                string.Format("$(function(){{ $('#IsWaitingListMHidden').val(true); $('.error-message span').text('Alert has been setup for {0:MMM dd, yyyy}').css({{'color': 'green'}});}});", checkInDate), 
                true);
        }

        private void CheckHaveBookingOnSelectedDate(DateTime date)
        {
            if (PublicDiscounts != null && PublicProduct.ProductType == (int)Enums.ProductType.DayPass)
            {
                // Check have booking with selected date
                var bookings = (from b in _productRepository.BookingList
                                join p in _productRepository.ProductList on b.ProductId equals p.ProductId
                                join h in _productRepository.HotelList on p.HotelId equals h.HotelId
                    join db in _productRepository.DiscountBookingList on b.BookingId equals db.BookingId
                    where b.CustomerId == PublicCustomerInfos.CustomerId &&
                          b.CheckinDate.HasValue &&
                          b.CheckinDate.Value.ToLosAngerlesTimeWithTimeZone(h.TimeZoneId).Date == date.Date &&
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
                    select b);

                TotalActiveBookingWithSubscription = bookingActive.Count();

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
    }
}