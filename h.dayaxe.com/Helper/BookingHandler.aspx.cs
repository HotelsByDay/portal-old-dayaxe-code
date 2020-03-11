using System;
using System.Linq;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using DayaxeDal;
using DayaxeDal.Repositories;

namespace h.dayaxe.com
{
    public partial class BookingHandler : BasePage
    {
        private readonly BookingRepository _bookingRepository = new BookingRepository();

        protected void Page_Init(object sender, EventArgs e)
        {
            Session["Active"] = "Booking";

            //var bookingHistories = BookingHistoryData.GetTodayBookingHistories();
            HistoricalRepeater.DataSource = _bookingRepository.GetAll().ToList();
            HistoricalRepeater.DataBind();
        }
        protected void Page_Load(object sender, EventArgs e)
        {
        
        }

        protected void HistoricalRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
            {
                if (e.Item.ItemType == ListItemType.AlternatingItem)
                {
                    var rowHistory = (HtmlTableRow)e.Item.FindControl("rowHistory");
                    rowHistory.Attributes.Add("class", "alternative");
                }
                var currentItem = (Bookings) e.Item.DataItem;
                var image = (Image) e.Item.FindControl("QRImage");
                if (image != null)
                {
                    image.ImageUrl = QRCode.GetImageSource(Request.Url.AbsoluteUri.Replace("/BookingHandler.aspx", "/Handler/Redeemed.ashx") + "?id=" + currentItem.BookingId);
                }
            }
        }
    }
}