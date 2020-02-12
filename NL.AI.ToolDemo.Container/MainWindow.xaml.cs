using NL.AI.ToolDemo.Container.IViewModels;
using NL.CardioReader.MidEnd.VM.KeyEnum;
using NL.CardioReader.VoidPower.VMModule.IF;
using NL.CardioReader.VoidPower.VOnly.IF;
using NL.CardioReader.VoidPower.VVMModule.IF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NL.AI.ToolDemo.Container
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly IMessageModule _messageModule;
        private readonly IDialogFactory _dialogFactory;

        public MainWindow()
        {
            InitializeComponent();

            Rect rc = SystemParameters.WorkArea;//获取工作区大小
            this.Left = 0;//设置位置
            this.Top = 0;
            this.Width = rc.Width;
            this.Height = rc.Height;

            _messageModule = IocManagerInstance.ResolveType<IMessageModule>();
            _dialogFactory = IocManagerInstance.ResolveType<IDialogFactory>();

            _messageModule.Register<WindowOperateEnum>(this, MessagerKeyEnum.MainWinChanged, MainWinChanged);

            DataContext = IocManagerInstance.ResolveType<IMainViewModel>();
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
    }
}
