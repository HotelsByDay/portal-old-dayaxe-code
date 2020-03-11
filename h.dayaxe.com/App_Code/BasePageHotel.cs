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
    public class BasePageHotel : Page
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
                if (!string.IsNullOrEmpty(sessionUser))
                {
                    PublicCustomerInfos = JsonConvert.DeserializeObject<CustomerInfos>(sessionUser);
                }
                if (!string.IsNullOrEmpty(sessionHotel) && !Request.Url.AbsoluteUri.Contains(Constant.HotelList))
                {
                    int hotelId = JsonConvert.DeserializeObject<int>(sessionHotel);
                    PublicHotel = _hotelRepository.GetById(hotelId);
                }
            }

            if (PublicCustomerInfos == null || !PublicCustomerInfos.IsActive)
            {
                Response.Redirect(string.Format(Constant.SignIpPageAdmin, HttpUtility.UrlEncode(Request.Url.PathAndQuery)));
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