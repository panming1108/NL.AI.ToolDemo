using NL.AmuletHeart.ECGL.IF;
using NL.CardioReader.MidEnd.ECGProvider;
using NL.CardioReader.VoidPower.VOnly.IF;
using NL.SkyCastle.Infra.ISerialize;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NL.AI.ToolDemo.Container
{
    public class ECGProviderFactory : IECGProviderFactory
    {
        public T GetProvider<T>()
        {
            var a = typeof(T).FullName;

            if (a == typeof(IWavePulseReader).FullName)
            {
                IWavePulseReader reader = new WavePulseReader(IocManagerInstance.ResolveType<IJsonSerializer>(), IocManagerInstance.ResolveType<IWaveMoveCheck>());
                return (T)reader;
            }
            else if (a == typeof(IWavePluseProvider).FullName)
            {
                IWavePluseProvider provider = new WavePulseProvider();
                return (T)provider;
            }
            //else if (a == typeof(IAnalysisProvider).FullName)
            //{
            //    IAnalysisProvider ana = new AnalysisProvider(IocManagerInstance.ResolveType<IMapping>(), IocManagerInstance.ResolveType<IJsonSerializer>());
            //    return (T)ana;
            //}
            else
            {
                throw new Exception();
            }
        }
    }
}
