using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using DayaxeDal;
using DayaxeDal.Repositories;

namespace h.dayaxe.com
{
    public partial class InventoryAndPricingAddOns : BasePageProduct
    {
        private HotelRepository _hotelRepository = new HotelRepository();
        private ProductRepository _productRepository = new ProductRepository();
        private bool _isReload;

        protected void Page_Init(object sender, EventArgs e)
        {
            Session["Active"] = "Iventory";
            ReloadPass();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
        
        }

        private void ReloadPass(bool isNew = false)
        {
            if (isNew)
            {
                _hotelRepository = new HotelRepository();
                _productRepository = new ProductRepository();
                _isReload = true;
            }

            var addOns = _productRepository.GetByHotelId(PublicHotel.HotelId, (int)Enums.ProductType.AddOns);
            RptAddOns.DataSource = addOns;
            RptAddOns.DataBind();
        }

        protected void SavePassClick(object sender, EventArgs e)
        {
            if (RptAddOns.Items.Count > 0)
            {
                var listProducts = new List<Products>();
                foreach (RepeaterItem item in RptAddOns.Items)
                {
                    //to get the dropdown of each line
                    HiddenField productIdHid = (HiddenField)item.FindControl("HidId");

                    var products = new Products
                    {
                        ProductId = int.Parse(productIdHid.Value)
                    };

                    double regularPrice;
                    //double upgradeDiscountPrice;
                    int quantity;
                    bool updateDefaultPrice = false;

                    var regularMonText = (TextBox)item.FindControl("RegularMonText");
                    double.TryParse(regularMonText.Text, out regularPrice);
                    if (!products.PriceMon.Equals(regularPrice))
                    {
                        updateDefaultPrice = true;
                    }
                    products.PriceMon = regularPrice;

                    var regularTueText = (TextBox)item.FindControl("RegularTueText");
                    double.TryParse(regularTueText.Text, out regularPrice);
                    if (!products.PriceTue.Equals(regularPrice))
                    {
                        updateDefaultPrice = true;
                    }
                    products.PriceTue = regularPrice;

                    var regularWedText = (TextBox)item.FindControl("RegularWedText");
                    double.TryParse(regularWedText.Text, out regularPrice);
                    if (!products.PriceWed.Equals(regularPrice))
                    {
                        updateDefaultPrice = true;
                    }
                    products.PriceWed = regularPrice;

                    var regularThuText = (TextBox)item.FindControl("RegularThuText");
                    double.TryParse(regularThuText.Text, out regularPrice);
                    if (!products.PriceThu.Equals(regularPrice))
                    {
                        updateDefaultPrice = true;
                    }
                    products.PriceThu = regularPrice;

                    var regularFriText = (TextBox)item.FindControl("RegularFriText");
                    double.TryParse(regularFriText.Text, out regularPrice);
                    if (!products.PriceFri.Equals(regularPrice))
                    {
                        updateDefaultPrice = true;
                    }
                    products.PriceFri = regularPrice;

                    var regularSatText = (TextBox)item.FindControl("RegularSatText");
                    double.TryParse(regularSatText.Text, out regularPrice);
                    if (!products.PriceSat.Equals(regularPrice))
                    {
                        updateDefaultPrice = true;
                    }
                    products.PriceSat = regularPrice;

                    var regularSunText = (TextBox)item.FindControl("RegularSunText");
                    double.TryParse(regularSunText.Text, out regularPrice);
                    if (!products.PriceSun.Equals(regularPrice))
                    {
                        updateDefaultPrice = true;
                    }
                    products.PriceSun = regularPrice;

                    // Quantity
                    var quantityMonText = (TextBox)item.FindControl("QuantityMonText");
                    int.TryParse(quantityMonText.Text, out quantity);
                    products.PassCapacityMon = quantity;

                    var quantityTueText = (TextBox)item.FindControl("QuantityTueText");
                    int.TryParse(quantityTueText.Text, out quantity);
                    products.PassCapacityTue = quantity;

                    var quantityWedText = (TextBox)item.FindControl("QuantityWedText");
                    int.TryParse(quantityWedText.Text, out quantity);
                    products.PassCapacityWed = quantity;

                    var quantityThuText = (TextBox)item.FindControl("QuantityThuText");
                    int.TryParse(quantityThuText.Text, out quantity);
                    products.PassCapacityThu = quantity;

                    var quantityFriText = (TextBox)item.FindControl("QuantityFriText");
                    int.TryParse(quantityFriText.Text, out quantity);
                    products.PassCapacityFri = quantity;

                    var quantitySatText = (TextBox)item.FindControl("QuantitySatText");
                    int.TryParse(quantitySatText.Text, out quantity);
                    products.PassCapacitySat = quantity;

                    var quantitySunText = (TextBox)item.FindControl("QuantitySunText");
                    int.TryParse(quantitySunText.Text, out quantity);
                    products.PassCapacitySun = quantity;


                    products.IsUpdateDefaultPrice = updateDefaultPrice;
                    listProducts.Add(products);
                }

                _hotelRepository.UpdateDailyPassLimit(listProducts, PublicHotel.TimeZoneId);
            }

            _hotelRepository.ResetCache();

            ReloadPass(true);

            saving.InnerText = "Saved!";
            saving.Attributes["class"] = "saving";
            ClientScript.RegisterClientScriptBlock(GetType(), "hideSaved", "setTimeout(function(){ $('.saving').animate({ opacity: 0 }, 1400, function () { });}, 100);", true);
        }

