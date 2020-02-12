using NL.CardioReader.VoidPower.VOnly.IF;
using NL.CardioReader.VoidPower.WPFContainer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace NL.AI.ToolDemo.Container
{
    public class Program
    {
        [STAThread]
        public static void Main(string[] orgs)
        {
            var mutex = new Mutex(true, "NL.AI.ToolDemo.Container", out bool createNew);

            if (createNew)
            {
                string[] list = new string[] {
                    "NL.SkyCastle.Infra.Trailer.NLogProvider.F4V"
                    ,"NL.SkyCastle.Infra.TrailerBase.F4V"

                    ,"NL.CardioReader.VoidPower.VMModule.CoreLib.F4V"
                    ,"NL.CardioReader.VoidPower.VMOnly.CoreLib.F4V"
                    ,"NL.CardioReader.VoidPower.WPFContainer.WPF4"

                    ,"NL.CardioReader.CommonControlsThemes.WPF4"
                    ,"NL.CardioReader.CommonControlsSource.MainFrame.WPF4"
                    ,"NL.CardioReader.CommonControlsSource.UnivDialog.WPF4"
                    ,"NL.CardioReader.CommonControlsSource.Navis.WPF4"
                    ,"NL.CardioReader.CommonControlsSource.Editors.WPF4"
                    ,"NL.CardioReader.CommonControlsSource.DMs.WPF4"
                    ,"NL.CardioReader.CommonControlsSource.FMs.WPF4"
                    ,"NL.AI.ToolDemo.Controls"

                    ,"NL.AI.ToolDemo.BLL"
                    ,"NL.AI.ToolDemo.DAL"
                    ,"NL.AI.ToolDemo.ECGData.Views"
                    ,"NL.AI.ToolDemo.CommonTools.Views"
                    ,"NL.AI.ToolDemo.CommonTools.ViewModels"
                    ,"NL.AI.ToolDemo.ECGData.ViewModels"
                    ,"NL.AI.ToolDemo.Container"
                };
                IocManager.RegisterAssemblyInterfaces(list);
                IocManagerInstance.Init(new IocManager());

                Startup startup = new Startup();
                startup.Run();
                mutex.WaitOne();
            }
        }
    }
}
