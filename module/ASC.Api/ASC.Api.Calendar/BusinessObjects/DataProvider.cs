﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using ASC.Api.Calendar.ExternalCalendars;
using ASC.Common.Data;
using ASC.Common.Data.Sql;
using ASC.Common.Data.Sql.Expressions;
using ASC.Core;
using ASC.Web.Core.Calendars;

namespace ASC.Api.Calendar.BusinessObjects
{
    public class DataProvider : IDisposable
    {
        internal static string DBId { get { return "calendar"; } }

        private DbManager DbManager
        {
            get;
            set;
        }

        private string _calendarTable = "calendar_calendars cal";
        private string _calendarItemTable = "calendar_calendar_item cal_itm";
        private string _calendarUserTable = "calendar_calendar_user cal_usr";
        private string _eventTable = "calendar_events evt";
        private string _eventItemTable = "calendar_event_item evt_itm";

        public DataProvider()
        {

            if (!DbRegistry.IsDatabaseRegistered(DBId))
            {
                DbRegistry.RegisterDatabase(DBId, ConfigurationManager.ConnectionStrings[DBId]);
            }
            DbManager = DbManager.FromHttpContext(DBId);
        }

        public void Dispose()
        {
            DbManager.Dispose();
        }       

        public List<UserViewSettings> GetUserViewSettings(Guid userId, List<string> calendarIds)
        {
            var cc = new ColumnCollection();

            var extCalId = cc.RegistryColumn("ext_calendar_id");
            var usrId = cc.RegistryColumn("user_id");
            var hideEvents = cc.RegistryColumn("hide_events");
            var isAccepted = cc.RegistryColumn("is_accepted");
            var textColor = cc.RegistryColumn("text_color");
            var background = cc.RegistryColumn("background_color");
            var alertType = cc.RegistryColumn("alert_type");
            var calId = cc.RegistryColumn("calendar_id");
            var calName = cc.RegistryColumn("name");
            var timeZone = cc.RegistryColumn("time_zone");

            var data = DbManager
               .ExecuteList(
               new SqlQuery("calendar_calendar_user").Select(cc.SelectQuery)
                   .Where((Exp.In(extCalId.Name, calendarIds) | Exp.In(calId.Name, calendarIds)) & Exp.Eq(usrId.Name, userId))
                   );

            var options = new List<UserViewSettings>();
            foreach (var r in data)
            {
                options.Add(new UserViewSettings()
                {
                    CalendarId = Convert.ToInt32(r[calId.Ind]) == 0 ? Convert.ToString(r[extCalId.Ind]) : Convert.ToString(r[calId.Ind]),
                    UserId = usrId.Parse<Guid>(r),
                    IsHideEvents = hideEvents.Parse<bool>(r),
                    IsAccepted = isAccepted.Parse<bool>(r),
                    TextColor = textColor.Parse<string>(r),
                    BackgroundColor = background.Parse<string>(r),
                    EventAlertType = (EventAlertType)alertType.Parse<int>(r),
                    Name = calName.Parse<string>(r),
                    TimeZone = timeZone.Parse<TimeZoneInfo>(r)
                });
            }

            return options;
        }

        public List<Calendar> LoadCalendarsForUser(Guid userId, out int newCalendarsCount)
        {
            var groups = CoreContext.UserManager.GetUserGroups(userId).Select(g => g.ID).ToList();
            groups.AddRange(CoreContext.UserManager.GetUserGroups(userId, ASC.Core.Users.Constants.SysGroupCategoryId).Select(g => g.ID));

            var calIds = DbManager.ExecuteList(new SqlQuery(_calendarItemTable).Select("cal_itm.calendar_id")
                                                .InnerJoin(_calendarTable, Exp.EqColumns("cal.id", "cal_itm.calendar_id"))
                                                .Where("cal.tenant", CoreContext.TenantManager.GetCurrentTenant().TenantId)
                                                .Where(Exp.Eq("cal_itm.item_id", userId) | (Exp.In("cal_itm.item_id", groups.ToArray()) & Exp.Eq("cal_itm.is_group", true)))
                                                .Union(new SqlQuery(_calendarTable).Select("cal.id").Where("cal.owner_id", userId))
                                            ).Select(r => r[0]);

            var cals = GetCalendarsByIds(calIds.ToArray());

            //filter by is_accepted field
            newCalendarsCount = cals.RemoveAll(c => (!c.OwnerId.Equals(userId) && !c.ViewSettings.Exists(v => v.UserId.Equals(userId) && v.IsAccepted))
                || (c.IsiCalStream() && c.ViewSettings.Exists(v => v.UserId.Equals(userId) && !v.IsAccepted)));
            return cals;
        }

        public List<Calendar> LoadiCalStreamsForUser(Guid userId)
        {

            var calIds = DbManager.ExecuteList(new SqlQuery(_calendarTable).Select("cal.id")
                                               .Where(Exp.Eq("cal.owner_id", userId) & !Exp.Eq("cal.ical_url", null))
                                            ).Select(r => r[0]);

            var calendars = GetCalendarsByIds(calIds.ToArray());
            return calendars;
        }

        public List<Calendar> LoadSubscriptionsForUser(Guid userId)
        {
            var groups = CoreContext.UserManager.GetUserGroups(userId).Select(g => g.ID).ToList();
            groups.AddRange(CoreContext.UserManager.GetUserGroups(userId, ASC.Core.Users.Constants.SysGroupCategoryId).Select(g => g.ID));

            var calIds = DbManager.ExecuteList(new SqlQuery(_calendarItemTable).Select("cal_itm.calendar_id")
                                                .InnerJoin(_calendarTable, Exp.EqColumns("cal.id", "cal_itm.calendar_id"))
                                                .Where("cal.tenant", CoreContext.TenantManager.GetCurrentTenant().TenantId)
                                                .Where(Exp.Eq("cal_itm.item_id", userId) | (Exp.In("cal_itm.item_id", groups.ToArray()) & Exp.Eq("cal_itm.is_group", true)))
                                            ).Select(r => r[0]);

            var calendars = GetCalendarsByIds(calIds.ToArray());
            return calendars;
        }

