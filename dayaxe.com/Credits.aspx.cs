using System;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using DayaxeDal;
using DayaxeDal.Extensions;
using DayaxeDal.Repositories;

namespace dayaxe.com
{
    public partial class Credits : BasePage
    {
        private CustomerCredits _customerCredits;
        private CustomerCreditRepository _crRepository = new CustomerCreditRepository();

        protected void Page_Init(object sender, EventArgs e)
        {
            if (PublicCustomerInfos == null)
            {
                var loginLinkButton = (HtmlAnchor)ControlExtensions.FindControlRecursive(Master, "LoginLinkButton");
                loginLinkButton.Visible = false;
                Response.Redirect(string.Format(Constant.SignIpPage, HttpUtility.UrlEncode(Request.Url.PathAndQuery)));
            }
            _customerCredits = _crRepository.GetById(PublicCustomerInfos.CustomerId);

            if (!IsPostBack)
            {
                if (_customerCredits == null)
                {
                    SendToFriendButton.Text = "Add";
                }
                ReferralCodeLit.Text = _customerCredits != null ? _customerCredits.ReferralCode : "N/A";
                TotalCreditLit.Text = Helper.FormatPrice(_customerCredits != null ? _customerCredits.Amount : 0);
                EnjoyFreeDayCationLit.Text = string.Format(ErrorMessage.EnjoyDayCations,
                    Helper.FormatPrice(_customerCredits != null ? _customerCredits.FirstRewardForOwner : 0),
                    Helper.FormatPrice(_customerCredits != null ? _customerCredits.FirstRewardForOwner : 0));

                RebindLogs();
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void SendToFriendButton_OnClick(object sender, EventArgs e)
        {
            if (_customerCredits == null)
            {
                _customerCredits = new CustomerCredits
                {
                    Amount = 0,
                    CreatedDate = DateTime.UtcNow,
                    CustomerId = PublicCustomerInfos.CustomerId,
                    FirstRewardForOwner = 5,
                    FirstRewardForReferral = 5,
                    IsActive = true,
                    IsDelete = false,
                    LastUpdatedDate = DateTime.UtcNow,
                    ReferralCode = Helper.RandomString(8)
                };

                _crRepository.Add(_customerCredits);
            }

            CacheLayer.Clear(CacheKeys.CustomerCreditsCacheKey);
            CacheLayer.Clear(CacheKeys.CustomerCreditLogsCacheKey);
        }

        protected void AddCreditButton_OnClick(object sender, EventArgs e)
        {
            errorRow.Visible = true;
            if (string.IsNullOrEmpty(GiftCodeText.Text))
            {
                MessageLabel.Text = ErrorMessage.InvalidCode;
                MessageLabel.CssClass = "error-message";
                return;
            }

            var giftCard = _crRepository.GetGiftCardByCode(GiftCodeText.Text.Trim());
            bool isGiftCard = giftCard != null;

            if (giftCard != null)
            {
                if (giftCard.Status == (short) Enums.GiftCardType.Used)
                {
                    MessageLabel.Text = ErrorMessage.GiftCardHasBeenUsed;
                    MessageLabel.CssClass = "error-message";
                    return;
                }

                if (!string.IsNullOrEmpty(giftCard.EmailAddress) &&
                    !PublicCustomerInfos.EmailAddress.Equals(giftCard.EmailAddress, StringComparison.OrdinalIgnoreCase))
                {
                    MessageLabel.Text = ErrorMessage.PleaseCheckYourGiftCardWithCurrentAccount;
                    MessageLabel.CssClass = "error-message";
                    return;
                }

                var amount = _crRepository.AddGiftCard(_customerCredits, GiftCodeText.Text.Trim());
                TotalCreditLit.Text = Helper.FormatPrice(amount);
                MessageLabel.CssClass = "success-message";

                CacheLayer.Clear(CacheKeys.GiftCardCacheKey);
            }

            if (!isGiftCard)
            {
                if (_customerCredits.ReferralCustomerId != 0)
                {
                    MessageLabel.Text = ErrorMessage.CanNotReferralTwice;
                    MessageLabel.CssClass = "error-message";
                    return;
                }

                var referralCustomer = _crRepository.GetByReferCode(GiftCodeText.Text.Trim());
                if (referralCustomer == null)
                {
                    MessageLabel.Text = ErrorMessage.InvalidCode;
                    MessageLabel.CssClass = "error-message";
                    return;
                }
                if (_crRepository.BookingList.Count(b => b.CustomerId == referralCustomer.CustomerId) == 0)
                {
                    MessageLabel.Text = ErrorMessage.DoNotHavePurchase;
                    MessageLabel.CssClass = "error-message";
                    return;
                }

                if (_crRepository.BookingList.Count(b => b.CustomerId == _customerCredits.CustomerId) > 0)
                {
                    MessageLabel.Text = ErrorMessage.YouAlreadyHaveBookings;
                    MessageLabel.CssClass = "error-message";
                    return;
                }

                var total = _crRepository.AddReferral(_customerCredits, referralCustomer);
                TotalCreditLit.Text = Helper.FormatPrice(total);
            }

            MessageLabel.Text = ErrorMessage.CreditHasBeenApplied;
            MessageLabel.CssClass = "success-message";
            CacheLayer.Clear(CacheKeys.CustomerCreditsCacheKey);
            CacheLayer.Clear(CacheKeys.CustomerCreditLogsCacheKey);
            RebindLogs();
        }

        private void RebindLogs(bool isReload = false)
        {
            if (isReload)
            {
                _crRepository = new CustomerCreditRepository();
            }
            CreditHistoryRpt.DataSource = _crRepository.GetAllLogsByCustomerId(PublicCustomerInfos.CustomerId);
            CreditHistoryRpt.DataBind();
        }

        protected void CreditHistoryRpt_OnItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var amountLit = (Literal) e.Item.FindControl("Amount");
                var credits = (CustomerCreditLogs) e.Item.DataItem;

                if (credits.CreditType == (byte) Enums.CreditType.Charge)
                {
                    amountLit.Text = string.Format("-${0:0.00}", credits.Amount);
                }
                else
                {
                    amountLit.Text = string.Format("${0:0.00}", credits.Amount);
                }
            }
        }
    }
}