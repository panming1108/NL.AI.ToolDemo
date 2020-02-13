using NL.AI.ToolDemo.Enum;
using NL.CardioReader.VoidPower.VMModule.IF;
using NL.CardioReader.VoidPower.VOnly.IF;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace NL.AI.ToolDemo.Container
{
    public class ProcessMessageHelper
    {
        private const int WS_SHOWNORMAL = 1;
        public const int WM_COPYDATA = 0x004A; // 固定数值，不可更改

        public struct COPYDATASTRUCT
        {
            public IntPtr dwData; // 任意值
            public int cbData;    // 指定lpData内存区域的字节数
            [MarshalAs(UnmanagedType.LPStr)]
            public string lpData; // 发送给目标窗口所在进程的数据
        }

        [DllImport("User32.dll")]
        private static extern bool ShowWindowAsync(IntPtr hWnd, int cmdShow);
        [DllImport("User32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);
        [DllImport("User32.dll", EntryPoint = "SendMessage")]
        private static extern int SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, ref COPYDATASTRUCT lParam);

        /// <summary>
        /// 获取正在运行的进程
        /// </summary>
        /// <returns>当前进程</returns>
        public static Process RunningInstance()
        {
            Process current = Process.GetCurrentProcess();
            Process[] processes = Process.GetProcessesByName(current.ProcessName);
            //Loop through the running processes in with the same name   
            foreach (Process process in processes)
            {
                //Ignore the current process   
                if (process.Id != current.Id)
                {
                    //Make sure that the process is running from the exe file.   
                    if (Assembly.GetExecutingAssembly().Location.Replace("/", "//") == current.MainModule.FileName)
                    {
                        //Return the other process instance.   
                        return process;
                    }
                }
            }
            //No other instance was found, return null. 
            return null;
        }

        /// <summary>
        /// 激活当前进程
        /// </summary>
        /// <param name="instance">目标进程</param>
        public static void HandleRunningInstance(Process instance)
        {
            //Make sure the window is not minimized or maximized   
            ShowWindowAsync(instance.MainWindowHandle, WS_SHOWNORMAL);
            //Set the real intance to foreground window
            SetForegroundWindow(instance.MainWindowHandle);
        }

        /// <summary>
        /// 发送进程间消息
        /// </summary>
        /// <param name="process">目标进程</param>
        /// <param name="message">发送的消息</param>
        public static void SendMessage(Process process, string message)
        {
            byte[] sarr = Encoding.Default.GetBytes(message);
            int len = sarr.Length;
            COPYDATASTRUCT cds2;
            cds2.dwData = (IntPtr)0;
            cds2.cbData = len + 1;
            cds2.lpData = message;
            // 发送消息
            SendMessage(process.MainWindowHandle, WM_COPYDATA, IntPtr.Zero, ref cds2);
        }

        public static IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == WM_COPYDATA)
            {
                IMessageModule _messageModule = IocManagerInstance.ResolveType<IMessageModule>();
                var st = (COPYDATASTRUCT)Marshal.PtrToStructure(lParam, typeof(COPYDATASTRUCT));
                _messageModule.Send(AIToolMessageKeyEnum.ProcessMessage, st.lpData);
            }
            return IntPtr.Zero;
        }
    }
}
