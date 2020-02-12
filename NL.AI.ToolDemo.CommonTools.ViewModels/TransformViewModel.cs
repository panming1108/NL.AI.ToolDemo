using NL.AI.ToolDemo.CommonTools.IViewModels;
using NL.CardioReader.VoidPower.VMModule.IF;
using NL.CardioReader.VoidPower.VMOnly.BaseLib;
using NL.CardioReader.VoidPower.VMOnly.IF;
using NL.SkyCastle.Infra.ITrailer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NL.AI.ToolDemo.CommonTools.ViewModels
{
    public class TransformViewModel : ViewModelBase, ITransformViewModel
    {
        public TransformViewModel(IMessageModule iMessageModule, ICacheManager iCacheManager, ISkyCastleTrailer skyCastleTrailer, IDialogFactory dialogFactory, ICommandFactory commandFactory) : base(iMessageModule, iCacheManager, skyCastleTrailer, dialogFactory, commandFactory)
        {
        }

        protected override Task Loaded(object o)
        {
            throw new NotImplementedException();
        }

        protected override Task UnLoaded(object o)
        {
            throw new NotImplementedException();
        }
    }
}
