using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace ASC.Api.Calendar.Wrappers
{
    [DataContract(Name = "timeZone", Namespace = "")]
    public class TimeZoneWrapper
    {
        private TimeZoneInfo _timeZone;
        public TimeZoneWrapper(TimeZoneInfo timeZone)
        {
            _timeZone = timeZone;
        }

        [DataMember(Name = "name", Order = 0)]
        public string Name
        {
            get
            {
                return _timeZone.DisplayName;
            }
            set { }
        }

        [DataMember(Name = "id", Order = 0)]
        public string Id
        {
            get
            {
                return _timeZone.Id;
            }
            set { }
        }

        [DataMember(Name = "offset", Order = 0)]
        public int Offset
        {
            get
            {
                return (int)_timeZone.GetUtcOffset(DateTime.UtcNow).TotalMinutes;
            }
            set { }
        }

        public static object GetSample()
        {
            return new { offset = 0, id = "UTC", name = "UTC" };
        }


    }

}
