#region Import

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ASC.Common.Security;

#endregion

namespace ASC.CRM.Core.Entities
{
    public class Task : DomainObject, ISecurityObjectId
    {

        public Guid CreateBy { get; set; }

        public DateTime CreateOn { get; set; }    

        public int ContactID { get; set; }

        public String Title { get; set; }

        public String Description { get; set; }

        public DateTime DeadLine { get; set; }

        public Guid ResponsibleID { get; set; }
      
        public bool IsClosed { get; set; }

        public int CategoryID { get; set; }

        public EntityType EntityType { get; set; }

        public int EntityID { get; set; }

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