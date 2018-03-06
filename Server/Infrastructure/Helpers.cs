
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
    }
}
