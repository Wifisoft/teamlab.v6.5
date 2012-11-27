using ASC.Core.Notify;
using ASC.Notify;
using ASC.Notify.Messages;
using ASC.Notify.Model;
using ASC.Notify.Patterns;
using System;
using System.Collections.Generic;
using ASC.Notify.Recipients;
using ASC.Core;
using ASC.Core.Users;
using ASC.Web.Studio.Utility;
using System.Web;
using ASC.Web.Core.Calendars;
using ASC.Api.Calendar.BusinessObjects;
using ASC.Core.Tenants;

namespace ASC.Api.Calendar.Notification
{
    public class CalendarNotifyClient
    {
        private static INotifyClient _notifyClient;

        private static string _syncName = "calendarNotifySyncName";

        static CalendarNotifyClient()
        {
            _notifyClient = ASC.Core.WorkContext.NotifyContext.NotifyService.RegisterClient(CalendarNotifySource.Instance);
        }

        private static bool _isRegistered = false;
        public static void RegisterSendMethod()
        {
            if (!_isRegistered)
            {
                lock (_syncName)
                {
                    if (!_isRegistered)
                    {
                        var now = DateTime.UtcNow;
                        _notifyClient.RegisterSendMethod(NotifyAbouFutureEvent, TimeSpan.FromMinutes(1), new DateTime(now.Year, now.Month, now.Day, now.Hour, 0, 0));

                        _isRegistered = true;
                    }
                }
            }
        }

        private static void NotifyAbouFutureEvent(DateTime scheduleDate)
        {
            using (var dataProvider = new DataProvider())
            {
                var notifications = dataProvider.ExtractAndRecountNotifications(scheduleDate);
                foreach (var data in notifications)
                {
                    if (data.Event == null) continue;

                    var tenant = CoreContext.TenantManager.GetTenant(data.TenantId);
                    if (tenant == null || tenant.Status != TenantStatus.Active) continue;

                    CoreContext.TenantManager.SetCurrentTenant(tenant);
                    var r = CalendarNotifySource.Instance.GetRecipientsProvider().GetRecipient(data.UserId.ToString());


                    var startDate = data.GetUtcStartDate().Add(data.TimeZone.GetUtcOffset(DateTime.UtcNow));
                    var endDate = data.GetUtcEndDate();
                    endDate = (endDate == DateTime.MinValue ? DateTime.MinValue : endDate.Add(data.TimeZone.GetUtcOffset(DateTime.UtcNow)));

                    _notifyClient.SendNoticeAsync(CalendarNotifySource.EventAlert, null, r, true,
                        new TagValue(new Tag("EventName"), data.Event.Name),
                        new TagValue(new Tag("EventDescription"), data.Event.Description ?? ""),
                        new TagValue(new Tag("EventStartDate"), startDate.ToShortDateString() + " " + startDate.ToShortTimeString()),
                        new TagValue(new Tag("EventEndDate"), (endDate > startDate) ? (endDate.ToShortDateString() + " " + endDate.ToShortTimeString()) : ""));
                }
            }
        }

        public static void NotifyAboutSharingCalendar(ASC.Api.Calendar.BusinessObjects.Calendar calendar)
        {
            NotifyAboutSharingCalendar(calendar, null);
        }
        public static void NotifyAboutSharingCalendar(ASC.Api.Calendar.BusinessObjects.Calendar calendar, ASC.Api.Calendar.BusinessObjects.Calendar oldCalendar)
        {
            var initatorInterceptor = new InitiatorInterceptor(new DirectRecipient(SecurityContext.CurrentAccount.ID.ToString(), SecurityContext.CurrentAccount.Name));
            try
            {
                var usr = CoreContext.UserManager.GetUsers(SecurityContext.CurrentAccount.ID);
                var userLink = PerformUrl(CommonLinkUtility.GetUserProfile(usr.ID.ToString(), Guid.Empty, UserProfileType.General, false));

                foreach (var item in calendar.SharingOptions.PublicItems)
                {
                    if (oldCalendar != null && oldCalendar.SharingOptions.PublicItems.Exists(i => i.Id.Equals(item.Id)))
                        continue;

                    var r = CalendarNotifySource.Instance.GetRecipientsProvider().GetRecipient(item.Id.ToString());
                    _notifyClient.SendNoticeAsync(CalendarNotifySource.CalendarSharing, null, r, true,
                        new TagValue(new Tag("SharingType"), "calendar"),
                        new TagValue(new Tag("UserName"), usr.DisplayUserName()),
                        new TagValue(new Tag("UserLink"), userLink),
                        new TagValue(new Tag("CalendarName"), calendar.Name));
                }
                _notifyClient.EndSingleRecipientEvent(_syncName);
            }
            finally
            {
                _notifyClient.RemoveInterceptor(initatorInterceptor.Name);
            }
        }

