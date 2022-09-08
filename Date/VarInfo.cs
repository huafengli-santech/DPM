using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPM_Utility.Date
{
    public class VarInfo
    {
        public int VarNum { get { return 6; } }
        public object Axis { get; set; }
        public object DetecVars { get; set; }
        public object Type { get; set; }
        public object Threshold { get; set; }
        public object Maxcurrent { get; set; }
        public object AnalogResolution { get; set; }

    }
}
