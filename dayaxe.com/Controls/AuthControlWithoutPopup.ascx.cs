using System;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.UI;
using Dayaxe.SendEmail;
using DayaxeDal;
using DayaxeDal.Repositories;

namespace dayaxe.com.Controls
{
    public partial class AuthControlWithoutPopup : UserControl
    {
        readonly CustomerInfoRepository _customerInfoRepository = new CustomerInfoRepository();

        protected void Page_Init(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var sp = Request.Params["sp"];
                if (sp != null 
                    && (HttpContext.Current.Session["UserSession"] == null 
                        || string.Equals(sp,(string)HttpContext.Current.Session["UserSession"], StringComparison.OrdinalIgnoreCase)))
                {
                    var sessionChangePassword = _customerInfoRepository.GetCustomerInfoBySessionChangePasswordId(sp);
                    if (sessionChangePassword != null)
                    {
                        HttpContext.Current.Session["UserSession"] = sessionChangePassword.ChangePasswordSessionId;
                        AuthMultiView.ActiveViewIndex = 3;
                    }
                }
                NewPassText.Text = string.Empty;
                ConfirmPasswordText.Text = string.Empty;
            }
            UpdatePanel1.Attributes["class"] = "model-max-w-h";
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            
        }

        protected void GetVipAccessClick(object sender, EventArgs e)
        {
            AuthMultiView.ActiveViewIndex = 0;
            if (!Helper.IsValidEmail(YourEmail.Text.Replace(" ", "").Trim()))
            {
                MessageLabel.Text = "Please provide valid email";
                wrapmessagelabel.Attributes["style"] = "line-height:30px;";
                return;
            }

            string searchPage = !string.IsNullOrEmpty((string)Session["SearchPage"])
                ? Session["SearchPage"].ToString()
                : Constant.SearchPageDefault;

            if (Page.RouteData.Values["market"] != null)
            {
                searchPage = string.Format("/s/{0}/day-passes", Page.RouteData.Values["market"]);
            }

            var response = _customerInfoRepository.GetVipAccess(YourEmail.Text.Replace(" ", ""), searchPage);
            if (!response.IsSuccessful)
            {
                wrapmessagelabel.Attributes["style"] = "line-height:16px;";
                MessageLabel.Text = response.Message.First();
                return;
            }

            CacheLayer.Clear(CacheKeys.CustomerInfosCacheKey);
            CacheLayer.Clear(CacheKeys.CustomerCreditsCacheKey);

            var responseEmail = EmailHelper.EmailNewAccount(YourEmail.Text.Replace(" ", ""), 
                string.Empty,
                response.Password,
                Helper.ResolveRelativeToAbsoluteUrl(Request.Url,
                    string.Format("{0}?sp={1}",
                        searchPage,
                        response.PasswordKey)), // Reset Password Url
                Helper.ResolveRelativeToAbsoluteUrl(Request.Url,
                    string.Format("{0}?c={1}",
                        searchPage,
                        response.AccessKey))); // Browse Day Pass

            //ScriptManager.RegisterClientScriptBlock(this.UpdatePanel1, 
            //    typeof(string), 
            //    "setSession", 
            //    string.Format("window.userSession = '{0}'; window.register = 'true'; window.useremail = '{1}'; window.referralcode = '{2}';", 
            //        response.AccessKey, 
            //        YourEmail.Text.Trim().Replace(" ", ""),
            //        response.ReferralCode), 
            //    true);
            HttpContext.Current.Session["IsRegister"] = true;
            HttpContext.Current.Session["UserSession"] = response.AccessKey;
            HttpContext.Current.Session["ReferralCode"] = response.ReferralCode;

            RedirectWithReturnUrl();
        }

        protected void SignInLinkClick(object sender, EventArgs e)
        {
            AuthMultiView.ActiveViewIndex = 1;
        }

        protected void ForgotPasswordLinkClick(object sender, EventArgs e)
        {
            MessageErrorForgotlabel.Text = string.Empty;
            MessageErrorForgotlabel.Attributes["style"] = string.Empty;
            AuthMultiView.ActiveViewIndex = 2;
        }

        protected void SignUpLinkClick(object sender, EventArgs e)
        {
            AuthMultiView.ActiveViewIndex = 0;
        }

