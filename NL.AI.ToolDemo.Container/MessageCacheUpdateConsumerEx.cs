using NL.AmuletHeart.ECGBL.IF;
using NL.AmuletHeart.ECGL.IF;
using NL.CardioReader.MidEnd.BizObject;
using NL.CardioReader.MidEnd.ContainerEx;
using NL.CardioReader.MidEnd.ECGCalculations;
using NL.CardioReader.MidEnd.IBLL;
using NL.CardioReader.MidEnd.VM.CacheObject;
using NL.CardioReader.MidEnd.VM.KeyEnum;
using NL.CardioReader.MidEnd.VM.MessageObject;
using NL.CardioReader.VoidPower.VMModule.IF;
using NL.CardioReader.VoidPower.VOnly.IF;
using NL.SkyCastle.DXC.Infrastructure.Treasury.Enum;
using NL.SkyCastle.Infra.Mapping.Interface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NL.AI.ToolDemo.Container
{
    public class MessageCacheUpdateConsumerEx : IMessageCacheUpdateConsumer
    {
        private readonly IMessageModule _messageModule;
        private readonly ICacheManager _cacheManager;
        //private readonly ICommonApi _commonApi;
        //private readonly IDiagCacheManagementBasic _diagCacheManagement;
        //private readonly IBeatInfoBusi _beatInfoBusi;
        private readonly IECGProviderFactory _eCGProviderFactory;
        //private readonly IMapping _mapping;

        public MessageCacheUpdateConsumerEx()
        {
            _messageModule = IocManagerInstance.ResolveType<IMessageModule>();
            _cacheManager = IocManagerInstance.ResolveType<ICacheManager>();
            //_commonApi = IocManagerInstance.ResolveType<ICommonApi>();
            //_diagCacheManagement = diagCacheManagement;
            //_beatInfoBusi = beatInfoBusi;
            _eCGProviderFactory = IocManagerInstance.ResolveType<IECGProviderFactory>(); 
            //_mapping = mapping;

            _messageModule.Register<string>(this, MessagerKeyEnum.ControlLoadData, ControlLoadData);
            _messageModule.Register<string>(this, MessagerKeyEnum.ControlClearData, ControlClearData);
            //_messageModule.Register<long>(this, MessagerKeyEnum.ShowAcquisitionMap, ShowAcquisitionMap);

            _messageModule.Register<int>(this, ECGMessagerKeyEnum.AmuletHeart_LeadLayoutDiag, UpdateLeadLayoutCache);
            _messageModule.Register<int>(this, ECGMessagerKeyEnum.AmuletHeart_DisplayModeDiag, UpdateDisplayModeCache);
            _messageModule.Register<Dictionary<ECGToolPopEnum, int>>(this, MessagerKeyEnum.FilterChange, UpdateFilterChangeCache);
            _messageModule.Register<WaveGainEnum>(this, ECGMessagerKeyEnum.AmuletHeart_SwitchGainDiag, UpdateSwitchGainCache);
            _messageModule.Register<WaveDotXAxisLandingMessageObject>(this, ECGMessagerKeyEnum.AmuletHeart_WaveMoveDiag, UpdateWaveMoveCache);
            _messageModule.Register<WaveDotPositionLandingMessageObject>(this, ECGMessagerKeyEnum.AmuletHeart_WaveDotChangedDiag, UpdateWaveDotChangedCache);
            _messageModule.Register<string>(this, MessagerKeyEnum.SwitchFileMeasureValue, SwitchFileMeasureValue);
        }

        public void Dispose()
        {
            _messageModule.Unregister<string>(this, MessagerKeyEnum.ControlLoadData, ControlLoadData);
            _messageModule.Unregister<string>(this, MessagerKeyEnum.ControlClearData, ControlClearData);
            //_messageModule.Unregister<long>(this, MessagerKeyEnum.ShowAcquisitionMap, ShowAcquisitionMap);

            _messageModule.Unregister<int>(this, ECGMessagerKeyEnum.AmuletHeart_LeadLayoutDiag, UpdateLeadLayoutCache);
            _messageModule.Unregister<int>(this, ECGMessagerKeyEnum.AmuletHeart_DisplayModeDiag, UpdateDisplayModeCache);
            _messageModule.Unregister<Dictionary<ECGToolPopEnum, int>>(this, MessagerKeyEnum.FilterChange, UpdateFilterChangeCache);
            _messageModule.Unregister<WaveGainEnum>(this, ECGMessagerKeyEnum.AmuletHeart_SwitchGainDiag, UpdateSwitchGainCache);
            _messageModule.Unregister<WaveDotXAxisLandingMessageObject>(this, ECGMessagerKeyEnum.AmuletHeart_WaveMoveDiag, UpdateWaveMoveCache);
            _messageModule.Unregister<WaveDotPositionLandingMessageObject>(this, ECGMessagerKeyEnum.AmuletHeart_WaveDotChangedDiag, UpdateWaveDotChangedCache);
            _messageModule.Unregister<string>(this, MessagerKeyEnum.SwitchFileMeasureValue, SwitchFileMeasureValue);
        }

        public void Init()
        {
        }

        private async Task ControlClearData(string obj)
        {
            //_diagCacheManagement.ClearCache();

            await TaskEx.FromResult(0);
        }

        private async Task ControlLoadData(string obj)
        {
            var maps = _cacheManager.TryGet<List<COAcquisitionFile>>(CacheKeyEnum.AcquisitionFileKey.ToString());

            if (maps.Count < 1)
            {
                throw new Exception("该份报告不存在采集记录，请联系管理员");
            }

            var originalDiagResult = _cacheManager.TryGet<CODiagnosisResultBase>(CacheKeyEnum.OriginalDiagnosisResultKey.ToString());
            var reportInfo = _cacheManager.TryGet<COReportInfo>(CacheKeyEnum.ReportBaseInfoKey);
            var config = _cacheManager.TryGet<List<BODoctorConfig>>(CacheKeyEnum.BaseDoctorConfig);

            COAcquisitionFile file = null;
            long id = 0;

            ReportConsultStatusEnum reportConsultStatusEnum = ReportConsultStatusEnum.None;

            file = maps.Last();
            id = file.Id;

            foreach (var item in maps)
            {
                //状态在未提交之前
                FileSetConfig(item, config);
            }

            reportConsultStatusEnum = ReportConsultStatusEnum.NotSubmitAndNotHang;

            _cacheManager.TrySet<long>(CacheKeyEnum.CurrentAcquisitionFileKey, id);

            ///Doc  必须获取Para,才能加载Provider
            var configKey = await ProviderLoad(file);

            ECGCalculationsHelper.ConfigToBitmapStyle(file, configKey, config);

            _cacheManager.TrySet<ReportConsultStatusEnum>(CacheKeyEnum.ReportConsultStatus, reportConsultStatusEnum);

            await TaskEx.Delay(500);
            //发送消息，继续传递采集Id
            _messageModule.Send<long>(MessagerKeyEnum.UpdateFileCacheAndEcgLoad, id);
        }

        //private async Task ShowAcquisitionMap(long obj)
        //{
        //    ///Doc 如果图谱传入相同的ID ，则无视,需要有这种判断
        //    ///
        //    _cacheManager.TrySet<long>(CacheKeyEnum.CurrentAcquisitionFileKey, obj);

        //    var file = _cacheManager.TryGet<List<COAcquisitionFile>>(CacheKeyEnum.AcquisitionFileKey.ToString()).Single(x => x.Id == obj);

        //    var originalResult = _cacheManager.TryGet<CODiagnosisResultBase>(CacheKeyEnum.OriginalDiagnosisResultKey.ToString());
        //    var config = _cacheManager.TryGet<List<BODoctorConfig>>(CacheKeyEnum.BaseDoctorConfig.ToString());

        //    var configKey = await _commonApi.ProviderLoad(file);

        //    var status = _cacheManager.TryGet<ReportConsultStatusEnum>(CacheKeyEnum.ReportConsultStatus);

        //    switch (status)
        //    {
        //        case ReportConsultStatusEnum.None:
        //            throw new Exception("ReportConsumerStatus.None");
        //            break;
        //        case ReportConsultStatusEnum.NotSubmitAndNotHang:

        //            ECGCalculationsHelper.ConfigToBitmapStyle(file, configKey, config);

        //            break;
        //        case ReportConsultStatusEnum.NotSubmitHang:
        //        case ReportConsultStatusEnum.Submited:
        //            if (file.Id != originalResult.AcquisitionFileId)
        //            {
        //                ECGCalculationsHelper.ConfigToBitmapStyle(file, configKey, config);
        //            }
        //            else
        //            {
        //                ECGCalculationsHelper.RowColumnToBitMap(file, configKey);

        //                ECGCalculationsHelper.IndependentLeadCompletion(file, config);

        //                file.OffsetWaveDot = (int)(file.Offset * _cacheManager.TryGet<IWavePulseReader>(CacheKeyEnum.ECGBaseInfoKey.ToString()).SamplePerSecond / 1000d);
        //            }
        //            break;
        //        default:
        //            throw new Exception("ReportConsumerStatus.default");
        //            break;
        //    }

        //    //发送消息，继续传递采集Id
        //    _messageModule.Send<long>(MessagerKeyEnum.UpdateFileCacheAndEcgLoad, obj);
        //}



        private async Task SwitchFileMeasureValue(string arg)
        {
            var id = _cacheManager.TryGet<long>(CacheKeyEnum.CurrentAcquisitionFileKey);

            var maps = _cacheManager.TryGet<List<COAcquisitionFile>>(CacheKeyEnum.AcquisitionFileKey.ToString());

            //给测值赋值当前采集Id
            _cacheManager.TrySet<COMeasuredValue>(CacheKeyEnum.MeasuredValueKey.ToString(), maps.Single(x => x.Id == id).MeasuredValue);

            _messageModule.Send<string>(MessagerKeyEnum.SendOutMeasuredValue, "");

            await TaskEx.FromResult(0);
        }

        /// <summary>
        /// 接收消息处理导联变化
        /// </summary>
        /// <param name="obj"></param>
        private async Task UpdateLeadLayoutCache(int obj)
        {
            var id = _cacheManager.TryGet<long>(CacheKeyEnum.CurrentAcquisitionFileKey);

            var currentFile = _cacheManager.TryGet<List<COAcquisitionFile>>(CacheKeyEnum.AcquisitionFileKey.ToString())
                .Single(x => x.Id == id);

            currentFile.BitmapStyle = obj;

            await TaskEx.FromResult(0);
        }

        private async Task UpdateDisplayModeCache(int arg)
        {
            var id = _cacheManager.TryGet<long>(CacheKeyEnum.CurrentAcquisitionFileKey);

            var currentFile = _cacheManager.TryGet<List<COAcquisitionFile>>(CacheKeyEnum.AcquisitionFileKey.ToString())
                .Single(x => x.Id == id);

            currentFile.LeadMod = arg;

            await TaskEx.FromResult(0);
        }

        /// <summary>
        /// 接收消息处理波形滤波变化
        /// </summary>
        /// <param name="obj"></param>
        private async Task UpdateFilterChangeCache(Dictionary<ECGToolPopEnum, int> obj)
        {
            var id = _cacheManager.TryGet<long>(CacheKeyEnum.CurrentAcquisitionFileKey);

            var currentFile = _cacheManager.TryGet<List<COAcquisitionFile>>(CacheKeyEnum.AcquisitionFileKey.ToString())
                .Single(x => x.Id == id);

            switch (obj.Single().Key)
            {
                case ECGToolPopEnum.FilterBaseLine:
                    currentFile.BaseLine = obj.Single().Value;
                    break;
                case ECGToolPopEnum.FilterLowPass:
                    currentFile.LowPass = obj.Single().Value;
                    break;
                case ECGToolPopEnum.FilterNotch:
                    currentFile.Notch = obj.Single().Value;
                    break;
                default:
                    break;
            }

            await TaskEx.FromResult(0);

        }

        /// <summary>
        /// 接收消息处理波形WaveGain，修改对应的缓存
        /// </summary>
        /// <param name="obj"></param>
        private async Task UpdateSwitchGainCache(WaveGainEnum obj)
        {
            var id = _cacheManager.TryGet<long>(CacheKeyEnum.CurrentAcquisitionFileKey);

            var currentFile = _cacheManager.TryGet<List<COAcquisitionFile>>(CacheKeyEnum.AcquisitionFileKey.ToString())
                .Single(x => x.Id == id);

            currentFile.WaveGain = (int)obj;

            await TaskEx.FromResult(0);
        }

        /// <summary>
        /// 接收消息处理波形移动和走速，修改对应的缓存
        /// </summary>
        /// <param name="obj"></param>
        private async Task UpdateWaveMoveCache(WaveDotXAxisLandingMessageObject obj)
        {
            var id = _cacheManager.TryGet<long>(CacheKeyEnum.CurrentAcquisitionFileKey);

            var currentFile = _cacheManager.TryGet<List<COAcquisitionFile>>(CacheKeyEnum.AcquisitionFileKey.ToString())
                .Single(x => x.Id == id);

            currentFile.WaveSpeed = (int)obj.PaperSpeed;
            currentFile.OffsetWaveDot = obj.WaveDotPosition;

            await TaskEx.FromResult(0);
        }
        /// <summary>
        ///  接收消息处理波形移动，修改对应的缓存
        /// </summary>
        /// <param name="obj"></param>
        private async Task UpdateWaveDotChangedCache(WaveDotPositionLandingMessageObject obj)
        {
            var id = _cacheManager.TryGet<long>(CacheKeyEnum.CurrentAcquisitionFileKey);

            var currentFile = _cacheManager.TryGet<List<COAcquisitionFile>>(CacheKeyEnum.AcquisitionFileKey.ToString())
                .Single(x => x.Id == id);

            currentFile.OffsetWaveDot = obj.WaveDotPosition;

            await TaskEx.FromResult(0);
        }

        public async Task<string> ProviderLoad(COAcquisitionFile file)
        {
            byte[] arrBytes;

            if (!File.Exists(file.LocalFileUrl))
            {
                throw new Exception("File not Exists");
            }

            using (FileStream fs = new FileStream(file.LocalFileUrl, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, FileOptions.Asynchronous))
            {
                arrBytes = new byte[fs.Length];
                await fs.ReadAsync(arrBytes, 0, arrBytes.Length);
            }

            var provider = _eCGProviderFactory.GetProvider<IWavePulseReader>();
            provider.Init(arrBytes, file.LocalFileUrl
                , (WaveSpeedEnum)file.WaveSpeed, (WaveGainEnum)file.WaveGain
                , (FilterBaseLineEnum)file.BaseLine, (FilterLowPassEnum)file.LowPass, (FilterNotchEnum)file.Notch
                , IocManagerInstance.ResolveType<IGraphParmNDPI>(), IocManagerInstance.ResolveType<IGraphDataConversionExtension>());

            //心搏信息
            //var beats = _beatInfoBusi.GetBeatInfo(file.LocalFileUrl);
            //_cacheManager.TrySet<List<BeatInfo>>(CacheKeyEnum.BeatInfoKey.ToString(), _mapping.IEnumerableMap<BOBeatInfo, BeatInfo>(beats).ToList());
            //_cacheManager.TrySet<List<BeatInterval>>(CacheKeyEnum.BeatIntervalKey.ToString(),
            //    _mapping.IEnumerableMap<BOBeatInterval, BeatInterval>(_beatInfoBusi.GenerateBeatInterval(beats, (int)provider.SamplePerSecond)).ToList());

            ///注销上一个 provider
            _cacheManager.TryGet<IWavePulseReader>(CacheKeyEnum.ECGBaseInfoKey)?.Dispose();

            _cacheManager.TrySet<IWavePulseReader>(CacheKeyEnum.ECGBaseInfoKey, provider);

            file.LeadTypes = provider.LeadTypes;

            string configKey = "";
            if (provider.LeadTypes.Length == 12)
            {
                configKey = "TwelveLead";
            }
            else if (provider.LeadTypes.Length == 15)
            {
                configKey = "FifteenLead";
            }
            else if (provider.LeadTypes.Length == 18)
            {
                configKey = "EighteenLead";
            }
            else
            {
                throw new Exception(nameof(ProviderLoad) + provider.LeadTypes.Length);
            }

            return configKey;
        }

        public void FileSetConfig(COAcquisitionFile file, List<BODoctorConfig> config)
        {
            file.WaveSpeed = (int)(WaveSpeedEnum)System.Enum.Parse(typeof(WaveSpeedEnum)
            , config.Single(x => x.ConfigKey == "WaveSpeed")?.ConfigValue ?? throw new InvalidOperationException());
            file.WaveGain = (int)(WaveGainEnum)System.Enum.Parse(typeof(WaveGainEnum)
            , config.Single(x => x.ConfigKey == "WaveGain")?.ConfigValue ?? throw new InvalidOperationException());
            file.BaseLine = (int)(FilterBaseLineEnum)System.Enum.Parse(typeof(FilterBaseLineEnum)
            , config.Single(x => x.ConfigKey == "FilterBaseLine")?.ConfigValue ?? throw new InvalidOperationException());
            file.LowPass = (int)(FilterLowPassEnum)System.Enum.Parse(typeof(FilterLowPassEnum)
            , config.Single(x => x.ConfigKey == "FilterLowPass")?.ConfigValue ?? throw new InvalidOperationException());
            file.Notch = (int)(FilterNotchEnum)System.Enum.Parse(typeof(FilterNotchEnum)
            , config.Single(x => x.ConfigKey == "FilterNotch")?.ConfigValue ?? throw new InvalidOperationException());
            file.IndependentLead = config.Single(x => x.ConfigKey == "LeadType1")?.ConfigValue
            + "," + config.Single(x => x.ConfigKey == "LeadType2")?.ConfigValue;
            file.LeadMod = (int)(WaveDisplayEnum)System.Enum.Parse(typeof(WaveDisplayEnum)
            , config.Single(x => x.ConfigKey == "WaveDisplay")?.ConfigValue ?? throw new InvalidOperationException());
        }

    }
}
