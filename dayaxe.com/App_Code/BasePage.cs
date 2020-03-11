using System;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.UI;
using DayaxeDal;
using DayaxeDal.Repositories;

namespace dayaxe.com
{
    /// <summary>
    /// Summary description for BasePage
    /// </summary>
    public class BasePage : Page
    {
        protected CustomerInfos PublicCustomerInfos { get; set; }
        protected Discounts PublicDiscounts { get; set; }
        protected CustomerInfoSearchCriteria PublicCustomerSearch { get; set; }
        protected int PublicSubscriptionId { get; set; }

        private readonly CustomerInfoRepository _customerInfoRepository = new CustomerInfoRepository();

        protected override void OnInit(EventArgs e)
        {
            if (!Request.IsLocal && !Request.IsSecureConnection)
            {
                string redirectUrl = Request.Url.ToString().Replace("http:", "https:");
                Response.Redirect(redirectUrl, false);
                HttpContext.Current.ApplicationInstance.CompleteRequest();
            }

            if (Session["UserSession"] != null)
            {
                PublicCustomerInfos = _customerInfoRepository.GetCustomerInfoBySessionId(Session["UserSession"].ToString());
            }

            if (PublicCustomerInfos != null)
            {
                PublicCustomerSearch =
                    _customerInfoRepository.CustomerInfoSearchCriteriaList.FirstOrDefault(
                        x => x.CustomerId == PublicCustomerInfos.CustomerId);

                PublicDiscounts = _customerInfoRepository.GetSubscriptionDiscount(PublicCustomerInfos.CustomerId);
                if (PublicDiscounts != null)
                {
                    var subscription = (from sb in _customerInfoRepository.SubscriptionBookingsList
                                        join sdu in _customerInfoRepository.SubscriptionDiscountUsedList on sb.Id equals sdu
                                            .SubscriptionBookingId
                                        join s in _customerInfoRepository.SubscriptionsList on sb.SubscriptionId equals s.Id
                                        where sdu.DiscountId == PublicDiscounts.Id
                                        select s).FirstOrDefault();
                    if (subscription != null)
                    {
                        PublicSubscriptionId = subscription.Id;
                    }
                }
            }

            if ((PublicCustomerInfos != null && string.IsNullOrEmpty(PublicCustomerInfos.EmailAddress)) || 
                (Session["UserSession"] != null && PublicCustomerInfos == null))
            {
                PublicCustomerInfos = null;
                Session.Remove("UserSession");
                Session.Remove("ReferralCode");
            }

            // 
            if (AppConfiguration.IsStageSite && (Page.RouteData.Values["urlSegment"] == null || 
                (Page.RouteData.Values["urlSegment"] != null && 
                    !Page.RouteData.Values["urlSegment"].ToString().Contains("signup") && 
                    !Page.RouteData.Values["urlSegment"].ToString().Contains("signin"))) && 
                (PublicCustomerInfos == null || !PublicCustomerInfos.IsActive))
            { 
                Response.Redirect(string.Format(Constant.SignIpPage, HttpUtility.UrlEncode(Request.Url.PathAndQuery)));
            }

            base.OnInit(e);
        }

        protected override void OnPreRender(EventArgs e)
        {
            //base.OnPreRender(e);
            //Page.Controls.AddAt(0, new LiteralControl(
            //    String.Format("<meta http-equiv='refresh' content='{0};url={1}'>",
            //     Session.Timeout * 60, Constant.SearchPage)));
        }
    }
}