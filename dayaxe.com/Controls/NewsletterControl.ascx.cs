using System;
using System.Web.UI;
using DayaxeDal;

namespace dayaxe.com.Controls
{
    public partial class NewsletterControl : UserControl
    {
        private readonly DalHelper _helper = new DalHelper();
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void SubmitButton_OnClick(object sender, EventArgs e)
        {
            //var email = EmailNewsletter.Text.Trim().ToLower();
            //var url = string.Format(Constant.KlaviyoListApiUrl, AppConfiguration.KlaviyoNewsletterListId);
            //var addToListKlaviyoRes = Helper.Post(url, email);

            //var newsletter = new Newsletters
            //{
            //    Email = email,
            //    CreatedDate = DateTime.UtcNow
            //};

            //var id = _helper.AddNewsletterEmail(newsletter);

            //var logs = new Logs
            //{
            //    LogKey = "Klaviyo_Newsletter_Response",
            //    UpdatedBy = (int)id,
            //    UpdatedDate = DateTime.UtcNow,
            //    UpdatedContent = string.Format("{0} - {1}", email, addToListKlaviyoRes)
            //};

            //_helper.AddLog(logs);
        }
    }
}