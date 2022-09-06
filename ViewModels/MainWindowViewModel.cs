using DPM_Utility.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DPM_Utility.ViewModels
{
    internal class MainWindowViewModel:InotifyBase
    {
        public ObservableCollection<string> ListButtonSource { get; set; } = new ObservableCollection<string>();
        private string[] ListButtonName = { "参数设定", "状态检测","软件配置" };
        private string[] ViewsList = { "DpmParameter","MotionDectection",""};
        public MainWindowViewModel()
        {
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
                
        }


    }
}
