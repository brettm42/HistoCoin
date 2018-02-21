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

        public Form(ICoinService coinService)
        {
            this._coinService = coinService;

            this.OnRouted((sender, e) =>
            {
                if (int.TryParse(e?.From?.Replace($"{AppLayout.FormPagePath}/", string.Empty), out var id))
                {
                    this.LoadCoin(id);
                }
            });
        }

        private void LoadCoin(int id)
        {
            var record = this._coinService.GetById(id);
            if (record != null)
            {
                this.Handle = record.Handle;
                this.Count = record.Count;
                this.StartingValue = record.StartingValue;
                this.Id = record.Id;
            }
        }
    }
}