        public TimeZoneInfo GetTimeZoneForSharedEventsCalendar(Guid userId)
        {
            var q = new SqlQuery(_calendarUserTable)
                .Select("time_zone")
                .Where("ext_calendar_id", SharedEventsCalendar.CalendarId)
                .Where("user_id", userId);

            var data = DbManager.ExecuteList(q);
            if (data.Count > 0)
                return data.Select(r => TimeZoneInfo.FindSystemTimeZoneById(Convert.ToString(r[0]))).First();

            return SharedEventsCalendar.CalendarTimeZone;
        }

        public TimeZoneInfo GetTimeZoneForCalendar(Guid userId, int caledarId)
        {
            return DbManager.ExecuteList(new SqlQuery(_calendarTable).Select("cal.time_zone", "cal_usr.time_zone")
                                            .LeftOuterJoin(_calendarUserTable, Exp.EqColumns("cal.id", "cal_usr.calendar_id") & Exp.Eq("cal_usr.user_id", userId))
                                            .Where(Exp.Eq("cal.id", caledarId)))
                                            .Select(r => (r[1] == null || r[1] == DBNull.Value) ? TimeZoneInfo.FindSystemTimeZoneById(Convert.ToString(r[0])) : TimeZoneInfo.FindSystemTimeZoneById(Convert.ToString(r[1]))).First();
        }

        public List<Calendar> GetCalendarsByIds(object[] calIds)
        {
            var cc = new ColumnCollection();

            var calId = cc.RegistryColumn("cal.id");
            var calName = cc.RegistryColumn("cal.name");
            var calDescription = cc.RegistryColumn("cal.description");
            var calTenant = cc.RegistryColumn("cal.tenant");
            var calTextColor = cc.RegistryColumn("cal.text_color");
            var calBackground = cc.RegistryColumn("cal.background_color");
            var calOwner = cc.RegistryColumn("cal.owner_id");
            var calAlertType = cc.RegistryColumn("cal.alert_type");
            var calTimeZone = cc.RegistryColumn("cal.time_zone");
            var iCalUrl = cc.RegistryColumn("cal.ical_url");

            var usrId = cc.RegistryColumn("cal_usr.user_id");
            var usrHideEvents = cc.RegistryColumn("cal_usr.hide_events");
            var usrIsAccepted = cc.RegistryColumn("cal_usr.is_accepted");
            var usrTextColor = cc.RegistryColumn("cal_usr.text_color");
            var usrBackground = cc.RegistryColumn("cal_usr.background_color");
            var usrAlertType = cc.RegistryColumn("cal_usr.alert_type");
            var usrCalName = cc.RegistryColumn("cal_usr.name");
            var usrTimeZone = cc.RegistryColumn("cal_usr.time_zone");

            var data = DbManager.ExecuteList(new SqlQuery(_calendarTable).Select(cc.SelectQuery)
                                            .LeftOuterJoin(_calendarUserTable, Exp.EqColumns(calId.Name, "cal_usr.calendar_id"))
                                            .Where(Exp.In(calId.Name, calIds)));

            var cc1 = new ColumnCollection();

            var itemCalId = cc1.RegistryColumn("cal_itm.calendar_id");
            var itemId = cc1.RegistryColumn("cal_itm.item_id");
            var itemIsGroup = cc1.RegistryColumn("cal_itm.is_group");

            var sharingData = DbManager.ExecuteList(new SqlQuery(_calendarItemTable).Select(cc1.SelectQuery)
                                                    .Where(Exp.In(itemCalId.Name, calIds)));


            //parsing
            var calendars = new List<Calendar>();
            foreach (var r in data)
            {
                var calendar = calendars.Find(c => string.Equals(c.Id, calId.Parse<int>(r).ToString(), StringComparison.InvariantCultureIgnoreCase));
                if (calendar == null)
                {
                    calendar = new Calendar()
                    {
                        Id = calId.Parse<int>(r).ToString(),
                        Name = calName.Parse<string>(r),
                        Description = calDescription.Parse<string>(r),
                        TenantId = calTenant.Parse<int>(r),
                        OwnerId = calOwner.Parse<Guid>(r),
                        EventAlertType = (EventAlertType)calAlertType.Parse<int>(r),
                        TimeZone = calTimeZone.Parse<TimeZoneInfo>(r),
                        iCalUrl = iCalUrl.Parse<string>(r),
                    };
                    calendar.Context.HtmlTextColor = calTextColor.Parse<string>(r);
                    calendar.Context.HtmlBackgroundColor = calBackground.Parse<string>(r);

                    calendars.Add(calendar);

                    foreach (var row in sharingData)
                    {
                        var _calId = itemCalId.Parse<int>(row).ToString();
                        if (String.Equals(_calId, calendar.Id, StringComparison.InvariantCultureIgnoreCase))
                        {
                            calendar.SharingOptions.PublicItems.Add(new ASC.Web.Core.Calendars.SharingOptions.PublicItem()
                            {
                                Id = itemId.Parse<Guid>(row),
                                IsGroup = itemIsGroup.Parse<bool>(row)
                            });
                        }
                    }
                }

                if (!usrId.IsNull(r))
                {
                    var uvs = new UserViewSettings()
                    {
                        CalendarId = calendar.Id.ToString(),
                        UserId = usrId.Parse<Guid>(r),
                        IsHideEvents = usrHideEvents.Parse<bool>(r),
                        IsAccepted = usrIsAccepted.Parse<bool>(r),
                        TextColor = usrTextColor.Parse<string>(r),
                        BackgroundColor = usrBackground.Parse<string>(r),
                        EventAlertType = (EventAlertType)usrAlertType.Parse<int>(r),
                        Name = usrCalName.Parse<string>(r),
                        TimeZone = usrTimeZone.Parse<TimeZoneInfo>(r)
                    };

                    calendar.ViewSettings.Add(uvs);
                }
            }

            return calendars;
        }

        public Calendar GetCalendarById(int calendarId)
        {
            var calendars = GetCalendarsByIds(new object[] { calendarId });
            if (calendars.Count > 0)
                return calendars[0];

            return null;
        }

