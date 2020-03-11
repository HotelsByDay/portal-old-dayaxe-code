using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using dayaxe.com.Controls;
//using dayaxe.com.LandingTemplate;
using DayaxeDal;
using DayaxeDal.Extensions;
using DayaxeDal.Repositories;
using Newtonsoft.Json;
using Stripe;

namespace dayaxe.com
{
    public partial class Default : BasePage
    {
        private readonly DalHelper _helper = new DalHelper();
        private readonly BookingRepository _bookingRepository = new BookingRepository();
        private readonly ProductRepository _productRepository = new ProductRepository();
        private HtmlContents _content;
        protected bool ShowAuth;
        private string StripeCardString { get; set; }
        //private bool IsHome { get; set; }

        protected void Page_PreInit(object sender, EventArgs e)
        {
            if (!Request.IsLocal && !Request.IsSecureConnection)
            {
                string redirectUrl = Request.Url.ToString().Replace("http:", "https:");
                Response.Redirect(redirectUrl, false);
                HttpContext.Current.ApplicationInstance.CompleteRequest();
            }

            if (Page.RouteData.Values["urlSegment"] != null)
            {
                var urlSegment = (string)Page.RouteData.Values["urlSegment"];
                switch (urlSegment)
                {
                    // case "membership":
                    // case "credits":
                    case "my-account":
                    // case "reviews":
                        //
                        MasterPageFile = "~/Client.master";
                        break;
                }
            }
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            if (Master == null)
            {
                throw new HttpException(404, ErrorMessage.MasterNotFound);
            }
            HtmlGenericControl body = (HtmlGenericControl)Master.FindControl("body");

            if (Page.RouteData.Values["urlSegment"] != null)
            {
                var urlSegment = (string)Page.RouteData.Values["urlSegment"];
                bool isInvalidTicket = false;

                switch (urlSegment)
                {
                    case "signup":
                    case "signin":
                        var signupControl = (AuthControlWithoutPopup)LoadControl("~/Controls/AuthControlWithoutPopup.ascx");
                        var multiView = (MultiView)ControlExtensions.FindControlRecursive(signupControl, "AuthMultiView");
                        if (multiView != null)
                        {
                            if (urlSegment == "signup")
                            {
                                multiView.ActiveViewIndex = 0;
                                TitleLiteral.Text = "Sign Up";
                            }
                            else
                            {
                                if (PublicCustomerInfos != null && Request.Params["ReturnUrl"] != null)
                                {
                                    if (Request.RawUrl.Contains(Constant.ConfirmPageProduct))
                                    {
                                        var ticketControl = (InvalidTicket) LoadControl("~/Controls/InvalidTicket.ascx");
                                        ContentPlaceHolder.Controls.Add(ticketControl);
                                        isInvalidTicket = true;
                                    }
                                    else
                                    {
                                        Response.Redirect(HttpUtility.UrlDecode(Request.Params["ReturnUrl"]));
                                    }
                                }
                                TitleLiteral.Text = "Sign In";
                                multiView.ActiveViewIndex = 1;
                            }

                            if (Request.Params["sp"] != null &&
                                (Session["UserSession"] == null || (Session["UserSession"] != null && string.Equals(Request.Params["sp"], (string)Session["UserSession"], StringComparison.OrdinalIgnoreCase))))
                            {
                                ShowAuth = true;
                                multiView.ActiveViewIndex = 3;
                            }
                        }
                        AuthControl.Visible = false;
                        // Sign in but do not have permission
                        if (!isInvalidTicket)
                        {
                            ContentPlaceHolder.Controls.Add(signupControl);
                        }
                        break;
                    case "invalid-ticket":
                        var invalidTicketControl = (InvalidTicket)LoadControl("~/Controls/InvalidTicket.ascx");
                        ContentPlaceHolder.Controls.Add(invalidTicketControl);

                        AuthControl.Visible = false;
                        break;
                    case "my-account":
                        body.Attributes["class"] += " my-account-page";
                        // Users Do not log in
                        if (PublicCustomerInfos == null)
                        {
                            var loginLinkButton = (HtmlAnchor)ControlExtensions.FindControlRecursive(Master, "LoginLinkButton");
                            loginLinkButton.Visible = false;
                            Response.Redirect(string.Format(Constant.SignIpPage, HttpUtility.UrlEncode(Request.Url.PathAndQuery)));
                        }

                        var myAccountControl = (MyAccount) LoadControl("~/Controls/MyAccount.ascx");
                        ContentPlaceHolder.Controls.Add(myAccountControl);

                        AuthControl.Visible = false;
                        footer.Visible = false;
                        NewsletterControl.Visible = false;
                        break;
                    default:
                        if (urlSegment == "my-day-passes")
                        {
                            if (PublicCustomerInfos == null)
                            {
                                Response.Redirect(string.Format(Constant.SignIpPage, HttpUtility.UrlEncode(Request.Url.PathAndQuery)));
                            }
                        }
                        _content = _helper.GetHtmlContentsByUrlSegment(urlSegment);

                        if (_content == null)
                        {
                            throw new HttpException(404, ErrorMessage.NotFound);
                        }
                        break;
                }
            }
            else
            {
                _content = _helper.GetDefaultHtmlContents();
                //var homeControl = (HomePageContent)LoadControl("~/LandingTemplate/homepage.ascx");
                //ContentPlaceHolder.Controls.Add(homeControl);
                //IsHome = true;
                body.Attributes["class"] += " home-page";
            }

            if (_content != null)
            {
                body.Attributes["class"] += string.Format(" {0} {1}", Page.RouteData.Values["urlSegment"], _content.BodyClass);
                BindHeaderInfo();

                BindContentInfo();

                BindMyDayPassInfo();

                BindImageLanding();

                BindHomeProductFeatured();
            }
        }

