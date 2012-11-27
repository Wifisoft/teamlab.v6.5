using System;
using System.Diagnostics;
using ASC.Common.Security.Authorizing;

namespace ASC.Common.Security
{
    [DebuggerDisplay("ObjectType: {ObjectType.Name}, SecurityId: {SecurityId}")]
    public class SecurityObjectId : ISecurityObjectId
    {
        public object SecurityId { get; private set; }

        public Type ObjectType { get; private set; }


        public SecurityObjectId(object id, Type objType)
        {
            if (objType == null) throw new ArgumentNullException("objType");

            SecurityId = id;
            ObjectType = objType;
        }

        public override int GetHashCode()
        {
            return AzObjectIdHelper.GetFullObjectId(this).GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var other = obj as SecurityObjectId;
            return other != null &&
                   Equals(AzObjectIdHelper.GetFullObjectId(other), AzObjectIdHelper.GetFullObjectId(this));
        }
    }
}