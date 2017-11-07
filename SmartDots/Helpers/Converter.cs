using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace SmartDots.Helpers
{
    public class EnumToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value.Equals(parameter);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value.Equals(true) ? parameter : Binding.DoNothing;
        }
    }

    public class EnumToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value.Equals(parameter) ? Visibility.Visible : Visibility.Hidden;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value.Equals(Visibility.Visible) ? parameter : Binding.DoNothing;
        }
    }

    public class ValuesToProportionConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            return $"{values[0] ?? "-"}/{values[1] ?? "-"}";
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
