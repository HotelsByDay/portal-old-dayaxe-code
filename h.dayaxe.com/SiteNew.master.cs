using System;
using DayaxeDal;
using Newtonsoft.Json;

namespace h.dayaxe.com
{
    public partial class SiteNew : System.Web.UI.MasterPage
    {
        private CustomerInfos _users;
        public string UpdateUrl = string.Empty;

        protected void Page_Init(object sender, EventArgs e)
        {
            string sessionUser = Session["CurrentUser"] != null ? Session["CurrentUser"].ToString() : string.Empty;
            _users = JsonConvert.DeserializeObject<CustomerInfos>(sessionUser);
            if (_users != null)
            {
                if (_users.IsSuperAdmin)
                {
                    UserHotel.Visible = true;
                    Markets.Visible = true;
                    ContentsPage.Visible = true;
                    PromoList.Visible = true;
                    SearchBookingsList.Visible = true;
                    GiftCardList.Visible = true;
                    SubscriptionList.Visible = true;
                    LogList.Visible = true;
                    EditPolicy.Visible = true;
                }
                string dayaxeUrl = Request.Url.AbsoluteUri;
                Uri baseUri = new Uri(dayaxeUrl);
                UpdateUrl = new Uri(baseUri, "/Handler/UpdateHotel.ashx").AbsoluteUri;
                UpdateUrl += "?userName=" + _users.EmailAddress;
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            var active = Session["Active"];
            if (active != null)
            {
                switch (active.ToString())
                {
                    case "HotelListing":
                        HotelListing.Attributes.Add("class", "active");
                        break;
                    case "UserHotel":
                        UserHotel.Attributes.Add("class", "active");
                        break;
                    case "Markets":
                        Markets.Attributes.Add("class", "active");
                        break;
                    case "ContentsPage":
                        ContentsPage.Attributes.Add("class", "active");
                        break;
                    case "DiscountsPage":
                        PromoList.Attributes.Add("class", "active");
                        break;
                    case "SearchBookingsList":
                        SearchBookingsList.Attributes.Add("class", "active");
                        break;
                    case "GiftCardList":
                        GiftCardList.Attributes.Add("class", "active");
                        break;
                    case "SubscriptionList":
                        SubscriptionList.Attributes.Add("class", "active");
                        break;
                    case "LogList":
                        LogList.Attributes.Add("class", "active");
                        break;
                    case "EditPolicy":
                        EditPolicy.Attributes.Add("class", "active");
                        break;
                }
            }
        }

        protected void LogoutLink_Click(object sender, EventArgs e)
        {
            Session.Abandon();
            Application.RemoveAll();
            Response.Redirect(Constant.DefaultPage);
        }
    }
}
