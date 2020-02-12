using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NL.AI.ToolDemo.Model
{
    public class QueryList<T>
    {
        public IList<T> Data { get; set; }
        public int ItemsCount { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
    }
}
