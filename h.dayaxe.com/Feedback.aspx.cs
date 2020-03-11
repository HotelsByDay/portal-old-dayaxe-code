using System;
using System.Globalization;
using System.Linq;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using DayaxeDal;
using DayaxeDal.Extensions;
using DayaxeDal.Repositories;

namespace h.dayaxe.com
{
    public partial class Feedback : BasePageProduct
    {
        private readonly SurveyRepository _surveyRepository = new SurveyRepository();
        ListResult<Surveys> _surveysListResult;
        protected bool IsAdmin { get; set; }

        protected void Page_Init(object sender, EventArgs e)
        {
            Session["Active"] = "CustomerInsight";
            IsAdmin = PublicCustomerInfos != null && PublicCustomerInfos.IsSuperAdmin;
            if (!IsPostBack)
            {
                SelectedFilterBy.Text = SelectedFilterDdl.Text;

                Session["CurrentPage"] = 1;
                _surveysListResult = _surveyRepository.SearchSurveys(PublicHotel.HotelId, null, null);
                RptFeedback.DataSource = _surveysListResult.Items;
                RptFeedback.DataBind();
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void SelectedFilterDdl_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            var surveys = new ListResult<Surveys>();
            switch (SelectedFilterDdl.SelectedValue)
            {
                case "All":
                    surveys = _surveyRepository.SearchSurveys(PublicHotel.HotelId, null, null);
                    break;
                case "Today":
                    surveys = _surveyRepository.SearchSurveys(PublicHotel.HotelId, 
                        DateTime.UtcNow.ToLosAngerlesTimeWithTimeZone(PublicHotel.TimeZoneId), 
                        DateTime.UtcNow.ToLosAngerlesTimeWithTimeZone(PublicHotel.TimeZoneId));
                    break;
                case "ThisWeek":
                    surveys = _surveyRepository.SearchSurveys(PublicHotel.HotelId,
                        DateTime.UtcNow.ToLosAngerlesTimeWithTimeZone(PublicHotel.TimeZoneId).StartOfWeek(DayOfWeek.Monday), 
                        DateTime.UtcNow.ToLosAngerlesTimeWithTimeZone(PublicHotel.TimeZoneId));
                    break;
                case "ThisMonth":
                    surveys = _surveyRepository.SearchSurveys(PublicHotel.HotelId,
                        new DateTime(DateTime.UtcNow.ToLosAngerlesTimeWithTimeZone(PublicHotel.TimeZoneId).Year, 
                        DateTime.UtcNow.ToLosAngerlesTimeWithTimeZone(PublicHotel.TimeZoneId).Month, 1),
                        DateTime.UtcNow.ToLosAngerlesTimeWithTimeZone(PublicHotel.TimeZoneId));
                    break;
                case "Custom":
                    DateFrom.Visible = true;
                    DateTo.Visible = true;
                    Search.Visible = true;
                    CustomForm.Visible = true;
                    break;
            }
            SelectedFilterBy.Text = SelectedFilterDdl.SelectedItem.Text;
            _surveysListResult = surveys;
            BindFeedback(surveys);
        }

        protected void BindFeedback(ListResult<Surveys> surveys)
        {
            RptFeedback.DataSource = surveys.Items;
            RptFeedback.DataBind();
        }

        protected void Search_OnClick(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(DateFrom.Text) || string.IsNullOrEmpty(DateTo.Text))
            {
                ErrorMessageLabel.Text = "Please enter From and To date.";
                return;
            }

            DateTime startDate;
            DateTime endDate;
            DateTime.TryParseExact(DateFrom.Text, "MM/dd/yyyy", null, DateTimeStyles.None, out startDate);
            DateTime.TryParseExact(DateTo.Text, "MM/dd/yyyy", null, DateTimeStyles.None, out endDate);
            _surveysListResult = _surveyRepository.SearchSurveys(PublicHotel.HotelId, startDate, endDate);

            Session["CurrentPage"] = 1;
            RptFeedback.DataSource = _surveysListResult.Items;
            RptFeedback.DataBind();
        }

