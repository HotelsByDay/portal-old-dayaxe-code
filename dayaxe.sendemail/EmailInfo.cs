using System.Collections.Generic;

namespace Dayaxe.SendEmail
{
    public class EmailInfo
    {
        public string UserEmail { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string FullName
        {
            get { return string.Format("{0} {1}", FirstName, LastName); }
        }

        public string HotelName { get; set; }

        public string Neighborhood { get; set; }

        public string HotelImage { get; set; }

        public double Rating { get; set; }

        public string RatingString
        {
            get
            {
                string str = string.Empty;
                for (var i = 0; i <= 4; i++)
                {
                    string url;

                    if (Rating - i >= 1)
                    {
                        url = EmailConfig.FullStar;
                    }
                    else if (Rating - i > 0)
                    {
                        url = EmailConfig.HalfStar;
                    }
                    else
                    {
                        url = EmailConfig.EmptyStar;
                    }
                    str += string.Format("<img src=\"{0}\" style=\"height:auto; width:14px !important; display: inline-block;\" class=\"img-responsive star\" alt=\"star\" />", url);
                }
                return str;
            }
        }

        public double CustomerRating { get; set; }

        public int TotalCustomerReviews { get; set; }

        public string CustomerRatingString
        {
            get
            {
                string str = string.Empty;
                if (CustomerRating > 0)
                {
                    for (var i = 0; i <= 4; i++)
                    {
                        string url;

                        url = CustomerRating - i >= 0.5 ? EmailConfig.FullStar : EmailConfig.EmptyStar;
                        str += string.Format(
                            "<img src=\"{0}\" style=\"height:auto; width:14px !important; display: inline-block;\" class=\"img-responsive star\" alt=\"star\" />",
                            url);
                    }
                    str += string.Format("<span style=\"padding-left:8px;color:white;\">{0}</span>", TotalCustomerReviews);
                }
                return str;
            }
        }

        public string UrlViewDayPass { get; set; }

        public string Address { get; set; }

        public string AddressInfo { get; set; }

        public string PhoneNumber { get; set; }

        public string BookedDate { get; set; }

        //public string Access { get; set; }

        //public string BlackoutDays { get; set; }

        public string ExpiredDate { get; set; }

        public string BookingCode { get; set; }

        public string PerTicketPrice { get; set; }
        public string Tickets { get; set; }
        public string MaxPerTicket { get; set; }

        public string BookingTotal { get; set; }

        public List<string> EmailBccList { get; set; }

        public string UrlLinkToTerms
        {
            get { return EmailConfig.TermsUrl; }
        }

        public string MaxGuest { get; set; }

        public string Discount { get; set; }

        public string CheckInPlace { get; set; }

        public string RedeemedDate { get; set; }

        public string CheckInDate { get; set; }

        public string CheckInText { get; set; }

        public string ViewDayPassString { get; set; }

        public string ProductName { get; set; }

        public string ProductType { get; set; }
        public string FinePrintVisible { get; set; }

        public string AddOnString { get; set; }

        public string Updated { get; set; }

        public string UpdatedReceipt { get; set; }

        public string BookingStatus { get; set; }

        public string SubscriptionReminder { get; set; }

        public string DiscountFinePrint { get; set; }

        public string TotalTicketInfo { get; set; }

        public string TotalPriceInfo { get; set; }
    }
}
