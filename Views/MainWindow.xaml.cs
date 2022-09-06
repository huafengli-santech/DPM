using DPM_Utility.ViewModels;
using DPM_Utility.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace DPM_Utility.Views
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        //左侧的按钮列表---可以动态生成
        string[] leftButtonList = {"参数设定","状态检测" };
        string[] leftButtonList_N = { "setPramButton", "decStateButton" };

        //全局变量值
        public static string IP;
        public static int S_selected_axis;
        public static int S_selected_buffer;

        public static ShowMessage show = new ShowMessage();
        public static  bool M_IsFaultHappend = true;//用于判断是否关闭视窗

        public static CreatBuffer creatbuffer = new CreatBuffer();

        //Paged
        public static string S_StructName;
        public static string S_MotionStructName;
        public static string S_ThresholdName;
        public static double S_Threshold;
        public static int S_TestItems;
        public static double S_TIME;

        public static string S_AllBufferString;
        public static string S_DBufferString;
        public static string S_PageD_String;
        public static string S_PageA_String;
        public static string S_PageB_String;

        //获取系统启动文件目录
        static string m_Path = AppDomain.CurrentDomain.BaseDirectory;
        
        //系统启动项目根目录下   Ini配置文件
        public static string m_IniPath = AppDomain.CurrentDomain.BaseDirectory + "\\IniConfig&Backup";
        public static string m_IniFileName = AppDomain.CurrentDomain.BaseDirectory + "\\IniConfig&Backup\\setup.ini";
        public static string m_ParaFileName = AppDomain.CurrentDomain.BaseDirectory + "\\IniConfig&Backup\\parameters.ini";
        public static string m_BackupFileName = AppDomain.CurrentDomain.BaseDirectory + "\\IniConfig&Backup\\Backup.prg";
        public static INI iniFile = new INI(m_IniFileName);
        public static INI paramFile = new INI(m_ParaFileName);

        public static List<string> T_DpmParaVar;
        public static List<string> T_DpmParaValue;

        MotionDetection motion = new MotionDetection();

        //ACS初始化
        ACSMotionControl m_chanel=new ACSMotionControl();

        bool IsInitFinsh=false;
        MainWindowViewModel model = new MainWindowViewModel();
        public MainWindow()
        {
            InitializeComponent();

            this.DataContext = model;

            T_DpmParaVar = new List<string>();
            T_DpmParaValue = new List<string>();

            //最大化时窗体不会覆盖状态栏
            this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
            this.MaxWidth = SystemParameters.MaximizedPrimaryScreenWidth;

            //创建存放INI 文件的文件夹
            if (!Directory.Exists(m_IniPath))
            {
                Directory.CreateDirectory(m_IniPath);
            }

            //先获取ip
            IniInit();
            // 创建备份文件
            if (!File.Exists(m_BackupFileName))
            {
                FileStream fs = new FileStream(m_BackupFileName, FileMode.CreateNew);
            }

            IsInitFinsh =true;
            //动态创建左侧列表
            DynamicAddLeftButtonList();

            //测试初始化
            ConInit();

        }


        private void IniInit()
        {
            if (!File.Exists(m_IniFileName))
            {
                iniFile.IniWriteValue("SystemParameter", "IP", "10.0.0.100");
            }
            IP = iniFile.IniReadValue("SystemParameter", "IP");
        }

        private void ConInit()
        {
            try
            {
                //m_chanel.Simconnect();
                m_chanel.Connect(IP, 701);
                MainWindow.show.Show("Connect success","连接提示",(Brush)new BrushConverter().ConvertFrom("#a1ffce"),5);
            }
            catch
            {

            }
        }



        //private void btnNav_Click(object sender, RoutedEventArgs e)
        //{
        //    RadioButton btn = sender as RadioButton;
        //    this.frmMain.Navigate(new Uri(btn.Tag.ToString() + ".xaml", UriKind.Relative));
        //}
        private RadioButtonName radioname;
        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (!IsInitFinsh) { return; }
            //this.dynamicWrapPanel.Children.Clear();
            RadioButton btn = sender as RadioButton;
            if (btn.Content.ToString() == "参数设定")
            {
                this.frmMain.Navigate(new Uri("DpmParameter.xaml", UriKind.Relative));

            }
            else if (btn.Content.ToString() == "状态检测")
            {
                this.frmMain.Navigate(new Uri("MotionDetection.xaml", UriKind.Relative));
            }

            UpdateLayout();
        }

        private void DynamicAddLeftButtonList()
        {
            //for (int i = 0; i < leftButtonList.Length; i++)
            //{
            //    RadioButton radio = new RadioButton();
            //    radio.DataContext = radioname;
            //    radio.Name = leftButtonList_N[i];
            //    radioname = new RadioButtonName();
            //    radioname.Name = leftButtonList[i];
            //    //绑定数据
            //    BindingOperations.SetBinding(radio, RadioButton.ContentProperty, new Binding { Source = radioname, Path = new PropertyPath("Name"), Mode = BindingMode.OneWay });
            //    //设置样式
            //    Style myStyle = (Style)this.FindResource("CheckRadioButtonStyle");
            //    radio.Style = myStyle;
            //    //设置左右间隔
            //    radio.Margin = new Thickness(10,0,0,0);
            //    radio.Height = 35;
            //    radio.Checked += RadioButton_Checked;
            //    if (radio.Content.ToString()== leftButtonList[0])
            //    {
            //        radio.IsChecked = false;
            //        radio.IsChecked = true;
            //    }
            //    this.leftButtonListPanel.Children.Add(radio);
            //}
        }
        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
        private void minFormButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState=this.WindowState==WindowState.Minimized?WindowState.Normal:WindowState.Minimized;
        }

        private void maxFormButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = this.WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
        }

        private void closeFormButton_Click(object sender, RoutedEventArgs e)
        {
            m_chanel.CloseCom();
            this.Close();
        }
    }
    public class RadioButtonName : INotifyPropertyChanged
    {
        private string _name;
        public string Name
        {
            set
            {
                _name = value;
                if (PropertyChanged != null)//有改变
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Name"));//对Name进行监听
                }
            }
            get
            {
                return _name;
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
