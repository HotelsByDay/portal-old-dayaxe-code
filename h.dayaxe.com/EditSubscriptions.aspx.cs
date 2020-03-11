using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web.UI.WebControls;
using DayaxeDal;
using DayaxeDal.Extensions;
using DayaxeDal.Repositories;
using Stripe;

namespace h.dayaxe.com
{
    public partial class EditSubscriptions : BasePage
    {
        private readonly SubscriptionRepository _subscriptionRepository = new SubscriptionRepository();

        private Subscriptions _subscriptions;
        private int _subscriptionId;
        protected void Page_Init(object sender, EventArgs e)
        {
            Session["Active"] = "SubscriptionList";
            // Max Guest
            Helper.CreateControl(MaxGuestControl);

            foreach (Enums.SubscriptionType subscriptionType in Enum.GetValues(typeof(Enums.SubscriptionType)))
            {
                SubscriptionTypeDdl.Items.Add(new ListItem(subscriptionType.ToDescription(), ((int)subscriptionType).ToString()));
            }

            int.TryParse(Request.Params["id"], out _subscriptionId);
            if (_subscriptionId != 0)
            {
                _subscriptions = _subscriptionRepository.GetById(_subscriptionId);
            }

            if (!IsPostBack)
            {
                if (_subscriptionId == 0) // Add new
                {
                    DeleteButton.Visible = false;
                    Deactivebutton.Visible = false;
                    ActiveButton.Visible = false;
                    UploadImage.Visible = false;
                    HidMaxGuest.Value = "1";
                }
                else
                {
                    // Edit
                    if (_subscriptions == null)
                    {
                        Response.Redirect(Constant.SubscriptionListPage);
                    }
                    BindProductData();
                }
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void UploadImageClick(object sender, EventArgs e)
        {
            if (SubscriptionImage.HasFile)
            {
                var file = SubscriptionImage.PostedFile;
                if (Request.Params["id"] != "0")
                {
                    string pathString = Server.MapPath(string.Format("/HotelImage/s{0}/", _subscriptions.Id));
                    string localImageFile = string.Format("/HotelImage/s{0}/{1}", _subscriptions.Id, file.FileName);

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

                    var image = new SubscriptionImages
                    {
                        SubscriptionId = _subscriptions.Id,
                        Url = localImageFile,
                        IsActive = true,
                        Order = IsCoverCheckbox.Checked ? 1 : 2,
                        IsCover = IsCoverCheckbox.Checked
                    };

                    _subscriptionRepository.AddImage(image);

                    _subscriptionRepository.ResetCache();

                    Response.Redirect(Request.Url.AbsoluteUri, true);
                }
            }
        }

        protected void CancelClick(object sender, EventArgs e)
        {
            Response.Redirect(Constant.SubscriptionListPage);
        }

        protected void DeleteClick(object sender, EventArgs e)
        {
            _subscriptionRepository.Delete(_subscriptions);

            _subscriptionRepository.ResetCache();

            Response.Redirect(Constant.SubscriptionListPage);
        }

        protected void DeactiveClick(object sender, EventArgs e)
        {
            _subscriptions.IsActive = false;
            _subscriptionRepository.Update(_subscriptions);

            _subscriptionRepository.ResetCache();

            Deactivebutton.Visible = false;
            ActiveButton.Visible = true;
        }

        protected void ActiveClick(object sender, EventArgs e)
        {
            _subscriptions.IsActive = true;
            _subscriptionRepository.Update(_subscriptions);

            _subscriptionRepository.ResetCache();

            Deactivebutton.Visible = true;
            ActiveButton.Visible = false;
        }

        protected void SaveSubscriptionClick(object sender, EventArgs e)
        {
            var subscriptions = new Subscriptions();
            var isNew = true;

            if (Request.Params["id"] != "0")
            {
                isNew = false;
                subscriptions = _subscriptions;
                _subscriptionId = _subscriptions.Id;
            }
            else
            {
                subscriptions.IsActive = true;
            }

            int productType = int.Parse(SubscriptionTypeDdl.SelectedValue);
            subscriptions.SubscriptionType = productType;
            subscriptions.Name = SubscriptionNameText.Text.Trim();
            subscriptions.MaxGuest = Int16.Parse(HidMaxGuest.Value);
            double price;
            double.TryParse(PriceText.Text, out price);
            subscriptions.Price = price;
            int maxPurchase;
            int.TryParse(MaxPurchaseText.Text, out maxPurchase);
            subscriptions.ProductHighlight = ProductHighlightText.Text;
            subscriptions.WhatYouGet = WhatYouGetEditor.Text;
            subscriptions.MaxPurchases = maxPurchase;

            // SEO Tab 2
            subscriptions.MetaDescription = MetaDescription.Text;
            subscriptions.MetaKeyword = MetaKeyword.Text;

            // Photos on tab 3
            if (isNew)
            {
                switch (int.Parse(SubscriptionTypeDdl.SelectedValue))
                {
                    case (int)Enums.SubscriptionType.Subscription:
                        string planId = SubscriptionNameText.Text.Trim().Replace(" ", "-").ToLower();
                        try
                        {
                            // Create Plans On Stripe
                            var planOptions = new StripePlanCreateOptions
                            {
                                Id = planId,
                                Name = SubscriptionNameText.Text.Trim(),
                                Amount = Convert.ToInt32(price * 100),
                                Currency = "usd",
                                Interval = "month"
                            };
                            CreateStripePlan(planOptions);
                        }
                        catch (Exception ex)
                        {
                            ErrorMessageLabel.Visible = true;
                            ErrorMessageLabel.Text = ex.Message + " - " + planId;
                            return;
                        }
                        subscriptions.StripePlanId = planId;
                        subscriptions.StripePlanCreated = DateTime.UtcNow;
                        break;
                    case (int)Enums.SubscriptionType.GiftCard:
                        break;
                }
                _subscriptionId = _subscriptionRepository.Add(subscriptions);
            }
            else
            {
                var photos = new List<SubscriptionImages>();
                foreach (ListViewDataItem item in SubscriptionImageListView.Items)
                {
                    //to get the dropdown of each line
                    HiddenField productPhotoId = (HiddenField)item.FindControl("PhotoId");

                    var photo = _subscriptionRepository.GetImageById(int.Parse(productPhotoId.Value));
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

                _subscriptionRepository.Update(subscriptions, photos);
            }

            string productImageDefault = Constant.ImageDefault;
            var productImage = _subscriptionRepository.SubscriptionImagesList.FirstOrDefault(x => x.SubscriptionId == subscriptions.Id && x.IsCover && x.IsActive);
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

            _subscriptionRepository.ResetCache();

            Response.Redirect(Constant.SubscriptionListPage);
        }

        #region Private region

        private void BindProductData()
        {
            if (_subscriptions.IsActive)
            {
                Deactivebutton.Visible = true;
                ActiveButton.Visible = false;
            }
            else
            {
                Deactivebutton.Visible = false;
                ActiveButton.Visible = true;
            }

            PurchaseUrlLink.NavigateUrl =
                Helper.ResolveRelativeToAbsoluteUrl(new Uri(AppConfiguration.DayaxeClientUrl), 
                String.Format("/subscription/{0}/{1}", _subscriptions.StripePlanId, _subscriptions.Id)); 

            SubscriptionTypeDdl.SelectedValue = _subscriptions.SubscriptionType.ToString();
            SubscriptionNameText.Text = _subscriptions.Name;
            SubscriptionNameText.Enabled = false;
            string maxGuest = _subscriptions.MaxGuest.ToString();
            CurrentMaxGuest.Text = maxGuest;
            HidMaxGuest.Value = maxGuest;
            PriceText.Text = _subscriptions.Price.ToString("00.00");
            PriceText.Enabled = false;
            MaxPurchaseText.Text = _subscriptions.MaxPurchases.ToString();
            ProductHighlightText.Text = _subscriptions.ProductHighlight;
            WhatYouGetEditor.Text = _subscriptions.WhatYouGet;

            MetaDescription.Text = _subscriptions.MetaDescription;
            MetaKeyword.Text = _subscriptions.MetaKeyword;


            RebindPhoto(false);
        }

        private void RebindPhoto(bool isGetNewData = true)
        {
            if (isGetNewData)
            {
                _subscriptions = _subscriptionRepository.GetById(_subscriptionId);
            }
            SubscriptionImageListView.DataSource = _subscriptionRepository.SubscriptionImagesList
                .Where(pi => pi.SubscriptionId == _subscriptions.Id && pi.IsActive)
                .OrderBy(p => p.Order);
            SubscriptionImageListView.DataBind();
        }

        #endregion

        #region Stripe Plan

        private void CreateStripePlan(StripePlanCreateOptions planOptions)
        {
            var planService = new StripePlanService();
            StripePlan plan = planService.Create(planOptions);
        }

        #endregion  
    }
}