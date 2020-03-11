using System;
using System.Linq;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using DayaxeDal;
using DayaxeDal.Repositories;

namespace h.dayaxe.com
{
    public partial class GiftCardList : BasePage
    {
        private readonly GiftCardRepository _giftCardRepository = new GiftCardRepository();
        protected void Page_Init(object sender, EventArgs e)
        {
            Session["Active"] = "GiftCardList";

            if (!IsPostBack)
            {
                Session["CurrentPage"] = 1;

                RptGiftCardListing.DataSource = _giftCardRepository.GetAll().OrderBy(x => x.Status).ThenByDescending(x => x.Id).Take(Constant.ItemPerPage);
                RptGiftCardListing.DataBind();
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void RptGiftCardListing_OnItemDataBound(object sender, RepeaterItemEventArgs e)
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
                var totaluser = _giftCardRepository.GetAll().Count();
                var totalPage = totaluser / Constant.ItemPerPage + (totaluser % Constant.ItemPerPage != 0 ? 1 : 0);
                litPage.Text = string.Format("Page {0} of {1}", Session["CurrentPage"], totalPage);
                litTotal.Text = totaluser + " Gift Cards";
            }
        }

        protected void Previous_OnClick(object sender, EventArgs e)
        {
            int currentPage = int.Parse(Session["CurrentPage"].ToString());
            var hotels = _giftCardRepository.GetAll().OrderByDescending(x => x.Id).Skip((currentPage - 2) * Constant.ItemPerPage).Take(Constant.ItemPerPage).ToList();
            if (hotels.Any() && currentPage - 2 >= 0)
            {
                Session["CurrentPage"] = currentPage - 1;
                RptGiftCardListing.DataSource = hotels;
                RptGiftCardListing.DataBind();
            }
        }

        protected void Next_OnClick(object sender, EventArgs e)
        {
            int currentPage = int.Parse(Session["CurrentPage"].ToString());
            var hotels = _giftCardRepository.GetAll().OrderByDescending(x => x.Id).Skip(currentPage * Constant.ItemPerPage).Take(Constant.ItemPerPage).ToList();
            if (hotels.Any())
            {
                Session["CurrentPage"] = currentPage + 1;
                RptGiftCardListing.DataSource = hotels;
                RptGiftCardListing.DataBind();
            }
        }
    }
}