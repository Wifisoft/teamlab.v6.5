using System;
using System.Web.Configuration;
using ASC.Core;
using ASC.Core.Tenants;
using ASC.Projects.Core.Services.NotifyService;
using ASC.Projects.Data;
using log4net;

using ASC.Projects.Core.Domain;
using System.Linq;

namespace ASC.Web.Projects.Classes
{
    static class TaskHelper
    {
        public static void SendAutoReminderAboutTask(DateTime state)
        {
            try
            {                
                var now = DateTime.UtcNow;
                foreach (var r in new DaoFactory(Global.DB_ID, Tenant.DEFAULT_TENANT).GetTaskDao().GetTasksForReminder(now))
                {
                    var tenant = CoreContext.TenantManager.GetTenant((int)r[0]);
                    if (tenant == null || tenant.Status != TenantStatus.Active) continue;

                    var localTime = TenantUtil.DateTimeFromUtc(tenant, now);
                    if (!TimeToSendReminderAboutTask(localTime)) continue;

                    var deadline = (DateTime)r[2];
                    if (deadline.Date == localTime.Date)
                    {
                        try
                        {
                            CoreContext.TenantManager.SetCurrentTenant(tenant);
                            var t = Global.EngineFactory.GetTaskEngine().GetByID((int)r[1]);
                            if (t != null)
                            {
                                var senders = t.Responsibles
                                    .Select(resp => NotifySource.Instance.GetRecipientsProvider().GetRecipient(resp.ToString()))
                                    .Where(resp => resp != null).ToList();
                                if (senders.Count != 0)
                                    NotifyClient.Instance.SendReminderAboutTaskDeadline(senders, t);
                            }
                        }
                        catch (Exception ex)
                        {
                            LogManager.GetLogger("ASC.Projects.Tasks").Error("SendAutoReminderAboutTask", ex);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogManager.GetLogger("ASC.Projects.Tasks").Error("SendAutoReminderAboutTask", ex);
            }
        }

        private static bool TimeToSendReminderAboutTask(DateTime currentTime)
        {
            var hourToSend = 7;
            if (!string.IsNullOrEmpty(WebConfigurationManager.AppSettings["remindertime"]))
            {
                var hour = 0;
                if (int.TryParse(WebConfigurationManager.AppSettings["remindertime"], out hour))
                {
                    hourToSend = hour;
                }
            }
            return currentTime.Hour == hourToSend;
        }
    }
}