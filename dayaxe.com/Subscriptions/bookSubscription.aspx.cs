using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Web.UI.WebControls;
using Dayaxe.SendEmail;
using DayaxeDal;
using DayaxeDal.Extensions;
using DayaxeDal.Parameters;
using DayaxeDal.Repositories;
using Newtonsoft.Json;
using Stripe;

namespace dayaxe.com.Subscriptions
{
    public partial class BookSubscriptionPage : BasePage
    {
        protected bool ShowAuth { get; set; }

        protected DayaxeDal.Subscriptions PublicSubscription { get; set; }

        protected string ProductTypeString { get; set; }

        protected string ProductTypeTrackString { get; set; }

        protected double NormalPrice { get; set; }

        private const int TotalTickets = 1;

        private string _discountKey = string.Empty;

        protected Discounts PublicDiscountUsed { get; set; }

        private CustomerCredits PublicCustomerCredits { get; set; }

        private readonly SubscriptionRepository _subscriptionRepository = new SubscriptionRepository();
        private readonly SubscriptionBookingRepository _subscriptionBookingRepository = new SubscriptionBookingRepository();
        private readonly CustomerInfoRepository _customerInfoRepository = new CustomerInfoRepository();
        private readonly CustomerCreditRepository _customerCreditRepository = new CustomerCreditRepository();
        private readonly DiscountRepository _discountRepository = new DiscountRepository();

