using DPM_Utility.Base;
using DPM_Utility.Date;
using DPM_Utility.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace DPM_Utility.ViewModels
{
    public class MainWindowViewModel:InotifyBase
    {
        public ObservableCollection<string> ListButtonSource { get; set; } = new ObservableCollection<string>();
        private string[] ListButtonName = { "参数配置", "状态检测","历史曲线" };

        //关闭、最小、最大按钮
        public IcommandBase MaxFormCommand { get; set; }
        public IcommandBase MinFormCommand { get; set; }
        public IcommandBase CloseFormCommand { get; set; }

        //右侧控件
        private FrameworkElement  _mainContent;

        public FrameworkElement MainContent
        {
            get { return _mainContent; }
            set { _mainContent = value; DoNotify(); }
        }

        private bool _isconnected;

        public bool IsConnected
        {
            get { return _isconnected; }
            set { _isconnected = value;DoNotify(); }
        }


        public MainWindowViewModel()
        {
            //UI线程未捕获异常处理事件
            Application.Current.DispatcherUnhandledException += OnDispatcherUnhandledException;
            //Task线程内未捕获异常处理事件
            TaskScheduler.UnobservedTaskException += OnUnobservedTaskException;
            //多线程异常
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;

            //默认窗体就是软件设置界面窗体
            MainContent = new DpmParameter();
            //默认未连接状态

            //加载列表
            for (int i = 0; i < ListButtonName.Length; i++)
            {
                ListButtonSource.Add(ListButtonName[i]);
            }
            //关闭窗体
            CloseFormCommand = new IcommandBase();
            CloseFormCommand.DoExeccute = new Action<object>((o) =>
            {
                Window window = (Window)o;
                window.Close();
            });
            CloseFormCommand.DoCanExeccute = new Func<object, bool>((o) => true);
            //最小化窗体
            MinFormCommand = new IcommandBase();
            MinFormCommand.DoExeccute = new Action<object>((o) =>
            {
                Window window = (Window)o;
                window.WindowState = window.WindowState == WindowState.Minimized ? WindowState.Normal : WindowState.Minimized;
            });
            MinFormCommand.DoCanExeccute = new Func<object, bool>((o) => true);
            //最大化窗体
            MaxFormCommand = new IcommandBase();
            MaxFormCommand.DoExeccute = new Action<object>((o) =>
            {
                Window window = (Window)o;
                window.WindowState = window.WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
            });
            MaxFormCommand.DoCanExeccute = new Func<object, bool>((o) => true);


        }


        #region 报错订阅
        private void OnDispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            //通常全局异常捕捉的都是致命信息
            MessageBox.Show($"{e.Exception.StackTrace},{e.Exception.Message}", "UI线程异常捕获");
            //MainWindow.show.Show($"{e.Exception.StackTrace},{e.Exception.Message}", "全局异常捕获", (Brush)new BrushConverter().ConvertFrom("#f50057"), 5);
        }

        private void OnUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            MessageBox.Show($"{e.Exception.StackTrace},{e.Exception.Message}", "Task异常捕获");
            //MainWindow.show.Show($"{e.Exception.StackTrace},{e.Exception.Message}", "全局异常捕获", (Brush)new BrushConverter().ConvertFrom("#f50057"), 5);
        }

        private void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = e.ExceptionObject as Exception;
            MessageBox.Show($"{ex.StackTrace},{ex.Message}", "多线程异常捕获");

            //MainWindow.show.Show($"{ex.StackTrace},{ex.Message}", "全局异常捕获", (Brush)new BrushConverter().ConvertFrom("#f50057"), 5);
        }
        #endregion
        private int _selectedIndex;

        public int SelectedIndex
        {
            get { return _selectedIndex; }
            set { 
                if (_selectedIndex == value)
                {
                    return;
                }
                ShowDialog(value);
                // At this point _selectedIndex is the old selected item's index

                _selectedIndex = value;

                // At this point _selectedIndex is the new selected item's index

                DoNotify(nameof(SelectedIndex));
            }
        }

        private void ShowDialog(int index)
        {
            switch (index)
            {
                case 0:
                    MainContent = new DpmParameter();
                    break;
                case 1:
                    MainContent = new MotionDetection();
                    break;
                case 2:
                    MainContent = new HistoryMonitor();
                    break;
                default:
                    break;
            }
        }


    }
}
