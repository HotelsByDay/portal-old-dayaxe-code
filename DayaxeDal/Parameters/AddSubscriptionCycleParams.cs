using System;
using System.Collections.Generic;

namespace DayaxeDal.Parameters
{
    public class AddSubscriptionCycleParams
    {
        public DateTime? CanceledDate { get; set; }

        public SubscriptionCycles SubscriptionCyclesObject { get; set; }

        public List<SubscriptionInvoices> SubscriptionInvoices { get; set; }
    }
}
