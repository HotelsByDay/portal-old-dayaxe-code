using System;
using System.Linq;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using DayaxeDal;
using DayaxeDal.Repositories;

namespace dayaxe.com
{
    public partial class AllReviews : BasePage
    {
        private readonly SurveyRepository _surveyRepository = new SurveyRepository();
        private readonly HotelRepository _hotelRepository = new HotelRepository();
        protected void Page_Init(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                TotalReviewsLabel.Text = string.Format(Constant.TotalReviewsText, 0, "All Reviews");

                int hotelId = 0;
                if (Page.RouteData.Values["hotelId"] != null)
                {
                    int.TryParse(Page.RouteData.Values["hotelId"].ToString(), out hotelId);
                }

                var hotels = _hotelRepository.GetAll()
                    .Where(h => h.IsActive && !h.IsDelete && h.IsPublished)
                    .OrderBy(h => h.HotelName)
                    .ToList();

                var selectedHotel = hotels.FirstOrDefault(h => h.HotelId == hotelId);

                var reviews = selectedHotel != null
                    ? _surveyRepository.GetByHotelId(hotelId)
                    : _surveyRepository.GetAllReviews();

                ReviewsRpt.DataSource = reviews;
                ReviewsRpt.DataBind();

                string hotelSelectedString = selectedHotel != null
                    ? string.Format(Constant.TotalReviewsText, reviews.Count(), 
                        string.Format("{0} - {1}", selectedHotel.HotelName, selectedHotel.City))
                    : string.Format(Constant.TotalReviewsText, reviews.Count(), "All Hotels");

                AddAllHotels();

                hotels.ForEach(hotel =>
                {
                    var liTag = new HtmlGenericControl("li");
                    var link = new HtmlAnchor
                    {
                        HRef = "#",
                        InnerText = string.Format("{0} - {1}", hotel.HotelName, hotel.City)
                    };
                    link.Attributes["data-href"] = Helper.ResolveRelativeToAbsoluteUrl(Request.Url, string.Format("/reviews/{0}", hotel.HotelId));
                    liTag.Controls.Add(link);

                    HotelDropdown.Controls.Add(liTag);
                });

                TotalReviewsLabel.Text = hotelSelectedString;
                HotelLabel.Text = selectedHotel != null ? string.Format("{0} - {1}", selectedHotel.HotelName, selectedHotel.City) : "All Hotels";
                FilterButton.NavigateUrl = selectedHotel != null
                    ? Helper.ResolveRelativeToAbsoluteUrl(Request.Url,
                        string.Format("/reviews/{0}", selectedHotel.HotelId))
                    : Helper.ResolveRelativeToAbsoluteUrl(Request.Url, "/reviews");
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            
        }

        protected void ReviewsRpt_OnItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var redeemedDateLabel = (Label)e.Item.FindControl("RedeemedDateLabel");
                var survey = (Surveys)e.Item.DataItem;
                if (survey.Bookings.RedeemedDate.HasValue && redeemedDateLabel != null)
                {
                    redeemedDateLabel.Text = string.Format("{0:ddd, MMM dd. yyyy}", survey.Bookings.RedeemedDate.Value);
                }
            }
        }

        protected void FilterButton_OnClick(object sender, EventArgs e)
        {
            
        }

        private void AddAllHotels()
        {
            var liTag = new HtmlGenericControl("li");
            var link = new HtmlAnchor
            {
                HRef = "#",
                InnerText = "All Hotels"
            };
            link.Attributes["data-href"] = Helper.ResolveRelativeToAbsoluteUrl(Request.Url, "/reviews");
            liTag.Controls.Add(link);

            HotelDropdown.Controls.Add(liTag);
        }
    }
}