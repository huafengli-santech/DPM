using DPM_Utility.ViewModels;
using System.Windows.Controls;

namespace DPM_Utility.Views
{
    /// <summary>
    /// SoftWareSetup.xaml 的交互逻辑
    /// </summary>
    public partial class SoftWareSetup : UserControl
    {
        public SoftWareSetup()
        {
            InitializeComponent();
            this.DataContext = new SoftWareViewModel();
        }
    }
}
