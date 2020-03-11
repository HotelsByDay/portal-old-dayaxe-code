using System;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using CloudinaryDotNet;
using DayaxeDal;
using DayaxeDal.Repositories;

namespace dayaxe.com
{
    public partial class locations : BasePage
    {
        private readonly ProductRepository _productRepository = new ProductRepository();
        protected Cloudinary Cloudinary;

        protected void Page_Init(object sender, EventArgs e)
        {
            Account account = new Account(
                "vietluyen",
                "385557456569739",
                "Si2Q2D3dxjqgya-Rl7-lZ0cy99Q");

            Cloudinary = new Cloudinary(account);
        }


        protected void Page_Load(object sender, EventArgs e)
        {
            var markets = _productRepository.MarketList;

            LocationRpt.DataSource = markets;
            LocationRpt.DataBind();
        }

        protected void LocationRpt_OnItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var locationImage = (HtmlImage)e.Item.FindControl("LocationImage");
                var markets = (Markets)e.Item.DataItem;
                if (markets != null)
                {
                    locationImage.Attributes["img-d"] = Cloudinary.Api.UrlImgUp.Secure()
                        .Transform(new Transformation().Width(1010).Height(530).Crop("fill"))
                        .BuildUrl(markets.PublicId);
                    locationImage.Attributes["img-r"] = Cloudinary.Api.UrlImgUp.Secure()
                        .Transform(new Transformation().Width(2500).Height(1000).Crop("fill"))
                        .BuildUrl(markets.PublicId);
                    locationImage.Attributes["img-m"] = Cloudinary.Api.UrlImgUp.Secure()
                        .Transform(new Transformation().Width(228).Height(432).Crop("fill"))
                        .BuildUrl(markets.PublicId);
                    //locationImage.ImageUrl = Cloudinary.Api.UrlImgUp.Secure()
                    //    .Transform(new Transformation().Width(432).Height(228).Crop("fill"))
                    //    .BuildUrl(markets.PublicId);
                    locationImage.Attributes["data-src"] = string.Format("{0}/w_auto,c_scale/{1}",
                        "https://res.cloudinary.com/vietluyen/image/upload/", 
                        markets.PublicId);
                }
            }
        }
    }
}