using System.ComponentModel;

namespace DayaxeDal.Custom
{
    public class ExportBookingObject
    {
        [Description("Booking ID")]
        public string BookingId { get; set; }
        [Description("Booking Code")]
        public string BookingCode { get; set; }
        [Description("Pass Status")]
        public string PassStatus { get; set; }
        [Description("Product Name")]
        public string ProductName { get; set; }
        [Description("Guest Name")]
        public string GuestName { get; set; }
        [Description("Booked Date")]
        public string BookedDate { get; set; }
        [Description("Check-in Date")]
        public string CheckInDate { get; set; }
        [Description("Redeemed")]
        public string RedeemedDate { get; set; }
        [Description("Ticket Quantity")]
        public string TicketQuantity { get; set; }
        [Description("Per Ticket Price")]
        public string PerTicketPrice { get; set; }
        [Description("Gross Earnings")]
        public string GrossEarnings { get; set; }
        [Description("Rating")]
        public string Rating { get; set; }
        [Description("Feedback")]
        public string Feedback { get; set; }
        [Description("Est Spend")]
        public string EstSpend { get; set; }
        [Description("Paid")]
        public string Paid { get; set; }
    }
}
