using System;
using System.Globalization;
using System.Windows.Data;

public class PriceWithCurrencyConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values.Length < 2)
            return "";

        if (values[0] is decimal cost && values[1] is string symbol)
            return $"{symbol}{cost:F2}";

        return "";
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}