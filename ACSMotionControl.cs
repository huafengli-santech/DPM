using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using ACS.SPiiPlusNET;

namespace DPM_Utility
{
    internal class ACSMotionControl
    {
        private static Api m_com = new Api();
        public static string m_sXuliehao;
        public static int m_TotalAxes;
        public static int m_TotalBuffers;

        private bool ACS_Connected;

        private bool states_dec;
        private bool led_dec;
        public ACSMotionControl()
        {

        }
        public void Connect(string ip, int port)
        {
            if (!m_com.IsConnected)
            {
                m_com.OpenCommEthernetTCP(ip, port);
                ACS_Connected = true;
            }
        }
        public void Simconnect()
        {
            if (!m_com.IsConnected)
            {
                m_com.OpenCommSimulator();
                ACS_Connected = true;
            }
        }
        public bool ConnectStateNow()
        {
            return m_com.IsConnected;
        }
        public int GetTotalAxes()
        {

            m_TotalAxes = (int)m_com.GetAxesCount();
            return m_TotalAxes;
        }

        public int GetTotalBuffers()
        {

            m_TotalBuffers = (int)m_com.GetBuffersCount();
            return m_TotalBuffers;
        }
        public void CloseCom()
        {
            m_com.CloseComm();
            ACS_Connected = false;
        }

        public double [] GetDPMValue(int arraySize,List<string> var, List<int> index)
        {
            double[] p = new double[arraySize];
            try
            {
                    for (int i = 0; i < var.Count; i++)
                    {
                    if (index[i]==-1)
                    {
                        p[i] = Convert.ToDouble(m_com.ReadVariable(var[i]));
                    }
                    else
                    {
                        p[i] = Convert.ToDouble(m_com.ReadVariable(var[i],ProgramBuffer.ACSC_NONE,index[i],index[i]));
                    }
                }
            }
            catch
            {
                if (!states_dec)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        MainWindow.show.Show("曲线所需变量不存在，停止查询，默认为0", "ACS查询错误", (Brush)new BrushConverter().ConvertFrom("#ffee58"),5);
                    });
                }
                states_dec = true;
            }
            return p;
        }
        public int[] GetLedValue(int arraySize, string[] varnames)
        {
            int[] p = new int[arraySize];
            try
            {
                for (int i = 0; i < varnames.Length; i++)
                {
                    p[i] = (int)m_com.ReadVariable(varnames[i]);
                }
            }
            catch (ACSException)
            {
                if (!led_dec)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        MainWindow.show.Show("LED变量不存在，停止查询，默认为0", "ACS查询错误", (Brush)new BrushConverter().ConvertFrom("#ffee58"),5);
                    });
                }
                led_dec = true;
            }


            return p;
        }

        public string GetBufferString(int buffernum)
        {
            string reply = "";
            reply = m_com.UploadBuffer((ProgramBuffer)buffernum);
            if (!string.IsNullOrEmpty(reply))
            {
                reply = reply.Replace("\r\n\r\n", "\n");
            }
            return reply;
        }

        public void LoadFormFile(string filename)
        {
            m_com.LoadBuffersFromFile(filename);
        }

        public void CompileBuffer(int buffernum)
        {
            try
            {
                m_com.CompileBuffer((ProgramBuffer)buffernum);
            }
            catch 
            {

            }
        }

        public int GetBufferLines(int buffernum)
        {
            int lines = 0;
            lines = (int)m_com.ReadVariable("PLINES", ProgramBuffer.ACSC_NONE,buffernum,buffernum);
            return lines;
        }

        public void ClearBuffer(int buffernum)
        {
            m_com.ClearBuffer((ProgramBuffer)buffernum,1, GetBufferLines(buffernum));
        }
        public void AppendBuffer(int buffernum,string code)
        {
            m_com.AppendBuffer((ProgramBuffer)buffernum, code);
        }
    }
}
