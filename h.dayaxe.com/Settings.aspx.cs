using System;
using System.Web.UI.HtmlControls;

namespace h.dayaxe.com
{
    public partial class Settings : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Session["Active"] = "Setting";
            for (var i = 1; i <= 70; i++)
            {
                var liTag = new HtmlGenericControl("li");
                var link = new HtmlAnchor();
                link.HRef = "#";
                link.InnerText = i.ToString();
                liTag.Controls.Add(link);
                DailyCapacity.Controls.Add(liTag);
            }

            for (var i = 1; i <= 300; i++)
            {
                var liTag = new HtmlGenericControl("li");
                var link = new HtmlAnchor();
                link.HRef = "#";
                link.InnerText = i.ToString() + " mi";
                liTag.Controls.Add(link);
                Distance.Controls.Add(liTag);
            }
        }
    }
}