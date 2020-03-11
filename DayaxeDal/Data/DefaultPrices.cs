using System.Linq;
using Newtonsoft.Json;

namespace DayaxeDal
{
    public partial class DefaultPrices
    {
        [JsonIgnore]
        public string ProductNameAcronym
        {
            get
            {
                return string.Join("", Products.ProductName.Split(' ').Select(x => x[0].ToString().ToUpper()).ToList());
            }
        }

        [JsonIgnore]
        public string DefaultCapacity
        {
            get
            {
                switch (Products.ProductType)
                {
                    case (int)Enums.ProductType.Cabana:
                        return DailyCabana.ToString();
                    case (int)Enums.ProductType.SpaPass:
                        return DailySpa.ToString();
                    case (int)Enums.ProductType.Daybed:
                        return DailyDaybed.ToString();
                    default:
                        return DailyDayPass.ToString();
                }
            }
        }
    }
}
