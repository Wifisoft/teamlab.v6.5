using System;
using System.Text;
using ASC.Web.Core.Utility.Skins;
using ASC.Web.Studio.Utility;

namespace ASC.Core.Users
{
    public static class StudioUserInfoExtension
    {
        public static bool IsOwner(this UserInfo userInfo)
        {
            if (userInfo == null)
                return false;

            var tenant = CoreContext.TenantManager.GetCurrentTenant();
            return tenant != null && tenant.OwnerId.Equals(userInfo.ID);
        }

        public static bool IsAdmin(this UserInfo userInfo)
        {
            return userInfo != null && CoreContext.UserManager.IsUserInGroup(userInfo.ID, Constants.GroupAdmin.ID);
        }

        public static bool IsOnline(this UserInfo userInfo)
        {
            if (userInfo == null)
                return false;

            return (UserOnlineManager.Instance.OnlineUsers.Find(uvi => uvi.UserInfo.ID.Equals(userInfo.ID)) != null);
        }

        public static string GetCurrentURL(this UserInfo userInfo)
        {
            if (userInfo == null)
                return "";

            var userVisitInfo = UserOnlineManager.Instance.OnlineUsers.Find(uvi => uvi.UserInfo.ID.Equals(userInfo.ID));
            return userVisitInfo != null ? userVisitInfo.CurrentURL : "";
        }

        public static string GetUserProfilePageURL(this UserInfo userInfo)
        {
            return CommonLinkUtility.GetUserProfile(userInfo.ID, CommonLinkUtility.GetProductID());
        }

        public static string GetUserProfilePageURL(this UserInfo userInfo, Guid productID)
        {
            return userInfo == null ? "" : CommonLinkUtility.GetUserProfile(userInfo.ID, productID);
        }

        public static string GetUserProfilePageURL(this UserInfo userInfo, Guid productID, UserProfileType profileType)
        {
            return userInfo == null ? "" : CommonLinkUtility.GetUserProfile(userInfo.ID, productID, profileType);
        }

        public static string GetCurrentModuleName(this UserInfo userInfo)
        {
            if (userInfo == null)
                return "";

            var userVisitInfo = UserOnlineManager.Instance.OnlineUsers.Find(uvi => uvi.UserInfo.ID.Equals(userInfo.ID));
            if (userVisitInfo != null)
            {
                var module = UserOnlineManager.Instance.GetCurrentModule(userVisitInfo.CurrentURL);
                if (module != null)
                    return module.Name;
            }

            return "";
        }

        public static Guid GetCurrentModuleID(this UserInfo userInfo)
        {
            if (userInfo == null)
                return Guid.Empty;

            var userVisitInfo = UserOnlineManager.Instance.OnlineUsers.Find(uvi => uvi.UserInfo.ID.Equals(userInfo.ID));
            if (userVisitInfo != null)
            {
                var module = UserOnlineManager.Instance.GetCurrentModule(userVisitInfo.CurrentURL);
                if (module != null)
                    return module.ID;
            }

            return Guid.Empty;
        }

        public static string RenderUserStatus(this UserInfo userInfo)
        {
            var sb = new StringBuilder();
            sb.AppendFormat("<img align=\"absmiddle\" alt=\"\" style=\"margin-right:3px;\" src=\"{0}\"/>", WebImageSupplier.GetAbsoluteWebPath(userInfo.IsOnline() ? "status_online.png" : "status_offline.png"));
            sb.AppendFormat("<span class=\"userlink\" onclick=\"ASC.Controls.JabberClient.open('{1}')\">{0}</span>", userInfo.IsOnline() ? Resources.Resource.Online : Resources.Resource.Offline, userInfo.UserName);

            return sb.ToString();
        }

        public static string RenderUserCommunication(this UserInfo userInfo)
        {
            return RenderUserStatus(userInfo);
        }

        public static string RenderProfileLink(this UserInfo userInfo, Guid productID)
        {
            var sb = new StringBuilder();

            if (userInfo == null || !CoreContext.UserManager.UserExists(userInfo.ID))
            {
                sb.Append("<span class='userLink textMediumDescribe'>");
                sb.Append(Resources.Resource.ProfileRemoved);
                sb.Append("</span>");
            }
            else if (Array.Exists(Configuration.Constants.SystemAccounts, a => a.ID == userInfo.ID))
            {
                sb.Append("<span class='userLink textMediumDescribe'>");
                sb.Append(userInfo.LastName);
                sb.Append("</span>");
            }
            else
            {
                var popupID = Guid.NewGuid();
                sb.AppendFormat("<span class=\"userLink\" id=\"{0}\" data-uid=\"{1}\" data-pid=\"{2}\">", popupID, userInfo.ID, productID);
                sb.AppendFormat("<a class='linkDescribe' href=\"{0}\">{1}</a>", userInfo.GetUserProfilePageURL(productID), userInfo.DisplayUserName());
                sb.Append("</span>");
            }
            return sb.ToString();
        }

        public static string RenderPopupInfoScript(this UserInfo userInfo, Guid productID, string elementID)
        {
            if (userInfo == Constants.LostUser) return "";
            var sb = new StringBuilder();
            sb.Append("<script language='javascript'> StudioUserProfileInfo.RegistryElement('" + elementID + "','\"" + userInfo.ID + "\",\"" + productID + "\"'); </script>");
            return sb.ToString();
        }
    }
}