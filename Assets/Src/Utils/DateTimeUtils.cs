using System;

namespace Suburb.Utils
{
    public class DateTimeUtils
    {
        private static string DEFAULT_DATE_TIME_FORMAT = "yyyy-MM-dd HH:mm:ss";
        private static string DETAIL_DATE_TIME_FORMAT = "yyyy-MM-dd HH:mm:ss-fff";

        public static string GetNow(string dateFormat = null)
        {
            dateFormat = dateFormat ?? DEFAULT_DATE_TIME_FORMAT;
            return DateTime.Now.ToString(dateFormat);
        }

        public static string GetDetailNow()
        {
            return DateTime.Now.ToString(DETAIL_DATE_TIME_FORMAT);
        }
    }
}
