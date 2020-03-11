using System;
using System.Collections.Generic;

namespace DayaxeDal
{
    public class SearchParams
    {
        public Markets SearchMarkets { get; set; }

        public int CustomerId { get; set; }

        public List<Enums.ProductType> ProductType { get; set; }

        public int AvailableTickets { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public double LowPrice { get; set; }

        public double HighPrice { get; set; }

        public double LowDistance { get; set; }

        public double HighDistance { get; set; }

        public bool IsForceResetFilter { get; set; }
    }
}
