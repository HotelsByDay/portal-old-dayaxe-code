using System;
using System.Collections.Generic;
using System.Linq;
using DayaxeDal.Custom;
using Newtonsoft.Json;

namespace DayaxeDal
{
    public partial class Products
    {
        public string ImageUrl { get; set; }

        public string ImageAddOnUrl { get; set; }

        public string AdminImageUrl { get; set; }

        public int CustomOrder { get; set; }

        public double DistanceWithUser { get; set; }

        public string CdnImage { get; set; }

        public int BookingsToday { get; set; }

        public string[] PoolFeatures { get; set; }

        public string[] GymFeatures { get; set; }

        public string[] SpaFeatures { get; set; }

        public string[] OfficeFeatures { get; set; }

        public string[] PhotoUrls { get; set; }

        public Amenties AmentiesHotels { get; set; }

        public List<BlockedDatesCustomPrice> GetNearBlockedDates { get; set; }

        public bool IsOnBlackOutDay { get; set; }

        public DateTime NextAvailableDate { get; set; }

        public string NextAvailableDay { get; set; }

        public string HotelAndProductName
        {
            get
            {
                return string.Format("{0} - {1}", Hotels.HotelName, ProductName);
            }
        }

        public string ProductNameUrl
        {
            get
            {
                if (!string.IsNullOrEmpty(ProductName))
                {
                    return ProductName.Trim().Replace(" ", "-").Replace("-&-", "-").Replace("$", "").ToLower();
                }
                return string.Empty;
            }
        }

        public int HotelOrder { get; set; }

        [JsonIgnore]
        public double LowestPrice
        {
            get
            {
                var lowest = PriceMon;
                if (PriceTue < lowest)
                {
                    lowest = PriceTue;
                }
                if (PriceWed < lowest)
                {
                    lowest = PriceWed;
                }
                if (PriceThu < lowest)
                {
                    lowest = PriceThu;
                }
                if (PriceFri < lowest)
                {
                    lowest = PriceFri;
                }
                if (PriceSat < lowest)
                {
                    lowest = PriceSat;
                }
                if (PriceSun < lowest)
                {
                    lowest = PriceSun;
                }
                return lowest;
            }
        }

        [JsonIgnore]
        public double LowestUpgradeDiscount
        {
            get
            {
                var lowest = UpgradeDiscountMon;
                if (UpgradeDiscountTue < lowest)
                {
                    lowest = UpgradeDiscountTue;
                }
                if (UpgradeDiscountWed < lowest)
                {
                    lowest = UpgradeDiscountWed;
                }
                if (UpgradeDiscountThu < lowest)
                {
                    lowest = UpgradeDiscountThu;
                }
                if (UpgradeDiscountFri < lowest)
                {
                    lowest = UpgradeDiscountFri;
                }
                if (UpgradeDiscountSat < lowest)
                {
                    lowest = UpgradeDiscountSat;
                }
                if (UpgradeDiscountSun < lowest)
                {
                    lowest = UpgradeDiscountSun;
                }
                return lowest;
            }
        }

        [JsonIgnore]
        public string ProductNameAcronym
        {
            get
            {
                return String.Join("", ProductName.Split(' ').Select(x => x[0].ToString().ToUpper()).ToList());
            }
        }

        [JsonIgnore]
        public List<Products> RelatedProducts { get; set; }

        [JsonIgnore]
        public List<Products> Similarproduct { get; set; }

        [JsonIgnore]
        public string ProductTypeString
        {
            get
            {
                switch (ProductType)
                {
                    case (int)Enums.ProductType.DayPass:
                        return Constant.DayPassString.Trim();
                    case (int)Enums.ProductType.Cabana:
                        return Constant.CabanasPassString.Trim();
                    case (int)Enums.ProductType.Daybed:
                        return Constant.DaybedsString.Trim();
                    default:
                        return Constant.SpaPassString.Trim();
                }
            }
        }

        public string HotelName
        {
            get { return Hotels.HotelName; }
        }

        public string CustomerName { get; set; }

        public ActualPriceObject ActualPriceWithDate { get; set; }

        public double PerGuestPrice { get; set; }

        public double PerGuestDiscountPrice { get; set; }

        public string KidAllowedString
        {
            get
            {
                switch (IsKidAllow)
                {
                    case (int)Enums.KidAllowType.Allowed:
                        return Constant.KidsUnder6Free;
                    case (int)Enums.KidAllowType.NotAllow:
                        return Constant.NoKidAllowed;
                    case (int)Enums.KidAllowType.AllowedFullPrice:
                        return Constant.KidAllowedAtFullPrice;
                    default:
                        return string.Empty;
                }
            }
        }
        public string ProductKidAllowedString
        {
            get
            {
                switch (IsKidAllow)
                {
                    case (int)Enums.KidAllowType.Allowed:
                        return Constant.ProductKidsUnder6Free;
                    case (int)Enums.KidAllowType.NotAllow:
                        return Constant.ProductNoKidAllowed;
                    case (int)Enums.KidAllowType.AllowedFullPrice:
                        return Constant.ProductKidAllowedAtFullPrice;
                    default:
                        return string.Empty;
                }
            }
        }

        [JsonIgnore]
        public List<Products> AddOnsproduct { get; set; }

        public bool IsUpdateDefaultPrice { get; set; }

        public List<Policies> FinePrint { get; set; }
    }
}
