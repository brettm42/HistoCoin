
namespace HistoCoin.Server.ViewModels.Forecast
{
    using System.Linq;

    public class ForecastGraph
    {
        public ForecastGraph()
        {
        }

        public ForecastGraph(ForecastGraph initialValues, string[] labels, double[] values)
        {
            var labelSet = initialValues.Labels.ToList();
            var valueSet = initialValues.Values.ToList();

            labelSet.AddRange(labels);
            valueSet.AddRange(values);

            this.Labels = labelSet.ToArray();
            this.Values = valueSet.ToArray();
        }

        public int Id => this.GetHashCode();

        public string[] Labels { get; set; }

        public double[] Values { get; set; }

        public ForecastGraph Append(ForecastGraph additions)
        {
            foreach (var label in additions.Labels)
            {
                this.Labels.Append(label);
            }

            foreach (var value in additions.Values)
            {
                this.Values.Append(value);
            }

            return this;
        }

        public ForecastGraph Prepend(ForecastGraph additions)
        {
            foreach (var label in additions.Labels.Reverse())
            {
                this.Labels.Prepend(label);
            }

            foreach (var value in additions.Values.Reverse())
            {
                this.Values.Prepend(value);
            }

            return this;
        }
    }
}
