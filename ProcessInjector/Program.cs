using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProcessInjector
{
    static class Program
    {
        public static int PID = -1;
        public static string PNAME = "Client.dat";
        public static string PATH = string.Empty;

        [DllImport("user32.dll")]
        private static extern bool SetProcessDPIAware();

        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            try
            {
                if (Environment.OSVersion.Version.Major >= 6)
                {
                    SetProcessDPIAware();
                }

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                // 获取命令行参数
                string[] commandLineArgs = Environment.GetCommandLineArgs();

                // 打印命令行参数
                Console.WriteLine("命令行参数:");
                foreach (string arg in commandLineArgs)
                {
                    Console.WriteLine(arg);
                }
                int pid = 0;
                int.TryParse(commandLineArgs[1],out pid);

                if (!new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator))
                {
                    ProcessStartInfo startInfo = new ProcessStartInfo {
                        UseShellExecute = true,
                        WorkingDirectory = Environment.CurrentDirectory,
                        FileName = Application.ExecutablePath,
                        Verb = "runas"
                    };
                    try
                    {
                        Process.Start(startInfo);
                    }
                    catch
                    {
                        return;
                    }
                    Application.Exit();
                }
                else
                {
                    Application.Run(new Injector_Form(pid));
                }
            }
            catch (Exception ex)
            {
                string message = ex.Message;
            }
        }
    }
}