        protected void RptAddOns_OnItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var products = (Products)e.Item.DataItem;
                var hidId = (HiddenField)e.Item.FindControl("HidId");

                hidId.Value = products.ProductId.ToString();

                if (!IsPostBack || _isReload)
                {
                    var regularMonText = (TextBox)e.Item.FindControl("RegularMonText");
                    var regularTueText = (TextBox)e.Item.FindControl("RegularTueText");
                    var regularWedText = (TextBox)e.Item.FindControl("RegularWedText");
                    var regularThuText = (TextBox)e.Item.FindControl("RegularThuText");
                    var regularFriText = (TextBox)e.Item.FindControl("RegularFriText");
                    var regularSatText = (TextBox)e.Item.FindControl("RegularSatText");
                    var regularSunText = (TextBox)e.Item.FindControl("RegularSunText");

                    var quantityMonText = (TextBox)e.Item.FindControl("QuantityMonText");
                    var quantityTueText = (TextBox)e.Item.FindControl("QuantityTueText");
                    var quantityWedText = (TextBox)e.Item.FindControl("QuantityWedText");
                    var quantityThuText = (TextBox)e.Item.FindControl("QuantityThuText");
                    var quantityFriText = (TextBox)e.Item.FindControl("QuantityFriText");
                    var quantitySatText = (TextBox)e.Item.FindControl("QuantitySatText");
                    var quantitySunText = (TextBox)e.Item.FindControl("QuantitySunText");

                    regularMonText.Text = products.PriceMon.ToString("00.00");
                    regularTueText.Text = products.PriceTue.ToString("00.00");
                    regularWedText.Text = products.PriceWed.ToString("00.00");
                    regularThuText.Text = products.PriceThu.ToString("00.00");
                    regularFriText.Text = products.PriceFri.ToString("00.00");
                    regularSatText.Text = products.PriceSat.ToString("00.00");
                    regularSunText.Text = products.PriceSun.ToString("00.00");

                    quantityMonText.Text = products.PassCapacityMon.ToString("0");
                    quantityTueText.Text = products.PassCapacityTue.ToString("0");
                    quantityWedText.Text = products.PassCapacityWed.ToString("0");
                    quantityThuText.Text = products.PassCapacityThu.ToString("0");
                    quantityFriText.Text = products.PassCapacityFri.ToString("0");
                    quantitySatText.Text = products.PassCapacitySat.ToString("0");
                    quantitySunText.Text = products.PassCapacitySun.ToString("0");
                }
            }
        }
    }
}