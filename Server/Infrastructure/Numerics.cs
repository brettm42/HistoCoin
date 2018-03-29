
namespace HistoCoin.Server.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using static HistoCoin.Server.Infrastructure.Constants;

    public static class Numerics
    {
        public static double CalculateTrend(double[] historicalValues, int depth)
        {
            double SquareDeviation(IReadOnlyList<double> values, int start, int end)
            {
                double sum = 0;
                int i;

                for (i = start; i < end; i++)
                {
                    sum += Math.Pow(values[i], 2);
                }

                return Math.Sqrt(sum / (end - start));
            }

            var rms = new List<double>();

            var total = historicalValues.TakeLast(depth).Sum();
            var rootMeanSqr = SquareDeviation(historicalValues.TakeLast(depth).ToArray(), 0, depth);

            for (var i = historicalValues.Length - 1; i >= historicalValues.Length - depth; i--)
            {
                rms.Add(SquareDeviation(historicalValues, i, historicalValues.Length));
            }

            var totalRms = SquareDeviation(rms, 0, rms.Count);

            return totalRms / total * 100;
        }
    }
}
