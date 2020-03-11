using System;

namespace DayaxeDal.Parameters
{
    public class CheckAvailableProductParams
    {
        public int ProductId { get; set; }

        public DateTime CheckInDate { get; set; }

        public int TotalTicket { get; set; }

        public int BookingId { get; set; }

        public bool IsAdmin { get; set; }

        public string TimezoneId { get; set; }
    }
}
