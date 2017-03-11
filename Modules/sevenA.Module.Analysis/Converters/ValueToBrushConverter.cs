namespace sevenA.Module.Analysis.Converters
{
    using System;
    using System.Windows.Data;
    using System.Windows.Media;

    public class ValueToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
            {
                return Brushes.WhiteSmoke;
            }

            double result;
            if (!double.TryParse(value.ToString(), out result))
            {
                return Brushes.WhiteSmoke;
            }

            return result >= 0 ? Brushes.Green : Brushes.Red;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}