
namespace HistoCoin.Server.ViewModels.Forecast
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DotNetify;
    using DotNetify.Routing;
    using DotNetify.Security;
    using HistoCoin.Server.Infrastructure;
    using HistoCoin.Server.Services.CoinService;
    using HistoCoin.Server.ViewModels.AppLayout;
    using static HistoCoin.Server.Infrastructure.Constants;
    using static HistoCoin.Server.Infrastructure.Helpers;
    using static HistoCoin.Server.Infrastructure.Numerics;

    [Authorize]
    public class Forecast : BaseVM, IRoutable
    {
        private const int PollDepth = 25;
        private const int NearPollDepth = PollDepth / 4;
        private const int FarPollDepth = PollDepth * 4;
        private const int ForecastReach = 7;
        private const bool Randomization = true;
        private const int DefaultAccuracyAdjust = 1;

        private readonly ICoinService _coinService;

        public RoutingState RoutingState { get; set; }

        public IEnumerable<CoinInfo> Coins =>
            this._coinService
                .GetAll()
                .Where(i => i != null)
                .OrderBy(i => i.Handle)
                .Select(
                    i =>
                        new CoinInfo
                        {
                            Id = i.Id,
                            Handle = i.Handle,
                            Count = i.Count,
                            StartingValue = i.StartingValue,
                            Route = this.Redirect(AppLayout.ForecastPagePath, i.Id.ToString())
                        });

        public int Id
        {
            get => Get<int>();

            set => Set(value);
        }

        public string Handle
        {
            get => Get<string>();

            set => Set(value);
        }

        public double Count
        {
            get => Get<double>();

            set => Set(value);
        }

        //public Currencies BaseCurrency =>
        //    Digits.HasKey(this.Handle)
        //        ? Digits[this.Handle]
        //        : this._coinService.BaseCurrency;

        public double StartingValue
        {
            get => Get<double>();

            set => Set(value);
        }

        public double CurrentValue
        {
            get => Get<double>();

            set => Set(value);
        }

        public double Worth
        {
            get => Get<double>();

            set => Set(value);
        }

        public double Delta
        {
            get => Get<double>();

            set => Set(value);
        }

        public ForecastGraph HistoricalGraph
        {
            get => Get<ForecastGraph>();

            set => Set(value);
        }

        public ForecastGraph ForecastGraph
        {
            get => Get<ForecastGraph>();

            set => Set(value);
        }

        public ForecastGraph NearForecastGraph
        {
            get => Get<ForecastGraph>();

            set => Set(value);
        }

        public ForecastGraph FarForecastGraph
        {
            get => Get<ForecastGraph>();

            set => Set(value);
        }

        public ForecastData ForecastData
        {
            get => Get<ForecastData>();

            set => Set(value);
        }

        public ForecastData NearForecastData
        {
            get => Get<ForecastData>();

            set => Set(value);
        }

        public ForecastData FarForecastData
        {
            get => Get<ForecastData>();

            set => Set(value);
        }

        public Forecast(ICoinService coinService)
        {
            this._coinService = coinService;

            this.OnRouted((sender, e) =>
            {
                if (!int.TryParse(e?.From?.Replace($"{AppLayout.ForecastPagePath}/", string.Empty), out var id))
                {
                    return;
                }

                this.LoadCoin(
                    id == 1 ? this._coinService.GetFirstId() : id);
            });
        }
        
        private void LoadCoin(int id)
        {
            var record = this._coinService.GetById(id);
            if (record is null)
            {
                return;
            }

            this.Id = record.Id;
            this.Handle = record.Handle;
            this.Count = record.Count;
            this.StartingValue = 
                Normalize(record.StartingValue, this._coinService.BaseCurrency);
            this.CurrentValue = 
                Normalize(record.CurrentValue, this._coinService.BaseCurrency);
            this.Delta = 
                Normalize(record.Delta, this._coinService.BaseCurrency, DefaultAccuracyAdjust);
            this.Worth = 
                Normalize(record.Worth, this._coinService.BaseCurrency);

            this.HistoricalGraph =
                new ForecastGraph
                {
                    Labels =
                        record.History?.GetDates(DefaultHistoryPopulation + 1) ?? new string[PollDepth + 1],
                    Values =
                        Normalize(
                            record.History?.GetValues(DefaultHistoryPopulation + 1) ?? new double[PollDepth + 1], 
                            this._coinService.BaseCurrency,
                            DefaultAccuracyAdjust),
                };

            // set linear forecasting values
            this.UpdateForecastData(record);
                
            // set eager forecasting values
            this.UpdateNearForecastData(record);
            
            // set skeptical forecasting values
            this.UpdateFarForecastData(record);
        }

        private void UpdateForecastData(ICoin record)
        {
            var dailyChange =
                Numerics.CalculateTrend(this.HistoricalGraph.Values, PollDepth);
            //var forecastValue =
            //    Numerics.CalculateFutureValue(dailyChange, record.CurrentValue, ForecastReach, Randomization);
            
            this.ForecastGraph =
                new ForecastGraph(
                    this.HistoricalGraph,
                    labels:
                        Helpers.DatesFromNow(DateTimeOffset.Now, DefaultForecastPopulation),
                    values:
                        Normalize(
                            Numerics.CalculateFutureValueSteps(dailyChange, record.CurrentValue, DefaultForecastPopulation, Randomization), 
                            this._coinService.BaseCurrency, 
                            DefaultAccuracyAdjust));

            var forecastValue = this.ForecastGraph.Values.LastOrDefault();

            this.ForecastData =
                new ForecastData
                {
                    DailyChange =
                        Normalize(dailyChange, this._coinService.BaseCurrency, DefaultAccuracyAdjust),
                    Trend =
                        Normalize(
                            Numerics.CalculateLinearTrend(this.HistoricalGraph.Values, PollDepth),
                            this._coinService.BaseCurrency, 
                            DefaultAccuracyAdjust),
                    ForecastValue =
                        Normalize(forecastValue, this._coinService.BaseCurrency),
                    ForecastWorth =
                        Normalize(
                            Numerics.CalculateFutureWorth(forecastValue, this.Count),
                            this._coinService.BaseCurrency),
                };
        }

        private void UpdateNearForecastData(ICoin record)
        {
            var nearDailyChange =
                Numerics.CalculateTrend(this.HistoricalGraph.Values, NearPollDepth);
            //var nearForecastValue =
            //    Numerics.CalculateFutureValue(nearDailyChange, record.CurrentValue, ForecastReach, Randomization);

            this.NearForecastGraph =
                new ForecastGraph(
                this.HistoricalGraph,
                    labels:
                        Helpers.DatesFromNow(DateTimeOffset.Now, DefaultForecastPopulation),
                    values:
                        Normalize(
                            Numerics.CalculateFutureValueSteps(nearDailyChange, record.CurrentValue, DefaultForecastPopulation, Randomization), 
                            this._coinService.BaseCurrency,
                            DefaultAccuracyAdjust));

            var nearForecastValue = this.NearForecastGraph.Values.LastOrDefault();

            this.NearForecastData =
                new ForecastData
                {
                    DailyChange =
                        Normalize(nearDailyChange, this._coinService.BaseCurrency, DefaultAccuracyAdjust),
                    Trend =
                        Normalize(
                            Numerics.CalculateLinearTrend(this.HistoricalGraph.Values, NearPollDepth),
                            this._coinService.BaseCurrency,
                            DefaultAccuracyAdjust),
                    ForecastValue =
                        Normalize(nearForecastValue, this._coinService.BaseCurrency),
                    ForecastWorth =
                        Normalize(
                            Numerics.CalculateFutureWorth(nearForecastValue, this.Count),
                            this._coinService.BaseCurrency)
                };
        }

        private void UpdateFarForecastData(ICoin record)
        {
            var farHistoricalValues =
                record.History?.GetValues(FarPollDepth + 1) ?? new double[FarPollDepth + 1];
            var farDailyChange =
                Numerics.CalculateTrend(farHistoricalValues, FarPollDepth);
            //var farForecastValue =
            //    Numerics.CalculateFutureValue(farDailyChange, record.CurrentValue, ForecastReach, Randomization);
            
            this.FarForecastGraph =
                new ForecastGraph(
                    this.HistoricalGraph,
                    labels:
                        Helpers.DatesFromNow(DateTimeOffset.Now, DefaultForecastPopulation),
                    values:
                        Normalize(
                            Numerics.CalculateFutureValueSteps(farDailyChange, record.CurrentValue, DefaultForecastPopulation, Randomization), 
                            this._coinService.BaseCurrency,
                            DefaultAccuracyAdjust));

            var farForecastValue = this.FarForecastGraph.Values.LastOrDefault();

            this.FarForecastData =
                new ForecastData
                {
                    DailyChange =
                        Normalize(farDailyChange, this._coinService.BaseCurrency, DefaultAccuracyAdjust),
                    Trend =
                        Normalize(
                            Numerics.CalculateLinearTrend(farHistoricalValues, FarPollDepth),
                            this._coinService.BaseCurrency,
                            DefaultAccuracyAdjust),
                    ForecastValue =
                        Normalize(farForecastValue, this._coinService.BaseCurrency),
                    ForecastWorth =
                        Normalize(
                            Numerics.CalculateFutureWorth(farForecastValue, this.Count),
                            this._coinService.BaseCurrency),
                };
        }
    }
}