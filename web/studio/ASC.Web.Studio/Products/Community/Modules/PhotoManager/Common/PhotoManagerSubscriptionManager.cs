using System;
using System.Collections.Generic;
using System.Web;
using ASC.Core;
using ASC.Notify.Model;
using ASC.PhotoManager.Data;
using ASC.PhotoManager.Model;
using ASC.PhotoManager.Resources;
using ASC.Web.Core.Subscriptions;
using ASC.Notify.Recipients;

namespace ASC.Web.Community.PhotoManager
{
    public class PhotoManagerSubscriptionManager : ISubscriptionManager
    {      
        private Guid _photoSubscriptionTypeID = new Guid("{E5BCAC27-C11B-4f75-9631-BCAA0ABA861F}");
        private Guid _commentSubscriptionTypeID = new Guid("{27AE9A2C-490F-426e-AA40-660FBA7DDE9D}");

        private List<SubscriptionObject> GetSubscriptionObjectsByType(Guid productID, Guid moduleID, Guid typeID)
        {           

            var storage = StorageFactory.GetStorage();

            List<SubscriptionObject> subscriptionObjects = new List<SubscriptionObject>();
            ISubscriptionProvider subscriptionProvider = storage.NotifySource.GetSubscriptionProvider();
                
            if (typeID.Equals(_photoSubscriptionTypeID))
            {
                
                List<string> userList = new List<string>(
                                                    subscriptionProvider.GetSubscriptions(
                                                            ASC.PhotoManager.PhotoConst.NewPhotoUploaded,
                                                            storage.NotifySource.GetRecipientsProvider().GetRecipient(SecurityContext.CurrentAccount.ID.ToString()))
                                                            );

                if (userList.Contains(null))
                {
                    subscriptionObjects.Add(new SubscriptionObject()
                    {
                        ID = new Guid(ASC.PhotoManager.PhotoConst._NewPhotoSubscribeCategory).ToString(),
                        Name = PhotoManagerResource.NotifyOnUploadsTitle,
                        URL = string.Empty,
                        SubscriptionType = GetSubscriptionTypes().Find(st => st.ID.Equals(_photoSubscriptionTypeID))
                    });
                }
            }
            else if (typeID.Equals(_commentSubscriptionTypeID))
            {
                List<string> eventsList = new List<string>(
                                                subscriptionProvider.GetSubscriptions(
                                                        ASC.PhotoManager.PhotoConst.NewEventComment,
                                                        storage.NotifySource.GetRecipientsProvider().GetRecipient(SecurityContext.CurrentAccount.ID.ToString()))
                                                        );
                if (eventsList.Count > 0)
                {
                    var events = storage.GetEvents(0, int.MaxValue);

                    foreach (Event eve in eventsList.ConvertAll<Event>((id) => events.Find((_event) => _event.Id.ToString() == id)))
                    {
                        if (eve == null) continue;

                        subscriptionObjects.Add(new SubscriptionObject()
                        {
                            ID = eve.Id.ToString(),
                            Name = eve.Name,
                            URL = VirtualPathUtility.ToAbsolute("~/products/community/modules/photomanager/default.aspx") + "?" + ASC.PhotoManager.PhotoConst.PARAM_EVENT + "=" + eve.Id.ToString(),
                            SubscriptionType = GetSubscriptionTypes().Find(st => st.ID.Equals(_commentSubscriptionTypeID))
                        });
                    }
                }
            }
            return subscriptionObjects;
        }

        private bool IsEmptySubscriptionType(Guid productID, Guid moduleID, Guid typeID)
        {
            var type = GetSubscriptionTypes().Find(t => t.ID.Equals(typeID));

            var objIDs = SubscriptionProvider.GetSubscriptions(type.NotifyAction, new DirectRecipient(SecurityContext.CurrentAccount.ID.ToString(), ""));
            if (objIDs != null && objIDs.Length > 0)
                return false;

            return true;
        }

        private INotifyAction GetNotifyActionBySubscriptionType(ASC.PhotoManager.SubscriptionType subscriptionType)
        {
            switch (subscriptionType)
            {
                case ASC.PhotoManager.SubscriptionType.NewComment:
                    return ASC.PhotoManager.PhotoConst.NewEventComment;

                case ASC.PhotoManager.SubscriptionType.NewPhoto:
                    return ASC.PhotoManager.PhotoConst.NewPhotoUploaded;

            }
            return null;
        }

		private ASC.PhotoManager.SubscriptionType GetPhotoManagerSubscriptionType(Guid subscriptionTypeID)
        {
            if (subscriptionTypeID.Equals(_photoSubscriptionTypeID))
                return ASC.PhotoManager.SubscriptionType.NewPhoto;

            else if (subscriptionTypeID.Equals(_commentSubscriptionTypeID))
                return ASC.PhotoManager.SubscriptionType.NewComment;

            return ASC.PhotoManager.SubscriptionType.NewPhoto;
        }

