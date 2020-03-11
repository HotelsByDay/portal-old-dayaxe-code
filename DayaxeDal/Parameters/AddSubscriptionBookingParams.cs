namespace DayaxeDal.Parameters
{
    public class AddSubscriptionBookingParams
    {
        public double RefundCreditByUpgrade { get; set; }

        public string RefundCreditDescription { get; set; }

        public string SubscriptionName { get; set; }

        public SubscriptionBookings SubscriptionBookingsObject { get; set; }

        public CustomerCredits CustomerCreditsObject { get; set; }

        public string Description { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public long BookingId { get; set; }

        public Discounts SubscriptionBookingDiscounts { get; set; }

        public double ActualPrice { get; set; }

        public double MerchantPrice { get; set; }

        public double PayByCredit { get; set; }

        public double TotalPrice { get; set; }

        public int MaxPurchases { get; set; }
    }
}
