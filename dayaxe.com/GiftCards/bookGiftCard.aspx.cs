using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Web.UI.WebControls;
using Dayaxe.SendEmail;
using DayaxeDal;
using DayaxeDal.Extensions;
using DayaxeDal.Repositories;
using Newtonsoft.Json;
using Stripe;

namespace dayaxe.com.GiftCards
{
    public partial class BookGiftCardPage : BasePage
    {
        protected bool ShowAuth { get; set; }

        private CustomerCredits PublicCustomerCredits { get; set; }

        private readonly CustomerInfoRepository _customerInfoRepository = new CustomerInfoRepository();
        private readonly CustomerCreditRepository _customerCreditRepository = new CustomerCreditRepository();
        private readonly GiftCardBookingRepository _giftCardBookingRepository = new GiftCardBookingRepository();

        protected void Page_Init(object sender, EventArgs e)
        {
            if (!IsPostBack)
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
            if (!IsPostBack)
            {
                ShowCustomerInfos();
                DeliveryDateText.Text = DateTime.UtcNow.ToLosAngerlesTime().AddDays(1).ToString("MM/dd/yyyy");

                if (Session["GiftCardValue"] != null)
                {
                    ValueText.Text = (string) Session["GiftCardValue"];
                }
            }

            BindeGiftCardInfos();
        }

        protected void HtmlAnchor_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(ValueText.Text))
            {
                giftValueDiv.Attributes["class"] = "input-group errorborder";
                ErrorMessageGiftCard.Text = Message.EnterGiftCardAmount;
                return;
            }

            if (string.IsNullOrEmpty(ToText.Text))
            {
                ToText.Attributes["class"] = "form-control errorborder";
                ErrorMessageGiftCard.Text = Message.EnterEmailOfRecipient;
                return;
            }

            if (!Helper.IsValidEmail(ToText.Text.Trim()))
            {
                ToText.Attributes["class"] = "form-control errorborder";
                ErrorMessageGiftCard.Text = Message.EnterValidEmailOfRecipient;
                return;
            }

            if (string.IsNullOrEmpty(NameText.Text))
            {
                NameText.Attributes["class"] = "form-control errorborder";
                ErrorMessageGiftCard.Text = Message.EnterNameOfRecipient;
                return;
            }

