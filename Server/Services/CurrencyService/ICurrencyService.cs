namespace HistoCoin.Server.Services.CurrencyService
{
    using System;
    using System.Collections.Generic;
    using static HistoCoin.Server.Infrastructure.Constants;

    public interface ICurrencyService
    {
        Currencies BaseCurrency { get; set; }

        IObservable<string[]> Coins { get; }

        IObservable<double[]> CurrentDeltas { get; }

        IObservable<int[]> DistributionUsd { get; }

        IObservable<int[]> DistributionBtc { get; }

        IObservable<Currency> CurrentValues { get; }

        IObservable<double> TotalValueUsd { get; }

        IObservable<double> TotalValueBtc { get; }

        IObservable<double> OverallDelta { get; }

        IObservable<History> ValueHistory { get; }
    }
}