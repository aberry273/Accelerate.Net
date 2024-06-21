using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Accelerate.Foundations.Common.Extensions
{
    public static class DateExtensions
    {
        public static string ToTimeSinceString(this DateTime? value)
        {
            if(value == null) return string.Empty;
            const int SECOND = 1;
            const int MINUTE = 60 * SECOND;
            const int HOUR = 60 * MINUTE;
            const int DAY = 24 * HOUR;
            const int MONTH = 30 * DAY;

            TimeSpan ts = new TimeSpan(DateTime.Now.Ticks - value.GetValueOrDefault().Ticks);
            double seconds = ts.TotalSeconds;

            // Less than one minute
            if (seconds < 1 * MINUTE)
                return ts.Seconds == 1 ? "Just now" : "Just now";

            if (seconds < 60 * MINUTE)
                return ts.Minutes + "m";

            if (seconds < 120 * MINUTE)
                return " 1h";

            if (seconds < 24 * HOUR)
                return ts.Hours + "h";

            if (seconds < 48 * HOUR)
                return "1d";

            if (seconds < 30 * DAY)
                return ts.Days + "d";

            if (seconds < 12 * MONTH)
            {
                int months = Convert.ToInt32(Math.Floor((double)ts.Days / 30));
                return months <= 1 ? "m" : months + "m";
            }

            int years = Convert.ToInt32(Math.Floor((double)ts.Days / 365));
            return years <= 1 ? "y" : years + "y";
        }
    }
}
