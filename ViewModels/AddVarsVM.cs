using DPM_Utility.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
namespace DPM_Utility.ViewModels
{
    public class AddVarsVM : InotifyBase
    {
        public IcommandBase CloseCommand { get; set; }
        public AddVarsVM()
        {
            CloseCommand = new IcommandBase();
            CloseCommand.DoExeccute = new Action<object>((o) =>
            {
                Window window=o as Window;
                window.Close();
            });
            CloseCommand.DoCanExeccute = new Func<object, bool>((o) => true);
        }
    }
}
