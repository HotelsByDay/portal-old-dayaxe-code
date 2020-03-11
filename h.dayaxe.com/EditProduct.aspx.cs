using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using AutoMapper;
using DayaxeDal;
using DayaxeDal.Extensions;
using DayaxeDal.Repositories;

namespace h.dayaxe.com
{
    public partial class EditProduct : BasePageProduct
    {
        private readonly HotelRepository _hotelRepository = new HotelRepository();
        private ProductRepository _productRepository = new ProductRepository();
        private readonly ProductImageRepository _productImageRepository = new ProductImageRepository();
        private readonly ProductUpgradeRepository _productUpgradeRepository = new ProductUpgradeRepository();
        private Products _products;
        private int _productId;
        protected void Page_Init(object sender, EventArgs e)
        {
            if (PublicHotel == null)
            {
                Response.Redirect(string.Format(Constant.HotelList + "?ReturnUrl={0}", HttpUtility.UrlEncode(Request.Url.PathAndQuery)));
            }

            // Max Guest
            Helper.CreateControl(MaxGuestControl);

            foreach (Enums.ProductType productyType in Enum.GetValues(typeof(Enums.ProductType)))
            {
                ProductTypeDdl.Items.Add(new ListItem(productyType.ToDescription(), ((int)productyType).ToString()));
            }

            // Redemption Period
            Helper.CreateControl(RedemptionPeriod);

            int.TryParse(Request.Params["id"], out _productId);
            if (_productId != 0)
            {
                _products = _productRepository.GetById(_productId);
            }

            foreach (Enums.KidAllowType productyType in Enum.GetValues(typeof(Enums.KidAllowType)))
            {
                KidAllowedDdl.Items.Add(new ListItem(productyType.ToDescription(), ((int)productyType).ToString()));
            }

            if (!IsPostBack)
            {
                if (_productId == 0) // Add new
                {
                    DeleteButton.Visible = false;
                    Deactivebutton.Visible = false;
                    ActiveButton.Visible = false;
                    UploadImage.Visible = false;
                    HidMaxGuest.Value = "2";
                    IsCheckedInRequiredHidden.Value = "false";
                    IsFeaturedRequiredHidden.Value = "false";
                    HidRedemptionPeriod.Value = Constant.DefaultRedemptionPeriod.ToString();
                }
                else
                {
                    // Edit

                    if (_products == null)
                    {
                        Response.Redirect(Constant.ProductListPage);
                    }
                    BindProductData();
                }
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void CancelClick(object sender, EventArgs e)
        {
            Response.Redirect(Constant.ProductListPage);
        }

        protected void DeleteClick(object sender, EventArgs e)
        {
            _productRepository.Delete(_products);

            _productRepository.ResetCache();

            Response.Redirect(Constant.ProductListPage);
        }

        protected void DeactiveClick(object sender, EventArgs e)
        {
            _products.IsActive = false;
            _productRepository.Update(_products);

            _productRepository.ResetCache();

            Deactivebutton.Visible = false;
            ActiveButton.Visible = true;
        }

        protected void ActiveClick(object sender, EventArgs e)
        {
            _products.IsActive = true;
            _productRepository.Update(_products);

            _productRepository.ResetCache();

            Deactivebutton.Visible = true;
            ActiveButton.Visible = false;
        }

        protected void SaveProductClick(object sender, EventArgs e)
        {
            var products = new Products();
            var isNew = true;
            bool isFeaturedRequired;
            bool isCheckedInRequired;

            if (Request.Params["id"] != "0")
            {
                isNew = false;
                products = _products;
                _productId = _products.ProductId;
            }
            else
            {
                products.HotelId = PublicHotel.HotelId;
                products.IsActive = true;
            }

            int productType = int.Parse(ProductTypeDdl.SelectedValue);
            products.ProductType = productType;
            products.ProductName = ProductNameText.Text.Trim();
            products.MaxGuest = Int16.Parse(HidMaxGuest.Value);
            products.ProductHighlight = ProductHighlightText.Text;
            products.WhatYouGet = WhatYouGetEditor.Text;
            products.Service = ServiceEditor.Text;


            bool.TryParse(IsFeaturedRequiredHidden.Value, out isFeaturedRequired);
            products.IsFeatured = isFeaturedRequired;

            products.RedemptionPeriod = Int16.Parse(HidRedemptionPeriod.Value);
            bool.TryParse(IsCheckedInRequiredHidden.Value, out isCheckedInRequired);
            products.IsCheckedInRequired = isCheckedInRequired;
            
            products.IsKidAllow = Int16.Parse(KidAllowedDdl.SelectedValue);
            // Available Upgrades

            products.MetaDescription = MetaDescription.Text;
            products.MetaKeyword = MetaKeyword.Text;

            // Photos on tab 4

            var defaultPrice = Mapper.Map<Products, DefaultPrices>(products);
            defaultPrice.UpdatedBy = 1;
            defaultPrice.UpdatedDate = DateTime.UtcNow;
            if (isNew)
            {
                // Default Value if add new product
                _productId = _productRepository.Add(products);
                defaultPrice.ProductId = _productId;

                var passLimit = _hotelRepository.GetDailyPassLimit(PublicHotel.HotelId);
                switch (products.ProductType)
                {
                    case (int) Enums.ProductType.DayPass:
                        products.DailySales = passLimit.DailyPass;
                        defaultPrice.DailyDayPass = passLimit.DailyPass;
                        break;
                    case (int) Enums.ProductType.Cabana:
                        products.DailySales = passLimit.CabanaPass;
                        defaultPrice.DailyCabana = passLimit.CabanaPass;
                        break;
                    case (int) Enums.ProductType.SpaPass:
                        products.DailySales = passLimit.SpaPass;
                        defaultPrice.DailySpa = passLimit.SpaPass;
                        break;
                    case (int) Enums.ProductType.Daybed:
                        products.DailySales = passLimit.DaybedPass;
                        defaultPrice.DailyDaybed = passLimit.DaybedPass;
                        break;
                    case (int)Enums.ProductType.AddOns:
                        products.DailySales = 0;
                        break;
                }

                defaultPrice.EffectiveDate = DateTime.UtcNow.AddDays(-7);
                _productRepository.AddDefaultPrices(defaultPrice);
            }
            else
            {
                var photos = new List<ProductImages>();
                foreach (ListViewDataItem item in ProductImageListView.Items)
                {
                    //to get the dropdown of each line
                    HiddenField productPhotoId = (HiddenField)item.FindControl("PhotoId");

                    var photo = _productImageRepository.GetById(int.Parse(productPhotoId.Value));
                    var orderItem = (HiddenField)item.FindControl("Order");
                    photo.Order = 1;
                    if (!string.IsNullOrEmpty(orderItem.Value))
                    {
                        photo.Order = int.Parse(orderItem.Value);
                        photo.IsCover = false;
                        photo.IsActive = true;
                    }

                    photos.Add(photo);
                }
                if (photos.FirstOrDefault(p => p.Order == 0 || p.Order == 1) != null)
                {
                    photos.First(p => p.Order == 0 || p.Order == 1).IsCover = true;
                }

                _productImageRepository.Update(photos);

                _productRepository.Update(products);
            }

            string productImageDefault = Constant.ImageDefault;
            var productImage = _productImageRepository.GetAll().FirstOrDefault(x => x.ProductId == products.ProductId && x.IsCover && x.IsActive);
            if (productImage != null)
            {
                productImageDefault = productImage.Url;
            }

            if (!string.IsNullOrEmpty(productImageDefault))
            {
                var imageName = Helper.ReplaceLastOccurrence(productImageDefault, ".", "-ovl.");
                string imageUrl = Server.MapPath(imageName);
                if (!File.Exists(imageUrl))
                {
                    System.Drawing.Image image = System.Drawing.Image.FromFile(Server.MapPath(productImageDefault));
                    Bitmap newImage = Helper.ChangeOpacity(image, 0.7f);
                    using (MemoryStream memory = new MemoryStream())
                    {
                        using (FileStream fs = new FileStream(imageUrl, FileMode.Create, FileAccess.ReadWrite))
                        {
                            newImage.Save(memory, ImageFormat.Jpeg);
                            byte[] bytes = memory.ToArray();
                            fs.Write(bytes, 0, bytes.Length);
                        }
                    }
                }
            }

            // Delete old records if edit on same day
            _productRepository.DeleteDefaultPrices(_productId, DateTime.UtcNow);

            _productRepository.ResetCache();

            Response.Redirect(Constant.ProductListPage);
        }

        #region Add Upgrades

        protected void AddUpgradesButtonClick(object sender, EventArgs e)
        {
            int upgradeId;
            int.TryParse(ProductAvailableUpgradesDdl.SelectedValue, out upgradeId);
            if (upgradeId != 0)
            {
                var productUpgrade = new ProductUpgrades
                {
                    UpgradeId = upgradeId,
                    ProductId = _productId
                };
                _productUpgradeRepository.Add(productUpgrade);

                _productRepository.ResetCache();

                RebindProductUpgrades(true);
            }
        }

        protected void RptProductListingsItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var rowHistory = (HtmlGenericControl)e.Item.FindControl("liAlternatie");
                rowHistory.Attributes.Add("class", "alternative");
            }
        }

