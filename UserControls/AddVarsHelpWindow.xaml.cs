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
using System.Windows.Shapes;
namespace DPM_Utility.UserControls
{
    /// <summary>
    /// AddVarsWindow.xaml 的交互逻辑
    /// </summary>
    public partial class AddVarsHelpWindow : Window
    {
        public AddVarsHelpWindow()
        {
            InitializeComponent();
            this.DataContext = new AddVarsVM();
        }
        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton==MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }
    }
}
