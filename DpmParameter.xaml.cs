using System;
using System.Collections.Generic;
using System.IO;
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

namespace DPM_Utility
{
    /// <summary>
    /// DpmParameter.xaml 的交互逻辑
    /// </summary>
    public partial class DpmParameter : Page
    {
        //ACS初始化
        ACSMotionControl m_chanel = new ACSMotionControl();
        public DpmParameter()
        {
            InitializeComponent();


            WrapPanelInit();
        }


        private void ReadIniVar()
        {
            List<string> TempParaVar = new List<string>();
            //获取para下的所有keys
            TempParaVar = MainWindow.paramFile.ReadKeys("para", MainWindow.m_ParaFileName);
            //将Ini文件导入到list
            for (int i = 0; i < TempParaVar.Count; i++)
            {
                MainWindow.T_DpmParaVar.Add(TempParaVar[i]);
                MainWindow.T_DpmParaValue.Add(MainWindow.paramFile.IniReadValue("para", TempParaVar[i]));
            }
        }

        private void WrapPanelInit()
        {
            if (File.Exists(MainWindow.m_ParaFileName)) { return; }
            TextBlock textBlock = new TextBlock();
            textBlock.Text = "单击此处配置DPM参数";
            textBlock.HorizontalAlignment = HorizontalAlignment.Center;
            textBlock.VerticalAlignment = VerticalAlignment.Center;
            textBlock.FontSize = 25;
            this.Pram_WarpPanel.Children.Add(textBlock);

            if (!File.Exists(MainWindow.m_ParaFileName))
            {
                MainWindow.paramFile.IniWriteValue("参数举例（具体参数请在  Para  组中添加添加）", "  ***参数中间用空格分隔***   ", "变量不能增加，其他变量请在状态检测界面添加");
                MainWindow.paramFile.IniWriteValue("Parameter", "测试项目", "0/1/2(0：PE检测 1：峰值电流检测 2：自定义变量检测，三者取其一，不填写默认0)");
                MainWindow.paramFile.IniWriteValue("Parameter", "采样轴号", "0 1(轴列表，最少填写一个，最大到控制器轴数限制)");
                MainWindow.paramFile.IniWriteValue("Parameter", "测量类型", "0 0(0：加速段；1：匀速段；2：加减速段；3：任意轨迹，需要与轴个数一一对应，不填写默认0)");
                MainWindow.paramFile.IniWriteValue("Parameter", "采样阈值", "0.001 0.002(需要与轴个数一一对应，不填写默认0)");
                MainWindow.paramFile.IniWriteValue("Parameter", "存放buffer号", "0(存放代码的buffer号，最大到控制器buffer限制)");
                MainWindow.paramFile.IniWriteValue("Parameter", "驱动峰值电流", "40 20(可选参数，选择峰值电流检测时使用，轴所在从站电流最大值，需要与轴个数一一对应，不填写默认0)");
                MainWindow.paramFile.IniWriteValue("Parameter", "模拟量输入分辨率", "12 16(可选参数，选择峰值电流检测时使用，需要与轴个数一一对应（bit位），不填写默认0)");

                MainWindow.paramFile.IniWriteValue("Para", "测试项目", "");
                MainWindow.paramFile.IniWriteValue("Para", "采样轴号", "");
                MainWindow.paramFile.IniWriteValue("Para", "测量类型", "");
                MainWindow.paramFile.IniWriteValue("Para", "采样阈值", "");
                MainWindow.paramFile.IniWriteValue("Para", "存放buffer号", "");
                MainWindow.paramFile.IniWriteValue("Para", "驱动峰值电流", "");
                MainWindow.paramFile.IniWriteValue("Para", "模拟量输入分辨率", "");
            }

            Pram_WarpPanel.MouseUp += Pram_WarpPanel_MouseUp;
        }

        //打开文件
        private void Pram_WarpPanel_MouseUp(object sender, MouseButtonEventArgs e)
        {
            System.Diagnostics.Process.Start(MainWindow.m_ParaFileName);
            this.Pram_WarpPanel.Children.Clear();
            ReadIniVar();
            DynamicAddPram(MainWindow.T_DpmParaVar, MainWindow.T_DpmParaValue);
        }

        /// <summary>
        /// 动态加载参数
        /// </summary>
        /// <param name="loopstr"></param>
        /// <param name="textstr"></param>
        /// <param name="defaulttext"></param>
        private void DynamicAddPram(List<string> text, List<string> value)
        {
            for (int i = 0; i < text.Count; i++)
            {
                StackPanel stackPanel = new StackPanel();
                TextBlock textBlock = new TextBlock();
                TextBox textBox = new TextBox();
                //先设置 stackpanel
                stackPanel.Orientation = Orientation.Horizontal;
                stackPanel.Margin = new Thickness(5);
                stackPanel.VerticalAlignment = VerticalAlignment.Top;
                //再设置 textBlock
                textBlock.Margin = new Thickness(5, 25, 5, 0);
                textBlock.Text = text[i];
                //再设置Textbox
                textBox.BorderThickness = new Thickness(0, 0, 0, 1);
                textBox.VerticalAlignment = VerticalAlignment.Bottom;
                textBox.Width = 600;
                textBox.Height = 25;
                textBox.Margin = new Thickness(0, 5, 0, 0);
                textBox.MouseEnter += C_MouseEnter;
                textBox.MouseLeave += C_MouseLeave;
                textBox.TabIndex = i;
                textBox.Text = value[i];
                stackPanel.Children.Add(textBlock);
                stackPanel.Children.Add(textBox);
                Pram_WarpPanel.Children.Add(stackPanel);
            }

        }

        private void C_MouseLeave(object sender, MouseEventArgs e)
        {
            
        }

        private void C_MouseEnter(object sender, MouseEventArgs e)
        {
            Control uI = sender as Control;
            if (MainWindow.S_TestItems == 1)
            {
                //toolTipsLabel.Text = m_tooltipsStr[uI.TabIndex];
            }
            else
            {
                //toolTipsLabel.Text = m_tooltipsStr2[uI.TabIndex];
            }

        }

        private void loadToIni_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder builder = null;
            if (MainWindow.T_DpmParaValue[0] == "2")
            {
                FileManeger_Popup.IsOpen = false;
                FileManeger_Popup.IsOpen = true;
            }
            else
            {
                //其他情况下，直接将软件界面上的参数导入
                builder = MainWindow.creatbuffer.GetBuffer(MainWindow.T_DpmParaVar, MainWindow.T_DpmParaValue);
                int.TryParse(CreatBuffer.TestBuffer, out int buffernum);
                m_chanel.AppendBuffer(buffernum, builder.ToString());
                m_chanel.CompileBuffer(buffernum);
            }

        }

        private void update_Click(object sender, RoutedEventArgs e)
        {
            Pram_WarpPanel.Children.Clear();
            MainWindow.T_DpmParaValue.Clear();
            MainWindow.T_DpmParaVar.Clear();
            ReadIniVar();
            DynamicAddPram(MainWindow.T_DpmParaVar, MainWindow.T_DpmParaValue);
        }

        private void setup_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(MainWindow.m_ParaFileName);
        }
    }
}
