namespace Dayaxe.SendEmail
{
    public class EmailCheckInReminder
    {
        public string UserEmail { get; set; }
        public string ProductName { get; set; }
        public string Neighborhood { get; set; }
        public string HotelName { get; set; }
        public string CheckInDate { get; set; }
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

        public string MaxGuest { get; set; }

        public string Tickets { get; set; }

        public string FirstName { get; set; }

        public string ImageUrl { get; set; }

        public string CheckInPlace { get; set; }

        public string UrlViewDayPass { get; set; }

        public string AddOnString { get; set; }

        public string SubscriptionReminder { get; set; }
    }
}
