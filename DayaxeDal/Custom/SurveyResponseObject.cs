using System;

namespace DayaxeDal.Custom
{
    public class SurveyResponseObject
    {
        public int HotelId { get; set; }

        public int BookingId { get; set; }

        public string SurveyCode { get; set; }

        public int CustomerId { get; set; }

        public double Rating { get; set; }

        public string RateCommend { get; set; }

        public DateTime UpdatedDate { get; set; }

        public string UpdatedDateWithFormat { get; set; }

        public string ByUser { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }
    }
}
