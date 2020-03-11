namespace Dayaxe.SendEmail
{
    public class EmailRefundInfo
    {
        public string UserEmail { get; set; }

        public string HotelName { get; set; }

        public string Neightborhood { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }


        public string FullName
        {
            get { return string.Format("{0} {1}", FirstName, LastName); }
        }

        public string ProductName { get; set; }

        public string BookedDate { get; set; }

        public string BookingCode { get; set; }

        public string Tickets { get; set; }

        public string RefundDate { get; set; }

        public string RefundStripeAmount { get; set; }

        public string RefundCreditAmount { get; set; }

        public string RefundAmount { get; set; }

        public string UrlViewDayPass { get; set; }
    }
}
