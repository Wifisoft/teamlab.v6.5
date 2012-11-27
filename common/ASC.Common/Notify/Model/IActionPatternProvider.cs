using System;
using ASC.Notify.Engine;
using ASC.Notify.Patterns;

namespace ASC.Notify.Model
{
    public interface IActionPatternProvider
    {
        Func<INotifyAction, string, NotifyRequest, IPattern> GetPatternMethod { get; set; }

        IPattern GetPattern(INotifyAction action, string senderName);

        IPattern GetPattern(INotifyAction action);
    }
}