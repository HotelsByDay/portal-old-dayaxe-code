using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using DayaxeDal;
using DayaxeDal.Repositories;

namespace h.dayaxe.com
{
    public partial class ProductListings : BasePageProduct
    {
        private readonly ProductRepository _productRepository = new ProductRepository();
        protected void Page_Init(object sender, EventArgs e)
        {
            Session["Active"] = "ProductListing";

            if (PublicCustomerInfos != null && PublicCustomerInfos.IsSuperAdmin)
            {
                AddNewRow.Visible = true;
            }

            if (PublicHotel == null)
            {
                Response.Redirect(string.Format(Constant.HotelList + "?ReturnUrl={0}", HttpUtility.UrlEncode(Request.Url.PathAndQuery)));
            }

            if (!IsPostBack)
            {
                Session["CurrentPageProduct"] = 1;
                List<Products> products = _productRepository.SearchProductsByHotelId(PublicHotel.HotelId).Take(Constant.ItemPerPage).ToList();
                RptProductListings.DataSource = products;
                RptProductListings.DataBind();
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void RptProductListings_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var rowHistory = (HtmlTableRow)e.Item.FindControl("rowHotel");
                rowHistory.Attributes.Add("class", rowHistory.Attributes["class"] + " alternative");
            }

            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var deleteLinkButton = (LinkButton) e.Item.FindControl("DeleteProductLinkButton");
                var product = (Products) e.Item.DataItem;
                deleteLinkButton.CommandArgument = product.ProductId.ToString();
            }

            if (e.Item.ItemType == ListItemType.Footer)
            {
                var litPage = (Literal)e.Item.FindControl("LitPage");
                var litTotal = (Literal)e.Item.FindControl("LitTotal");
                var totalProducts = _productRepository.SearchProductsByHotelId(PublicHotel.HotelId).Count;
                var totalPage = totalProducts / Constant.ItemPerPage + (totalProducts % Constant.ItemPerPage != 0 ? 1 : 0);
                litPage.Text = string.Format("Page {0} of {1}", Session["CurrentPageProduct"], totalPage);
                litTotal.Text = totalProducts + " Products";
            }
        }

        protected void Previous_OnClick(object sender, EventArgs e)
        {
            int currentPage = int.Parse(Session["CurrentPageProduct"].ToString());
            var hotels = _productRepository.SearchProductsByHotelId(PublicHotel.HotelId).Skip((currentPage - 2) * Constant.ItemPerPage).Take(Constant.ItemPerPage).ToList();
            if (hotels.Any() && currentPage - 2 >= 0)
            {
                Session["CurrentPageProduct"] = currentPage - 1;
                RptProductListings.DataSource = hotels;
                RptProductListings.DataBind();
            }
        }

        protected void Next_OnClick(object sender, EventArgs e)
        {
            int currentPage = int.Parse(Session["CurrentPageProduct"].ToString());
            var hotels = _productRepository.SearchProductsByHotelId(PublicHotel.HotelId).Skip(currentPage * Constant.ItemPerPage).Take(Constant.ItemPerPage).ToList();
            if (hotels.Any())
            {
                Session["CurrentPageProduct"] = currentPage + 1;
                RptProductListings.DataSource = hotels;
                RptProductListings.DataBind();
            }
        }

        protected void DeleteProductLinkButton_OnCommand(object sender, CommandEventArgs e)
        {
            var productId = int.Parse(e.CommandArgument.ToString());
            _productRepository.Delete(productId);

            _productRepository.ResetCache();

            Response.Redirect(Constant.ProductListPage);
        }
    }
}