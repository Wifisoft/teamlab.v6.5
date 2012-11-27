using System;
using System.Collections.Generic;
using System.Reflection;
using ASC.Common.Security;
using ASC.Common.Security.Authorizing;
using ASC.Core;
using ASC.Core.Users;
using System.Web;
using System.Collections;

namespace ASC.Forum
{
    public enum TopicType
    {
        Informational = 0,

        Poll = 1
    }

    [Flags]
    public enum TopicStatus
    {
        Normal = 0,

        Closed = 1,

        Sticky = 2
    }

    public class Topic : ISecurityObject
    {
        public virtual int ID { get; set; }

        public virtual string Title { get; set; }

        public virtual TopicType Type { get; set; }

        public virtual TopicStatus Status
        {
            get
            {
                TopicStatus status = TopicStatus.Normal;
                if (Closed)
                    status |= TopicStatus.Closed;
                if (Sticky)
                    status |= TopicStatus.Sticky;

                return status;

            }
        }

        public virtual Guid PosterID { get; set; }

        public virtual UserInfo Poster
        {
            get
            {
                return ASC.Core.CoreContext.UserManager.GetUsers(PosterID);
            }
        }

        public virtual UserInfo RecentPostAuthor
        {
            get
            {
                return ASC.Core.CoreContext.UserManager.GetUsers(RecentPostAuthorID);
            }
        }

        public virtual DateTime CreateDate { get; set; }

        public virtual bool IsApproved { get; set; }

        public virtual int ViewCount { get; set; }

        public virtual int PostCount { get; set; }

        public virtual bool Closed { get; set; }

        public virtual bool Sticky { get; set; }

        public Topic()
        {
            CreateDate = DateTime.MinValue;
            Tags = new List<Tag>(0);
        }

        public virtual List<Tag> Tags { get; set; }

        public virtual int ThreadID { get; set; }

        public virtual int RecentPostID { get; set; }

        public virtual int TenantID { get; set; }

        public virtual int QuestionID { get; set; }

        public virtual string RecentPostText { get; set; }

        public virtual DateTime RecentPostCreateDate { get; set; }

        public virtual Guid RecentPostAuthorID { get; set; }

        public virtual PostTextFormatter RecentPostFormatter { get; set; }

        public virtual string ThreadTitle { get; set; }

        public Topic(string title, TopicType type)
        {
            Title = title;
            Type = type;
            Tags = new List<Tag>(0);
        }

        public bool IsNew()
        {
            var tvi = ThreadVisitInfo.GetThreadVisitInfo(this.ThreadID);
            if (tvi == null)
                return true;

            if (tvi.TopicViewRecentPostIDs.ContainsKey(this.ID) && this.RecentPostID > 0)
            {
                if (tvi.TopicViewRecentPostIDs[this.ID] >= this.RecentPostID)
                    return false;
            }
            else if (this.RecentPostID > 0 && tvi.RecentVisitDate.CompareTo(this.RecentPostCreateDate) >= 0)
                return false;

            else if (this.PostCount == 0)
                return false;

            return true;
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
            {
                if (callContext.ObjectsStack == null || callContext.ObjectsStack.Find(so => so.ObjectType.Equals(typeof(Post))) == null)
                    roles.Add(ASC.Common.Security.Authorizing.Constants.Owner);
            }

            return roles;
        }

        /// <inheritdoc/>
        public ISecurityObjectId InheritFrom(ISecurityObjectId objectId)
        {
            return new Thread() { ID = this.ThreadID };
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