        public Calendar CreateCalendar(Guid ownerId, string name, string description, string textColor, string backgroundColor, TimeZoneInfo timeZone, EventAlertType eventAlertType, string iCalUrl, List<SharingOptions.PublicItem> publicItems, List<UserViewSettings> viewSettings)
        {
            int calendarId = 0;
            using (var tr = DbManager.Connection.BeginTransaction())
            {

                calendarId = DbManager.ExecuteScalar<int>(new SqlInsert("calendar_calendars")
                                                    .InColumnValue("id", 0)
                                                    .InColumnValue("tenant", CoreContext.TenantManager.GetCurrentTenant().TenantId)
                                                    .InColumnValue("owner_id", ownerId)
                                                    .InColumnValue("name", name)
                                                    .InColumnValue("description", description)
                                                    .InColumnValue("text_color", textColor)
                                                    .InColumnValue("background_color", backgroundColor)
                                                    .InColumnValue("alert_type", (int)eventAlertType)
                                                    .InColumnValue("time_zone", timeZone.Id)
                                                    .InColumnValue("ical_url", iCalUrl)
                                                    .Identity<int>(0, 0, true));

                if (publicItems != null)
                {
                    foreach (var item in publicItems)
                    {
                        DbManager.ExecuteScalar(new SqlInsert("calendar_calendar_item")
                                                    .InColumnValue("calendar_id", calendarId)
                                                    .InColumnValue("item_id", item.Id)
                                                    .InColumnValue("is_group", item.IsGroup));
                    }
                }

                if (viewSettings != null)
                {
                    foreach (var view in viewSettings)
                    {
                        DbManager.ExecuteScalar(new SqlInsert("calendar_calendar_user")
                                                    .InColumnValue("calendar_id", calendarId)
                                                    .InColumnValue("user_id", view.UserId)
                                                    .InColumnValue("hide_events", view.IsHideEvents)
                                                    .InColumnValue("is_accepted", view.IsAccepted)
                                                    .InColumnValue("text_color", view.TextColor)
                                                    .InColumnValue("background_color", view.BackgroundColor)
                                                    .InColumnValue("alert_type", (int)view.EventAlertType)
                                                    .InColumnValue("name", view.Name ?? "")
                                                    .InColumnValue("time_zone", view.TimeZone != null ? view.TimeZone.Id : null)
                                                    );

                    }
                }
                tr.Commit();
            }

            return GetCalendarById(calendarId);
        }

        public Calendar UpdateCalendar(int calendarId, string name, string description, List<SharingOptions.PublicItem> publicItems, List<UserViewSettings> viewSettings)
        {
            using (var tr = DbManager.Connection.BeginTransaction())
            {

                DbManager.ExecuteNonQuery(new SqlUpdate("calendar_calendars")
                                                    .Set("name", name)
                                                    .Set("description", description)
                                                    .Where("id", calendarId));

                //sharing
                DbManager.ExecuteNonQuery(new SqlDelete("calendar_calendar_item").Where("calendar_id", calendarId));
                foreach (var item in publicItems)
                {
                    DbManager.ExecuteScalar(new SqlInsert("calendar_calendar_item")
                                                .InColumnValue("calendar_id", calendarId)
                                                .InColumnValue("item_id", item.Id)
                                                .InColumnValue("is_group", item.IsGroup));
                }

                //view
                DbManager.ExecuteNonQuery(new SqlDelete("calendar_calendar_user").Where("calendar_id", calendarId));
                foreach (var view in viewSettings)
                {
                    DbManager.ExecuteScalar(new SqlInsert("calendar_calendar_user")
                                                .InColumnValue("calendar_id", calendarId)
                                                .InColumnValue("user_id", view.UserId)
                                                .InColumnValue("hide_events", view.IsHideEvents)
                                                .InColumnValue("is_accepted", view.IsAccepted)
                                                .InColumnValue("text_color", view.TextColor)
                                                .InColumnValue("background_color", view.BackgroundColor)
                                                .InColumnValue("alert_type", (int)view.EventAlertType)
                                                .InColumnValue("name", view.Name ?? "")
                                                .InColumnValue("time_zone", view.TimeZone != null ? view.TimeZone.Id : null)
                                                );

                }

                //update notifications
                var cc = new ColumnCollection();
                var eId = cc.RegistryColumn("e.id");
                var eStartDate = cc.RegistryColumn("e.start_date");
                var eAlertType = cc.RegistryColumn("e.alert_type");
                var eRRule = cc.RegistryColumn("e.rrule");

                var eventsData = DbManager.ExecuteList(new SqlQuery("calendar_events e").Select(cc.SelectQuery)
                                                                      .Where("e.calendar_id", calendarId));

                foreach (var r in eventsData)
                {
                    UpdateEventNotifications(eId.Parse<int>(r), calendarId,
                                             eStartDate.Parse<DateTime>(r),
                                            (EventAlertType)eAlertType.Parse<int>(r),
                                             eRRule.Parse<RecurrenceRule>(r), null, publicItems);
                }

                tr.Commit();
            }

            return GetCalendarById(calendarId);
        }

