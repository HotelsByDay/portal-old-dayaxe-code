using System;

namespace DayaxeDal.Custom
{
    public class ReportAvailObject
    {
        public string HotelName { get; set; }
        public int HotelId { get; set; }
        public int Available { get; set; }
        public int Sold { get; set; }
        public DateTime Date { get; set; }
    }
}
