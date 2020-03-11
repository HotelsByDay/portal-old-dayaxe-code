using System;
using System.Collections.Generic;
using DayaxeDal.Extensions;

namespace DayaxeDal
{
    public partial class Bookings : ICloneable
    {
        //[JsonIgnore]
        //public string Access
        //{
        //    get
        //    {
        //        List<string> poolAccess = new List<string>();
        //        if (Products.Hotels.AmentiesItem.PoolActive)
        //        {
        //            poolAccess.Add("pool");
        //        }
        //        if (Products.Hotels.AmentiesItem.GymActive)
        //        {
        //            poolAccess.Add("gym");
        //        }
        //        if (Products.Hotels.AmentiesItem.SpaActive)
        //        {
        //            poolAccess.Add("spa");
        //        }
        //        if (Products.Hotels.AmentiesItem.BusinessActive)
        //        {
        //            poolAccess.Add("business services");
        //        }
        //        return string.Join(", ", poolAccess);
        //    }
        //}

        //[JsonIgnore]
        //public List<BlockedDates> GetNearBlockedDates
        //{
        //    get
        //    {
        //        var dayPassValid = Products.RedemptionPeriod >= 0 ? Products.RedemptionPeriod : Constant.DefaultRedemptionPeriod;
        //        return Products.Hotels.BlockedDates
        //            .Where(x => x.Date.HasValue
        //                && x.Date.Value.Date >= DateTime.UtcNow.ToLosAngerlesTime().Date
        //                && x.IsActive.HasValue && x.IsActive.Value
        //                && ExpiredDate.HasValue
        //                && x.Date.Value.Date <= ExpiredDate.Value.Date
        //                && x.ProductTypeId == Products.ProductType)
        //            .DistinctBy(x => x.Date)
        //            .OrderBy(x => x.Date)
        //            .Take(dayPassValid)
        //            .ToList();
        //    }
        //}

        //[JsonIgnore]
        //public string BlackoutDays
        //{
        //    get { return Helper.GetCompactBlackout(GetNearBlockedDates); }
        //}

        public string BookingIdString
        {
            get
            {
                IEnumerable<string> parts = BookingCode.SplitInParts(4);
                return String.Join(" ", parts);
            }
        }

        public string FullName
        {
            get { return string.Format("{0} {1}", CustomerInfos.FirstName, CustomerInfos.LastName); }
        }

        public string PassExpires
        {

            get
            {
                DateTime expiredDate = ExpiredDate ?? DateTime.UtcNow;
                var passExpires = (expiredDate - DateTime.UtcNow).Days;
                return (passExpires > 0 ? passExpires : 0) + " days";
            }
        }

        public double UserRating { get; set; }

        public string BookingStatusString
        {
            get
            {
                string status = @"<span class='not-redeemed'>Not Redeemed</span>";
                switch (PassStatus)
                {
                    case (int)Enums.BookingStatus.Expired:
                        status = @"<span class='not-redeemed'>Expired</span>";
                        break;
                    case (int)Enums.BookingStatus.Redeemed:
                        status = @"<span class='redeemed'>Redeemed</span>";
                        break;
                    case (int)Enums.BookingStatus.Refunded:
                        status = @"<span class='not-redeemed'>Refunded</span>";
                        break;
                }
                return status;
            }
        }

        public double PassRevenue
        {
            get { return Quantity * MerchantPrice; }
        }

        public double? EstSpend { get; set; }

        public string ProductName { get; set; }

        public string BookingsTypeString { get; set; }

        public int UpdatedBy { get; set; }

        public byte PaymentType { get; set; }

        public string StripeCardString { get; set; }

        public string SurveyFeedback { get; set; }

        public string TimeZoneId { get; set; }

        public bool IsMaintainInvoices { get; set; }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
