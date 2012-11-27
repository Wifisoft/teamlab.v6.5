using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using ASC.Bookmarking.Business.Permissions;
using ASC.Bookmarking.Common;
using ASC.Bookmarking.Pojo;
using ASC.Core;
using ASC.Web.Controls;
using ASC.Web.Core.Utility.Skins;
using ASC.Web.Studio.Controls.Common;
using ASC.Web.Studio.Core;
using ASC.Web.UserControls.Bookmarking.Common;
using ASC.Web.UserControls.Bookmarking.Common.Presentation;
using BookmarkingResource = ASC.Web.UserControls.Bookmarking.Resources.BookmarkingUCResource;

namespace ASC.Web.Community.Bookmarking.Util
{
    public class BookmarkingNavigationUtil
    {

        public static string GetCreateContentPageUrl()
        {
            if (BookmarkingPermissionsCheck.PermissionCheckCreateBookmark())
                return VirtualPathUtility.ToAbsolute("~/products/community/modules/bookmarking/" + BookmarkingServiceHelper.GetCreateBookmarkPageUrl());

            return null;
        }

        public static void SetBookmarkingActionsAndNavigation(ControlCollection c)
        {
            SetBookmarkingActions(c);
            SetBookmarkingNavigation(c);
        }

        public static void SetBookmarkingActions(ControlCollection c)
        {
            var actions = GetBookmarkingActions();
            if (0 < actions.Controls.Count)
            {
                c.Add(actions);
            }
        }

        public static void SetBookmarkInfoActionsAndNavigation(ControlCollection c)
        {
            var actions = GetBookmarkInfoActions();
            if (0 < actions.Controls.Count)
            {
                c.Add(actions);
            }
            SetBookmarkingNavigation(c);
        }

        public static void SetBookmarkingNavigation(ControlCollection c)
        {
            c.Add(GetBookmarkingNavigation());
        }

        public static void InitBookmarkingPage(ControlCollection c)
        {
            var provider = new BookmarkingScriptProvider();

            BookmarkingSettings.ModuleId = BookmarkingConst.BookmarkingId;

            c.Add(provider);
        }

        public static SideNavigator GetBookmarkingNavigation()
        {
            SideNavigator navigator = new SideNavigator();

            navigator.Controls.Add(new NavigationItem()
            {
                Name = BookmarkingResource.BookmarksNavigationItem,
                URL = BookmarkingPathConstants.BookmarkingPageUrl
            });

            navigator.Controls.Add(new NavigationItem()
            {
                Name = BookmarkingResource.TagsNavigationItem,
                URL = BookmarkingPathConstants.TagsPageUrl
            });

            navigator.Controls.Add(new NavigationItem()
            {
                Name = BookmarkingResource.FavouritesNavigationItem,
                URL = BookmarkingPathConstants.FavouriteBookmarksPageUrl
            });

            return navigator;
        }

        private static SideActions GetBookmarkingActions()
        {
            var actions = new SideActions();

            if (BookmarkingPermissionsCheck.PermissionCheckCreateBookmark())
            {
                actions.Controls.Add(new NavigationItem(BookmarkingResource.AddBookmarkLink, BookmarkingServiceHelper.GetCreateBookmarkPageUrl()));
            }

            if (SecurityContext.IsAuthenticated)
            {
                var serviceHelper = BookmarkingServiceHelper.GetCurrentInstanse();
                var isSubscribed = serviceHelper.IsSubscribed(serviceHelper.SubscriptionRecentBookmarkID, BookmarkingBusinessConstants.NotifyActionNewBookmark);
                actions.Controls.Add(new BookmarkingNavigationItem()
                {
                    Name = BookmarkingResource.SubscribeOnRecentBookmarks,
                    URL = "javascript:subscribeOnRecentBookmarks()",
                    BookmarkingClientID = "subscribeOnRecentBookmarks",
                    DisplayOnPage = !isSubscribed,
                    IsPromo = (SetupInfo.WorkMode == WorkMode.Promo)
                });

                actions.Controls.Add(new BookmarkingNavigationItem()
                {
                    Name = BookmarkingResource.UnSubscribeOnRecentBookmarks,
                    URL = "javascript:unSubscribeOnRecentBookmarks()",
                    BookmarkingClientID = "unSubscribeOnRecentBookmarks",
                    DisplayOnPage = isSubscribed,
                    IsPromo = (SetupInfo.WorkMode == WorkMode.Promo)
                });
            }

            return actions;
        }