            if (string.IsNullOrEmpty(DeliveryDateText.Text))
            {
                DeliveryDateText.Attributes["class"] = "form-control errorborder";
                ErrorMessageGiftCard.Text = Message.EnterDeliveryDate;
                return;
            }

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
                int bookingId = PurchaseGiftCard(out isSuccess);
                if (isSuccess)
                {
                    Session["BookingSuccess"] = true;
                    Response.Redirect(string.Format(Constant.GiftCardConfirmPage,
                        bookingId), false);
                }
            }
            catch (Exception ex)
            {
                ErrorMessageLit.Text = ex.Message;

                var logs = new Logs
                {
                    LogKey = "eGiftCard-Booking-Error-On-Submit",
                    UpdatedDate = DateTime.UtcNow,
                    UpdatedContent = string.Format("eGiftCard Booking Error - {0} - {1} - {2}", ex.Message, ex.StackTrace, ex.Source),
                    UpdatedBy = PublicCustomerInfos != null ? PublicCustomerInfos.CustomerId : 0
                };
                _customerInfoRepository.AddLog(logs);
            }
        }

        private int PurchaseGiftCard(out bool success)
        {
            // Check customer change their email
            success = true;
            bool isPayByCredit;

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
                PublicCustomerCredits = _customerInfoRepository.GetCustomerCredits(response.CustomerId);


                Session["UserSession"] = response.AccessKey;
                Session["IsRegister"] = true;
                Session["ReferralCode"] = response.ReferralCode;
            }

            Regex regex = new Regex(@"([\d]+)(\s)?/(\s)?([\d]+)");
            int month;
            int.TryParse(regex.Match(txtexpdat.Value).Groups[1].Value, out month);
            int year;
            int.TryParse("20" + regex.Match(txtexpdat.Value).Groups[4].Value, out year);
            string fullName = string.Format("{0} {1}", FirstName.Text, LastName.Text);

            // Check customer has exists with email
            StripeCustomer stripeCustomer = null;
            //var discounts = new Discounts();
            double actualPrice;

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

            if (PublicCustomerInfos.FirstName != FirstName.Text.Trim() || PublicCustomerInfos.LastName != LastName.Text.Trim())
            {
                PublicCustomerInfos.FirstName = FirstName.Text.Trim();
                PublicCustomerInfos.LastName = LastName.Text.Trim();
                _customerInfoRepository.Update(PublicCustomerInfos);

                // Update Stripe exists customer
                stripeCustomer = UpdateCustomer(PublicCustomerInfos.EmailAddress, fullName, PublicCustomerInfos.StripeCustomerId, string.Empty);
            }

            // Discount 100% ??
            // Price equal 0, so we should not charge with this

            double.TryParse(ValueText.Text, out actualPrice);
            
            double chargePrice = actualPrice;
            string creditLogDescription = string.Format("eGift Cards – {0}", Helper.FormatPrice(actualPrice * -1));
            DateTime deliveryDate;
            string stripeChargeId = string.Empty;

            // Use DayAxe Credit
            if (isPayByCredit && PublicCustomerCredits != null && PublicCustomerCredits.Amount > 0)
            {
                // Create Coupon Id because we used DayAxe Credit
                chargePrice = actualPrice - PublicCustomerCredits.Amount;
                if (chargePrice <= 0)
                {
                    chargePrice = 0;
                }
            }

            if (chargePrice > 0)
            {
                StripeCharge stripeCharge = CreateCharges(chargePrice, stripeCustomer.Id, creditLogDescription);
                stripeChargeId = stripeCharge.Id;
            }

            DateTime.TryParseExact(DeliveryDateText.Text, "MM/dd/yyyy", null, DateTimeStyles.None, out deliveryDate);

            double userBookedDate;
            double.TryParse(HidUserBookedDate.Value, out userBookedDate);

            var param = new GiftCardBookings
            {
                CustomerId = PublicCustomerInfos.CustomerId,
                GiftCardId = 0,
                Price = chargePrice,
                PayByCredit = actualPrice-chargePrice,
                TotalPrice = actualPrice,
                StripeChargeId = stripeChargeId,
                BookedDate = DateTime.UtcNow,
                RecipientEmail = ToText.Text.Trim(),
                RecipientName = NameText.Text.Trim(),
                Message = !string.IsNullOrEmpty(MessageText.Text) ? MessageText.Text : "Enjoy the gift of DayAxe from me!",
                DeliveryDate = deliveryDate.Date.AddHours(9 + 5), // 9AM + EST = 5 hours
                Description = creditLogDescription,
                LastUpdatedBy = PublicCustomerInfos.CustomerId,
                LastUpdatedDate = DateTime.UtcNow,
                UserBookedDate = userBookedDate
            };

            int giftCardBooking = _giftCardBookingRepository.Add(param);

            CacheLayer.Clear(CacheKeys.GiftCardCacheKey);
            CacheLayer.Clear(CacheKeys.GiftCardBookingCacheKey);

            CacheLayer.Clear(CacheKeys.CustomerCreditsCacheKey);
            CacheLayer.Clear(CacheKeys.CustomerCreditLogsCacheKey);

            return giftCardBooking;
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
            if (string.IsNullOrEmpty(PromoText.Text))
            {
                InitDefaultPromo(Message.InvalidOrExpiredPromo, false);
                return;
            }

            if (PublicCustomerCredits == null)
            {
                InitDefaultPromo(ErrorMessage.YouMustLogIn, false);
                return;
            }

            var giftCard = _customerCreditRepository.GetGiftCardByCode(PromoText.Text.Trim());

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
                BindeGiftCardInfos();
                InitDefaultPromo(ErrorMessage.GiftCardHasBeenApplied);
                BookProductUpdatePanel.Update();
            }
        }

        private void InitDefaultPromo(string message, bool isSuccess = true)
        {
            rowMessage.Visible = true;
            MessageLabel.Text = message;
            MessageLabel.Visible = true;
            MessageLabel.CssClass = isSuccess ? "success-message" : "error-message";
            //moneyPrice.Text = Helper.FormatPrice(NormalPrice * TotalTickets);
            //perMoneyPrice.Text = Helper.FormatPrice(NormalPrice);
            //perMoneyPrice.Attributes["data-price"] = NormalPrice.ToString();
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
                            //AnchorButton.Attributes["class"] = "AnchorButton";

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
                        _customerInfoRepository.AddLog(log);
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

        private void BindeGiftCardInfos()
        {
            var isAllowCheckout = IsAllowCheckout.Value;
            if (String.Equals(isAllowCheckout, "true", StringComparison.OrdinalIgnoreCase))
            {
                AnchorButton.CssClass = "AnchorButton";
            }
            
            double actualPrice;
            double.TryParse(ValueText.Text, out actualPrice);
            
            moneyPrice.Text = Helper.FormatPrice(actualPrice);
            perMoneyPrice.Text = Helper.FormatPrice(actualPrice);

            if (PublicCustomerCredits != null)
            {
                DayaxeCreditCardRow.Visible = true;
                double totalPrice = actualPrice;
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

        protected void ValueText_OnTextChanged(object sender, EventArgs e)
        {
            Session["GiftCardValue"] = ValueText.Text;
        }
    }
}