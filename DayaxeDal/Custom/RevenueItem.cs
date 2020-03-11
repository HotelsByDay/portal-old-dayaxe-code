namespace DayaxeDal.Custom
{
    public class RevenueItem
    {
        public int TicketRedeemed { get; set; }
        public double TicketRevenue { get; set; }

        public double AvgPerTicketSpend { get; set; }

        public double IncrementalRevenue { get; set; }

        public double SpendPerGuest
        {
            get
            {
                if (TicketRedeemed > 0)
                    return AvgPerTicketSpend / TicketRedeemed;
                return 0;
            }
        }

        public int AvgGuestPerBooking { get; set; }

        public double FoodDrink { get; set; }
        public double GiftShop { get; set; }
        public double AvgSpa { get; set; }
        public double Parking { get; set; }
        public double Other { get; set; }

        public double PoolPercent { get; set; }
        public double GymPercent { get; set; }
        public double SpaPercent { get; set; }
        public double BusinessCenterPercent { get; set; }

        public double TotalRevenue { get; set; }
    }
}
