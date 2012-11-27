using System;
using System.Collections.Generic;
using ASC.Common.Security;
using ASC.Common.Security.Authorizing;

namespace ASC.Bookmarking.Business.Permissions
{
    public class BookmarkPermissionSecurityObject : ISecurityObject
    {
        public Type ObjectType
        {
            get { return GetType(); }
        }

        public object SecurityId { get; set; }

        public Guid CreatorID { get; set; }


        public BookmarkPermissionSecurityObject(Guid userID, Guid id)
        {
            CreatorID = userID;
            SecurityId = id;
        }

        public BookmarkPermissionSecurityObject(Guid userID)
            : this(userID, Guid.NewGuid())
        {
        }

        public bool ObjectRolesSupported
        {
            get { return true; }
        }

        public IEnumerable<IRole> GetObjectRoles(ASC.Common.Security.Authorizing.ISubject account, ISecurityObjectId objectId, SecurityCallContext callContext)
        {
            return account.ID == CreatorID ? new[] { Constants.Owner } : new IRole[0];
        }

        public ISecurityObjectId InheritFrom(ISecurityObjectId objectId)
        {
            throw new NotImplementedException();
        }

        public bool InheritSupported
        {
            get { return false; }
        }
    }
}
