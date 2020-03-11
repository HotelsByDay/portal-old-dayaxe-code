using System;
using System.Collections.Generic;
using DayaxeDal.Extensions;

namespace DayaxeDal
{
    public partial class SubscriptionBookings
    {
        public string BookingIdString
        {
            get
            {
                IEnumerable<string> parts = BookingCode.SplitInParts(4);
                return String.Join(" ", parts);
            }
        }

        public string Description { get; set; }
    }
}
