using System;
using System.Collections.Generic;
using System.Reflection;
using ASC.Common.Security;
using ASC.Common.Security.Authorizing;
using ASC.Core;
using ASC.Core.Users;

namespace ASC.Forum
{
    public enum PostTextFormatter
    { 
        BBCode = 0,
        FCKEditor =1
    }

	public class Post : ISecurityObject
    {
        public virtual int ID { get; set; }

        public virtual Guid PosterID { get; set; }

        public virtual UserInfo Poster
        {
            get
            {
                return ASC.Core.CoreContext.UserManager.GetUsers(PosterID);
            }
        }

        public virtual Guid EditorID { get; set; }

        public virtual UserInfo Editor
        {
            get
            {
                return ASC.Core.CoreContext.UserManager.GetUsers(EditorID);
            }
        }

        public virtual string Subject { get; set; }

        public virtual string Text { get; set; }

        public virtual DateTime CreateDate { get; set; }

        public virtual DateTime EditDate { get; set; }

        public virtual int EditCount { get; set; }       

        public virtual bool IsApproved { get; set; }

        public virtual List<Attachment> Attachments { get; set; }

        public virtual PostTextFormatter Formatter { get; set; }

        public virtual int TenantID { get; set; }

        public Post()
        {
            Attachments = new List<Attachment>();
            CreateDate = ASC.Core.Tenants.TenantUtil.DateTimeNow();
            EditDate = DateTime.MinValue;            
            EditCount = 0;
            Formatter = PostTextFormatter.BBCode;
            PosterID = SecurityContext.CurrentAccount.ID;
        }

        public virtual int TopicID { get; set; }

        public virtual int ParentPostID { get; set; }

        public Post(string subject, string text)
        {
            Attachments = new List<Attachment>();
            CreateDate = ASC.Core.Tenants.TenantUtil.DateTimeNow();
            EditDate = DateTime.MinValue;            
            EditCount = 0;
            Subject = subject;
            Text = text;
            PosterID = SecurityContext.CurrentAccount.ID;
        }

        #region ISecurityObjectId Members

		/// <inheritdoc/>
        public object SecurityId
        {
            get { return this.ID; }
        }

		/// <inheritdoc/>
        public Type ObjectType
        {
            get { return this.GetType(); }
        }

        #endregion




        #region ISecurityObjectProvider Members

		/// <inheritdoc/>
        public IEnumerable<IRole> GetObjectRoles(ISubject account, ISecurityObjectId objectId, SecurityCallContext callContext)
        {
            List<IRole> roles = new List<IRole>();
            if (account.ID.Equals(this.PosterID))
                roles.Add(ASC.Common.Security.Authorizing.Constants.Owner);

            return roles;
        }

		/// <inheritdoc/>
        public ISecurityObjectId InheritFrom(ISecurityObjectId objectId)
        {
            return new Topic() {ID = this.TopicID };
        }

		/// <inheritdoc/>
        public bool InheritSupported
        {
            get { return true; }
        }

		/// <inheritdoc/>
        public bool ObjectRolesSupported
        {
            get { return true; }
        }

        #endregion
    }
}
