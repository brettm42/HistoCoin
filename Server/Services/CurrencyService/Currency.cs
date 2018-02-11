namespace HistoCoin.Server.Services.CurrencyService
{
    using System;
    using System.Diagnostics;
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

        public override string ToString()
        {
            return $"{this.Handle} ({this.BaseCurrency})";
        }
    }
}