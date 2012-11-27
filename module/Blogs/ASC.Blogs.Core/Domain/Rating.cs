using System;
using System.Text;

namespace ASC.Blogs.Core.Domain
{
    public class Rating
    {
        private string _Name;
        private Guid _ID;
        private int _BlogsCount;
        private int _CommentsCount;
        private Post _lastBlog;

        public Rating()
        { }
        public virtual Post LastBlog
        {
            get { return _lastBlog; }
            set { _lastBlog = value; }
        }
        public virtual string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }
        public virtual Guid ID
        {
            get { return _ID; }
            set { _ID = value; }
        }
        public virtual int BlogsCount
        {
            get { return _BlogsCount; }
            set { _BlogsCount = value; }
        }
        public virtual int CommentsCount
        {
            get { return _CommentsCount; }
            set { _CommentsCount = value; }
        }
    }
}
