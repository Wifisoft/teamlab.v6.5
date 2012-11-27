using System;
using ASC.Xmpp.Server.Authorization;
using ASC.Xmpp.Server.Gateway;
using ASC.Xmpp.Server.Session;
using ASC.Xmpp.Server.Storage;
using ASC.Xmpp.Server.Users;

namespace ASC.Xmpp.Server.Handler
{
    public class XmppHandlerContext
    {
        public IServiceProvider ServiceProvider
        {
            get;
            private set;
        }

        public IXmppSender Sender
        {
            get { return (IXmppSender)ServiceProvider.GetService(typeof(IXmppSender)); }
        }

        public UserManager UserManager
        {
            get { return (UserManager)ServiceProvider.GetService(typeof(UserManager)); }
        }

        public XmppSessionManager SessionManager
        {
            get { return (XmppSessionManager)ServiceProvider.GetService(typeof(XmppSessionManager)); }
        }

        public StorageManager StorageManager
        {
            get { return (StorageManager)ServiceProvider.GetService(typeof(StorageManager)); }
        }

        public AuthManager AuthManager
        {
            get { return (AuthManager)ServiceProvider.GetService(typeof(AuthManager)); }
        }

        public XmppHandlerContext(IServiceProvider serviceProvider)
        {
            if (serviceProvider == null) throw new ArgumentNullException("serviceProvider");

            ServiceProvider = serviceProvider;
        }
    }
}
