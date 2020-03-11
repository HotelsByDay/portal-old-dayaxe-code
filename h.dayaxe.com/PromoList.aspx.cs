using System;
using System.Linq;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using DayaxeDal;
using DayaxeDal.Repositories;

namespace h.dayaxe.com
{
    public partial class PromoList : BasePage
    {
        private readonly DiscountRepository _discountRepository = new DiscountRepository();

        protected void Page_Init(object sender, EventArgs e)
        {
            Session["Active"] = "DiscountsPage";

            if (!IsPostBack)
            {
                Session["CurrentPage"] = 1;

                RptDiscountListings.DataSource = _discountRepository.GetAll().OrderBy(x => x.Status).Take(Constant.ItemPerPage);
                RptDiscountListings.DataBind();
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void RptDiscountListings_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var rowHistory = (HtmlTableRow)e.Item.FindControl("rowHotel");
                rowHistory.Attributes.Add("class", "alternative");
            }

            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var statusLabel = (Label) e.Item.FindControl("LitStatus");
                var percentOffLit = (Literal) e.Item.FindControl("PercentOffLit");
                var discounts = (Discounts) e.Item.DataItem;
                switch (discounts.Status)
                {
                    case Enums.DiscountStatus.Active:
                        statusLabel.CssClass = "status-red";
                        break;
                    case Enums.DiscountStatus.Ended:
                        statusLabel.CssClass = "status-blue";
                        break;
                    default:
                        statusLabel.CssClass = "status-normal";
                        break;
                }
                switch (discounts.PromoType)
                {
                    case (int)Enums.PromoType.Fixed:
                        percentOffLit.Text = string.Format("{0}$ OFF", discounts.PercentOff);
                        break;
                    default:
                        percentOffLit.Text = string.Format("{0}% OFF", discounts.PercentOff);
                        break;
                }
            }

            if (e.Item.ItemType == ListItemType.Footer)
            {
                var litPage = (Literal)e.Item.FindControl("LitPage");
                var litTotal = (Literal)e.Item.FindControl("LitTotal");
                var totaluser = _discountRepository.GetAll().Count();
                var totalPage = totaluser / Constant.ItemPerPage + (totaluser % Constant.ItemPerPage != 0 ? 1 : 0);
                litPage.Text = string.Format("Page {0} of {1}", Session["CurrentPage"], totalPage);
                litTotal.Text = totaluser + " Discounts";
            }
        }

        protected void Previous_OnClick(object sender, EventArgs e)
        {
            int currentPage = int.Parse(Session["CurrentPage"].ToString());
            var hotels = _discountRepository.GetAll().OrderBy(x => x.Status).Skip((currentPage - 2) * Constant.ItemPerPage).Take(Constant.ItemPerPage).ToList();
            if (hotels.Any() && currentPage - 2 >= 0)
            {
                Session["CurrentPage"] = currentPage - 1;
                RptDiscountListings.DataSource = hotels;
                RptDiscountListings.DataBind();
            }
        }

        protected void Next_OnClick(object sender, EventArgs e)
        {
            int currentPage = int.Parse(Session["CurrentPage"].ToString());
            var hotels = _discountRepository.GetAll().OrderBy(x => x.Status).Skip(currentPage * Constant.ItemPerPage).Take(Constant.ItemPerPage).ToList();
            if (hotels.Any())
            {
                Session["CurrentPage"] = currentPage + 1;
                RptDiscountListings.DataSource = hotels;
                RptDiscountListings.DataBind();
            }
        }
    }
}