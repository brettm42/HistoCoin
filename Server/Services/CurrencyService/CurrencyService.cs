
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
    using static HistoCoin.Server.Infrastructure.Helpers;

    public class CurrencyService : ICurrencyService
    {
        private readonly TimeSpan _maxDataAge = TimeSpan.FromMinutes(1);
        private readonly History _valueHistoryUsd = new History();
        private readonly History _valueHistoryBtc = new History();
        private readonly History _valueHistoryEth = new History();

        private readonly string _cacheServiceLocation;
        private readonly bool _cacheServiceStoreEnabled;
        private readonly ConcurrentBag<Currency> _cache = new ConcurrentBag<Currency>();

        public IObservable<double[]> CurrentDeltas { get; }

        public IObservable<int[]> DistributionUsd { get; }

        public IObservable<int[]> DistributionBtc { get; }

        public IObservable<Currency> CurrentValues { get; }

        public IObservable<double> TotalValueUsd { get; }

        public IObservable<double> TotalValueBtc { get; }

        public IObservable<double> OverallDelta { get; }

        public IObservable<History> ValueHistory { get; }

        public IObservable<string[]> Coins { get; }

        public Currencies BaseCurrency { get; set; } = Currencies.USD;

        public CurrencyService(ICoinService coinService, ICacheService<ConcurrentBag<Currency>> cacheService)
        {
            if (coinService != null)
            {
                this.BaseCurrency = coinService.BaseCurrency;
            }

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
                            new History(
                                CurrencyService.LoadHistoricalValue(
                                    cacheService.PollHistoricalCache(), this.BaseCurrency));
                        break;

                    case Currencies.BTC:
                        this._valueHistoryBtc =
                            new History(
                                CurrencyService.LoadHistoricalValue(
                                    cacheService.PollHistoricalCache(), this.BaseCurrency));
                        break;

                    case Currencies.ETH:
                        this._valueHistoryEth =
                            new History(
                                CurrencyService.LoadHistoricalValue(
                                    cacheService.PollHistoricalCache(), this.BaseCurrency));
                        break;

                    default:
                        break;
                }
            }
            
            this.Coins =
                CurrencyService.SyncCoinList(in this._cache, coinService)
                    .Select(c => c.Handle)
                    .ToObservable()
                    .ToArray();

            this.CurrentValues =
                Observable
                    .Interval(UpdateInterval)
                    .StartWith(0)
                    .Select(
                        _ => 
                            RefreshCache(
                                in this._cache, in coinService, this._maxDataAge, this.BaseCurrency, (this._cacheServiceStoreEnabled, this._cacheServiceLocation)))
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

            this.ValueHistory =
                Observable
                    .Interval(UpdateInterval + TimeSpan.FromSeconds(1))
                    .StartWith(0)
                    .Select(_ => this._valueHistoryUsd);
        }
        
        private static IEnumerable<(string Handle, double Count)> SyncCoinList(in ConcurrentBag<Currency> cache, ICoinService coinService)
        {
            var coins = coinService.GetAll().ToList();

            var count = cache.Count(c => c.BaseCurrency == coinService.BaseCurrency);
            if (count != coins.Count)
            {
                cache.Clear();
            
                foreach (var coin in coins)
                {
                    cache.Add(
                        new Currency(Currencies.USD)
                        {
                            Id = coin.Id,
                            Handle = coin.Handle,
                            Count = coin.Count,
                            StartingValue = coin.StartingValue,
                            LastUpdated = DateTimeOffset.MinValue,
                        });

                    cache.Add(
                        new Currency(Currencies.BTC)
                        {
                            Id = coin.Id,
                            Handle = coin.Handle,
                            Count = coin.Count,
                            StartingValue = coin.StartingValue,
                            LastUpdated = DateTimeOffset.MinValue,
                        });
                }
            }

            return cache
                .Where(c => c.BaseCurrency == coinService.BaseCurrency)
                .Select(c => (c.Handle, c.Count));
        }
        
        private static IEnumerable<Currency> RefreshCache(
            in ConcurrentBag<Currency> cache, 
            in ICoinService coinService,
            TimeSpan minAge, 
            Currencies filter, 
            (bool StoreEnabled, string StoreLocation) backup = default)
        {
            var queue =
                cache.Where(
                    c => c.LastUpdated < (DateTimeOffset.Now - minAge));
            
            var (_, results) = DataFetcher.FetchComparisons(queue.Select(i => i.Handle));
            if (results is null || results.Count < 1)
            {
                return cache.Where(c => c.BaseCurrency == filter);
            }

            foreach (var coin in cache)
            {
                var value =
                    double.Parse(
                        results
                            .FirstOrDefault(
                                i => i.Name.Equals(coin.Handle))?
                            .Contents?[$"{coin.BaseCurrency}"] ?? "-1");

                if (value < 0)
                {
                    continue;
                }

                coin.CurrentValue = value;

                coin.Delta =
                    DataFetcher.CalculateDelta(coin.Handle, coin.CurrentValue, coin.StartingValue, filter) * coin.Count;
                
                coin.LastUpdated = DateTimeOffset.Now;

                coinService.Update(coin);
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
                    .Where(coin => !(coin.CurrentValue < 0))
                    .Sum(coin => coin.Worth);

            output = Normalize(output, currency);

            if (output > 0)
            {
                switch (currency)
                {
                    case Currencies.USD:
                        if (this._valueHistoryUsd.GetLastValue() != output)
                        {
                            this._valueHistoryUsd.Add(Normalize(DateTime.Now), output);
                        }

                        break;

                    case Currencies.BTC:
                        if (this._valueHistoryBtc.GetLastValue() != output)
                        {
                            this._valueHistoryBtc.Add(Normalize(DateTime.Now), output);
                        }

                        break;

                    case Currencies.ETH:
                        if (this._valueHistoryEth.GetLastValue() != output)
                        {
                            this._valueHistoryEth.Add(Normalize(DateTime.Now), output);
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
                        coin.CurrentValue < 0
                        ? 0
                        : (int) Math.Round(coin.Worth / total * 100, 0))
                .ToArray();
        }

        private static double[] CalculateDeltas(in ConcurrentBag<Currency> cache, Currencies currency)
        {
            return cache
                .Where(c => c.BaseCurrency == currency)
                .Select(coin => Normalize(coin.Delta, currency))
                .ToArray();
        }

        private static double CalculateOverallDelta(in ConcurrentBag<Currency> cache, Currencies currency)
        {
            var total = 
                cache
                    .Where(c => c.BaseCurrency == currency)
                    .Sum(coin => coin.Delta);

            return Normalize(total, currency);
        }
        
        private static Result CreateCacheStore(in ConcurrentBag<Currency> cache, string storeLocation)
        {
            var cacheStore =
                new CacheService<ConcurrentBag<Currency>>(storeLocation, cache);
            
            return cacheStore.Store();
        }

        private static Dictionary<string, double> LoadHistoricalValue(IEnumerable<Cache<ConcurrentBag<Currency>>> caches, Currencies currency)
        {
            return caches
                .Where(cache => cache != null)
                .Select(
                    cache =>
                        cache.Get().Where(i => i.BaseCurrency == currency))
                .GroupBy(
                    cache =>
                        (int) (DateTime.Now - cache.FirstOrDefault().LastUpdated).TotalHours / 10)
                .Select(
                    group =>
                        (Sum: group.LastOrDefault()?.Sum(i => i.Worth) ?? 0,
                        LastUpdate: group.LastOrDefault()?.LastOrDefault()?.LastUpdated ?? default))
                .Where(t => t.Sum > 0)
                //.Where((sum, _) => sum > 0)
                .ToDictionary(
                    t => Normalize(t.LastUpdate), t => Normalize(t.Sum, currency));
                    //(_, date) => Normalize(date), (sum, _) => Normalize(sum, currency));
        }
    }
}