        #region ISubscriptionManager Members

        public List<SubscriptionObject> GetSubscriptionObjects()
        {  
            List<SubscriptionObject> subscriptionObjects = new List<SubscriptionObject>();
            var storage = StorageFactory.GetStorage();
            
            ISubscriptionProvider subscriptionProvider = storage.NotifySource.GetSubscriptionProvider();
            List<string> userList = new List<string>(
                                                subscriptionProvider.GetSubscriptions(
                                                        ASC.PhotoManager.PhotoConst.NewPhotoUploaded,
                                                        storage.NotifySource.GetRecipientsProvider().GetRecipient(SecurityContext.CurrentAccount.ID.ToString()))
                                                        );
            
            if(userList.Contains(null))
            {
                subscriptionObjects.Add(new SubscriptionObject()
                    {
                        ID = new Guid(ASC.PhotoManager.PhotoConst._NewPhotoSubscribeCategory).ToString(),
                        Name = PhotoManagerResource.NotifyOnUploadsTitle,
                        URL = string.Empty,
                        SubscriptionType = GetSubscriptionTypes().Find(st => st.ID.Equals(_photoSubscriptionTypeID))
                    });
            }

            List<string> eventsList = new List<string>(
                                                subscriptionProvider.GetSubscriptions(
                                                        ASC.PhotoManager.PhotoConst.NewEventComment,
                                                        storage.NotifySource.GetRecipientsProvider().GetRecipient(SecurityContext.CurrentAccount.ID.ToString()))
                                                        );
            if (eventsList.Count > 0)
            {
                var events = storage.GetEvents(0, int.MaxValue);

                foreach (Event eve in eventsList.ConvertAll<Event>((id) => events.Find((_event)=>_event.Id.ToString() == id)))
                {
                    if (eve == null) continue;

                    subscriptionObjects.Add(new SubscriptionObject()
                    {
                        ID = eve.Id.ToString(),
                        Name = eve.Name,
                        URL = VirtualPathUtility.ToAbsolute("~/products/community/modules/photomanager/default.aspx") + "?" + ASC.PhotoManager.PhotoConst.PARAM_EVENT + "=" + eve.Id.ToString(),
                        SubscriptionType = GetSubscriptionTypes().Find(st => st.ID.Equals(_commentSubscriptionTypeID))
                    });
                }
            }
            return subscriptionObjects;
            
        }

        public List<SubscriptionType> GetSubscriptionTypes()
        {
            var subscriptionTypes = new List<SubscriptionType>();

            subscriptionTypes.Add(new SubscriptionType()
            {
                ID = _photoSubscriptionTypeID,
                Name = PhotoManagerResource.NotifyOnUploadsTitle,
                NotifyAction = ASC.PhotoManager.PhotoConst.NewPhotoUploaded,
                Single = true,
                IsEmptySubscriptionType = new IsEmptySubscriptionTypeDelegate(IsEmptySubscriptionType)
            });

            subscriptionTypes.Add(new SubscriptionType()
            {
                ID = _commentSubscriptionTypeID,
                Name = PhotoManagerResource.NotifyOnNewEventCommentsTitle,
                NotifyAction = ASC.PhotoManager.PhotoConst.NewEventComment,
                GetSubscriptionObjects = new GetSubscriptionObjectsDelegate(GetSubscriptionObjectsByType),
                IsEmptySubscriptionType = new IsEmptySubscriptionTypeDelegate(IsEmptySubscriptionType)
            });

            return subscriptionTypes;
        }

        public void UnsubscribeForObject(string subscriptionObjectID, Guid subscriptionTypeID)
        {
            var storage = StorageFactory.GetStorage();
            ISubscriptionProvider subscriptionProvider = storage.NotifySource.GetSubscriptionProvider();
            if (subscriptionTypeID == _photoSubscriptionTypeID)
            {
                subscriptionProvider.UnSubscribe(
                         ASC.PhotoManager.PhotoConst.NewPhotoUploaded,  
                         null,                                          
                         storage.NotifySource.GetRecipientsProvider().
                         GetRecipient(SecurityContext.CurrentAccount.
                         ID.ToString())                                 
                    );
            }
            else if (subscriptionTypeID == _commentSubscriptionTypeID)
            {
                subscriptionProvider.UnSubscribe(
                         ASC.PhotoManager.PhotoConst.NewEventComment,   
                         subscriptionObjectID,                          
                         storage.NotifySource.GetRecipientsProvider().
                         GetRecipient(SecurityContext.CurrentAccount.
                         ID.ToString())                                 
                    );
            }
        }

        public void UnsubscribeForType(Guid subscriptionTypeID)
        {

        }

		
		public ISubscriptionProvider SubscriptionProvider
        {
            get 
            {
                var storage = StorageFactory.GetStorage();
                return storage.NotifySource.GetSubscriptionProvider();
            }
        }

        #endregion

	}
}
