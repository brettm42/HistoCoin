
namespace HistoCoin.Server.ViewModels.Forecast
{
    public class ForecastGraph
    {
        public int Id => this.GetHashCode();

        public string[] Labels { get; set; }

        public double[] Values { get; set; }
    }
}
