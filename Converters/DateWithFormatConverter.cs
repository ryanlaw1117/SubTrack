using System;
using System.Globalization;
using System.Windows.Data;

namespace Subscription_Manager.Converters
{
    public class DateWithFormatConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {

            if (values == null || values.Length < 2)
                return "Never";

            if (values[0] is not DateTime dt)
                return "Never";

            var fmt = values[1] as string;
            if (string.IsNullOrWhiteSpace(fmt))
                fmt = "MMM d, yyyy";

            try
            {
                return dt.ToString(fmt, CultureInfo.CurrentCulture);
            }
            catch
            {

                return dt.ToString("MMM d, yyyy", CultureInfo.CurrentCulture);
            }
        }
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
            => throw new NotSupportedException();
    }
}