        protected void Page_Init(object sender, EventArgs e)
        {
            if (Page.RouteData.Values["SubscriptionId"] != null)
            {
                int id;
                int.TryParse((string) Page.RouteData.Values["SubscriptionId"], out id);
                PublicSubscription = _subscriptionRepository.GetById(id);
            }

            //goBack.HRef = AppConfiguration.LandingPageSubscription;

            if (!IsPostBack)
            {
                hotelname.Text = PublicSubscription.Name;
                Neighborhood.Text = PublicSubscription.ProductHighlight;

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

            if (PublicCustomerInfos != null)
            {
                PublicCustomerCredits = _customerCreditRepository.GetById(PublicCustomerInfos.CustomerId);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (PublicSubscription == null)
            {
                if (PublicCustomerInfos != null && !string.IsNullOrEmpty(PublicCustomerInfos.BrowsePassUrl))
                {
                    Response.Redirect(PublicCustomerInfos.BrowsePassUrl, true);
                }
                Response.Redirect(!string.IsNullOrEmpty((string)Session["SearchPage"])
                    ? Session["SearchPage"].ToString()
                    : Constant.SearchPageDefault, true);
            }
            if (!IsPostBack)
            {
                ShowCustomerInfos();
            }

            BindSubscriptionInfos();

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

            try
            {
                bool isSuccess;
                int bookingId = PurchaseSubscription(out isSuccess);
                if (isSuccess)
                {
                    Session["BookingSuccess"] = true;
                    Response.Redirect(string.Format(Constant.SubscriptionConfirmPage,
                        PublicSubscription.StripePlanId,
                        bookingId,
                        ""), false);
                }
                else
                {
                    ScriptManager.RegisterClientScriptBlock(BookProductUpdatePanel, typeof(string), "openConfirmPopup", "window.showConfirmM = 'true';", true);
                }
            }
            catch (Exception ex)
            {
                ErrorMessageLit.Text = ex.Message;

                var logs = new Logs
                {
                    LogKey = "Subscription-Booking-Error-On-Submit",
                    UpdatedDate = DateTime.UtcNow,
                    UpdatedContent = string.Format("Booking Error - {0} - {1} - {2}", ex.Message, ex.StackTrace, ex.Source),
                    UpdatedBy = PublicCustomerInfos != null ? PublicCustomerInfos.CustomerId : 0
                };
                _subscriptionBookingRepository.AddLog(logs);
            }
        }

        private int PurchaseSubscription(out bool success)
        {
            // Check customer change their email
            success = true;
            bool isPayByCredit;

            var bookingExists = _subscriptionBookingRepository.GetBookingInLast3Minutes(PublicSubscription.Id,
                PublicCustomerInfos != null ? PublicCustomerInfos.EmailAddress : email.Text.Replace(" ", ""));
            if (bookingExists != null)
            {
                success = false;
                newBookingModal.Visible = true;
                return bookingExists.Id;
            }

            if (PublicCustomerInfos == null)
            {
                PublicCustomerInfos = new CustomerInfos
                {
                    EmailAddress = email.Text.Replace(" ", ""),
                    FirstName = FirstName.Text.Trim(),
                    LastName = LastName.Text.Trim(),
                    ZipCode = txtzipcode.Text.Trim()
                };

                // Create new account with email
                var response = _customerInfoRepository.GetVipAccess(email.Text.Replace(" ", ""), Constant.SearchPageDefault, FirstName.Text.Trim(), LastName.Text.Trim());
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
                PublicCustomerCredits = _subscriptionBookingRepository.GetCustomerCredits(response.CustomerId);


                Session["UserSession"] = response.AccessKey;
                Session["IsRegister"] = true;
                Session["ReferralCode"] = response.ReferralCode;
            }

            Regex regex = new Regex(@"([\d]+)(\s)?/(\s)?([\d]+)");
            int month;
            int year;
            int.TryParse(regex.Match(txtexpdat.Value).Groups[1].Value, out month);
            int.TryParse("20" + regex.Match(txtexpdat.Value).Groups[4].Value, out year);
            string fullName = string.Format("{0} {1}", FirstName.Text, LastName.Text);

            // Check customer has exists with email
            int customerId = PublicCustomerInfos.CustomerId;
            StripeCustomer stripeCustomer = null;
            Discounts discounts = new Discounts();
            double actualPrice = PublicSubscription.Price;
            double totalPrice;
            bool hasCoupon = false;
            string stripeCouponId = string.Empty;

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
                    stripeCustomer = CreateCustomer(PublicCustomerInfos.EmailAddress, fullName, stripeToken.Id);
                    PublicCustomerInfos.StripeCustomerId = stripeCustomer.Id;
                }
                else
                {
                    // Update Stripe exists customer with New Card
                    var card = CreateCard(stripeToken.Id, PublicCustomerInfos.StripeCustomerId);
                    UpdateCustomer(PublicCustomerInfos.EmailAddress, fullName,
                        PublicCustomerInfos.StripeCustomerId, card.Id);
                }
                _customerInfoRepository.Update(PublicCustomerInfos);

                isPayByCredit = IsPayByCreditCheckBox.Checked;
            }
            else
            {
                isPayByCredit = DCreditCheckBox.Checked;
            }

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
            if (discounts != null && discounts.Id > 0)
            {
                discounts = _discountRepository.VerifyDiscountsSubscription(discounts, customerId, PublicSubscription.Id);

                // Discount Invalid
                if (discounts != null)
                {
                    actualPrice = Helper.CalculateDiscount(discounts, actualPrice, TotalTickets);
                    hasCoupon = true;
                }
                else
                {
                    InitDefaultPromo(Message.ExceedLimit, false);
                    Session[_discountKey] = null;
                    success = false;
                    return 0;
                }

                if (discounts.BillingCycleNumber > 1)
                {
                    stripeCouponId = string.Format("{0}C{1}", PublicCustomerInfos.CustomerId,
                        Helper.RandomString(Constant.BookingCodeLength));
                    if (discounts.PercentOff > 0)
                    {
                        var couponPrice = PublicSubscription.Price - actualPrice;
                        //Create the Coupon
                        var coupon = new StripeCouponCreateOptions
                        {
                            Duration = "repeating",
                            Id = stripeCouponId,
                            MaxRedemptions = discounts.BillingCycleNumber,
                            DurationInMonths = discounts.BillingCycleNumber
                        };


                        if (discounts.PromoType == (int) Enums.PromoType.Fixed)
                        {
                            coupon.AmountOff = (int)(couponPrice * 100);
                            coupon.Currency = "usd";
                        }

                        if (discounts.PromoType == (int) Enums.PromoType.Percent)
                        {
                            coupon.PercentOff = (int) (discounts.PercentOff);
                        }

                        //coupon.AmountOff - AmountOff is not a property of StripeCouponCreateOptions
                        var couponService = new StripeCouponService();
                        StripeCoupon createdCoupon = couponService.Create(coupon);

                        //Apply it to the customer
                        var customerOptions = new StripeCustomerUpdateOptions
                        {
                            Coupon = createdCoupon.Id
                        };

                        var customerService = new StripeCustomerService();
                        customerService.Update(PublicCustomerInfos.StripeCustomerId, customerOptions);
                    }
                }
            }

            if (PublicCustomerInfos.FirstName != FirstName.Text.Trim() || PublicCustomerInfos.LastName != LastName.Text.Trim())
            {
                PublicCustomerInfos.FirstName = FirstName.Text.Trim();
                PublicCustomerInfos.LastName = LastName.Text.Trim();
                _customerInfoRepository.Update(PublicCustomerInfos);

                // Update Stripe exists customer
                UpdateCustomer(PublicCustomerInfos.EmailAddress, fullName, PublicCustomerInfos.StripeCustomerId, string.Empty);
            }

            // Discount 100% ??
            // Price equal 0, so we should not charge with this
            totalPrice = actualPrice * TotalTickets;
            double chargePrice = totalPrice;
            string creditLogDescription = string.Empty;
            
            var subscriptionOptions = new StripeSubscriptionCreateOptions
            {
                PlanId = PublicSubscription.StripePlanId
            };

            // Use DayAxe Credit
            if (isPayByCredit && PublicCustomerCredits != null && PublicCustomerCredits.Amount > 0)
            {
                hasCoupon = true;

                chargePrice = totalPrice - PublicCustomerCredits.Amount;
                if (chargePrice <= 0)
                {
                    chargePrice = 0;
                }
            }

            if (hasCoupon && string.IsNullOrEmpty(stripeCouponId))
            {
                var couponPrice = PublicSubscription.Price - chargePrice;
                if (couponPrice > 0)
                {
                    // Create Coupon Id because we used DayAxe Credit
                    stripeCouponId = string.Format("{0}C{1}", PublicCustomerInfos.CustomerId,
                        Helper.RandomString(Constant.BookingCodeLength));
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

                    subscriptionOptions.CouponId = stripeCouponId;

                    creditLogDescription = string.Format("{0} – {1}",
                        PublicSubscription.Name, coupon.Id);
                }
            }

            // Create Subscription on Stripe
            var subscriptionService = new StripeSubscriptionService();
            StripeSubscription subscription = subscriptionService.Create(PublicCustomerInfos.StripeCustomerId, subscriptionOptions);

            var booking = new SubscriptionBookings
            {
                SubscriptionId = PublicSubscription.Id,
                Quantity = TotalTickets,
                StripeCouponId = stripeCouponId,
                BookedDate = DateTime.UtcNow,
                ActivedDate = subscription.Start,
                StartDate = subscription.CurrentPeriodStart,
                EndDate = subscription.CurrentPeriodEnd,
                Status = (int)Enums.SubscriptionBookingStatus.Active,
                CustomerId = customerId,
                LastUpdatedDate = DateTime.UtcNow,
                LastUpdatedBy = customerId,
                StripeSubscriptionId = subscription.Id
            };
            
            var param = new AddSubscriptionBookingParams
            {
                SubscriptionBookingsObject = booking,
                CustomerCreditsObject = PublicCustomerCredits,
                Description = creditLogDescription,
                SubscriptionName = PublicSubscription.Name,
                FirstName = PublicCustomerInfos.FirstName,
                LastName = PublicCustomerInfos.LastName,
                SubscriptionBookingDiscounts = discounts,
                ActualPrice = actualPrice,
                MerchantPrice = PublicSubscription.Price,
                PayByCredit = totalPrice.Equals(chargePrice) ? 0 : totalPrice - chargePrice,
                TotalPrice = chargePrice,
                MaxPurchases = PublicSubscription.MaxPurchases
            };

            int bookingId = _subscriptionBookingRepository.Add(param);

            //Session.Remove(_discountKey);

            CacheLayer.Clear(CacheKeys.SubscriptionBookingsCacheKey);
            CacheLayer.Clear(CacheKeys.CustomerInfosCacheKey);
            CacheLayer.Clear(CacheKeys.CustomerCreditsCacheKey);
            CacheLayer.Clear(CacheKeys.CustomerCreditLogsCacheKey);
            CacheLayer.Clear(CacheKeys.DiscountsCacheKey);
            CacheLayer.Clear(CacheKeys.SubscriptionBookingDiscountsCacheKey);
            CacheLayer.Clear(CacheKeys.SubsciptionDiscountUsedCacheKey);
            CacheLayer.Clear(CacheKeys.SubscriptionDiscountsCacheKey);
            CacheLayer.Clear(CacheKeys.SubscriptionCyclesCacheKey);

            return bookingId;
        }

