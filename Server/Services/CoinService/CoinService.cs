namespace HistoCoin.Server.Services.CoinService
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Concurrent;
    using System.Linq;
    using HistoCoin.Server.Data;
    using HistoCoin.Server.Infrastructure;
    using HistoCoin.Server.Infrastructure.Interfaces;
    using HistoCoin.Server.Services.CacheService;
    using HistoCoin.Server.Services.CurrencyService;
    using static HistoCoin.Server.Infrastructure.Constants;
    using static HistoCoin.Server.Infrastructure.Helpers;
    
    public class CoinService : ICoinService
    {
        private readonly List<CoinModel> _coins;

        public CoinService()
        {
            this._coins =
                DataFetcher.BuildCurrencies()
                    .Select(c =>
                        new CoinModel
                        {
                            Id = c.ToString().GetHashCode(),
                            BaseCurrency = this.BaseCurrency,
                            Handle = c.Handle,              
                            StartingValue = c.StartingValue,
                            Count = c.Count,
                        })
                    .ToList();
        }

        public CoinService(ICacheService<ConcurrentBag<Currency>> cacheService) : this()
        {
            this.AddCacheService(cacheService);
        }
        
        public Currencies BaseCurrency { get; set; } = Currencies.USD;

        public IEnumerable<ICoin> GetAll() => this._coins;
        
        public ICoin GetById(int id) => this._coins.FirstOrDefault(i => i?.Id == id);

        public int GetFirstId() => this._coins.FirstOrDefault()?.Id ?? 1;

        public int Add(ICoin record)
        {
            if (record is null)
            {
                return -1;
            }

            record.IsModified = true;
            this._coins.Add(record as CoinModel);

            return record.Id;
        }

        public void Update(ICoin record)
        {
            if (record is null)
            {
                return;
            }

            var idx = this._coins.FindIndex(i => i?.Id == record.Id);
            if (idx >= 0)
            {
                record.IsModified = true;
                this._coins[idx].Update(record);
            }

            var alternates = 
                this._coins.Where(
                    c =>
                        c.Handle.Equals(record.Handle, StringComparison.InvariantCultureIgnoreCase));

            foreach (var alt in alternates)
            {
                alt.Touch(record);
            }
        }
        
        public void Delete(int id) => 
            this._coins.Remove(
                this._coins.FirstOrDefault(i => i.Id == id));

        public CoinService AddCacheService(ICacheService<ConcurrentBag<Currency>> cacheService)
        {
            if (cacheService is null)
            {
                return this;
            }

            foreach (var coin in this._coins)
            {
                coin.History =
                    new History(
                        CoinService.LoadHistoricalValue(
                            cacheService.PollHistoricalCache(),
                            coin.Handle,
                            this.BaseCurrency));

                coin.CurrentValue =
                    coin.CurrentValue > -1
                        ? coin.CurrentValue
                        : coin.History?.GetLastEntryValue() ?? -1;

                coin.Delta =
                    coin.Delta > -1
                        ? coin.Delta
                        : DataFetcher.CalculateDelta(
                            coin.CurrentValue * coin.Count,
                            coin.StartingValue * coin.Count, 
                            coin.BaseCurrency);
            }

            return this;
        }

        private static Dictionary<string, double> LoadHistoricalValue(IEnumerable<Cache<ConcurrentBag<Currency>>> caches, string handle, Currencies currency)
        {
            return caches
                .Where(cache => cache != null)
                .Select(
                    cache =>
                        cache.Get()
                            .Where(i => i.BaseCurrency == currency)
                            .Where(i => i.Handle.Equals(handle, StringComparison.InvariantCultureIgnoreCase)))
                .Where(cache => cache.Any())
                .GroupBy(
                    cache =>
                        (int)(DateTime.Now - cache.FirstOrDefault().LastUpdated).TotalHours / DefaultBucketSize)
                .Select(
                    group =>
                        (Value: group.LastOrDefault()?.LastOrDefault()?.CurrentValue ?? 0,
                        LastUpdate: group.LastOrDefault()?.LastOrDefault()?.LastUpdated ?? default))
                .Where(t => t.Value > 0)
                .ToDictionary(
                    t => Normalize(t.LastUpdate), t => Normalize(t.Value, currency));
        }
    }
}