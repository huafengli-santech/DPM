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
        public StringBuilder GetBuffer(List<string> var,List<string> value)
        {
            SB.Clear();
            for (int i = 0; i < var.Count; i++)
            {
                switch (var[i])
                {
                    case "测试项目":
                        TestItemFunc(value[i]);
                        break;
                    case "采样轴号":
                        TestAxisFunc(value[i]);
                        break;
                    case "测量类型":
                        TestTypeFunc(value[i]);
                        break;
                    case "采样阈值":
                        TestThresholdFunc(value[i]);
                        break;
                    case "存放buffer号":
                        TestBufferFunc(value[i]);
                        break;
                    case "驱动峰值电流":
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
        public StringBuilder GetBuffer(List<string> var)
        {
            SB.Clear();
            string[] varname = new string[var.Count];
            string[] varvalue = new string[var.Count];
            for (int i = 0; i < var.Count; i++)
            {
                string[] vars = var[i].Split('：');
                varname[i] = vars[0];
                varvalue[i] = vars[1];
            }
            for (int i = 0; i < var.Count; i++)
            {
                switch (var[i])
                {
                    //case "测试项目":
                    //    TestItemFunc(value[i]);
                    //    break;
                    //case "采样轴号":
                    //    TestAxisFunc(value[i]);
                    //    break;
                    //case "测量类型":
                    //    TestTypeFunc(value[i]);
                    //    break;
                    //case "采样阈值":
                    //    TestThresholdFunc(value[i]);
                    //    break;
                    //case "存放buffer号":
                    //    TestBufferFunc(value[i]);
                    //    break;
                    //case "驱动峰值电流":
                    //    DriveCurrentFunc(value[i]);
                    //    break;
                    //case "模拟量输入分辨率":
                    //    AnalogResFunc(value[i]);
                    //    break;
                    //default:
                    //    break;
                }
            }
            return DynamicCreatbuffer();
        }
        private StringBuilder DynamicCreatbuffer()
        {
            //D-BUFFER里面的程序部分
            StringBuilder D_s=new StringBuilder();
            string ss = "";//用于中转
            //DPM结构体名称
            for (int i = 0; i < TestAxis.Length; i++)
            {
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
            D_s.Append($"GLOBAL  INT Sample_set_size_Y\n");
            D_s.Append($"GLOBAL STATIC INT measure_continuously\n");

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
                    ss += $"{TestItem}_{TestAxis[i]}.MeasureProcess(PE({TestAxis[i]}),Motion_status_{TestAxis[i]}.{TestType[i]},Sample_set_size, measure_continuously);\n";
                }
                O_s.Append($"{ss}\n");
                ss = "";
            }
            else
            {
                for (int i = 0; i < TestAxis.Length; i++)
                {
                    ss += $"{TestItem}_{TestAxis[i]}.MeasurePeriodically(PE({TestAxis[i]}),Motion_status_{TestAxis[i]}.{TestType[i]},Sample_set_size, measure_continuously);\n";
                }
                O_s.Append($"{ss}\n");
                ss = "";
            }
            O_s.Append($"\nSTOP\n");
            D_s.Append(O_s+"\r\n");
            //test
            //System.Windows.MessageBox.Show(D_s.ToString());
            return D_s;
        }
        #region 属性
        private void TestItemFunc(string v)
        {
            if (!string.IsNullOrEmpty(v)) TestItem = v; else { TestItem = "0"; }
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
                TestAxis = v.Split(' ');
                AxisCount=TestAxis.Length;
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
                }
            }

        }

        private void TestTypeFunc(string v)
        {
            if (!string.IsNullOrEmpty(v)) { TestType = v.Split(' '); }
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
            if (!string.IsNullOrEmpty(v)) { TestThreshold = v.Split(' '); }
            else
            {
                TestThreshold = new string[AxisCount];
                for (int i = 0; i < AxisCount; i++)
                {
                    TestThreshold[i] = "0";
                }
            }
        }

        private void TestBufferFunc(string v)
        {
            if(!string.IsNullOrEmpty(v)) TestBuffer = v; else { TestBuffer = "0"; }
        }

        private void DriveCurrentFunc(string v)
        {
            if (TestItem == "0") { return; }//如果测试项不是峰值电流检测，则返回
            if (!string.IsNullOrEmpty(v)) { DriveCurrent = v.Split(' '); }
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
            if (!string.IsNullOrEmpty(v)) { AnalogRes = v.Split(' '); }
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
        private void UploadBuffer()
        {
            //需要将程序上载，并保存在TXT中防止软件崩溃

            for (int i = 0; i < m_chanel.GetTotalBuffers(); i++)
            {
                if (!string.IsNullOrEmpty(m_chanel.GetBufferString(i)))
                {
                    MainWindow.S_AllBufferString += $"#{i}\n{m_chanel.GetBufferString(i)}";
                }
            }
            if (!string.IsNullOrEmpty(MainWindow.S_PageD_String))
            {
                if (!m_chanel.GetBufferString(m_chanel.GetTotalBuffers()).Contains(MainWindow.S_StructName))
                {
                    MainWindow.S_DBufferString = $"#A\n{m_chanel.GetBufferString(m_chanel.GetTotalBuffers())}\n!DPM Test\n{MainWindow.S_PageD_String}";
                }
            }
            else
            {
                System.Windows.MessageBox.Show("请按照步骤操作", "参数设定提示");
            }
            SaveBuffer();
        }

        private void SaveBuffer()
        {
            StreamWriter writer = new StreamWriter(MainWindow.m_BackupFileName);
            writer.WriteLine(MainWindow.S_AllBufferString);
            writer.WriteLine(MainWindow.S_DBufferString);
            //将设定的参数值写入到指定的buffer
            writer.WriteLine();
            //刷新缓存
            writer.Flush();
            //关闭流
            writer.Close();
            //导入数据
            IfAdd(MainWindow.S_selected_buffer, MainWindow.S_PageA_String);
        }

        private void IfAdd(int buffernum, string s)
        {

            //倒回buffer
            WriteBuffer();
            //再清除响应的BUFFER
            if (m_chanel.GetBufferLines(buffernum) != 0)
            {
                DialogResult result = (DialogResult)System.Windows.MessageBox.Show($"所选择的Buffer存在{m_chanel.GetBufferLines(buffernum)}行程序，是否清除后导入？", "提示", MessageBoxButton.OKCancel);

                if (result == DialogResult.OK)
                {
                    //delete
                    m_chanel.ClearBuffer(buffernum);
                    m_chanel.AppendBuffer(buffernum, s);
                    m_chanel.CompileBuffer(buffernum);
                }
            }
            else
            {
                m_chanel.AppendBuffer(buffernum, s);
                m_chanel.CompileBuffer(buffernum);
            }
            System.Windows.MessageBox.Show("写入成功", "程序导入");
        }

        private void WriteBuffer()
        {
            m_chanel.LoadFormFile(MainWindow.m_BackupFileName);
            m_chanel.CompileBuffer(m_chanel.GetTotalBuffers());
            for (int i = 0; i < m_chanel.GetTotalBuffers(); i++)
            {
                m_chanel.CompileBuffer(i);
            }
        }

    }
}