        protected void RemoveProductClick(object sender, EventArgs e)
        {
            if (Request.Params["id"] != "0")
            {
                List<ProductUpgrades> productUpgradeses = new List<ProductUpgrades>();
                foreach (RepeaterItem item in RptProductListings.Items)
                {
                    //to get the dropdown of each line
                    CheckBox chkAmentyList = (CheckBox)item.FindControl("ChkRemove");

                    if (chkAmentyList.Checked)
                    {
                        HiddenField hidProductId = (HiddenField)item.FindControl("HidProductId");
                        productUpgradeses.Add(new ProductUpgrades
                        {
                            ProductId = int.Parse(Request.Params["id"]),
                            UpgradeId = int.Parse(hidProductId.Value)
                        });
                    }
                }
                _productUpgradeRepository.Delete(productUpgradeses);

                _productRepository.ResetCache();

                RebindProductUpgrades(true);
            }
        }

        #endregion

        protected void UploadImageClick(object sender, EventArgs e)
        {
            if (ProductImage.HasFile)
            {
                var file = ProductImage.PostedFile;
                if (Request.Params["id"] != "0")
                {
                    string pathString = Server.MapPath(string.Format("/HotelImage/{0}/", _products.ProductId));
                    string localImageFile = string.Format("/HotelImage/{0}/{1}", _products.ProductId, file.FileName);

                    if (!Directory.Exists(pathString))
                    {
                        Directory.CreateDirectory(pathString);
                    }
                    try
                    {
                        file.SaveAs(Server.MapPath(localImageFile));
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message);
                    }

                    var image = new ProductImages
                    {
                        ProductId = _products.ProductId,
                        Url = localImageFile,
                        IsActive = true,
                        Order = IsCoverCheckbox.Checked ? 1 : 2,
                        IsCover = IsCoverCheckbox.Checked
                    };

                    _productImageRepository.Add(image);

                    _productRepository.ResetCache();

                    Response.Redirect(Request.Url.AbsoluteUri, true);
                }
            }
        }