        protected void SignInButtonClick(object sender, EventArgs e)
        {
            AuthMultiView.ActiveViewIndex = 1;
            MessageSignInLabel.Text = string.Empty;
            var response = _customerInfoRepository.SignIn(EmailText.Text, PasswordText.Text);

            if (!response.IsSuccessful)
            {
                MessageSignInLabel.Text = response.Message.First();
                return;
            }

            CacheLayer.Clear(CacheKeys.CustomerInfosCacheKey);

            ScriptManager.RegisterClientScriptBlock(this.UpdatePanel1, typeof(string), "setSession", string.Format("window.userSession = '{0}'; window.useremail = '{1}';", response.AccessKey, EmailText.Text), true);
            if (!string.IsNullOrEmpty(response.SearchPageUrl) && HttpContext.Current.Session["SearchPage"] == null)
            {
                HttpContext.Current.Session["SearchPage"] = response.SearchPageUrl;
            }

            HttpContext.Current.Session["UserSession"] = response.AccessKey;

            RedirectWithReturnUrl();
        }

        protected void ChangePasswordButtonClick(object sender, EventArgs e)
        {
            AuthMultiView.ActiveViewIndex = 3;
            if (string.IsNullOrEmpty(NewPassText.Text) || string.IsNullOrEmpty(ConfirmPasswordText.Text) ||
                NewPassText.Text != ConfirmPasswordText.Text)
            {
                MessageChangePasswordLabel.Text = "Please enter new valid password";
                return;
            }
            var accessKey = Request.Params["sp"];
            var response = _customerInfoRepository.ChangePassword(accessKey, NewPassText.Text);
        
            if (response.IsSuccessful)
            {
                CacheLayer.Clear(CacheKeys.CustomerInfosCacheKey);
                AuthMultiView.ActiveViewIndex = 1;
                MessageSignInLabel.Text = "Your password has been changed, sign in please.";
                MessageSignInLabel.Attributes["style"] = "color:green !important;";
                var responseEmail = EmailHelper.EmailNewPasswordchange(response.Message.FirstOrDefault(), response.Message.LastOrDefault());
            }
            else
            {
                MessageChangePasswordLabel.Text = response.Message.First();
            }
        }

        protected void GetPasswordButtonClick(object sender, EventArgs e)
        {
            AuthMultiView.ActiveViewIndex = 2;
            if (GetPasswordButton.Text == "Done")
            {
                GetPasswordButton.Text = "GET PASSWORD";
                AuthMultiView.ActiveViewIndex = 1;
            }
            else
            {
                var response = _customerInfoRepository.ForgotPassword(EmailForgotText.Text);
                if (response.IsSuccessful)
                {
                    var url = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Path);
                    url = string.Format("{0}?sp={1}", url, response.AccessKey);
                    if (Request.Params["ReturnUrl"] != null)
                    {
                        url = string.Format("{0}&ReturnUrl={1}", url, HttpUtility.UrlEncode(Request.Params["ReturnUrl"]));
                    }
                    var responseEmail = EmailHelper.EmailForgotPassword(EmailForgotText.Text.Trim().Replace(" ", ""),
                        response.Message.FirstOrDefault(),
                        url);
                    MessageErrorForgotlabel.Text = "We have sent an email to reset your password, please check your email";
                    MessageErrorForgotlabel.Attributes["style"] = "color:green !important;";
                    GetPasswordButton.Text = "Done";
                    CacheLayer.Clear(CacheKeys.CustomerInfosCacheKey);
                }
                else
                {
                    MessageErrorForgotlabel.Text = response.Message.First();
                    MessageErrorForgotlabel.Attributes["style"] = string.Empty;
                }
            }
        }

        private void RedirectWithReturnUrl()
        {
            if (Request.Params["ReturnUrl"] != null)
            {
                Response.Redirect(HttpUtility.UrlDecode(Request.Params["ReturnUrl"]));
            }
            Response.Redirect(Constant.SearchPageDefault);
        }

        protected void CancelChangePasswordButtonOnClick(object sender, EventArgs e)
        {
            AuthMultiView.ActiveViewIndex = 1;
        }

        protected void RememberPasswordLinkClick(object sender, EventArgs e)
        {
            AuthMultiView.ActiveViewIndex = 1;
        }
    }
}