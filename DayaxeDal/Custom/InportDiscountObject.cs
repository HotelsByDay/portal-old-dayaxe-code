using System;

namespace DayaxeDal.Custom
{
    public class InportDiscountObject
    {
        public string DiscountName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Code { get; set; }
        public bool CodeRequired { get; set; }
        public string PromoType { get; set; }
        public string MinAmount { get; set; }
        public bool IsAllProduct { get; set; }
        public int MaxPurchases { get; set; }
    }
}
