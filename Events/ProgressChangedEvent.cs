using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PageCompiler.Events
{
    public class ProgressChangedEvent : IProgressChangedEvent
    {
        public int Index { get; set; }
        public int PageCount { get; set; }
        public string Message { get; set; }
        public Guid TaskId { get; set; }
        public ProgressCode Code { get; set; }
        public string Origin { get; set; }
    }

    public enum ProgressCode
    {
        Status,
        General,
        Error,
        Restart,
        Stop
    }
}
