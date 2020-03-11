using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using DayaxeDal;

namespace h.dayaxe.com
{
    public partial class SearchBookings : BasePage
    {
        private readonly DalHelper _helper = new DalHelper();
        private SearchDataResponse _response;
        private List<SearchDataObject> _currentData;
        protected void Page_Init(object sender, EventArgs e)
        {
            try
            {
                Session["Active"] = "SearchBookingsList";

                _response = _helper.SearchData();
                _currentData = SearchDataByCondition().ToList();
                if (!IsPostBack)
                {
                    Session["CurrentPage"] = 1;

                    BindRepeater(_currentData.Take(Constant.ItemPerPage));
                }
            }
            catch (Exception ex)
            {
                var logs = new Logs
                {
                    LogKey = "Admin_SearchBookingPage_Error",
                    UpdatedBy = PublicCustomerInfos != null ? PublicCustomerInfos.CustomerId : 1,
                    UpdatedDate = DateTime.UtcNow,
                    UpdatedContent = string.Format("{0} - {1} - {2}", ex.Message, ex.StackTrace, ex.Source)
                };
                _helper.AddLog(logs);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void SearchRepeater_OnItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var rowHistory = (HtmlTableRow)e.Item.FindControl("rowHistory");
                rowHistory.Attributes.Add("class", "alternative");
            }

            if (e.Item.ItemType == ListItemType.Footer)
            {
                var totalPass = (Literal)e.Item.FindControl("TotalPass");
                var litPage = (Literal)e.Item.FindControl("LitPage");
                var totalData = SearchDataByCondition().Count();
                var totalPage = totalData / Constant.ItemPerPage + (totalData % Constant.ItemPerPage != 0 ? 1 : 0);

                litPage.Text = string.Format("Page {0} of {1}", Session["CurrentPage"], totalPage);
                totalPass.Text = SearchDataByCondition().Count() + " records";
            }
        }

        protected void Previous_OnClick(object sender, EventArgs e)
        {
            int currentPage = int.Parse(Session["CurrentPage"].ToString());
            var dataObjects = SearchDataByCondition().Skip((currentPage - 2) * Constant.ItemPerPage).Take(Constant.ItemPerPage).ToList();
            if (dataObjects.Any() && currentPage - 2 >= 0)
            {
                Session["CurrentPage"] = currentPage - 1;
                SearchRepeater.DataSource = dataObjects;
                SearchRepeater.DataBind();
            }
        }

        protected void Next_OnClick(object sender, EventArgs e)
        {
            int currentPage = int.Parse(Session["CurrentPage"].ToString());
            var dataObjects = SearchDataByCondition().Skip(currentPage * Constant.ItemPerPage).Take(Constant.ItemPerPage).ToList();
            if (dataObjects.Any())
            {
                Session["CurrentPage"] = currentPage + 1;
                SearchRepeater.DataSource = dataObjects;
                SearchRepeater.DataBind();
            }
        }

        private void BindRepeater(IEnumerable<SearchDataObject> bookings)
        {
            SearchRepeater.DataSource = bookings;
            SearchRepeater.DataBind();

            AllHyperLink.Text = string.Format("All <span class=\"{1}\">{0}</span>", 
                _response.TotalRecords, 
                Request.Params["s"] == "all" || Request.Params["s"] == null ? "active": string.Empty);

            BookingsHyperLink.Text = string.Format("Bookings <span class=\"{1}\">{0}</span>", 
                _response.TotalBookingsRecord, 
                Request.Params["s"] == "bookings" ? "active" : string.Empty);

            CustomersHyperLink.Text = string.Format("Customers <span class=\"{1}\">{0}</span>", 
                _response.TotalCustomersRecord, 
                Request.Params["s"] == "customers" ? "active" : string.Empty);
        }

        protected void SearchButton_OnClick(object sender, EventArgs e)
        {
            var result = SearchDataByCondition().Take(Constant.ItemPerPage);
            Session["CurrentPage"] = 1;
            BindRepeater(result);
        }

        private IEnumerable<SearchDataObject> SearchDataByCondition()
        {
            IEnumerable<SearchDataObject> query = new List<SearchDataObject>();

            if (Request.Params["s"] == null || Request.Params["s"] == "all")
            {
                query = _response.Data;
            }

            if (Request.Params["s"] == "bookings")
            {
                query = _response.ListBookingsData;
            }

            if (Request.Params["s"] == "customers")
            {
                query = _response.ListCustomerInfosData;
            }

            if (!string.IsNullOrEmpty(SearchText.Text))
            {
                query = query.Where(d => d.Description.ToUpper().Contains(SearchText.Text.Trim().ToUpper()));
            }

            return query.OrderByDescending(x => x.Id).ToList();
        }
    }
}