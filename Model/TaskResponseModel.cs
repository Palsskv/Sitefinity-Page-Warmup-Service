using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PageCompiler.Model
{
    public class TaskResponseModel
    {
        public string Message { get; set; }

        public string TaskKey { get; set; }

        public int TaskProgress { get; set; }

        public int PagesCount { get; set; }

        public int CurrentPage { get; set; }
    }
}
