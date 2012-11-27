using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ASC.Core.Common;
using ASC.Core.Notify.Jabber;
using ASC.Notify.Messages;

namespace ASC.Notify
{
    class JabberAgregator
    {
        private JabberServiceClient jsc = new JabberServiceClient();
        public JabberAgregator()
        {
        }

        internal void SendByJabber(NotifyMessage m)
        {
            jsc.SendMessage(m.To, m.Subject, m.Content, m.Tenant);
        }
    }
}
