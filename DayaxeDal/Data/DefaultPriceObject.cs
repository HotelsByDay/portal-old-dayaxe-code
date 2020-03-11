using System;

namespace DayaxeDal
{
    public class DefaultPriceObject
    {
        public int ProductId { get; set; }
        public string ProductNameAcronym { get; set; }
        public double PriceMon { get; set; }
        public double PriceTue { get; set; }
        public double PriceWed { get; set; }
        public double PriceThu { get; set; }
        public double PriceFri { get; set; }
        public double PriceSat { get; set; }
        public double PriceSun { get; set; }

        public double UpgradeDiscountMon { get; set; }
        public double UpgradeDiscountTue { get; set; }
        public double UpgradeDiscountWed { get; set; }
        public double UpgradeDiscountThu { get; set; }
        public double UpgradeDiscountFri { get; set; }
        public double UpgradeDiscountSat { get; set; }
        public double UpgradeDiscountSun { get; set; }

        public string DefaultCapacity { get; set; }

        public int DailyDayPass { get; set; }
        public int DailyCabana { get; set; }
        public int DailySpa { get; set; }
        public int DailyDaybed { get; set; }
        public DateTime EffectiveDate { get; set; }

        public double PassCapacityMon { get; set; }
        public double PassCapacityTue { get; set; }
        public double PassCapacityWed { get; set; }
        public double PassCapacityThu { get; set; }
        public double PassCapacityFri { get; set; }
        public double PassCapacitySat { get; set; }
        public double PassCapacitySun { get; set; }
    }
}
