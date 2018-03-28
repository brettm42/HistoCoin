
namespace HistoCoin.Server.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using static HistoCoin.Server.Infrastructure.Constants;

    public static class Helpers
    {
        public static double CalculateTrend(double[] historicalValues, int depth)
        {
            var deltas = new List<double>();

            for (var i = historicalValues.Length - 1; i > 0; i--)
            {
                deltas.Add(
                    (historicalValues[i - 1] - historicalValues[i]) / historicalValues[i] * 100);
            }

            var average = deltas.TakeLast(depth).Average();
            var sum = deltas.TakeLast(depth).Sum();

            return Math.Abs(average) < 1 ? 0 : average;
        }

        public static double Normalize(double value, Currencies currency)
        {
            return Math.Round(
                value, 
                currency == Currencies.USD 
                    ? Digits[Currencies.USD] 
                    : Digits[Currencies.ETH]);
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
    }
}
