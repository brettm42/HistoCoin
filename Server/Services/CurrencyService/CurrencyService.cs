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
    using static HistoCoin.Server.Infrastructure.Constants;
    
    public class CurrencyService : ICurrencyService
    {
        private readonly TimeSpan _maxDataAge = TimeSpan.FromMinutes(1);
        private readonly List<double> _valueHistoryUsd = new List<double>();
        private readonly List<double> _valueHistoryBtc = new List<double>();
        private readonly ConcurrentBag<Currency> _cache = new ConcurrentBag<Currency>();
        private readonly bool _cacheServiceStoreEnabled = false;
        private readonly string _cacheServiceLocation;

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

        public CurrencyService(ICacheService<ConcurrentBag<Currency>> cacheService)
        {
            if (cacheService.Cache != null)
            {
                this._cache = cacheService.Cache.Get() ?? new ConcurrentBag<Currency>();
                this._cacheServiceLocation = cacheService.StorageLocation;
                this._cacheServiceStoreEnabled = true;
            }

            this.Coins = 
                SyncCoinList(in this._cache, this.BaseCurrency)
                    .Select(c => c.Handle)
                    .ToObservable()
                    .ToArray();

            this.CurrentValues =
                Observable
                    .Interval(UpdateInterval)
                    .StartWith(0)
                    .Select(
                        _ => 
                            RefreshCache(in this._cache, this._maxDataAge, this.BaseCurrency, (this._cacheServiceStoreEnabled, this._cacheServiceLocation)))
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
                    .Interval(UpdateInterval + TimeSpan.FromSeconds(3))
                    .StartWith(0)
                    .Select(_ => CalculateDeltas(in this._cache, this.BaseCurrency));

            this.OverallDelta =
                Observable
                    .Interval(UpdateInterval)
                    .StartWith(0)
                    .Select(_ => CalculateOverallDelta(in this._cache, this.BaseCurrency));

            this.Value =
                Observable
                    .Interval(UpdateInterval + TimeSpan.FromSeconds(3))
                    .StartWith(0)
                    .Select(_ => this._valueHistoryUsd.Take(50).ToArray());
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
            if (Results is null)
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

        private double CalculateAverageValue(
            IEnumerable<(string Handle, double Count)> coins, 
            (string Handles, List<Packet<Dictionary<string, string>>> Results) currentValues,
            string currency)
        {
            double output = 0;

            foreach (var (Handle, Count) in coins)
            {
                var currentValue =
                    currentValues.Results
                        .FirstOrDefault(i => i.Name.Equals(Handle))?
                        .Contents
                        .FirstOrDefault(i => i.Key.Contains(currency))
                        .Value;

                if (currentValue is null)
                {
                    continue;
                }

                output += double.Parse(currentValue) * Count;
            }

            switch (currency)
            {
                case nameof(Currencies.USD):
                    if (this._valueHistoryUsd.LastOrDefault() != output)
                    {
                        this._valueHistoryUsd.Add(output);
                    }

                    break;
                case nameof(Currencies.BTC):
                    if (this._valueHistoryBtc.LastOrDefault() != output)
                    {
                        this._valueHistoryBtc.Add(output);
                    }

                    break;
                case nameof(Currencies.ETH):
                default:
                    break;
            }

            return 
                Math.Round(
                    output, 
                    currency.Contains(nameof(Currencies.BTC)) ? 6 : 2);
        }

        private double CalculateAverageValue(in ConcurrentBag<Currency> cache, Currencies currency)
        {
            double output = 0;

            foreach (var coin in cache.Where(c => c.BaseCurrency == currency))
            {
                if (coin.Value < 0)
                {
                    continue;
                }

                output += coin.Value * coin.Count;
            }

            output = Math.Round(output, currency == Currencies.BTC ? 6 : 2);

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
                default:
                    break;
            }

            return output;
        }

        private int[] CalculateValueDistribution(
            IEnumerable<(string Handle, double Count)> coins,
            (string Handles, List<Packet<Dictionary<string, string>>> Results) currentValues,
            string currency)
        {
            var output = new List<int>();

            var total = CalculateAverageValue(coins, currentValues, currency);

            foreach (var (Handle, Count) in coins)
            {
                var currentValue =
                    currentValues.Results
                        .FirstOrDefault(i => i.Name.Equals(Handle))?
                        .Contents
                        .FirstOrDefault(i => i.Key.Contains(currency))
                        .Value;

                if (currentValue is null)
                {
                    continue;
                }

                var percent = (double.Parse(currentValue) * Count) / total;

                output.Add((int)(percent * 100));
            }

            return output.ToArray();
        }

        private int[] CalculateValueDistribution(in ConcurrentBag<Currency> cache, Currencies currency)
        {
            var output = new List<int>();

            var total = CalculateAverageValue(in cache, currency);

            foreach (var coin in cache.Where(c => c.BaseCurrency == currency))
            {
                output.Add(
                    (int)Math.Round((coin.Value * coin.Count / total) * 100, 0));
            }

            return output.ToArray();
        }

        private static double[] CalculateDeltas(in ConcurrentBag<Currency> cache, Currencies currency)
        {
            var output = new List<double>();

            foreach (var coin in cache.Where(c => c.BaseCurrency == currency))
            {
                output.Add(coin.Delta);
            }

            return output.ToArray();
        }

        private static double CalculateOverallDelta(in ConcurrentBag<Currency> cache, Currencies currency)
        {
            double total = 0;

            foreach (var coin in cache.Where(c => c.BaseCurrency == currency))
            {
                total += coin.Delta;
            }

            return Math.Round(total, currency == Currencies.BTC ? 6 : 2);
        }

        private static Result CreateCacheStore(in ConcurrentBag<Currency> cache, string storeLocation)
        {
            var cacheStore =
                new CacheService<ConcurrentBag<Currency>>(storeLocation, cache);
            
            return cacheStore.Store();
        }
    }
}