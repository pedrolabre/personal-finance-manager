using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using PersonalFinanceManager.Models.Enums;

namespace PersonalFinanceManager.Resources.Converters
{
    public class StatusToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is StatusPendencia status)
            {
                return status switch
                {
                    StatusPendencia.EmAberto => new SolidColorBrush(Color.FromRgb(243, 156, 18)), // #F39C12
                    StatusPendencia.Acordada => new SolidColorBrush(Color.FromRgb(52, 152, 219)), // #3498DB
                    StatusPendencia.Quitada => new SolidColorBrush(Color.FromRgb(39, 174, 96)),   // #27AE60
                    StatusPendencia.Atrasada => new SolidColorBrush(Color.FromRgb(231, 76, 60)),  // #E74C3C
                    _ => new SolidColorBrush(Colors.Gray)
                };
            }
            return new SolidColorBrush(Colors.Gray);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
