using System;
using System.Collections.Generic;
using System.Text;
using ASC.Common.Data;

namespace ASC.Blogs.Core.Data
{
    public class BlogsStorage
        :IDisposable
    {

        readonly DbManager _db = null;
        readonly int _tenant = 0;

        IBlogDao _blogDao = null;
        IPostDao _postDao = null;
        public BlogsStorage(string dbId,int tenant)
        {
            _db = new DbManager(dbId);
            _tenant = tenant;

            _blogDao = new DbBlogDao(_db, _tenant);
            _postDao = new DbPostDao(_db, _tenant); 
        }


        public IBlogDao GetBlogDao() 
        {
            return _blogDao;
        }

        public IPostDao GetPostDao()
        {
            return _postDao;
        }

        #region IDisposable Members

        public void Dispose()
        {
            _db.Dispose();
        }

        #endregion
    }
}
