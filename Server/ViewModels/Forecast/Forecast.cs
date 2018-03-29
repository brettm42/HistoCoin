
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
        private readonly ICoinService _coinService;
        
        public RoutingState RoutingState { get; set; }

        public IEnumerable<CoinInfo> Coins =>
            this._coinService
                .GetAll()
                .Where(i => i != null)
                .OrderBy(i => i.Handle)
                .Select(i =>
                    new CoinInfo
                    {
                        Id = i.Id,
                        Handle = i.Handle,
                        Count = i.Count,
                        StartingValue = i.StartingValue,
                        Route = this.Redirect(AppLayout.FormPagePath, i.Id.ToString())
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

        public double[] HistoricalValues
        {
            get => Get<double[]>();

            set => Set(value);
        }

        public string[] HistoricalDates
        {
            get => Get<string[]>();

            set => Set(value);
        }

        public double DailyChange
        {
            get => Get<double>();

            set => Set(value);
        }
        
        public Forecast(ICoinService coinService)
        {
            this._coinService = coinService;

            this.OnRouted((sender, e) =>
            {
                if (!int.TryParse(e?.From?.Replace($"{AppLayout.FormPagePath}/", string.Empty), out var id))
                {
                    return;
                }

                this.LoadCoin(
                    id == 1 
                        ? this._coinService.GetFirstId() 
                        : id);
            });
        }
        
        public Action<int> Cancel => LoadCoin;

        private void LoadCoin(int id)
        {
            var record = this._coinService.GetById(id);
            if (record != null)
            {
                this.Id = record.Id;
                this.Handle = record.Handle;
                this.Count = record.Count;
                this.StartingValue = Normalize(record.StartingValue, this._coinService.BaseCurrency);
                this.CurrentValue = Normalize(record.CurrentValue, this._coinService.BaseCurrency);
                this.Delta = Normalize(record.Delta, this._coinService.BaseCurrency);
                this.Worth = Normalize(record.Worth, this._coinService.BaseCurrency);
                this.HistoricalDates = record.History?.GetDates(DefaultHistoryPopulation) ?? new string[0];
                this.HistoricalValues = record.History?.GetValues(DefaultHistoryPopulation) ?? new double[0];
                this.DailyChange =
                    Normalize(
                        Numerics.CalculateTrend(this.HistoricalValues, depth: 25), 
                        this._coinService.BaseCurrency);
                //this.LowerBound
                //this.UpperBound
                //this.Forecast
            }
        }
    }
}