namespace sevenA.Module.Analysis.Converters
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Windows.Data;

    using Core.Stats;

    public class LinearPredictionListValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var data = value as List<Tuple<string, double?, double?>>;

            if (data == null)
            {
                return null;
            }

            var convertedData = new List<Tuple<DateTime, double>>();
            foreach (var tuple in data)
            {
                DateTime date;
                if (DateTime.TryParseExact(
                    tuple.Item1,
                    "yyyy-MM",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out date) && tuple.Item2.HasValue)
                {
                    convertedData.Add(Tuple.Create(date, tuple.Item2.Value));
                }
            }

            var selectedData = convertedData
                .Where(x => x.Item1 >= convertedData.Last().Item1.AddYears(-3))
                .Select(x => Tuple.Create(x.Item1.ToOADate(), x.Item2)).ToList();

            try
            {
                var linearFitCoeffs = Stats.LinearFit(
                    selectedData.Select(x => x.Item1),
                    selectedData.Select(x => x.Item2));
                var predictions = Stats.LinearFitPredict(DateTime.FromOADate(selectedData.Last().Item1).AddYears(1).ToOADate(), linearFitCoeffs);

                return $"{predictions?.Predicted:0.00}   [{predictions?.Lower1Sigma:0.00}; {predictions?.Upper1Sigma:0.00}]";
            }
            catch (Exception)
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}