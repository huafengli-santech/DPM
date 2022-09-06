using DPM_Utility.Views;
using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace DPM_Utility.Views
{
    /// <summary>
    /// MotionDetection.xaml 的交互逻辑
    /// </summary>
    public partial class MotionDetection : UserControl
    {
        //曲线变量
        private string[] StandardVar = { "measurement_status", "variable_value", "variable_abs_value",
                                                                "measured_abs_value", "sampled_value", "peak_value",
                                                                "moving_average_value", "std_dev_value", "rms_value", };
        private string[] StandardMean = { "DPM状态", "变量值", "变量绝对值",
                                                                "测量绝对值", "样本当前值", "样本峰值",
                                                                "样本平均值", "样本标准偏差", "样本均方根值", };
        //定义两个中间变量，用于存放结构体名+标准变量
        private string[] TempStandardVar;
        private string[] TempStandardMean;


        //状态变量
        private string[] stateLED = { "during_motion", "during_accel", "during_decel", "during_cv", "on_off", "selected_axis" };
        private string[] stateLedTooltpis = { "运动状态", "加速状态", "减速状态", "匀速状态", "监控状态", "选定轴号" };
        //绘制曲线
        public List<SeriesCollection> seriesCollection { get; set; }
        public List<string> Labels { get; set; }
        public List<string> MonitorVar { get; set; }
        public List<string> MonitorMean { get; set; }
        public List<int> MonitorIndex { get; set; }



        public List<string > UserDefineVar { get; set; }
        public List<string > UserDefineMean { get; set; }
        public List<int > UserDefineIndex { get; set; }


        private double[] temp = { 1, 2, 3, 4, 5, 4, 3, 2 };

        private double [] _trend=new double [10000];
        private double [] _led=new double [10000];

        private int TotalAxis = 0;


        ACSMotionControl m_chanel = new ACSMotionControl();

        public MotionDetection()
        {
            InitializeComponent();
            seriesCollection = new List<SeriesCollection>();
            MonitorVar = new List<string>();
            MonitorMean = new List<string>();
            MonitorIndex = new List<int>();

            UserDefineVar = new List<string>();
            UserDefineMean = new List<string>();
            UserDefineIndex = new List<int>();

            TempStandardVar =new string [StandardVar.Length];
            TempStandardMean =new string [StandardMean.Length];

            SeriesCollection series;


            //添加曲线时不能存在  . 
            //将标准函数里面的变量加进List中
            StandardListInit(StandardVar, StandardMean);

            //先清除
            Lines_WarpPanel.Children.Clear();
            seriesCollection.Clear();
            //根据变量值个数来添加曲线数
            for (int i = 0; i < MonitorVar.Count; i++)
            {
                series = new SeriesCollection();
                seriesCollection.Add(series);
            }

            //动态生成曲线
            DynamicAddBorder(MonitorVar, MonitorMean);

            DynamicAddLed(stateLED);

            //循环加载名称 .
            for (int i = 0; i < StandardVar.Length; i++)
            {
                TempStandardVar[i] = MainWindow.S_StructName + "." + StandardVar[i];
            }

            for (int i = 0; i < stateLED.Length; i++)
            {
                TempStandardMean[i] = MainWindow.S_MotionStructName + "." + stateLED[i];
            }

            linestart(MonitorVar, MonitorIndex);
            Ledstart(stateLED);
        }


        private void DynamicAddBorder(List<string> var, List<string> mean)
        {
            for (int i = 0; i < var.Count; i++)
            {
                Border border = new Border();
                Grid grid = new Grid();
                CartesianChart chart = new CartesianChart();
                border.CornerRadius = new CornerRadius(10);
                border.Background = (Brush)new BrushConverter().ConvertFrom("#e8e8e8");
                border.Width = 298;
                border.Height = 150;
                border.Margin = new Thickness(5);
                //创建行列
                RowDefinition rowDefinition = new RowDefinition();
                RowDefinition rowDefinition1 = new RowDefinition();
                rowDefinition.Height = System.Windows.GridLength.Auto;
                grid.RowDefinitions.Add(rowDefinition);
                grid.RowDefinitions.Add(rowDefinition1);

                //设定中文变量名称************
                TextBlock textBlock = new TextBlock();
                textBlock.Text = mean[i];
                textBlock.Margin = new Thickness(5, 5, 0, 5);
                textBlock.VerticalAlignment = VerticalAlignment.Center;
                textBlock.HorizontalAlignment = HorizontalAlignment.Center;

                TextBlock textBlock1 = new TextBlock();
                textBlock1.Text = "0";
                textBlock1.Name = mean[i];
                textBlock1.Margin = new Thickness(0, 5, 15, 5);
                textBlock1.VerticalAlignment = VerticalAlignment.Center;
                textBlock1.HorizontalAlignment = HorizontalAlignment.Right;
                //设置textblock所处位置

                double[] threshold = new double[8];
                for (int j = 0; j < threshold.Length; j++)
                {
                    threshold[j] = MainWindow.S_Threshold;
                }

                Labels = new List<string> { "0", "0", "0", "0", "0", "0", "0", "0" };

                seriesCollection[i] = new SeriesCollection {
                        new LineSeries
                        {
                            Title = mean[i],
                            LineSmoothness = 1,
                            PointGeometry = null,
                            Values = new ChartValues<double> (temp)
                        },
                        new LineSeries
                        {
                            Title = mean[i]+"阈值",
                            LineSmoothness = 1,
                            PointGeometry = null,
                            Fill=(Brush)new SolidColorBrush(Colors.Transparent),
                            Values = new ChartValues<double> (threshold)
                        }
                    };
                //SeriesCollection[i].Add(mylineseries);

                grid.Children.Add(textBlock);
                grid.Children.Add(textBlock1);
                grid.Children.Add(chart);

                Grid.SetRow(textBlock, 0);
                //Grid.SetColumn(textBlock, 0);
                Grid.SetRow(chart, 1);
                //Grid.SetColumn(chart, 0);
                //绑定数据
                BindingOperations.SetBinding(chart, CartesianChart.SeriesProperty, new Binding { Source = seriesCollection[i] });
                chart.DataContext = this;

                border.Child = grid;
                Lines_WarpPanel.Children.Add(border);
            }
        }

        /// <summary>
        /// 启动折线图函数
        /// </summary>
        public void linestart( List<string> var ,List<int> index)
        {
            //获取当前一共多少个变量
                _trend = new double[var.Count];
            if (var.Count == 0) { return; }
            Task.Run(() =>
            {
                while (true)
                {
                    Thread.Sleep(100);

                     Array.Copy(m_chanel.GetDPMValue(_trend.Length, var, index), _trend, _trend.Length);


                    //通过Dispatcher在工作线程中更新窗体的UI元素
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        //更新横坐标时间
                        Labels.Add(DateTime.Now.ToString());
                        Labels.RemoveAt(0);
                        //更新纵坐标数据
                        for (int i = 0; i < _trend.Length; i++)
                        {
                            seriesCollection[i][0].Values.Add(_trend[i]);
                            PutFault();
                            seriesCollection[i][0].Values.RemoveAt(0);
                        }
                        UpdateValue(Lines_WarpPanel.Children);
                    });
                }
            });
        }
        
        private void PutFault()
        {
            //通过Dispatcher在工作线程中更新窗体的UI元素
            Application.Current.Dispatcher.Invoke(() =>
            {
                        if ((_trend[5]>MainWindow.S_Threshold)&& MainWindow.M_IsFaultHappend)
                    {
                        MainWindow.show.Show($"最大值超过设定值，当前最大值为{String.Format("{0:N3}", _trend[5])}",$"超限报警 {DateTime.Now}", (Brush)new BrushConverter().ConvertFrom("#f50057"),86400);
                         MainWindow.M_IsFaultHappend = false;
                    }
            });
        }

        private void UpdateValue(UIElementCollection uiControls)
        {
            foreach (UIElement element in uiControls)
            {
                if (element is TextBlock)
                {
                    TextBlock current = ((TextBlock)element);
                    for (int i = 0; i < _trend.Length; i++)
                    {
                        if (current.Name == MonitorMean[i])
                        {
                            current.Text = String.Format("{0:N3}", _trend[i]);
                        }
                    }
                }
                else if (element is Grid)
                {
                    this.UpdateValue((element as Grid).Children);
                }
                else if (element is WrapPanel)
                {
                    this.UpdateValue((element as WrapPanel).Children);
                }
                else if (element is Border)
                {
                    if ((element as Border).Child is Grid)
                    {
                        Grid sa = (element as Border).Child as Grid;
                        this.UpdateValue(sa.Children);
                    }

                }

            }
        }

        public void Ledstart(string[] lednamestr)
        {
            _led = new double[lednamestr.Length];
            Task.Run(() =>
            {
                while (true)
                {
                    Thread.Sleep(1000);
                    Array.Copy(m_chanel.GetLedValue(_led.Length, lednamestr), _led, _led.Length);
                    //通过Dispatcher在工作线程中更新窗体的UI元素
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        UpdateLed(States_WarpPanel.Children);
                        Thread.Sleep(100);
                    });
                }
            });
        }

        private void UpdateLed(UIElementCollection uiControls)
        {
            foreach (UIElement element in uiControls)
            {

                 if (element is Border)
                {
                    Border border = ((Border)element);
                    if (_led[(int)border.Tag]==1 && ((MainWindow.S_MotionStructName + "." + border.Name) != stateLED[stateLED.Length - 1]))
                    {
                        border.Background = (Brush)new BrushConverter().ConvertFrom("#b7d332");
                    }
                    else
                    {
                        border.Background = (Brush)new BrushConverter().ConvertFrom("#bec2bc");
                    }
                    if ((element as Border).Child is TextBlock)
                    {
                        TextBlock current = (element as Border).Child as TextBlock;

                            if (current.Name == stateLED[stateLED.Length-1])
                            {
                                current.Text = String.Format("{0:N0}", _led[_led.Length - 1]);
                            }
                    }
                }

            }
        }
        private void Border_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void DynamicAddLed(string [] ledStr)
        {
            for (int i = 0; i <stateLED.Length; i++)
            {
                Border border = new Border();
                border.Background= (Brush)new BrushConverter().ConvertFrom("#bec2bc");
                border.Width = 25;
                border.Height = 25;
                border.CornerRadius = new CornerRadius(12.5);
                border.Margin = new Thickness(5);
                border.Name = stateLED[i];
                border.ToolTip= stateLedTooltpis[i];
                border.Tag = i;

                //设定显示值
                TextBlock textBlock = new TextBlock();  
                textBlock.Text = MainWindow.S_selected_axis.ToString();
                textBlock.HorizontalAlignment = HorizontalAlignment.Center;
                textBlock.VerticalAlignment = VerticalAlignment.Center;
                textBlock.Foreground = (Brush)new BrushConverter().ConvertFrom("#0073bd");
                textBlock.FontWeight = FontWeights.Bold;
                textBlock.Opacity = 0;
                textBlock.Name = stateLED[i];
                if (border.Name== stateLED[stateLED.Length-1])
                {
                    
                    textBlock.Opacity = 1;
                }
                border.Child= textBlock;

                States_WarpPanel.Children.Add(border);
            }

        }


        private void StandardListInit(string [] varstr,string [] meanstr)
        {

            //按照轴数来实现多个轴变量的添加
            TotalAxis = CreatBuffer.AxisCount;

            for (int i = 0; i < TotalAxis; i++)
            {
                string axisstr = CreatBuffer.TestAxis[i];
                for (int j = 0; j < varstr.Length; j++)
                {
                    MonitorVar.Add($"{varstr[i]}_{axisstr}");
                }
                string axismean = CreatBuffer.TestAxis[i];
                for (int j = 0; j < varstr.Length; j++)
                {
                    MonitorMean.Add($"{meanstr[i]}_{axismean}");
                    MonitorIndex.Add(-1);
                }
            }
        }


        private void addButton_Click(object sender, RoutedEventArgs e)
        {
            if (UserDefineMean.Contains(this.cNameTextbox.Text))
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    MainWindow.show.Show("已经添加过该变量，请重新命名", "添加变量失败", (Brush)new BrushConverter().ConvertFrom("#ffee58"), 10);
                });
                return;
            }



            //先清除
            seriesCollection.Clear();
            MonitorVar.Clear();
            MonitorMean.Clear();
            MonitorIndex.Clear();



            UserDefineVar.Add(this.eNameTextbox.Text);
            UserDefineMean.Add(this.cNameTextbox.Text);
            UserDefineIndex.Add(Convert.ToInt32(this.axisorbufferTextBox.Text));
            for (int j = UserDefineVar.Count-1; j >=0; j--)
            {
                MonitorVar.Add(UserDefineVar[j]);
                MonitorMean.Add(UserDefineMean[j]);
                MonitorIndex.Add(UserDefineIndex[j]);
            }
            //将标准变量加入LIST
            StandardListInit(StandardVar, StandardMean);


            SeriesCollection series;

            //先清除
            Lines_WarpPanel.Children.Clear();
            seriesCollection.Clear();
            //根据变量值个数来添加曲线数
            for (int i = 0; i < MonitorVar.Count; i++)
            {
                series = new SeriesCollection();
                seriesCollection.Add(series);
            }

            DynamicAddBorder(MonitorVar, MonitorMean);

            MonitorVar.Clear();
            MonitorMean.Clear();
            MonitorIndex.Clear();



            for (int j = UserDefineVar.Count-1; j >=0 ; j--)
            {
                MonitorVar.Add(UserDefineVar[j]);
                MonitorMean.Add(UserDefineMean[j]);
                MonitorIndex.Add(UserDefineIndex[j]);
            }
            //将结构体加名称加入LIST
            StandardListInit(TempStandardVar, TempStandardMean);
            //循环扫描
            linestart(MonitorVar, MonitorIndex);


        }
    }
}
