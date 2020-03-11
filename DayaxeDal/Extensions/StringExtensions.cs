using System;
using System.Collections.Generic;
using System.Linq;
using TimeZoneConverter;

namespace DayaxeDal.Extensions
{
    public static class StringExtensions
    {
        public static IEnumerable<String> SplitInParts(this String s, Int32 partLength)
        {
            if (s == null)
                throw new ArgumentNullException("s");
            if (partLength <= 0)
                throw new ArgumentException("Part length has to be positive.", "partLength");

            for (var i = 0; i < s.Length; i += partLength)
                yield return s.Substring(i, Math.Min(partLength, s.Length - i));
        }

        public static String ToCustomStringUrl(this String s)
        {
            if (s == null)
                throw new ArgumentNullException("s");

            return s.Trim().Replace(" ", "-").Replace("-&-", "-").Replace("$", "").ToLower();
        }

        public static String ToFirstLetter(this String s)
        {
            if (s == null)
                throw new ArgumentNullException("s");

            return string.Join("", s.Split(' ').Select(s1 => s1[0])).ToUpper();
        }


        public static String GetTimeZoneInfo(this String s)
        {
            if (string.IsNullOrEmpty(s))
            {
                s = Constant.UsDefaultTime;
            }
            string tz = TZConvert.IanaToWindows(s);

            return tz.ToFirstLetter();
        }
    }
}
