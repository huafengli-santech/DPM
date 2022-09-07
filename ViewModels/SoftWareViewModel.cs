using DPM_Utility.Base;
using DPM_Utility.Date;
using DPM_Utility.UserControls;
using DPM_Utility.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DPM_Utility.ViewModels
{
    public class SoftWareViewModel:InotifyBase 
    {
		ACSMotionControl acs = new ACSMotionControl();

		public IcommandBase ConnectCommand { get; set; }
		public IcommandBase DisconnectCommand { get; set; }
		public IcommandBase AddNewVarCommand { get; set; }
		public IcommandBase RemoveVarCommand { get; set; }
		public IcommandBase VarHelpCommand { get; set; }
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


		public SoftWareViewModel()
		{
			//初始化显示为默认参数
			IP = acs.GetGetEthernetCardsIP();
			Port = 701;
			//初始化列表测试
			VarInfos = new ObservableCollection<VarInfo>();


			//连接控制器指令
            ConnectCommand = new IcommandBase();
			ConnectCommand.DoExeccute=new Action<object>((o)=>
			{
				acs.Connect(o.ToString(),Port);
                //MainWindowViewModel.ConnectLed = Brushes.Green;
            });
			ConnectCommand.DoCanExeccute=new Func<object, bool>((o)=>true);

			//断开控制器指令
            DisconnectCommand = new IcommandBase();
            DisconnectCommand.DoExeccute = new Action<object>((o)=>
			{
				acs.CloseCom();
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
        }

	}
}
