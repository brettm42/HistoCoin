
namespace HistoCoin.Server.ViewModels.Forecast
{
    using System.Linq;

    public class ForecastGraph
    {
        public int Id => this.GetHashCode();

        public string[] Labels { get; set; }

        public double[] Values { get; set; }

        public void Append(ForecastGraph additions)
        {
            foreach (var label in additions.Labels)
            {
                this.Labels.Append(label);
            }

            foreach (var value in additions.Values)
            {
                this.Values.Append(value);
            }
        }

        public void Prepend(ForecastGraph additions)
        {
            foreach (var label in additions.Labels.Reverse())
            {
                this.Labels.Prepend(label);
            }

            foreach (var value in additions.Values.Reverse())
            {
                this.Values.Prepend(value);
            }
        }
    }
}
