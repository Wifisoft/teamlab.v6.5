using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace ASC.Blogs.Core.Domain
{
    public class TagStat {
        public string Name;
        public int Count;
    }
	public class Tag
    {
        private Guid _ID;
        //private Guid _UserID;
        private string _Content;
        //private DateTime _Datetime;
        private Post _Post;

        public Guid PostId;

        public Tag() { }

        public Tag(Post blog)
        {
            _Post = blog;
            //_Blog.AddTag(this);
        }

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
        //public virtual Guid UserID
        //{
        //    get { return _UserID; }
        //    set { _UserID = value; }
        //}
        public virtual string Content
        {
            get { return _Content; }
            set { _Content = value; }
        }
        //public virtual DateTime Datetime
        //{
        //    get { return _Datetime; }
        //    set { _Datetime = value; }
        //}

        /// <summary>
        /// Hash code should ONLY contain the "business value signature" of the object and not the ID
        /// </summary>
        public override int GetHashCode()
        {
            return (GetType().FullName + "|" +
                    _ID.ToString()).GetHashCode();
        }

    }
}
