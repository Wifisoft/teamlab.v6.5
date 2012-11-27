using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using ASC.Blogs.Core.Domain;

namespace ASC.Blogs.Core.Data
{

    public interface IPostDao
    {
        List<Post> Select(Guid? id, long? blogId, Guid? userId, string tag, bool withContent, bool asc, int? from, int? count, bool fillTags, bool withCommentsCount);

        List<Post> Select(Guid? id, long? blogId, Guid? userId, bool withContent, bool asc, int? from, int? count, bool fillTags, bool withCommentsCount);
        
        List<Post> Select(Guid? id, long? blogId, Guid? userId, bool withContent, bool fillTags, bool withCommentsCount);

        List<Post> GetPosts(List<Guid> ids,bool withContent,bool withTags);

        List<Guid> SearchPostsByWord(string word);

        void SavePost(Post post);
        void DeletePost(Guid postId);

        int GetCount(Guid? id, long? blogId, Guid? userId, string tag);

        List<Comment> GetComments(Guid postId);
        Comment GetCommentById(Guid commentId);
        
        List<int> GetCommentsCount(List<Guid> postsIds);

        List<Typle<Guid, int>> GetPostsCountByAuthor(Guid? authorId);
        List<Typle<Guid, int>> GetCommentsCountByAuthorPosts(Guid? authorId);
        List<Typle<Guid, int>> GetReviewCountByAuthorPosts(Guid? authorId);

        void SaveComment(Comment comment);

        void UpdateCommentText(Guid commentId, string newText);

        List<TagStat> GetTagStat(int? top);

        List<string> GetTags(string like, int limit);

        List<Typle<Comment, Post>> GetLastCommentedPosts(int? top);

        Dictionary<Guid, Typle<int, int>> GetCommentsStatistic(ICollection<Guid> posts, Guid forUser);

        void SavePostReview(Guid userId, Guid postId, DateTime datetime);
        IEnumerable<Post> GetPostsByDate(DateTime @from, DateTime to);
        IEnumerable<Comment> GetCommentsByDate(DateTime @from, DateTime to, Guid forUser);
    }

    

    public class Typle<T1, T2> { public T1 Value1; public T2 Value2;}
    public class Typle<T1, T2, T3> { public T1 Value1; public T2 Value2; public T3 Value3;}
    public class Typle<T1, T2, T3, T4> { public T1 Value1; public T2 Value2; public T3 Value3; public T4 Value4;}

    public static class PostDaoExtention
    {
        public static Post Get(this IPostDao pDao, Guid id)
        {
            return Get(pDao, id, true,false);
        }
        public static Post Get(this IPostDao pDao, Guid id, bool withTags, bool withCommentsCount)
        {
            var list = pDao.Select(id, null, null, true, withTags, withCommentsCount);
            if (list.Count == 1)
                return list[0];
            else
                return null;
        }

        public static int GetUserPostsCount(this IPostDao pDao, Guid userId, string tag)
        {
            return pDao.GetCount(null, null, userId, tag);
        }
        public static List<Post> GetUserPostsDesc(this IPostDao pDao, Guid userId, string tag, int? from, int? count)
        {
            return pDao.Select(null, null, userId,tag, true, false, from, count, true, true);
        }
        public static int GetPostsByTagCount(this IPostDao pDao, string tag)
        {
            return pDao.GetCount(null, null, null, tag);
        }
        public static int GetAllPostsCount(this IPostDao pDao)
        {
            return pDao.GetCount(null, null, null, null);
        }
        public static List<Post> GetAllPostsDesc(this IPostDao pDao, int? from, int? count)
        {
            return pDao.Select(null, null, null, null, true, false, from, count, true, true);
        }
        public static List<Post> GetPostsByTagDesc(this IPostDao pDao, string tag, int? from, int? count)
        {
            return pDao.Select(null, null, null, tag, true, false, from, count, true, true);
        }

        public static List<Typle<Post, Typle<int, int>>> GetCommentsStatistic(this IPostDao pDao,List<Post> posts, Guid forUser)
        {
            List<Guid> ids = posts.ConvertAll(p => p.ID);
            var dic = pDao.GetCommentsStatistic(ids, forUser);

            var result = new List<Typle<Post, Typle<int, int>>>(posts.Count);
            foreach (var post in posts)
            {
                result.Add(new Typle<Post, Typle<int, int>>
                {
                    Value1 = post,
                    Value2 = dic[post.ID]
                });
            }

            return result;
        }
    }

    public interface IBlogDao
    {
        List<Blog> Select(long? id, Guid? userId, Guid? groupId);

        int GetCount(Guid? userId, Guid? groupId);

        void SaveBlog(Blog blog);
    }

    public static class BlogDaoExtention
    {

        public static Blog Get(this IBlogDao dDao, long id)
        {
            var res = dDao.Select(id, null, null);
            if (res.Count == 1)
                return res[0];
            else
                return null;
        }

        public static List<Blog> GetUserBlogs(this IBlogDao dDao, Guid userID)
        {
            return dDao.Select(null, userID, null);
        }

        public static int GetCountAll(this IBlogDao dDao)
        {
            return dDao.GetCount(null, null);
        }
    }
}
