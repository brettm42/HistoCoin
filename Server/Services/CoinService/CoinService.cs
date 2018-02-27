namespace HistoCoin.Server.Services.CoinService
{
    using System.Collections.Generic;
    using System.Linq;
    using HistoCoin.Server.Data;
    using HistoCoin.Server.Infrastructure;
    using HistoCoin.Server.Services.CurrencyService;
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
                            BaseCurrency = this.BaseCurrency,
                            Handle = c.Handle,              
                            StartingValue = c.StartingValue,
                            Count = c.Count,
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
    }
}