        #region  Private Function

        private void RebindPhoto(bool isGetNewData = true)
        {
            if (isGetNewData)
            {
                _products = _productRepository.GetById(_products.ProductId);
            }
            ProductImageListView.DataSource = _productImageRepository.ProductImageList
                .Where(pi => pi.ProductId == _products.ProductId && pi.IsActive)
                .OrderBy(p => p.Order);
            ProductImageListView.DataBind();
        }

        private void RebindProductUpgrades(bool isNew = false)
        {
            if (_products != null && _products.ProductType != (int) Enums.ProductType.AddOns)
            {
                if (isNew)
                {
                    _productRepository = new ProductRepository();
                }
                List<Products> allProducts = _productRepository.SearchProductsByHotelId(_products.HotelId)
                    .Where(p => p.ProductType != (int) Enums.ProductType.AddOns)
                    .ToList();

                int id = int.Parse(Request.Params["id"]);

                IEnumerable<Products> upgradeHotels = _productRepository.GetProductsUpgradeByProductId(id);
                ProductAvailableUpgradesDdl.DataSource = allProducts.Where(x => x.ProductId != _products.ProductId)
                    .Except(upgradeHotels);
                ProductAvailableUpgradesDdl.DataTextField = "ProductName";
                ProductAvailableUpgradesDdl.DataValueField = "ProductId";
                ProductAvailableUpgradesDdl.DataBind();

                RptProductListings.DataSource = upgradeHotels
                    .Where(x => x.ProductId != id &&
                                x.ProductType != (int) Enums.ProductType.DayPass &&
                                x.ProductType != (int) Enums.ProductType.AddOns);
                RptProductListings.DataBind();
            }
        }

