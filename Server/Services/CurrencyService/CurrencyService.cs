namespace HistoCoin.Server.Services.CurrencyService
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reactive.Linq;
    using HistoCoin.Server.Data;
    using HistoCoin.Server.Infrastructure;
    using HistoCoin.Server.Services.CacheService;
    using HistoCoin.Server.Services.CoinService;
    using static HistoCoin.Server.Infrastructure.Constants;
    
    public class CurrencyService : ICurrencyService
    {
        private readonly TimeSpan _maxDataAge = TimeSpan.FromMinutes(1);
        private readonly List<double> _valueHistoryUsd = new List<double>();
        private readonly List<double> _valueHistoryBtc = new List<double>();
        private readonly List<double> _valueHistoryEth = new List<double>();

        private readonly string _cacheServiceLocation;
        private readonly bool _cacheServiceStoreEnabled = false;
        private readonly ConcurrentBag<Currency> _cache = new ConcurrentBag<Currency>();

        public IObservable<double[]> CurrentDeltas { get; }

        public IObservable<int[]> DistributionUsd { get; }

        public IObservable<int[]> DistributionBtc { get; }

        public IObservable<Currency> CurrentValues { get; }

        public IObservable<double> TotalValueUsd { get; }

        public IObservable<double> TotalValueBtc { get; }

        public IObservable<double> OverallDelta { get; }

        public IObservable<double[]> Value { get; }

        public IObservable<string[]> Coins { get; }

        public Currencies BaseCurrency { get; set; }

        public CurrencyService(ICacheService<ConcurrentBag<Currency>> cacheService, ICoinService coinService)
        {
            if (cacheService.Cache != null)
            {
                this._cache = cacheService.Cache.Get() ?? new ConcurrentBag<Currency>();
                this._cacheServiceLocation = cacheService.StorageLocation;
                this._cacheServiceStoreEnabled = true;
                
                // load historical data from CacheService
                switch (this.BaseCurrency)
                {
                    case Currencies.USD:
                        this._valueHistoryUsd =
                            CurrencyService.LoadHistoricalValue(
                                cacheService.PollHistoricalCache(), this.BaseCurrency);
                        break;

                    case Currencies.BTC:
                        this._valueHistoryBtc =
                            CurrencyService.LoadHistoricalValue(
                                cacheService.PollHistoricalCache(), this.BaseCurrency);
                        break;

                    case Currencies.ETH:
                        this._valueHistoryEth =
                            CurrencyService.LoadHistoricalValue(
                                cacheService.PollHistoricalCache(), this.BaseCurrency);
                        break;

                    default:
                        break;
                }
            }

            this.Coins =
                CurrencyService.SyncCoinList(in this._cache, this.BaseCurrency)
                    .Select(c => c.Handle)
                    .ToObservable()
                    .ToArray();

            //this.Coins =
            //    coinService.GetAll()
            //        .Select(c => c.Handle)
            //        .ToObservable()
            //        .ToArray();

            this.CurrentValues =
                Observable
                    .Interval(UpdateInterval)
                    .StartWith(0)
                    .Select(
                        _ => 
                            RefreshCache(
                                in this._cache, this._maxDataAge, this.BaseCurrency, (this._cacheServiceStoreEnabled, this._cacheServiceLocation)))
                    .SelectMany(i => i);

            this.DistributionUsd =
                Observable
                    .Interval(UpdateInterval)
                    .StartWith(0)
                    .Select(_ => CalculateValueDistribution(in this._cache, Currencies.USD));

            this.DistributionBtc =
                Observable
                    .Interval(UpdateInterval)
                    .StartWith(0)
                    .Select(_ => CalculateValueDistribution(in this._cache, Currencies.BTC));
            
            this.TotalValueUsd =
                Observable
                    .Interval(UpdateInterval)
                    .StartWith(0)
                    .Select(_ => CalculateAverageValue(in this._cache, Currencies.USD));

            this.TotalValueBtc =
                Observable
                    .Interval(UpdateInterval)
                    .StartWith(0)
                    .Select(_ => CalculateAverageValue(in this._cache, Currencies.BTC));

            this.CurrentDeltas =
                Observable
                    .Interval(UpdateInterval)
                    .StartWith(0)
                    .Select(_ => CalculateDeltas(in this._cache, this.BaseCurrency));

            this.OverallDelta =
                Observable
                    .Interval(UpdateInterval)
                    .StartWith(0)
                    .Select(_ => CalculateOverallDelta(in this._cache, this.BaseCurrency));

            this.Value =
                Observable
                    .Interval(UpdateInterval + TimeSpan.FromSeconds(1))
                    .StartWith(0)
                    .Select(_ => this._valueHistoryUsd.TakeLast(50).ToArray());
        }
        
        internal static double Normalize(double value, Currencies currency)
        {
            return Math.Round(value, currency == Currencies.USD ? 2 : 6);
        }

        private static IEnumerable<(string Handle, double Count)> SyncCoinList(in ConcurrentBag<Currency> cache, Currencies currency)
        {
            var coins = DataFetcher.BuildCurrencies().ToArray();

            var count = cache.Count(c => c.BaseCurrency == currency);
            if (count != coins.Length)
            {
                cache.Clear();
            
                foreach (var (Handle, Count, StartingValue) in coins)
                {
                    cache.Add(
                        new Currency(Currencies.USD)
                        {
                            Id = Handle.GetHashCode(),
                            Handle = Handle,
                            Count = Count,
                            StartingValue = StartingValue,
                            LastUpdated = DateTimeOffset.MinValue,
                        });

                    cache.Add(
                        new Currency(Currencies.BTC)
                        {
                            Id = Handle.GetHashCode(),
                            Handle = Handle,
                            Count = Count,
                            StartingValue = StartingValue,
                            LastUpdated = DateTimeOffset.MinValue,
                        });
                }
            }

            return cache
                .Where(c => c.BaseCurrency == currency)
                .Select(c => (c.Handle, c.Count));
        }

        private static IEnumerable<Currency> RefreshCache(
            in ConcurrentBag<Currency> cache, 
            TimeSpan minAge, 
            Currencies filter, 
            (bool StoreEnabled, string StoreLocation) backup = default)
        {
            var queue =
                cache.Where(
                    c => c.LastUpdated < (DateTimeOffset.Now - minAge));
            
            var (_, Results) = DataFetcher.FetchComparisons(queue.Select(i => i.Handle));
            if (Results is null || Results.Count < 1)
            {
                return cache.Where(c => c.BaseCurrency == filter);
            }

            foreach (var coin in cache)
            {
                var value =
                    double.Parse(
                        Results
                            .FirstOrDefault(
                                i => i.Name.Equals(coin.Handle))?
                            .Contents?[$"{coin.BaseCurrency}"] ?? "-1");

                if (value < 0)
                {
                    continue;
                }

                coin.Value = value;

                coin.Delta =
                    DataFetcher.CalculateDelta(coin.Handle, coin.Value, coin.StartingValue, filter) * coin.Count;
                
                coin.LastUpdated = DateTimeOffset.Now;
            }

            if (backup.StoreEnabled)
            {
                var result = CreateCacheStore(in cache, backup.StoreLocation);
                if (!result.Success)
                {
                    // TODO: ignore cache storage exceptions for now
                }
            }

            return cache.Where(c => c.BaseCurrency == filter);
        }
        
        private double CalculateAverageValue(in ConcurrentBag<Currency> cache, Currencies currency)
        {
            var output = 
                cache
                    .Where(c => c.BaseCurrency == currency)
                    .Where(coin => !(coin.Value < 0))
                    .Sum(coin => coin.Worth);

            output = CurrencyService.Normalize(output, currency);

            if (output > 0)
            {
                switch (currency)
                {
                    case Currencies.USD:
                        if (this._valueHistoryUsd.LastOrDefault() != output)
                        {
                            this._valueHistoryUsd.Add(output);
                        }

                        break;

                    case Currencies.BTC:
                        if (this._valueHistoryBtc.LastOrDefault() != output)
                        {
                            this._valueHistoryBtc.Add(output);
                        }

                        break;

                    case Currencies.ETH:
                        if (this._valueHistoryEth.LastOrDefault() != output)
                        {
                            this._valueHistoryEth.Add(output);
                        }

                        break;
                    default:
                        break;
                }
            }

            return output;
        }
        
        private int[] CalculateValueDistribution(in ConcurrentBag<Currency> cache, Currencies currency)
        {
            var total = CalculateAverageValue(in cache, currency);

            return cache
                .Where(c => c.BaseCurrency == currency)
                .Select(
                    coin => 
                        coin.Value < 0
                        ? 0
                        : (int) Math.Round(coin.Worth / total * 100, 0))
                .ToArray();
        }

        private static double[] CalculateDeltas(in ConcurrentBag<Currency> cache, Currencies currency)
        {
            return cache
                .Where(c => c.BaseCurrency == currency)
                .Select(coin => CurrencyService.Normalize(coin.Delta, currency))
                .ToArray();
        }

        private static double CalculateOverallDelta(in ConcurrentBag<Currency> cache, Currencies currency)
        {
            var total = 
                cache
                    .Where(c => c.BaseCurrency == currency)
                    .Sum(coin => coin.Delta);

            return CurrencyService.Normalize(total, currency);
        }
        
        private static Result CreateCacheStore(in ConcurrentBag<Currency> cache, string storeLocation)
        {
            var cacheStore =
                new CacheService<ConcurrentBag<Currency>>(storeLocation, cache);
            
            return cacheStore.Store();
        }

        private static List<double> LoadHistoricalValue(IEnumerable<Cache<ConcurrentBag<Currency>>> caches, Currencies currency)
        {
            return caches
                .Where(cache => cache != null)
                .Select(
                    cache => 
                        cache.Get().Where(i => i.BaseCurrency == currency))
                .GroupBy(
                    cache => 
                        (DateTime.Now - cache.FirstOrDefault().LastUpdated).Days)
                .Select(
                    group => 
                        group
                            .LastOrDefault()?
                            .Sum(i => i.Worth) ?? 0)
                .Where(sum => sum > 0)
                .Select(sum => CurrencyService.Normalize(sum, currency))
                .ToList();
        }
    }
}