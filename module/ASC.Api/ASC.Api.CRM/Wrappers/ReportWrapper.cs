#region Import

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

#endregion

namespace ASC.Api.CRM.Wrappers
{
    [DataContract]
    public class ReportWrapper
    {
        [DataMember]
        public String ReportTitle { get; set; }

        [DataMember]
        public String ReportDescription { get; set; }

        [DataMember]
        public IEnumerable<String> Lables { get; set; }

        [DataMember]
        public Object Data { get; set; }
    }
}
