using Dayaxe.SendEmail;
using DayaxeDal;
using DayaxeDal.Repositories;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class ResetPassword : System.Web.UI.Page
{
    readonly CustomerInfoRepository _customerInfoRepository = new CustomerInfoRepository();

    #region Methods

    private void BindData()
    {
        var q = Request.Params["q"];
        if (q != null
            && (HttpContext.Current.Session["UserSession"] == null
                || string.Equals(q, (string)HttpContext.Current.Session["UserSession"], StringComparison.OrdinalIgnoreCase)))
        {
            var sessionChangePassword = _customerInfoRepository.GetCustomerInfoBySessionChangePasswordId(q);
            if (sessionChangePassword != null)
            {
                HttpContext.Current.Session["UserSession"] = sessionChangePassword.ChangePasswordSessionId;

                phRemindPassword.Visible = false;
                phResetPassword.Visible = true;
            }
        }
    }

    private void RemindPassword()
    {
        var response = _customerInfoRepository.ForgotPassword(tbEmail.Text);
        if (response.IsSuccessful)
        {
            var url = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Path);
            url = string.Format("{0}?q={1}", url, response.AccessKey);
            if (Request.Params["ReturnUrl"] != null)
            {
                url = string.Format("{0}&ReturnUrl={1}", url, HttpUtility.UrlEncode(Request.Params["ReturnUrl"]));
            }
            var responseEmail = EmailHelper.EmailForgotPassword(tbEmail.Text.Trim().Replace(" ", string.Empty),
                response.Message.FirstOrDefault(),
                url);
            lblMessage.Text = "We have sent an email to reset your password, please check your email";
            lblMessage.Attributes["style"] = "color:green !important;";
            btnGetPassword.Enabled = false;
            CacheLayer.Clear(CacheKeys.CustomerInfosCacheKey);
        }
    }

    private void UpdatePassword()
    {
        if (string.IsNullOrWhiteSpace(tbNewPassword.Text) || string.IsNullOrWhiteSpace(tbConfirmPassword.Text) ||
                tbNewPassword.Text != tbConfirmPassword.Text)
        {
            lblMessageResetPassword.Text = "Please enter new valid password";
            return;
        }
        var accessKey = Request.Params["q"];
        var response = _customerInfoRepository.ChangePassword(accessKey, tbNewPassword.Text);

        if (response.IsSuccessful)
        {
            CacheLayer.Clear(CacheKeys.CustomerInfosCacheKey);
            var responseEmail = EmailHelper.EmailNewPasswordchange(response.Message.FirstOrDefault(), response.Message.LastOrDefault());
            lblMessageResetPassword.Text = "Your password has been changed, sign in please.";
            lblMessageResetPassword.Attributes["style"] = "color:green !important;";
        }
        else
        {
            lblMessageResetPassword.Text = response.Message.First();
        }
    }

    #endregion

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            BindData();
        }
    }

    protected void btnGetPassword_Click(object sender, EventArgs e)
    {
        using (var client = new WebClient())
        {
            var values = new NameValueCollection();
            values["email"] = tbEmail.Text.Trim().Replace(" ", string.Empty);

            string newBackend = ConfigurationManager.AppSettings["newBackend"];
            var response = client.UploadValues(newBackend + "/api/v1/users/reset_password", values);

            var responseString = Encoding.Default.GetString(response);

            lblMessage.Text = "We have sent an email to reset your password, please check your email";
            lblMessage.Attributes["style"] = "color:green !important;";
            btnGetPassword.Enabled = false;
        }
    }

    protected void btnSubmitResetPassword_Click(object sender, EventArgs e)
    {
       // UpdatePassword();
    }
}