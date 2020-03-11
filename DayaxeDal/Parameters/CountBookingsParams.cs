using System;

namespace DayaxeDal.Parameters
{
    public class CountBookingsParams
    {
        public int HotelId { get; set; }

        public DateTime Date { get; set; }

        public int ProductTypeId { get; set; }
    }
}
