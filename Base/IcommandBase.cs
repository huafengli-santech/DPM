using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DPM_Utility.Base
{
    public class IcommandBase : ICommand
    {
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return DoCanExeccute?.Invoke(parameter) == true;
        }

        public void Execute(object parameter)
        {
            DoExeccute?.Invoke(parameter);
        }

        public Action<object> DoExeccute { get; set; }
        public Func<object, bool> DoCanExeccute { get; set; }
    }
}
