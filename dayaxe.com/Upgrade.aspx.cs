using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using AutoMapper;
using DayaxeDal;
using DayaxeDal.Parameters;
using DayaxeDal.Repositories;
using Stripe;

namespace dayaxe.com
{
    public partial class Upgrade : BasePage
    {
        private readonly BookingRepository _bookingRepository = new BookingRepository();
        private readonly ProductRepository _productRepository = new ProductRepository();
        private readonly CustomerCreditRepository _customerCreditRepository = new CustomerCreditRepository();
        private Tuple<BookingsTemps, DiscountBookingsTemps> _bookingTemp;
        private Hotels _hotel;
        private Products _products;

        protected bool ShowAuth = false;
        protected bool SearchCall = false;
        private CustomerCredits _customerCredits { get; set; }


        protected void Page_Init(object sender, EventArgs e)
        {
            if (Page.RouteData.Values["hotelName"] != null && 
                Page.RouteData.Values["productName"] != null)
            {
                _products = _productRepository.GetProductsByName((string)Page.RouteData.Values["hotelName"], 
                    (string)Page.RouteData.Values["productName"], 
                    (string) Session["UserSession"]);
                _hotel = _productRepository.HotelList.First(h => h.HotelId == _products.ProductId);
            }

            if (Session[Constant.UpgradeKey] != null)
            {
                int bookingsTempId = int.Parse(Session[Constant.UpgradeKey].ToString());
                _bookingTemp = _productRepository.GetBookingsTempById(bookingsTempId);
            }

            if (!IsPostBack)
            {
                List<Products> availableUpgradeProducts = _productRepository.GetAvailbleUpgradeProducts(_products.ProductId, _bookingTemp.Item1.CheckinDate);
                const String mixpanelscript = "MixpanelScript";
                string strScript = string.Empty;
                availableUpgradeProducts.ForEach(item =>
                {
                    string strUrl = string.Format("/{0}/{1}/{2}/{3}-",
                        Page.RouteData.Values["market"],
                        Page.RouteData.Values["city"],
                        Page.RouteData.Values["hotelName"],
                        item.ProductName.Trim().Replace(" ", "-").ToLower());
                    switch (item.ProductType)
                    {
                        case (int)Enums.ProductType.SpaPass:
                            strUrl += "spa-pass";
                            break;
                        case (int)Enums.ProductType.Daybed:
                            strUrl += "daybeds";
                            break;
                        default:
                            strUrl += "cabanas";
                            break;
                    }
                    strScript += Helper.GetMixpanelScriptRedirect(item.ProductId, strUrl);
                });
                if (!string.IsNullOrEmpty(strScript))
                {
                    ScriptManager.RegisterClientScriptBlock(HotelList, typeof(string), mixpanelscript, strScript, true);
                }

                LvHotelRepeater.DataSource = availableUpgradeProducts;
                LvHotelRepeater.DataBind();
            }

            if (PublicCustomerInfos != null)
            {
                _customerCredits = _customerCreditRepository.GetById(PublicCustomerInfos.CustomerId);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void LvHotelRepeater_ItemDataBound(object sender, ListViewItemEventArgs e)
        {
            if (e.Item.ItemType == ListViewItemType.DataItem)
            {
                var product = (Products)e.Item.DataItem;

                var recommendationLit = (Literal) e.Item.FindControl("RecommendationLit");
                var upgradeButton = (Button) e.Item.FindControl("Upgrade");

                var aTag = (HtmlAnchor)e.Item.FindControl("HotelItem");
                var image = (Image)e.Item.FindControl("ProductImage");
                var header = (HtmlGenericControl)e.Item.FindControl("ProductHeader");
                var price = (Literal)e.Item.FindControl("PriceProduct");
                var msrpSpan = (HtmlGenericControl)e.Item.FindControl("msrp");
                var priceMsrp = (Literal)e.Item.FindControl("PriceProductOff");

                var passleftDiv = (HtmlGenericControl)e.Item.FindControl("passleftDiv");
                var litPassleft = (Literal)e.Item.FindControl("LitPassleft");
                //var litMaxGuest = (Literal)e.Item.FindControl("MaxGuestLit");

                upgradeButton.Attributes["productId"] = product.ProductId.ToString();
                upgradeButton.Attributes["productName"] = product.ProductName;
                recommendationLit.Text = _hotel.Recommendation;

                aTag.ID = product.HotelId.ToString();
                aTag.Attributes.Add("onclick", string.Format("f{0}(event)", product.ProductId));
                image.Attributes["data-original"] = product.CdnImage;
                image.Attributes["src"] = Constant.ImageDefault;
                header.InnerText = product.ProductName;

                //var actualPrice = product.LowestPrice;
                var customPrice = _productRepository.GetById(product.ProductId, _bookingTemp.Item1.CheckinDate).ActualPriceWithDate;
                var discountPrice = customPrice.Price - customPrice.DiscountPrice - _bookingTemp.Item1.HotelPrice;
                
                var actualPrice = customPrice.Price;
                //List<Discounts> discounts = _productRepository.GetAutoPromosByProductId(product.HotelId).ToList();
                //discounts.ForEach(d =>
                //{
                //    if (d.PercentOff > 0)
                //    {
                //        actualPrice -= actualPrice * d.PercentOff / 100;
                //        priceMsrp.Visible = true;
                //        msrpSpan.Visible = true;
                //    }
                //});

                priceMsrp.Text = Helper.FormatPrice(actualPrice);

                price.Text = Helper.FormatPrice(discountPrice);

                if (_hotel.IsComingSoon.HasValue && _hotel.IsComingSoon.Value)
                {
                    price.Visible = false;
                    passleftDiv.Visible = true;
                    litPassleft.Text = Constant.CommingSoonString;
                    msrpSpan.Visible = false;
                    priceMsrp.Visible = false;
                }
            }
        }

        protected void Upgrade_OnClick(object sender, EventArgs e)
        {
            Response.Redirect(string.Format(Constant.BookProductPage,
                    Page.RouteData.Values["market"],
                    _hotel.CityUrl,
                    _hotel.HotelNameUrl,
                    _products.ProductNameUrl, 
                    _products.ProductId));
        }

        protected void RejectUpgrade_OnClick(object sender, EventArgs e)
        {
            int bookingId = 0;
            if (Session[Constant.UpgradeKey] != null)
            {
                int bookingsTempId = int.Parse(Session[Constant.UpgradeKey].ToString());
                var bookingsTemp = _productRepository.GetBookingsTempById(bookingsTempId);

                if (bookingsTemp != null)
                {
                    var booking = Mapper.Map<BookingsTemps, Bookings>(bookingsTemp.Item1);

                    var products = _bookingRepository.ProductList
                        .FirstOrDefault(p => p.ProductId == booking.ProductId);
                    var hotels = _bookingRepository.HotelList
                        .FirstOrDefault(h => h.HotelId == (products != null ? products.HotelId : 0));
                    var markets = (from m in _productRepository.MarketList
                                   join mh in _productRepository.MarketHotelList on m.Id equals mh.MarketId
                                   where mh.HotelId == (hotels != null ? hotels.HotelId : 0) && m.IsActive
                                   select m).FirstOrDefault();

                    var creditLogDescription = string.Format("{0} – {1} – {2} – ",
                        products != null ? products.ProductName : "",
                        hotels != null ? hotels.HotelName : "",
                        markets != null ? markets.LocationName : "");

                    int discountId = bookingsTemp.Item2 != null ? bookingsTemp.Item2.DiscountId : 0;

                    var addParam = new AddBookingParams
                    {
                        BookingObject = booking,
                        DiscountId = discountId,
                        CustomerCreditObject = _customerCredits,
                        Description = creditLogDescription
                    };

                    bookingId = _bookingRepository.Add(addParam);
                }
            }

            CacheLayer.Clear(CacheKeys.BookingsCacheKey);
            CacheLayer.Clear(CacheKeys.CustomerInfosCacheKey);
            CacheLayer.Clear(CacheKeys.DiscountsCacheKey);
            CacheLayer.Clear(CacheKeys.DiscountBookingsCacheKey);
            CacheLayer.Clear(CacheKeys.SurveysCacheKey);

            Session.Remove(Constant.UpgradeKey);
            Response.Redirect(string.Format(Constant.ConfirmProductPage,
                        Page.RouteData.Values["market"],
                        Page.RouteData.Values["city"],
                        Page.RouteData.Values["hotelName"],
                        Page.RouteData.Values["productName"],
                        bookingId));
        }

        #region Private Function

        protected StripeCustomer GetCustomerById(string customerId)
        {
            var customerService = new StripeCustomerService();
            StripeCustomer stripeCustomer = customerService.Get(customerId);

            return stripeCustomer;
        }

        protected StripeCharge CreateCharges(double amount, string customerId, string description)
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

        #endregion
    }
}