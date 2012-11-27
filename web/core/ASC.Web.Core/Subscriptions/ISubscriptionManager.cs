using System.Collections.Generic;
using ASC.Notify.Model;


namespace ASC.Web.Core.Subscriptions
{   

    public interface ISubscriptionManager
    {
        List<SubscriptionObject> GetSubscriptionObjects();

        List<SubscriptionType> GetSubscriptionTypes();

        ISubscriptionProvider SubscriptionProvider { get; }        
    }
}