        private void BindHomeProductFeatured()
        {
            var productFeaturedPlaceHolder =
                ControlExtensions.FindControlRecursive(ContentPlaceHolder, "FeaturedProductPlaceHolder") as PlaceHolder;

            var str = "<asp:Repeater ID=\"LvProductRepeater\" runat=\"server\" Visible=\"False\">" +
                      "      <HeaderTemplate>" +
                      "          <div id=\"owl-product-featured\" class=\"owl-carousel owl-theme\">" +
                      "      </HeaderTemplate>" +
                      "      <ItemTemplate>" +
                      "          <a id=\"ProductItem\" runat=\"server\">" +
                      "              <div class=\"product-item\">" +
                      "                  <asp:Image ID=\"ProductImage\" CssClass=\"lazyOwl img-responsive\" AlternateText=\"\" runat=\"server\" />" +
                      "                  <div class=\"product-info\">" +
                      "                      <div class=\"type\">" +
                      "                          <asp:Literal runat=\"server\" ID=\"ProductNameLit\"></asp:Literal>" +
                      "                      </div>" +
                      "                      <div class=\"hotel-name\">" +
                      "                          <asp:Literal runat=\"server\" ID=\"HotelNameLit\"></asp:Literal>" +
                      "                      </div>" +
                      "                      <div class=\"max-guest\">" +
                      "                          <asp:Label CssClass=\"price\" runat=\"server\" ID=\"PriceLit\"></asp:Label> per guest" +
                      "                      </div>" +
                      "                      <div class=\"rating\">" +
                      "                          <asp:Literal runat=\"server\" ID=\"RatingLit\"></asp:Literal>" +
                      "                          <span>" +
                      "                              <asp:Literal runat=\"server\" ID=\"TotalReviewsLit\"></asp:Literal>" +
                      "                          </span>" +
                      "                      </div>" +
                      "                  </div>" +
                      "              </div>" +
                      "          </a>" +
                      "      </ItemTemplate>" +
                      "  </asp:Repeater>";
            if (productFeaturedPlaceHolder != null)
            {
                var strControl = Page.ParseControl(str);
                if (strControl != null)
                {
                    productFeaturedPlaceHolder.Controls.Add(strControl);

                    var productReater = ControlExtensions.FindControlRecursive(ContentPlaceHolder, "LvProductRepeater") as Repeater;
                    if (productReater != null)
                    {
                        var products = _productRepository.GetFeaturedProducts();
                        if (products.Any())
                        {
                            const string mixpanelscript = "MixpanelScript";
                            string strScript = string.Empty;
                            products.ForEach(item =>
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

                            productReater.ItemDataBound += ProductReaterOnItemDataBound;
                            productReater.DataSource = products;
                            productReater.Visible = true;
                            productReater.DataBind();
                        }
                    }
                }
            }
        }

        private void ProductReaterOnItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var aTag = (HtmlAnchor)e.Item.FindControl("ProductItem");
                var image = (Image)e.Item.FindControl("ProductImage");
                var priceLit = (Label)e.Item.FindControl("PriceLit");
                var productNameLit = (Literal) e.Item.FindControl("ProductNameLit");
                var hotelNameLit = (Literal) e.Item.FindControl("HotelNameLit");
                var ratingLit = (Literal) e.Item.FindControl("RatingLit");
                var totalReviewsLit = (Literal) e.Item.FindControl("TotalReviewsLit");
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

                productNameLit.Text = product.ProductName;
                hotelNameLit.Text = product.HotelName;
                ratingLit.Text = product.Hotels.CustomerRating > 0 ? product.Hotels.CustomerRatingString : product.Hotels.Rating;
                totalReviewsLit.Text = product.Hotels.TotalReviews;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
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
                && Session["UserSession"] == null && multiView != null)
            {
                multiView.ActiveViewIndex = 0;
                ShowAuth = true;
            }
        }

