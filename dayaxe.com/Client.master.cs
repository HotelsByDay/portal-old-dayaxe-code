using System;
using System.Net;
using System.Web;
using System.Web.UI;
using DayaxeDal;
using DayaxeDal.Repositories;
using DayaxeDal.Ultility;

namespace dayaxe.com
{
    public partial class Client : MasterPage
    {
        private CustomerInfos _customerInfos;
        protected string RequestUrl = string.Empty;
        protected void Page_Init(object sender, EventArgs e)
        {
            ScriptAnalyticsHeader.Visible = AppConfiguration.EnableTracking;
            if (Session["IsLogOut"] != null)
            {
                Session["IsLogOut"] = null;
                Page.ClientScript.RegisterClientScriptBlock(GetType(), "resetMixpanel", "if (window.mixpanel) { window.mixpanel.reset(); }", true);
            }
            RequestUrl = Request.Url.AbsoluteUri.ToLower();
            if (RequestUrl.Contains("reviews"))
            {
                body.Attributes["class"] += " reviews-page";
            }
            if (RequestUrl.Contains("membership"))
            {
                body.Attributes["class"] += " membership-page";
            }

            if (RequestUrl.Contains("credits"))
            {
                body.Attributes["class"] += " credits-page";
            }

            var createSession = Request.Params["c"];
            if (Session["UserSession"] == null && createSession != null)
            {
                Session["UserSession"] = createSession;
            }

            partnerLink.HRef = AppConfiguration.ForPartnerLink;

            var currentSession = Session["UserSession"];
            if (currentSession != null)
            {
                using (var customerInfoRepository = new CustomerInfoRepository())
                {
                    _customerInfos = customerInfoRepository.GetCustomerInfoBySessionId(currentSession.ToString());
                    if (_customerInfos != null)
                    {
                        string firstName = string.IsNullOrEmpty(_customerInfos.FirstName)
                            ? _customerInfos.EmailAddress.Split('@')[0]
                            : _customerInfos.FirstName;
                        FirstnameLiteral.Text = firstName;
                        FirstnameMobileLiteral.Text = firstName;
                        LoginLinkButton.Attributes["class"] += " hidden-sm hidden-xs";
                        LoginLinkButton.Attributes["onclick"] = "";
                        if (Request.Browser["IsMobileDevice"] == "true")
                        {
                            LoginLinkButton.Visible = false;
                        }
                        hiw.Visible = false;
                        mdp.Visible = true;
                        MyAccount.Visible = true;
                        if (_customerInfos.IsAdmin)
                        {
                            partnerLink.InnerText = "Host";
                            partnerLink.HRef = String.Format("{0}?e={1}",
                                AppConfiguration.HostLink,
                                HttpUtility.UrlEncode(Algoritma.Encrypt(_customerInfos.EmailAddress, Constant.EncryptPassword)));
                            partnerLink.Attributes["class"] = "is-host";
                        }
                    }
                }
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (!string.IsNullOrEmpty((string)Session["SearchPage"]))
                {
                    SearchLink.NavigateUrl = Session["SearchPage"].ToString();
                }
            }

            if (RequestUrl.Contains("credits")) // Credits
            {
                cr.Attributes["class"] += string.Format("{0} {1}", cr.Attributes["class"], "selected");
            }
            else if (RequestUrl.Contains("membership")) // Membership
            {
                mbs.Attributes["class"] += string.Format("{0} {1}", mbs.Attributes["class"], "selected");
            }
            else if (RequestUrl.Contains("my-account")) // My Account
            {
                mya.Attributes["class"] += string.Format("{0} {1}", mya.Attributes["class"], "selected");
            }
        }

        protected void LogoutClick(object sender, EventArgs e)
        {
            string searchPage = !string.IsNullOrEmpty((string)Session["SearchPage"])
                ? Session["SearchPage"].ToString()
                : Constant.SearchPageDefault;
            Session.Abandon();
            Session["IsLogOut"] = true;
            Response.Redirect(searchPage.Split('?')[0]);
        }
    }
}
