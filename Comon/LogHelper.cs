using DPM_Utility.Views;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPM_Utility.Comon
{
    public class LogHelper
    {
        private string newPath { get; set; }
        private string oldPath { get; set; }
        public void SetVarsNameLogFunc(string path,List<string> varsMean)
        {
            try
            {
                newPath = $"VarCount{varsMean.Count}";
                if (!path.Contains(newPath))
                {
                    MainWindow.m_LogFileName = "";
                    MainWindow.m_LogFileName = $"{MainWindow.m_LogFileStartName}{newPath}";
                    //此处的true表示如果存在，则会在后面追加
                    using (StreamWriter sw = new StreamWriter(MainWindow.m_LogFileName, true))
                    {
                        string date = "";
                        for (int i = 0; i < varsMean.Count; i++)
                        {
                            date += $"{varsMean[i]}\t";
                        }
                        sw.WriteLine($"参数列表-{date}");
                    }
                }
            }
            catch 
            {
            }

        }

    }
}
