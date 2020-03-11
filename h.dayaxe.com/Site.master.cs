using System;
using System.Web.UI;
using DayaxeDal;
using DayaxeDal.Repositories;
using Newtonsoft.Json;

namespace h.dayaxe.com
{
    public partial class SiteMaster : MasterPage
    {
        protected string UploadUrl = string.Empty;
        private Hotels _hotel;
        private CustomerInfos _users;
        private readonly HotelRepository _hotelRepository = new HotelRepository();

        protected void Page_Init(object sender, EventArgs e)
        {
            string dayaxeUrl = Request.Url.AbsoluteUri;
            Uri baseUri = new Uri(dayaxeUrl);
            UploadUrl = new Uri(baseUri, "/Handler/Upload.ashx").AbsoluteUri;
            if (Request.Params["id"] != null)
            {
                UploadUrl += "?id=" + Request.Params["id"];
            }
            string sessionUser = Session["CurrentUser"] != null ? Session["CurrentUser"].ToString() : string.Empty;
            _users = JsonConvert.DeserializeObject<CustomerInfos>(sessionUser);
            if (_users != null)
            {
                UploadUrl += "&userName=" + _users.EmailAddress;
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            var active = Session["Active"];
            if (active != null)
            {
                switch (active.ToString())
                {
                    case "Calendar":
                        Calendar.Attributes.Add("class", "active");
                        break;
                    case "Booking":
                        Booking.Attributes.Add("class", "active");
                        break;
                    case "SalesReport":
                        SalesReport.Attributes.Add("class", "active");
                        break;
                    case "Revenue":
                        Revenue.Attributes.Add("class", "active");
                        break;
                    case "CustomerInsight":
                        CustomerInsight.Attributes.Add("class", "active");
                        break;
                    case "Iventory":
                        Iventory.Attributes.Add("class", "active");
                        break;
                    case "TargetAudienceObject":
                        TargetAudience.Attributes.Add("class", "active");
                        break;
                    case "HotelListing":
                        HotelListing.Attributes.Add("class", "active");
                        break;
                    case "Settings":
                        Settings.Attributes.Add("class", "active");
                        break;
                    case "ProductListing":
                        ProductPage.Attributes.Add("class", "active");
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
            string sessionHotel = Session["Hotel"] != null ? Session["Hotel"].ToString() : string.Empty;
            _hotel = _hotelRepository.GetById(int.Parse(sessionHotel));
            if (sessionHotel != "0" && _hotel == null)
            {
                Response.Redirect(Constant.DefaultPage);
            }

            if (_hotel != null)
            {
                hotelName.InnerText = _hotel.HotelName;
                if (_hotel.IsMenuVisible)
                {
                    AnalyticsMenu.Visible = true;
                    ControlMenu.Visible = true;
                }
            }
            if (_users != null)
            {
                if (_users.IsSuperAdmin)
                {
                    UserHotel.Visible = true;
                    Markets.Visible = true;
                    ContentsPage.Visible = true;
                    PromoList.Visible = true;
                    ProductPage.Visible = true;
                    SearchBookingsList.Visible = true;
                    GiftCardList.Visible = true;
                    SubscriptionList.Visible = true;
                    EditPolicy.Visible = true;
                }
                if (_users.IsCheckInOnly)
                {
                    Revenue.Visible = false;
                    SalesReport.Visible = false;
                    CustomerInsight.Visible = false;
                    ControlMenu.Visible = false;
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