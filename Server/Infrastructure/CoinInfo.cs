
namespace HistoCoin.Server.Infrastructure
{
    using DotNetify.Routing;

    public class CoinInfo
    {
        public int Id { get; set; }

        public string Handle { get; set; }

        public double? Count { get; set; }

        public double? StartingValue { get; set; }

        public Route Route { get; set; }
    }
}
