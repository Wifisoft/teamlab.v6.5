using System;
using System.Collections.Generic;
using ASC.Blogs.Core.Security;
using ASC.Common.Security;
using ASC.Common.Security.Authorizing;
using ASC.Core.Users;
using System.Reflection;

namespace ASC.Blogs.Core.Domain
{
    public class CommentStat {
        public Guid PostId;
        public Guid ForUserID;
        public int TotalCount;
        public int UndreadCount;
    }

	public class Comment 
        : ISecurityObject
    {
        #region members

        private Guid _ID;
        private Guid _UserID;
        private string _Content;
        private DateTime _Datetime;
        private Post _Post;
        private bool inactive;
        private IList<Comment> _CommentList = new List<Comment>();

        #endregion

        public Comment() { }
        public Comment(Post post)
        {
            _Post = post;
            //_Blog.AddComment(this);
        }


        public bool IsRoot() { return ParentId == Guid.Empty; }
        public List<Comment> SelectChildLevel(List<Comment> from)
        {
            return SelectChildLevel(ID, from);
        }
        public static List<Comment> SelectRootLevel(List<Comment> from)
        {
            return SelectChildLevel(Guid.Empty, from);
        }
        public static List<Comment> SelectChildLevel(Guid forParentId, List<Comment> from)
        {
            return from.FindAll(comm => comm.ParentId == forParentId);
        }
        

        #region Properties
        public Guid PostId { get; set; }
        public Guid ParentId { get; set; }
       
        public virtual Guid ID
        {
            get { return _ID; }
            set { _ID = value; }
        }
        public virtual Post Post
        {
            get { return _Post; }
            protected set { _Post = value; }
        }
        public virtual bool Inactive
        {
            get { return inactive; }
            set { inactive = value; }
        }
        public virtual Guid UserID
        {
            get { return _UserID; }
            set { _UserID = value; }
        }
        public virtual string Content
        {
            get { return _Content; }
            set { _Content = value; }
        }
        public virtual DateTime Datetime
        {
            get { return _Datetime; }
            set { _Datetime = value; }
        }

        public virtual bool IsReaded { get; set; }
        
        public virtual IList<Comment> CommentList
        {
            get { return new List<Comment>(_CommentList).AsReadOnly(); }
            protected set { _CommentList = value; }
        }

        #endregion

        #region Methods

        public override int GetHashCode()
        {
            return (GetType().FullName + "|" +
                    _ID.ToString()).GetHashCode();
        }

        #endregion

        #region ISecurityObjectId Members

        public Type ObjectType
        {
            get { return this.GetType(); }
        }

        public object SecurityId
        {
            get { return this.ID; }
        }

        #endregion

        #region ISecurityObjectProvider Members

        public IEnumerable<ASC.Common.Security.Authorizing.IRole> GetObjectRoles(ASC.Common.Security.Authorizing.ISubject account, ISecurityObjectId objectId, SecurityCallContext callContext)
        {
            var roles = new List<IRole>();
            if (Equals(account.ID, this.UserID))
            {
                roles.Add(ASC.Common.Security.Authorizing.Constants.Owner);
            }
            return roles;
        }

        public ISecurityObjectId InheritFrom(ISecurityObjectId objectId)
        {
            throw new NotImplementedException();
        }

        public bool InheritSupported
        {
            get { return false; }
        }

        public bool ObjectRolesSupported
        {
            get { return true; }
        }

        #endregion
    }

    public class CommentSecurityObjectProvider
        :ISecurityObjectProvider
    {
        Guid _author = Guid.Empty;

        public CommentSecurityObjectProvider(Comment comment)
        {
            _author = comment.UserID;
        }
        public CommentSecurityObjectProvider(Guid author)
        {
            _author = author;
        }

        #region ISecurityObjectProvider

        public IEnumerable<IRole> GetObjectRoles(ISubject account, ISecurityObjectId objectId, SecurityCallContext callContext)
        {
            var roles = new List<IRole>();
            if (Equals(account.ID, _author))
                roles.Add(ASC.Common.Security.Authorizing.Constants.Owner);
            return roles;
        }

        public ISecurityObjectId InheritFrom(ISecurityObjectId objectId)
        {
            throw new NotImplementedException();
        }

        public bool InheritSupported
        {
            get { return false; }
        }

        public bool ObjectRolesSupported
        {
            get { return true; }
        }


        #endregion
    }
}
