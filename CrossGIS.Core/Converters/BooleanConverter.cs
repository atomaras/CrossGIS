using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace CrossGIS.Core.Converters
{
    public class BooleanConverter : IValueConverter
    {
        public bool Opposite { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var b = (bool) value;
            b = Opposite ? !b : b;
            if (targetType == typeof (bool) || targetType == typeof (Boolean))
            {
                return b;
            }
            if (targetType == typeof (Visibility))
            {
                return b ? Visibility.Visible : Visibility.Collapsed;
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
