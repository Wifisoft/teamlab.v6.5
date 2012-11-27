using System;
using System.Web.Configuration;
using ASC.Forum;
using ASC.Web.Community.Product;
using ASC.Web.UserControls.Forum.Common;

namespace ASC.Web.Community.Forum
{
    public static class ForumManager
    {
        public static Guid ModuleID
        {
            get { return new Guid("853B6EB9-73EE-438d-9B09-8FFEEDF36234"); }
        }

        public static string BaseVirtualPath
        {
            get { return "~/products/community/modules/forum"; }
        }


        public static UserControls.Forum.Common.ForumManager Instance
        {
            get { return Settings.ForumManager; }
        }

        public static Settings Settings { get; private set; }

        static ForumManager()
        {
            Settings = new Settings(new ForumPresenterSettings())
                           {
                               ProductID = CommunityProduct.ID,
                               ModuleID = new Guid("853B6EB9-73EE-438d-9B09-8FFEEDF36234"),
                               ImageItemID = new Guid("853B6EB9-73EE-438d-9B09-8FFEEDF36234"),
                               UserControlsVirtualPath = "~/products/community/modules/forum/forumusercontrols",
                               StartPageVirtualPath = "~/products/community/modules/forum/default.aspx",
                               TopicPageVirtualPath = "~/products/community/modules/forum/topics.aspx",
                               PostPageVirtualPath = "~/products/community/modules/forum/posts.aspx",
                               SearchPageVirtualPath = "~/products/community/modules/forum/search.aspx",
                               NewPostPageVirtualPath = "~/products/community/modules/forum/newpost.aspx",
                               EditTopicPageVirtualPath = "~/products/community/modules/forum/edittopic.aspx",
                               FileStoreModuleID = "forum",
                               ConfigPath = "~/products/community/modules/forum/web.config"
                           };

            ForumSettings.Configure("community");
        }
    }
}