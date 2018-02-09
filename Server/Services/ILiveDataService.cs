namespace HistoCoin.Server.Services
{
    using System;
    using static HistoCoin.Server.Infrastructure.Constants;

    public interface ILiveDataService
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

        IObservable<double[]> Value { get; }
    }
}