        private StripeCustomer CreateCustomer(string emailStr, string name, string tokenId)
        {
            var myCustomer = new StripeCustomerCreateOptions();
            myCustomer.Email = emailStr;
            myCustomer.Description = string.Format("{0} ({1})", name, emailStr);

            myCustomer.SourceToken = tokenId;

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
            msrp.Visible = false;
            MessageLabel.Text = string.Empty;
            LitDisclaimer.Text = string.Format(Message.TermWithPromo, string.Empty);
            discountInfoRow.Visible = false;
            bool isLimit;
            if (string.IsNullOrEmpty(PromoText.Text))
            {
                InitDefaultPromo(Message.InvalidOrExpiredPromo, false);
                return;
            }
            
            var giftCard = _customerCreditRepository.GetGiftCardByCode(PromoText.Text.Trim());
            bool isGiftCard = giftCard != null;

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
                BindSubscriptionInfos();
                InitDefaultPromo(ErrorMessage.GiftCardHasBeenApplied);
                BookProductUpdatePanel.Update();
            }

            if (!isGiftCard)
            {
                var param = new GetDiscountValidByCodeParams
                {
                    Code = PromoText.Text.Trim(),
                    SubscriptionId = PublicSubscription.Id,
                    CustomerId = PublicCustomerInfos != null ? PublicCustomerInfos.CustomerId : 0,
                    IsAdmin = false,
                    BookingId = 0
                };

                Discounts discounts = _discountRepository.GetDiscountValidByCode(param, out isLimit);
                if (discounts == null)
                {
                    InitDefaultPromo(isLimit ? Message.ExceedLimit : Message.InvalidOrExpiredPromo, false);
                    return;
                }

                if (discounts.MinAmount > PublicSubscription.Price * TotalTickets)
                {
                    InitDefaultPromo(ErrorMessage.MinAmountNotMatch, false);
                    return;
                }

                double actualPrice = NormalPrice;
                actualPrice = Helper.CalculateDiscount(discounts, actualPrice, TotalTickets);

                if (!actualPrice.Equals(NormalPrice))
                {
                    msrp.Visible = true;
                    msrpPrice.Text = Helper.FormatPrice(NormalPrice * TotalTickets);
                    LitDisclaimer.Text = string.Format(Message.TermWithPromo, Message.PeriodString);
                }

                moneyPrice.Text = Helper.FormatPrice(actualPrice * TotalTickets);
                perMoneyPrice.Text = Helper.FormatPrice(actualPrice);

                string json = JsonConvert.SerializeObject(discounts, CustomSettings.SerializerSettings());
                Session[_discountKey] = json;
                BindSubscriptionInfos();
            }
        }

