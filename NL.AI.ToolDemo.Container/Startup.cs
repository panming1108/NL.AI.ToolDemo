using Library;
using NL.AI.ToolDemo.Enum;
using NL.AI.ToolDemo.Modules.ProcessControl;
using NL.CardioReader.VoidPower.VMModule.IF;
using NL.CardioReader.VoidPower.VOnly.IF;
using NL.CardioReader.VoidPower.WPFContainer;
using NL.SkyCastle.Infra.ITrailer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;

namespace NL.AI.ToolDemo.Container
{
    public class Startup : AppBase
    {
        private ISkyCastleTrailer _skyCastleTrailer;
        private IMessageModule _messageModule;
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            _skyCastleTrailer = IocManagerInstance.ResolveType<ISkyCastleTrailer>();           

            string[] list = new string[]
            {
                "NL.AI.ToolDemo.CommonTools.Views",
                "NL.AI.ToolDemo.ECGData.Views",
                "NL.AI.ToolDemo.Controls",
                "NL.CardioReader.CommonControlsThemes.WPF4",
                "NL.CardioReader.CommonControlsSource.MainFrame.WPF4",
                "NL.CardioReader.CommonControlsSource.UnivDialog.WPF4",
                "NL.CardioReader.CommonControlsSource.Navis.WPF4",
                "NL.CardioReader.CommonControlsSource.Editors.WPF4",
                "NL.CardioReader.CommonControlsSource.DMs.WPF4",
                "NL.CardioReader.CommonControlsSource.FMs.WPF4"
            };
            StaticThemeManager.Init(list, "DarkBlackTheme");
            Application.Current.MainWindow = new MainWindow();
            if(e.Args.Length == 1)
            {
                _messageModule = IocManagerInstance.ResolveType<IMessageModule>();
                FileInfo fileInfo = new FileInfo(e.Args[0]);
                if (fileInfo.Exists)
                {
                    _messageModule.Send(AIToolMessageKeyEnum.ImportFile, fileInfo.FullName);
                }
            }
            Application.Current.MainWindow.ShowDialog();
        }
    }
}
