namespace DayaxeDal
{
    public partial class Markets
    {
        public int HotelsOfMarket { get; set; }

        public string HotelDisplayName
        {
            get { return string.Format("{0} - {1}", MarketCode, LocationName); }
        }

        public void SetDistanceWithSearchRegion(string lat, string lng)
        {
            if (!string.IsNullOrEmpty(lat)
                && !string.IsNullOrEmpty(lng)
                && !string.IsNullOrEmpty(Latitude)
                && !string.IsNullOrEmpty(Longitude))
            {
                float userLat;
                float userLng;
                float.TryParse(lat, out userLat);
                float.TryParse(lng, out userLng);

                float hotelLat;
                float hotelLng;
                float.TryParse(Latitude, out hotelLat);
                float.TryParse(Longitude, out hotelLng);

                DistanceWithSearchRegion = Helper.GetDistanceFromLatLonInMiles(userLng, userLat, hotelLng, hotelLat);
                return;
            }
            
            DistanceWithSearchRegion = 0;
        }

        public double DistanceWithSearchRegion { get; set; }
    }
}
