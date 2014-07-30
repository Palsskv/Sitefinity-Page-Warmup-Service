using PageCompiler.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PageCompiler
{
    public class CompilationTaskRunner
    {
        public static void RunTask()
        {
            var service = new PageCompilationService();
            service.ClearTasks();
            service.StopTasks();
            service.Start(string.Empty, true);
        }


    }
}
