using EasyHook;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Threading.Tasks;

namespace WPELibrary.Lib
{
    public class MainClass : IEntryPoint
    {
        [DllImport("user32.dll")]
        private static extern bool SetProcessDPIAware();

        public MainClass(RemoteHooking.IContext context, string channelName)
        {

        }

        public void Run(RemoteHooking.IContext context, string channelName)
        {
            System.Diagnostics.Debugger.Launch();
            if (Environment.OSVersion.Version.Major >= 6)
            {
                //SetProcessDPIAware();
            }

            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);
            SendPacket();
            //Application.Run(new DLL_Form());

        }

        private WinSockHook ws = new WinSockHook();
        private SocketOperation so = new SocketOperation();
        private int SendBatchCNT = 0;
        private int Send_Success_CNT = 0;
        private int Send_Fail_CNT = 0;

        public void SendPacket()
        {
            try
            {

                //1 | 3412 | 49.234.46.151:7200 | 31 | 23 37 56 72 61 73 59 54 79 6E 5B 73 71 77 5A 50 59 41 75 58 48 4A 4F 4C 72 78 73 3F 5D 6C 21
                int number = 5;
                int times = 5000;
                int count = 1;
                for (int i = 0; i < number; i++)
                {
                    for (int j = 0; j < count; j++)
                    {
                        int socket = 3384;
                        string hexString = "23 37 56 72 61 73 59 54 79 6E 5B 73 71 77 5A 50 59 41 62 67 70 4A 4F 4C 72 78 73 3F 5D 6C 21";
                        string[] hexValues = hexString.Split(' ');
                        byte[] buffer = new byte[hexValues.Length];

                        for (int k = 0; k < hexValues.Length; k++)
                        {
                            buffer[k] = Convert.ToByte(hexValues[k], 16);
                        }

                        int len = buffer.Length;

                        if ((socket > 0) && (buffer.Length != 0))
                        {
                            IntPtr destination = Marshal.AllocHGlobal(buffer.Length);
                            Marshal.Copy(buffer, 0, destination, buffer.Length);
                            if (this.ws.SendPacket(socket, destination, buffer.Length))
                            {
                                this.Send_Success_CNT++;
                            }
                            else
                            {
                                this.Send_Fail_CNT++;
                            }
                            Thread.Sleep(times);
                        }
                    }
                    this.SendBatchCNT++;
                    //int cnt = number - this.SendBatchCNT;
                    //if (cnt > 0)
                    //{
                    //    this.txtSend_CNT.Text = cnt.ToString();
                    //}
                }
            }
            catch
            {
            }
        }
    }
}
