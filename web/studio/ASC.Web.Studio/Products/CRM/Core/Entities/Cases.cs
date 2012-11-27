#region Import

using System;
using ASC.Common.Security;

#endregion

namespace ASC.CRM.Core.Entities
{

    public class Cases : DomainObject, ISecurityObjectId
    {
        public Guid CreateBy { get; set; }

        public DateTime CreateOn { get; set; }

        public String Title { get; set; }

        public bool IsClosed { get; set; }

        // TODO: to finish the field
        public DateTime ClosedDate { get; set; }

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