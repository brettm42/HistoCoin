
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
            double SquareDeviation(double[] values, int start, int end)
            {
                double sum = 0;
                int i;

                for (i = start; i < end; i++)
                {
                    sum += Math.Pow(values[i], 2);
                }

                return Math.Sqrt(sum / (end - start));
            }

            var rms = new List<double>();

            var rootMeanSqr = SquareDeviation(historicalValues.TakeLast(depth).ToArray(), 0, depth);

            for (var i = historicalValues.Length - 1; i > historicalValues.Length - depth; i--)
            {
                rms.Add(SquareDeviation(historicalValues, i, historicalValues.Length));
            }

            return (rms.First() - rms.Last()) / rootMeanSqr;
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
