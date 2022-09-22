using DPM_Utility.ViewModels;
using System.Windows;
using System.Windows.Controls;
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
    }
}
