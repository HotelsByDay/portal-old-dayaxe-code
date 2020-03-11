using System.Collections.Generic;

namespace DayaxeDal.Custom
{
    public class ProductSalesReportObject
    {
        public Products ProductObject { get; set; }

        public List<SalesReportObject> SalesReportObject { get; set; }
    }
}
