using DPM_Utility.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
namespace DPM_Utility.Views
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        //全局变量值
        public static string IP;
        public static int[] S_selected_axis;
        public static int S_selected_buffer;
        public static ShowMessage show = new ShowMessage();
        public static bool M_IsFaultHappend = true;//用于判断是否关闭视窗
        public static CreatBuffer creatbuffer = new CreatBuffer();
        //Paged
        public static string[] S_StructName;
        public static string[] S_MotionStructName;
        public static string S_ThresholdName;
        public static string[] S_Threshold;
        public static int S_TestItems;
        public static double S_TIME;
        public static string S_AllBufferString;//最终上载上来的所有buffer
        public static string S_DBufferString;//最终写入到D-Buffer内的字符串
        public static string S_DpmD_String;//生成的D-Buffer内的字符串
        public static string S_DpmO_String;//生成的其他Buffre内的字符串
        //系统启动项目根目录下   Ini配置文件
        public static string m_IniPath = AppDomain.CurrentDomain.BaseDirectory + "\\IniConfig&Backup";
        public static string m_IniFileName = AppDomain.CurrentDomain.BaseDirectory + "\\IniConfig&Backup\\setup.ini";
        public static string m_ParaFileName = AppDomain.CurrentDomain.BaseDirectory + "\\IniConfig&Backup\\parameters.ini";
        public static string m_BackupFileName = AppDomain.CurrentDomain.BaseDirectory + "\\IniConfig&Backup\\Backup.prg";
        public static string m_LogFileName = AppDomain.CurrentDomain.BaseDirectory + "\\IniConfig&Backup\\Log.log";
        public static string m_LogFileStartName = AppDomain.CurrentDomain.BaseDirectory + "\\IniConfig&Backup\\Log.log";
        public static INI iniFile = new INI(m_IniFileName);
        public static INI paramFile = new INI(m_ParaFileName);
        public static List<string> T_DpmParaVar;
        public static List<string> T_DpmParaValue;
        public static List<string> T_DpmMeanNames;
        //ACS初始化
        ACSMotionControl m_com = new ACSMotionControl();
        MainWindowVM model = new MainWindowVM();
        public MainWindow()
        {
            InitializeComponent();
            //绑定ViewModel
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
                m_com.Connect(IP, 701);
                MainWindow.show.Show("连接成功", "连接提示", (Brush)new BrushConverter().ConvertFrom("#ffee58"), 5);
                model.IsConnected=true;
            }
            catch
            {
                try
                {
                    m_com.Simconnect();
                    MainWindow.show.Show("连接失败，已自动连接仿真", "连接提示", (Brush)new BrushConverter().ConvertFrom("#ffee58"), 5);
                }
                catch
                {
                    MainWindow.show.Show("仿真连接仍然失败，连接失败", "连接提示", (Brush)new BrushConverter().ConvertFrom("#a1ffce"), 5);
                }
            }
        }
        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
    }
}
