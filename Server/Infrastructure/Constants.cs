﻿
namespace HistoCoin.Server.Infrastructure
{
    using System;
    using System.Collections.Generic;

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
            XLM,
            TRX,
        }

        public const string NoneValue = "n/a";

        public const int MaxCachedFiles = 5000;
        public const int DefaultHistoryPopulation = 50;
        public const int DefaultForecastPopulation = 25;
        public const int DefaultBucketSize = 10;

        public static readonly TimeSpan UpdateInterval = TimeSpan.FromSeconds(1);

        public static readonly Dictionary<Currencies, int> Digits = 
            new Dictionary<Currencies, int>
            {
                { Currencies.USD, 2 },
                { Currencies.ETH, 4 },
                { Currencies.BTC, 5 },
                { Currencies.XLM, 5 },
                { Currencies.TRX, 5 },
            };
    }
}
