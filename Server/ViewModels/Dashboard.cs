
namespace HistoCoin.Server.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reactive.Linq;
    using System.Threading;
    using DotNetify;
    using DotNetify.Routing;
    using DotNetify.Security;
    using HistoCoin.Server.Services.CurrencyService;
    using static HistoCoin.Server.Infrastructure.Constants;

    [Authorize]
    public class Dashboard : BaseVM, IRoutable
    {
        private IDisposable _subscription;

        private bool _isSyncing;

        public class Currency
        {
            public string Handle { get; set; }

            public Route Route { get; set; }

            public string Value { get; set; }

            public string Count { get; set; }

            public string Result { get; set; }
        }

        public RoutingState RoutingState { get; set; }

        public bool IsSyncing
        {
            get
            {
                var value = this._isSyncing;
                this._isSyncing = false;

                return value;
            }
            set
            {
                this._isSyncing = value;
                Changed(nameof(this.IsSyncing));
            }
        }

        public Dashboard(ICurrencyService dataService)
        {
            AddProperty<string[]>("Currencies").SubscribeTo(dataService.Coins);
            AddProperty<double[]>("Value").SubscribeTo(dataService.Value);
            AddProperty<double>("TotalValueUsd").SubscribeTo(dataService.TotalValueUsd);
            AddProperty<double>("TotalValueBtc").SubscribeTo(dataService.TotalValueBtc);
            AddProperty<double[]>("CurrentDeltas").SubscribeTo(dataService.CurrentDeltas);
            AddProperty<double>("OverallDelta").SubscribeTo(dataService.OverallDelta);
            AddProperty<int[]>("DistributionUsd").SubscribeTo(dataService.DistributionUsd);
            AddProperty<int[]>("DistributionBtc").SubscribeTo(dataService.DistributionBtc);

            AddProperty<Currency[]>("CurrentValues")
                .SubscribeTo(
                    dataService.CurrentValues.Select(
                        value =>
                        {
                            var values =
                                new Queue<Currency>(
                                    Get<Currency[]>(nameof(dataService.CurrentValues))?.Reverse() 
                                ?? new Currency[] { });

                            values.Enqueue(
                                new Currency
                                {
                                    Handle = value.Handle,
                                    Value = $"{value.Value}",
                                    Count = $"{value.Count}",
                                    Result = $"{(value.Value < 0 ? 0 : value.Value) * value.Count}",
                                    Route = this.Redirect(AppLayout.FormPagePath, value.Id.ToString())
                                });

                            if (values.Count > Get<string[]>("Currencies").Length)
                            {
                                values.Dequeue();
                            }

                            return values.Reverse().ToArray();
                        }));

            // Regulate data update interval to no less than every 200 msecs.
            _subscription =
                Observable
                    .Interval(TimeSpan.FromMilliseconds(200))
                    .StartWith(0)
                    .Subscribe(_ => PushUpdates());
        }

        public Action Sync =>
            () =>
            {
                this.IsSyncing = true;
                
                this.PushUpdates();
                
                this.IsSyncing = false;
            };

        public override void Dispose()
        {
            _subscription?.Dispose();
            base.Dispose();
        }
    }
}