using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace ASC.Blogs.Core.Domain
{
	public class Blog
    {
        private long blogID;
        private string name;
        private Guid userID;
        private Guid groupID;
        private IList<Guid> memberList = new List<Guid>();
        private IList<Post> posts = new List<Post>();

        public virtual long BlogID
        {
            get { return blogID; }
            set { blogID = value; }
        }
        public virtual BlogType BlogType
        {
            get { return this.GroupID.Equals(Guid.Empty) ? BlogType.Corporate : BlogType.Personal; }
        }
        public virtual string Name
        {
            get { return name; }
            set { name = value; }
        }
        public virtual Guid UserID
        {
            get { return userID; }
            set { userID = value; }
        }
        public virtual Guid GroupID
        {
            get { return groupID; }
            set { groupID = value; }
        }
        public virtual IList<Post> Posts
        {
            get { return posts; }
            set { posts = value; }
        }

        public virtual IList<Guid> MemberList
        {
            get { return memberList; }
            set { memberList = value; }
        }
    }
}
