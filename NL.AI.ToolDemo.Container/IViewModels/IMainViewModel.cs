using NL.CardioReader.VoidPower.VVM.IF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace NL.AI.ToolDemo.Container.IViewModels
{
    public interface IMainViewModel : IViewModel
    {
        ICommand MinCommand { get; set; }
        ICommand CloseCommand { get; set; }
    }
}
