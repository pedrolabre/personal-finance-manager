
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using PersonalFinanceManager.Models.Enums;

namespace PersonalFinanceManager.Resources.Converters;

public class PrioridadeToColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is Prioridade prioridade)
        {
            return prioridade switch
            {
                Prioridade.Baixa => new SolidColorBrush(Color.FromRgb(149, 165, 166)), // #95A5A6
                Prioridade.Media => new SolidColorBrush(Color.FromRgb(243, 156, 18)),  // #F39C12
                Prioridade.Alta => new SolidColorBrush(Color.FromRgb(231, 76, 60)),    // #E74C3C
                _ => new SolidColorBrush(Colors.Gray)
            };
        }
        return new SolidColorBrush(Colors.Gray);
    }
    
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
