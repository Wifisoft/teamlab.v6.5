#region Import

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ASC.CRM.Core.Entities;
using ASC.Core;
using ASC.Core.Users;
using ASC.Notify;
using ASC.Notify.Model;
using ASC.Notify.Patterns;
using ASC.Notify.Recipients;
using ASC.Core.Tenants;
using ASC.Web.CRM.Classes;
using ASC.CRM.Core;
using System.Collections.Specialized;
using ASC.Web.CRM.Resources;

#endregion

namespace ASC.Web.CRM.Services.NotifyService
{
    public class NotifyClient
    {

        private static NotifyClient instance;
        private readonly INotifyClient client;
        private readonly INotifySource source;

        public static NotifyClient Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (typeof(NotifyClient))
                    {
                        if (instance == null) instance = new NotifyClient(WorkContext.NotifyContext.NotifyService.RegisterClient(NotifySource.Instance), NotifySource.Instance);
                    }
                }
                return instance;
            }
        }

        public void SendAboutCreateNewContact(List<Guid> recipientID, int contactID, String contactTitle, NameValueCollection fields)
        {
            if ((recipientID.Count == 0) || String.IsNullOrEmpty(contactTitle)) return;

            client.SendNoticeToAsync(
                NotifyConstants.Event_CreateNewContact,
                null,
                recipientID.ConvertAll(item => ToRecipient(item)).ToArray(),
                true,
                new TagValue(NotifyConstants.Tag_AdditionalData, fields),
                new TagValue(NotifyConstants.Tag_EntityTitle, contactTitle),
                new TagValue(NotifyConstants.Tag_EntityID, contactID)
             );

        }

        public void SendAboutSetAccess(EntityType entityType, int entityID, params Guid[] userID)
        {
            if (userID.Length == 0) return;

            var baseData = ExtractBaseDataFrom(entityType, entityID);

            client.SendNoticeToAsync(
                   NotifyConstants.Event_SetAccess,
                   String.Empty,
                   userID.Select(item => ToRecipient(item)).ToArray(),
                   true,
                   new TagValue(NotifyConstants.Tag_EntityID, baseData["id"]),
                   new TagValue(NotifyConstants.Tag_EntityTitle, baseData["title"]),
                   new TagValue(NotifyConstants.Tag_EntityRelativeURL, baseData["entityRelativeURL"])
                );
        }

        private NameValueCollection ExtractBaseDataFrom(EntityType entityType, int entityID)
        {

            var result = new NameValueCollection();

            String title;
            String relativeURL;

            switch (entityType)
            {
                case EntityType.Person:
                case EntityType.Company:
                case EntityType.Contact:
                    {
                        var contact = Global.DaoFactory.GetContactDao().GetByID(entityID);
                        title = contact != null ? contact.GetTitle() : string.Empty;
                        relativeURL = "default.aspx?id=" + entityID;
                        break;
                    }
                case EntityType.Opportunity:
                    {
                        var deal = Global.DaoFactory.GetDealDao().GetByID(entityID);
                        title = deal != null ? deal.Title : string.Empty;
                        relativeURL = "deals.aspx?id=" + entityID;
                        break;
                    }
                case EntityType.Case:
                    {
                        var cases = Global.DaoFactory.GetCasesDao().GetByID(entityID);
                        title = cases != null ? cases.Title : string.Empty;
                        relativeURL = "cases.aspx?id=" + entityID;
                        break;
                    }

                default:
                    throw new ArgumentException();
            }

            result.Add("title", title);
            result.Add("id", entityID.ToString());
            result.Add("entityRelativeURL", String.Concat(PathProvider.BaseAbsolutePath, relativeURL));

            return result;
        }

        public void SendAboutAddRelationshipEventAdd(RelationshipEvent entity,
                                                    Hashtable fileListInfoHashtable, params Guid[] userID)
        {
            if (userID.Length == 0) return;

            NameValueCollection baseEntityData;

            if (entity.EntityID != 0)
            {
                baseEntityData = ExtractBaseDataFrom(entity.EntityType, entity.EntityID);
            }
            else
            {
                var contact = Global.DaoFactory.GetContactDao().GetByID(entity.ContactID);

                baseEntityData = new NameValueCollection();
                baseEntityData["title"] = contact.GetTitle();
                baseEntityData["id"] = contact.ID.ToString();
                baseEntityData["entityRelativeURL"] = "default.aspx?id=" + contact.ID;

                if (contact is Person)
                    baseEntityData["entityRelativeURL"] += "&type=people";

                baseEntityData["entityRelativeURL"] = String.Concat(PathProvider.BaseAbsolutePath,
                                                                    baseEntityData["entityRelativeURL"]);
            }

            client.BeginSingleRecipientEvent("send about add relationship event add");

            var interceptor = new InitiatorInterceptor(new DirectRecipient(ASC.Core.SecurityContext.CurrentAccount.ID.ToString(), ""));

            client.AddInterceptor(interceptor);

            try
            {

                client.SendNoticeToAsync(
                      NotifyConstants.Event_AddRelationshipEvent,
                      null,
                      userID.Select(item => ToRecipient(item)).ToArray(),
                      true,
                      new TagValue(NotifyConstants.Tag_EntityTitle, baseEntityData["title"]),
                      new TagValue(NotifyConstants.Tag_EntityID, baseEntityData["id"]),
                      new TagValue(NotifyConstants.Tag_EntityRelativeURL, baseEntityData["entityRelativeURL"]),
                      new TagValue(NotifyConstants.Tag_AdditionalData,
                      new Hashtable { 
                      { "Files", fileListInfoHashtable },
                      {"EventContent", entity.Content}}));

            }
            finally
            {
                client.RemoveInterceptor(interceptor.Name);
                client.EndSingleRecipientEvent("send about add relationship event add");
            }


        }

        public void SendAboutExportCompleted(Guid recipientID, String filePath)
        {
            if (recipientID == Guid.Empty) return;

            var recipient = ToRecipient(recipientID);

            client.SendNoticeToAsync(NotifyConstants.Event_ExportCompleted,
               null,
               new[] { recipient },
               true,
               new TagValue(NotifyConstants.Tag_EntityRelativeURL, filePath));

        }


        public void SendAboutResponsibleByTask(Task task, Hashtable fileListInfoHashtable)
        {
            var recipient = ToRecipient(task.ResponsibleID);

            if (recipient == null) return;

            client.SendNoticeToAsync(
               NotifyConstants.Event_ResponsibleForTask,
               null,
               new[] { recipient },
               null,
               new TagValue(NotifyConstants.Tag_EntityTitle, task.Title),
               new TagValue(NotifyConstants.Tag_EntityID, task.ID),
               new TagValue(NotifyConstants.Tag_AdditionalData,
                 new Hashtable { 
                      { "TaskDescription", HttpUtility.HtmlEncode(task.Description) }, 
                      { "Files", fileListInfoHashtable } 
                     }));
        }

        private IRecipient ToRecipient(Guid userID)
        {
            return source.GetRecipientsProvider().GetRecipient(userID.ToString());
        }

        public INotifyClient Client
        {
            get { return client; }
        }

        private NotifyClient(INotifyClient client, INotifySource source)
        {
            this.client = client;
            this.source = source;
        }

    }
}