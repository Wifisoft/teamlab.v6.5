using System;
using ASC.Notify.Model;
using System.Collections.Generic;

namespace ASC.Forum
{
    public interface ISubscriptionGetcherView
    {
        IList<object> SubscriptionObjects { get; set; }

        event EventHandler<SubscriptionEventArgs> GetSubscriptionObjects;
    }

    public class SubscriptionEventArgs: EventArgs
    {
        public Guid UserID { get; private set; }

        public int TenantID { get; private set; }

        public INotifyAction NotifyAction { get; private set; }

        public SubscriptionEventArgs(INotifyAction notifyAction, Guid userID, int tenantID)
        {
            this.NotifyAction = notifyAction;         
            this.UserID = userID;
            this.TenantID = tenantID;
        }
    }
}
