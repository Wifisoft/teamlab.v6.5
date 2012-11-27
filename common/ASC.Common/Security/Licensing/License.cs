using System;
using System.Collections.Generic;

namespace ASC.Common.Security.Licensing
{
    public class License
    {
        public DateTime Issued { get; set; }
        public DateTime ValidTo { get; set; }
        public string Email { get; set; }
        public Dictionary<string, object> Params { get; set; }
        public DateTime TimeNow { get; set; }
        public DateTime? RestartAt { get; set; }
        public DateTime? RefreshAt { get; set; }
    }
}