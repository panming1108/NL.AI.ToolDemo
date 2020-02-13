using NL.CardioReader.VoidPower.VOnly.IF;
using NL.CardioReader.VoidPower.WPFContainer;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace NL.AI.ToolDemo.Container
{
    public class Program
    {
        [STAThread]
        public static void Main(string[] orgs)
        {
            Process instance = ProcessMessageHelper.RunningInstance();
            if (instance == null)
            {
                string[] list = new string[] {
                    "NL.SkyCastle.Infra.Trailer.NLogProvider.F4V"
                    ,"NL.SkyCastle.Infra.TrailerBase.F4V"
                    ,"NL.SkyCastle.Infra.Mapping.EmitMapping.F4V"
                    ,"NL.SkyCastle.Infra.Serialize.JsonSerialize.F4V"

                    ,"NL.CardioReader.VoidPower.VMModule.CoreLib.F4V"
                    ,"NL.CardioReader.VoidPower.VMOnly.CoreLib.F4V"
                    ,"NL.CardioReader.VoidPower.WPFContainer.WPF4"

                    ,"NL.CardioReader.ImagineBreaker.BaseLib.F4V"
                    ,"NL.CardioReader.ImagineBreaker.CoreLib.WPF4"
                    ,"NL.AmuletHeart.ECGL.CoreLib.WPF4"
                    ,"NL.AmuletHeart.ECGL.BaseLib.F4V"
                    ,"NL.AmuletHeart.ECGL.InfLib.WPF4"
                    ,"NL.AmuletHeart.ECGL.RealizationLib.WPF4"
                    ,"NL.GradationProjection.Delineation.WPF4"
                    ,"NL.CardioReader.MidEnd.Views.DiagECG.WPF4"
                    ,"NL.CardioReader.MidEnd.ContainerEx"
                    ,"NL.CardioReader.MidEnd.Themes.WPF4"
                    ,"NL.CardioReader.MidEnd.Views.BeatEditor.WPF4"
                    ,"NL.CardioReader.MidEnd.ECGProvider"

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
                    ,"NL.AI.ToolDemo.Modules.ProcessControl"
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
            }
            else
            {
                if (orgs.Count() == 1)
                {
                    FileInfo fileInfo = new FileInfo(orgs[0]);
                    if (fileInfo.Exists)
                    {
                        ProcessMessageHelper.SendMessage(instance, fileInfo.FullName);
                    }
                }
                ProcessMessageHelper.HandleRunningInstance(instance);                              
            }
        }
    }
}
