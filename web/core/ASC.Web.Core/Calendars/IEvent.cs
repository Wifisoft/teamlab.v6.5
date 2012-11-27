﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ASC.Web.Core.Calendars
{
    [System.AttributeUsage(System.AttributeTargets.Class,
                   AllowMultiple = false,
                   Inherited = true)]
    public class AllDayLongUTCAttribute : Attribute
    { }

    public enum EventRepeatType
    {
        Never = 0,
        EveryDay = 3,
        EveryWeek = 4,
        EveryMonth = 5,
        EveryYear = 6
    }

    public enum EventAlertType
    {
        Default = -1,
        Never = 0,
        FiveMinutes = 1,
        FifteenMinutes = 2,
        HalfHour = 3,
        Hour = 4,
        TwoHours = 5,
        Day = 6
    }
    

    public class EventContext : ICloneable
    {
        //public EventRepeatType RepeatType { get; set; }

        #region ICloneable Members

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        #endregion
    }

    public interface IEvent : IiCalFormatView
    {
        string Id { get; }
        string CalendarId { get; }
        string Name { get; }
        string Description { get; }
        Guid OwnerId { get; }
        DateTime UtcStartDate { get; }
        DateTime UtcEndDate { get; }        
        EventAlertType AlertType { get; }
        bool AllDayLong { get; }
        RecurrenceRule RecurrenceRule { get; }
        EventContext Context { get; }
        SharingOptions SharingOptions { get;}
    }
}
