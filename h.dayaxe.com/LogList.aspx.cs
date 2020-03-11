using System;
using System.Linq;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using DayaxeDal.Repositories;

namespace h.dayaxe.com
{
    public partial class LogListPage : BasePage
    {
        private readonly LogRepository _logRepository = new LogRepository();
        private const int ItemPerPage = 50;

        protected void Page_Init(object sender, EventArgs e)
        {
            Session["Active"] = "LogList";

            if (!IsPostBack)
            {
                Session["CurrentPage"] = 1;

                LogRepeater.DataSource = _logRepository.GetAll().Take(ItemPerPage).ToList();
                LogRepeater.DataBind();
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void LogRepeater_OnItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var rowHistory = (HtmlTableRow)e.Item.FindControl("rowHotel");
                rowHistory.Attributes.Add("class", "alternative");
            }

            if (e.Item.ItemType == ListItemType.Footer)
            {
                var litPage = (Literal)e.Item.FindControl("LitPage");
                var litTotal = (Literal)e.Item.FindControl("LitTotal");
                var totalLogs = _logRepository.GetAll().Count();
                var totalPage = totalLogs / ItemPerPage + (totalLogs % ItemPerPage != 0 ? 1 : 0);
                litPage.Text = string.Format("Page {0} of {1}", Session["CurrentPage"], totalPage);
                litTotal.Text = totalLogs + " Records";
            }
        }

        protected void Previous_OnClick(object sender, EventArgs e)
        {
            int currentPage = int.Parse(Session["CurrentPage"].ToString());
            var logs = _logRepository.GetAll().Skip((currentPage - 2) * ItemPerPage).Take(ItemPerPage).ToList();
            if (logs.Any() && currentPage - 2 >= 0)
            {
                Session["CurrentPage"] = currentPage - 1;
                LogRepeater.DataSource = logs;
                LogRepeater.DataBind();
            }
        }

        protected void Next_OnClick(object sender, EventArgs e)
        {
            int currentPage = int.Parse(Session["CurrentPage"].ToString());
            var logs = _logRepository.GetAll().Skip(currentPage * ItemPerPage).Take(ItemPerPage).ToList();
            if (logs.Any())
            {
                Session["CurrentPage"] = currentPage + 1;
                LogRepeater.DataSource = logs;
                LogRepeater.DataBind();
            }
        }
    }
}