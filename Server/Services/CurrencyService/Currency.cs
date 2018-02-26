
namespace HistoCoin.Server.Services.CurrencyService
{
    using System;
    using HistoCoin.Server.Infrastructure;
    using static HistoCoin.Server.Infrastructure.Constants;
    
    public class Currency : ICoin
    {
        public Currency(Currencies currency)
        {
            this.BaseCurrency = currency;
        }

        public int Id { get; set; }

        public string Handle { get; set; }

        public double Count { get; set; }

        public Currencies BaseCurrency { get; set; }

        public double StartingValue { get; set; }

        public double CurrentValue { get; set; } = -1;

        public double Worth => this.CurrentValue < 0 ? 0 : this.CurrentValue * this.Count;

        public double Delta { get; set; } = 0;

        public History History { get; set; }

        public DateTimeOffset LastUpdated { get; set; }

        public override string ToString() => $"{this.Handle} ({this.BaseCurrency})";

    }
}