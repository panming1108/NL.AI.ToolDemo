using NL.CardioReader.VoidPower.VOnly.IF;
using NL.CardioReader.VoidPower.VVMModule.IF;
using NL.CardioReader.VoidPower.WPFContainer;
using NL.GradationProjection.ModalWindow.IF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NL.AI.ToolDemo.Container
{
    public class DialogFactoryConsumerEx : DialogFactoryConsumer
    {
        public override T GetDialog<T>()
        {
            var a = typeof(T).FullName;

            if (a == typeof(IMessageDialog).FullName)
            {
                return GetMessage<T>();
            }
            else if (a == typeof(IConfirmDialog).FullName)
            {
                var confirm = IocManagerInstance.ResolveType<IConfirmDialogBase>();
                confirm.WindowOwner = _owner;
                return (T)confirm;
            }           
            else
            {
                throw new Exception();
            }
        }
    }
}
