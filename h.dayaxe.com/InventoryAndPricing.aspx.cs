using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using DayaxeDal;
using DayaxeDal.Repositories;

namespace h.dayaxe.com
{
    public partial class InventoryAndPricing : BasePageProduct
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
            var products = _productRepository.SearchProductsByHotelId(PublicHotel.HotelId)
                .Where(p => p.ProductType != (int) Enums.ProductType.AddOns);
            RptProducts.DataSource = products;
            RptProducts.DataBind();
        }

        protected void SavePassClick(object sender, EventArgs e)
        {
            CacheLayer.Clear(CacheKeys.BookingsCacheKey);
            CacheLayer.Clear(CacheKeys.BlockedDatesCustomPricesCacheKey);

            if (RptProducts.Items.Count > 0)
            {
                var listProducts = new List<Products>();
                foreach (RepeaterItem item in RptProducts.Items)
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
                    
                    var regularMonText = (TextBox) item.FindControl("RegularMonText");
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

                    // Upgrade Price
                    //var upgradeMonText = (TextBox)item.FindControl("UpgradeMonText");
                    //double.TryParse(upgradeMonText.Text, out upgradeDiscountPrice);
                    //if (!products.UpgradeDiscountMon.Equals(regularPrice))
                    //{
                    //    updateDefaultPrice = true;
                    //}
                    //products.UpgradeDiscountMon = upgradeDiscountPrice;

                    //var upgradeTueText = (TextBox)item.FindControl("UpgradeTueText");
                    //double.TryParse(upgradeTueText.Text, out upgradeDiscountPrice);
                    //if (!products.UpgradeDiscountTue.Equals(regularPrice))
                    //{
                    //    updateDefaultPrice = true;
                    //}
                    //products.UpgradeDiscountTue = upgradeDiscountPrice;

                    //var upgradeWedText = (TextBox)item.FindControl("UpgradeWedText");
                    //double.TryParse(upgradeWedText.Text, out upgradeDiscountPrice);
                    //if (!products.UpgradeDiscountWed.Equals(regularPrice))
                    //{
                    //    updateDefaultPrice = true;
                    //}
                    //products.UpgradeDiscountWed = upgradeDiscountPrice;

                    //var upgradeThuText = (TextBox)item.FindControl("UpgradeThuText");
                    //double.TryParse(upgradeThuText.Text, out upgradeDiscountPrice);
                    //if (!products.UpgradeDiscountThu.Equals(regularPrice))
                    //{
                    //    updateDefaultPrice = true;
                    //}
                    //products.UpgradeDiscountThu = upgradeDiscountPrice;

                    //var upgradeFriText = (TextBox)item.FindControl("UpgradeFriText");
                    //double.TryParse(upgradeFriText.Text, out upgradeDiscountPrice);
                    //if (!products.UpgradeDiscountFri.Equals(regularPrice))
                    //{
                    //    updateDefaultPrice = true;
                    //}
                    //products.UpgradeDiscountFri = upgradeDiscountPrice;

                    //var upgradeSatText = (TextBox)item.FindControl("UpgradeSatText");
                    //double.TryParse(upgradeSatText.Text, out upgradeDiscountPrice);
                    //if (!products.UpgradeDiscountSat.Equals(regularPrice))
                    //{
                    //    updateDefaultPrice = true;
                    //}
                    //products.UpgradeDiscountSat = upgradeDiscountPrice;

                    //var upgradeSunText = (TextBox)item.FindControl("UpgradeSunText");
                    //double.TryParse(upgradeSunText.Text, out upgradeDiscountPrice);
                    //if (!products.UpgradeDiscountSun.Equals(regularPrice))
                    //{
                    //    updateDefaultPrice = true;
                    //}
                    //products.UpgradeDiscountSun = upgradeDiscountPrice;

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

        protected void RptProductsOnItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var products = (Products)e.Item.DataItem;
                var hidId = (HiddenField)e.Item.FindControl("HidId");

                hidId.Value = products.ProductId.ToString();

                if (!IsPostBack || _isReload)
                {
                    var regularMonText = (TextBox) e.Item.FindControl("RegularMonText");
                    var regularTueText = (TextBox) e.Item.FindControl("RegularTueText");
                    var regularWedText = (TextBox) e.Item.FindControl("RegularWedText");
                    var regularThuText = (TextBox) e.Item.FindControl("RegularThuText");
                    var regularFriText = (TextBox) e.Item.FindControl("RegularFriText");
                    var regularSatText = (TextBox) e.Item.FindControl("RegularSatText");
                    var regularSunText = (TextBox) e.Item.FindControl("RegularSunText");

                    //var upgradeMonText = (TextBox) e.Item.FindControl("UpgradeMonText");
                    //var upgradeTueText = (TextBox) e.Item.FindControl("UpgradeTueText");
                    //var upgradeWedText = (TextBox) e.Item.FindControl("UpgradeWedText");
                    //var upgradeThuText = (TextBox) e.Item.FindControl("UpgradeThuText");
                    //var upgradeFriText = (TextBox) e.Item.FindControl("UpgradeFriText");
                    //var upgradeSatText = (TextBox) e.Item.FindControl("UpgradeSatText");
                    //var upgradeSunText = (TextBox) e.Item.FindControl("UpgradeSunText");

                    var quantityMonText = (TextBox) e.Item.FindControl("QuantityMonText");
                    var quantityTueText = (TextBox) e.Item.FindControl("QuantityTueText");
                    var quantityWedText = (TextBox) e.Item.FindControl("QuantityWedText");
                    var quantityThuText = (TextBox) e.Item.FindControl("QuantityThuText");
                    var quantityFriText = (TextBox) e.Item.FindControl("QuantityFriText");
                    var quantitySatText = (TextBox) e.Item.FindControl("QuantitySatText");
                    var quantitySunText = (TextBox) e.Item.FindControl("QuantitySunText");

                    regularMonText.Text = products.PriceMon.ToString("00.00");
                    regularTueText.Text = products.PriceTue.ToString("00.00");
                    regularWedText.Text = products.PriceWed.ToString("00.00");
                    regularThuText.Text = products.PriceThu.ToString("00.00");
                    regularFriText.Text = products.PriceFri.ToString("00.00");
                    regularSatText.Text = products.PriceSat.ToString("00.00");
                    regularSunText.Text = products.PriceSun.ToString("00.00");

                    //upgradeMonText.Text = products.UpgradeDiscountMon.ToString("0.00");
                    //upgradeTueText.Text = products.UpgradeDiscountTue.ToString("0.00");
                    //upgradeWedText.Text = products.UpgradeDiscountWed.ToString("0.00");
                    //upgradeThuText.Text = products.UpgradeDiscountThu.ToString("0.00");
                    //upgradeFriText.Text = products.UpgradeDiscountFri.ToString("0.00");
                    //upgradeSatText.Text = products.UpgradeDiscountSat.ToString("0.00");
                    //upgradeSunText.Text = products.UpgradeDiscountSun.ToString("0.00");

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