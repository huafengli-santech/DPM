using DPM_Utility.Base;
using DPM_Utility.Date;
using LiveCharts.Wpf;
using LiveCharts;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Navigation;

namespace DPM_Utility.ViewModels
{
    public class MotionDetectionVM:InotifyBase
    {
        //隐藏勾选框列表
        public ObservableCollection<CheckInfo> CheckList { get; set; }

        public MotionDetectionVM()
        {
            CheckList = new ObservableCollection<CheckInfo>();

        }

    }
}
