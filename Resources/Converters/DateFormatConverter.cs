using System;
using System.Globalization;
using System.Windows.Data;

namespace PersonalFinanceManager.Resources.Converters;

public class DateFormatConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is DateTime date)
        {
            var format = parameter as string ?? "dd/MM/yyyy";
            return date.ToString(format);
        }
        return string.Empty;
    }
    
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string dateString && DateTime.TryParse(dateString, out var date))
        {
            return date;
        }
        return DateTime.Now;
    }
}
