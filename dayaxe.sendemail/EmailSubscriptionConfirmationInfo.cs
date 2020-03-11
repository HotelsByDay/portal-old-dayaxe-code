namespace Dayaxe.SendEmail
{
    public class EmailSubscriptionConfirmationInfo
    {
        public string UserEmail { get; set; }
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string FullName
        {
            get { return string.Format("{0} {1}", FirstName, LastName); }
        }

        public string SubscriptionName { get; set; }

        public string ImageUrl { get; set; }

        public string SubscriptionDescription { get; set; }

        public string UrlReserveDayPass { get; set; }

        public string PurchasedDate { get; set; }

        public string MembershipId { get; set; }

        public string Price { get; set; }

        public string Credit { get; set; }

        public string TotalPrice { get; set; }

        public string FinePrint { get; set; }

        public string UrlLinkToTerms
        {
            get { return EmailConfig.TermsUrl; }
        }

        public string Discount { get; set; }

        public string DiscountFinePrint { get; set; }
    }
}
