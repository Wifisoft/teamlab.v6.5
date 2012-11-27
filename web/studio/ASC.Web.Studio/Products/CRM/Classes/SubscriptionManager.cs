#region Import

using System;
using System.Collections.Generic;
using System.Web;
using ASC.Data.Storage;
using ASC.Notify.Model;
using ASC.Web.CRM.Resources;
using ASC.Web.CRM.Services.NotifyService;
using ASC.Web.Core.Subscriptions;
using ASC.Web.Core.Utility.Skins;

#endregion

namespace ASC.Web.CRM
{

    public class ProductSubscriptionManager : IProductSubscriptionManager
    {

        private readonly Guid _setAccess = new Guid("{D4D58C55-D32E-41dc-9D22-D123AFAFC7E7}");
        private readonly Guid _responsibleForTask = new Guid("{2479B115-EAEB-4d9a-86DA-51BD708DEFDC}");
        private readonly Guid _addRelationshipEvent = new Guid("{4E16DBC5-A427-469e-9EF7-A8DEA0F61310}");
        private readonly Guid _exportCompleted = new Guid("{88D3DC5E-3E46-46a1-9FEF-6B8FFF020BA4}");
        private readonly Guid _createNewContact = new Guid("{ADAC1E70-4163-41c1-8968-67A44E4D24E7}");
  
        public List<SubscriptionObject> GetSubscriptionObjects()
        {
            return new List<SubscriptionObject>();
        }

        public List<SubscriptionType> GetSubscriptionTypes()
        {
            return new List<SubscriptionType>
                       {
                           new SubscriptionType
                           {
                               ID = _setAccess,
                               Name = CRMCommonResource.SubscriptionType_SetAccess,
                               NotifyAction = NotifyConstants.Event_SetAccess,
                               Single = true,
                               CanSubscribe = true
                           },
                           new SubscriptionType
                               {
                                   ID = _responsibleForTask,
                                   Name = CRMCommonResource.SubscriptionType_ResponsibleForTask,
                                   NotifyAction = NotifyConstants.Event_ResponsibleForTask,
                                   Single = true,
                                   CanSubscribe = true
                               },
                           new SubscriptionType
                           {
                               ID = _addRelationshipEvent,
                               Name = CRMCommonResource.SubscriptionType_AddRelationshipEvent,
                               NotifyAction = NotifyConstants.Event_AddRelationshipEvent,
                               Single = true,
                               CanSubscribe = true
                           },
                            new SubscriptionType
                           {
                               ID = _exportCompleted,
                               Name = CRMCommonResource.SubscriptionType_ExportCompleted,
                               NotifyAction = NotifyConstants.Event_ExportCompleted,
                               Single = true,
                               CanSubscribe = true
                           },
                            new SubscriptionType
                           {
                               ID = _createNewContact,
                               Name = CRMCommonResource.SubscriptionType_CreateNewContact,
                               NotifyAction = NotifyConstants.Event_CreateNewContact,
                               Single = true,
                               CanSubscribe = true
                           }
                       };
        }

        public ISubscriptionProvider SubscriptionProvider
        {
            get { return NotifySource.Instance.GetSubscriptionProvider(); }
        }

        public GroupByType GroupByType
        {
            get { return GroupByType.Simple; }
        }

        public List<SubscriptionGroup> GetSubscriptionGroups()
        {
            return  new List<SubscriptionGroup>();
        }
    }

}