        public void UpdateCalendarUserView(List<UserViewSettings> viewSettings)
        {
            using (var tr = DbManager.Connection.BeginTransaction())
            {
                foreach (var s in viewSettings)
                    UpdateCalendarUserView(s);

                tr.Commit();
            }
        }
        public void UpdateCalendarUserView(UserViewSettings viewSettings)
        {
            int calendarId;
            var cc = new ColumnCollection();
            var eId = cc.RegistryColumn("e.id");
            var eStartDate = cc.RegistryColumn("e.start_date");
            var eAlertType = cc.RegistryColumn("e.alert_type");
            var eRRule = cc.RegistryColumn("e.rrule");
            var eCalId = cc.RegistryColumn("e.calendar_id");

            if (int.TryParse(viewSettings.CalendarId, out calendarId))
            {
                DbManager.ExecuteNonQuery(new SqlInsert("calendar_calendar_user", true)
                                                        .InColumnValue("calendar_id", calendarId)
                                                        .InColumnValue("user_id", viewSettings.UserId)
                                                        .InColumnValue("hide_events", viewSettings.IsHideEvents)
                                                        .InColumnValue("text_color", viewSettings.TextColor)
                                                        .InColumnValue("background_color", viewSettings.BackgroundColor)
                                                        .InColumnValue("is_accepted", viewSettings.IsAccepted)
                                                        .InColumnValue("alert_type", (int)viewSettings.EventAlertType)
                                                        .InColumnValue("name", viewSettings.Name ?? "")
                                                        .InColumnValue("time_zone", viewSettings.TimeZone != null ? viewSettings.TimeZone.Id : null)
                                                        );



                //update notifications
                var eventsData = DbManager.ExecuteList(new SqlQuery("calendar_events e").Select(cc.SelectQuery)
                                                                      .Where("e.calendar_id", calendarId));

                foreach (var r in eventsData)
                {
                    UpdateEventNotifications(eId.Parse<int>(r), calendarId,
                                            eStartDate.Parse<DateTime>(r),
                                           (EventAlertType)eAlertType.Parse<int>(r),
                                           eRRule.Parse<RecurrenceRule>(r), null, null);
                }

            }
            else
            {
                DbManager.ExecuteNonQuery(new SqlInsert("calendar_calendar_user", true)
                                                        .InColumnValue("ext_calendar_id", viewSettings.CalendarId)
                                                        .InColumnValue("user_id", viewSettings.UserId)
                                                        .InColumnValue("hide_events", viewSettings.IsHideEvents)
                                                        .InColumnValue("text_color", viewSettings.TextColor)
                                                        .InColumnValue("background_color", viewSettings.BackgroundColor)
                                                        .InColumnValue("alert_type", (int)viewSettings.EventAlertType)
                                                        .InColumnValue("is_accepted", viewSettings.IsAccepted)
                                                        .InColumnValue("name", viewSettings.Name ?? "")
                                                        .InColumnValue("time_zone", viewSettings.TimeZone != null ? viewSettings.TimeZone.Id : null)
                                                        );

                if (String.Equals(viewSettings.CalendarId, SharedEventsCalendar.CalendarId, StringComparison.InvariantCultureIgnoreCase))
                {
                    //update notifications
                    var groups = CoreContext.UserManager.GetUserGroups(viewSettings.UserId).Select(g => g.ID).ToList();
                    groups.AddRange(CoreContext.UserManager.GetUserGroups(viewSettings.UserId, ASC.Core.Users.Constants.SysGroupCategoryId).Select(g => g.ID));

                    var q = new SqlQuery("calendar_events e")
                        .Select(cc.SelectQuery)
                        .InnerJoin("calendar_event_item ei", Exp.EqColumns("ei.event_id", eId.Name))
                        .Where("e.tenant", CoreContext.TenantManager.GetCurrentTenant().TenantId)
                        .Where((Exp.Eq("ei.is_group", false) & Exp.Eq("ei.item_id", viewSettings.UserId)) | (Exp.Eq("ei.is_group", true) & Exp.In("ei.item_id", groups.ToArray())));
                    var eventsData = DbManager.ExecuteList(q);

                    foreach (var r in eventsData)
                    {
                        UpdateEventNotifications(eId.Parse<int>(r), eCalId.Parse<int>(r),
                                                 eStartDate.Parse<DateTime>(r),
                                                 (EventAlertType)eAlertType.Parse<int>(r),
                                                 eRRule.Parse<RecurrenceRule>(r), null, null);
                    }
                }
            }
        }

        public void RemoveCalendar(int calendarId)
        {
            using (var tr = DbManager.Connection.BeginTransaction())
            {

                DbManager.ExecuteNonQuery(new SqlDelete("calendar_calendars").Where("id", calendarId));
                DbManager.ExecuteNonQuery(new SqlDelete("calendar_calendar_user").Where("calendar_id", calendarId));
                DbManager.ExecuteNonQuery(new SqlDelete("calendar_calendar_item").Where("calendar_id", calendarId));

                var data = DbManager.ExecuteList(new SqlQuery("calendar_events").Select("id").Where("calendar_id", calendarId))
                                    .Select(r => r[0]);

                DbManager.ExecuteNonQuery(new SqlDelete("calendar_events").Where("calendar_id", calendarId));
                DbManager.ExecuteNonQuery(new SqlDelete("calendar_event_item").Where(Exp.In("event_id", data.ToArray())));
                DbManager.ExecuteNonQuery(new SqlDelete("calendar_event_user").Where(Exp.In("event_id", data.ToArray())));
                DbManager.ExecuteNonQuery(new SqlDelete("calendar_notifications").Where(Exp.In("event_id", data.ToArray())));

                tr.Commit();
            }
        }

        public void RemoveExternalCalendarData(string calendarId)
        {
            using (var tr = DbManager.Connection.BeginTransaction())
            {
                DbManager.ExecuteNonQuery(new SqlDelete("calendar_calendar_user").Where("ext_calendar_id", calendarId));
                tr.Commit();
            }
        }

        internal List<Event> LoadSharedEvents(Guid userId, int tenantId, DateTime utcStartDate, DateTime utcEndDate)
        {
            var groups = CoreContext.UserManager.GetUserGroups(userId).Select(g => g.ID).ToList();
            groups.AddRange(CoreContext.UserManager.GetUserGroups(userId, ASC.Core.Users.Constants.SysGroupCategoryId).Select(g => g.ID));

            var evIds = DbManager
                .ExecuteList(
                new SqlQuery(_eventTable).Select("evt.id")
                    .InnerJoin(_eventItemTable, Exp.EqColumns("evt_itm.event_id", "evt.id"))
                    .Where("evt.tenant", tenantId)
                    .Where(
                    (Exp.Eq("evt_itm.item_id", userId) | (Exp.In("evt_itm.item_id", groups.ToArray()) & Exp.Eq("evt_itm.is_group", true)))
                    & Exp.Eq("evt.tenant", tenantId)
                    & ((Exp.Ge("evt.start_date", utcStartDate) & Exp.Le("evt.start_date", utcEndDate) & Exp.Eq("evt.rrule", "")
                       | !Exp.Eq("evt.rrule", "")))

                    & !Exp.Eq("evt.owner_id", userId)

                    & !Exp.Exists(new SqlQuery("calendar_event_user evt_usr").Select("evt_usr.event_id")
                                                                              .Where(Exp.EqColumns("evt_usr.event_id", "evt.id")
                                                                                     & Exp.Eq("evt_usr.user_id", userId)
                                                                                     & Exp.Eq("evt_usr.is_unsubscribe", true)))
                    )).Select(r => r[0]);

            return GetEventsByIds(evIds.ToArray(), userId);
        }

