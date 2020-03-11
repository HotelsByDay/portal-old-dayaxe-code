namespace Dayaxe.SendEmail
{
    public class EmailGoldPassNoShowInfo
    {
        public string UserEmail { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string FullName
        {
            get { return string.Format("{0} {1}", FirstName, LastName); }
        }

        public string ProductName { get; set; }

        public string HotelName { get; set; }

        public string Neighborhood { get; set; }

        public string CheckinDate { get; set; }

        public string BookingCode { get; set; }

        public string ChargeDate { get; set; }

        public string ChargeAmount { get; set; }

        public string ChargeAccount { get; set; }
    }
}
