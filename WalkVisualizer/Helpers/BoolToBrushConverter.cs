using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace WalkVisualizer
{
    public sealed class BoolToBrushConverter: IValueConverter
    {
        public Brush OnTrue { get; set; }
        
        public Brush OnFalse { get; set; }
        
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool) value? OnTrue : OnFalse;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var brush = (Brush) value;
            if (brush == null && OnTrue == null && OnFalse == null)
                return DependencyProperty.UnsetValue;
            else if (brush == OnTrue)
                return true;
            else if (brush == OnFalse)
                return false;
            else
                return DependencyProperty.UnsetValue;
        }
    }
}