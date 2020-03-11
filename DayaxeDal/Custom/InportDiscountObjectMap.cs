using System;
using CsvHelper.Configuration;

namespace DayaxeDal.Custom
{
    public sealed class InportDiscountObjectMap: CsvClassMap<InportDiscountObject>
    {
        public InportDiscountObjectMap()
        {
            Map(m => m.DiscountName).Index(0);
            Map(m => m.StartDate).Index(1).ConvertUsing(row => row.GetField<DateTime>(1));
            Map(m => m.EndDate).Index(2).ConvertUsing(row => row.GetField<DateTime>(2));
            Map(m => m.Code).Index(3);
            Map(m => m.CodeRequired).Index(4).TypeConverterOption(true, "TRUE");
            Map(m => m.PromoType).Index(5);
            Map(m => m.MinAmount).Index(6);
            Map(m => m.IsAllProduct).Index(7).TypeConverterOption(true, "TRUE");
            Map(m => m.MaxPurchases).Index(8).ConvertUsing(row => row.GetField<int>(8));
        }
    }
}
