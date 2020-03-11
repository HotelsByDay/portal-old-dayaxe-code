namespace DayaxeDal.Parameters
{
    public class SearchAllBookingsTodayParams
    {
        public int HotelId { get; set; }

        public bool IsForRevenue { get; set; }

        public string FilterText { get; set; }
    }
}
