﻿using DPM_Utility.Base;
using DPM_Utility.Date;
using DPM_Utility.UserControls;
using DPM_Utility.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DPM_Utility.ViewModels
{
    public  class SoftWareViewModel:InotifyBase 
    {
		ACSMotionControl acs = new ACSMotionControl();


		public IcommandBase ConnectCommand { get; set; }
		public IcommandBase DisconnectCommand { get; set; }
		public IcommandBase AddNewVarCommand { get; set; }
		public IcommandBase RemoveVarCommand { get; set; }
		public IcommandBase VarHelpCommand { get; set; }
		public IcommandBase LoadToFileCommand { get; set; }
        public ObservableCollection<VarInfo> VarInfos { get; set; }
        //IP配置
        private string[] _ip;

		public string[] IP
		{
			get { return _ip; }
			set { _ip = value; DoNotify(); }
		}

		private int _port;

		public int Port
		{
			get { return _port; }
			set { _port = value; DoNotify(); }
		}
		//连接状态
        private Brush _connnectled;

        public Brush ConnectLed
        {
            get { return _connnectled; }
            set { _connnectled = value; DoNotify(); }
        }


        public SoftWareViewModel()
		{
			//初始化显示为默认参数
			IP = acs.GetGetEthernetCardsIP();
			Port = 701;

			ConnectLed = Brushes.Gray;

			//初始化列表测试
			VarInfos = new ObservableCollection<VarInfo>();
            //添加测试
            VarInfos.Add(new VarInfo() { Axis=10086,Type=3});


            //连接控制器指令
            ConnectCommand = new IcommandBase();
			ConnectCommand.DoExeccute=new Action<object>((o)=>
			{
				if (o!=null)
				{
                    acs.Connect(o.ToString(), Port);
                    ConnectLed = Brushes.LightGreen;
                }
				else
				{
                    MainWindow.show.Show("IP或Port为空", "ACS连接错误", (Brush)new BrushConverter().ConvertFrom("#f50057"), 5);
                }
            });
			ConnectCommand.DoCanExeccute=new Func<object, bool>((o)=>true);

			//断开控制器指令
            DisconnectCommand = new IcommandBase();
            DisconnectCommand.DoExeccute = new Action<object>((o)=>
			{
				acs.CloseCom();
                ConnectLed = Brushes.Gray;
            });
            DisconnectCommand.DoCanExeccute = new Func<object, bool>((o) => true);

			//往表格中添加数据指令
            AddNewVarCommand = new IcommandBase();
            AddNewVarCommand.DoExeccute = new Action<object>((o)=>
			{
                //添加测试
                VarInfos.Add(new VarInfo() { });
            });
            AddNewVarCommand.DoCanExeccute = new Func<object, bool>((o) => true);

			//表格选中项删除指令
            RemoveVarCommand = new IcommandBase();
            RemoveVarCommand.DoExeccute = new Action<object>((o) =>
            {
                DataGrid grid = (DataGrid)o;
                //没有选中状态
                if (grid.SelectedIndex != -1)
                {
                    VarInfos.RemoveAt(grid.SelectedIndex);
                }
            });
            RemoveVarCommand.DoCanExeccute = new Func<object, bool>((o) => true);

			//展示帮助指令
            VarHelpCommand = new IcommandBase();
            VarHelpCommand.DoExeccute = new Action<object>((o)=>
			{
				AddVarsHelpWindow add = new AddVarsHelpWindow();
				add.ShowDialog();
			});
            VarHelpCommand.DoCanExeccute = new Func<object, bool>((o) => true);


            //下发表格中的数据到文件
            LoadToFileCommand = new IcommandBase();
            LoadToFileCommand.DoExeccute = new Action<object>((o)=>
			{
                //读取表格数据
                ReadDataGridSource(o);
			});
            LoadToFileCommand.DoCanExeccute = new Func<object, bool>((o) => true);
        }

        private void ReadDataGridSource( object obj)
        {

            for (int i = 0; i < VarInfos.Count; i++)
            {
                //datagrid中需要将EnableRowVirtualization属性设置为Fasle,不然仅一行时会全部为Null
                VarInfo info = VarInfos[i] as VarInfo;
                if (IsPropNull(info))
                {
                    string keys = $"轴号：{info.Axis}";
                    MainWindow.T_DpmParaVar.Add(keys);
                    keys = $"检测变量：{info.DetecVars}";
                    MainWindow.T_DpmParaVar.Add(keys);
                    keys = $"检测类型：{info.Type}";
                    MainWindow.T_DpmParaVar.Add(keys);
                    keys = $"阈值：{info.Threshold}";
                    MainWindow.T_DpmParaVar.Add(keys);
                    keys = $"峰值电流：{info.Maxcurrent}";
                    MainWindow.T_DpmParaVar.Add(keys);
                    keys = $"模拟量输入分辨率：{info.AnalogResolution}";
                    MainWindow.T_DpmParaVar.Add(keys);
                    MainWindow.show.Show("数据生成中，请稍后", "生成提示", (Brush)new BrushConverter().ConvertFrom("#a1ffce"), 5);
                }
                else
                {
                    MainWindow.show.Show("表格内数据为空，请全部填写后尝试", "生成提示", (Brush)new BrushConverter().ConvertFrom("#ffee58"), 5);
                }

            }
        }

        /// <summary>
        /// 判断一个对象所有字段是否为空
        /// </summary>
        /// <param name="obj">要判断的任意对象</param>
        /// <returns>如果有值则为True，如果无值则为False</returns>
        public static bool IsPropNull(object obj)
        {
            Type t = obj.GetType();//拿到对象类型
            PropertyInfo[] props = t.GetProperties();//拿到属性数组

            List<string> list = new List<string>();
            foreach (var item in props)
            {
                if (item.GetValue(obj)==null)//判断值是否不为空
                {
                    list.Add(item.Name + ":" + item.GetValue(obj));
                }
            }
            return list.Count == 0 ? true : false;//返回对象是否为空数据
        }
    }
}