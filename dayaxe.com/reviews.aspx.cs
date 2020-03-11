using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using DayaxeDal;
using DayaxeDal.Repositories;

namespace dayaxe.com
{
    public partial class ReviewsPage : BasePage
    {
        private Products ProductReview { get; set; }
        private readonly SurveyRepository _surveyRepository = new SurveyRepository();
        private readonly ProductRepository _productRepository = new ProductRepository();
        protected void Page_Init(object sender, EventArgs e)
        {
            ProductReview = _productRepository.GetProductsByName((string)Page.RouteData.Values["hotelName"],
                (string)Page.RouteData.Values["productName"],
                (string)Session["UserSession"]);
            if (!IsPostBack && ProductReview != null)
            {
                var  surveys = _surveyRepository.GetByHotelId(ProductReview.HotelId).ToList();
                MarketRepeater.DataSource = surveys;
                MarketRepeater.DataBind();
            }

            ReviewTitleLit.Text = GetReviewTitleText();
            LocationLit.Text = string.Format("{0}, {1}", ProductReview.Hotels.Neighborhood, ProductReview.Hotels.City);
            RatingLit.Text = ProductReview.Hotels.Rating;

            //goBack.HRef = string.Format("/{0}/{1}/{2}/{3}/{4}",
            //    Page.RouteData.Values["market"],
            //    Page.RouteData.Values["city"],
            //    Page.RouteData.Values["hotelName"],
            //    Page.RouteData.Values["productName"],
            //    ProductReview.ProductId);
        }
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void MarketRepeaterItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var redeemedDateLabel = (Label)e.Item.FindControl("RedeemedDateLabel");
                var survey = (Surveys)e.Item.DataItem;
                if (survey.Bookings.RedeemedDate.HasValue && redeemedDateLabel != null)
                {
                    redeemedDateLabel.Text = string.Format("{0}",
                        survey.Bookings.RedeemedDate.Value.ToString(Constant.ReviewsDateFormat));
                }
            }
        }

        protected string GetReviewTitleText()
        {
            switch (ProductReview.ProductType)
            {
                case (int)Enums.ProductType.SpaPass:
                    return string.Format("{0}{1} Reviews", ProductReview.ProductName, Constant.SpaPassString);
                case (int)Enums.ProductType.Daybed:
                    return string.Format("{0}{1} Reviews", ProductReview.ProductName, Constant.DaybedsString);
                case (int)Enums.ProductType.Cabana:
                    return string.Format("{0}{1} Reviews", ProductReview.ProductName, Constant.CabanasPassString);
                default:
                    return string.Format("{0}{1} Reviews", ProductReview.ProductName, Constant.DayPassString);
            }
        }
    }
}