using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using ASC.Web.Core.Calendars;

namespace ASC.Api.Calendar.Wrappers
{
   
    [DataContract(Name = "alert", Namespace = "")]
    public class EventAlertWrapper
    {
        [DataMember(Name = "type")]
        public int Type{ get; set; }

        public static EventAlertWrapper ConvertToTypeSurrogated(EventAlertType type)
        {
            return new EventAlertWrapper() { Type = (int)type };
        }

        public static object GetSample()
        {
            return new { type = -1 };
        }
    }
}
