using System.Linq;
using ASC.Notify.Engine;
using ASC.Notify.Recipients;

namespace ASC.Notify
{
    public class InitiatorInterceptor : SendInterceptorSkeleton
    {
        public InitiatorInterceptor(params IRecipient[] initiators)
            : base("Sys.InitiatorInterceptor", InterceptorPlace.GroupSend | InterceptorPlace.DirectSend, InterceptorLifetime.Call,
                (r, p) => (initiators ?? Enumerable.Empty<IRecipient>()).Any(recipient => r.Recipient.Equals(recipient)))
        {
        }
    }
}