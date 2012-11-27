using System;
using System.Globalization;
using ASC.Core.Tenants;
using ASC.Web.UserControls.Wiki.Resources;
using ASC.Web.Core.Users.Activity;
using ASC.Web.Core.Utility.Skins;
using ASC.Web.Studio.Utility;
using ASC.Web.UserControls.Wiki.Data;

namespace ASC.Web.UserControls.Wiki
{
    public class WikiActivityPublisher : BaseUserActivityPublisher
    {
        internal static void PublishInternal(UserActivity activity)
        {
            UserActivityPublisher.Publish<WikiActivityPublisher>(activity);
        }

        internal static string GetContentID(object page)
        {
            var result = string.Empty;
            if (page is Page)
            {
                result = string.Format(CultureInfo.CurrentCulture, "wikiPage#{0}", (page as Page).PageName);
            }
            else if(page is File)
            {
                result = string.Format(CultureInfo.CurrentCulture, "wikiFile#{0}", (page as File).FileName);
            }

            return result;
        }

        internal static string GetTitle(object page)
        {
            var result = string.Empty;
            if (page is Page)
            {
                result = (page as Page).PageName;
                if(string.IsNullOrEmpty(result))
                {
                    result = WikiResource.MainWikiCaption;
                }
            }
            else if (page is File)
            {
                result = (page as File).FileName;
            }

            return result;
        }

        internal static string GetUrl(object page)
        {
            var result = string.Empty;
            if (page is Page)
            {
                result = ActionHelper.GetViewPagePath(WikiManager.ViewVirtualPath, (page as Page).PageName); 
            }
            else if (page is File)
            {
                result = ActionHelper.GetViewFilePath(WikiManager.ViewVirtualPath, (page as File).FileName);
            }

            return result;
        }

        internal static UserActivity ComposeActivityByPage(object page)
        {
            var ua = new UserActivity
            {
                TenantID = TenantProvider.CurrentTenantID,
                ContentID = GetContentID(page),
                Date = TenantUtil.DateTimeNow(),
                ModuleID = WikiManager.ModuleId,
                ProductID = Community.Product.CommunityProduct.ID,
                Title = GetTitle(page),
                URL = GetUrl(page)
            };
            return ua;
        }

        internal static UserActivity ApplyCustomeActivityParams(UserActivity ua, string actionText, Guid userID, int actionType, int businessValue)
        {
            ua.ImageOptions = new ImageOptions {PartID = WikiManager.ModuleId, ImageFileName = string.Empty};
            ua.ActionText = actionText;
            ua.UserID = userID;
            ua.ActionType = actionType;
            ua.BusinessValue = businessValue;
            return ua;
        }


        public static void AddPage(Page page)
        {
            var ua =
                        ApplyCustomeActivityParams(
                            ComposeActivityByPage(page),
                            WikiResource.wikiAction_PageAdded,
                            page.UserID,
                            UserActivityConstants.ContentActionType,
                            UserActivityConstants.NormalContent
                        );

                PublishInternal(ua);
        }

        public static bool EditPage(Page page)
        {
            if(page.Version == 1) //New Page Saved!!!
            {
                AddPage(page);
                return false;
            }
            var ua =
                        ApplyCustomeActivityParams(
                            ComposeActivityByPage(page),
                            WikiResource.wikiAction_PageEdited,
                            page.UserID,
                            UserActivityConstants.ActivityActionType,
                            UserActivityConstants.ImportantActivity
                        );

            PublishInternal(ua);
            return true;
        }

        public static void RevertPage(Page page)
        {
            var ua =
                        ApplyCustomeActivityParams(
                            ComposeActivityByPage(page),
                            WikiResource.wikiAction_VersionRevert,
                            page.UserID,
                            UserActivityConstants.ActivityActionType,
                            UserActivityConstants.SmallActivity
                        );

            PublishInternal(ua);
        }

        public static void AddFile(File file)
        {
            var ua =
                        ApplyCustomeActivityParams(
                            ComposeActivityByPage(file),
                            WikiResource.wikiAction_FileAdded,
                            file.UserID,
                            UserActivityConstants.ActivityActionType,
                            UserActivityConstants.ImportantActivity
                        );

            PublishInternal(ua);
        }

        public static void DeleteFile(File file)
        {
            var ua =
                        ApplyCustomeActivityParams(
                            ComposeActivityByPage(file),
                            WikiResource.wikiAction_FileDeleted,
                            file.UserID,
                            UserActivityConstants.ActivityActionType,
                            UserActivityConstants.SmallActivity
                        );

            PublishInternal(ua);
        }

        public static void AddPageComment(Page page, Comment newComment)
        {
            var ua =
                        ApplyCustomeActivityParams(
                            ComposeActivityByPage(page),
                            WikiResource.wikiAction_CommentAdded,
                            newComment.UserId,
                            UserActivityConstants.ActivityActionType,
                            UserActivityConstants.NormalActivity
                        );

            PublishInternal(ua);
        }

        public static void EditPageComment(Page page, Comment newComment)
        {
            var ua =
                        ApplyCustomeActivityParams(
                            ComposeActivityByPage(page),
                            WikiResource.wikiAction_CommentEdited,
                            newComment.UserId,
                            UserActivityConstants.ActivityActionType,
                            UserActivityConstants.SmallActivity
                        );

            PublishInternal(ua);
        }

        public static void DeletePageComment(Page page, Comment newComment)
        {
            var ua =
                        ApplyCustomeActivityParams(
                            ComposeActivityByPage(page),
                            WikiResource.wikiAction_CommentDeleted,
                            newComment.UserId,
                            UserActivityConstants.ActivityActionType,
                            UserActivityConstants.SmallActivity
                        );

            PublishInternal(ua);
        }
    }
}
