using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.Sitefinity.Services.Events;

namespace PageCompiler.Events
{
    public interface IProgressChangedEvent : IEvent
    {
        int Index { get; set;  }
        ProgressCode Code { get; set; }
        string Message { get; set; }
        Guid TaskId { get; set; }
    }
}
