using System;
using System.Linq;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using DayaxeDal;
using DayaxeDal.Repositories;

namespace h.dayaxe.com
{
    public partial class SubscriptionsList : BasePage
    {
        private readonly SubscriptionRepository _subscriptionRepository = new SubscriptionRepository();
        protected void Page_Init(object sender, EventArgs e)
        {
            Session["Active"] = "SubscriptionList";
            if (PublicCustomerInfos != null && PublicCustomerInfos.IsSuperAdmin)
            {
                AddNewRow.Visible = true;
            }

            if (!IsPostBack)
            {
                Session["CurrentPageSubscription"] = 1;
                var subscriptionses = _subscriptionRepository.GetAll().Where(s => !s.IsDelete).Take(Constant.ItemPerPage).ToList();
                RptSubscriptionsListing.DataSource = subscriptionses;
                RptSubscriptionsListing.DataBind();
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void DeleteSubscriptionsLinkButtonOnCommand(object sender, CommandEventArgs e)
        {
            
        }

        protected void Previous_OnClick(object sender, EventArgs e)
        {
            
        }

        protected void Next_OnClick(object sender, EventArgs e)
        {
            
        }

        protected void RptSubscriptionsListing_OnItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var rowHistory = (HtmlTableRow)e.Item.FindControl("rowHotel");
                rowHistory.Attributes.Add("class", rowHistory.Attributes["class"] + " alternative");
            }

            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var deleteLinkButton = (LinkButton)e.Item.FindControl("DeleteSubscriptionsLinkButton");
                var product = (Subscriptions)e.Item.DataItem;
                deleteLinkButton.CommandArgument = product.Id.ToString();
            }

            if (e.Item.ItemType == ListItemType.Footer)
            {
                var litPage = (Literal)e.Item.FindControl("LitPage");
                var litTotal = (Literal)e.Item.FindControl("LitTotal");
                var totalProducts = _subscriptionRepository.GetAll().Count(s => !s.IsDelete);
                var totalPage = totalProducts / Constant.ItemPerPage + (totalProducts % Constant.ItemPerPage != 0 ? 1 : 0);
                litPage.Text = string.Format("Page {0} of {1}", Session["CurrentPageSubscription"], totalPage);
                litTotal.Text = totalProducts + " Listings";
            }
        }
    }
}