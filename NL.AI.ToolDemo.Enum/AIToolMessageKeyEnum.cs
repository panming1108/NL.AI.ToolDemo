using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace NL.AI.ToolDemo.Enum
{
    public class AIToolMessageKeyEnum
    {
        [Description("进程间消息")]
        public static readonly string ProcessMessage = nameof(ProcessMessage);

        [Description("导入ANB文件")]
        public static readonly string ImportFile = nameof(ImportFile);

        [Description("刷新数据列表")]
        public static readonly string FreshDataList = nameof(FreshDataList);
    }
}
