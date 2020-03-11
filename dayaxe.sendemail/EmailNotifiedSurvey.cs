using System.Collections.Generic;

namespace Dayaxe.SendEmail
{
    public class EmailNotifiedSurvey
    {
        public List<string> UserEmail { get; set; }

        public string ReplyToEmail { get; set; }

        public string ReplyToName { get; set; }

        public List<string> EmailBccList { get; set; }

        public string HotelName { get; set; }

        public string ProductName { get; set; }

        public string Quantity { get; set; }

        public string CustomerFullName { get; set; }

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

        public string RateCommend { get; set; }

        public string TotalSpend { get; set; }
    }
}
