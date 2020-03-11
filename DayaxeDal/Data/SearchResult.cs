namespace DayaxeDal
{
    public class SearchResult
    {
        public ListResult<Products> Result { get; set; }

        public double MinPrice { get; set; }

        public double MaxPrice { get; set; }

        public double MinDistance { get; set; }

        public double MaxDistance { get; set; }

        public bool IsResetFilter { get; set; }
    }
}
