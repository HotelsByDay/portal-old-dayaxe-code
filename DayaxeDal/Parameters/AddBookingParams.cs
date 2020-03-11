namespace DayaxeDal.Parameters
{
    public class AddBookingParams
    {
        public Bookings BookingObject { get; set; }

        public int DiscountId { get; set; }

        public CustomerCredits CustomerCreditObject { get; set; }

        public string Description { get; set; }

        public int SubscriptionDiscountId { get; set; }

        public bool IsUpgradeToSubscription { get; set; }
    }
}
