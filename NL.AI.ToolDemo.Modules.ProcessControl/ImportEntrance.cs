using NL.AI.ToolDemo.Enum;
using NL.AI.ToolDemo.IBLL;
using NL.AI.ToolDemo.Model;
using NL.AI.ToolDemo.Tools;
using NL.CardioReader.MidEnd.BizObject;
using NL.CardioReader.MidEnd.VM.KeyEnum;
using NL.CardioReader.VoidPower.VMModule.IF;
using NL.CardioReader.VoidPower.VVMModule.IF;
using NL.SkyCastle.Infra.ISerialize;
using NL.SkyCastle.Infra.ITrailer;
using NL.SkyCastle.Infra.Mapping.Interface;
using NLC.ECG.Core.AnalysisService.Analysis;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NL.AI.ToolDemo.Modules.ProcessControl
{
    public class ImportEntrance : IImportEntrance
    {
        private IMessageModule _messageModule;
        private ISkyCastleTrailer _skyCastleTrailer;
        private IFileInfoBusi _fileInfoBusi;
        private IDialogFactory _dialogFactory;
        private IMapping _mapping;
        private IJsonSerializer _jsonSerializer;

        public const string BEAT = ".beat";

        public ImportEntrance(IMessageModule messageModule, ISkyCastleTrailer skyCastleTrailer, IFileInfoBusi fileInfoBusi, IDialogFactory dialogFactory, IMapping mapping, IJsonSerializer jsonSerializer)
        {
            _messageModule = messageModule;
            _dialogFactory = dialogFactory;
            _skyCastleTrailer = skyCastleTrailer;
            _fileInfoBusi = fileInfoBusi;
            _mapping = mapping;
            _jsonSerializer = jsonSerializer;

            _messageModule.Register<string>(this, AIToolMessageKeyEnum.ImportFile, OnImportFile);
        }

        private async Task OnImportFile(string fileUrl)
        {
            _skyCastleTrailer.WriteInfo("开始导入：" + fileUrl);
            _messageModule.Send<LoadingStatusEnum>(MessagerKeyEnum.Loading, LoadingStatusEnum.IsBusy);
            if (System.IO.File.Exists(fileUrl))
            {
                FileDataInfo fileInfo = new FileDataInfo()
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

                await Task.Factory.StartNew(() =>
                {
                    var analysis = new FileAnalysis().GetAnalysisResult(fileUrl);
                    var beatInfos = _mapping.IEnumerableMap<NLC.ECG.Core.AnalysisService.Models.BeatInfo, BOBeatInfo>(analysis.BeatInfos).ToArray();
                    SaveBeatInfoFile(fileInfo.LocalFileUrl, beatInfos);
                });

                _messageModule.Send<LoadingStatusEnum>(MessagerKeyEnum.Loading, LoadingStatusEnum.NotBusy);
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

        /// <summary>
        /// 保存心搏数据
        /// </summary>
        /// <param name="localFileUrl"></param>
        /// <returns>心搏文件路径</returns>
        private string SaveBeatInfoFile(string localFileUrl, BOBeatInfo[] beatInfos)
        {
            var beatFilePath = localFileUrl.Replace(Path.GetExtension(localFileUrl), BEAT);
            //判断心拍文件是否存在，存在直接return
            if (File.Exists(beatFilePath))
            {
                _skyCastleTrailer.WriteError("心搏信息文件已存在，路径：" + beatFilePath, null);
                return beatFilePath;
            }

            if (beatInfos == null)
            {
                _skyCastleTrailer.WriteWarn("没有心搏信息，anb路径：" + localFileUrl);
                beatInfos = new BOBeatInfo[0];
            }
            var beatInfoList = beatInfos.ToList();

            //创建beatinfo文件
            var beatjson = _jsonSerializer.Serialize(beatInfoList);
            File.AppendAllText(beatFilePath, beatjson);

            return beatFilePath;
        }
    }
}
