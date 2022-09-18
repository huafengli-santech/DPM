using DPM_Utility.Base;
using LiveCharts.Wpf;
using LiveCharts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Threading;
using System.Windows;
using DPM_Utility.Views;
using System.Windows.Media;
using System.IO;
using System.Security.Cryptography;

namespace DPM_Utility.ViewModels
{
    public class HistoryMonitorViewModel : InotifyBase
    {
        private string[] StandardMean = { "DPM状态", "变量值", "变量绝对值",
                                                                "测量绝对值", "样本当前值", "样本峰值",
                                                                "样本平均值", "样本标准偏差", "样本均方根值", };
        /// <summary>
        /// 数据List
        /// </summary>
        List<string[]> dateList = new List<string[]>();
        /// <summary>
        /// 数据时间list
        /// </summary>
        List<string> dateTimeList = new List<string>();

        private int[] _axis;

        public int[] Axis
        {
            get { return _axis; }
            set { _axis = value; DoNotify(); }
        }

        private string[] _vars;
        public string[] Vars
        {
            get { return _vars; }
            set { _vars = value; DoNotify(); }
        }

        private int _axisIndex;

        public int AxisIndex
        {
            get { return _axisIndex; }
            set { _axisIndex = value; DoNotify(); }
        }

        private int _varsIndex;

        public int VarsIndex
        {
            get { return _varsIndex; }
            set { _varsIndex = value; DoNotify(); }
        }


        private SeriesCollection _seriesCollection;

        public SeriesCollection SeriesCollection
        {
            get { return _seriesCollection; }
            set { _seriesCollection = value; DoNotify(); }
        }

        public ObservableCollection<string> Labels { get; set; }
        private double _trend;
        private double[] temp;

        public IcommandBase RefreshCommand { get; set; }

        public HistoryMonitorViewModel(int[] axis)
        {

            Vars = StandardMean;
            Axis = axis;
            GetLogDate();
            if (axis != null& dateTimeList.Count!=0)
            {
                temp = new double[dateTimeList.Count];
                LineInit();
            }
            else
            {
                MainWindow.show.Show("轴数或历史记录为空，请开启检测后使用", "曲线显示提示", (Brush)new BrushConverter().ConvertFrom("#ffee58"), 10);
            }
            //连接控制器指令
            RefreshCommand = new IcommandBase();
            RefreshCommand.DoExeccute = new Action<object>((o) =>
            {
                if (axis != null & dateTimeList.Count != 0)
                {
                    temp = new double[dateTimeList.Count];
                    LineInit();
                }
                else
                {
                    MainWindow.show.Show("轴数或历史记录为空，请开启检测后使用", "曲线显示提示", (Brush)new BrushConverter().ConvertFrom("#ffee58"), 10);
                }
            });
            RefreshCommand.DoCanExeccute = new Func<object, bool>((o) => true);
        }

        public void LineInit()
        {
            //实例化一条折线图
            LineSeries line1 = new LineSeries();
            //设置折线的标题
            line1.Title = "历史曲线";
            //设置折线的形式
            line1.LineSmoothness = 1;
            //是否显示数值
            line1.DataLabels = true;
            //折线图的无点样式
            line1.PointGeometry = null;

            //添加横坐标
            Labels = new ObservableCollection<string>();
            foreach (var item in temp)
            {
                Labels.Add(item.ToString());
            }
            //添加绘图的数据
            line1.Values = new ChartValues<double>(temp);
            SeriesCollection = new SeriesCollection();
            SeriesCollection.Add(line1);
            _trend = 8;

            GetLogDate();
            //再次之前需要读取保存的参数，判断两个combobox状态来输出
            lineStart();
        }
        public void lineStart()
        {
            Task.Run(() =>
            {
                for (int i = 0; i < dateTimeList.Count; i++)
                {
                    Thread.Sleep(100);
                    int index = (AxisIndex * StandardMean.Length) + VarsIndex;
                    _trend = double.Parse(dateList[i][index]);
                    //通过Dispatcher在工作线程中更新窗体的UI元素
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        //更新横坐标时间
                        Labels.Add(dateTimeList[i]);
                        Labels.RemoveAt(0);
                        //更新纵坐标数据
                        SeriesCollection[0].Values.Add(_trend);
                        SeriesCollection[0].Values.RemoveAt(0);
                    });
                }
            });
        }

        private void GetLogDate()
        {
            dateTimeList.Clear();
            dateList.Clear();
            using (StreamReader reader = new StreamReader(MainWindow.m_LogFileName))
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    string[] ds = line.Split('-');
                    dateTimeList.Add(ds[0]);
                    string[] tDate = ds[1].Trim().Split('\t');
                    dateList.Add(tDate);
                }
            }
        }


    }
}
