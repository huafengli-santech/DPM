using DPM_Utility.Base;
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
        private string[] ListButtonName = { "参数配置", "状态检测" };

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

        public MainWindowViewModel()
        {
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
                default:
                    break;
            }
        }


    }
}
