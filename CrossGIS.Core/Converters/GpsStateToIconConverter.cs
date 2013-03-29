using System;
using System.Globalization;
using System.Windows.Data;
using CrossGIS.Core.ViewModels;

namespace CrossGIS.Core.Converters
{
    public class GpsStateToIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            GpsState state = (GpsState) value;
            if (state == GpsState.Off)
                return new Uri("/Assets/current-location.png", UriKind.Relative);
            else if(state == GpsState.AutoPanOn)
                return new Uri("/Assets/current-location-track.png", UriKind.Relative);
            else
                return new Uri("/Assets/current-location-notrack.png", UriKind.Relative);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
