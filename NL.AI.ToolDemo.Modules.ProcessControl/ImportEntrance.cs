using NL.AI.ToolDemo.Enum;
using NL.AI.ToolDemo.IBLL;
using NL.AI.ToolDemo.Model;
using NL.AI.ToolDemo.Tools;
using NL.CardioReader.VoidPower.VMModule.IF;
using NL.CardioReader.VoidPower.VVMModule.IF;
using NL.SkyCastle.Infra.ITrailer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NL.AI.ToolDemo.Modules.ProcessControl
{
    public class ImportEntrance : IImportEntrance
    {
        private IMessageModule _messageModule;
        private ISkyCastleTrailer _skyCastleTrailer;
        private IFileInfoBusi _fileInfoBusi;
        private IDialogFactory _dialogFactory;

        public ImportEntrance(IMessageModule messageModule, ISkyCastleTrailer skyCastleTrailer, IFileInfoBusi fileInfoBusi, IDialogFactory dialogFactory)
        {
            _messageModule = messageModule;
            _dialogFactory = dialogFactory;
            _skyCastleTrailer = skyCastleTrailer;
            _fileInfoBusi = fileInfoBusi;

            _messageModule.Register<string>(this, AIToolMessageKeyEnum.ImportFile, OnImportFile);
        }

        private async Task OnImportFile(string fileUrl)
        {
            _skyCastleTrailer.WriteInfo("开始导入：" + fileUrl);
            if (System.IO.File.Exists(fileUrl))
            {
                FileInfo fileInfo = new FileInfo()
                {
                    Id = IdWorker.NewDefaultId,
                    PatientName = "李四" + DateTime.Now.Second,
                    PatientGender = 1,
                    PatientAge = 30,
                    PatientAgeUnit = 1,
                    AIDiagnosis = "asdfasdfasdfasdf",
                    Remark = DateTime.Now.ToString(),
                    LocalFileUrl = fileUrl,
                };
                var result = await _fileInfoBusi.CreateFileInfo(fileInfo);
                var message = _dialogFactory.GetDialog<IMessageDialog>();
                if (result)
                {
                    message.ShowMessage("提示", "导入成功", null, null);
                    _messageModule.Send(AIToolMessageKeyEnum.FreshDataList, string.Empty);
                }
                else
                {
                    message.ShowMessage("提示", "导入失败", null, null);
                }
            }
        }

        public void Init()
        {
            
        }

        public void Dispose()
        {
            _messageModule.Unregister<string>(this, AIToolMessageKeyEnum.ImportFile, OnImportFile);
        }
    }
}
