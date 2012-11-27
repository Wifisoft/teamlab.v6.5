#region Import

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

#endregion

namespace ASC.CRM.Core
{
    [DataContract]
    [Serializable]
    public class DomainObject
    {
     
        [DataMember(Name = "id")]
        public virtual int ID
        {
            get;
            set;
        }

        public override int GetHashCode()
        {
            return (GetType().FullName + "|" + ID.GetHashCode()).GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var compareTo = obj as DomainObject;
            return compareTo != null && (
                (!IsTransient() && !compareTo.IsTransient() && ID.Equals(compareTo.ID)) ||
                ((IsTransient() || compareTo.IsTransient()) && GetHashCode().Equals(compareTo.GetHashCode())));
        }

        private bool IsTransient()
        {
            return ID.Equals(default(int));
        }
        
    }
}
