using DPM_Utility.Base;
using DPM_Utility.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
        private string[] ListButtonName = { "参数设定", "状态检测","软件配置" };
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
            MainContent = new SoftWareSetup();
            
            //默认未连接状态
            //加载列表
            for (int i = 0; i < ListButtonName.Length; i++)
            {
                ListButtonSource.Add(ListButtonName[i]);
            }

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
                    MainContent = new SoftWareSetup();
                    break;
                case 1:
                    MainContent = new DpmParameter();
                    break;
                case 2:
                    MainContent = new MotionDetection();
                    break;
                default:
                    break;
            }
        }


    }
}
