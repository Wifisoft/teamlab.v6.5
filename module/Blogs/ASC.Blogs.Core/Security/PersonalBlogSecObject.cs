using System;
using ASC.Common.Security;
using ASC.Common.Security.Authorizing;
using ASC.Core.Users;
using System.Collections.Generic;
using ASC.Blogs.Core.Domain;

namespace ASC.Blogs.Core.Security
{
    public class PersonalBlogSecObject : SecurityObjectId, ISecurityObject
    {
        private UserInfo blogOwner;

        public PersonalBlogSecObject()
            : base((int)BlogType.Personal, typeof(BlogType))
        {

        }

        public PersonalBlogSecObject(UserInfo blogOwner)
            : this()
        {

            this.blogOwner = blogOwner;
        }

        public override string ToString()
        {
            return "personal blog";
        }

        #region ISecurityObjectProvider Members

        public bool InheritSupported
        {
            get { return false; }
        }

        public bool ObjectRolesSupported
        {
            get { return blogOwner != null; }
        }

        public ISecurityObjectId InheritFrom(ISecurityObjectId objectId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IRole> GetObjectRoles(ISubject account, ISecurityObjectId objectId, SecurityCallContext callContext)
        {
            var roles = new List<IRole>();
            if (blogOwner != null && blogOwner.ID.Equals(account.ID))
            {
                roles.Add(ASC.Common.Security.Authorizing.Constants.Owner);
            }
            return roles;
        }

        #endregion
    }
}
