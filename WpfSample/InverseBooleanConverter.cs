using System.Globalization;
using System.Windows.Data;

namespace WpfSample;

public class InverseBooleanConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => !System.Convert.ToBoolean(value);
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => !System.Convert.ToBoolean(value);
}