        private void BindHeaderInfo()
        {
            TitleLiteral.Text = _content.Title;
            string strMeta = string.Format("<meta name=\"description\" content=\"{0}\" />"
                                           + "<meta name=\"keywords\" content=\"{1}\" />"
                                           + "<link rel=\"canonical\" href=\"{2}/{3}\" />",
                _content.MetaDescription,
                _content.MetaKeyword,
                AppConfiguration.ClientUrlDefault,
                Page.RouteData.Values["urlSegment"]);

            var control = Page.ParseControl(strMeta);
            if (control != null)
            {
                MetaPlaceHolder.Controls.Add(control);
            }
        }

        private void BindContentInfo()
        {
            Control control;
            if (!string.IsNullOrEmpty(_content.ScriptAnalyticsHeader))
            {
                control = Page.ParseControl(_content.ScriptAnalyticsHeader);
                if (control != null)
                {
                    ScriptAnalyticsHeaderPlaceHolder.Controls.Add(control);
                }
            }

            if (!string.IsNullOrEmpty(_content.Data))
            {
                control = Page.ParseControl(_content.Data);
                if (control != null)
                {
                    ContentPlaceHolder.Controls.Add(control);
                }
            }
        }

        private void BindMyDayPassInfo()
        {
            var daypassRepeater = ControlExtensions.FindControlRecursive(ContentPlaceHolder, "DayPassRepeater") as Repeater;
            if (daypassRepeater != null)
            {
                if (Session["UserSession"] == null)
                {
                    throw new HttpException(401, "UnAuthorized");
                }

                CustomerInfos customerInfos = _bookingRepository.GetCustomerInfoBySessionId((string)Session["UserSession"]);
                if (customerInfos == null)
                {
                    throw new HttpException(401, "UnAuthorized");
                }

                daypassRepeater.ItemDataBound += DaypassRepeaterOnItemDataBound;
                var daypasses = _bookingRepository.GetBookingsByCustomerId(customerInfos.CustomerId);
                daypassRepeater.DataSource = daypasses;
                daypassRepeater.DataBind();
            }
        }

