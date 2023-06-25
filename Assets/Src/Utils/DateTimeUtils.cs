using System;

namespace Suburb.Utils
{
    public class DateTimeUtils
    {
        private static string DEFAULT_DATE_FORMAT = "yyyy-MM-dd HH:mm-ss";

        public static string GetNow(string dateFormat = null)
        {
            dateFormat = dateFormat ?? DEFAULT_DATE_FORMAT;
            return DateTime.Now.ToString(dateFormat);
        }
    }
}