        public static void NotifyAboutSharingEvent(ASC.Api.Calendar.BusinessObjects.Event calendarEvent)
        {
            NotifyAboutSharingEvent(calendarEvent, null);
        }
        public static void NotifyAboutSharingEvent(ASC.Api.Calendar.BusinessObjects.Event calendarEvent, ASC.Api.Calendar.BusinessObjects.Event oldCalendarEvent)
        {
            var initatorInterceptor = new InitiatorInterceptor(new DirectRecipient(SecurityContext.CurrentAccount.ID.ToString(), SecurityContext.CurrentAccount.Name));
            try
            {
                var usr = CoreContext.UserManager.GetUsers(SecurityContext.CurrentAccount.ID);
                var userLink = PerformUrl(CommonLinkUtility.GetUserProfile(usr.ID.ToString(), Guid.Empty, UserProfileType.General, false));

                foreach (var item in calendarEvent.SharingOptions.PublicItems)
                {
                    if (oldCalendarEvent != null && oldCalendarEvent.SharingOptions.PublicItems.Exists(i => i.Id.Equals(item.Id)))
                        continue;

                    var r = CalendarNotifySource.Instance.GetRecipientsProvider().GetRecipient(item.Id.ToString());
                    _notifyClient.SendNoticeAsync(CalendarNotifySource.CalendarSharing, null, r, true,
                        new TagValue(new Tag("SharingType"), "event"),
                        new TagValue(new Tag("UserName"), usr.DisplayUserName()),
                        new TagValue(new Tag("UserLink"), userLink),
                        new TagValue(new Tag("EventName"), calendarEvent.Name));
                }
                _notifyClient.EndSingleRecipientEvent(_syncName);
            }
            finally
            {
                _notifyClient.RemoveInterceptor(initatorInterceptor.Name);
            }
        }

        private static string PerformUrl(string url)
        {
            string port = string.Empty;
            if (HttpContext.Current != null && HttpContext.Current.Request != null)
                port = HttpContext.Current.Request.GetUrlRewriter().IsDefaultPort ? string.Empty : ":" + HttpContext.Current.Request.GetUrlRewriter().Port;
            
            var result = string.Format("{0}://{1}{2}",
                   (HttpContext.Current != null && HttpContext.Current.Request!=null) ? HttpContext.Current.Request.GetUrlRewriter().Scheme : Uri.UriSchemeHttp,
                   CoreContext.TenantManager.GetCurrentTenant().TenantDomain,
                   port)+ ("/" + url).Replace("//","/");

            return result;
        }
    }


    public class CalendarNotifySource : NotifySource, IDependencyProvider
    {
        public static INotifyAction CalendarSharing = new NotifyAction("calendar_sharing", "calendar sharing");
        public static INotifyAction EventAlert = new NotifyAction("event_alert", "event alert");

        private static CalendarNotifySource _instance = null;

        public static CalendarNotifySource Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (typeof(CalendarNotifySource))
                    {
                        if (_instance == null)
                            _instance = new CalendarNotifySource();
                    }
                }

                return _instance;
            }
        }

        private static Guid _sourceId = new Guid("{40650DA3-F7C1-424c-8C89-B9C115472E08}");


        private CalendarNotifySource()
            : base(_sourceId) { }


        protected override IActionPatternProvider CreateActionPatternProvider()
        {

            return new XmlActionPatternProvider(
                  GetType().Assembly,
                  "ASC.Api.Calendar.Notification.action_pattern.xml",
                  ActionProvider,
                  PatternProvider
              );
        }

        protected override IActionProvider CreateActionProvider()
        {
            return new ConstActionProvider(
                    CalendarSharing,
                    EventAlert
                );
        }

        protected override IDependencyProvider CreateDependencyProvider()
        {
            return this;
        }

        protected override IPatternProvider CreatePatternsProvider()
        {
            return new XmlPatternProvider(ASC.Api.Calendar.Notification.CalendarPatterns.calendar_patterns);
        }

        #region IDependencyProvider Members

        public override ITagValue[] GetDependencies(INoticeMessage message, string objectID, ITag[] tags)
        {
            return new ITagValue[0];
        }

        #endregion
    }

}
