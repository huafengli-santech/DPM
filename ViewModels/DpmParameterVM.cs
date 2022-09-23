using DPM_Utility.Base;
using DPM_Utility.Date;
using DPM_Utility.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
namespace DPM_Utility.ViewModels
{
    public class DpmParameterVM : InotifyBase
    {
        //ACS初始化
        ACSMotionControl m_com = new ACSMotionControl();
        private Boolean _isOpen;
        private Boolean _isShow;
        private string _notice;
        public Boolean IsOpen
        {
            get { return _isOpen; }
            set { _isOpen = value; DoNotify(); }
        }
        public Boolean IsShow
        {
            get { return _isShow; }
            set { _isShow = value; DoNotify(); }
        }
        public string Notice
        {
            get { return _notice; }
            set { _notice = value; DoNotify(); }
        }
        public ObservableCollection<DpmParaInfo> ParaInfoList { get; set; }
        public IcommandBase OpenIniCommand { get; set; }
        public IcommandBase UpdateCommand { get; set; }
        public IcommandBase ResetCommand { get; set; }
        public IcommandBase SaveCommand { get; set; }
        public DpmParameterVM()
        {
            //初始化参数列表信息
            ParaInfoList = new ObservableCollection<DpmParaInfo>();
            //判断是否需要显示配置参数按钮
            ParaIniInit();
            //实现打开ini文件
            OpenIniCommand = new IcommandBase(new Action<object>((o) => OpenIniFile()), new Func<object, bool>((o) => true));
            UpdateCommand = new IcommandBase(new Action<object>((o) => ReadIniVar()), new Func<object, bool>((o) => true));
            ResetCommand = new IcommandBase(new Action<object>((o) => OpenIniFile()), new Func<object, bool>((o) => true));
            SaveCommand = new IcommandBase(new Action<object>((o) => SaveFunc()), new Func<object, bool>((o) => true));
        }
        private void SaveFunc()
        {
            List<string> varnamelist = new List<string>();
            List<string> varvaluelist = new List<string>();
            for (int i = 0; i < ParaInfoList.Count; i++)
            {
                varnamelist.Add(ParaInfoList[i].ParaName);
                varvaluelist.Add(ParaInfoList[i].ParaValues);
            }
            //打开窗体
            if (varvaluelist[0] == "2")
            {
                IsOpen = false;
                IsOpen = true;
            }
            else
            {
                //其他情况下，直接将软件界面上的参数导入
                StringBuilder[] builders = null;
                builders = MainWindow.creatbuffer.GetBuffer(varnamelist, varvaluelist);
                //将数据添加进D-BUFFER中
                MainWindow.S_DpmD_String = builders[0].ToString();
                //将数据添加进指定Buffer中
                MainWindow.S_DpmO_String = builders[1].ToString();
                m_com.UploadBuffer();
            }
        }
        private void OpenIniFile()
        {
            System.Diagnostics.Process.Start(MainWindow.m_ParaFileName);
            ReadIniVar();
        }
        private void ParaIniInit()
        {
            if (File.Exists(MainWindow.m_ParaFileName)) { IsShow = false; ReadIniVar(); return; }
            Notice = "单击此处配置DPM参数";
            IsShow = true;
            if (!File.Exists(MainWindow.m_ParaFileName))
            {
                //File.Create(MainWindow.m_ParaFileName);
                MainWindow.paramFile.IniWriteValue("参数举例（具体参数请在  Para  组中添加添加）", "  ***参数中间用空格分隔***   ", "变量不能增加，其他变量请在状态检测界面添加");
                MainWindow.paramFile.IniWriteValue("Parameter", "检测类型", "0/1/2(0：PE检测 1：峰值电流检测 2：自定义变量检测，三者取其一，不填写默认0)");
                MainWindow.paramFile.IniWriteValue("Parameter", "采样轴号", "0 1(轴列表，最少填写一个，最大到控制器轴数限制)");
                MainWindow.paramFile.IniWriteValue("Parameter", "测量类型", "0 0(0：加速段；1：匀速段；2：加减速段；3：任意轨迹，需要与轴个数一一对应，不填写默认0)");
                MainWindow.paramFile.IniWriteValue("Parameter", "采样阈值", "0.001 0.002(需要与轴个数一一对应，不填写默认0)");
                MainWindow.paramFile.IniWriteValue("Parameter", "Buffer号", "0(存放代码的buffer号，最大到控制器buffer限制)");
                MainWindow.paramFile.IniWriteValue("Parameter", "峰值电流", "40 20(可选参数，选择峰值电流检测时使用，轴所在从站电流最大值，需要与轴个数一一对应，不填写默认0)");
                MainWindow.paramFile.IniWriteValue("Parameter", "模拟量输入分辨率", "12 16(可选参数，选择峰值电流检测时使用，需要与轴个数一一对应（bit位），不填写默认0)");
                MainWindow.paramFile.IniWriteValue("Para", "检测项目", "");
                MainWindow.paramFile.IniWriteValue("Para", "采样轴号", "");
                MainWindow.paramFile.IniWriteValue("Para", "检测阶段", "");
                MainWindow.paramFile.IniWriteValue("Para", "采样阈值", "");
                MainWindow.paramFile.IniWriteValue("Para", "Buffer号", "");
                MainWindow.paramFile.IniWriteValue("Para", "峰值电流", "");
                MainWindow.paramFile.IniWriteValue("Para", "模拟量输入分辨率", "");
                ReadIniVar();
            }
        }
        private void ReadIniVar()
        {
            IsShow = false;
            ParaInfoList.Clear();
            List<string> TempParaVar = new List<string>();
            //获取para下的所有keys
            TempParaVar = MainWindow.paramFile.ReadKeys("para", MainWindow.m_ParaFileName);
            //将Ini文件导入到list
            for (int i = 0; i < TempParaVar.Count; i++)
            {
                ParaInfoList.Add(new DpmParaInfo()
                {
                    ParaName = TempParaVar[i],
                    ParaValues = MainWindow.paramFile.IniReadValue("para", TempParaVar[i])
                });
            }
        }
    }
}
