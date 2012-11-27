using System;
using System.Collections.Generic;
using ASC.Notify.Recipients;

namespace ASC.Notify.Engine
{
    class SingleRecipientInterceptor : ISendInterceptor
    {
        private const string prefix = "__singlerecipientinterceptor";
        private readonly List<IRecipient> sendedTo = new List<IRecipient>(10);


        public string Name { get; private set; }

        public InterceptorPlace PreventPlace { get { return InterceptorPlace.GroupSend | InterceptorPlace.DirectSend; } }

        public InterceptorLifetime Lifetime { get { return InterceptorLifetime.Call; } }


        internal SingleRecipientInterceptor(string name)
        {
            if (String.IsNullOrEmpty(name)) throw new ArgumentException("name");
            Name = name;
        }

        public bool PreventSend(NotifyRequest request, InterceptorPlace place)
        {
            var sendTo = request.Recipient;
            if (!sendedTo.Exists(rec => Equals(rec, sendTo)))
            {
                sendedTo.Add(sendTo);
                return false;
            }
            return true;
        }
    }
}