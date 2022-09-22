using DPM_Utility.ViewModels;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
namespace DPM_Utility.Views
{
    /// <summary>
    /// DpmParameter.xaml 的交互逻辑
    /// </summary>
    public partial class DpmParameter : UserControl
    {
        public DpmParameter()
        {
            InitializeComponent();
            DataContext = new DpmParameterVM();
        }
    }
}
