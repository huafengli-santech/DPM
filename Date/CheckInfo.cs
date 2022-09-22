using DPM_Utility.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace DPM_Utility.Date
{
    public class CheckInfo:InotifyBase
    {
        private string _checkName;
        public string CheckName
        {
            get { return _checkName; }
            set { _checkName = value; DoNotify(); }
        }
        private Boolean _isCheck;
        public Boolean IsChecked
        {
            get { return _isCheck; }
            set { _isCheck = value; DoNotify(); }
        }
        public CheckInfo()
        {
        }
    }
}
