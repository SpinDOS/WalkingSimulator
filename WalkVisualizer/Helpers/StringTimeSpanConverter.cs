using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace WalkVisualizer
{
    public sealed class StringTimeSpanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var timeSpan = (TimeSpan) value;
            var milliseconds = (int) Math.Ceiling(timeSpan.TotalMilliseconds);
            return milliseconds.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return int.TryParse((string)value, out var num)
                ? TimeSpan.FromMilliseconds(num) 
                : DependencyProperty.UnsetValue;
        }
    }
}