        public List<Event> LoadEvents(int calendarId, Guid userId, int tenantId, DateTime utcStartDate, DateTime utcEndDate)
        {
            var evIds = DbManager
                .ExecuteList(
                new SqlQuery(_eventTable).Select("evt.id")
                    .Where(Exp.Eq("evt.calendar_id", calendarId) & Exp.Eq("evt.tenant", tenantId)
                            & ((Exp.Ge("evt.start_date", utcStartDate) & Exp.Le("evt.start_date", utcEndDate) & Exp.Eq("evt.rrule", "")
                               | !Exp.Eq("evt.rrule", "")))
                    )).Select(r => r[0]);

            return GetEventsByIds(evIds.ToArray(), userId);
        }

        public Event GetEventById(int eventId)
        {
            var events = GetEventsByIds(new object[] { eventId }, SecurityContext.CurrentAccount.ID);
            if (events.Count > 0)
                return events[0];

            return null;
        }

        public List<Event> GetEventsByIds(object[] evtIds, Guid userId)
        {
            var cc = new ColumnCollection();
            var eId = cc.RegistryColumn("evt.id");
            var eName = cc.RegistryColumn("evt.name");
            var eDescription = cc.RegistryColumn("evt.description");
            var eTenant = cc.RegistryColumn("evt.tenant");
            var eCalId = cc.RegistryColumn("evt.calendar_id");
            var eStartDate = cc.RegistryColumn("evt.start_date");
            var eEndDate = cc.RegistryColumn("evt.end_date");
            var eIsAllDay = cc.RegistryColumn("evt.all_day_long");
            var eRRule = cc.RegistryColumn("evt.rrule");
            var eOwner = cc.RegistryColumn("evt.owner_id");
            var usrAlertType = cc.RegistryColumn("evt_usr.alert_type");
            var eAlertType = cc.RegistryColumn("evt.alert_type");

            var data = DbManager.ExecuteList(new SqlQuery(_eventTable)
                                .LeftOuterJoin("calendar_event_user evt_usr", Exp.EqColumns(eId.Name, "evt_usr.event_id") & Exp.Eq("evt_usr.user_id", userId))
                                .Select(cc.SelectQuery).Where(Exp.In(eId.Name, evtIds)));

            var cc1 = new ColumnCollection();
            var evId = cc1.RegistryColumn("evt_itm.event_id");
            var itemId = cc1.RegistryColumn("evt_itm.item_id");
            var itemIsGroup = cc1.RegistryColumn("evt_itm.is_group");

            var sharingData = DbManager.ExecuteList(new SqlQuery(_eventItemTable).Select(cc1.SelectQuery)
                                                        .Where(Exp.In(evId.Name, evtIds)));

            //parsing           
            var events = new List<Event>();

            foreach (var r in data)
            {
                var ev = events.Find(e => String.Equals(e.Id, eId.Parse<string>(r), StringComparison.InvariantCultureIgnoreCase));
                if (ev == null)
                {
                    ev = new Event()
                    {
                        Id = eId.Parse<string>(r),
                        Name = eName.Parse<string>(r),
                        Description = eDescription.Parse<string>(r),
                        TenantId = eTenant.Parse<int>(r),
                        CalendarId = eCalId.Parse<string>(r),
                        UtcStartDate = eStartDate.Parse<DateTime>(r),
                        UtcEndDate = eEndDate.Parse<DateTime>(r),
                        AllDayLong = eIsAllDay.Parse<bool>(r),
                        OwnerId = eOwner.Parse<Guid>(r),
                        AlertType = (usrAlertType.IsNull(r)) ? (EventAlertType)eAlertType.Parse<int>(r) : (EventAlertType)usrAlertType.Parse<int>(r),
                        RecurrenceRule = eRRule.Parse<RecurrenceRule>(r)

                    };
                    events.Add(ev);
                }

                foreach (var row in sharingData)
                {
                    if (String.Equals(evId.Parse<string>(row), ev.Id, StringComparison.InvariantCultureIgnoreCase))
                    {
                        ev.SharingOptions.PublicItems.Add(new ASC.Web.Core.Calendars.SharingOptions.PublicItem()
                        {
                            Id = itemId.Parse<Guid>(row),
                            IsGroup = itemIsGroup.Parse<bool>(row)
                        });
                    }
                }
            }
            return events;
        }

        public void UnsubscribeFromEvent(int eventID, Guid userId)
        {
            using (var tr = DbManager.Connection.BeginTransaction())
            {
                if (DbManager.ExecuteNonQuery(new SqlDelete("calendar_event_item").Where(Exp.Eq("event_id", eventID)
                                                                                & Exp.Eq("item_id", userId)
                                                                                & Exp.Eq("is_group", false))) == 0)
                {
                    DbManager.ExecuteNonQuery(new SqlInsert("calendar_event_user", true).InColumnValue("event_id", eventID)
                                                                                        .InColumnValue("user_id", userId)
                                                                                        .InColumnValue("is_unsubscribe", true));
                }

                DbManager.ExecuteNonQuery(new SqlDelete("calendar_notifications").Where(Exp.Eq("event_id", eventID) & Exp.Eq("user_id", userId)));

                tr.Commit();
            }
        }

        public void RemoveEvent(int eventId)
        {
            using (var tr = DbManager.Connection.BeginTransaction())
            {

                DbManager.ExecuteNonQuery(new SqlDelete("calendar_events").Where("id", eventId));
                DbManager.ExecuteNonQuery(new SqlDelete("calendar_event_item").Where("event_id", eventId));
                DbManager.ExecuteNonQuery(new SqlDelete("calendar_event_user").Where("event_id", eventId));
                DbManager.ExecuteNonQuery(new SqlDelete("calendar_notifications").Where("event_id", eventId));

                tr.Commit();
            }
        }

