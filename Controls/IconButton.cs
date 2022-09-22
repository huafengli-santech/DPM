using System.Windows;
using System.Windows.Controls;
namespace DPM_Utility.Controls
{
    class IconButton : Button
    {
        public string IconOnlyData
        {
            get { return (string)GetValue(IconOnlyDataProperty); }
            set { SetValue(IconOnlyDataProperty, value); }
        }
        // Using a DependencyProperty as the backing store for IconOnlyData.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IconOnlyDataProperty =
            DependencyProperty.Register(nameof(IconOnlyData), typeof(string), typeof(IconButton), new PropertyMetadata(default(string)));
    }
}
