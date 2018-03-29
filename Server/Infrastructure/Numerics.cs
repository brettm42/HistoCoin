
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

            return (totalRms / total * 100) * 2;
        }

        public static double CalculateLinearTrend(double[] historicalValues, int depth)
        {
            var (rSquared, yIntercept, slope) = 
                Numerics.LinearRegression(historicalValues, depth);

            return slope * 100;
        }

        private static (double RSquared, double YIntercept, double Slope) LinearRegression(IReadOnlyList<double> values, int depth)
        {
            double sumX = 0;
            double sumY = 0;
            double sumSqX = 0;
            double sumSqY = 0;
            double ssX = 0;
            double ssY = 0;
            double sumCod = 0;
            double sCo = 0;

            for (var i = values.Count - depth - 1; i < values.Count; i++)
            {
                var x = i - depth;
                var y = values[i];

                sumCod += x * y;
                sumX += x;
                sumY += y;
                sumSqX += Math.Pow(x, 2);
                sumSqY += Math.Pow(y, 2);
            }

            ssX = sumSqX - (Math.Pow(sumX, 2) / depth);
            ssY = sumSqY - (Math.Pow(sumY, 2) / depth);

            var rNum = (depth * sumCod) - (sumX * sumY);
            var rDenom =
                (depth * sumSqX - Math.Pow(sumX, 2))
                * (depth * sumSqY - Math.Pow(sumY, 2));

            sCo = sumCod - (sumX * sumY / depth);

            var meanX = sumX / depth;
            var meanY = sumY / depth;

            var doubleR = rNum / Math.Sqrt(rDenom);

            return (Math.Pow(doubleR, 2), meanY - (sCo / ssX * meanX), sCo / ssX);
        }
    }
}
