
namespace HistoCoin.Server.Infrastructure
{
    using System;

    public static class Constants
    {
        public const string DefaultCacheStoreLocation = @"C:\Temp\HistoCoin";

        public const string DefaultWalletExtension = "hcw";
        public const string DefaultWalletPrefix = "_currencies";
        public const string DefaultWalletFilename = DefaultWalletPrefix + "." + DefaultWalletExtension;

        public const string DefaultCacheExtension = "hcc";
        public const string DefaultCachePrefix = "store_";
        public const string DefaultCacheDatePlaceholder = "%date%";
        public const string DefaultCacheFilename = DefaultCachePrefix + DefaultCacheDatePlaceholder + "." + DefaultCacheExtension;

        public enum Currencies
        {
            USD,
            BTC,
            ETH,
        }

        public const string NoneValue = "n/a";

        public const int MaxCachedFiles = 5000;

        public static readonly TimeSpan UpdateInterval = TimeSpan.FromSeconds(1);
    }
}
