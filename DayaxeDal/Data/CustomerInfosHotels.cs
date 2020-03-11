namespace DayaxeDal
{
    public partial class CustomerInfosHotels
    {
        public string HotelInfo
        {
            get { return string.Format("{0}, {1}", Hotels.HotelName, Hotels.Neighborhood); }
        }
    }
}