        protected void RptFeedback_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var rowHistory = (HtmlTableRow)e.Item.FindControl("rowHotel");
                rowHistory.Attributes.Add("class", rowHistory.Attributes["class"] + " alternative");
            }

            if (e.Item.ItemType == ListItemType.Footer)
            {
                var litPage = (Literal)e.Item.FindControl("LitPage");
                var litTotal = (Literal)e.Item.FindControl("LitTotal");
                var totalPage = _surveysListResult.TotalRecords / Constant.ItemPerPage + (_surveysListResult.TotalRecords % Constant.ItemPerPage != 0 ? 1 : 0);
                litPage.Text = string.Format("Page {0} of {1}", Session["CurrentPage"], totalPage);
                litTotal.Text = _surveysListResult.TotalRecords + " Feedbacks";
            }
        }

        private ListResult<Surveys> SearchFeedback(int currentPage)
        {
            switch (SelectedFilterDdl.SelectedValue)
            {
                case "All":
                    _surveysListResult = _surveyRepository.SearchSurveys(PublicHotel.HotelId, null, null, currentPage);
                    break;
                case "Today":
                    _surveysListResult = _surveyRepository.SearchSurveys(PublicHotel.HotelId, 
                        DateTime.UtcNow.ToLosAngerlesTimeWithTimeZone(PublicHotel.TimeZoneId), 
                        DateTime.UtcNow.ToLosAngerlesTimeWithTimeZone(PublicHotel.TimeZoneId), 
                        currentPage);
                    break;
                case "ThisWeek":
                    _surveysListResult = _surveyRepository.SearchSurveys(PublicHotel.HotelId,
                        DateTime.UtcNow.ToLosAngerlesTimeWithTimeZone(PublicHotel.TimeZoneId).StartOfWeek(DayOfWeek.Monday), 
                        DateTime.UtcNow.ToLosAngerlesTimeWithTimeZone(PublicHotel.TimeZoneId), 
                        currentPage);
                    break;
                case "ThisMonth":
                    _surveysListResult = _surveyRepository.SearchSurveys(PublicHotel.HotelId,
                        new DateTime(DateTime.UtcNow.ToLosAngerlesTimeWithTimeZone(PublicHotel.TimeZoneId).Year, 
                        DateTime.UtcNow.ToLosAngerlesTimeWithTimeZone(PublicHotel.TimeZoneId).Month, 1),
                        DateTime.UtcNow.ToLosAngerlesTimeWithTimeZone(PublicHotel.TimeZoneId), currentPage);
                    break;
                case "Custom":
                    if (string.IsNullOrEmpty(DateFrom.Text) || string.IsNullOrEmpty(DateTo.Text))
                    {
                        ErrorMessageLabel.Text = "Please enter From and To date.";
                        return new ListResult<Surveys>();
                    }
                    DateTime startDate;
                    DateTime endDate;
                    DateTime.TryParseExact(DateFrom.Text, "MM/dd/yyyy", null, DateTimeStyles.None, out startDate);
                    DateTime.TryParseExact(DateTo.Text, "MM/dd/yyyy", null, DateTimeStyles.None, out endDate);
                    _surveysListResult = _surveyRepository.SearchSurveys(PublicHotel.HotelId, startDate, endDate, currentPage);
                    break;
            }
            return _surveysListResult;
        }

        protected void Previous_OnClick(object sender, EventArgs e)
        {
            int currentPage = int.Parse(Session["CurrentPage"].ToString());
            _surveysListResult = SearchFeedback(currentPage - 1);
            if (_surveysListResult.Items.Any() && currentPage - 2 >= 0)
            {
                Session["CurrentPage"] = currentPage - 1;
                RptFeedback.DataSource = _surveysListResult.Items;
                RptFeedback.DataBind();
            }
        }

        protected void Next_OnClick(object sender, EventArgs e)
        {
            int currentPage = int.Parse(Session["CurrentPage"].ToString());
            _surveysListResult = SearchFeedback(currentPage + 1);
            if (_surveysListResult.Items.Any())
            {
                Session["CurrentPage"] = currentPage + 1;
                RptFeedback.DataSource = _surveysListResult.Items;
                RptFeedback.DataBind();
            }
        }
    }
}