using System;
using Newtonsoft.Json;

namespace DayaxeDal
{
    public partial class Surveys
    {
        [JsonIgnore]
        public string CustomerFullName
        {
            get { return string.Format("{0} {1}", Bookings.CustomerInfos.FirstName, Bookings.CustomerInfos.LastName); }
        }

        public DateTime? RedeemedDate { get; set; }

        public double HotelPrice { get; set; }

        public string ImageUrl { get; set; }

        public string HotelInfo { get; set; }

        public double EstSpend { get; set; }

        public string UserRating
        {
            get
            {
                string str = string.Empty;
                if (Rating.HasValue && Rating.Value > 0)
                {
                    str = Helper.GetRatingString(Rating.Value, false);
                }

                return str;
            }
        }

        public string UserRatingWeb
        {
            get
            {
                string str = string.Empty;
                if (Rating.HasValue && Rating.Value > 0)
                {
                    str = Helper.GetRatingString(Rating.Value);
                }

                return str;
            }
        }

        [JsonIgnore]
        public string ByUser
        {
            get
            {
                var customerInfos = Helper.GetCustomerInfosByBookingId(BookingId);
                 return string.Format("{0} {1}.", 
                     !string.IsNullOrEmpty(customerInfos.FirstName) ? customerInfos.FirstName : string.Empty, 
                     (!string.IsNullOrEmpty(customerInfos.LastName) ? customerInfos.LastName[0] : ' ')); 
            }
        }

        [JsonIgnore]
        public int Quantity
        {
            get { return Bookings.Quantity; }
        }

        public int TicketPurchased { get; set; }

        public string HotelName { get; set; }

        public string City { get; set; }

        public string ProductName { get; set; }

        public DateTime? CheckInDate { get; set; }

        public double PerTicketPrice { get; set; }

        public double TotalPrice { get; set; }
    }
}
