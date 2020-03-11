using System.Web.Configuration;
using DayaxeDal.Extensions;

namespace DayaxeDal.Custom
{
    public class SalesReportObject
    {
        public Enums.MonthType Month { get; set; }

        public string MonthName
        {
            get { return Month.ToDescription(); }
        }

        public int Inventory { get; set; }

        public double Utilization { get; set; }

        public int TicketsSold { get; set; }

        public int TicketsRedeemed { get; set; }

        public int TicketsExpired { get; set; }

        public int TicketsRefunded { get; set; }

        public double GrossSales { get; set; }

        public double NetSales { get; set; }

        public double NetRevenue { get; set; }

        public double AvgIncrementalRevenue { get; set; }

        public float PercentSold { get; set; }

        public float PercentRedeemed { get; set; }

        public float PercentExpired { get; set; }

        public float PercentRefunded { get; set; }
    }
}
