using DPM_Utility.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace DPM_Utility
{
    public  class ShowMessage
    {
        //红色    #f50057
        //绿色    #a1ffce
        //黄色    #ffee58
        public static List<NotificationWindow> _dialogs = new List<NotificationWindow>();
        public void Show(string code,string Title , System.Windows.Media.Brush color,int time)
        {
            NotifyData data = new NotifyData();
            data.Title = Title;
            data.Content = code;
            data.Color = color;
            data.Time = time;
            NotificationWindow dialog = new NotificationWindow();//new 一个通知
            dialog.Closed += Dialog_Closed;
            dialog.TopFrom = GetTopFrom();
            _dialogs.Add(dialog);
            dialog.DataContext = data;//设置通知里要显示的数据
            dialog.Show();
        }
        private void Dialog_Closed(object sender, EventArgs e)
        {
            var closedDialog = sender as NotificationWindow;
            _dialogs.Remove(closedDialog);
        }
        double GetTopFrom()
        {
            //屏幕的高度-底部TaskBar的高度。
            double topFrom = System.Windows.SystemParameters.WorkArea.Bottom - 10;
            bool isContinueFind = _dialogs.Any(o => o.TopFrom == topFrom);
            while (isContinueFind)
            {
                topFrom = topFrom - 110;//此处100是NotifyWindow的高 110-100剩下的10  是通知之间的间距
                isContinueFind = _dialogs.Any(o => o.TopFrom == topFrom);
            }
            if (topFrom <= 0)
                topFrom = System.Windows.SystemParameters.WorkArea.Bottom - 10;
            return topFrom;
        }
    }
}
