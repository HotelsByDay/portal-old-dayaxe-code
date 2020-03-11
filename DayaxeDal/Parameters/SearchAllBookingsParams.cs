namespace DayaxeDal.Parameters
{
    public class SearchAllBookingsParams
    {
        public int HotelId { get; set; }

        public bool IsForRevenue { get; set; }

        public string FilterText { get; set; }
    }
}
