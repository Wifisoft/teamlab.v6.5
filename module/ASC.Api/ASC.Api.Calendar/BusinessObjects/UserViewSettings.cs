using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ASC.Web.Core.Calendars;

namespace ASC.Api.Calendar.BusinessObjects
{
    public class UserViewSettings
    { 
        public virtual string CalendarId { get; set; }
        public Guid UserId { get; set; }
        public string Name { get; set; }      
        public bool IsHideEvents { get; set; }
        public string TextColor { get; set; }
        public string BackgroundColor { get; set; }
        public bool IsAccepted { get; set; }        
        public EventAlertType EventAlertType { get; set; }
        public TimeZoneInfo TimeZone { get; set; }

        public UserViewSettings()
        {
            this.TextColor = String.Empty;
            this.BackgroundColor = String.Empty;
        }
    }
}
