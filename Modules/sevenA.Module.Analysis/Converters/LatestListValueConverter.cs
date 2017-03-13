﻿namespace sevenA.Module.Analysis.Converters
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Data;

    public class LatestListValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var data = value as List<Tuple<string, double?, double?>>;

            return data != null && data.Any() && data.Any(x => x.Item2 != null) ? data.Last(x => x.Item2 != null).Item2 : null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}