        public Event CreateEvent(int calendarId, Guid ownerId, string name, string description,
                DateTime utcStartDate, DateTime utcEndDate, RecurrenceRule rrule, EventAlertType alertType, bool isAllDayLong, List<ASC.Web.Core.Calendars.SharingOptions.PublicItem> publicItems)
        {
            int eventId = 0;
            using (var tr = DbManager.Connection.BeginTransaction())
            {

                eventId = DbManager.ExecuteScalar<int>(new SqlInsert("calendar_events")
                                                    .InColumnValue("id", 0)
                                                    .InColumnValue("tenant", CoreContext.TenantManager.GetCurrentTenant().TenantId)
                                                    .InColumnValue("name", name)
                                                    .InColumnValue("description", description)
                                                    .InColumnValue("calendar_id", calendarId)
                                                    .InColumnValue("owner_id", ownerId)
                                                    .InColumnValue("start_date", utcStartDate.ToString("yyyy-MM-dd HH:mm:ss"))
                                                    .InColumnValue("end_date", utcEndDate.ToString("yyyy-MM-dd HH:mm:ss"))
                                                    .InColumnValue("all_day_long", isAllDayLong)
                                                    .InColumnValue("rrule", rrule.ToString())
                                                    .InColumnValue("alert_type", (int)alertType)
                                                    .Identity<int>(0, 0, true));

                foreach (var item in publicItems)
                {
                    DbManager.ExecuteScalar(new SqlInsert("calendar_event_item")
                                                .InColumnValue("event_id", eventId)
                                                .InColumnValue("item_id", item.Id)
                                                .InColumnValue("is_group", item.IsGroup));

                }

                //update notifications
                UpdateEventNotifications(eventId, calendarId, utcStartDate, alertType, rrule, publicItems, null);

                tr.Commit();
            }

            return GetEventById(eventId);
        }

        public Event UpdateEvent(int eventId, int calendarId, Guid ownerId, string name, string description,
                DateTime utcStartDate, DateTime utcEndDate, RecurrenceRule rrule, EventAlertType alertType, bool isAllDayLong, List<ASC.Web.Core.Calendars.SharingOptions.PublicItem> publicItems)
        {

            using (var tr = DbManager.Connection.BeginTransaction())
            {
                var query = new SqlUpdate("calendar_events")
                                .Set("name", name)
                                .Set("description", description)
                                .Set("calendar_id", calendarId)
                                .Set("owner_id", ownerId)
                                .Set("start_date", utcStartDate.ToString("yyyy-MM-dd HH:mm:ss"))
                                .Set("end_date", utcEndDate.ToString("yyyy-MM-dd HH:mm:ss"))
                                .Set("all_day_long", isAllDayLong)
                                .Set("rrule", rrule.ToString())
                                .Where(Exp.Eq("id", eventId));

                if (ownerId.Equals(SecurityContext.CurrentAccount.ID))
                    query = query.Set("alert_type", (int)alertType);
                else
                    DbManager.ExecuteNonQuery(new SqlInsert("calendar_event_user", true).InColumnValue("event_id", eventId)
                                                                                    .InColumnValue("user_id", SecurityContext.CurrentAccount.ID)
                                                                                    .InColumnValue("alert_type", alertType));


                DbManager.ExecuteNonQuery(query);

                var userIds = DbManager.ExecuteList(new SqlQuery("calendar_event_user").Select("user_id").Where("event_id", eventId)).Select(r => new Guid(Convert.ToString(r[0])));
                foreach (var usrId in userIds)
                {
                    if (!publicItems.Exists(i => (i.IsGroup && CoreContext.UserManager.IsUserInGroup(usrId, i.Id))
                                              || (!i.IsGroup && i.Id.Equals(usrId))))
                    {
                        DbManager.ExecuteNonQuery(new SqlDelete("calendar_event_user").Where(Exp.Eq("user_id", usrId) & Exp.Eq("event_id", eventId)));
                    }
                }

                DbManager.ExecuteNonQuery(new SqlDelete("calendar_event_item").Where("event_id", eventId));
                foreach (var item in publicItems)
                {
                    DbManager.ExecuteScalar(new SqlInsert("calendar_event_item")
                                                   .InColumnValue("event_id", eventId)
                                                   .InColumnValue("item_id", item.Id)
                                                   .InColumnValue("is_group", item.IsGroup));


                }

                //update notifications
                var baseAlertType = DbManager.ExecuteList(new SqlQuery("calendar_events").Select("alert_type").Where("id", eventId))
                                    .Select(r => (EventAlertType)Convert.ToInt32(r[0])).First();
                UpdateEventNotifications(eventId, calendarId, utcStartDate, baseAlertType, rrule, publicItems, null);


                tr.Commit();
            }

            return GetEventById(eventId);
        }

        #region Event Notifications

        internal static int GetBeforeMinutes(EventAlertType eventAlertType)
        {
            switch (eventAlertType)
            {
                case EventAlertType.Day:
                    return -24 * 60;
                case EventAlertType.FifteenMinutes:
                    return -15;
                case EventAlertType.FiveMinutes:
                    return -5;
                case EventAlertType.HalfHour:
                    return -30;
                case EventAlertType.Hour:
                    return -60;
                case EventAlertType.TwoHours:
                    return -120;
            }

            return 0;
        }

        private DateTime GetNextAlertDate(DateTime utcStartDate, RecurrenceRule rrule, EventAlertType eventAlertType, TimeZoneInfo timeZone)
        {
            if (eventAlertType == EventAlertType.Never)
                return DateTime.MinValue;

            var localFromDate = DateTime.UtcNow.Add(timeZone.GetUtcOffset(DateTime.UtcNow));
            var localStartDate = utcStartDate.Add(timeZone.GetUtcOffset(DateTime.UtcNow));
            var dates = rrule.GetDates(localStartDate, localFromDate, 3);
            for (var i = 0; i < dates.Count; i++)
            {
                dates[i] = dates[i].AddMinutes((-1) * (int)timeZone.GetUtcOffset(DateTime.UtcNow).TotalMinutes);
            }

            foreach (var d in dates)
            {
                var dd = d.AddMinutes(GetBeforeMinutes(eventAlertType));
                if (dd > DateTime.UtcNow)
                    return dd;
            }

            return DateTime.MinValue;
        }

