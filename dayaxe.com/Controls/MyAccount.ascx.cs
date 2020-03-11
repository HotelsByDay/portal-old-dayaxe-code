using System;
using System.Web.UI;
using DayaxeDal;
using DayaxeDal.Repositories;

namespace dayaxe.com.Controls
{
    public partial class MyAccount : UserControl
    {
        private CustomerInfos _customerInfo;
        private readonly CustomerInfoRepository _customerInfoRepository = new CustomerInfoRepository();

        protected void Page_Init(object sender, EventArgs e)
        {
            _customerInfo = _customerInfoRepository.GetCustomerInfoBySessionId(Session["UserSession"].ToString());
            if (!IsPostBack)
            {
                if (_customerInfo != null)
                {
                    FullNameHeader.InnerText = string.Format("{0} {1}", _customerInfo.FirstName, _customerInfo.LastName);

                    FirstNameText.Text = _customerInfo.FirstName;
                    LastNameText.Text = _customerInfo.LastName;
                    EmailAddressText.Text = _customerInfo.EmailAddress;
                }
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            //if (IsPostBack)
            //{
            //    changePass.Visible = !string.IsNullOrEmpty(NewPasswordText.Text);
            //}
        }

        protected void SaveButton_OnClick(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(FirstNameText.Text) || string.IsNullOrEmpty(LastNameText.Text))
            {
                MessageLabel.Visible = true;
                MessageLabel.CssClass = "error-message";
                MessageLabel.Text = ErrorMessage.FailedToUpdated;
                return;
            }

            if(NewPasswordText.Text.Length > 0)
            {
                if (NewPasswordText.Text.Length < 7)
                {
                    MessageLabel.Visible = true;
                    MessageLabel.Text = ErrorMessage.PasswordNotValid;
                    return;
                }
                if (!string.Equals(OldPasswordText.Text, _customerInfo.Password, StringComparison.Ordinal))
                {
                    MessageLabel.Visible = true;
                    MessageLabel.Text = ErrorMessage.OldPasswordNotValid;
                    return;
                }
                _customerInfo.Password = NewPasswordText.Text;
            }

            _customerInfo.FirstName = FirstNameText.Text.Trim();
            _customerInfo.LastName = LastNameText.Text.Trim();
            _customerInfoRepository.Update(_customerInfo);

            MessageLabel.Visible = true;
            FullNameHeader.InnerText = string.Format("{0} {1}", _customerInfo.FirstName, _customerInfo.LastName);
            MessageLabel.Text = ErrorMessage.SuccessfullyToUpdated;
            MessageLabel.CssClass = "success-message";
            CacheLayer.Clear(CacheKeys.CustomerInfosCacheKey);
        }
    }
}