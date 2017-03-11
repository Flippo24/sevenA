using System;
using System.Collections.Generic;
using System.Linq;
using sevenA.Core.Helpers;

namespace sevenA.Core.Stats
{
    public class Stats
    {
        public static LinearFitCoefficients? LinearFit(IEnumerable<double> datax, IEnumerable<double> datay)
        {
            if (datax.Any() && datay.Any())
            {
                return new LinearFitCoefficients
                {
                    A = LinearFitA(datax, datay),
                    B = LinearFitB(datax, datay),
                    R2 = RSquared(datax, datay),
                    Sigma = LinearFitSigma(datax, datay)
                };
            }

            return null;
        }

        public static LinearFit? LinearFitPredict(double x, LinearFitCoefficients? fit)
        {
            if (!fit.HasValue) return null;

            var predicted = fit.Value.A + fit.Value.B * x;
            return new LinearFit
            {
                Predicted = predicted,
                Lower1Sigma = predicted - fit.Value.Sigma * 1.65,
                Upper1Sigma = predicted + fit.Value.Sigma * 1.65
            };
        }

        public static double SimpleAverage(IEnumerable<double> data)
        {
            return data.Average();
        }

        public static double SimpleAverage(IEnumerable<double> data, int nPoints)
        {
            if (data.Count() < nPoints) throw new ArgumentException("Not enough datapoints to calculate average");

            return data.Take(nPoints).Average();
        }

        public static double PercentageChange(IEnumerable<double> data, int nPoints)
        {
            if (data.Count() < nPoints) throw new ArgumentException("Not enough datapoints to calculate change");

            var subSet = data.Take(nPoints);

            return subSet.Last() != 0 ? (subSet.First() - subSet.Last()) / subSet.Last() * 100.0 / nPoints : ApplicationHelper.Instance.RiskFreeRate;
        }

        public static double GetMedianPercentageChange(IEnumerable<double> data, int delta)
        {
            var listOfCAGRs = new List<double>();
            for (int i = 0; i < data.Count(); i++)
            {
                try
                {
                    var selectedPoints = data.Skip(i).Take(delta);
                    double growth = PercentageChange(selectedPoints, delta);
                    if (!double.IsInfinity(growth) && !double.IsNaN(growth))
                    {
                        listOfCAGRs.Add(growth);
                    }
                }
                catch
                {
                    // ignore
                }
            }

            return GetMedian(listOfCAGRs);
        }

        public static double CAGR(double start, double end, int nYears)
        {
            return Math.Pow(end / start, 1.0 / nYears) - 1.0;
        }

        public static double WeightedAverage(IEnumerable<double> data, IEnumerable<double> weights)
        {
            return Enumerable.Range(0, data.Count()).Select(i => data.ElementAt(i) * weights.ElementAt(i)).Sum() / weights.Sum();
        }

        public static double GetMedian(IEnumerable<double> sourceNumbers)
        {
            if (sourceNumbers == null || !sourceNumbers.Any())
                throw new Exception("Median of empty array not defined.");

            double[] sortedPNumbers = sourceNumbers.ToArray();
            Array.Sort(sortedPNumbers);

            int size = sortedPNumbers.Length;
            int mid = size / 2;
            double median = size % 2 != 0 ? sortedPNumbers[mid] : (sortedPNumbers[mid] + sortedPNumbers[mid - 1]) / 2;
            return median;
        }

        private static double SumSquared(IEnumerable<double> data)
        {
            var average = data.Average();
            return data.Select(x => Math.Pow(x - average, 2)).Sum();
        }

        private static double SumSquaredxy(IEnumerable<double> datax, IEnumerable<double> datay)
        {
            if (datax.Count() != datay.Count())
                throw new ArgumentException("x and y arrays must have the same dimensions");

            var averagex = datax.Average();
            var averagey = datay.Average();
            return Enumerable.Range(0, datax.Count()).Select(i => (datax.ElementAt(i) - averagex) * (datay.ElementAt(i) - averagey)).Sum();
        }

        private static double LinearFitB(IEnumerable<double> datax, IEnumerable<double> datay)
        {
            return SumSquaredxy(datax, datay) / SumSquared(datax);
        }

        private static double LinearFitA(IEnumerable<double> datax, IEnumerable<double> datay)
        {
            return datay.Average() - LinearFitB(datax, datay) * datax.Average();
        }

        private static double RSquared(IEnumerable<double> datax, IEnumerable<double> datay)
        {
            return Math.Pow(SumSquaredxy(datax, datay), 2) / (SumSquared(datax) * SumSquared(datay));
        }

        private static double LinearFitSigma(IEnumerable<double> datax, IEnumerable<double> datay)
        {
            var n = datax.Count();
            var s = Math.Sqrt((SumSquared(datay) - LinearFitB(datax, datay) * SumSquaredxy(datax, datay)) / (n - 2));
            return s;
        }
    }


}