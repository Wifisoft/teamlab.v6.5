using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace ASC.Projects.Core.Domain
{
    [DataContract]
    public class Template
    {
        [DataMember(Name = "id")]
        public int Id { get; set; }

        [DataMember(Name = "title")]
        public string Title { get; set; }

        [DataMember(Name = "description")]
        public String Description { get; set; }

        public Guid CreateBy { get; set; }

        public DateTime CreateOn { get; set; }

        public Guid LastModifiedBy { get; set; }

        public DateTime LastModifiedOn { get; set; }
    }
}