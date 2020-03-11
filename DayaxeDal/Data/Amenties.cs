namespace DayaxeDal
{
    public partial class Amenties
    {
        public int ActiveFeatures
        {
            get
            {
                int active = 0;
                if (GymActive)
                {
                    active += 1;
                }
                if (SpaActive)
                {
                    active += 1;
                }
                if (PoolActive)
                {
                    active += 1;
                }
                if (BusinessActive)
                {
                    active += 1;
                }
                if (DinningActive)
                {
                    active += 1;
                }
                if (EventActive)
                {
                    active += 1;
                }
                return active;
            }
        }
    }
}
