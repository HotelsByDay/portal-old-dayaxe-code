using System.Collections.Generic;
namespace DayaxeDal
{
    public class ResponseData
    {
        public bool IsSuccessful { get; set; }
        public List<string> Message { get; set; }
        public string AccessKey { get; set; }
        public string PasswordKey { get; set; }
        public string Password { get; set; }

        public int CustomerId { get; set; }

        public string SearchPageUrl { get; set; }
        public string StripeCustomerId { get; set; }

        public string ReferralCode { get; set; }
    }
}
