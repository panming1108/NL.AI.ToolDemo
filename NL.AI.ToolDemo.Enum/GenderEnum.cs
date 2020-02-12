using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace NL.AI.ToolDemo.Enum
{
    public enum GenderEnum
    {
        [Description("未说明")]
        UnSign = 0,
        [Description("男")]
        Male = 1,
        [Description("女")]
        Female = 2,
        [Description("未知")]
        Unknown = 9
    }
}
