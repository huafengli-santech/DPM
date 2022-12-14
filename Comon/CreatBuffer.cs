using DPM_Utility.Views;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;
namespace DPM_Utility
{
    public class CreatBuffer
    {
        //ACS初始化
        ACSMotionControl m_chanel = new ACSMotionControl();
        public static int AxisCount { get; set; }
        //默认属性
        public static string TestItem { get; set; }
        public static string []TestAxis { get; set; }
        public static string []TestType { get; set; }
        public static string []TestThreshold { get; set; }
        public static string TestBuffer { get; set; }
        public static string [] DriveCurrent { get; set; }
        public static string [] AnalogRes { get; set; }
        //自定义属性名&实现方式
        public static string UserVar { get; set; }
        public static string UertVarCode { get; set; }
        StringBuilder SB = new StringBuilder();
        public StringBuilder[] GetBuffer(List<string> var,List<string> value)
        {
            SB.Clear();
            for (int i = 0; i < var.Count; i++)
            {
                switch (var[i])
                {
                    case "检测项目":
                        TestItemFunc(value[i]);
                        break;
                    case "采样轴号":
                        TestAxisFunc(value[i]);
                        break;
                    case "检测阶段":
                        TestTypeFunc(value[i]);
                        break;
                    case "采样阈值":
                        TestThresholdFunc(value[i]);
                        break;
                    case "Buffer号":
                        TestBufferFunc(value[i]);
                        break;
                    case "峰值电流":
                        DriveCurrentFunc(value[i]);
                        break;
                    case "模拟量输入分辨率":
                        AnalogResFunc(value[i]);
                        break;
                    default:
                        break;
                }
            }
            return DynamicCreatbuffer();
        }
        public StringBuilder[] GetBuffer(List<string> var)
        {
            SB.Clear();
            for (int i = 0; i < var.Count; i++)
            {
                //先将字符串分割成字符串
                string[] vars = var[i].Split('\n');
                //将数据分为变量名+值
                string[] varname = new string[vars.Length];
                string[] varvalue = new string[vars.Length];
                for (int j = 0; j < vars.Length; j++)
                {
                    string[] varstr = vars[j].Split('：');
                    varname[j] = varstr[0];
                    varvalue[j] = varstr[1];
                    switch (varname[j])
                    {
                        case "Buffer号":
                            TestBufferFunc(varvalue[j]);
                            break;
                        case "轴号":
                            TestAxisFunc(varvalue[j]);
                            break;
                        case "检测变量":
                            TestItemFunc(varvalue[j]);
                            break;
                        case "检测类型":
                            TestTypeFunc(varvalue[j]);
                            break;
                        case "采样阈值":
                            TestThresholdFunc(varvalue[j]);
                            break;
                        case "峰值电流":
                            DriveCurrentFunc(varvalue[j]);
                            break;
                        case "模拟量输入分辨率":
                            AnalogResFunc(varvalue[j]);
                            break;
                        default:
                            break;
                    }
                }
                
            }
            return DynamicCreatbuffer();
        }
        private StringBuilder[] DynamicCreatbuffer()
        {
            StringBuilder[] builders = new StringBuilder[2];
            MainWindow.S_StructName = new string[TestAxis.Length];
            MainWindow.S_MotionStructName = new string[TestAxis.Length];
            //******************************************************************
            //D-BUFFER里面的程序部分
            StringBuilder D_s =new StringBuilder();
            string ss = "";//用于中转
            //DPM结构体名称
            for (int i = 0; i < TestAxis.Length; i++)
            {
                //将结构体名称保存至MainWindow，用于判断是否需要重复添加
                MainWindow.S_StructName[i] = $"{TestItem}_{TestAxis[i]}";
                ss += $"{TestItem}_{TestAxis[i]},";
            }
            ss = ss.Remove(ss.LastIndexOf(','), 1);
            D_s.Append($"GLOBAL STATIC DPM_Measurement {ss}\n");
            ss = "";
            //motion结构体名称
            if (!TestType.Contains("3"))
            {
                for (int i = 0; i < TestAxis.Length; i++)
                {
                    MainWindow.S_MotionStructName[i]= $"Motion_status_{TestAxis[i]}";
                    ss += $"Motion_status_{TestAxis[i]},";
                }
                ss = ss.Remove(ss.LastIndexOf(','), 1);
            }
            D_s.Append($"GLOBAL STATIC DPM_Motion_Status {ss}\n");
            ss = "";
            for (int i = 0; i < TestAxis.Length; i++)
            {
                ss += $"{TestItem}_{TestAxis[i]}_Threshold,";
            }
            ss = ss.Remove(ss.LastIndexOf(','), 1);
            D_s.Append($"GLOBAL STATIC REAL {ss}\n");
            ss = "";
            //定义一些隐藏参数
            D_s.Append($"GLOBAL  INT Sample_set_size\n");
            D_s.Append($"GLOBAL STATIC INT measure_continuously\n");
            D_s.Append($"\r\n");
            //将D-BUFFER保存到第一个参数内
            builders[0] = new StringBuilder();
            builders[0].Append(D_s.ToString());
            D_s.Clear();
            //******************************************************************
            //其他buffer里面的字符串
            StringBuilder O_s = new StringBuilder();
            
            for (int i = 0; i < TestAxis.Length; i++)
            {
                ss += $"Axis{TestAxis[i]}={TestAxis[i]},";
            }
            ss=ss.Remove(ss.LastIndexOf(','),1);
            O_s.Append($"GLOBAL INT {ss}\n");
            ss = "";
            for (int i = 0; i < TestAxis.Length; i++)
            {
                ss += $"{TestItem}_{TestAxis[i]}_Threshold={TestThreshold[i]};";
            }
            O_s.Append($"{ss}\n");
            ss = "";
            //定义一些隐藏参数
            O_s.Append($"measure_continuously = 1\n");
            O_s.Append($"Sample_set_size = 20\n");
            //如果是峰值电流模式，需要将驱动器峰值电流+分辨率设置
            if (TestItem == "PeckCurrent")
            {
                for (int i = 0; i < TestAxis.Length; i++)
                {
                    ss += $"DRIVE_PEAK_CURRENT_AMP_{TestAxis[i]} ={DriveCurrent[i]},";
                }
                ss = ss.Remove(ss.LastIndexOf(','), 1);
                O_s.Append($"GLOBAL INT {ss}\n");
                ss = "";
                for (int i = 0; i < TestAxis.Length; i++)
                {
                    ss += $" ADC_RANGE_{TestAxis[i]} ={DriveCurrent[i]},";
                }
                ss = ss.Remove(ss.LastIndexOf(','), 1);
                O_s.Append($"GLOBAL INT {ss}\n");
                ss = "";
            }
            //开启检测
            O_s.Append($"\n!Start Measurement\n");
            ss = "";//用于中转
            //先将之前的全部停止掉
            for (int i = 0; i < TestAxis.Length; i++)
            {
                ss += $"{TestItem}_{TestAxis[i]}.Stop();";
            }
            O_s.Append($"{ss}\n");
            ss = "";
            //motion结构体名称
            if (!TestType.Contains("3"))
            {
                for (int i = 0; i < TestAxis.Length; i++)
                {
                    ss += $"Motion_status_{TestAxis[i]}.SelectAxis({TestAxis[i]});";
                }
                O_s.Append($"{ss}\n");
                ss = "";
                for (int i = 0; i < TestAxis.Length; i++)
                {
                    ss += $"Motion_status_{TestAxis[i]}.MonitorOn();";
                }
                O_s.Append($"{ss}\n");
                ss = "";
            }
            O_s.Append($"\n!Measurement activation\n");
            if (!TestType.Contains("3"))
            {
                for (int i = 0; i < TestAxis.Length; i++)
                {
                    ss += $"{TestItem}_{TestAxis[i]}.MeasureProcess(PE(Axis{TestAxis[i]}),Motion_status_{TestAxis[i]}.{TestType[i]},Sample_set_size, measure_continuously);\n";
                }
                O_s.Append($"{ss}\n");
                ss = "";
            }
            else
            {
                for (int i = 0; i < TestAxis.Length; i++)
                {
                    ss += $"{TestItem}_{TestAxis[i]}.MeasurePeriodically(PE(Axis{TestAxis[i]}),Motion_status_{TestAxis[i]}.{TestType[i]},Sample_set_size, measure_continuously);\n";
                }
                O_s.Append($"{ss}\n");
                ss = "";
            }
            O_s.Append($"\nSTOP\n");
            D_s.Append(O_s+"\r\n");
            //将其余buffer的内容拷贝到index1内
            builders[1] = new StringBuilder();
            builders[1].Append(D_s.ToString());
            return builders;
        }
        #region 属性
        private void TestItemFunc(string v)
        {
            if (!string.IsNullOrEmpty(v)) TestItem = v.Trim(); else { TestItem = "0"; }
                switch (TestItem)
                {
                    case "0":
                        TestItem = "PE";
                        break;
                    case "1":
                        TestItem = "PeckCurrent";
                        break;
                    case "2":
                        TestItem = UserVar;
                        break;
                }
        }
        private void TestAxisFunc(string v)
        {
            
            if (!string.IsNullOrEmpty(v))
            { 
                TestAxis = v.Trim().Split(' ');
                AxisCount=TestAxis.Length;
                MainWindow.S_selected_axis = new int[AxisCount];
                for (int i = 0; i < TestAxis.Length; i++)
                {
                    for (int j = i+1; j < TestAxis.Length; j++)
                    {
                        if (TestAxis[j]== TestAxis[i])
                        {
                            System.Windows.Application.Current.Dispatcher.Invoke(() =>
                            {
                                MainWindow.show.Show("轴号重复，请重新配置", "ini配置错误", (Brush)new BrushConverter().ConvertFrom("#ffee58"), 10);
                            });
                        }
                    }
                    MainWindow.S_selected_axis[i] = int.Parse(TestAxis[i]);
                }
            }
        }
        private void TestTypeFunc(string v)
        {
            if (!string.IsNullOrEmpty(v)) { TestType = v.Trim().Split(' '); }
            else
            {
                TestType = new string[AxisCount];
                for (int i = 0; i < AxisCount; i++)
                {
                    TestType[i] = "0";
                }
            }
            for (int i = 0; i < AxisCount; i++)
            {
                switch (TestType[i])
                {
                    case "0":
                        TestType[i] = "during_accel";
                        break;
                    case "1":
                        TestType[i] = "during_cv";
                        break;
                    case "2":
                        TestType[i] = "during_cv | during_cv";
                        break;
                    case "3":
                        TestType[i] = "Measure_ON_OFF";
                        break;
                }
            }
        }
        private void TestThresholdFunc(string v)
        {
            if (!string.IsNullOrEmpty(v)) { TestThreshold = v.Trim().Split(' '); }
            else
            {
                TestThreshold = new string[AxisCount];
                for (int i = 0; i < AxisCount; i++)
                {
                    TestThreshold[i] = "0";
                }
            }
            MainWindow.S_Threshold =TestThreshold;
        }
        private void TestBufferFunc(string v)
        {
            if(!string.IsNullOrEmpty(v)) TestBuffer = v.Trim(); else { TestBuffer = "0"; }
            //测试程序保存位置
            MainWindow.S_selected_buffer = int.Parse(TestBuffer);
        }
        private void DriveCurrentFunc(string v)
        {
            if (TestItem == "0") { return; }//如果测试项不是峰值电流检测，则返回
            if (!string.IsNullOrEmpty(v)) { DriveCurrent = v.Trim().Split(' '); }
            else
            {
                DriveCurrent = new string[AxisCount];
                for (int i = 0; i < AxisCount; i++)
                {
                    DriveCurrent[i] = "0";
                }
                
            }
        }
        private void AnalogResFunc(string v)
        {
            if (TestItem == "0") { return; }//如果测试项不是峰值电流检测，则返回
            if (!string.IsNullOrEmpty(v)) { AnalogRes = v.Trim().Split(' '); }
            else 
            { 
                AnalogRes = new string[AxisCount];
                for (int i = 0; i < AxisCount; i++)
                {
                    AnalogRes[i] = "0";
                }
            }
        }
        #endregion
    }
}
