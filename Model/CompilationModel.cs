using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PageCompiler.Model
{
    public class CompilationModel
    {
        public string StartNodeUrl { get; set; }

        public bool IsRootNode { get; set; }

        public int PageCount { get; set; }

        public int CurrentIndex { get; set; }
    }
}
