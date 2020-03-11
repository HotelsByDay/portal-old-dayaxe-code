using System;
using System.Linq;
using System.Net;
using System.Web;
using AutoMapper;
using DayaxeDal;
using DayaxeDal.Repositories;

namespace dayaxe.com.Subscriptions
{
    public partial class ConfirmSubscriptionPage : BasePage
    {
        protected SubscriptionBookings PublicSubscriptionBooking { get; set; }
        protected SubscriptionCycles PublicSubscriptionCycles { get; set; }
        protected Discounts PublicDiscountUsed { get; set; }
        protected DayaxeDal.Subscriptions PublicSubscriptions { get; set; }
        protected const string ProductTypeTrackString = "Subscription";
        private Products PublicProducts { get; set; }

        private readonly CustomerInfoRepository _customerInfoRepository = new CustomerInfoRepository();
        private readonly SubscriptionBookingRepository _subscriptionBookingRepository = new SubscriptionBookingRepository();
        private readonly SubscriptionRepository _subscriptionRepository = new SubscriptionRepository();


        protected void Page_Init(object sender, EventArgs e)
        {
            try
            {
                if (Page.RouteData.Values["SubscriptionBookingId"] != null)
                {
                    int subBookingId;
                    if (int.TryParse(Page.RouteData.Values["SubscriptionBookingId"].ToString(), out subBookingId))
                    {
                        PublicSubscriptionBooking = _subscriptionBookingRepository.GetById(subBookingId);
                        PublicSubscriptionCycles = _subscriptionBookingRepository.SubscriptionCyclesList
                            .Where(sc => sc.SubscriptionBookingId == subBookingId)
                            .OrderByDescending(sc => sc.CycleNumber)
                            .FirstOrDefault();
                        PublicSubscriptions = _subscriptionRepository.GetById(PublicSubscriptionBooking.SubscriptionId);
                        PublicDiscountUsed = _subscriptionRepository.GetDiscountsBySubscriptionBookingId(subBookingId);
                        ReBindConfirmInfo();

                        if (PublicDiscountUsed != null)
                        {
                            PromoAppliedRow.Visible = true;
                            PromoAppliedSeperateRow.Visible = true;
                            DiscountCodeLit.Text = string.Format("{0} OFF",
                                PublicDiscountUsed.PromoType == (int)Enums.PromoType.Fixed
                                    ? Helper.FormatPrice(PublicDiscountUsed.PercentOff)
                                    : PublicDiscountUsed.PercentOff + "%");
                        }

                        var promoPrice = PublicSubscriptionCycles.MerchantPrice * PublicSubscriptionBooking.Quantity - PublicSubscriptionCycles.TotalPrice - PublicSubscriptionCycles.PayByCredit;

                        if (promoPrice.Equals(0))
                        {
                            promoRow.Visible = false;
                        }
                        PromoPrice.Text = Helper.FormatPrice(promoPrice);

                        if (PublicSubscriptionCycles.PayByCredit.Equals(0))
                        {
                            creditRow.Visible = false;
                        }
                        CreditPrice.Text = Helper.FormatPrice(PublicSubscriptionCycles.PayByCredit * -1);
                        TotalPriceLit.Text = Helper.FormatPrice(PublicSubscriptionCycles.TotalPrice);
                    }
                }

                int bookingId;
                if (Page.RouteData.Values["BookingId"] != null && 
                    int.TryParse(Page.RouteData.Values["BookingId"].ToString(), out bookingId))
                {
                    var productRepository = new ProductRepository();
                    PublicProducts = productRepository.GetProductsByBookingId(bookingId);
                }

                if (PublicCustomerInfos == null ||
                    PublicSubscriptionBooking != null &&
                    PublicCustomerInfos.CustomerId != PublicSubscriptionBooking.CustomerId)
                {
                    Response.Redirect(Constant.InvalidTicketPage, false);
                    //Response.Redirect(string.Format(Constant.SignIpPage, WebUtility.UrlEncode(Request.Url.PathAndQuery)));
                }

                if (PublicDiscounts == null)
                {
                    Response.Redirect(string.Format(Constant.SignIpPage, HttpUtility.UrlEncode(Request.Url.PathAndQuery)));
                }

                if (PublicSubscriptionBooking == null)
                {
                    if (PublicCustomerInfos != null && !string.IsNullOrEmpty(PublicCustomerInfos.BrowsePassUrl))
                    {
                        Response.Redirect(PublicCustomerInfos.BrowsePassUrl, true);
                    }
                    Response.Redirect(!string.IsNullOrEmpty((string)Session["SearchPage"])
                        ? Session["SearchPage"].ToString()
                        : Constant.SearchPageDefault, true);
                }

                if (PublicCustomerInfos == null || string.IsNullOrEmpty(PublicCustomerInfos.EmailAddress))
                {
                    PublicCustomerInfos = _customerInfoRepository.GetById(PublicSubscriptionBooking.CustomerId);
                }
                if (!PublicCustomerInfos.IsConfirmed)
                {
                    PasswordErrorMessage.Visible = false;
                    CustomerMV.ActiveViewIndex = 0;
                }

                if (PublicProducts != null)
                {
                    FirstPassReserveLit.Text = "and your first day pass has been reserved";
                    ViewReservation.Visible = true;
                    AnchorButton.Text = "Reserve Pass";
                }
            }
            catch (Exception ex)
            {
                var logs = new Logs
                {
                    LogKey = "Subscription-Confirmation-Error",
                    UpdatedDate = DateTime.UtcNow,
                    UpdatedContent = string.Format("Subscription Confirmation Error - {0} - {1} - {2}", ex.Message, ex.StackTrace, ex.Source),
                    UpdatedBy = PublicCustomerInfos != null ? PublicCustomerInfos.CustomerId : 0
                };
                _subscriptionBookingRepository.AddLog(logs);
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
            if (PublicProducts != null)
            {
                RedirectToConfirmPage();
            }
            Response.Redirect(Constant.SearchPageDefault, true);
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

            customerInfo.Password = Password.Text;
            customerInfo.IsConfirmed = true;
            _customerInfoRepository.Update(customerInfo);

            CustomerMV.ActiveViewIndex = 1;
        }

        private void ReBindConfirmInfo()
        {
            imageProduct.Src = string.Format("{0}",
                new Uri(new Uri(AppConfiguration.CdnImageUrlDefault), PublicSubscriptions.ImageUrl).AbsoluteUri);
        }

        protected void ViewReservationOnClick(object sender, EventArgs e)
        {
            RedirectToConfirmPage();
        }

        private void RedirectToConfirmPage()
        {
            var hotels = _subscriptionRepository.HotelList.First(h => h.HotelId == PublicProducts.HotelId);
            var markets = (from m in _subscriptionRepository.MarketList
                           join mh in _subscriptionRepository.MarketHotelList on m.Id equals mh.MarketId
                           where mh.HotelId == hotels.HotelId
                           select m).FirstOrDefault();
            var confirmUrl = string.Format(Constant.ConfirmProductPage,
                markets != null ? markets.LocationName : "socal",
                hotels.CityUrl,
                hotels.HotelNameUrl,
                PublicProducts.ProductNameUrl,
                Page.RouteData.Values["BookingId"]);

            Response.Redirect(confirmUrl, true);
        }
    }
}