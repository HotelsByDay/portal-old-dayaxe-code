using System.Collections.Generic;

namespace DayaxeDal.Custom
{
    public class ReturnSubscriptionInvoiceObject
    {
        public string Id { get; set; }

        public string Charge { get; set; }

        public string Customer { get; set; }

        public LinesObject Lines { get; set; }

        public bool Paid { get; set; }

        public double? Aamount_Due { get; set; }

        public int? Attempt_Count { get; set; }

        public string Subscription { get; set; }

        public StripeDiscountObject Discount { get; set; }
    }

    public class LinesObject
    {
        public List<StripeSubscriptionObject> Data { get; set; }
    }

    public class StripeSubscriptionObject
    {
        public string Id { get; set; }

        public string Object { get; set; }

        public double Amount { get; set; }

        public string Currency { get; set; }

        public string Description { get; set; }

        public bool Discountable { get; set; }

        public bool Livemode { get; set; }

        public StripePeriodObject Period { get; set; }

        public StripePlanObject Plan { get; set; }
    }

    public class StripePeriodObject
    {
        public long Start { get; set; }

        public long End { get; set; }
    }

    public class StripePlanObject
    {
        public string Id { get; set; }

        public double Amount { get; set; }

        public long Created { get; set; }

        public string Currency { get; set; }

        public string Interval { get; set; }

        public string Name { get; set; }
    }

    public class StripeDiscountObject
    {
        public CouponObject Coupon { get; set; }
    }

    public class CouponObject
    {
        public string Id { get; set; }

        public string Amount_Off { get; set; }

        public bool Valid { get; set; }
    }
}
