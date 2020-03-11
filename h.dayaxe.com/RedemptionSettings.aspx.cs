using System;
using System.Collections.Generic;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using DayaxeDal;
using DayaxeDal.Repositories;

namespace h.dayaxe.com
{
    public partial class RedemptionSettings : BasePage
    {
        private HotelRepository _hotelRepository = new HotelRepository();
        private ProductRepository _productRepository = new ProductRepository();
        private Hotels _hotels;
        private bool _isReload = false;

        protected void Page_Init(object sender, EventArgs e)
        {
            string sessionHotel = Session["Hotel"] != null ? Session["Hotel"].ToString() : string.Empty;
            _hotels = _hotelRepository.GetById(int.Parse(sessionHotel));
            if (_hotels == null)
            {
                Response.Redirect(Constant.HotelList);
            }
            Session["Active"] = "Iventory";

            // Add item 1 to 70 
            Helper.CreateControl(DailyCapacity);
            Helper.CreateControl(CabanaDailyLimit);
            Helper.CreateControl(SpaPassDailyLimit);
            Helper.CreateControl(DayBedDailyLimit);

            if (!IsPostBack)
            {
                ReloadPass();
            }
            if (_hotels != null)
            {
                var addOns = _productRepository.GetByHotelId(_hotels.HotelId, (int) Enums.ProductType.AddOns);
                RptAddOns.DataSource = addOns;
                RptAddOns.DataBind();
            }
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
            var passLimit = _hotelRepository.GetDailyPassLimit(_hotels.HotelId);
            CurrentPass.Text = passLimit.DailyPass.ToString();
            HidPass.Value = passLimit.DailyPass.ToString();

            CurrentCabana.Text = passLimit.CabanaPass.ToString();
            HidCabana.Value = passLimit.CabanaPass.ToString();

            CurrentSpa.Text = passLimit.SpaPass.ToString();
            HidSpaPass.Value = passLimit.SpaPass.ToString();

            CurrentDayBed.Text = passLimit.DaybedPass.ToString();
            HidDayBed.Value = passLimit.DaybedPass.ToString();

            var addOns = _productRepository.GetByHotelId(_hotels.HotelId, (int)Enums.ProductType.AddOns);
            RptAddOns.DataSource = addOns;
            RptAddOns.DataBind();
        }

        protected void SavePassClick(object sender, EventArgs e)
        {
            int dayPass = int.Parse(HidPass.Value);
            int cabanaPass = int.Parse(HidCabana.Value);
            int spaPass = int.Parse(HidSpaPass.Value);
            int dayBedPass = int.Parse(HidDayBed.Value);
            var passLimit = new DailyPassLimit
            {
                HotelId = _hotels.HotelId,
                DailyPass = dayPass,
                CabanaPass = cabanaPass,
                SpaPass = spaPass,
                DaybedPass = dayBedPass
            };
            // TODO: didn't allow to compile
            //_hotelRepository.UpdateDailyPassLimit(passLimit, "");

            if (RptAddOns.Items.Count > 0)
            {
                var listProducts = new List<Products>();
                foreach (RepeaterItem item in RptAddOns.Items)
                {
                    //to get the dropdown of each line
                    HiddenField productIdHid = (HiddenField)item.FindControl("HidId");

                    var product = _productRepository.GetById(int.Parse(productIdHid.Value));
                    var dailySalesHid = (HiddenField)item.FindControl("HidDailySales");
                    product.PassCapacity = 1;

                    if (!string.IsNullOrEmpty(dailySalesHid.Value))
                    {
                        product.PassCapacity = int.Parse(dailySalesHid.Value);
                    }

                    listProducts.Add(product);
                }

                // TODO didn't allow to compile
                //_productRepository.Update(listProducts);
            }

            _hotelRepository.ResetCache();

            ReloadPass(true);

            CurrentPass.Text = dayPass.ToString();

            saving.InnerText = "Saved!";
            saving.Attributes["class"] = "saving";
            ClientScript.RegisterClientScriptBlock(GetType(), "hideSaved", "setTimeout(function(){ $('.saving').animate({ opacity: 0 }, 1400, function () { });}, 100);", true);
        }

        protected void RptAddOns_OnItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var products = (Products) e.Item.DataItem;
                var dailySales = (Label)e.Item.FindControl("CurrentDailySales");
                var hidDailySales = (HiddenField) e.Item.FindControl("HidDailySales");
                var dailySalesLimitControl = (HtmlGenericControl) e.Item.FindControl("DailySalesLimit");
                var hidId = (HiddenField) e.Item.FindControl("HidId");

                hidId.Value = products.ProductId.ToString();

                if (!IsPostBack || _isReload)
                {
                    dailySales.Text = products.PassCapacity.ToString();
                    hidDailySales.Value = products.PassCapacity.ToString();
                }

                Helper.CreateControl(dailySalesLimitControl);
            }
        }
    }
}