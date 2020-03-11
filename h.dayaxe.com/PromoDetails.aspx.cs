using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using DayaxeDal;
using DayaxeDal.Compare;
using DayaxeDal.Repositories;
using WebGrease.Css.Extensions;

namespace h.dayaxe.com
{
    public partial class PromoDetails : BasePage
    {
        private Discounts _discounts;
        private readonly ProductRepository _productRepository = new ProductRepository();
        private DiscountRepository _discountRepository = new DiscountRepository();

        protected void Page_Init(object sender, EventArgs e)
        {
            BindMaxPurchases();
            int id = int.Parse(Request.Params["id"]);
            if (id == 0) // Add new
            {
                DdlHotels.Visible = false;
                IsCodeRequiredHidden.Value = "true";
                BtnAddDiscountHotel.Visible = false;
                BtnAddAllDiscountHotel.Visible = false;
                BtnAddDiscountSubscription.Visible = false;
                BtnAddAllDiscountSubscription.Visible = false;
                MaxPurchasesDdl.SelectedValue = "4";
                BillingCycleDdl.SelectedValue = "1";
                IsAllProductHidden.Value = "false";
                MinAmountText.Text = "0";
            }
            else
            {
                _discounts = _discountRepository.GetById(id);
                if (_discounts == null)
                {
                    Response.Redirect(Constant.PromoListPage);
                }

                DiscountNameText.Text = _discounts.DiscountName;
                StartDateText.Text = _discounts.StartDate.HasValue ? _discounts.StartDate.Value.ToString(Constant.DiscountDateFormat) : string.Empty;
                EndDateText.Text = _discounts.EndDate.HasValue ? _discounts.EndDate.Value.ToString(Constant.DiscountDateFormat) : string.Empty;
                CodeText.Text = _discounts.Code;
                PercentOffText.Text = string.Format("{0:0.00}", _discounts.PercentOff);
                IsCodeRequiredHidden.Value = _discounts.CodeRequired ? "true" : "false";
                IsAllProductHidden.Value = _discounts.IsAllProducts ? "true" : "false";
                PromoTypeDdl.SelectedValue = _discounts.PromoType.ToString();
                MaxPurchasesDdl.SelectedValue = _discounts.MaxPurchases.ToString();
                BillingCycleDdl.SelectedValue = _discounts.BillingCycleNumber.ToString();
                MinAmountText.Text = string.Format("{0:0.00}", _discounts.MinAmount);
                FinePrintText.Text = _discounts.FinePrint;

                RebindDiscount(id);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void RptHotelListings_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var rowHistory = (HtmlGenericControl)e.Item.FindControl("liAlternatie");
                rowHistory.Attributes.Add("class", "alternative");
            }
        }

        protected void UnAssignHotelClick(object sender, EventArgs e)
        {
            int discountId = int.Parse(Request.Params["id"]);
            List<DiscountProducts> discountHotelses = new List<DiscountProducts>();
            foreach (RepeaterItem item in RptHotelListings.Items)
            {
                //to get the dropdown of each line
                CheckBox chkAmentyList = (CheckBox)item.FindControl("ChkRemove");

                if (chkAmentyList.Checked)
                {
                    HiddenField hidProductId = (HiddenField)item.FindControl("HidProductId");
                    HiddenField hidDiscountId = (HiddenField)item.FindControl("HidDiscountId");
                    discountHotelses.Add(new DiscountProducts
                    {
                        DiscountId = int.Parse(hidDiscountId.Value),
                        ProductId = int.Parse(hidProductId.Value)
                    });
                }
            }
            _discountRepository.RemoveDiscountProducts(discountHotelses);
            _discountRepository.ResetCache();
            RebindDiscount(discountId, true);
        }

        protected void BtnAddDiscountHotel_OnClick(object sender, EventArgs e)
        {
            int hotelId;
            int.TryParse(DdlHotels.SelectedValue, out hotelId);
            if (hotelId != 0)
            {
                int discountId = int.Parse(Request.Params["id"]);
                _discountRepository.AddDiscountProducts(new DiscountProducts
                {
                    DiscountId = discountId,
                    ProductId = hotelId
                });
                _discountRepository.ResetCache();
                RebindDiscount(discountId, true);
            }
        }

        protected void BtnAddAllDiscountHotel_OnClick(object sender, EventArgs e)
        {
            var allHotel = _productRepository.SearchProductsByCode();
            int discountId = int.Parse(Request.Params["id"]);
            List<DiscountProducts> newDiscountHotels = new List<DiscountProducts>();

            var hotels = _discountRepository.SearchProductsByDiscountId(discountId);

            allHotel.Except(hotels).ForEach(hotel =>
            {
                newDiscountHotels.Add(new DiscountProducts
                {
                    ProductId = hotel.HotelId,
                    DiscountId = discountId
                });
            });

            _discountRepository.AddAllDiscountProducts(newDiscountHotels);
            _discountRepository.ResetCache();

            RebindDiscount(discountId, true);
        }

