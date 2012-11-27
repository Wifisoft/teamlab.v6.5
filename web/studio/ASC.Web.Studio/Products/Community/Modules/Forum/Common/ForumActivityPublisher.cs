using System;
using System.Web;
using ASC.Core;
using ASC.Forum;
using ASC.Web.Community.Product;
using ASC.Web.Core.Users.Activity;
using ASC.Web.Studio.Utility;
using ASC.Web.UserControls.Forum.Common;

namespace ASC.Web.Community.Forum
{
    public class ForumActivityPublisher : ForumUserActivityPublisher
    {
        private class ForumUserActivity : UserActivity
        {
            public ForumUserActivity()
            {
                this.Date = ASC.Core.Tenants.TenantUtil.DateTimeNow();
                this.ModuleID = ForumManager.ModuleID;
                this.ProductID = CommunityProduct.ID;
                this.UserID = SecurityContext.CurrentAccount.ID;
                this.TenantID = TenantProvider.CurrentTenantID;
            }
        }

        internal static void NewThread(Thread thread)
        {
            UserActivityPublisher.Publish<ForumActivityPublisher>(new ForumUserActivity()
            {
                URL = VirtualPathUtility.ToAbsolute(ForumManager.BaseVirtualPath + "/topics.aspx") + "?f=" + thread.ID.ToString(),
                ActionType = UserActivityConstants.ContentActionType,
                ContentID = thread.ID.ToString(),
                BusinessValue = UserActivityConstants.ImportantContent,                
                ActionText = Resources.ForumResource.ThreadActionText,
                Title = thread.Title,
            });
        }

        internal static void EditThread(Thread thread)
        {
            UserActivityPublisher.Publish<ForumActivityPublisher>(new ForumUserActivity()
            {
                URL = VirtualPathUtility.ToAbsolute(ForumManager.BaseVirtualPath + "/topics.aspx") + "?f=" + thread.ID.ToString(),
                ActionType = UserActivityConstants.ActivityActionType,
                ContentID = thread.ID.ToString(),
                BusinessValue = UserActivityConstants.NormalActivity,
                ActionText = Resources.ForumResource.ThreadEditActionText,
                Title = thread.Title
            });
        }

        internal static void DeleteThread(Thread thread)
        {
            UserActivityPublisher.Publish<ForumActivityPublisher>(new ForumUserActivity()
            {
                URL = VirtualPathUtility.ToAbsolute(ForumManager.BaseVirtualPath + "/default.aspx") + "?f=" + thread.ID.ToString(),
                ActionType = UserActivityConstants.ActivityActionType,
                ContentID = thread.ID.ToString(),
                BusinessValue = UserActivityConstants.ImportantActivity,
                ActionText = Resources.ForumResource.ThreadDeleteActionText,
                Title = thread.Title
            });
        }
    }
}
