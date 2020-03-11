using System.Collections.Generic;

namespace DayaxeDal
{
    public class ListResult<T>
    {
        public List<T> Items { get; set; }

        public int TotalRecords { get; set; }
    }
}