        private class UserAlertType
        {
            public Guid UserId { get; set; }
            public EventAlertType AlertType { get; set; }
            public TimeZoneInfo TimeZone { get; set; }


            public UserAlertType(Guid userId, EventAlertType alertType, TimeZoneInfo timeZone)
            {
                this.UserId = userId;
                this.AlertType = alertType;
                this.TimeZone = timeZone;
            }
        }

        private void UpdateEventNotifications(int eventId, int calendarId, DateTime eventUtcStartDate, EventAlertType baseEventAlertType, RecurrenceRule rrule,
            IEnumerable<ASC.Web.Core.Calendars.SharingOptions.PublicItem> eventPublicItems,
            IEnumerable<ASC.Web.Core.Calendars.SharingOptions.PublicItem> calendarPublicItems)
        {
            var cc = new ColumnCollection();
            var userIdCol = cc.RegistryColumn("user_id");
            var alertTypeCol = cc.RegistryColumn("alert_type");
            var isUnsubscribeCol = cc.RegistryColumn("is_unsubscribe");

            var eventUsersData = DbManager.ExecuteList(new SqlQuery("calendar_event_user").Select(cc.SelectQuery).Where(Exp.Eq("event_id", eventId)));

            var calendarData = DbManager.ExecuteList(new SqlQuery("calendar_calendars").Select("alert_type", "owner_id", "time_zone").Where(Exp.Eq("id", calendarId)));
            var calendarAlertType = calendarData.Select(r => (EventAlertType)Convert.ToInt32(r[0])).First();
            Guid calendarOwner = calendarData.Select(r => new Guid(Convert.ToString(r[1]))).First();
            TimeZoneInfo calendarTimeZone = calendarData.Select(r => TimeZoneInfo.FindSystemTimeZoneById(Convert.ToString(r[2]))).First();

            List<UserAlertType> eventUsers = new List<UserAlertType>();

            #region shared event's data

            if (eventPublicItems == null)
            {
                eventPublicItems = new List<SharingOptions.PublicItem>(DbManager.ExecuteList(new SqlQuery("calendar_event_item").Select("item_id", "is_group").Where(Exp.Eq("event_id", eventId)))
                                                                       .Select(r => new ASC.Web.Core.Calendars.SharingOptions.PublicItem() { Id = new Guid(Convert.ToString(r[0])), IsGroup = Convert.ToBoolean(r[1]) }));
            }

            foreach (var item in eventPublicItems)
            {
                if (item.IsGroup)
                    eventUsers.AddRange(CoreContext.UserManager.GetUsersByGroup(item.Id).Select(u => new UserAlertType(u.ID, baseEventAlertType, calendarTimeZone)));
                else
                    eventUsers.Add(new UserAlertType(item.Id, baseEventAlertType, calendarTimeZone));
            }

            //remove calendar owner
            eventUsers.RemoveAll(u => u.UserId.Equals(calendarOwner));

            //remove unsubscribed and exec personal alert_type
            if (eventUsers.Count > 0)
            {
                foreach (var r in eventUsersData)
                {
                    if (isUnsubscribeCol.Parse<bool>(r))
                        eventUsers.RemoveAll(u => u.UserId.Equals(userIdCol.Parse<Guid>(r)));
                    else
                        eventUsers.ForEach(u =>
                        {
                            if (u.UserId.Equals(userIdCol.Parse<Guid>(r)))
                                u.AlertType = (EventAlertType)alertTypeCol.Parse<int>(r);
                        });

                }
            }

            //remove and exec sharing calendar options
            if (eventUsers.Count > 0)
            {
                var extCalendarAlertTypes = DbManager.ExecuteList(new SqlQuery("calendar_calendar_user cu")
                                                        .Select("cu.user_id", "cu.alert_type", "cu.is_accepted", "cu.time_zone")
                                                        .Where(Exp.Eq("cu.ext_calendar_id", SharedEventsCalendar.CalendarId) & Exp.In("cu.user_id", eventUsers.Select(u => u.UserId).ToArray())));

                foreach (var r in extCalendarAlertTypes)
                {
                    if (!Convert.ToBoolean(r[2]))
                    {
                        //remove unsubscribed from shared events calendar
                        eventUsers.RemoveAll(u => u.UserId.Equals(new Guid(Convert.ToString(r[0]))));
                        continue;
                    }
                    eventUsers.ForEach(u =>
                    {
                        if (u.UserId.Equals(new Guid(Convert.ToString(r[0]))))
                            u.TimeZone = ((r[3] == null || r[3] == DBNull.Value) ? calendarTimeZone : TimeZoneInfo.FindSystemTimeZoneById(Convert.ToString(r[3])));

                        if (u.AlertType == EventAlertType.Default && u.UserId.Equals(new Guid(Convert.ToString(r[0]))))
                            u.AlertType = (EventAlertType)Convert.ToInt32(r[1]);
                    });
                };

                eventUsers.ForEach(u =>
                {
                    if (u.AlertType == EventAlertType.Default)
                        u.AlertType = SharedEventsCalendar.AlertType;
                });

            }
            #endregion

            #region calendar's data

            if (calendarPublicItems == null)
            {
                calendarPublicItems = new List<SharingOptions.PublicItem>(DbManager.ExecuteList(new SqlQuery("calendar_calendar_item").Select("item_id", "is_group").Where(Exp.Eq("calendar_id", calendarId)))
                                                                       .Select(r => new ASC.Web.Core.Calendars.SharingOptions.PublicItem() { Id = new Guid(Convert.ToString(r[0])), IsGroup = Convert.ToBoolean(r[1]) }));
            }

            //calendar users
            List<UserAlertType> calendarUsers = new List<UserAlertType>();
            foreach (var item in eventPublicItems)
            {
                if (item.IsGroup)
                    calendarUsers.AddRange(CoreContext.UserManager.GetUsersByGroup(item.Id).Select(u => new UserAlertType(u.ID, baseEventAlertType, calendarTimeZone)));
                else
                    calendarUsers.Add(new UserAlertType(item.Id, baseEventAlertType, calendarTimeZone));
            }

            calendarUsers.Add(new UserAlertType(calendarOwner, baseEventAlertType, calendarTimeZone));

            //remove event's users
            calendarUsers.RemoveAll(u => eventUsers.Exists(eu => eu.UserId.Equals(u.UserId)));

            //calendar options            
            if (calendarUsers.Count > 0)
            {
                //set personal alert_type
                foreach (var r in eventUsersData)
                {
                    eventUsers.ForEach(u =>
                    {
                        if (u.UserId.Equals(new Guid(Convert.ToString(r[0]))))
                            u.AlertType = (EventAlertType)(Convert.ToInt32(r[1]));
                    });

                }



                var calendarAlertTypes = DbManager.ExecuteList(new SqlQuery("calendar_calendar_user")
                                                      .Select("user_id", "alert_type", "is_accepted", "time_zone")
                                                      .Where(Exp.Eq("calendar_id", calendarId) & Exp.In("user_id", calendarUsers.Select(u => u.UserId).ToArray())));


                foreach (var r in calendarAlertTypes)
                {
                    if (!Convert.ToBoolean(r[2]))
                    {
                        //remove unsubscribed
                        calendarUsers.RemoveAll(u => u.UserId.Equals(new Guid(Convert.ToString(r[0]))));
                        continue;
                    }
                    calendarUsers.ForEach(u =>
                    {
                        if (u.UserId.Equals(new Guid(Convert.ToString(r[0]))))
                            u.TimeZone = ((r[3] == null || r[3] == DBNull.Value) ? calendarTimeZone : TimeZoneInfo.FindSystemTimeZoneById(Convert.ToString(r[3])));

                        if (u.AlertType == EventAlertType.Default && u.UserId.Equals(new Guid(Convert.ToString(r[0]))))
                            u.AlertType = (EventAlertType)Convert.ToInt32(r[1]);
                    });
                };

                calendarUsers.ForEach(u =>
                {
                    if (u.AlertType == EventAlertType.Default)
                        u.AlertType = calendarAlertType;
                });
            }

            #endregion


            //clear notifications
            DbManager.ExecuteNonQuery(new SqlDelete("calendar_notifications").Where("event_id", eventId));

            eventUsers.AddRange(calendarUsers);

            foreach (var u in eventUsers)
            {
                ///todo: recount
                var alertDate = GetNextAlertDate(eventUtcStartDate, rrule, u.AlertType, u.TimeZone);
                if (!alertDate.Equals(DateTime.MinValue))
                {
                    DbManager.ExecuteNonQuery(new SqlInsert("calendar_notifications", true).InColumnValue("user_id", u.UserId)
                                                                                     .InColumnValue("event_id", eventId)
                                                                                     .InColumnValue("rrule", rrule.ToString())
                                                                                     .InColumnValue("alert_type", (int)u.AlertType)
                                                                                     .InColumnValue("tenant", CoreContext.TenantManager.GetCurrentTenant().TenantId)
                                                                                     .InColumnValue("notify_date", alertDate)
                                                                                     .InColumnValue("time_zone", u.TimeZone.Id));
                }
            }
        }