        protected void CancelClick(object sender, EventArgs e)
        {
            Response.Redirect(Constant.PromoListPage);
        }

        protected void DeleteClick(object sender, EventArgs e)
        {
            int discountId = int.Parse(Request.Params["id"]);
            if (discountId != 0)
            {
                _discountRepository.Delete(discountId);
                _discountRepository.ResetCache();
            }
            Response.Redirect(Constant.PromoListPage);
        }

        protected void SaveDiscountClick(object sender, EventArgs e)
        {
            LblMessage.Visible = false;
            LblMessage.Text = "";

            if (string.IsNullOrEmpty(DiscountNameText.Text.Trim()))
            {
                LblMessage.Visible = true;
                LblMessage.Text = "Discount Name is required";
                return;
            }

            if (string.IsNullOrEmpty(StartDateText.Text) || string.IsNullOrEmpty(EndDateText.Text))
            {
                LblMessage.Visible = true;
                LblMessage.Text = "Start Date and End Date is required";
                return;
            }

            if (string.IsNullOrEmpty(CodeText.Text.Trim()))
            {
                LblMessage.Visible = true;
                LblMessage.Text = "Code is required";
                return;
            }

            if (string.IsNullOrEmpty(PercentOffText.Text.Trim()))
            {
                LblMessage.Visible = true;
                LblMessage.Text = "Percent Off is required";
                return;
            }

            int discountId = int.Parse(Request.Params["id"]);
            double percentOff;
            double.TryParse(PercentOffText.Text, out percentOff);

            double minAmount;
            double.TryParse(MinAmountText.Text, out minAmount);

            DateTime startDate;
            DateTime.TryParseExact(StartDateText.Text, "MM/dd/yyyy", null, DateTimeStyles.None, out startDate);
            DateTime endDate;
            DateTime.TryParseExact(EndDateText.Text, "MM/dd/yyyy", null, DateTimeStyles.None, out endDate);

            if (startDate.Date > endDate.Date)
            {
                LblMessage.Visible = true;
                LblMessage.Text = "Start Date should not be greater than End Date";
                return;
            }

            bool isCodeRequired;
            bool.TryParse(IsCodeRequiredHidden.Value, out isCodeRequired);

            bool isAllProducts;
            bool.TryParse(IsAllProductHidden.Value, out isAllProducts);

            if (discountId == 0)
            {
                _discounts = new Discounts
                {
                    DiscountName = DiscountNameText.Text.Trim(),
                    Code = CodeText.Text.Trim(),
                    StartDate = startDate,
                    EndDate = endDate,
                    CodeRequired = isCodeRequired,
                    PercentOff = percentOff,
                    PromoType = byte.Parse(PromoTypeDdl.SelectedValue),
                    MinAmount = minAmount,
                    IsAllProducts = isAllProducts,
                    MaxPurchases = byte.Parse(MaxPurchasesDdl.SelectedValue),
                    FinePrint = FinePrintText.Text,
                    BillingCycleNumber = byte.Parse(BillingCycleDdl.SelectedValue)
                };
                try
                {
                    discountId = _discountRepository.Add(_discounts);
                }
                catch (Exception ex)
                {
                    LblMessage.Visible = true;
                    LblMessage.Text = ex.Message;
                    return;
                }
            }
            else
            {
                _discounts = _discountRepository.GetById(discountId);
                _discounts.DiscountName = DiscountNameText.Text.Trim();
                _discounts.StartDate = startDate;
                _discounts.EndDate = endDate;
                _discounts.Code = CodeText.Text.Trim();
                _discounts.PercentOff = percentOff;
                _discounts.CodeRequired = isCodeRequired;
                _discounts.PromoType = byte.Parse(PromoTypeDdl.SelectedValue);
                _discounts.MinAmount = minAmount;
                _discounts.IsAllProducts = isAllProducts;
                _discounts.MaxPurchases = byte.Parse(MaxPurchasesDdl.SelectedValue);
                _discounts.FinePrint = FinePrintText.Text;
                _discounts.BillingCycleNumber = byte.Parse(BillingCycleDdl.SelectedValue);

                _discountRepository.Update(_discounts);
            }

            _discountRepository.ResetCache();
            RebindDiscount(discountId);

            Response.Redirect(Constant.PromoDetailPage + "?id=" + discountId);
        }

