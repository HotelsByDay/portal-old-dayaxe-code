using System;
using System.Collections.Generic;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using DayaxeDal;
using DayaxeDal.Repositories;

namespace h.dayaxe.com
{
    public partial class EditPolicy : BasePage
    {
        private HotelRepository _hotelRepository = new HotelRepository();
        protected void Page_Init(object sender, EventArgs e)
        {
            Session["Active"] = "EditPolicy";
            if (!IsPostBack)
            {
                BindPolicies();
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void AddPolicyButton_OnClick(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(PolicyNameText.Text))
            {
                return;
            }
            if (PolicyId.Value != "0")
            {
                var policies = _hotelRepository.GetPolicesById(int.Parse(PolicyId.Value));
                if (policies != null)
                {
                    policies.Name = PolicyNameText.Text.Trim();
                    _hotelRepository.UpdatePolicies(policies);
                }
            }
            else
            {
                var policies = new Policies
                {
                    IsDelete = false,
                    Name = PolicyNameText.Text.Trim()
                };
                _hotelRepository.AddPolicies(policies);
            }

            CacheLayer.Clear(CacheKeys.PoliciesCacheKey);
            BindPolicies(true);
        }

        protected void RptPoliciesListing_OnItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var rowHistory = (HtmlGenericControl)e.Item.FindControl("liAlternatie");
                rowHistory.Attributes.Add("class", "alternative");
            }
        }

        protected void RemoveButton_OnClick(object sender, EventArgs e)
        {
            List<long> policies = new List<long>();
            foreach (RepeaterItem item in RptPoliciesListing.Items)
            {
                //to get the dropdown of each line
                CheckBox chkAmentyList = (CheckBox)item.FindControl("ChkRemove");
                if (chkAmentyList.Checked)
                {
                    HiddenField hidId = (HiddenField)item.FindControl("HidId");
                    policies.Add(long.Parse(hidId.Value));
                }
            }
            _hotelRepository.DeletePolicies(policies);

            _hotelRepository.ResetCache();

            BindPolicies(true);
        }

        private void BindPolicies(bool isReload = false)
        {
            if (isReload)
            {
                _hotelRepository = new HotelRepository();
            }
            var policies = _hotelRepository.GetAllPolices();

            RptPoliciesListing.DataSource = policies;
            RptPoliciesListing.DataBind();
        }
    }
}