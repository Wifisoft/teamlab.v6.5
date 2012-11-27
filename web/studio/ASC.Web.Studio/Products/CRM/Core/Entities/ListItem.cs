#region Import

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using ASC.CRM.Core.Dao;

#endregion

namespace ASC.CRM.Core.Entities
{
    [DataContract]
    public class ListItem : DomainObject
    {
        [DataMember(Name = "title")]
        public String Title { get; set; }

        [DataMember(Name = "description")]
        public String Description { get; set; }

        [DataMember(Name = "color")]
        public String Color { get; set; }
        
        [DataMember(Name = "sort_order")]
        public int SortOrder { get; set; }

        [DataMember(Name = "additional_params")]
        public String AdditionalParams { get; set; }

    }
}
