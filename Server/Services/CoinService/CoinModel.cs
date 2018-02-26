
namespace HistoCoin.Server.Services.CoinService
{
    using HistoCoin.Server.Infrastructure;

    public class CoinModel
    {
        public int Id { get; set; }

        public string Handle { get; set; }

        public double Count { get; set; }

        public double StartingValue { get; set; }

        public double CurrentValue { get; set; }

        public double Worth => this.CurrentValue < 0 ? 0 : this.CurrentValue * this.Count;

        public double Delta { get; set; } = 0;

        public History History { get; set; }
    }
}
