using System;
namespace DayaxeDal.Parameters
{
    public class SearchBookingsParams
    {
        public int HotelId { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public bool IsBookingForRevenue { get; set; }

        public string FilterText { get; set; }
    }
}
