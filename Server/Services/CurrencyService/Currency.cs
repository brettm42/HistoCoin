
namespace HistoCoin.Server.Services.CurrencyService
{
    using System;
    using HistoCoin.Server.Infrastructure;
    using static HistoCoin.Server.Infrastructure.Constants;
    using static HistoCoin.Server.Infrastructure.Helpers;
    
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

        public double Worth => Normalize(this.CurrentValue < 0 ? 0 : this.CurrentValue * this.Count, this.BaseCurrency);

        public double Delta { get; set; } = -1;

        public History History { get; set; }

        public DateTimeOffset LastUpdated { get; set; }

        public override string ToString() => $"{this.Handle} ({this.BaseCurrency})";

    }
}