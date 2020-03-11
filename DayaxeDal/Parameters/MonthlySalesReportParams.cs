using System.Collections.Generic;

namespace DayaxeDal.Parameters
{
    public class MonthlySalesReportParams
    {
        public int HotelId { get; set; }

        public string TimeZoneId { get; set; }

        public double GrossSale { get; set; }

        public int TicketSold { get; set; }

        public int TicketRedeemed { get; set; }

        public double GrossSaleDayPass { get; set; }

        public double GrossSaleCabana { get; set; }

        public List<Bookings> BookingsLastMonth { get; set; }

        public List<Bookings> BookingsDayPassLastMonth { get; set; }

        public List<Bookings> BookingsCabanaLastMonth { get; set; }

        public List<Surveys> SurveyLastMonth { get; set; }

        public List<Surveys> SurveyDayPassLastMonth { get; set; }

        public List<Surveys> SurveyCabanaLastMonth { get; set; }

        public List<Bookings> BookingsPreviousMonth { get; set; }

        public List<Products> Productses { get; set; }

        public int IventoryLastMonth { get; set; }

        public int IventoryDayPassLastMonth { get; set; }

        public int IventoryCabanalastMonth { get; set; }

        public int TotalBookingDayPassLastMonth { get; set; }

        public int TotalBookingCabanaLastMonth { get; set; }

        public int IventoryWeekdays { get; set; }

        public int IventoryWeekens { get; set; }

        public int TotalBookingWeekdaysLastMonth { get; set; }

        public int TotalBookingWeekensLastMonth { get; set; }
    }
}
