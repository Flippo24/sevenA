namespace sevenA.Core.Stats
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using sevenA.Core.Helpers;

    public static class Stats
    {
        public static LinearFitCoefficients? LinearFit(double[] datax, double[] datay)
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
            if (!fit.HasValue)
            {
                return null;
            }

            var predicted = fit.Value.A + (fit.Value.B * x);
            return new LinearFit
            {
                Predicted = predicted,
                Lower1Sigma = predicted - (fit.Value.Sigma * 1.65),
                Upper1Sigma = predicted + (fit.Value.Sigma * 1.65)
            };
        }

        public static double SimpleAverage(double[] data)
        {
            return data.Average();
        }

        public static double SimpleAverage(double[] data, int numberPoints)
        {
            if (data.Length < numberPoints)
            {
                throw new ArgumentException("Not enough datapoints to calculate average");
            }

            return data.Take(numberPoints).Average();
        }

        public static double PercentageChange(double[] data, int numberPoints)
        {
            if (data.Length < numberPoints)
            {
                throw new ArgumentException("Not enough datapoints to calculate change");
            }

            var subSet = data.Take(numberPoints).ToArray();

            // ReSharper disable once CompareOfFloatsByEqualityOperator
            return subSet.Last() != 0 ? (subSet.First() - subSet.Last()) / subSet.Last() * 100.0 / numberPoints : ApplicationHelper.Instance.RiskFreeRate;
        }

        public static double GetMedianPercentageChange(double[] data, int delta)
        {
            var listOfCAGRs = new List<double>();
            for (int i = 0; i < data.Length; i++)
            {
                try
                {
                    var selectedPoints = data.Skip(i).Take(delta).ToArray();
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

            return GetMedian(listOfCAGRs.ToArray());
        }

        public static double CAGR(double start, double end, int numberYears)
        {
            return Math.Pow(end / start, 1.0 / numberYears) - 1.0;
        }

        public static double WeightedAverage(double[] data, double[] weights)
        {
            return Enumerable.Range(0, data.Length).Select(i => data.ElementAt(i) * weights.ElementAt(i)).Sum() / weights.Sum();
        }

        public static double GetMedian(double[] sourceNumbers)
        {
            if (sourceNumbers == null || !sourceNumbers.Any())
            {
                throw new Exception("Median of empty array not defined.");
            }

            double[] sortedPNumbers = sourceNumbers.ToArray();
            Array.Sort(sortedPNumbers);

            int size = sortedPNumbers.Length;
            int mid = size / 2;
            double median = size % 2 != 0 ? sortedPNumbers[mid] : (sortedPNumbers[mid] + sortedPNumbers[mid - 1]) / 2;
            return median;
        }

        private static double SumSquared(double[] data)
        {
            var average = data.Average();
            return data.Select(x => Math.Pow(x - average, 2)).Sum();
        }

        private static double SumSquaredxy(double[] datax, double[] datay)
        {
            if (datax.Length != datay.Length)
            {
                throw new ArgumentException("x and y arrays must have the same dimensions");
            }

            var averagex = datax.Average();
            var averagey = datay.Average();
            return Enumerable.Range(0, datax.Length).Select(i => (datax.ElementAt(i) - averagex) * (datay.ElementAt(i) - averagey)).Sum();
        }

        private static double LinearFitB(double[] datax, double[] datay)
        {
            return SumSquaredxy(datax, datay) / SumSquared(datax);
        }

        private static double LinearFitA(double[] datax, double[] datay)
        {
            return datay.Average() - (LinearFitB(datax, datay) * datax.Average());
        }

        private static double RSquared(double[] datax, double[] datay)
        {
            return Math.Pow(SumSquaredxy(datax, datay), 2) / (SumSquared(datax) * SumSquared(datay));
        }

        private static double LinearFitSigma(double[] datax, double[] datay)
        {
            var s = Math.Sqrt((SumSquared(datay) - (LinearFitB(datax, datay) * SumSquaredxy(datax, datay))) / (datax.Length - 2));
            return s;
        }
    }
}