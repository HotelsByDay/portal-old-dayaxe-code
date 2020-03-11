namespace DayaxeDal
{
    public partial class DiscountProducts
    {
        public string HotelInfo
        {
            get { return string.Format("{0} - {1}, {2}", Products.Hotels.HotelName, Products.ProductName, Products.Hotels.Neighborhood); }
        }
    }
}
