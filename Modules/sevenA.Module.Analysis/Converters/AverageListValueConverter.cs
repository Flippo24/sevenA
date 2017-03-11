namespace sevenA.Module.Analysis.Converters
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Windows.Data;

    using Core.Stats;

    public class AverageListValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var data = value as List<Tuple<string, double?, double?>>;

            if (data == null)
            {
                return null;
            }

            var dict = new Dictionary<DateTime, double?>();
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
                    dict.Add(date, tuple.Item2);
                }
            }

            var selectedData = dict.Where(x => x.Key >= dict.Last().Key.AddYears(-3));
            var keyValuePairs = selectedData as IList<KeyValuePair<DateTime, double?>> ?? selectedData.ToList();
            var weights = Enumerable.Range(1, keyValuePairs.Count).Select(i => (double)i);

            return dict.Any() ? Stats.WeightedAverage(keyValuePairs.Select(x => x.Value.GetValueOrDefault()), weights) : (double?)null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}