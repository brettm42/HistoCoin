
namespace HistoCoin.Server.ViewModels.Forecast
{
    public class ForecastData
    {
        public int Id => this.GetHashCode();

        public double DailyChange { get; set; }

        public double Trend { get; set; }

        public double ForecastValue { get; set; }

        public double ForecastWorth { get; set; }
    }
}
