
namespace HistoCoin.Server.Infrastructure
{
    using System;
    using System.Globalization;
    using System.Linq;
    using static HistoCoin.Server.Infrastructure.Constants;

    public static class Helpers
    {
        public static double Normalize(double value, Currencies currency, int shift = 0)
        {
            return Math.Round(
                value,
                Digits[currency] + shift);
        }

        public static double[] Normalize(double[] values, Currencies currency, int shift = 0)
        {
            return
                values.Select(
                    value =>
                        Math.Round(
                            value, Digits[currency] + shift))
                    .ToArray();
        }

        public static string Normalize(DateTimeOffset dateTime)
        {
            return dateTime.DateTime.ToString(CultureInfo.CurrentCulture);
        }

        public static string TimeOffsetAsString(DateTimeOffset lastTime, DateTimeOffset currentTime, CultureInfo culture = default)
        {
            if (currentTime < lastTime)
            {
                return "Just now";
            }

            var difference = currentTime - lastTime;

            if (difference.Days > 1)
            {
                return $"{difference.Days} days ago";
            }

            if (difference.Days == 1)
            {
                return $"{difference.Days} day ago";
            }

            if (difference.Minutes < 1)
            {
                return $"{difference.Seconds} seconds ago";
            }

            if (difference.Minutes == 1)
            {
                return $"{difference.Minutes} minute ago";
            }

            return $"{difference.Minutes} minutes ago";    
        }

        public static string[] DatesFromNow(DateTimeOffset now, int days, CultureInfo culture = default)
        {
            var output = new string[days + 1];

            for (var i = 0; i < output.Length; i++)
            {
                output[i] = (now + TimeSpan.FromDays(i)).Date.ToShortDateString();
            }

            return output;
        }
    }
}
