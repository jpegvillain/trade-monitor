using System.Windows;
using System.Windows.Controls;

namespace TradeMonitor.App.Controls
{
    public class StatusBadge : Control
    {
        static StatusBadge()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(StatusBadge),
                new FrameworkPropertyMetadata(typeof(StatusBadge)));
        }

        public string Status
        {
            get => (string)GetValue(StatusProperty);
            set => SetValue(StatusProperty, value);
        }

        public static readonly DependencyProperty StatusProperty =
            DependencyProperty.Register(
                nameof(Status),
                typeof(string),
                typeof(StatusBadge),
                new PropertyMetadata(string.Empty));
    }
}