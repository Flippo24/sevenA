namespace sevenA.Module.Analysis.Models
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;

    using Core.Stats;

    using DevExpress.Mvvm;

    using Enums;

    using JetBrains.Annotations;

    public class FinancialRatio : BindableBase
    {
        private double? _regressionCoef;

        public List<Tuple<string, double?, double?>> Data
        {
            get
            {
                return this.GetProperty(() => this.Data);
            }

            set
            {
                if (this.SetProperty(() => this.Data, value))
                {
                    this.SetRegressionCoef();
                }
            }
        }

        public double? Latest
        {
            get
            {
                try
                {
                    // ReSharper disable once StyleCop.SA1305
                    return this.Data != null && this.Data.Any()
           ? this.Data.Last(
               x =>
               DateTime.TryParseExact(x.Item1, "yyyy-MM", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime _)).Item2
           : null;
                }
                catch
                {
                    return null;
                }
            }
        }

        public string Name { get; set; }

        [UsedImplicitly]
        public double? RegressionCoef
        {
            get => this._regressionCoef.HasValue && !double.IsNaN(this._regressionCoef.Value)
                ? this._regressionCoef
                : null;

            set => this._regressionCoef = value;
        }

        public FinancialRatioSectionEnum Section { get; set; }

        public string StockName { get; set; }

        private void SetRegressionCoef()
        {
            try
            {
                var convertedData = new List<Tuple<DateTime, double>>();
                foreach (var tuple in this.Data)
                {
                    if (DateTime.TryParseExact(tuple.Item1, "yyyy-MM", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date) && tuple.Item2.HasValue)
                    {
                        convertedData.Add(Tuple.Create(date, tuple.Item2.Value));
                    }
                }

                if (!convertedData.Any())
                {
                    return;
                }

                var selectedData =
                    convertedData.Where(x => x.Item1 >= convertedData.Last().Item1.AddYears(-3))
                        .Select(x => Tuple.Create(x.Item1.ToOADate(), x.Item2)).ToList();

                var linearFitCoeffs = Stats.LinearFit(
                    selectedData.Select(x => x.Item1).ToArray(),
                    selectedData.Select(x => x.Item2).ToArray());

                this._regressionCoef = linearFitCoeffs?.B;
            }
            catch
            {
                this._regressionCoef = null;
            }
        }
    }
}