        private static SideActions GetBookmarkInfoActions()
        {
            var actions = new SideActions();

            if (BookmarkingPermissionsCheck.PermissionCheckCreateBookmark())
            {
                actions.Controls.Add(new NavigationItem(BookmarkingResource.AddBookmarkLink, BookmarkingServiceHelper.GetCreateBookmarkPageUrl()));
            }
            if (SecurityContext.IsAuthenticated)
            {
                var serviceHelper = BookmarkingServiceHelper.GetCurrentInstanse();
                var commentsID = serviceHelper.SubscriptionBookmarkCommentsID;
                var isSubscribed = false;
                if (!commentsID.Equals("0"))
                {
                    isSubscribed = serviceHelper.IsSubscribed(commentsID, BookmarkingBusinessConstants.NotifyActionNewComment);
                    actions.Controls.Add(new BookmarkingNavigationItem()
                    {
                        Name = BookmarkingResource.SubscribeOnBookmarkComments,
                        URL = "javascript:subscribeOnBookmarkComments()",
                        BookmarkingClientID = "subscribeOnBookmarkComments",
                        DisplayOnPage = !isSubscribed,
                        IsPromo = (SetupInfo.WorkMode == WorkMode.Promo)
                    });
                    actions.Controls.Add(new BookmarkingNavigationItem()
                    {
                        Name = BookmarkingResource.UnSubscribeOnBookmarkComments,
                        URL = "javascript:unSubscribeOnBookmarkComments()",
                        BookmarkingClientID = "unSubscribeOnBookmarkComments",
                        DisplayOnPage = isSubscribed,
                        IsPromo = (SetupInfo.WorkMode == WorkMode.Promo)
                    });
                }
            }
            return actions;
        }


        private static TagCloud GetTagsCloud(IList<Tag> tagsCloud)
        {
            TagCloud tagCloud = new TagCloud();

            var ServiceHelper = BookmarkingServiceHelper.GetCurrentInstanse();

            var tags = tagsCloud;
            foreach (var tag in tags)
            {
                TagCloudItem item = new TagCloudItem();
                item.TagName = tag.Name;
                item.Rank = tag.Populatiry;
                item.URL = ServiceHelper.GetSearchByTagUrl(tag);

                tagCloud.Items.Add(item);
            }

            return tagCloud;
        }

        public static void SetTagsCloud(ControlCollection c, string label, TagCloud tagCloud)
        {
            if (tagCloud.Items == null || tagCloud.Items.Count == 0)
            {
                return;
            }
            var ServiceHelper = BookmarkingServiceHelper.GetCurrentInstanse();

            var tagsCloudContainer = new SideContainer()
            {
                Title = label,
                ImageURL = WebImageSupplier.GetAbsoluteWebPath("tag_32.png", BookmarkingConst.BookmarkingId)
            };

            tagsCloudContainer.Controls.Add(tagCloud);
            c.Add(tagsCloudContainer);
        }

        public static void SetPopularTagsCloud(ControlCollection c)
        {
            var ServiceHelper = BookmarkingServiceHelper.GetCurrentInstanse();
            try
            {
                SetTagsCloud(c, BookmarkingResource.PopularTags, GetTagsCloud(ServiceHelper.GetTagsCloud()));
            }
            catch { };
        }

        public static void SetRelatedTagsCloud(ControlCollection c)
        {
            try
            {
                var ServiceHelper = BookmarkingServiceHelper.GetCurrentInstanse();
                var tags = ServiceHelper.GetRelatedTagsCloud(ServiceHelper.BookmarkToAdd.ID);
                //var tags = ServiceHelper.GetTagsCloud();
                SetTagsCloud(c, BookmarkingResource.RelatedTags, GetTagsCloud(tags));
            }
            catch
            {
            }
        }

        public static void SetFavouriteTagsCloud(ControlCollection c)
        {
            var ServiceHelper = BookmarkingServiceHelper.GetCurrentInstanse();
            try
            {
                var tags = ServiceHelper.GetFavouriteTagsCloud();
                SetTagsCloud(c, BookmarkingResource.FavouriteTags, GetTagsCloud(tags));
            }
            catch { };
        }

    }
}
