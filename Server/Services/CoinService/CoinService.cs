namespace HistoCoin.Server.Services.CoinService
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using HistoCoin.Server.Data;
    using static HistoCoin.Server.Infrastructure.Constants;
    
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
                            Handle = c.Handle,
                            StartingValue = c.StartingValue,
                            Count = c.Count,
                        })
                    .ToList();

            this._newId = this._coins.Count;
        }

        public Currencies BaseCurrency { get; set; } = Currencies.USD;

        public IList<CoinModel> GetAll() => this._coins;

        public CoinModel GetById(int id) => this._coins.FirstOrDefault(i => i.Id == id);

        public int Add(CoinModel record)
        {
            record.Id = ++_newId;
            this._coins.Add(record);

            return record.Id;
        }

        public void Update(CoinModel record)
        {
            var idx = this._coins.FindIndex(i => i.Id == record.Id);
            if (idx >= 0)
            {
                this._coins[idx] = record;
            }
        }

        public void Delete(int id) => 
            this._coins.Remove(
                this._coins.FirstOrDefault(i => i.Id == id));
    }
}