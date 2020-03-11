namespace DayaxeDal
{
    public partial class CustomerInfos
    {
        public string Fullname
        {
            get { return string.Format("{0} {1}", FirstName, LastName); }
        }

        public string SubscriptionCode { get; set; }
    }
}