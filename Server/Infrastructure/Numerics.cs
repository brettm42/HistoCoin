
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
            if (historicalValues.Length < depth)
            {
                depth = historicalValues.Length;
            }

            var sample = historicalValues.TakeLast(depth).ToArray();
            var total = sample.Sum();
            var rootMeanSqr = SquareDeviation(sample, 0, depth);

            for (var i = historicalValues.Length - 1; i >= historicalValues.Length - depth; i--)
            {
                rms.Add(SquareDeviation(historicalValues, i, historicalValues.Length));
            }

            var totalRms = SquareDeviation(rms, 0, rms.Count);

            //return (rms.Last() - rms.First()) / depth;
            return (rms.First() - rms.Last()) / depth;
        }

        public static double CalculateLinearTrend(double[] historicalValues, int depth = 25)
        {
            var (rSquared, yIntercept, slope) = 
                Numerics.LinearRegression(historicalValues, depth);

            return slope;
        }

        public static double CalculateFutureValue(double dailyChange, double currentValue, int reach, bool randomEnabled = false)
        {
            var random =
                new Random((int)DateTimeOffset.UtcNow.ToUnixTimeSeconds());

            return currentValue 
                   + dailyChange 
                   * reach 
                   * (randomEnabled ? (random.NextDouble() + random.NextDouble()) + random.NextDouble() : 1);
        }

        public static double[] CalculateFutureValueSteps(double dailyChange, double currentValue, int steps, bool randomEnabled = false)
        {
            var output = new double[steps + 1];

            output[0] = currentValue;

            var random = 
                new Random((int)DateTimeOffset.UtcNow.ToUnixTimeSeconds());

            for (var i = 1; i < output.Length; i++)
            {
                output[i] = 
                    output[i - 1] 
                    + dailyChange 
                    * (randomEnabled ? random.NextDouble() : 1);
            }

            return output;
        }

        public static double CalculateFutureWorth(double futureValue, double walletCount)
        {
            return futureValue * walletCount;
        }

        public static (string Label, double value) CalculateBest(double[] values, string[] labels)
        {
            if (values is null || labels is null)
            {
                return ("-", 0);
            }

            var (index, maxValue) = (0, 0.0);

            for (var i = 0; i < values.Length; i++)
            {
                if (values[i] > maxValue)
                {
                    index = i;
                    maxValue = values[i];
                }
            }

            return (labels[index], maxValue);
        }

        public static string CalculateBestAsString(double[] values, string[] labels)
        {
            var (label, maxValue) = Numerics.CalculateBest(values, labels);

            return $"{maxValue}\r\n({label.Substring(0, label.IndexOf(' '))})";
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

            if (values.Count <= depth)
            {
                depth = values.Count - 1;
            }

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

            return 
                (Math.Pow(doubleR, 2), meanY - (sCo / ssX * meanX), sCo / ssX);
        }

        public static double AverageTrends(double trend0, double trend1)
        {
            return (trend0 + trend1) / 2;
        }
    }
}
