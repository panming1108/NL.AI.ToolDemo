using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NL.AI.ToolDemo.Model
{
    public class MenuNode
    {
        public string Icon { get; set; }

        public string Name { get; set; }

        public string InputGestureText { get; set; }

        public List<MenuNode> Children { get; set; }

        public string GroupName { get; set; }
        public bool IsParent { get; set; } = false;
        public string SelectedIcon { get; set; }
    }
}
