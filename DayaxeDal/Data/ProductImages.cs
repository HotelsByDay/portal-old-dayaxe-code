namespace DayaxeDal
{
    public partial class ProductImages
    {
        public string ImagePath
        {
            get { return Url.Substring(Url.LastIndexOf('/') + 1, Url.Length - Url.LastIndexOf('/') - 1); }
        }
    }
}
