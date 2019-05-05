using System.Windows;
using System.Windows.Controls;

namespace WalkVisualizer
{
    public class ControlWithLabel : ContentControl
    {
        public static readonly DependencyProperty LabelTextProperty = 
            DependencyProperty.Register(nameof(LabelText), typeof(string), typeof(ControlWithLabel));

        static ControlWithLabel()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(ControlWithLabel), 
                new FrameworkPropertyMetadata(typeof(ControlWithLabel)));
        }

        public string LabelText
        {
            get => (string)GetValue(LabelTextProperty);
            set => SetValue(LabelTextProperty, value);
        }
    }
}