using DPM_Utility.Views;
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

namespace DPM_Utility.UserControls
{
    /// <summary>
    /// Buffer.xaml 的交互逻辑
    /// </summary>
    public partial class Buffer : UserControl
    {
        public Buffer()
        {
            InitializeComponent();
        }

        private void helpButton_Click(object sender, RoutedEventArgs e)
        {
            helpPng_Popup.IsOpen = false;
            helpPng_Popup.IsOpen = true;
        }

        private void saveToCreatBuffer_Click(object sender, RoutedEventArgs e)
        {
            //保存两个文本框内的数据
            CreatBuffer.UserVar=uservarTextBox.Text;
            CreatBuffer.UertVarCode = uservarcodeTextBox.Text;
            //将软件界面上的参数导入
            StringBuilder builder = MainWindow.creatbuffer.GetBuffer(MainWindow.T_DpmParaVar, MainWindow.T_DpmParaValue);
        }
    }
}
