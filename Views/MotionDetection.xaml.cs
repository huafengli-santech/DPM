using DPM_Utility.TypeConvert;
using DPM_Utility.ViewModels;
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
        #region 变量定义
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
        //曲线的LIST
        public List<string> MonitorVar { get; set; }
        public List<string> MonitorMean { get; set; }
        public List<int> MonitorIndex { get; set; }
        //
        public List<string> LedVar { get; set; }
        public List<string> LedMean { get; set; }
        public List<int> LedIndex { get; set; }
        public List<string> UserDefineVar { get; set; }
        public List<string> UserDefineMean { get; set; }
        public List<int> UserDefineIndex { get; set; }
        private double[] temp = { 1, 2, 3, 4, 5, 4, 3, 2 };
        private double[] _trend = new double[10000];
        private double[] _led = new double[10000];
        private int TotalAxis = 0;
        #endregion
        ACSMotionControl m_com = new ACSMotionControl();
        MotionDetectionVM viewModel = new MotionDetectionVM();
        public MotionDetection()
        {
            InitializeComponent();
            this.DataContext = viewModel;
            seriesCollection = new List<SeriesCollection>();
            MonitorVar = new List<string>();
            MonitorMean = new List<string>();
            MonitorIndex = new List<int>();
            LedVar = new List<string>();
            LedMean = new List<string>();
            LedIndex = new List<int>();
            UserDefineVar = new List<string>();
            UserDefineMean = new List<string>();
            UserDefineIndex = new List<int>();
            TempStandardVar = new string[StandardVar.Length];
            TempStandardMean = new string[stateLED.Length];
        }
        /// <summary>
        /// 输出异常函数
        /// </summary>
        private void PutFault()
        {
            //通过Dispatcher在工作线程中更新窗体的UI元素
            Application.Current.Dispatcher.Invoke(() =>
            {
                for (int j = 0; j < MainWindow.S_Threshold.Length; j++)
                {
                    for (int i = 0; i < MonitorVar.Count; i++)
                    {
                        if (MonitorMean[i].Contains("样本峰值"))
                        {
                            if ((_trend[i] > double.Parse(MainWindow.S_Threshold[j])) && MainWindow.M_IsFaultHappend)
                            {
                                MainWindow.show.Show($"{StandardMean[i]}超过设定值，当前最大值为{String.Format("{0:F6}", _trend[i])}", $"超限报警 {DateTime.Now}", (Brush)new BrushConverter().ConvertFrom("#f50057"), 86400);
                                MainWindow.M_IsFaultHappend = false;
                            }
                        }
                    }
                }
            });
        }
        /// <summary>
        /// 遍历所有的控件函数
        /// </summary>
        /// <param name="uiControls">UI控件的母体容器</param>
        private void UpdateValue(UIElementCollection uiControls)
        {
            foreach (UIElement element in uiControls)
            {
                if (element is TextBlock)
                {
                    TextBlock current = ((TextBlock)element);
                    for (int i = 0; i < MonitorVar.Count; i++)
                    {
                        if (current.Name == MonitorMean[i])
                        {
                            current.Text = String.Format("{0:F6}", _trend[i]);
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
        private void Border_Loaded(object sender, RoutedEventArgs e)
        {
            SeriesCollection series;
            //添加曲线时不能存在  . 
            //将标准函数里面的变量加进List中
            LinesListInit(StandardVar, StandardMean);
            LedListInit(stateLED, stateLedTooltpis);
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
            DynamicAddLines(MonitorVar, MonitorMean);
            //动态生成Led
            DynamicAddLed(LedVar, LedMean);
            if (MainWindow.S_StructName != null || MainWindow.S_MotionStructName != null)
            {
                //循环加载名称 .
                for (int i = 0; i < MainWindow.S_StructName.Length; i++)
                {
                    for (int j = 0; j < StandardVar.Length; j++)
                    {
                        TempStandardVar[j] = MainWindow.S_StructName[i] + "." + StandardVar[j];
                    }
                }
                for (int i = 0; i < MainWindow.S_MotionStructName.Length; i++)
                {
                    for (int j = 0; j < stateLED.Length; j++)
                    {
                        TempStandardMean[j] = MainWindow.S_MotionStructName[i] + "." + stateLED[j];
                    }
                }
                linestart(MonitorVar, MonitorIndex, MonitorMean);
                Ledstart(LedVar, LedIndex);
            }
            else
            {
                MainWindow.show.Show("请在主页面刷新，并保存参数后进行检测", "曲线生成错误提示", (Brush)new BrushConverter().ConvertFrom("#ffee58"), 10);
            }
        }
        #region 曲线添加、刷新函数
        /// <summary>
        /// Lines List 根据轴号添加值
        /// </summary>
        /// <param name="linesVarStr">lines变量名称</param>
        /// <param name="linesMeanStr">lines对应显示名称</param>
        private void LinesListInit(string[] linesVarStr, string[] linesMeanStr)
        {
            //按照轴数来实现多个轴变量的添加
            TotalAxis = CreatBuffer.AxisCount;
            for (int i = 0; i < TotalAxis; i++)
            {
                string axisstr = CreatBuffer.TestAxis[i];
                for (int j = 0; j < linesVarStr.Length; j++)
                {
                    MonitorVar.Add($"{MainWindow.S_StructName[i]}.{linesVarStr[j]}");
                }
                string axismean = CreatBuffer.TestAxis[i];
                for (int j = 0; j < linesVarStr.Length; j++)
                {
                    MonitorMean.Add($"轴_{axismean}_{linesMeanStr[j]}");
                    MonitorIndex.Add(-1);
                }
            }
        }
        /// <summary>
        /// 动态生成曲线
        /// </summary>
        /// <param name="var">曲线检测变量名称</param>
        /// <param name="mean">曲线显示名称</param>
        private void DynamicAddLines(List<string> var, List<string> mean)
        {
            viewModel.CheckList.Clear();
            for (int i = 0; i < var.Count; i++)
            {
                //更新勾选项
                viewModel.CheckList.Add(new Date.CheckInfo() { CheckName = mean[i], IsChecked = true }) ;
                //更新曲线
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
                rowDefinition.Height = GridLength.Auto;
                grid.RowDefinitions.Add(rowDefinition);
                grid.RowDefinitions.Add(rowDefinition1);
                //设定中文变量名称************
                TextBlock textBlock = new TextBlock();
                textBlock.Text = GetTypeName(mean[i]);
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
                //设定阈值
                double[] threshold = new double[8];
                for (int m = 0; m < MainWindow.S_Threshold.Length; m++)
                {
                    for (int j = 0; j < threshold.Length; j++)
                    {
                        threshold[j] = double.Parse(MainWindow.S_Threshold[m]);
                    }
                }
                Labels = new List<string> { "0", "0", "0", "0", "0", "0", "0", "0" };
                if (var[i].Contains("peak_value"))
                {
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
                }
                else
                {
                    seriesCollection[i] = new SeriesCollection {
                        new LineSeries
                        {
                            Title = mean[i],
                            LineSmoothness = 1,
                            PointGeometry = null,
                            Values = new ChartValues<double> (temp)
                        }
                    };
                }
                grid.Children.Add(textBlock);
                grid.Children.Add(textBlock1);
                grid.Children.Add(chart);
                Grid.SetRow(textBlock, 0);
                Grid.SetRow(chart, 1);
                BooleanToVisibilityConverter booltoVisability = new BooleanToVisibilityConverter();
                //绑定数据-绑定是否显示
                BindingOperations.SetBinding(border, VisibilityProperty, new Binding { Source = viewModel.CheckList[i],Path=new PropertyPath("IsChecked"),Converter= booltoVisability });
                //绑定数据-绑定曲线
                BindingOperations.SetBinding(chart, CartesianChart.SeriesProperty, new Binding { Source = seriesCollection[i] });
                chart.DataContext = this;
                border.Child = grid;
                Lines_WarpPanel.Children.Add(border);
            }
        }
        /// <summary>
        /// 将曲线名称中包含变量的字符串转换为用户自定义变量名称值
        /// </summary>
        /// <param name="meanName">曲线名称字符串</param>
        /// <returns></returns>
        private string GetTypeName(string meanName)
        {
            string typeName = "";
            if (meanName != null && meanName.Contains("变量"))
                typeName = meanName.Replace("变量", CreatBuffer.TestItem);
            else
                typeName = meanName;
            return typeName;
        }
        /// <summary>
        /// 更新曲线数值函数
        /// </summary>
        /// <param name="var">曲线变量名称</param>
        /// <param name="index">曲线对应index，自定义变量时需要使用到[buffer号、轴号]</param>
        public void linestart(List<string> var, List<int> index, List<string> varsMean)
        {
            //获取当前一共多少个变量
            _trend = new double[var.Count];
            _trend = m_com.GetDPMValue(_trend.Length, var, index, varsMean);
            if (var.Count == 0) { return; }
            Task.Run(() =>
            {
                while (true)
                {
                    Thread.Sleep(100);
                    //Array.Copy(m_com.GetDPMValue(_trend.Length, var, index, varsMean), _trend, _trend.Length);
                    //通过Dispatcher在工作线程中更新窗体的UI元素
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        //更新横坐标时间
                        Labels.Add(DateTime.Now.ToString());
                        Labels.RemoveAt(0);
                        //更新纵坐标数据
                        for (int i = 0; i < MonitorVar.Count; i++)
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
        #endregion
        #region Led添加、刷新函数
        /// <summary>
        /// Led List 根据轴号添加值
        /// </summary>
        /// <param name="ledVarStr">led变量名称</param>
        /// <param name="meanStr">led对应显示名称</param>
        private void LedListInit(string[] ledVarStr, string[] meanStr)
        {
            //按照轴数来实现多个轴变量的添加
            TotalAxis = CreatBuffer.AxisCount;
            for (int i = 0; i < TotalAxis; i++)
            {
                for (int j = 0; j < ledVarStr.Length; j++)
                {
                    LedVar.Add($"{MainWindow.S_MotionStructName[i]}.{ledVarStr[j]}");
                }
                string axismean = CreatBuffer.TestAxis[i];
                for (int j = 0; j < ledVarStr.Length; j++)
                {
                    LedMean.Add($"{meanStr[j]}_{axismean}");
                    LedIndex.Add(-1);
                }
            }
        }
        public void Ledstart(List<string> ledList, List<int> index)
        {
            _led = new double[ledList.Count];
            Task.Run(() =>
            {
                while (true)
                {
                    Thread.Sleep(1000);
                    Array.Copy(m_com.GetLedValue(_led.Length, ledList, index), _led, _led.Length);
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
                    if (_led[(int)border.Tag] == 1 && ((MainWindow.S_MotionStructName + "." + border.Name) != stateLED[stateLED.Length - 1]))
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
                        if (current.Name == stateLED[stateLED.Length - 1])
                        {
                            current.Text = String.Format("{0:N0}", _led[_led.Length - 1]);
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 动态生成LED
        /// </summary>
        /// <param name="var">LED变量名称</param>
        /// <param name="meanStr">LED中文对应名称</param>
        private void DynamicAddLed(List<string> var, List<string> meanStr)
        {
            for (int i = 0; i < var.Count; i++)
            {
                Border border = new Border();
                border.Background = (Brush)new BrushConverter().ConvertFrom("#bec2bc");
                border.Width = 25;
                border.Height = 25;
                border.CornerRadius = new CornerRadius(12.5);
                border.Margin = new Thickness(5);
                border.Name = meanStr[i];
                border.ToolTip = meanStr[i];
                border.Tag = i;
                //设定显示值
                int axis = 0;
                double index = (double)i / stateLED.Length - 1 > 0 ? axis++ : 0;
                TextBlock textBlock = new TextBlock();
                textBlock.Text = MainWindow.S_selected_axis[axis].ToString();
                textBlock.HorizontalAlignment = HorizontalAlignment.Center;
                textBlock.VerticalAlignment = VerticalAlignment.Center;
                textBlock.Foreground = (Brush)new BrushConverter().ConvertFrom("#0073bd");
                textBlock.FontWeight = FontWeights.Bold;
                textBlock.Opacity = 0;
                textBlock.Name = meanStr[i];
                if (border.Name.Contains("选定轴号"))
                {
                    textBlock.Opacity = 1;
                }
                border.Child = textBlock;
                States_WarpPanel.Children.Add(border);
            }
        }
        #endregion
        /// <summary>
        /// 添加新变量按键事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
            for (int j = UserDefineVar.Count - 1; j >= 0; j--)
            {
                MonitorVar.Add(UserDefineVar[j]);
                MonitorMean.Add(UserDefineMean[j]);
                MonitorIndex.Add(UserDefineIndex[j]);
            }
            //将标准变量加入LIST
            LinesListInit(StandardVar, StandardMean);
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
            DynamicAddLines(MonitorVar, MonitorMean);
            MonitorVar.Clear();
            MonitorMean.Clear();
            MonitorIndex.Clear();
            for (int j = UserDefineVar.Count - 1; j >= 0; j--)
            {
                MonitorVar.Add(UserDefineVar[j]);
                MonitorMean.Add(UserDefineMean[j]);
                MonitorIndex.Add(UserDefineIndex[j]);
            }
            //将结构体加名称加入LIST
            LinesListInit(StandardVar, StandardMean);
            //循环扫描
            linestart(MonitorVar, MonitorIndex, MonitorMean);
        }
    }
}
