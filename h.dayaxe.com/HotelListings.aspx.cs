using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using DayaxeDal;
using DayaxeDal.Custom;
using DayaxeDal.Extensions;
using DayaxeDal.Repositories;
using Newtonsoft.Json;

namespace h.dayaxe.com
{
    public partial class HotelListings : BasePageHotel
    {
        protected bool IsAdmin { get; set; }

        private readonly HotelRepository _hotelRepository = new HotelRepository();

        protected void Page_Init(object sender, EventArgs e)
        {
            Session["Active"] = "HotelListing";
            IsAdmin = PublicCustomerInfos != null && PublicCustomerInfos.IsSuperAdmin;
            if (PublicCustomerInfos != null && PublicCustomerInfos.IsSuperAdmin)
            {
                AddNewRow.Visible = true;
            }
            if (!IsPostBack)
            {
                Session["Hotel"] = null;
                Session["CurrentPage"] = 1;
                List<Hotels> hotels = _hotelRepository.SearchHotelsByUser(PublicCustomerInfos != null ? PublicCustomerInfos.EmailAddress : string.Empty)
                    .Take(Constant.ItemPerPage)
                    .ToList();

                //UpdateHotelTimeZone(hotels);
                RptHotelListings.DataSource = hotels;
                RptHotelListings.DataBind();

                CacheLayer.Clear(CacheKeys.BookingsCacheKey);
                CacheLayer.Clear(CacheKeys.CustomerInfosCacheKey);
                CacheLayer.Clear(CacheKeys.DiscountsCacheKey);
                CacheLayer.Clear(CacheKeys.DiscountBookingsCacheKey);
                CacheLayer.Clear(CacheKeys.SurveysCacheKey);
                CacheLayer.Clear(CacheKeys.CustomerCreditsCacheKey);
                CacheLayer.Clear(CacheKeys.CustomerCreditLogsCacheKey);
                CacheLayer.Clear(CacheKeys.InvoicesCacheKey);
                CacheLayer.Clear(CacheKeys.GiftCardCacheKey);
                CacheLayer.Clear(CacheKeys.BookingHistoriesCacheKey);

                CacheLayer.Clear(CacheKeys.SubscriptionBookingsCacheKey);
                CacheLayer.Clear(CacheKeys.SubsciptionDiscountUsedCacheKey);
                CacheLayer.Clear(CacheKeys.SubscriptionDiscountsCacheKey);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
        
        }

        protected void Previous_OnClick(object sender, EventArgs e)
        {
            int currentPage = int.Parse(Session["CurrentPage"].ToString());
            var hotels = _hotelRepository.SearchHotelsByUser(PublicCustomerInfos.EmailAddress)
                .Skip((currentPage - 2) * Constant.ItemPerPage)
                .Take(Constant.ItemPerPage)
                .ToList();
            if (hotels.Any() && currentPage -2 >= 0)
            {
                Session["CurrentPage"] = currentPage - 1;
                RptHotelListings.DataSource = hotels;
                RptHotelListings.DataBind();
            }
        }

        protected void Next_OnClick(object sender, EventArgs e)
        {
            int currentPage = int.Parse(Session["CurrentPage"].ToString());
            var hotels = _hotelRepository.SearchHotelsByUser(PublicCustomerInfos.EmailAddress)
                .Skip(currentPage * Constant.ItemPerPage)
                .Take(Constant.ItemPerPage)
                .ToList();
            if (hotels.Any())
            {
                //UpdateHotelTimeZone(hotels);
                Session["CurrentPage"] = currentPage + 1;
                RptHotelListings.DataSource = hotels;
                RptHotelListings.DataBind();
            }
        }

        protected void RptHotelListings_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var rowHistory = (HtmlTableRow)e.Item.FindControl("rowHotel");
                rowHistory.Attributes.Add("class", rowHistory.Attributes["class"] + " alternative");
            }

            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var hotel = (Hotels) e.Item.DataItem;
                var url = string.Format("/Revenues.aspx?hotelId={0}", hotel.HotelId);
                if (PublicCustomerInfos.IsCheckInOnly)
                {
                    url = string.Format("/BookingPage.aspx?hotelId={0}", hotel.HotelId);
                }
                var rowHistory = (HtmlTableRow)e.Item.FindControl("rowHotel");
                rowHistory.Attributes.Add("data-href", url);
            }

            if (e.Item.ItemType == ListItemType.Footer)
            {
                var litPage = (Literal)e.Item.FindControl("LitPage");
                var litTotal = (Literal)e.Item.FindControl("LitTotal");
                var totalHotel = _hotelRepository.SearchHotelsByUser(PublicCustomerInfos.EmailAddress).Count;
                var totalPage = totalHotel/Constant.ItemPerPage + (totalHotel%Constant.ItemPerPage != 0 ? 1 : 0);
                litPage.Text = string.Format("Page {0} of {1}", Session["CurrentPage"], totalPage);
                litTotal.Text = totalHotel + " Listings";
            }
        }

        private void UpdateHotelTimeZone(List<Hotels> hotels)
        {
            // Update Hotel TimeZone by Location
            hotels.ForEach(hotel =>
            {
                var url = string.Format(Constant.GoogleTimeZoneUrl,
                    hotel.Latitude,
                    hotel.Longitude,
                    DateTime.UtcNow.GetTimestamp(),
                    AppConfiguration.GoogleApiKey);
                var response = Helper.Get(url);
                var responseObj = JsonConvert.DeserializeObject<GoogleTimeZoneObject>(response);
                hotel.TimeZoneId = Constant.UsDefaultTime;
                hotel.TimeZoneOffset = Constant.LosAngelesTimeWithUtc;
                if (responseObj != null)
                {
                    hotel.TimeZoneId = responseObj.TimeZoneId;
                    hotel.TimeZoneOffset = (Int16)(responseObj.RawOffset / 3600);
                    //if (hotel.TimeZoneId == Constant.DefaultLosAngelesTimezoneId)
                    //{
                    //    var currentDate = DateTime.UtcNow;
                    //    if ((currentDate.Month < 11 || currentDate.Month == 11 && currentDate.Day < 5) &&
                    //        (currentDate.Month > 3 || currentDate.Month == 3 && currentDate.Day > 11))
                    //    {
                    //        hotel.TimeZoneOffset = Constant.LosAngelesSummerTimeWithUtc;
                    //    }
                    //    else
                    //    {
                    //        hotel.TimeZoneOffset = Constant.LosAngelesTimeWithUtc;
                    //    }
                    //}
                }
            });
            _hotelRepository.Update(hotels);
        }
    }
}