        private void DaypassRepeaterOnItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
            {
                var bookings = (Bookings)e.Item.DataItem;
                var linkButton = (HyperLink)e.Item.FindControl("DayPassLinkButton");
                var submitSurvey = (HyperLink) e.Item.FindControl("SubmitSurveyLinkButton");
                var cancelLinkButton = (LinkButton) e.Item.FindControl("CancelLinkButton");
                var products = _bookingRepository.GetProduct(bookings.ProductId);
                var hotel = _bookingRepository.GetHotel(products.HotelId);
                if (linkButton != null && bookings != null)
                {
                    linkButton.NavigateUrl = string.Format("/{0}/ViewDayPass.aspx", bookings.BookingId);
                    linkButton.Text = string.Format("{0}, {1}, {2}, {3} - {4}, {5}, {6} - {7}",
                        hotel.HotelName,
                        products.ProductName,
                        hotel.Neighborhood,
                        hotel.City,
                        (bookings.CheckinDate.HasValue ? bookings.CheckinDate.Value : bookings.BookedDate).ToLosAngerlesTimeWithTimeZone(hotel.TimeZoneId).ToString(Constant.DiscountDateFormat),
                        bookings.Quantity,
                        Helper.FormatPrice(bookings.TotalPrice),
                        bookings.BookingStatusString);
                }

                if (submitSurvey != null && bookings != null)
                {
                    if (bookings.PassStatus == (int) Enums.PassStatus.Redeemed)
                    {
                        try
                        {
                            var url = Helper.GetUrlSendSurvey(bookings);
                            submitSurvey.NavigateUrl = url;
                        }
                        catch (Exception ex)
                        {
                             var str = ex.Message;
                        }
                    }
                    else
                    {
                        submitSurvey.Visible = false;
                    }
                }

                if (cancelLinkButton != null && bookings != null)
                {
                    if (bookings.PassStatus == (int) Enums.BookingStatus.Active)
                    {
                        var dateNow = DateTime.UtcNow.ToLosAngerlesTimeWithTimeZone(hotel.TimeZoneId).AddDays(2).AddHours(9);
                        var bookDate = bookings.BookedDate.ToLosAngerlesTimeWithTimeZone(hotel.TimeZoneId).AddDays(14);
                        if ((products.IsCheckedInRequired && bookings.CheckinDate > dateNow) 
                            || (!products.IsCheckedInRequired && bookDate.Date > DateTime.UtcNow.ToLosAngerlesTimeWithTimeZone(hotel.TimeZoneId).Date))
                        {
                            cancelLinkButton.Command += CancelBookingClick;
                            cancelLinkButton.CommandArgument = bookings.BookingId.ToString();
                            cancelLinkButton.Visible = true;
                        }
                    }
                }
            }
        }

        private void BindImageLanding()
        {
            var imageDesktop = ControlExtensions.FindControlRecursive(ContentPlaceHolder, "ImageLandingDesktop") as Image;
            if (imageDesktop != null)
            {
                imageDesktop.Visible = true;
                imageDesktop.ImageUrl = new Uri(new Uri(AppConfiguration.CdnImageUrlDefault), _content.ImageLandingDesktop).AbsoluteUri;
                imageDesktop.CssClass += " img-background";
            }
            var imageMobile = ControlExtensions.FindControlRecursive(ContentPlaceHolder, "ImageLandingMobile") as Image;
            if (imageMobile != null)
            {
                imageMobile.Visible = true;
                imageMobile.ImageUrl = new Uri(new Uri(AppConfiguration.CdnImageUrlDefault), _content.ImageLandingMobile).AbsoluteUri;
                imageMobile.CssClass += " img-background";
            }
        }

        private void CancelBookingClick(object sender, CommandEventArgs e)
        {
            var bookingId = int.Parse(e.CommandArgument.ToString());
            var bookings = _bookingRepository.GetById(bookingId);
            var products = _bookingRepository.ProductList.First(x => x.ProductId == bookings.ProductId);
            var hotels = _bookingRepository.HotelList.First(x => x.HotelId == products.HotelId);
            var market = (from mh in _bookingRepository.MarketHotelList
                join m in _bookingRepository.MarketList on mh.MarketId equals m.Id
                where mh.HotelId == hotels.HotelId
                select m).FirstOrDefault();

            var dateNow = DateTime.UtcNow.ToLosAngerlesTimeWithTimeZone(hotels.TimeZoneId).AddDays(2).AddHours(9);
            var bookDate = bookings.BookedDate.ToLosAngerlesTimeWithTimeZone(hotels.TimeZoneId).AddDays(14);
            if ((products.IsCheckedInRequired && bookings.CheckinDate <= dateNow) 
                || (!products.IsCheckedInRequired && bookDate.Date < DateTime.UtcNow.ToLosAngerlesTimeWithTimeZone(hotels.TimeZoneId).Date))
            {
                ClientScript.RegisterStartupScript(GetType(), "Cancel_Not_Available", "$(function(){$('#updateNotPossible').modal('show');});", true);
                return;
            }

            if (!string.IsNullOrEmpty(bookings.StripeChargeId))
            {
                MaintainOldInvoices(bookings);
            }

            var logs = new CustomerCreditLogs();
            double payByCard = bookings.TotalPrice - bookings.PayByCredit;
            if (bookings.PayByCredit > 0)
            {
                logs = new CustomerCreditLogs
                {
                    CustomerId = bookings.CustomerId,
                    ProductId = bookings.ProductId,
                    Description = string.Format("{0} – {1} – {2} – {3}",
                        products.ProductName,
                        hotels.HotelName,
                        market != null ? market.LocationName : "",
                        bookings.BookingIdString),
                    Amount = Math.Abs(bookings.PayByCredit),
                    CreatedBy = PublicCustomerInfos != null ? PublicCustomerInfos.CustomerId : 0,
                    CreatedDate = DateTime.UtcNow,
                    CreditType = payByCard >= 0 ? (byte)Enums.CreditType.FullPurchaseRefund : (byte)Enums.CreditType.PartialPuchaseRefund,
                    ReferralId = 0,
                    BookingId = bookings.BookingId,
                    Status = true,
                    GiftCardId = 0
                };
                bookings.PaymentType = (byte)Enums.PaymentType.DayAxeCredit;
                bookings.RefundCreditAmount = bookings.PayByCredit;
            }

            if (payByCard > 0)
            {
                Refund(bookings, PublicCustomerInfos, payByCard, bookings.MerchantPrice, bookings.HotelPrice);

                bookings.PaymentType = (byte)Enums.PaymentType.Stripe;
                var cardService = new StripeCardService();
                StripeCard stripeCard = cardService.Get(PublicCustomerInfos.StripeCustomerId, PublicCustomerInfos.StripeCardId);
                StripeCardString = string.Format("{0} {1}", stripeCard.Brand, stripeCard.Last4);
                bookings.StripeRefundAmount = payByCard;
            }

            bookings.TotalRefundAmount = bookings.TotalPrice;
            bookings.StripeCardString = StripeCardString;
            bookings.CancelDated = DateTime.UtcNow;
            bookings.PassStatus = (int) Enums.BookingStatus.Refunded;
            _bookingRepository.RefundBooking(bookings, logs);
            CacheLayer.Clear(CacheKeys.CustomerCreditsCacheKey);
            CacheLayer.Clear(CacheKeys.CustomerCreditLogsCacheKey);

            BindMyDayPassInfo();
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

        private void MaintainOldInvoices(Bookings booking)
        {
            var chargeInvoice = new Invoices
            {
                BookingId = booking.BookingId,
                PassStatus = booking.PassStatus,
                StripeChargeId = booking.StripeChargeId,
                ChargeAmount = booking.TotalPrice,
                CreatedDate = DateTime.UtcNow,
                CreatedBy = PublicCustomerInfos != null ?  PublicCustomerInfos.CustomerId : 1,
                HotelPrice = booking.HotelPrice,
                InvoiceStatus = (int)Enums.InvoiceStatus.Charge,
                MerchantPrice = booking.MerchantPrice,
                Quantity = booking.Quantity,
                RefundAmount = 0,
                StripeRefundId = string.Empty,
                StripeRefundStatus = string.Empty,
                TotalPrice = booking.TotalPrice
            };
            _bookingRepository.AddInvoices(chargeInvoice, string.Empty);

            if (!string.IsNullOrEmpty(booking.StripeRefundTransactionId))
            {
                double refundAmount = booking.StripeRefundAmount.HasValue
                    ? booking.StripeRefundAmount.Value
                    : 0;
                var refundInvoice = new Invoices
                {
                    BookingId = booking.BookingId,
                    PassStatus = booking.PassStatus,
                    StripeChargeId = string.Empty,
                    ChargeAmount = 0,
                    CreatedDate = DateTime.UtcNow,
                    CreatedBy = PublicCustomerInfos != null ? PublicCustomerInfos.CustomerId : 1,
                    HotelPrice = booking.HotelPrice,
                    InvoiceStatus = refundAmount >= booking.TotalPrice * 100 ? (int)Enums.InvoiceStatus.FullRefund : (int)Enums.InvoiceStatus.PartialRefund,
                    MerchantPrice = booking.MerchantPrice,
                    Quantity = booking.Quantity,
                    RefundAmount = (double)refundAmount / 100,
                    StripeRefundId = booking.StripeRefundTransactionId,
                    StripeRefundStatus = booking.StripeRefundStatus,
                    TotalPrice = booking.TotalPrice
                };
                _bookingRepository.AddInvoices(refundInvoice, string.Empty);
            }

            booking.StripeChargeId = string.Empty;
            booking.StripeRefundAmount = 0;
            booking.StripeRefundStatus = string.Empty;
            booking.StripeRefundTransactionId = string.Empty;
            booking.HasInvoice = true;
            _bookingRepository.UpdateStatus(new List<Bookings> { booking });
        }


        #region Bookings Details Function

        private void Refund(Bookings booking, CustomerInfos customerInfos, double diffPrice, double merchantPrice, double actualPrice)
        {
            var bookingInvoices = _bookingRepository.GetInvoicesByBookingId(booking.BookingId).ToList();
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

                    var oldRefund = refundOfCurrentCharge.Sum(x => x.RefundAmount);
                    var newDiffPrice = currentCharge.ChargeAmount - oldRefund;

                    if (newDiffPrice > 0)
                    {
                        double newRefundPrice = diffPrice;
                        diffPrice -= newDiffPrice;
                        var chargeInvoice = new Invoices
                        {
                            BookingId = booking.BookingId,
                            PassStatus = booking.PassStatus,
                            StripeChargeId = currentCharge.StripeChargeId,
                            ChargeAmount = currentCharge.ChargeAmount,
                            CreatedDate = DateTime.UtcNow,
                            CreatedBy = customerInfos != null ? customerInfos.CustomerId : 0,
                            HotelPrice = actualPrice,
                            MerchantPrice = merchantPrice,
                            Quantity = booking.Quantity,
                            TotalPrice = booking.TotalPrice
                        };

                        if (diffPrice >= 0)
                        {
                            newRefundPrice = newDiffPrice;
                            chargeInvoice.InvoiceStatus = (int)Enums.InvoiceStatus.FullRefund;
                        }
                        else
                        {
                            chargeInvoice.InvoiceStatus = (int)Enums.InvoiceStatus.PartialRefund;
                        }
                        var refund = CallStripeRefund(currentCharge.StripeChargeId, newRefundPrice);
                        chargeInvoice.RefundAmount = (double)refund.Amount / 100;
                        chargeInvoice.StripeRefundId = refund.Id;
                        chargeInvoice.StripeRefundStatus = refund.Status;

                        string requestObj = JsonConvert.SerializeObject(refund, CustomSettings.SerializerSettings());
                        string message = string.Format("{0} Refund Response: {1}, Refund Post Data: {2}",
                            booking.BookingCode,
                            refund.StripeResponse.ResponseJson,
                            requestObj);
                        _bookingRepository.AddInvoices(chargeInvoice, message);
                    }
                    chargeInvoices.Remove(currentCharge);
                }
            }
        }

        #endregion  
    }
}