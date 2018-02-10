
namespace HistoCoin.Server.Infrastructure
{
    using System;

    public static class Constants
    {
        public const string DefaultCacheStoreLocation = @"C:\Temp\HistoCoin";

        public enum Currencies
        {
            USD,
            BTC,
            ETH,
        }

        public const string NoneValue = "n/a";

        public static readonly TimeSpan UpdateInterval = TimeSpan.FromSeconds(90);
    }
}