        protected void RebindDiscount(int discountId, bool isReload = false)
        {
            if (isReload)
            {
                _discountRepository = new DiscountRepository();
            }

            // Discount Products
            var products = _productRepository.SearchProductsByCode();

            var productDiscount = _discountRepository.SearchProductsByDiscountId(discountId);
            DdlHotels.DataSource = products.Except(productDiscount,new ProductComparer());
            DdlHotels.DataTextField = "HotelAndProductName";
            DdlHotels.DataValueField = "ProductId";
            DdlHotels.DataBind();

            List<DiscountProducts> discountHotels = _discountRepository.GetDiscountsProductsById(discountId);
            RptHotelListings.DataSource = discountHotels;
            RptHotelListings.DataBind();

            // Discount Subscriptions
            var subscriptions = _discountRepository.SubscriptionsList.Where(s => !s.IsDelete && s.IsActive).ToList();

            var subscriptionDiscount = _discountRepository.SearchSubscriptionsByDiscountId(discountId);
            DdlSubscription.DataSource = subscriptions.Except(subscriptionDiscount, new SubscriptionComparer());
            DdlSubscription.DataTextField = "Name";
            DdlSubscription.DataValueField = "Id";
            DdlSubscription.DataBind();

            List<DiscountSubscriptions> discountSubscriptionses = _discountRepository.GetDiscountSubscriptionsById(discountId);
            RptSubscriptionListings.DataSource = discountSubscriptionses;
            RptSubscriptionListings.DataBind();
        }

        private void BindMaxPurchases()
        {
            for (var i = 1; i <= 20; i++)
            {
                MaxPurchasesDdl.Items.Add(new ListItem(i.ToString()));
                BillingCycleDdl.Items.Add(new ListItem(i.ToString()));
            }
        }

        #region Subscription

        protected void RptSubscriptionListings_OnItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var rowHistory = (HtmlGenericControl)e.Item.FindControl("liAlternatie");
                rowHistory.Attributes.Add("class", "alternative");
            }
        }

        protected void RemovePromoSubscriptionClick(object sender, EventArgs e)
        {
            int discountId = int.Parse(Request.Params["id"]);
            List<DiscountSubscriptions> discountHotelses = new List<DiscountSubscriptions>();
            foreach (RepeaterItem item in RptSubscriptionListings.Items)
            {
                //to get the dropdown of each line
                CheckBox chkAmentyList = (CheckBox)item.FindControl("ChkRemove");

                if (chkAmentyList.Checked)
                {
                    HiddenField hidProductId = (HiddenField)item.FindControl("HidProductId");
                    HiddenField hidDiscountId = (HiddenField)item.FindControl("HidDiscountId");
                    discountHotelses.Add(new DiscountSubscriptions
                    {
                        DiscountId = int.Parse(hidDiscountId.Value),
                        SubscriptionId = int.Parse(hidProductId.Value)
                    });
                }
            }
            _discountRepository.RemoveDiscountSubscriptions(discountHotelses);
            _discountRepository.ResetCache();
            RebindDiscount(discountId, true);
        }

        protected void BtnAddDiscountSubscription_OnClick(object sender, EventArgs e)
        {
            int subscriptionId;
            int.TryParse(DdlSubscription.SelectedValue, out subscriptionId);
            if (subscriptionId != 0)
            {
                int discountId = int.Parse(Request.Params["id"]);
                _discountRepository.AddDiscountSubscriptions(new DiscountSubscriptions
                {
                    DiscountId = discountId,
                    SubscriptionId = subscriptionId
                });
                _discountRepository.ResetCache();
                RebindDiscount(discountId, true);
            }
        }

        protected void BtnAddAllDiscountSubscription_OnClick(object sender, EventArgs e)
        {
            var allSubscriptions = _productRepository.SubscriptionsList.Where(s => !s.IsDelete && s.IsActive);
            int discountId = int.Parse(Request.Params["id"]);
            List<DiscountSubscriptions> newDiscountSubscriptions = new List<DiscountSubscriptions>();

            var subscriptions = _discountRepository.SearchSubscriptionsByDiscountId(discountId);

            allSubscriptions.Except(subscriptions).ForEach(subscription =>
            {
                newDiscountSubscriptions.Add(new DiscountSubscriptions
                {
                    SubscriptionId = subscription.Id,
                    DiscountId = discountId
                });
            });

            _discountRepository.AddAllDiscountSubscriptions(newDiscountSubscriptions);
            _discountRepository.ResetCache();

            RebindDiscount(discountId, true);
        }

        #endregion
    }
}