#region Import

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using ASC.Common.Security;

#endregion

namespace ASC.CRM.Core.Entities
{
    [DataContract]
    public class Report
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