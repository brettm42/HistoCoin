namespace HistoCoin.Server.Services.CoinService
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Concurrent;
    using System.Linq;
    using HistoCoin.Server.Data;
    using HistoCoin.Server.Infrastructure;
    using HistoCoin.Server.Services.CacheService;
    using HistoCoin.Server.Services.CurrencyService;
    using static HistoCoin.Server.Infrastructure.Constants;
    using static HistoCoin.Server.Infrastructure.Helpers;
    
    public class CoinService : ICoinService
    {
        private List<CoinModel> _coins;
        private int _newId;

        public CoinService()
        {
            this._coins =
                DataFetcher.BuildCurrencies()
                    .Select(c =>
                        new CoinModel
                        {
                            Id = c.GetHashCode(),
                            BaseCurrency = this.BaseCurrency,
                            Handle = c.Handle,              
                            StartingValue = c.StartingValue,
                            Count = c.Count,
                        })
                    .ToList();
            
            this._newId = this._coins.Count;
        }

        public CoinService(ICacheService<ConcurrentBag<Currency>> cacheService)
        {
            this._coins =
                DataFetcher.BuildCurrencies()
                    .Select(c =>
                        new CoinModel
                        {
                            Id = c.GetHashCode(),
                            BaseCurrency = this.BaseCurrency,
                            Handle = c.Handle,
                            StartingValue = c.StartingValue,
                            Count = c.Count,
                            History = 
                                new History(
                                    CoinService.LoadHistoricalValue(
                                        cacheService.PollHistoricalCache(), 
                                        c.Handle, 
                                        this.BaseCurrency)),
                        })
                    .ToList();

            this._newId = this._coins.Count;
        }

        public Currencies BaseCurrency { get; set; } = Currencies.USD;

        public IEnumerable<ICoin> GetAll() => this._coins;

        public ICoin GetById(int id) => this._coins.FirstOrDefault(i => i.Id == id);

        public int Add(ICoin record)
        {
            record.Id = ++_newId;
            this._coins.Add(record as CoinModel);

            return record.Id;
        }

        public void Update(ICoin record)
        {
            var idx = this._coins.FindIndex(i => i.Id == record.Id);
            if (idx >= 0)
            {
                this._coins[idx] = record as CoinModel;
            }
        }
        
        public void Delete(int id) => 
            this._coins.Remove(
                this._coins.FirstOrDefault(i => i.Id == id));
        
        private static Dictionary<string, double> LoadHistoricalValue(IEnumerable<Cache<ConcurrentBag<Currency>>> caches, string handle, Currencies currency)
        {
            return caches
                .Where(cache => cache != null)
                .Select(
                    cache =>
                        cache.Get()
                            .Where(i => i.BaseCurrency == currency)
                            .Where(i => i.Handle.Equals(handle, StringComparison.InvariantCultureIgnoreCase)))
                .GroupBy(
                    cache =>
                        (int)(DateTime.Now - cache.FirstOrDefault().LastUpdated).TotalHours / 10)
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