using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ASC.Web.Core.Calendars;

namespace ASC.Api.Calendar.iCalParser
{
    public class iCalEvent : BaseEvent
    {
        internal DateTime OriginalStartDate { get; set; }
        internal DateTime OriginalEndDate { get; set; }

        private string _name;
        public override string Name
        {
            get
            {
                if (String.IsNullOrEmpty(_name))
                    return Resources.CalendarApiResource.NoNameEvent;

                return _name;
            }
            set
            {
                _name = value;
            }
        }
    }
}
