
namespace HistoCoin.Server.Services.CurrencyService
{
    using System;
    using static HistoCoin.Server.Infrastructure.Constants;
    
    public class Currency
    {
        public Currency(Currencies currency)
        {
            this.BaseCurrency = currency;
        }

        public Currencies BaseCurrency { get; }
        
        public int Id { get; set; }
        
        public string Handle { get; set; }

        public double Value { get; set; } = -1;

        public double Count { get; set; } = 0;

        public double StartingValue { get; set; } = 0;
        
        public double Delta { get; set; } = 0;

        public DateTimeOffset LastUpdated { get; set; }

        public double Worth => this.Value < 0 ? 0 : this.Value * this.Count;

        public override string ToString() => $"{this.Handle} ({this.BaseCurrency})";
    }
}