using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreModels
{
    public static  class TimeUtility
    {
        private static readonly TimeZoneInfo IST = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");

        public static DateTimeOffset NowIST()
        {
            return TimeZoneInfo.ConvertTime(DateTimeOffset.UtcNow, IST);
        }



        public static DateTime StartOffCurrentMonth()
        {
            var indianNow = NowIST();
            return new DateTime(indianNow.Year, indianNow.Month, 1);
        }

        public static DateTime StartOffDay()
        {
            var indianNow = NowIST();
            return indianNow.Date;
        }

       public static int CurrentYear()
        {
            var now = NowIST();
            return now.Year;
        }
    }
}
