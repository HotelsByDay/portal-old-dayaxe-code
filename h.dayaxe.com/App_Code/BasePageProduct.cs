using System;
using System.Net;
using System.Web;
using System.Web.UI;
using DayaxeDal;
using DayaxeDal.Repositories;
using Newtonsoft.Json;

namespace h.dayaxe.com
{
    /// <summary>
    /// Summary description for BasePage
    /// </summary>
    public class BasePageProduct : Page
    {
        protected CustomerInfos PublicCustomerInfos { get; set; }

        protected Hotels PublicHotel { get; set; }
        
        private readonly HotelRepository _hotelRepository = new HotelRepository();

        protected override void OnInit(EventArgs e)
        {
            if (!Request.IsLocal && !Request.IsSecureConnection)
            {
                string redirectUrl = Request.Url.ToString().Replace("http:", "https:");
                Response.Redirect(redirectUrl, false);
                HttpContext.Current.ApplicationInstance.CompleteRequest();
            }
            if (Context.Session != null)
            {
                string sessionUser = Session["CurrentUser"] != null ? Session["CurrentUser"].ToString() : string.Empty;
                string sessionHotel = Session["Hotel"] != null ? Session["Hotel"].ToString() : string.Empty;
                int hotelId;

                if (!string.IsNullOrEmpty(sessionUser))
                {
                    PublicCustomerInfos = JsonConvert.DeserializeObject<CustomerInfos>(sessionUser);
                }

                if (!string.IsNullOrEmpty(sessionHotel) && !Request.Url.AbsoluteUri.Contains(Constant.HotelList))
                {
                    hotelId = JsonConvert.DeserializeObject<int>(sessionHotel);
                    PublicHotel = _hotelRepository.GetById(hotelId);
                }

                if (Request.Params["hotelId"] != null)
                {
                    int.TryParse(Request.Params["hotelId"], out hotelId);
                    PublicHotel = _hotelRepository.GetHotel(hotelId, PublicCustomerInfos != null ? PublicCustomerInfos.EmailAddress : string.Empty);
                    if (PublicHotel != null)
                    {
                        Session["Hotel"] = PublicHotel.HotelId;
                    }
                }
            }

            if (PublicCustomerInfos == null || !PublicCustomerInfos.IsActive)
            {
                Response.Redirect(string.Format(Constant.SignIpPageAdmin, HttpUtility.UrlEncode(Request.Url.PathAndQuery)));
            }

            if (PublicHotel == null || (PublicCustomerInfos != null && PublicCustomerInfos.IsCheckInOnly && !Request.Url.AbsoluteUri.Contains("BookingPage.aspx")))
            {
                Response.Redirect(string.Format(Constant.HotelList));
            }
            base.OnInit(e);
        }

        protected override void OnPreRender(EventArgs e)
        {
            //if (!IsPostBack)
            //{
            //    base.OnPreRender(e);
            //    Page.Controls.AddAt(0, new LiteralControl(
            //    String.Format("<meta http-equiv='refresh' content='{0};url={1}'>",
            //     Session.Timeout * 60, Constant.DefaultPage)));
            //}
        }
    }
}