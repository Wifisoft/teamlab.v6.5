using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ASC.Blogs.Core.Publisher;
using ASC.Blogs.Core.Resources;
using ASC.Blogs.Core.Security;
using ASC.Core;
using ASC.Web.Community.Blogs.Common;
using ASC.Web.Community.Product;
using ASC.Web.Core;
using ASC.Web.Core.ModuleManagement;
using ASC.Web.Core.Users.Activity;

[assembly: Module(typeof(BlogsModule))]

namespace ASC.Web.Community.Blogs.Common
{
    public class BlogsModule : Module
    {
        public override Guid ID
        {
            get { return new Guid("6A598C74-91AE-437d-A5F4-AD339BD11BB2"); }
        }

        public override string Name
        {
            get { return BlogsResource.AddonName; }
        }

        public override string Description
        {
            get { return BlogsResource.AddonDescriptionResourceKey; }
        }

        public override string StartURL
        {
            get { return "~/products/community/modules/blogs/"; }
        }

        public override IEnumerable<Widget> Widgets
        {
            get
            {
                var w = new Widget
                {
                    ID = new Guid("15FD28E1-FB2D-4410-9BF0-6369093A6C4F"),
                    Name = BlogsResource.AddonName,
                    Description = BlogsResource.WidgetDescriptionResourceKey,
                    StartURL = "~/products/community/modules/blogs/",
                    SettingsType = typeof(BlogsWidgetSettingsProvider),
                    WidgetType = typeof(BlogsWidget),
                    Context = new WebItemContext { IconFileName = "BlogIconWG.png" }
                };
                return new[] { w };
            }
        }

        public override IEnumerable<IWebItem> Actions
        {
            get
            {
                if (CanEdit())
                {
                    var a1 = new NavigationWebItem
                    {
                        ID = new Guid("98DB8D88-EDF2-4f82-B3AF-B95E87E3EE5C"),
                        Name = BlogsResource.NewPost,
                        StartURL = VirtualPathUtility.ToAbsolute("~/products/community/modules/blogs/addblog.aspx"),
                    };
                    return new[] { a1, };
                }
                return Enumerable.Empty<IWebItem>();
            }
        }

        public override IEnumerable<IWebItem> Navigations
        {
            get
            {
                var navigations = new List<IWebItem>();
                var n1 = new NavigationWebItem
                {
                    ID = new Guid("7B933FF3-2FF1-4d93-9F79-1830446BECF8"),
                    Name = BlogsResource.ListBlogsPageName,
                    StartURL = VirtualPathUtility.ToAbsolute("~/products/community/modules/blogs/"),
                };
                navigations.Add(n1);
                if (CanEdit())
                {
                    var n2 = new NavigationWebItem
                    {
                        ID = new Guid("82022032-E16C-4955-B9E9-C505E89285F7"),
                        Name = BlogsResource.AllBlogsMenuTitle,
                        StartURL = VirtualPathUtility.ToAbsolute("~/products/community/modules/blogs/allblogs.aspx"),
                    };
                    navigations.Add(n2);
                }
                return navigations;
            }
        }


        public BlogsModule()
        {
            Context = new ModuleContext
            {
                DefaultSortOrder = 1,
                ThemesFolderVirtualPath = "~/products/community/modules/blogs/app_themes",
                ImageFolder = "blogsimages",
                SmallIconFileName = "blog_add.png",
                IconFileName = "blogiconwg.png",
                SubscriptionManager = new BlogsSubscriptionManager(),
                StatisticProvider = new BlogsStatisticProvider(),
                UserActivityPublishers = new List<IUserActivityPublisher>() { new BlogUserActivityPublisher() },
                GetCreateContentPageAbsoluteUrl = () => CanEdit() ? VirtualPathUtility.ToAbsolute(ASC.Blogs.Core.Constants.BaseVirtualPath + "addblog.aspx") : null,
                SearchHandler = new BlogsSearchHandler(),
            };
        }

        private static bool CanEdit()
        {
            return CommunitySecurity.CheckPermissions(new PersonalBlogSecObject(CoreContext.UserManager.GetUsers(SecurityContext.CurrentAccount.ID)), ASC.Blogs.Core.Constants.Action_AddPost);
        }
    }
}
