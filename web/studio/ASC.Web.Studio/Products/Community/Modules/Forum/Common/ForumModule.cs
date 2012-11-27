using System;
using System.Collections.Generic;
using System.Web;
using ASC.Web.Community.Forum.Common;
using ASC.Web.Community.Forum.Resources;
using ASC.Web.Core;
using ASC.Web.Core.ModuleManagement;
using ASC.Web.Core.Users.Activity;
using ASC.Web.UserControls.Forum.Common;

[assembly: Module(typeof(ForumModule))]

namespace ASC.Web.Community.Forum.Common
{
    public class ForumModule : Module
    {
        public override Guid ID
        {
            get { return ForumManager.ModuleID; }
        }

        public override string Name
        {
            get { return ForumResource.AddonName; }
        }

        public override string Description
        {
            get { return ForumResource.AddonDescription; }
        }

        public override string StartURL
        {
            get { return "~/products/community/modules/forum/"; }
        }

        public override IEnumerable<Widget> Widgets
        {
            get
            {
                var w = new Widget
                {
                    ID = new Guid("4F5C69D0-7F12-48fd-AA62-5DDAC62B2F70"),
                    Name = ForumResource.RecentForumUpdateWidgetTitle,
                    Description = ForumResource.RecentForumUpdateWidgetDescription,
                    StartURL = "~/products/community/modules/forum/default.aspx",
                    SettingsType = typeof(LastUpdateSettingsProvider),
                    WidgetType = typeof(ForumLastUpdatesWidget),
                    Context = new WebItemContext { IconFileName = "forum_icon.png" }
                };
                return new[] { w };
            }
        }

        public override IEnumerable<IWebItem> Actions
        {
            get
            {
                var actions = new List<IWebItem>();

                var a = new NavigationWebItem
                {
                    ID = new Guid("A04A7DBF-6B73-4579-BECE-3F6E346133DB"),
                    Name = ForumResource.ForumEditorBreadCrumbs,
                    StartURL = VirtualPathUtility.ToAbsolute("~/products/community/modules/forum/managementcenter.aspx"),
                };
                if (ForumShortcutProvider.CheckPermissions(a.ID))
                {
                    actions.Add(a);
                }

                a = new NavigationWebItem
                {
                    ID = new Guid("24CD48B2-C40F-43ec-B3A6-3212C51B8D34"),
                    Name = ForumResource.NewTopicButton,
                    Context = new WebItemContext { IconFileName = "topic.png" },
                };
                if (ForumShortcutProvider.CheckPermissions(a.ID))
                {
                    a.StartURL = ForumShortcutProvider.GetAbsoluteWebPathForShortcut(a.ID);
                    actions.Add(a);
                }

                a = new NavigationWebItem
                {
                    ID = new Guid("84DF7BE7-315B-4ba3-9BE1-1E348F6697A5"),
                    Name = ForumResource.NewPollButton,
                };
                if (ForumShortcutProvider.CheckPermissions(a.ID))
                {
                    a.StartURL = ForumShortcutProvider.GetAbsoluteWebPathForShortcut(a.ID);
                    actions.Add(a);
                }

                if (ForumManager.Instance.CurrentPage.Page == ForumPage.PostList || ForumManager.Instance.CurrentPage.Page == ForumPage.NewPost || ForumManager.Instance.CurrentPage.Page == ForumPage.EditTopic)
                {
                    a = new NavigationWebItem
                    {
                        ID = new Guid("FA5C4BD5-25E7-41c8-A0DC-64DC2A977391"),
                        Name = ForumResource.NewPostButton,
                    };
                    if (ForumShortcutProvider.CheckPermissions(a.ID))
                    {
                        a.StartURL = ForumShortcutProvider.GetAbsoluteWebPathForShortcut(a.ID);
                        actions.Add(a);
                    }
                }

                return actions;
            }
        }

        public override IEnumerable<IWebItem> Navigations
        {
            get
            {
                /*
                var navigations = new List<IWebItem>();

                var n1 = new NavigationWebItem
                {
                    ID = new Guid("17AB51E9-4A8A-49d4-BC03-3235887E12BB"),
                    Name = ForumResource.ForumListPageDescription,
                    StartURL = VirtualPathUtility.ToAbsolute("~/products/community/modules/forum/default.aspx"),
                };
                navigations.Add(n1);

                var n2 = new NavigationWebItem
                {
                    ID = new Guid("87A6B7FC-E872-49db-A327-CEA9CBA59CCC"),
                    Name = ForumResource.TagEditorBreadCrumbs,
                    StartURL = VirtualPathUtility.ToAbsolute("~/products/community/modules/forum/managementcenter.aspx") + "?type=1",
                };
                if (ForumShortcutProvider.CheckPermissions(n2.ID))
                {
                    navigations.Add(n2);
                }

                return navigations;*/
                return base.Navigations;
            }
        }


        public ForumModule()
        {
            var activityPublisher = new ForumActivityPublisher();
            activityPublisher.InitSettings(ForumManager.Settings);

            Context = new ModuleContext
            {
                DefaultSortOrder = 2,
                ThemesFolderVirtualPath = "~/products/community/modules/forum/app_themes",
                ImageFolder = "forumimages",
                SmallIconFileName = "forum_mini_icon.png",
                IconFileName = "forum_icon.png",
                SubscriptionManager = new ForumSubscriptionManager(),
                GlobalHandler = new ForumGlobalHandler(),
                StatisticProvider = new ForumStatisticProvider(),
                UserActivityPublishers = new List<IUserActivityPublisher>() { activityPublisher },
                GetCreateContentPageAbsoluteUrl = ForumShortcutProvider.GetCreateContentPageUrl,
                SearchHandler = new ForumSearchHandler(),
            };
        }
    }
}
