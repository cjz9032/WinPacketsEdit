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
using System.Net.Sockets;


using System;
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;


namespace WPELibrary.Lib
{


    public class CodeModifier
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, uint nSize, out int lpNumberOfBytesWritten);

        public static void AntDesign()
        {
            IntPtr targetMemoryAddress = (IntPtr)0x0064547C; 
            byte[] newMachineCode = new byte[] { 0xEB, 0x04 };
            Process currentProcess = Process.GetCurrentProcess();

            IntPtr currentProcessHandle = currentProcess.Handle;

            int bytesWritten;
            WriteProcessMemory(currentProcessHandle, targetMemoryAddress, newMachineCode, (uint)newMachineCode.Length, out bytesWritten);

        }
    }
    public class MainClass : IEntryPoint
    {
        [DllImport("user32.dll")]
        private static extern bool SetProcessDPIAware();

        public MainClass(RemoteHooking.IContext context, string channelName)
        {

        }

        public void Run(RemoteHooking.IContext context, string channelName)
        {
            CodeModifier.AntDesign();
            //System.Diagnostics.Debugger.Launch();
            SendPacket();

            //if (Environment.OSVersion.Version.Major >= 6)
            //{
            //    //SetProcessDPIAware();
            //}

            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);
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
                ws.StartHook();
                RemoteHooking.WakeUpProcess();
                Thread.Sleep(5000);
                ws.StopHook();

                var ls = ws.lastSocket;
                var socketId = ls.Socket;

                //int targetId = RemoteHooking.GetCurrentProcessId();



                //1 | 3412 | 49.234.46.151:7200 | 31 | 23 37 56 72 61 73 59 54 79 6E 5B 73 71 77 5A 50 59 41 75 58 48 4A 4F 4C 72 78 73 3F 5D 6C 21
                int number = 99999;
                string[] hexsString = new string[]{ "23 33 56 72 61 73 59 54 79 6E 5B 73 71 77 5A 50 59 41 3E 3E 78 4A 4F 4C 72 79 6E 7A 6D 3C 21",
                                                    "23 38 56 72 61 73 59 54 75 6E 5B 73 71 77 5A 50 59 41 4B 3D 64 4A 4F 4C 72 79 6A 63 68 4C 21",
                                                    "23 36 56 72 61 73 59 54 75 6E 5B 73 71 77 5A 50 59 41 61 77 3C 4A 4F 4C 72 79 72 4E 40 3C 21",
                                                    "23 37 56 72 61 73 59 54 75 6E 5B 73 71 77 5A 50 59 41 75 58 48 4A 4F 4C 72 78 73 3F 5D 6C 21",
                                                    "23 37 56 72 61 73 59 54 75 6E 5B 73 71 77 5A 50 59 41 79 79 3C 4A 4F 4C 72 78 5A 4E 5F 6C 21",
                                                    "23 37 56 72 61 73 59 54 75 6E 5B 73 71 77 5A 50 59 41 3C 4E 54 4A 4F 4C 72 78 5B 4A 5F 6C 21",
                                                    "23 31 56 72 61 73 59 54 75 6E 5B 73 71 77 5A 50 59 41 5E 46 78 4A 4F 4C 72 78 5B 48 73 4C 21",
                                                    "23 38 56 72 61 73 59 54 75 6E 5B 73 71 77 5A 50 59 41 73 7B 5C 4A 4F 4C 72 78 6B 46 6E 5C 21"
                                                    };

                for (int i = 0; i < number; i++)
                {
                    int times = i < 10 ? 5000 : 15000;

                    for (int j = 0; j < hexsString.Length; j++)
                    {
                 
                        string hexString = hexsString[j];
                        string[] hexValues = hexString.Split(' ');
                        byte[] buffer = new byte[hexValues.Length];

                        for (int k = 0; k < hexValues.Length; k++)
                        {
                            buffer[k] = Convert.ToByte(hexValues[k], 16);
                        }
                        if (i > 2)
                        {
                            buffer[8] = Convert.ToByte(i % 2 == 0 ? 0x75 : 0x79);
                        }

                        IntPtr destination = Marshal.AllocHGlobal(buffer.Length);
                        Marshal.Copy(buffer, 0, destination, buffer.Length);
                        if (this.ws.SendPacket(socketId, destination, buffer.Length))
                        {
                            this.Send_Success_CNT++;
                        }
                        else
                        {
                            this.Send_Fail_CNT++;
                        }
                        Marshal.FreeHGlobal(destination);
                        Thread.Sleep(times);
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
