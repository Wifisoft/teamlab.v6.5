using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ASC.Blogs.Core;
using ASC.Blogs.Core.Resources;
using ASC.Core;
using ASC.Web.Community.Product;
using ASC.Web.Core.ModuleManagement.Common;
using ASC.Web.Core.Utility;
using ASC.Web.Core.Utility.Skins;
using ASC.Web.Core.Users;
using ASC.Web.Studio.Controls.Common;

namespace ASC.Web.Community.Blogs
{
    public class BlogsSearchHandler : BaseSearchHandlerEx
    {

        public override SearchResultItem[] Search(string text)
        {

            var posts = BasePage.GetEngine().SearchPosts(text, new PagingQuery());
            var result = new List<SearchResultItem>(posts.Count);
            result.AddRange(posts.Select(post => new SearchResultItem
                                                     {
                                                         Description = BlogsResource.Blogs + ", " + DisplayUserSettings.GetFullUserName(CoreContext.UserManager.GetUsers(post.UserID), false) + ", " + post.Datetime.ToLongDateString(),
                                                         Name = post.Title,
                                                         URL = VirtualPathUtility.ToAbsolute(Constants.BaseVirtualPath) + "viewblog.aspx?blogid=" + post.ID.ToString(),
                                                         Date = post.Datetime
                                                     }));

            return result.ToArray();
        }


        public override string AbsoluteSearchURL
        {
            get { return VirtualPathUtility.ToAbsolute(Constants.BaseVirtualPath + "/"); }
        }

        public override ImageOptions Logo
        {
            get { return new ImageOptions {ImageFileName = "blog_add.png", PartID = Constants.ModuleID}; }
        }

        public override string PlaceVirtualPath
        {
            get { return Constants.BaseVirtualPath; }
        }

        public override string SearchName
        {
            get { return BlogsResource.SearchDefaultString; }
        }

        public override Guid ProductID
        {
            get { return CommunityProduct.ID; }
        }

        public override Guid ModuleID
        {
            get { return Constants.ModuleID; }
        }

        public override IItemControl Control
        {
            get { return new ResultsView(); }
        }
    }
}