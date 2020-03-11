using System;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using AutoMapper;
using DayaxeDal;
using DayaxeDal.Extensions;
using DayaxeDal.Repositories;

namespace dayaxe.com
{
    public partial class Confirm : BasePage
    {
        protected Bookings PublicBooking { get; set; }
        protected Hotels PublicHotels { get; set; }
        protected Products PublicProduct { get; set; }
        protected Discounts PublicDiscountUsed { get; set; }
        protected Markets PublicMarkets { get; set; }
        protected string ProductTypeString { get; set; }
        protected string ProductTypeTrackString { get; set; }

        private readonly BookingRepository _bookingRepository = new BookingRepository();
        private readonly ProductRepository _productRepository = new ProductRepository();
        private readonly CustomerInfoRepository _customerInfoRepository = new CustomerInfoRepository();
        private readonly SubscriptionBookingRepository _subscriptionBookingRepository = new SubscriptionBookingRepository();

        protected void Page_Init(object sender, EventArgs e)
        {
            try
            {
                if (Page.RouteData.Values["bookingId"] != null)
                {
                    int bookingId;
                    if (int.TryParse(Page.RouteData.Values["bookingId"].ToString(), out bookingId))
                    {
                        PublicBooking = _bookingRepository.GetById(bookingId);
                        PublicHotels = _bookingRepository.GetHotelsByBookingId(bookingId);

                        if (PublicBooking != null && PublicHotels != null)
                        {
                            PublicMarkets = _productRepository.GetMarketsByHotelId(PublicHotels.HotelId);

                            PublicDiscountUsed = _bookingRepository.GetDiscountsByBookingId(bookingId);
                            if (PublicDiscountUsed != null)
                            {
                                PromoAppliedRow.Visible = true;
                                PromoAppliedSeperateRow.Visible = true;
                                DiscountCodeLit.Text = string.Format("{0} OFF",
                                    PublicDiscountUsed.PromoType == (int)Enums.PromoType.Fixed
                                        ? Helper.FormatPriceWithFixed(PublicDiscountUsed.PercentOff)
                                        : PublicDiscountUsed.PercentOff + "%");
                            }

                            var checkInDate = DateTime.UtcNow.ToLosAngerlesTimeWithTimeZone(PublicHotels.TimeZoneId).Date;
                            if (PublicBooking.CheckinDate.HasValue)
                            {
                                checkInDate = PublicBooking.CheckinDate.Value.ToLosAngerlesTimeWithTimeZone(PublicHotels.TimeZoneId).Date;
                            }

                            PublicProduct = _productRepository.GetById(PublicBooking.ProductId, checkInDate);

                            if (PublicProduct != null)
                                ReBindConfirmInfo();

                            var promoPrice = PublicBooking.MerchantPrice * PublicBooking.Quantity + PublicBooking.TaxPrice - PublicBooking.TotalPrice;

                            TicketFirstLabel.Text = PublicBooking.Quantity.ToString();
                            TicketLabel.Text = PublicBooking.Quantity.ToString();
                            if (promoPrice.Equals(0))
                            {
                                promoRow.Visible = false;
                            }
                            PromoPrice.Text = string.Format("-{0}", Helper.FormatPriceWithFixed(promoPrice));

                            if (PublicBooking.PayByCredit.Equals(0))
                            {
                                creditRow.Visible = false;
                            }
                            CreditPrice.Text = Helper.FormatPriceWithFixed(PublicBooking.PayByCredit * -1);
                            PerPriceLabel.Text = Helper.FormatPriceWithFixed(PublicBooking.MerchantPrice);
                            var totalPrice = PublicBooking.TotalPrice - PublicBooking.PayByCredit;
                            TotalPriceLit.Text = Helper.FormatPriceWithFixed(totalPrice);

                            var quantity = PublicBooking.IsActiveSubscription
                                ? PublicBooking.Quantity - 1
                                : PublicBooking.Quantity;

                            TicketNumberLit.Text = string.Format("{0} x {1}",
                                Helper.FormatPriceWithFixed(PublicBooking.MerchantPrice),
                                quantity == 1 ? "1 ticket" : quantity + " tickets");
                            TicketPriceLit.Text = Helper.FormatPriceWithFixed(PublicBooking.MerchantPrice * quantity);

                            if (!PublicBooking.TaxPrice.Equals(0))
                            {
                                taxRow.Visible = true;
                                TaxLit.Text = Helper.FormatPriceWithFixed(PublicBooking.TaxPrice);
                            }

                            var goldPassDiscount = _bookingRepository.GetGoldPassDiscountByBookingId(bookingId);
                            if (goldPassDiscount != null)
                            {
                                reminderRow.Visible = true;
                            }
                        }
                    }
                }

                if (PublicCustomerInfos == null ||
                    PublicBooking != null &&
                    PublicCustomerInfos.CustomerId != PublicBooking.CustomerId)
                {
                    Response.Redirect(string.Format(Constant.SignIpPage, HttpUtility.UrlEncode(Request.Url.PathAndQuery)));
                }

                if (PublicBooking == null)
                {
                    if (PublicCustomerInfos != null && !string.IsNullOrEmpty(PublicCustomerInfos.BrowsePassUrl))
                    {
                        Response.Redirect(PublicCustomerInfos.BrowsePassUrl);
                    }
                    Response.Redirect(!string.IsNullOrEmpty((string)Session["SearchPage"])
                        ? Session["SearchPage"].ToString()
                        : Constant.SearchPageDefault);
                }

                if (PublicCustomerInfos == null || string.IsNullOrEmpty(PublicCustomerInfos.EmailAddress))
                {
                    PublicCustomerInfos = _customerInfoRepository.GetById(PublicBooking.CustomerId);
                }
                if (!PublicCustomerInfos.IsConfirmed)
                {
                    PasswordErrorMessage.Visible = false;
                    CustomerMV.ActiveViewIndex = 0;
                }

                var maxGuest = PublicProduct.MaxGuest <= 0 ? Constant.DefaultMaxGuest : PublicProduct.MaxGuest;
                MaxGuestLabel.Text = string.Format(Constant.MaxGuestText,
                    maxGuest,
                    maxGuest > 1 ? "s" : string.Empty,
                    PublicProduct.ProductTypeString,
                    PublicProduct.KidAllowedString
                );

                if (PublicProduct.IsCheckedInRequired)
                {
                    FinePrintRow.Visible = false;
                    FinePrintSeperateRow.Visible = false;
                }

                if (PublicBooking.CheckinDate.HasValue)
                {
                    CheckInDateLabel.Text = PublicBooking.CheckinDate.Value.ToString(Constant.DateFormat);
                    CheckInDateButtonLabel.Text = "Change";
                    Session["CheckInDateSearch"] = PublicBooking.CheckinDate.Value.ToLosAngerlesTimeWithTimeZone(PublicHotels.TimeZoneId).Date
                        .ToString("MM/dd/yyyy");
                }
                else
                {
                    CheckInDateLabel.CssClass += " not-selected";
                }

                if (PublicProduct.AddOnsproduct.Any())
                {
                    AddOnPanel.Visible = true;
                    AddOnsListView.DataSource = PublicProduct.AddOnsproduct;
                    AddOnsListView.DataBind();

                    const string mixpanelscript = "MixpanelScript";
                    string strScript = string.Empty;
                    PublicProduct.AddOnsproduct.ToList().ForEach(item =>
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

                ProductInfoLit.Text = GetProductInfo();

                AssignPassString();

                bool showGoldPassConfirmation;
                if (Session["ShowGoldPassConfirmation"] != null &&
                    bool.TryParse(Session["ShowGoldPassConfirmation"].ToString(), out showGoldPassConfirmation) &&
                    showGoldPassConfirmation)
                {
                    btnViewSubscriptionTop.Visible = true;
                    btnViewSubscriptionBottom.Visible = true;
                }
            }
            catch (Exception ex)
            {
                var logs = new Logs
                {
                    LogKey = "Confirm-Error-On-Init",
                    UpdatedDate = DateTime.UtcNow,
                    UpdatedContent = string.Format("Confirm Error - {0} - {1} - {2} - {3}", ex.Message, ex.StackTrace, ex.Source, ex.InnerException),
                    UpdatedBy = PublicCustomerInfos != null ? PublicCustomerInfos.CustomerId : 0
                };
                _bookingRepository.AddLog(logs);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected override void OnUnload(EventArgs e)
        {
            base.OnUnload(e);
            Session["IsRegister"] = null;
        }

        protected void HtmlAnchor_Click(object sender, EventArgs e)
        {
            Response.Redirect(string.Format("/{0}/ViewDayPass.aspx", Page.RouteData.Values["bookingId"]));
        }

        protected void ConfirmButtonClick(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(Password.Text))
            {
                PasswordErrorMessage.Visible = true;
                PasswordErrorMessage.Text = ErrorMessage.EnterPassword;
                return;
            }

            if (Password.Text.Length < 7)
            {
                PasswordErrorMessage.Visible = true;
                PasswordErrorMessage.Text = ErrorMessage.PasswordNotValid;
                return;
            }

            if (Password.Text != PasswordConfirm.Text)
            {
                PasswordErrorMessage.Visible = true;
                PasswordErrorMessage.Text = ErrorMessage.ConfirmPasswordNotValid;
                return;
            }

            var customerInfo = Mapper.Map<CustomerInfos>(PublicCustomerInfos);
            if (customerInfo != null)
            {
                customerInfo.Password = Password.Text;
                customerInfo.IsConfirmed = true;
                _customerInfoRepository.Update(customerInfo);

                CustomerMV.ActiveViewIndex = 1;
            }
        }

        private void ReBindConfirmInfo()
        {
            imageProduct.Src = string.Format("{0}",
                        new Uri(new Uri(AppConfiguration.CdnImageUrlDefault), PublicProduct.ImageUrl).AbsoluteUri);
            ProductNameLit.Text = string.Format("{0} at {1}", PublicProduct.ProductName, PublicHotels.HotelName);
        }

        private string GetProductInfo()
        {
            return string.Format("{0} at {1}",
                PublicProduct.ProductName,
                PublicHotels.HotelName);
        }

        private void AssignPassString()
        {
            switch (PublicProduct.ProductType)
            {
                case (int)Enums.ProductType.Cabana:
                    ProductTypeString = "CABANA";
                    break;
                case (int)Enums.ProductType.DayPass:
                    ProductTypeString = "PASS";
                    break;
                case (int)Enums.ProductType.Daybed:
                    ProductTypeString = "DAYBED";
                    break;
                case (int)Enums.ProductType.SpaPass:
                    ProductTypeString = "PASS";
                    break;
            }

            ProductTypeTrackString = Helper.GetStringPassByProductType(PublicProduct.ProductType);
        }

        protected void AddOnsListView_OnItemDataBound(object sender, ListViewItemEventArgs e)
        {
            if (e.Item.ItemType == ListViewItemType.DataItem)
            {
                var aTag = (HtmlAnchor)e.Item.FindControl("HotelItem");
                var image = (Image)e.Item.FindControl("ProductImage");
                var priceLit = (Label)e.Item.FindControl("PriceLit");
                var maxGuest = (Label)e.Item.FindControl("MaxGuest");
                var product = (Products)e.Item.DataItem;

                image.Attributes["data-original"] = product.CdnImage;
                image.Attributes["src"] = Constant.ImageDefault;

                aTag.Attributes.Add("onclick", string.Format("f{0}(event)", product.ProductId));

                priceLit.Text = Helper.FormatPriceWithFixed(product.ActualPriceWithDate.Price);

                maxGuest.Text = Helper.GetStringMaxGuest(product.MaxGuest);
            }
        }

        protected void btnViewSubscription_Click(object sender, EventArgs e)
        {
            int subscriptionId = AppConfiguration.SubscriptionUpgradeId;
            var upgradeSubscription = _bookingRepository.SubscriptionsList.FirstOrDefault(s => s.Id == subscriptionId && s.IsActive);

            SubscriptionBookings subscriptionBookings = _subscriptionBookingRepository.GetByCustomerId(PublicCustomerInfos.CustomerId, PublicDiscounts.Id);

            if (subscriptionBookings != null)
            {
                var subscriptionDetailUrl = string.Format(Constant.SubscriptionConfirmPage,
                            upgradeSubscription.StripePlanId,
                            subscriptionBookings.Id,
                            "/" + Page.RouteData.Values["bookingId"]);
                Response.Redirect(subscriptionDetailUrl, false);
            }
        }
    }
}