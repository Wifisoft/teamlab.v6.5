using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace ASC.Blogs.Core.Domain
{
	public class ReviewPost
    {
        private Guid _ReviewID;
        private Post _Post;
        private Guid _UserID;
        private DateTime _Timestamp;
        private int _Count;

        public virtual Guid ReviewID
        {
            get { return _ReviewID; }
            set { _ReviewID = value; }
        }
        public virtual Post Post
        {
            get { return _Post; }
            set { _Post = value; }
        }
        public virtual Guid UserID
        {
            get { return _UserID; }
            set { _UserID = value; }
        }
        public virtual DateTime Timestamp
        {
            get { return _Timestamp; }
            set { _Timestamp = value; }
        }
        public virtual int Count
        {
            get { return _Count; }
            set { _Count = value; }
        }
    }
}
