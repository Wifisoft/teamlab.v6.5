using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ASC.Web.Core.Calendars;

namespace ASC.Api.Calendar.BusinessObjects
{
    public class EventNotificationData
    {
        public int TenantId { get; set; }
        public Guid UserId { get; set; }
        public int EventId { get; set; }
        public Event Event { get; set; }
        public DateTime NotifyUtcDate { get; set; }
        public RecurrenceRule RRule { get; set; }
        public EventAlertType AlertType { get; set; }
        public TimeZoneInfo TimeZone { get; set; }

        public EventNotificationData()
        {
            RRule = new RecurrenceRule();
        }

        public DateTime GetUtcStartDate()
        {
            if(Event==null)
                return DateTime.MinValue;

            if(RRule.Freq == Frequency.Never)
                return Event.UtcStartDate;

            return NotifyUtcDate.AddMinutes((-1) * DataProvider.GetBeforeMinutes(AlertType));
            
        }

        public DateTime GetUtcEndDate()
        {
             if(Event==null)
                return DateTime.MinValue;

             if (RRule.Freq == Frequency.Never || Event.AllDayLong || Event.UtcEndDate == DateTime.MinValue)
                return Event.UtcEndDate;
            
            return GetUtcStartDate().Add(Event.UtcEndDate - Event.UtcStartDate);
        }

    }
}
