namespace DayaxeDal.Custom
{
    public class GoogleTimeZoneObject
    {
        public int DstOffset { get; set; }

        public long RawOffset { get; set; }

        public string Status { get; set; }

        public string TimeZoneId { get; set; }

        public string TimeZoneName { get; set; }
    }

    public class GoogleAddressResultsObject
    {
        public GoogleAddressObject[] Results { get; set; }

        public string Status { get; set; }
    }

    public class GoogleAddressObject
    {
        public GoogleLocationObject Geometry { get; set; }
    }

    public class GoogleLocationObject
    {
        public GoogleLatLongObject Location { get; set; }
    }

    public class GoogleLatLongObject
    {
        public double Lat { get; set; }

        public double Lng { get; set; }
    }
}
