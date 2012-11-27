#region Import

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ASC.Core;
using ASC.Core.Users;
using ASC.Notify;
using ASC.Notify.Model;
using ASC.Notify.Patterns;
using ASC.Notify.Recipients;
using ASC.Core.Tenants;

#endregion

namespace ASC.Web.CRM.Services.NotifyService
{
    public static class NotifyConstants
    {

        public static readonly INotifyAction Event_SetAccess = new NotifyAction("SetAccess", "set access for users");

        public static readonly INotifyAction Event_ResponsibleForTask = new NotifyAction("ResponsibleForTask", "responsible for task");
        
        public static readonly INotifyAction Event_AddRelationshipEvent = new NotifyAction("AddRelationshipEvent", "add relationship event");

        public static readonly INotifyAction Event_ExportCompleted = new NotifyAction("ExportCompleted", "export is completed");
        
        public static readonly INotifyAction Event_CreateNewContact = new NotifyAction("CreateNewContact", "create new contact");
        
        public static readonly ITag Tag_AdditionalData = new Tag("AdditionalData");

        public static readonly ITag Tag_EntityID = new Tag("EntityID");

        public static readonly ITag Tag_EntityTitle = new Tag("EntityTitle");

        public static readonly ITag Tag_EntityRelativeURL = new Tag("EntityRelativeURL");

    }
}