using System;

namespace DayaxeDal.Extensions
{
    public static class ExtensionMethod
    {
        public static DateTime ToDateTime(this double currentTime)
        {
            return new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified).AddSeconds(currentTime);
        }
    }
}
