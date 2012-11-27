#region Import

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using ASC.Common.Security;
using ASC.Common.Security.Authorizing;
using ASC.Core.Users;
#endregion

namespace ASC.CRM.Core.Entities
{
    public class RelationshipEvent : DomainObject, ISecurityObjectId
    {
        public Guid CreateBy { get; set; }

        public DateTime CreateOn { get; set; }    


        public String Content { get; set; }

        public int ContactID { get; set; }

        public EntityType EntityType { get; set; }

        public int EntityID { get; set; }

        public int CategoryID { get; set; }

        public object SecurityId
        {
            get { return ID; }
        }

        public Type ObjectType
        {
            get { return GetType(); }
        }
    }
}