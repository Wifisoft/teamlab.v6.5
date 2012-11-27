#region Import

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Web;
using ASC.CRM.Core.Entities;
using ASC.Core;
using ASC.Web.CRM;
using ASC.Web.CRM.Classes;
using ASC.Web.CRM.Configuration;
using ASC.Web.Core.Users.Activity;
using ASC.Web.Studio.Utility;

#endregion

namespace ASC.CRM.Core
{
    public class TimeLinePublisher : BaseUserActivityPublisher
    {

        private const String SecurityDataPattern = "{0}|{1}";

        private sealed class TimeLineUserActivity : UserActivity
        {
            public TimeLineUserActivity(String actionText, int actionType, int businessValue)
            {
                ProductID = ProductEntryPoint.ID;
                ModuleID = ProductEntryPoint.ID;
                Date = ASC.Core.Tenants.TenantUtil.DateTimeNow();
                UserID = SecurityContext.CurrentAccount.ID;
                ActionText = actionText;
                ActionType = actionType;
                BusinessValue = businessValue;
                TenantID = TenantProvider.CurrentTenantID;
            }
        }


        public static void Task(Task task, String actionText, int actionType, int businessValue)
        {
            Task(task, actionText, actionType, businessValue, false);
        }

        public static void Task(Task task, String actionText, int actionType, int businessValue, bool withPreview)
        {
            UserActivityPublisher.Publish<TimeLinePublisher>(new TimeLineUserActivity(actionText, actionType, businessValue)
            {
                ContentID = task.ID.ToString(),
                SecurityId = String.Format(SecurityDataPattern, (int)EntityType.Task, task.ID),
                Title = task.Title,
                URL = String.Concat(VirtualPathUtility.ToAbsolute(PathProvider.BaseVirtualPath + "tasks.aspx"), String.Format("?id={0}", task.ID))
            });
        }

        public static void Contact(Contact contact, String actionText, int actionType, int businessValue)
        {
            var type = (int)EntityType.Contact;
            if(contact is Company)
                type = (int)EntityType.Company;
            
            UserActivityPublisher.Publish<TimeLinePublisher>(new TimeLineUserActivity(actionText, actionType, businessValue)
            {
                ContentID = contact.ID.ToString(),
                Title = contact.GetTitle(),
                SecurityId = String.Format(SecurityDataPattern, type, contact.SecurityId),
                URL = String.Concat(VirtualPathUtility.ToAbsolute(PathProvider.BaseVirtualPath + "default.aspx"), String.Format("?id={0}", contact.ID))
            });
        }

        public static void Deal(Deal deal, String actionText, int actionType, int businessValue)
        {
            UserActivityPublisher.Publish<TimeLinePublisher>(new TimeLineUserActivity(actionText, actionType, businessValue)
            {
                ContentID = deal.ID.ToString(),
                Title = deal.Title,
                SecurityId = String.Format(SecurityDataPattern, (int)EntityType.Opportunity, deal.SecurityId),
                URL = String.Concat(VirtualPathUtility.ToAbsolute(PathProvider.BaseVirtualPath + "deals.aspx"), String.Format("?id={0}", deal.ID))
            });
        }

        public static void Cases(Entities.Cases cases, String actionText, int actionType, int businessValue)
        {
            UserActivityPublisher.Publish<TimeLinePublisher>(new TimeLineUserActivity(actionText, actionType, businessValue)
            {
                ContentID = cases.ID.ToString(),
                Title = cases.Title,
                SecurityId = String.Format(SecurityDataPattern, (int)EntityType.Case, cases.SecurityId),
                URL = String.Concat(VirtualPathUtility.ToAbsolute(PathProvider.BaseVirtualPath + "cases.aspx"), String.Format("?id={0}", cases.ID))
            });
        }

        public static bool CanAccessTo(UserActivity activity)
        {
            try
            {
                if (activity.ProductID != ProductEntryPoint.ID || String.IsNullOrEmpty(activity.SecurityId))
                    throw new ArgumentException();

                var parts = activity.SecurityId.Split(new[] { '|' });
                var entityType = (EntityType)Convert.ToInt32(parts[0]);
                var entityID = Convert.ToInt32(parts[1]);

                switch (entityType)
                {
                    case EntityType.Case:
                        var cases = Global.DaoFactory.GetCasesDao().GetByID(entityID);
                        
                        return cases != null && CRMSecurity.CanAccessTo(cases);
                    case EntityType.Task:
                        var task = Global.DaoFactory.GetTaskDao().GetByID(entityID);

                        return task != null && CRMSecurity.CanAccessTo(task);
                    case EntityType.Opportunity:
                        var deal = Global.DaoFactory.GetDealDao().GetByID(entityID);

                        return deal != null && CRMSecurity.CanAccessTo(deal);
                    case EntityType.Contact:
                    case EntityType.Person:
                    case EntityType.Company:
                        var contact = Global.DaoFactory.GetDealDao().GetByID(entityID);

                        return contact != null && CRMSecurity.CanAccessTo(contact);
                    case EntityType.RelationshipEvent:
                        
                        return true;
                    default:
                        return true;
                }
            }
            catch
            {
                return false;
            }



        }
    }
}
