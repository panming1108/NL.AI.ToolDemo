using Library;
using NL.CardioReader.VoidPower.VOnly.IF;
using NL.CardioReader.VoidPower.WPFContainer;
using NL.SkyCastle.Infra.ITrailer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace NL.AI.ToolDemo.Container
{
    public class Startup : AppBase
    {
        private ISkyCastleTrailer _skyCastleTrailer;
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            _skyCastleTrailer = IocManagerInstance.ResolveType<ISkyCastleTrailer>();
            if (e.Args.Length <= 1)
            {
                if(e.Args.Length == 1)
                {
                    System.IO.FileInfo file = new System.IO.FileInfo(e.Args[0]);
                    if (file.Exists)
                    {
                        MessageBox.Show(file.FullName);
                    }
                }
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
                Application.Current.MainWindow.ShowDialog();
            }
        }
    }
}
