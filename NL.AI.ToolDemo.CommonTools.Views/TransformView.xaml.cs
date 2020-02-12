using NL.AI.ToolDemo.CommonTools.IViewModels;
using NL.CardioReader.VoidPower.VOnly.IF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NL.AI.ToolDemo.CommonTools.Views
{
    /// <summary>
    /// TransformView.xaml 的交互逻辑
    /// </summary>
    public partial class TransformView : UserControl
    {
        public TransformView()
        {
            InitializeComponent();
            DataContext = IocManagerInstance.ResolveType<ITransformViewModel>();
        }
    }
}
