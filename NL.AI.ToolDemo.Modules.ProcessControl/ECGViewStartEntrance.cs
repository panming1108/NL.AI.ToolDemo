using NL.CardioReader.MidEnd.VM.KeyEnum;
using NL.CardioReader.VoidPower.VMModule.IF;
using NL.SkyCastle.Infra.ITrailer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NL.AI.ToolDemo.Modules.ProcessControl
{
    public class ECGViewStartEntrance : IECGViewStartEntrance
    {
        private readonly IMessageModule _messageModule;
        private readonly ICacheManager _cacheManager;
        private readonly ISkyCastleTrailer _skyCastleTrailer;

        public ECGViewStartEntrance(IMessageModule iMessageModule, ICacheManager iCacheManager, ISkyCastleTrailer skyCastleTrailer)
        {
            _messageModule = iMessageModule;
            _cacheManager = iCacheManager;
            _skyCastleTrailer = skyCastleTrailer;

            _messageModule.Register<string>(this, MessagerKeyEnum.StartDiagModule, StartDiag);
        }

        private async Task StartDiag(string diagModel)
        {
            _messageModule.Send(MessagerKeyEnum.PageNavi, NaviKeyEnum.DiagnosisStation);
            _messageModule.Send<LoadingStatusEnum>(MessagerKeyEnum.Loading, LoadingStatusEnum.IsBusy);

            ///发送消息给各个模块
            _messageModule.Send<string>(MessagerKeyEnum.ControlLoadData, "");

            _messageModule.Send<LoadingStatusEnum>(MessagerKeyEnum.Loading, LoadingStatusEnum.NotBusy);

            await TaskEx.FromResult(0);
        }

        public void Dispose()
        {
            _messageModule.Unregister<string>(this, MessagerKeyEnum.StartDiagModule, StartDiag);
        }
    }
}
