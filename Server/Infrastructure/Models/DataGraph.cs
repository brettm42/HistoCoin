
namespace HistoCoin.Server.Infrastructure.Models
{
    using System.Collections.Generic;
    using System.Linq;

    public class DataGraph
    {
        public DataGraph()
        {
        }

        public DataGraph(IEnumerable<string> labels, IEnumerable<double> values)
        {
            this.Labels = labels.ToArray();
            this.Values = values.ToArray();
        }
        
        public DataGraph(DataGraph initialValues, IEnumerable<string> labels, IEnumerable<double> values)
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
    }
}
