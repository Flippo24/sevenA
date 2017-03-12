namespace sevenA.Module.Analysis.Models
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;

    using DevExpress.Mvvm;

    using Enums;

    using sevenA.Core.Stats;

    public class FinancialRatio : BindableBase
    {
        private double? _deltaLongTerm;

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

        public double? DeltaLongTerm
        {
            get
            {
                return this._deltaLongTerm.HasValue && !double.IsNaN(this._deltaLongTerm.Value)
                           ? this._deltaLongTerm
                           : null;
            }

            set
            {
                this._deltaLongTerm = value;
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
               DateTime.TryParseExact(
                   x.Item1,
                   "yyyy-MM",
                   CultureInfo.InvariantCulture,
                   DateTimeStyles.None,
                   out DateTime cDate)).Item2
           : null;
                }
                catch
                {
                    return null;
                }
            }
        }

        public string Name { get; set; }

        public double? RegressionCoef
        {
            get
            {
                return this._regressionCoef.HasValue && !double.IsNaN(this._regressionCoef.Value)
                           ? this._regressionCoef
                           : null;
            }

            set
            {
                this._regressionCoef = value;
            }
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
                    if (DateTime.TryParseExact(
                        tuple.Item1,
                        "yyyy-MM",
                        CultureInfo.InvariantCulture,
                        DateTimeStyles.None,
                        out DateTime date) && tuple.Item2.HasValue)
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
                    selectedData.Select(x => x.Item1),
                    selectedData.Select(x => x.Item2));

                this._regressionCoef = linearFitCoeffs?.B;

                // long term
                var selectedDataLongTerm =
                    convertedData.Where(x => x.Item1 >= convertedData.Last().Item1.AddYears(-6))
                        .Select(x => Tuple.Create(x.Item1.ToOADate(), x.Item2)).ToList();

                var linearFitCoeffsLongTerm = Stats.LinearFit(
                    selectedDataLongTerm.Select(x => x.Item1),
                    selectedDataLongTerm.Select(x => x.Item2));

                var date0 = selectedDataLongTerm.First().Item1;
                var date1 = selectedDataLongTerm.Last().Item1;

                // ReSharper disable once StyleCop.SA1305
                double nYears = (DateTime.FromOADate(date1) - DateTime.FromOADate(date0)).TotalDays / 365.0;

                this._deltaLongTerm = linearFitCoeffsLongTerm?.B * (date1 - date0)
                                      / (linearFitCoeffsLongTerm?.A + linearFitCoeffsLongTerm?.B * date0) * 100.0
                                      / nYears;
            }
            catch
            {
                this._regressionCoef = null;
                this._deltaLongTerm = null;
            }
        }
    }
}