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
    using static HistoCoin.Server.Infrastructure.Constants;
    
    [Authorize]
    public class Form : BaseVM, IRoutable
    {
        private readonly ICoinService _coinService;
        
        public RoutingState RoutingState { get; set; }

        public IEnumerable<CoinInfo> Coins =>
            this._coinService
                .GetAll()
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
        
        public Form(ICoinService coinService)
        {
            this._coinService = coinService;

            this.OnRouted((sender, e) =>
            {
                if (!int.TryParse(e?.From?.Replace($"{AppLayout.FormPagePath}/", string.Empty), out var id))
                {
                    return;
                }

                this.LoadCoin(id == 1 
                    ? this._coinService.GetFirstId() 
                    : id);
            });
        }
        
        public Action<int> Cancel => LoadCoin;

        public Action<SavedCoinInfo> Save => changes =>
        {
            var record = this._coinService.GetById(changes.Id);
            if (record != null)
            {
                record.Handle = changes.Handle;
                record.Count = changes.Count ?? record.Count;
                record.StartingValue = changes.StartingValue ?? record.StartingValue;

                this._coinService.Update(record);
                Changed(nameof(this.Coins));
            }
        };

        private void LoadCoin(int id)
        {
            var record = this._coinService.GetById(id);
            if (record != null)
            {
                this.Handle = record.Handle;
                this.Count = record.Count;
                this.StartingValue = record.StartingValue;
                this.Id = record.Id;
                this.CurrentValue = record.CurrentValue;
                this.Delta = record.Delta;
                this.Worth = record.Worth;
                this.HistoricalDates = record.History?.GetDates(DefaultHistoryPopulation) ?? new string[0];
                this.HistoricalValues = record.History?.GetValues(DefaultHistoryPopulation) ?? new double[0];
            }
        }
    }
}