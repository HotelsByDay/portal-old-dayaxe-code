using System.Collections.Generic;

namespace Dayaxe.SendEmail
{
    public class EmailMonthlySalesReportInfo
    {
        public List<string> UserEmail { get; set; }

        public string HotelNameWithNeighborhood { get; set; }

        public string SelectedMonth { get; set; }

        public string GrossSales { get; set; }

        public string IncreaseFromMonth { get; set; }

        public string IncreaseFromPreviousMonth { get; set; }

        public string GrossSalesDayPass { get; set; }

        public string GrossSalesCabana { get; set; }

        public string Incremental { get; set; }

        public string TicketSold { get; set; }

        public string TicketSoldDayPass { get; set; }

        public string TicketSoldCabana { get; set; }

        public string Utilization { get; set; }

        public string UtilizationWeedDays { get; set; }

        public string UtilizationWeedkens { get; set; }

        public string AvgSpendGuest { get; set; }

        public string AvgSpendCabana { get; set; }
    }
}
