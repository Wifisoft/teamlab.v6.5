using System;

namespace ASC.Web.UserControls.Forum.Common
{   
    public enum ForumPage
    {
        Default,
        TopicList,
        PostList,
        NewPost,
        EditTopic,
        Search,
        UserProfile,
        ManagementCenter
    }

    public class PageLocation : ICloneable
    {
        public ForumPage Page{get; set;}
        public string Url { get; set;}
        
        public PageLocation(ForumPage page, string url)
        {
            this.Page = page;
            this.Url = url;
        }

        #region ICloneable Members

        public object Clone()
        {
            return new PageLocation(this.Page, this.Url);
        }

        #endregion
    }
}
