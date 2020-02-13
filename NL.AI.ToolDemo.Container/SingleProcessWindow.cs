using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Interop;

namespace NL.AI.ToolDemo.Container
{
    public class SingleProcessWindow : Window
    {
        public void InitWindowSize()
        {
            Rect rc = SystemParameters.WorkArea;//获取工作区大小
            this.Left = 0;//设置位置
            this.Top = 0;
            this.Width = rc.Width;
            this.Height = rc.Height;
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            HwndSource sorce = PresentationSource.FromVisual(this) as HwndSource;
            sorce.AddHook(new HwndSourceHook(ProcessMessageHelper.WndProc));
        }
    }
}
