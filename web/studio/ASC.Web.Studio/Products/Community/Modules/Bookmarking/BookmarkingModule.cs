using System;
using System.Collections.Generic;
using System.Web;
using ASC.Bookmarking.Business.Permissions;
using ASC.Bookmarking.Business.Statistics;
using ASC.Bookmarking.Business.Subscriptions;
using ASC.Bookmarking.Business.UserActivityPublisher;
using ASC.Web.Community.Bookmarking;
using ASC.Web.Community.Bookmarking.Resources;
using ASC.Web.Community.Bookmarking.Util;
using ASC.Web.Community.Bookmarking.Widget;
using ASC.Web.Core;
using ASC.Web.Core.ModuleManagement;
using ASC.Web.Core.Users.Activity;
using ASC.Web.UserControls.Bookmarking.Common.Presentation;
using ASC.Web.UserControls.Bookmarking.Common.Search;

[assembly: Module(typeof(BookmarkingModule))]

namespace ASC.Web.Community.Bookmarking
{
    public class BookmarkingModule : Module
    {
        public override Guid ID
        {
            get { return BookmarkingConst.BookmarkingId; }
        }

        public override string Name
        {
            get { return BookmarkingResource.AddonName; }
        }

        public override string Description
        {
            get { return BookmarkingResource.AddonDescriptionResourceKey; }
        }

        public override string StartURL
        {
            get { return "~/products/community/modules/bookmarking/"; }
        }

        public override IEnumerable<ASC.Web.Core.ModuleManagement.Widget> Widgets
        {
            get
            {
                var w1 = new ASC.Web.Core.ModuleManagement.Widget
                {
                    ID = new Guid("E6B0ED54-7CF4-4385-9638-73879A11B620"),
                    Name = BookmarkingResource.BookmarkingWidgetTitle,
                    Description = BookmarkingResource.BookmarkingWidgetDescription,
                    StartURL = "~/products/community/modules/bookmarking/",
                    SettingsType = typeof(BookmarkingWidgetSettingsProvider),
                    WidgetType = typeof(BookmarkingWidget),
                    Context = new WebItemContext { IconFileName = "bookmarking_icon.png" },
                };
                return new[] { w1 };
            }
        }

        public BookmarkingModule()
        {
            Context = new ModuleContext
            {
                DefaultSortOrder = 4,
                ThemesFolderVirtualPath = "~/products/community/modules/bookmarking/app_themes",
                ImageFolder = "images",
                SmallIconFileName = "bookmarking_mini_icon.png",
                IconFileName = "bookmarking_icon.png",
                SubscriptionManager = new BookmarkingSubscriptionManager(),
                SearchHandler = new BookmarkingSearchHandler(),
                StatisticProvider = new BookmarkingStatisticProvider(),
                UserActivityPublishers = new List<IUserActivityPublisher> { new BookmarkingUserActivityPublisher() },
                GetCreateContentPageAbsoluteUrl = () => BookmarkingPermissionsCheck.PermissionCheckCreateBookmark() ? VirtualPathUtility.ToAbsolute("~/products/community/modules/bookmarking/" + BookmarkingServiceHelper.GetCreateBookmarkPageUrl()) : null,
            };
        }
    }
}
