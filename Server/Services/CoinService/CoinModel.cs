
namespace HistoCoin.Server.Services.CoinService
{
    using System;
    using HistoCoin.Server.Infrastructure;
    using static HistoCoin.Server.Infrastructure.Constants;
    using static HistoCoin.Server.Infrastructure.Helpers;
    using Newtonsoft.Json;

    public class CoinModel : ICoin
    {
        public int Id { get; set; }
        
        [JsonIgnore]
        public bool IsModified { get; set; }

        public string Handle { get; set; }

        public double Count { get; set; }

        public Currencies BaseCurrency { get; set; }

        public double StartingValue { get; set; }

        public double CurrentValue { get; set; } = -1;

        public double Worth => Normalize(this.CurrentValue < 0 ? 0 : this.CurrentValue * this.Count, this.BaseCurrency);

        public double Delta { get; set; } = -1;

        public History History { get; set; }

        public override string ToString() => $"{this.Handle} ({this.BaseCurrency})";

        public ICoin Update(ICoin coin)
        {
            this.Handle =
                this.Handle.Equals(coin.Handle, StringComparison.InvariantCultureIgnoreCase)
                    ? this.Handle
                    : coin.Handle;
            this.Count =
                this.Count.Equals(coin.Count)
                    ? this.Count
                    : coin.Count;
            this.StartingValue =
                this.StartingValue.Equals(coin.StartingValue)
                    ? this.StartingValue
                    : coin.StartingValue;
            this.BaseCurrency =
                this.BaseCurrency == coin.BaseCurrency
                    ? this.BaseCurrency
                    : coin.BaseCurrency;
            
            this.IsModified = true;

            return this;
        }
    }
}
