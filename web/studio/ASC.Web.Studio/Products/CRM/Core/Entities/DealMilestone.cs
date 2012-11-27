#region Import

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using ASC.Common.Security;
using ASC.Common.Security.Authorizing;

#endregion

namespace ASC.CRM.Core.Entities
{

    [DataContract]
    public class DealMilestone : DomainObject
    {
        [DataMember(Name = "title")]
        public String Title { get; set; }

        [DataMember(Name = "description")]
        public String Description { get; set; }

        [DataMember(Name = "color")]
        public String Color { get; set; }

        [DataMember(Name = "sort_order")]
        public int SortOrder { get; set; }

        [DataMember(Name = "probability")]
        public int Probability { get; set; }

        [DataMember(Name = "status")]
        public DealMilestoneStatus Status { get; set; }

    }
}