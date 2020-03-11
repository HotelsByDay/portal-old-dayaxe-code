using System;
using Newtonsoft.Json;

namespace DayaxeDal
{
    public partial class Discounts: ICloneable
    {
        [JsonIgnore]
        public Enums.DiscountStatus Status { get; set; }
        public int DiscountUses { get; set; }
        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