        private void BindProductData()
        {
            if (_products.IsActive)
            {
                Deactivebutton.Visible = true;
                ActiveButton.Visible = false;
            }
            else
            {
                Deactivebutton.Visible = false;
                ActiveButton.Visible = true;
            }

            ProductTypeDdl.SelectedValue = _products.ProductType.ToString();
            ProductNameText.Text = _products.ProductName;
            string maxGuest = _products.MaxGuest.ToString();
            CurrentMaxGuest.Text = maxGuest;
            HidMaxGuest.Value = maxGuest;
            ProductHighlightText.Text = _products.ProductHighlight;
            WhatYouGetEditor.Text = _products.WhatYouGet;
            ServiceEditor.Text = _products.Service;

            string redemptionPeriod = (_products.RedemptionPeriod == 0 ? 30 : _products.RedemptionPeriod).ToString();
            CurrentRedemptionPeriod.Text = redemptionPeriod;
            HidRedemptionPeriod.Value = redemptionPeriod;
            KidAllowedDdl.SelectedValue = _products.IsKidAllow.ToString();
            IsCheckedInRequiredHidden.Value = _products.IsCheckedInRequired ? "true" : "false";
            IsFeaturedRequiredHidden.Value = _products.IsFeatured ? "true" : "false";

            RebindProductUpgrades();
            RebindAddOns();

            MetaDescription.Text = _products.MetaDescription;
            MetaKeyword.Text = _products.MetaKeyword;

            RebindPhoto(false);
        }

        private void RebindAddOns(bool isNew = false)
        {
            if (_products != null && _products.ProductType != (int) Enums.ProductType.AddOns)
            {
                if (isNew)
                {
                    _productRepository = new ProductRepository();
                }
                List<Products> allProducts = _productRepository.SearchProductsByHotelId(_products.HotelId)
                    .Where(p => p.ProductType == (int) Enums.ProductType.AddOns)
                    .ToList();

                int id = int.Parse(Request.Params["id"]);

                IEnumerable<Products> addOnsHotels = _productRepository.GetProductsAdOnsByProductId(id);
                ProductAvailableAddOnsDdl.DataSource = allProducts.Where(x => x.ProductId != _products.ProductId)
                    .Except(addOnsHotels);
                ProductAvailableAddOnsDdl.DataTextField = "ProductName";
                ProductAvailableAddOnsDdl.DataValueField = "ProductId";
                ProductAvailableAddOnsDdl.DataBind();

                RptAddOnsListings.DataSource = addOnsHotels
                    .Where(x => x.ProductId != id &&
                                x.ProductType != (int) Enums.ProductType.DayPass &&
                                x.ProductType == (int) Enums.ProductType.AddOns);
                RptAddOnsListings.DataBind();
            }
        }

        #endregion

        #region Add-Ons

        protected void AddAddOnsButtonClick(object sender, EventArgs e)
        {
            int addOnId;
            int.TryParse(ProductAvailableAddOnsDdl.SelectedValue, out addOnId);
            if (addOnId != 0)
            {
                var productAddOns = new ProductAddOns
                {
                    AddOnId = addOnId,
                    ProductId = _productId
                };
                _productRepository.AddAddOns(productAddOns);

                _productRepository.ResetCache();

                RebindAddOns(true);
            }
        }

        protected void RptAddOnsListingsItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var rowHistory = (HtmlGenericControl)e.Item.FindControl("liAlternatie");
                rowHistory.Attributes.Add("class", "alternative");
            }
        }

        protected void RemoveAddOnsClick(object sender, EventArgs e)
        {
            if (Request.Params["id"] != "0")
            {
                List<ProductAddOns> addOns = new List<ProductAddOns>();
                foreach (RepeaterItem item in RptAddOnsListings.Items)
                {
                    //to get the dropdown of each line
                    CheckBox chkAmentyList = (CheckBox)item.FindControl("ChkRemove");

                    if (chkAmentyList.Checked)
                    {
                        HiddenField hidProductId = (HiddenField)item.FindControl("HidProductId");
                        addOns.Add(new ProductAddOns
                        {
                            ProductId = int.Parse(Request.Params["id"]),
                            AddOnId = int.Parse(hidProductId.Value)
                        });
                    }
                }
                _productRepository.DeleteAddOns(addOns);

                _productRepository.ResetCache();

                RebindAddOns(true);
            }
        }

        #endregion
    }
}