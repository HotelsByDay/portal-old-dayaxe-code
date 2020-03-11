using System.Collections.Generic;

namespace DayaxeDal.Custom
{
    public class FaqResultObject
    {
        public int Count { get; set; }

        public List<FaqArticlesObject> Articles { get; set; }
    }

    public class FaqArticlesObject
    {
        public long Id { get; set; }
        
        public string Url { get; set; }

        public string Name { get; set; }

        public string Title { get; set; }
        
        public string Body { get; set; }
    }
}
