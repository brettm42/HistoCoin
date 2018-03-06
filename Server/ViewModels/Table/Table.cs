namespace HistoCoin.Server.ViewModels.Table
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DotNetify;
    using DotNetify.Security;
    using HistoCoin.Server.Infrastructure;
    using HistoCoin.Server.Services.CoinService;

    [Authorize]
    public class Table : BaseVM
    {
        private readonly ICoinService _coinService;
        private const int _recordsPerPage = 15;
        
        public Table(ICoinService coinService)
        {
            this._coinService = coinService;
        }

        // If you use CRUD methods on a list, you must set the item key prop name of that list
        // by defining a string property that starts with that list's prop name, followed by "_itemKey".
        public string CoinsItemKey => nameof(CoinInfo.Id);

        public IEnumerable<CoinInfo> Coins => 
            this.Paginate(
                this._coinService
                    .GetAll()
                    .Where(i => i != null)
                    .Select(i => 
                        new CoinInfo
                        {
                            Id = i.Id,
                            Handle = i.Handle,
                            Count = i.Count,
                            StartingValue = i.StartingValue,
                            LastUpdate = DateTimeOffset.Parse(i.History.GetLastEntryTime()),
                        }));

        public Action<string> Add => coinDetails =>
        {
            var details = coinDetails.Split(new [] {' '}, 3);
            var newRecord = 
                new CoinModel
                {
                    Handle = details.First(),
                    Count = double.Parse(details.Skip(1).First()),
                    StartingValue = double.Parse(details.Length > 2 ? details.Last() : "0"),
                };

            this.AddList(
                nameof(this.Coins), 
                new CoinInfo
                {
                    Id = this._coinService.Add(newRecord),
                    Handle = newRecord.Handle,
                    Count = newRecord.Count,
                    StartingValue = newRecord.StartingValue,
                    LastUpdate = DateTimeOffset.MinValue,
                });

            this.SelectedPage = this.GetPageCount(this._coinService.GetAll().Count());
        };

        public Action<CoinInfo> Update => changes =>
        {
            var record = this._coinService.GetById(changes.Id);
            if (record != null)
            {
                record.Handle = changes.Handle ?? record.Handle;
                record.Count = changes.Count ?? record.Count;
                record.StartingValue = changes.StartingValue ?? record.StartingValue;
                
                this._coinService.Update(record);

                this.ShowNotification = true;
            }
        };

        public Action<int> Remove => id =>
        {
            this._coinService.Delete(id);
            this.RemoveList(nameof(this.Coins), id);

            this.ShowNotification = true;

            Changed(nameof(this.SelectedPage));
            Changed(nameof(this.Coins));
        };

        // Whether to show notification that changes have been saved.
        // Once this property is accessed, it will revert itself back to false.
        private bool _showNotification;

        public bool ShowNotification
        {
            get
            {
                var value = this._showNotification;
                this._showNotification = false;

                return value;
            }

            set
            {
                this._showNotification = value;
                Changed(nameof(this.ShowNotification));
            }
        }

        public int[] Pages
        {
            get => Get<int[]>();

            set
            {
                Set(value);
                this.SelectedPage = 1;
            }
        }

        public int SelectedPage
        {
            get => Get<int>();

            set
            {
                Set(value);
                Changed(nameof(this.Coins));
            }
        }
        
        private IEnumerable<CoinInfo> Paginate(IEnumerable<CoinInfo> coins)
        {
            // ChangedProperties is a base class property that contains a list of changed properties.
            // Here it's used to check whether user has changed the SelectedPage property value by clicking a pagination button.
            if (this.HasChanged(nameof(this.SelectedPage)))
            {
                return coins
                    .Skip(_recordsPerPage * (this.SelectedPage - 1))
                    .Take(_recordsPerPage);
            }

            this.Pages = 
                Enumerable.Range(1, this.GetPageCount(coins.Count())).ToArray();

            return coins.Take(_recordsPerPage);
        }

        private int GetPageCount(int records) => (int)Math.Ceiling(records / (double)_recordsPerPage);
    }
}