        private void InitDefaultPromo(string message, bool isSuccess = true)
        {
            rowMessage.Visible = true;
            MessageLabel.Text = message;
            MessageLabel.Visible = true;
            Session[_discountKey] = JsonConvert.SerializeObject(null, CustomSettings.SerializerSettings());
            moneyPrice.Text = Helper.FormatPrice(NormalPrice * TotalTickets);
            perMoneyPrice.Text = Helper.FormatPrice(NormalPrice);
            perMoneyPrice.Attributes["data-price"] = NormalPrice.ToString();
            MessageLabel.CssClass = isSuccess ? "success-message" : "error-message";
        }

        private void AssignPassString()
        {
            ProductTypeTrackString = ((Enums.SubscriptionType)PublicSubscription.SubscriptionType).ToDescription();

            ProductTypeString = "GOLD PASS";
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
                        _subscriptionBookingRepository.AddLog(log);
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

        private void BindSubscriptionInfos()
        {
            var isAllowCheckout = IsAllowCheckout.Value;
            LitDisclaimer.Text = string.Format(Message.TermWithPromo, string.Empty);
            if (String.Equals(isAllowCheckout, "true", StringComparison.OrdinalIgnoreCase))
            {
                AnchorButton.CssClass = "AnchorButton";
            }

            _discountKey = string.Format("Discount_Subscription_{0}_{1:ddMMyyyy}_latest", PublicSubscription.Id, DateTime.UtcNow.ToLosAngerlesTime());
            NormalPrice = PublicSubscription.Price;
            var actualPrice = NormalPrice;
            bool isShowMsrp = false;
            
            if (Session[_discountKey] == null)
            {
                PublicDiscountUsed = _subscriptionBookingRepository.GetAutoPromosBySubscriptionId(PublicSubscription.Id).DistinctBy(d => d.Code).FirstOrDefault();

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
                    PublicDiscountUsed = _discountRepository.VerifyDiscountsSubscription(PublicDiscountUsed, PublicCustomerInfos.CustomerId, PublicSubscription.Id);
                }
                if (PublicDiscountUsed != null)
                {
                    actualPrice = Helper.CalculateDiscount(PublicDiscountUsed, actualPrice, TotalTickets);
                    if (!actualPrice.Equals(NormalPrice))
                    {
                        msrp.Visible = true;
                        isShowMsrp = true;

                        // Use Discount so, display Code
                        if (!IsPostBack)
                        {
                            PromoText.Text = PublicDiscountUsed.Code.ToUpper();
                        }
                        discountInfoRow.Visible = true;
                        DiscountOffLit.Text = string.Format("{0} - {1} OFF", PublicDiscountUsed.DiscountName,
                            PublicDiscountUsed.PromoType == (int) Enums.PromoType.Fixed
                                ? "$" + PublicDiscountUsed.PercentOff
                                : PublicDiscountUsed.PercentOff + "%");
                    }
                }
            }

            if (isShowMsrp)
            {
                LitDisclaimer.Text = string.Format(Message.TermWithPromo, Message.PeriodString);
                msrp.Visible = true;
                msrpPrice.Text = Helper.FormatPrice(NormalPrice * TotalTickets);
            }
            moneyPrice.Text = Helper.FormatPrice(actualPrice * TotalTickets);
            perMoneyPrice.Text = Helper.FormatPrice(actualPrice);

            if (PublicCustomerCredits != null)
            {
                DayaxeCreditCardRow.Visible = true;
                double totalPrice = actualPrice * TotalTickets;
                PayByCreditRow.Visible = true;
                if (PublicCustomerCredits.Amount >= totalPrice)
                {
                    if (!IsPostBack)
                    {
                        IsPayByCreditCheckBox.Checked = true;
                    }
                    PayByCreditLiteral.Text = string.Format("Use <b>${0:0.00} of ${1:0.00} DayAxe Credit balance</b>",
                        totalPrice, PublicCustomerCredits.Amount);
                    DCreditInfoLit.Text = string.Format("Use <b>${0:0.00} of ${1:0.00} DayAxe Credit balance</b>",
                        totalPrice, PublicCustomerCredits.Amount);
                    DCardInfoLiteral.Text = string.Format("Use your <b>${0:0.00} of ${1:0.00} DayAxe Credit balance</b>",
                        totalPrice, PublicCustomerCredits.Amount);
                }
                else if (PublicCustomerCredits.Amount > 0)
                {
                    if (!IsPostBack)
                    {
                        IsPayByCreditCheckBox.Checked = true;
                    }
                    PayByCreditLiteral.Text = string.Format("<b>${0:0.00} DayAxe Credit balance</b>",
                        PublicCustomerCredits.Amount);
                    DCreditInfoLit.Text =
                        string.Format("{0:0.00} DayAxe Credit balance", PublicCustomerCredits.Amount);
                    DCardInfoLiteral.Text =
                        string.Format("{0:0.00} DayAxe Credit balance", PublicCustomerCredits.Amount);
                }
                else
                {
                    PayByCreditRow.Visible = false;
                    DayaxeCreditCardRow.Visible = false;
                }
            }
        }
    }
}