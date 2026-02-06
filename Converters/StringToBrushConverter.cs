using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Subscription_Manager.Converters
{
    public class StringToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not string s || string.IsNullOrWhiteSpace(s))
                return Brushes.Transparent;

            try
            {
                return (SolidColorBrush)new BrushConverter().ConvertFromString(s)!;
            }
            catch
            {
                return Brushes.Transparent;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => Binding.DoNothing;
    }
}