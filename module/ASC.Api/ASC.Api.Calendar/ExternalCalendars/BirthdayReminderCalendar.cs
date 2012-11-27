using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ASC.Web.Core.Calendars;
using ASC.Core;
using ASC.Core.Users;

namespace ASC.Api.Calendar.ExternalCalendars
{
    public class BirthdayReminderCalendar : BaseCalendar
    {
        public readonly static string CalendarId = "users_birthdays";

        public BirthdayReminderCalendar()
        {
            this.Id = CalendarId;
            this.Context.HtmlBackgroundColor = "#f08e1c";
            this.Context.HtmlTextColor = "#000000";
            this.Context.GetGroupMethod = delegate(){return Resources.CalendarApiResource.CommonCalendarsGroup;};
            this.Context.CanChangeTimeZone = false;
            this.EventAlertType = EventAlertType.Day;
            this.SharingOptions.SharedForAll = true;
        }

        private class BirthdayEvent : BaseEvent
        {           
            public BirthdayEvent(string id, string name, DateTime birthday)
            {
                this.Id = "bde_"+id;
                this.Name= name;                
                this.OwnerId = Guid.Empty;
                this.AlertType = EventAlertType.Day;
                this.AllDayLong = true;
                this.CalendarId = BirthdayReminderCalendar.CalendarId;
                this.UtcEndDate = birthday;
                this.UtcStartDate = birthday;
                this.RecurrenceRule.Freq = Frequency.Yearly;
            }
        }
       
        public override List<IEvent> LoadEvents(Guid userId, DateTime utcStartDate, DateTime utcEndDate)
        {
            var events = new List<IEvent>();
            foreach (var usr in CoreContext.UserManager.GetUsers())
            {
                if (usr.BirthDate.HasValue)
                {
                    var bd = new DateTime(utcStartDate.Year,usr.BirthDate.Value.Month, usr.BirthDate.Value.Day);

                    if (bd >= utcStartDate && bd <= utcEndDate)
                    {
                        events.Add(new BirthdayEvent(usr.ID.ToString(), usr.DisplayUserName(), usr.BirthDate.Value));
                        continue;
                    }

                    bd = new DateTime( utcEndDate.Year,usr.BirthDate.Value.Month, usr.BirthDate.Value.Day);

                    if (bd >= utcStartDate && bd <= utcEndDate)
                        events.Add(new BirthdayEvent(usr.ID.ToString(), usr.DisplayUserName(), usr.BirthDate.Value));
                }
            }
            return events;
        }

        public override string Name
        {
            get { return Resources.CalendarApiResource.BirthdayCalendarName; }
        }

        public override string Description
        {
            get { return Resources.CalendarApiResource.BirthdayCalendarDescription; }
        }
        
        public override TimeZoneInfo TimeZone
        {
            get { return CoreContext.TenantManager.GetCurrentTenant().TimeZone;}
        }
    }
}
