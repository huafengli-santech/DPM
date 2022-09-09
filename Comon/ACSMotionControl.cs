﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;
using ACS.SPiiPlusNET;
using DPM_Utility.Views;

namespace DPM_Utility
{
    internal class ACSMotionControl
    {
        private static Api m_com = new Api();
        public static int m_TotalAxes;
        public static int m_TotalBuffers;

        private bool ACS_Connected=false;

        private bool states_dec;
        private bool led_dec;
        public ACSMotionControl()
        {

        }

        public string[] GetGetEthernetCardsIP()
        {
            // 获取所有IP
            IPAddress[] ipAddresses = m_com.GetEthernetCards(IPAddress.Broadcast);
            string[] address = new string[ipAddresses.Length];
            for (int index = 0; index < ipAddresses.Length; index++)
            {
                address[index] = ipAddresses[index].ToString();
            }
            return address;
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
                    System.Windows.Application.Current.Dispatcher.Invoke(() =>
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
                    System.Windows.Application.Current.Dispatcher.Invoke(() =>
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


        private void UploadBuffer()
        {
            //需要将程序上载，并保存在TXT中防止软件崩溃

            for (int i = 0; i < GetTotalBuffers(); i++)
            {
                if (!string.IsNullOrEmpty(GetBufferString(i)))
                {
                    MainWindow.S_AllBufferString += $"#{i}\n{GetBufferString(i)}";
                }
            }
            if (!string.IsNullOrEmpty(MainWindow.S_PageD_String))
            {
                if (!GetBufferString(GetTotalBuffers()).Contains(MainWindow.S_StructName))
                {
                    MainWindow.S_DBufferString = $"#A\n{GetBufferString(GetTotalBuffers())}\n!DPM Test\n{MainWindow.S_PageD_String}";
                }
            }
            else
            {
                System.Windows.MessageBox.Show("请按照步骤操作", "参数设定提示");
            }
            SaveBufferToFile();
        }

        private void SaveBufferToFile()
        {
            StreamWriter writer = new StreamWriter(MainWindow.m_BackupFileName);
            writer.WriteLine(MainWindow.S_AllBufferString);
            writer.WriteLine(MainWindow.S_DBufferString);
            //将设定的参数值写入到指定的buffer
            writer.WriteLine();
            //刷新缓存
            writer.Flush();
            //关闭流
            writer.Close();
            //导入数据
            IfAdd(MainWindow.S_selected_buffer, MainWindow.S_PageA_String);
        }

        private void IfAdd(int buffernum, string s)
        {

            //倒回buffer
            WriteBuffer();
            //再清除响应的BUFFER
            if (GetBufferLines(buffernum) != 0)
            {
                DialogResult result = (DialogResult)System.Windows.MessageBox.Show($"所选择的Buffer存在{GetBufferLines(buffernum)}行程序，是否清除后导入？", "提示", MessageBoxButton.OKCancel);

                if (result == DialogResult.OK)
                {
                    //delete
                    ClearBuffer(buffernum);
                    AppendBuffer(buffernum, s);
                    CompileBuffer(buffernum);
                }
            }
            else
            {
                AppendBuffer(buffernum, s);
                CompileBuffer(buffernum);
            }
            System.Windows.MessageBox.Show("写入成功", "程序导入");
        }

        private void WriteBuffer()
        {
            //LoadFormFile(MainWindow.m_BackupFileName);
            //m_com.CompileBuffer(GetTotalBuffers());
            //for (int i = 0; i < GetTotalBuffers(); i++)
            //{
            //    m_com.CompileBuffer(i);
            //}
        }
    }
}
