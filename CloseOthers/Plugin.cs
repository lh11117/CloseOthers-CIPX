
using ClassIsland.Core.Abstractions;
using ClassIsland.Core.Attributes;
using ClassIsland.Core.Controls.CommonDialog;
using ClassIsland.Core.Extensions.Registry;
using ClassIsland.Shared.Helpers;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using static unvell.Common.Win32Lib.Win32;

namespace CloseOthers
{
    public class Settings : ObservableObject
    {
        public bool CW { get; set; } = false;
        public bool TimeNest { get; set; } = false;
    };

    public class Datas
    {
        public static string PluginConfigFolder { get; set; } = "";
        public static Settings Settings { get; set; } = new();

        public void SetPluginConfigFolder(string s)
        {
            PluginConfigFolder = s;
        }
        public string GetPluginConfigFolder()
        {
            return PluginConfigFolder;
        }
    }

    [PluginEntrance]
    public class Plugin : PluginBase
    {
        private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

        // 导入 user32.dll 的函数
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        private System.Timers.Timer timer = new System.Timers.Timer();

        public static string GetWindowTitle(IntPtr hWnd)
        {
            int length = GetWindowTextLength(hWnd);

            if (length <= 0)
            {
                return string.Empty;
            }

            StringBuilder titleBuilder = new StringBuilder(length + 1);

            int copiedLength = GetWindowText(hWnd, titleBuilder, titleBuilder.Capacity);

            if (copiedLength > 0)
            {
                return titleBuilder.ToString();
            }
            else
            {
                return string.Empty;
            }
        }

        public static string GetWindowClassName(IntPtr hWnd)
        {
            int bufferSize = 256;
            StringBuilder classNameBuilder = new StringBuilder(bufferSize);

            int length = GetClassName(hWnd, classNameBuilder, bufferSize);

            if (length > 0)
            {
                return classNameBuilder.ToString();
            }
            else
            {
                return string.Empty;
            }
        }

        public override void Initialize(HostBuilderContext context, IServiceCollection services)
        {
            Datas datas = new();
            datas.SetPluginConfigFolder(PluginConfigFolder);
            Datas.Settings = ConfigureFileHelper.LoadConfig<Settings>(Path.Combine(PluginConfigFolder, "Settings.json"));
            Datas.Settings.PropertyChanged += (sender, args) =>
            {
                ConfigureFileHelper.SaveConfig<Settings>(Path.Combine(PluginConfigFolder, "Settings.json"), Datas.Settings);
            };


            services.AddSettingsPage<SettingsPage>();


            timer.Interval = 100;
            timer.AutoReset = true;
            timer.Enabled = true;
            timer.Elapsed += (e, o) =>
            {
                //Console.Write((Datas.Settings.CW?"1":"0") + (Datas.Settings.TimeNest?"1":"0"));
                // CW
                if (Datas.Settings.CW)
                {
                    List<IntPtr> windowHandles = new List<IntPtr>();
                    EnumWindowsProc enumWindowsProc = (hWnd, lParam) =>
                    {
                        uint windowProcessId;
                        GetWindowThreadProcessId(hWnd, out windowProcessId);
                        Process process;
                        try
                        {
                            process = Process.GetProcessById((int)windowProcessId);
                        }
                        catch (Exception)
                        {
                            return false;
                        }
                        if (process.ProcessName.ToLower().Contains("classwidgets"))
                        {
                            process.Kill();
                            process.Close();
                            Console.WriteLine("关闭CW成功(0)");
                            return false;
                        }
                        string ClassName = GetWindowClassName(hWnd);
                        if (ClassName == "Qt5152QWindowToolSaveBits")
                        {
                            string WindowName = GetWindowTitle(hWnd);
                            Console.WriteLine(WindowName);
                            if (WindowName == "更多活动" || WindowName == "当前活动" || WindowName == "活动倒计时" || WindowName == "基本组件")
                            {
                                process.Kill();
                                process.Close();
                                Console.WriteLine("关闭CW成功(1)");
                                return false;
                            }
                        }
                        if (ClassName == "Qt5152QWindowPopupSaveBits")
                        {
                            string WindowName = GetWindowTitle(hWnd);
                            if (WindowName.Contains("ClassWidgets"))
                            {
                                process.Kill();
                                process.Close();
                                Console.WriteLine("关闭CW成功(2)");
                                return false;
                            }
                        }
                        process.Close();
                        return true;
                    };
                    EnumWindows(enumWindowsProc, IntPtr.Zero);
                }

                // TimeNest
                if (Datas.Settings.TimeNest)
                {
                    List<IntPtr> windowHandles = new List<IntPtr>();
                    EnumWindowsProc enumWindowsProc = (hWnd, lParam) =>
                    {
                        uint windowProcessId;
                        GetWindowThreadProcessId(hWnd, out windowProcessId);
                        Process process;
                        try
                        {
                            process = Process.GetProcessById((int)windowProcessId);
                        }
                        catch (Exception)
                        {
                            return false;
                        }
                        if (process.ProcessName.ToLower().Contains("timenest"))
                        {
                            process.Kill();
                            process.Close();
                            Console.WriteLine("关闭TimeNest成功(0)");
                            return false;
                        }
                        string ClassName = GetWindowClassName(hWnd);
                        if (ClassName.Contains("TkTopLevel"))
                        {
                            string WindowName = GetWindowTitle(hWnd);
                            if (WindowName.Contains("课程表悬浮窗"))
                            {
                                process.Kill();
                                process.Close();
                                Console.WriteLine("关闭TimeNest成功(1)");
                                return false;
                            }
                        }
                        process.Close();
                        return true;
                    };
                    EnumWindows(enumWindowsProc, IntPtr.Zero);
                }
            };

        }
    }
}
