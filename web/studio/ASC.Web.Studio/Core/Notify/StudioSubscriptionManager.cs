using System;
using System.Collections.Generic;
using ASC.Web.Core.Subscriptions;

namespace ASC.Web.Studio.Core.Notify
{
    internal class StudioSubscriptionManager : ISubscriptionManager
    {
        private static StudioSubscriptionManager _instance = new StudioSubscriptionManager();

        public static StudioSubscriptionManager Instance
        {
            get { return _instance; }
        }

        private StudioSubscriptionManager()
        { }

        #region ISubscriptionManager Members

        public List<SubscriptionObject> GetSubscriptionObjects()
        {
            return new List<SubscriptionObject>();
        }

        public List<SubscriptionType> GetSubscriptionTypes()
        {
            var types = new List<SubscriptionType>();
            types.Add(new SubscriptionType()
            {
                ID = new Guid("{148B5E30-C81A-4ff8-B749-C46BAE340093}"),
                Name = Resources.Resource.WhatsNewSubscriptionName,
                NotifyAction = Constants.ActionSendWhatsNew,
                Single = true
            });

            var astype = new SubscriptionType()
            {
                ID = new Guid("{A4FFC01F-BDB5-450e-88C4-03FED17D67C5}"),
                Name = Resources.Resource.AdministratorNotifySenderTypeName,
                NotifyAction = Constants.ActionSendWhatsNew,
                Single = false
            };
            
            types.Add(astype);

            return types;
        }

        public ASC.Notify.Model.ISubscriptionProvider SubscriptionProvider
        {
            get { return StudioNotifyService.Instance.source.GetSubscriptionProvider(); }
        }

        #endregion
    }
}
