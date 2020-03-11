namespace DayaxeDal.Parameters
{
    public class GetDiscountValidByCodeParams
    {
        public string Code { get; set; }

        public int ProductId { get; set; }

        public int SubscriptionId { get; set; }

        public int CustomerId { get; set; }

        public bool IsAdmin { get; set; }

        public int BookingId { get; set; }

        public string TimezoneId { get; set; }
    }
}
