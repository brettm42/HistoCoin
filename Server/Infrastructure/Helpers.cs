﻿
namespace HistoCoin.Server.Infrastructure
{
    using System;
    using System.Globalization;
    using static HistoCoin.Server.Infrastructure.Constants;

    public static class Helpers
    {

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
