using ASC.Web.Core.Subscriptions;

namespace ASC.Web.Community.Product
{
    public class CommunitySubscriptionManager : IProductSubscriptionManager
    {
        #region IProductSubscriptionManager Members

        public System.Collections.Generic.List<SubscriptionGroup> GetSubscriptionGroups()
        {
            return new System.Collections.Generic.List<SubscriptionGroup>();
        }

        public GroupByType GroupByType
        {
            get { return GroupByType.Modules; }
        }

        #endregion

        #region ISubscriptionManager Members

        public System.Collections.Generic.List<SubscriptionObject> GetSubscriptionObjects()
        {
            return null;
        }

        public System.Collections.Generic.List<SubscriptionType> GetSubscriptionTypes()
        {
            return null;
        }

        public ASC.Notify.Model.ISubscriptionProvider SubscriptionProvider
        {
            get { return null; }
        }

        #endregion

    }
}
