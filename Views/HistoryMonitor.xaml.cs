using DPM_Utility.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DPM_Utility.Views
{
    /// <summary>
    /// HistoryMonitor.xaml 的交互逻辑
    /// </summary>
    public partial class HistoryMonitor : UserControl
    {
        public HistoryMonitor()
        {
            InitializeComponent();
            this.DataContext = new HistoryMonitorViewModel();
        }

        private void Border_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
