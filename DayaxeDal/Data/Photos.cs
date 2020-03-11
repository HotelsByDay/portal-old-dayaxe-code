namespace DayaxeDal
{
    public partial class Photos
    {

        public string Title
        {
            get
            {
                string title = string.Empty;
                switch (ImageTypeId ?? 0)
                {
                    case 0:
                        title = "Pool";
                        break;
                    case 1:
                        title = "Gym";
                        break;
                    case 2:
                        title = "Spa";
                        break;
                    case 3:
                        title = "Business Services";
                        break;
                    case 4:
                        title = "Other";
                        break;
                    default:
                        title = "Cover";
                        break;
                }
                return title;
            }
        }

        public string ImagePath
        {
            get { return Url.Substring(Url.LastIndexOf('/') + 1, Url.Length - Url.LastIndexOf('/') - 1); }
        }
    }
}