        public List<EventNotificationData> ExtractAndRecountNotifications(DateTime utcDate)
        {
            var data = new List<EventNotificationData>();

            using (var tr = DbManager.Connection.BeginTransaction())
            {
                var cc = new ColumnCollection();
                var userIdCol = cc.RegistryColumn("user_id");
                var tenantCol = cc.RegistryColumn("tenant");
                var eventIdCol = cc.RegistryColumn("event_id");
                var notifyDateCol = cc.RegistryColumn("notify_date");
                var rruleCol = cc.RegistryColumn("rrule");
                var alertTypeCol = cc.RegistryColumn("alert_type");
                var timeZoneCol = cc.RegistryColumn("time_zone");

                data = new List<EventNotificationData>(DbManager.ExecuteList(new SqlQuery("calendar_notifications").Select(cc.SelectQuery)
                                      .Where(Exp.Le(notifyDateCol.Name, utcDate)))
                                      .Select(r => new EventNotificationData()
                                      {
                                          UserId = userIdCol.Parse<Guid>(r),
                                          TenantId = tenantCol.Parse<int>(r),
                                          EventId = eventIdCol.Parse<int>(r),
                                          NotifyUtcDate = notifyDateCol.Parse<DateTime>(r),
                                          RRule = rruleCol.Parse<RecurrenceRule>(r),
                                          AlertType = (EventAlertType)alertTypeCol.Parse<int>(r),
                                          TimeZone = timeZoneCol.Parse<TimeZoneInfo>(r)
                                      }));


                var events = GetEventsByIds(data.Select(d => (object)d.EventId).Distinct().ToArray(), Guid.Empty);
                data.ForEach(d => d.Event = events.Find(e => String.Equals(e.Id, d.EventId.ToString(), StringComparison.InvariantCultureIgnoreCase)));

                foreach (var d in data)
                {
                    if (d.RRule.Freq == Frequency.Never)
                        DbManager.ExecuteNonQuery(new SqlDelete("calendar_notifications").Where(Exp.Eq("user_id", d.UserId) & Exp.Eq("event_id", d.EventId)));
                    else
                    {
                        var alertDate = GetNextAlertDate(d.Event.UtcStartDate, d.RRule, d.AlertType, d.TimeZone);
                        if (!alertDate.Equals(DateTime.MinValue))
                        {
                            DbManager.ExecuteNonQuery(new SqlInsert("calendar_notifications", true).InColumnValue("user_id", d.UserId)
                                                                                             .InColumnValue("event_id", d.EventId)
                                                                                             .InColumnValue("rrule", d.RRule.ToString())
                                                                                             .InColumnValue("alert_type", (int)d.AlertType)
                                                                                             .InColumnValue("tenant", d.TenantId)
                                                                                             .InColumnValue("notify_date", alertDate)
                                                                                             .InColumnValue("time_zone", d.TimeZone.Id));
                        }
                        else
                            DbManager.ExecuteNonQuery(new SqlDelete("calendar_notifications").Where(Exp.Eq("user_id", d.UserId) & Exp.Eq("event_id", d.EventId)));
                    }
                }

                tr.Commit();
            }


            return data;
        }

        #endregion
    }
}
