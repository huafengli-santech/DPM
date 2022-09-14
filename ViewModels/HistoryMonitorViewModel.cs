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

namespace DPM_Utility.ViewModels
{
    public class HistoryMonitorViewModel : InotifyBase
    {
        private string[] StandardMean = { "DPM状态", "变量值", "变量绝对值",
                                                                "测量绝对值", "样本当前值", "样本峰值",
                                                                "样本平均值", "样本标准偏差", "样本均方根值", };

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
            set { _axisIndex = value; }
        }


        private SeriesCollection _seriesCollection;

        public SeriesCollection SeriesCollection
        {
            get { return _seriesCollection; }
            set { _seriesCollection = value; }
        }


        public List<string> Labels { get; set; }
        private double _trend;
        private double[] temp = { 1, 3, 2, 4, 3, 6, 3, 2, 2, 4, 7, 4, 2, -3, 5, 2, 1, 7 };

        public HistoryMonitorViewModel(int[] axis)
        {
            Vars = StandardMean;
            Axis = axis;
            LineInit();
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
            line1.DataLabels = false;
            //折线图的无点样式
            line1.PointGeometry = null;
            
            //添加横坐标
            Labels = new List<string>();
            foreach (var item in temp)
            {
                Labels.Add(item.ToString());
            }
            //添加绘图的数据
            line1.Values = new ChartValues<double>(temp);
            SeriesCollection = new SeriesCollection();
            SeriesCollection.Add(line1);
            _trend = 8;
            //再次之前需要读取保存的参数，判断两个combobox状态来输出
            lineStart();
        }
        public void lineStart()
        {
            Task.Run(() =>
            {
                Random r = new Random();
                while (true)
                {
                    Thread.Sleep(100);
                    _trend = r.Next(-10, 10);
                    //通过Dispatcher在工作线程中更新窗体的UI元素
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        //更新横坐标时间
                        Labels.Add(DateTime.Now.ToString());
                        Labels.RemoveAt(0);
                        //更新纵坐标数据
                        SeriesCollection[0].Values.Add(_trend);
                        SeriesCollection[0].Values.RemoveAt(0);
                    });
                }
            });
        }


    }
}
