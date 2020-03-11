using System;
using System.Web.UI;
using DayaxeDal;
using DayaxeDal.Repositories;

namespace dayaxe.com.ErrorPages
{
    public partial class resource_not_found : Page
    {
        private readonly CustomerInfoRepository _helper = new CustomerInfoRepository();
        private CustomerInfos _customerInfos;

        protected void Page_Init(object sender, EventArgs e)
        {
            var currentSession = Session["UserSession"];
            if (currentSession != null && _customerInfos == null)
            {
                _customerInfos = _helper.GetCustomerInfoBySessionId(currentSession.ToString());
                if (_customerInfos != null && !string.IsNullOrEmpty(_customerInfos.BrowsePassUrl))
                {
                    HomeLink.NavigateUrl = _customerInfos.BrowsePassUrl;
                }
                else if (!string.IsNullOrEmpty((string)Session["SearchPage"]))
                {
                    HomeLink.NavigateUrl = Session["SearchPage"].ToString();
                }
            }
            else
            {
                HomeLink.NavigateUrl = Constant.SearchPageDefault;
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {

        }
    }
}