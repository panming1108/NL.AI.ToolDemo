using NL.AI.ToolDemo.Container.IViewModels;
using NL.AI.ToolDemo.Enum;
using NL.AI.ToolDemo.Modules.ProcessControl;
using NL.CardioReader.MidEnd.BizObject;
using NL.CardioReader.MidEnd.ContainerEx;
using NL.CardioReader.MidEnd.VM.KeyEnum;
using NL.CardioReader.VoidPower.VMModule.IF;
using NL.CardioReader.VoidPower.VOnly.IF;
using NL.CardioReader.VoidPower.VVMModule.IF;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NL.AI.ToolDemo.Container
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : SingleProcessWindow
    {
        private readonly IMessageModule _messageModule;
        private readonly IDialogFactory _dialogFactory;
        private readonly IImportEntrance _importEntrance;
        private readonly IECGViewStartEntrance _eCGViewStartEntrance;
        private readonly ICacheManager _cacheManager;
        private readonly IMessageCacheUpdateConsumer _messageCacheUpdateConsumer;

        public MainWindow()
        {
            InitializeComponent();

            InitWindowSize();

            Loaded += MainWindow_Loaded;
            Unloaded += MainWindow_Unloaded;

            _messageModule = IocManagerInstance.ResolveType<IMessageModule>();
            _dialogFactory = IocManagerInstance.ResolveType<IDialogFactory>();
            _cacheManager = IocManagerInstance.ResolveType<ICacheManager>();
            _importEntrance = IocManagerInstance.ResolveType<IImportEntrance>();
            _eCGViewStartEntrance = IocManagerInstance.ResolveType<IECGViewStartEntrance>();
            _messageCacheUpdateConsumer = new MessageCacheUpdateConsumerEx();

            _messageModule.Register<WindowOperateEnum>(this, MessagerKeyEnum.MainWinChanged, MainWinChanged);
            _messageModule.Register<string>(this, AIToolMessageKeyEnum.ProcessMessage, OnReceiveProcessMessage);            

            DataContext = IocManagerInstance.ResolveType<IMainViewModel>();
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            _messageCacheUpdateConsumer.Init();

            List<BODoctorConfig> doctorConfigs = new List<BODoctorConfig>()
            {
                new BODoctorConfig()
                {
                    ConfigKey = "ECGBackViewMode",
                    ConfigValue = "2"
                },
                new BODoctorConfig()
                {
                    ConfigKey = "TwelveLead",
                    ConfigValue = "2"
                },
                new BODoctorConfig()
                {
                    ConfigKey = "FifteenLead",
                    ConfigValue = "6"
                },
                new BODoctorConfig()
                {
                    ConfigKey = "EighteenLead",
                    ConfigValue = "9"
                }
            };
            _cacheManager.TrySet(CacheKeyEnum.BaseDoctorConfig, doctorConfigs);
        }

        private void MainWindow_Unloaded(object sender, RoutedEventArgs e)
        {
            _messageModule.Unregister<string>(this, AIToolMessageKeyEnum.ProcessMessage, OnReceiveProcessMessage);
            _messageModule.Unregister<WindowOperateEnum>(this, MessagerKeyEnum.MainWinChanged, MainWinChanged);
            Loaded -= MainWindow_Loaded;
            Unloaded -= MainWindow_Unloaded;
        }

        private async Task MainWinChanged(WindowOperateEnum obj)
        {
            await TaskEx.FromResult(0);
            switch (obj)
            {
                case WindowOperateEnum.None:
                    throw new Exception("MainWinChanged WindowOperate.None");
                case WindowOperateEnum.WinMin:
                    this.WindowState = WindowState.Minimized;
                    break;
                case WindowOperateEnum.Close:
                    var confirm = _dialogFactory.GetDialog<IConfirmDialog>();
                    confirm.ShowDialog("消息提示", "是否确认退出系统？"
                        , () =>
                        {
                            Close();
                        }, null, null);
                    break;
                default:
                    break;
            }
        }

        private async Task OnReceiveProcessMessage(string message)
        {
            await TaskEx.FromResult(0);
            this.Dispatcher.Invoke(new Action(delegate
            {
                _messageModule.Send(AIToolMessageKeyEnum.ImportFile, message);
            }));
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            ProcessExit();
        }

        private void ProcessExit()
        {
            var exeName = Process.GetCurrentProcess().MainModule.FileName;
            var exeArray = exeName.Split('\\');
            RunCmd("taskkill /im " + exeArray[exeArray.Length - 1] + " /f ");
        }

        private string RunCmd(string command)
        {
            //實例一個Process類，啟動一個獨立進程
            var p = new Process();

            //Process類有一個StartInfo屬性，這個是ProcessStartInfo類，包括了一些屬性和方法，下面我們用到了他的幾個屬性：

            p.StartInfo.FileName = "cmd.exe";           //設定程序名
            p.StartInfo.Arguments = "/c " + command;    //設定程式執行參數
            p.StartInfo.UseShellExecute = false;        //關閉Shell的使用
            p.StartInfo.RedirectStandardInput = true;   //重定向標準輸入
            p.StartInfo.RedirectStandardOutput = true;  //重定向標準輸出
            p.StartInfo.RedirectStandardError = true;   //重定向錯誤輸出
            p.StartInfo.CreateNoWindow = true;          //設置不顯示窗口

            p.Start();   //啟動

            return p.StandardOutput.ReadToEnd();        //從輸出流取得命令執行結果
        }
    }
}
