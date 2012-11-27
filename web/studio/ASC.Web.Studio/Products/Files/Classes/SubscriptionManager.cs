using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ASC.Core;
using ASC.Notify.Model;
using ASC.Web.Core.Subscriptions;
using ASC.Web.Files.Services.NotifyService;

namespace ASC.Web.Files.Classes
{
    public class SubscriptionManager : IProductSubscriptionManager
    {
        private readonly Guid subscrTypeUpdateDoc = new Guid("{EAC597BD-9C2A-4470-898E-FE105C120222}");
        private readonly Guid subscrTypeShareDoc = new Guid("{552846EC-AC94-4408-AAC6-17C8989B8B38}");
        private readonly Guid subscrGroupsUpdateDoc = new Guid("{45272EA9-3AB3-4ab4-B118-057B783DEFE7}");

        
        private bool IsEmptySubscriptionShare(Guid productID, Guid moduleID, Guid typeID)
        {
            return !productID.Equals(Configuration.ProductEntryPoint.ID) || !moduleID.Equals(subscrGroupsUpdateDoc) || !typeID.Equals(subscrTypeShareDoc);
        }

        private bool IsEmptySubscriptionUpdate(Guid productID, Guid moduleID, Guid typeID)
        {
            var objIDs = GetFilesShare(productID, moduleID, typeID);
            return objIDs == null || !objIDs.Any();
        }

        private List<SubscriptionObject> GetFilesShare(Guid productID, Guid moduleID, Guid typeID)
        {
            if (!productID.Equals(Configuration.ProductEntryPoint.ID) || !moduleID.Equals(subscrGroupsUpdateDoc) || !typeID.Equals(subscrTypeUpdateDoc))
                return null;

            var groupFollow = GetSubscriptionGroups().Find(r => r.ID.Equals(moduleID));
            var typeFollow = GetSubscriptionTypes().Find(r => r.ID.Equals(typeID));
            var subscriptionObjects = new List<SubscriptionObject>();

            using (var fileDao = Global.DaoFactory.GetFileDao())
            {
                var filesId = new List<object>();

                NotifySource.Instance.GetSubscriptionProvider()
                    .GetSubscriptions(NotifyConstants.Event_UpdateDocument, NotifySource.Instance.GetRecipientsProvider().GetRecipient(SecurityContext.CurrentAccount.ID.ToString()))
                    .ToList().ForEach(
                        id =>
                        {
                            if (id.StartsWith("file_"))
                                filesId.Add(id.Substring("file_".Length));
                        }
                    );

                Global.GetFilesSecurity().FilterRead(fileDao.GetFiles(filesId.ToArray()))
                    .ToList()
                    .ForEach(file => subscriptionObjects.Add(new SubscriptionObject
                                                                 {
                                                                     ID = file.UniqID,
                                                                     Name = HttpUtility.HtmlDecode(file.Title),
                                                                     URL = file.ViewUrl,
                                                                     SubscriptionGroup = groupFollow,
                                                                     SubscriptionType = typeFollow
                                                                 })
                    );
            }

            return subscriptionObjects;

        }


        public GroupByType GroupByType
        {
            get { return GroupByType.Groups; }
        }

        public List<SubscriptionObject> GetSubscriptionObjects()
        {
            return GetFilesShare(Configuration.ProductEntryPoint.ID, subscrGroupsUpdateDoc, subscrTypeUpdateDoc);
        }

        public List<SubscriptionType> GetSubscriptionTypes()
        {
            return new List<SubscriptionType>
                                    {
                                        new SubscriptionType
                                            {
                                                ID = subscrTypeShareDoc,
                                                Name = Resources.FilesCommonResource.SubscriptForAccess,
                                                NotifyAction = NotifyConstants.Event_ShareDocument,
                                                Single = true,
                                                CanSubscribe = true,
                                                IsEmptySubscriptionType = IsEmptySubscriptionShare
                                            },
                                        new SubscriptionType
                                            {
                                                ID = subscrTypeUpdateDoc,
                                                Name = Resources.FilesCommonResource.SubscriptForUpdate,
                                                NotifyAction = NotifyConstants.Event_UpdateDocument,
                                                Single = false,
                                                IsEmptySubscriptionType = IsEmptySubscriptionUpdate,
                                                GetSubscriptionObjects = GetFilesShare
                                            }
                                    };
        }

        public ISubscriptionProvider SubscriptionProvider
        {
            get { return NotifySource.Instance.GetSubscriptionProvider(); }
        }

        public List<SubscriptionGroup> GetSubscriptionGroups()
        {
            return new List<SubscriptionGroup>
            {
                new SubscriptionGroup { ID = subscrGroupsUpdateDoc, Name = Resources.FilesCommonResource.SharedForMe }
            };
        }
    }
}