namespace HistoCoin.Server.ViewModels.Form
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
    public class Form : BaseVM, IRoutable
    {
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

        public double Trend
        {
            get => Get<double>();

            set => Set(value);
        }
        
        public Form(ICoinService coinService)
        {
            this._coinService = coinService;

            this.OnRouted((sender, e) =>
            {
                if (!int.TryParse(e?.From?.Replace($"{AppLayout.FormPagePath}/", string.Empty), out var id))
                {
                    return;
                }

                this.LoadCoin(
                    id == 1 ? this._coinService.GetFirstId() : id);
            });
        }
        
        public Action<int> Cancel => this.LoadCoin;

        public Action<SavedCoinInfo> Save => changes =>
        {
            var record = this._coinService.GetById(changes.Id);
            if (record is null)
            {
                return;
            }
            
            record.Handle = changes.Handle;
            record.Count = changes.Count ?? record.Count;
            record.StartingValue = changes.StartingValue ?? record.StartingValue;

            this._coinService.Update(record);
            Changed(nameof(this.Coins));
        };

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
            this.StartingValue = Normalize(record.StartingValue, this._coinService.BaseCurrency);
            this.CurrentValue = Normalize(record.CurrentValue, this._coinService.BaseCurrency);
            this.Delta = Normalize(record.Delta, this._coinService.BaseCurrency);
            this.Worth = Normalize(record.Worth, this._coinService.BaseCurrency);
            this.HistoricalDates = record.History?.GetDates(DefaultHistoryPopulation) ?? new string[0];
            this.HistoricalValues = record.History?.GetValues(DefaultHistoryPopulation) ?? new double[0];
            this.Trend =
                Normalize(
                    Numerics.CalculateLinearTrend(this.HistoricalValues), 
                    this._coinService.BaseCurrency);
        }
    }
}