
namespace HistoCoin.Server.Infrastructure
{
    using System;

    public static class Constants
    {
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
