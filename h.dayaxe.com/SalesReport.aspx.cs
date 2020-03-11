using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using DayaxeDal;
using DayaxeDal.Custom;
using DayaxeDal.Repositories;
using DayaxeDal.Ultility;

namespace h.dayaxe.com
{
    public partial class SalesReport : BasePageProduct
    {
        readonly BookingRepository _bookingRepository = new BookingRepository();
        private List<ProductSalesReportObject> _result;
 
        protected void Page_Init(object sender, EventArgs e)
        {
            Session["Active"] = "SalesReport";
            if (!IsPostBack)
            {
                _result = _bookingRepository.GetSalesReportByHotelId(PublicHotel.HotelId, PublicHotel.TimeZoneId, int.Parse(YearDdl.SelectedValue));
                SaleReportRpt.DataSource = _result;
                SaleReportRpt.DataBind();
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void ExportToExcelButtonOnClick(object sender, EventArgs e)
        {
            string[] columns = { "MonthName", "Inventory", "Utilization",
                "TicketsSold", "TicketsRedeemed", "TicketsExpired", "TicketsRefunded",
                "GrossSales", "NetSales", "NetRevenue", "AvgIncrementalRevenue",
                "PercentSold", "PercentRedeemed", "PercentExpired", "PercentRefunded" };

            _result = _bookingRepository.GetSalesReportByHotelId(PublicHotel.HotelId, PublicHotel.TimeZoneId, int.Parse(YearDdl.SelectedValue));
            byte[] filecontent = ExcelExportHelper.ExportExcel(_result, columns);

            var fileName = string.Format("SalesReport_{2}_{0}_{1:yyyyMMdd}.xlsx",
                PublicHotel.HotelName.Replace(" ", "_"), DateTime.Now, YearDdl.SelectedValue);

            using (var writer = new BinaryWriter(File.OpenWrite(Server.MapPath(string.Format("/Helper/{0}", fileName)))))
            {
                writer.Write(filecontent);
                writer.Flush();
                writer.Close();
            }

            Response.ContentType = ExcelExportHelper.ExcelContentType;
            Response.AppendHeader("Content-Disposition", "attachment; filename=" + fileName);
            Response.TransmitFile(Server.MapPath("~/Helper/" + fileName));
            Response.End();
        }

        protected void SaleReportRpt_OnItemDataBound(object sender, RepeaterItemEventArgs e)
        {

        }

        protected void ProductsRpt_OnItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Footer)
            {
                var totalIventoryLit = (Literal)e.Item.FindControl("TotalIventoryLit");
                var totalUtilizationLit = (Literal)e.Item.FindControl("TotalUtilizationLit");
                var totalTicketsSoldLit = (Literal)e.Item.FindControl("TotalTicketsSoldLit");
                var totalTicketsRedeemedLit = (Literal)e.Item.FindControl("TotalTicketsRedeemedLit");
                var totalTicketsExpiredLit = (Literal)e.Item.FindControl("TotalTicketsExpiredLit");
                var totalTicketsRefundedLit = (Literal)e.Item.FindControl("TotalTicketsRefundedLit");
                var totalGrossSalesLit = (Literal)e.Item.FindControl("TotalGrossSalesLit");
                var totalNetSalesLit = (Literal)e.Item.FindControl("TotalNetSalesLit");
                var totalNetRevenueLit = (Literal)e.Item.FindControl("TotalNetRevenueLit");
                var totalAvgIncrementalRevenueLit = (Literal)e.Item.FindControl("TotalAvgIncrementalRevenueLit");
                var totalPercentSoldLit = (Literal)e.Item.FindControl("TotalPercentSoldLit");
                var totalPercentRedeemedLit = (Literal)e.Item.FindControl("TotalPercentRedeemedLit");
                var totalPercentExpiredLit = (Literal)e.Item.FindControl("TotalPercentExpiredLit");
                var totalPercentRefundedLit = (Literal)e.Item.FindControl("TotalPercentRefundedLit");

                var repeater = (Repeater)sender;
                var parentItem = (RepeaterItem)repeater.NamingContainer;
                var parentDataItem = (ProductSalesReportObject)parentItem.DataItem;
                var data = parentDataItem.SalesReportObject;

                int inventory = data.Sum(x => x.Inventory);
                int ticketSold = data.Sum(x => x.TicketsSold);
                int ticketRedeemed = data.Sum(x => x.TicketsRedeemed);
                int ticketExpired = data.Sum(x => x.TicketsExpired);
                int ticketRefunded = data.Sum(x => x.TicketsRefunded);

                totalIventoryLit.Text = inventory.ToString();
                totalUtilizationLit.Text = data.Count != 0 ? string.Format("{0:0}%", data.Average(x => x.Utilization)) : "0%";
                totalTicketsSoldLit.Text = ticketSold.ToString();
                totalTicketsRedeemedLit.Text = ticketRedeemed.ToString();
                totalTicketsExpiredLit.Text = ticketExpired.ToString();
                totalTicketsRefundedLit.Text = ticketRefunded.ToString();
                totalGrossSalesLit.Text = Helper.FormatPrice(data.Sum(x => x.GrossSales));
                totalNetSalesLit.Text = Helper.FormatPrice(data.Sum(x => x.NetSales));
                totalNetRevenueLit.Text = Helper.FormatPrice(data.Sum(x => x.NetRevenue));
                totalAvgIncrementalRevenueLit.Text = Helper.FormatPrice(data.Sum(x => x.AvgIncrementalRevenue));

                totalPercentSoldLit.Text = (inventory != 0 ? ticketSold * 100 / inventory : 0) + "%";
                totalPercentRedeemedLit.Text = (ticketSold != 0 ? ticketRedeemed * 100 / ticketSold : 0) + "%";
                totalPercentExpiredLit.Text = (ticketSold != 0 ? ticketExpired * 100 / ticketSold : 0) + "%";
                totalPercentRefundedLit.Text = (ticketSold != 0 ? ticketRefunded * 100 / ticketSold : 0) + "%";
            }

            if (e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var rowHistory = (HtmlTableRow)e.Item.FindControl("rowReport");
                rowHistory.Attributes.Add("class", rowHistory.Attributes["class"] + " alternative");
            }
        }

        protected void YearChange(object sender, EventArgs e)
        {
            var selectedYear = int.Parse(YearDdl.SelectedValue);
            _result = _bookingRepository.GetSalesReportByHotelId(PublicHotel.HotelId, PublicHotel.TimeZoneId, selectedYear);
            SaleReportRpt.DataSource = _result;
            SaleReportRpt.DataBind();
        }
    }
}