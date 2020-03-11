using System;
using System.Net;
using System.Web;
using System.Web.UI.HtmlControls;
using DayaxeDal;
using DayaxeDal.Extensions;
using DayaxeDal.Repositories;
using Stripe;

namespace dayaxe.com.Subscriptions
{
    public partial class MembershipDetailPage : BasePage
    {
        private DayaxeDal.Subscriptions _subscriptions;
        private SubscriptionBookings _subscriptionBookings;
        private readonly CustomerInfoRepository _customerInfoRepository = new CustomerInfoRepository();
        private readonly SubscriptionBookingRepository _subscriptionBookingRepository = new SubscriptionBookingRepository();

        protected void Page_Init(object sender, EventArgs e)
        {
            if (PublicCustomerInfos == null)
            {
                var loginLinkButton = (HtmlAnchor)ControlExtensions.FindControlRecursive(Master, "LoginLinkButton");
                loginLinkButton.Visible = false;
                Response.Redirect(string.Format(Constant.SignIpPage, HttpUtility.UrlEncode(Request.Url.PathAndQuery)));
            }

            MemberShipDetailView.ActiveViewIndex = 0;
            YourPlanLabel.Text = ErrorMessage.MembershipNotActive;
            reactiveMembershipLink.Visible = true;
            cancelMembershipLink.Visible = false;


            _subscriptionBookings = _subscriptionBookingRepository.GetSuspendedByCustomerId(PublicCustomerInfos.CustomerId);

            if (_subscriptionBookings != null)
            {
                PublicDiscounts = _customerInfoRepository.GetSubscriptionDiscountSuspended(PublicCustomerInfos.CustomerId);
                ErrorMessageLabel.Text = "Your subscription is suspended. Please contact our customer support at <a href=\"mailto:help@dayaxe.com\">help@dayaxe.com</a> to resolve this issue.";
                ErrorMessageLabel.Visible = true;
                ErrorMessageLabel.CssClass = "error-message";
            }

            if (PublicDiscounts != null)
            {
                MemberShipDetailView.ActiveViewIndex = 1;

                _subscriptions = _customerInfoRepository.GetSubscriptionByCustomerId(PublicCustomerInfos.CustomerId, PublicDiscounts.Id);
                if (_subscriptions != null)
                {
                    YourPlanLabel.Text = string.Format("{0} - {1} <br/> {2} / month",
                        _subscriptions.Name.ToUpper(),
                        _subscriptions.ProductHighlight,
                        Helper.FormatPrice(_subscriptions.Price));
                    WhatYouGetLit.Text = _subscriptions.WhatYouGet;
                }

                // Avoid get again by Suspended case
                if (_subscriptionBookings == null)
                {
                    _subscriptionBookings = _subscriptionBookingRepository.GetByCustomerId(PublicCustomerInfos.CustomerId, PublicDiscounts.Id);
                }

                if (_subscriptionBookings != null)
                {
                    switch (_subscriptionBookings.Status)
                    {
                        case (int) Enums.SubscriptionBookingStatus.Active:
                            NextCycleLit.Text = string.Format("Next cycle begins {0:MMMM dd}", 
                                PublicDiscounts.EndDate.HasValue ? 
                                    PublicDiscounts.EndDate.Value.ToLosAngerlesTime().AddDays(1) : DateTime.UtcNow);
                            reactiveMembershipLink.Visible = false;
                            cancelMembershipLink.Visible = true;
                            break;
                        case (int) Enums.SubscriptionBookingStatus.End:
                            NextCycleLit.Visible = false;
                            break;
                        case (int)Enums.SubscriptionBookingStatus.Suspended:
                            reactiveMembershipLink.Visible = false;
                            cancelMembershipLink.Visible = false;
                            break;
                    }
                }

                ValidLit.Text = string.Format("Valid through {0:MMMM dd, yyyy}", 
                    PublicDiscounts.EndDate.HasValue ? 
                        PublicDiscounts.EndDate.Value.ToLosAngerlesTime() : DateTime.UtcNow);
                MembershipIdLabel.Text = PublicDiscounts.Code;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void CancelMembershipLinkButton_OnClick(object sender, EventArgs e)
        {
            var subscriptionService = new StripeSubscriptionService();
            StripeSubscription subscription = subscriptionService.Cancel(_subscriptionBookings.StripeSubscriptionId, true);

            _subscriptionBookings.Status = (int) Enums.SubscriptionBookingStatus.End;
            _subscriptionBookings.CancelDate = DateTime.UtcNow;

            _subscriptionBookings.Description = subscription.StripeResponse.ObjectJson;

            _subscriptionBookingRepository.Update(_subscriptionBookings);

            NextCycleLit.Visible = false;
            cancelMembershipLink.Visible = false;
            reactiveMembershipLink.Visible = true;
            ErrorMessageLabel.Visible = true;
            ErrorMessageLabel.Text = Message.SubscriptionCancel;
            ErrorMessageLabel.CssClass = "error-message";
        }

        protected void ActiveMembershipLinkButton_OnClick(object sender, EventArgs e)
        {
            Response.Redirect(Constant.GoldPassLink + "?source=account", true);
        }

        protected void ActiveNowLinkButton_OnClick(object sender, EventArgs e)
        {
            Response.Redirect(Constant.GoldPassLink, true);
        }

        protected void ReActiveMembershipLinkButton_OnClick(object sender, EventArgs e)
        {
            var subscriptionOptions = new StripeSubscriptionUpdateOptions()
            {
                PlanId = _subscriptions.StripePlanId
            };

            var subscriptionService = new StripeSubscriptionService();
            StripeSubscription subscription = subscriptionService.Update(_subscriptionBookings.StripeSubscriptionId, subscriptionOptions);

            var bookingRepository = new BookingRepository();

            switch (subscription.Status)
            {
                case "past_due":
                case "unpaid":
                    _subscriptionBookings.Status = (int)Enums.SubscriptionBookingStatus.Suspended;
                    bookingRepository.ExpiredBookingByCancelledSubscription(PublicDiscounts.Id);
                    break;
                case "active":
                    // Set Date of Discounts
                    _subscriptionBookings.Status = (int)Enums.SubscriptionBookingStatus.Active;

                    // Set Date of Subscription Bookings
                    _subscriptionBookings.StartDate = subscription.CurrentPeriodStart;
                    _subscriptionBookings.EndDate = subscription.CurrentPeriodEnd;
                    _subscriptionBookings.LastUpdatedDate = DateTime.UtcNow;
                    _subscriptionBookings.LastUpdatedBy = 0;
                    break;
                case "canceled":
                    _subscriptionBookings.Status = (int)Enums.SubscriptionBookingStatus.End;
                    bookingRepository.ExpiredBookingByCancelledSubscription(PublicDiscounts.Id);
                    break;
                default: //trialing
                    break;
            }

            _subscriptionBookings.Description = subscription.StripeResponse.ObjectJson;

            _subscriptionBookingRepository.Update(_subscriptionBookings);

            NextCycleLit.Visible = true;
            cancelMembershipLink.Visible = true;
            reactiveMembershipLink.Visible = false;
            ErrorMessageLabel.Visible = true;
            ErrorMessageLabel.Text = Message.SubscriptionReActive;
            ErrorMessageLabel.CssClass = "success-message";
        }
    }
}