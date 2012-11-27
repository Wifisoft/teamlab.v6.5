using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Web;
using ASC.Web.Core.Users;

namespace ASC.Core.Users
{
    public static class UserInfoExtension
    {
        public static string DisplayUserName(this UserInfo userInfo)
        {
            return DisplayUserName(userInfo, true);
        }

        public static string DisplayUserName(this UserInfo userInfo, bool withHtmlEncode)
        {
            return DisplayUserSettings.GetFullUserName(userInfo, withHtmlEncode);
        }

        public static List<UserInfo> SortByUserName(this IEnumerable<UserInfo> userInfoCollection)
        {
            if (userInfoCollection == null) return new List<UserInfo>();

            var users = new List<UserInfo>(userInfoCollection);
            users.Sort(UserInfoComparer.Default);
            return users;
        }

        public static GroupInfo GetUserDepartment(this UserInfo userInfo)
        {
            if (userInfo == null) return null;

            return CoreContext.UserManager.GetUserGroups(userInfo.ID).FirstOrDefault();
        }


        public static Size GetPhotoSize(this UserInfo userInfo)
        {
            return UserPhotoManager.GetPhotoSize(Guid.Empty, userInfo.ID);
        }

        public static Size GetPhotoSize(this UserInfo userInfo, Guid moduleID)
        {
            return UserPhotoManager.GetPhotoSize(moduleID, userInfo.ID);
        }

        public static string GetPhotoURL(this UserInfo userInfo)
        {
            return UserPhotoManager.GetPhotoAbsoluteWebPath(Guid.Empty, userInfo.ID);
        }

        public static string GetPhotoURL(this UserInfo userInfo, Guid moduleID)
        {
            return UserPhotoManager.GetPhotoAbsoluteWebPath(moduleID, userInfo.ID);
        }

        public static string GetBigPhotoURL(this UserInfo userInfo)
        {
            return UserPhotoManager.GetBigPhotoURL(userInfo.ID);
        }

        public static string GetMediumPhotoURL(this UserInfo userInfo)
        {
            return UserPhotoManager.GetMediumPhotoURL(userInfo.ID);
        }

        public static string GetSmallPhotoURL(this UserInfo userInfo)
        {
            return UserPhotoManager.GetSmallPhotoURL(userInfo.ID);
        }


        public static string RenderProfileLinkBase(this UserInfo userInfo, Guid productID)
        {
            var sb = new StringBuilder();

            //check for removed users
            if (userInfo == null || !CoreContext.UserManager.UserExists(userInfo.ID))
            {
                sb.Append("<span class='userLink textMediumDescribe' style='white-space:nowrap;'>profile removed</span>");
            }
            else
            {
                var popupID = Guid.NewGuid();
                sb.Append("<span class=\"userLink\" style='white-space:nowrap;' id='" + popupID + "'>");
                sb.Append("<a class='linkDescribe' href=\"" + userInfo.GetUserProfilePageURLGeneral(productID) + "\">" + userInfo.DisplayUserName() + "</a>");
                sb.Append("</span>");

                sb.Append("<script language='javascript'> StudioUserProfileInfo.RegistryElement('" + popupID + "','\"" + userInfo.ID + "\",\"" + productID + "\"'); </script>");
            }
            return sb.ToString();
        }

        /// <summary>
        /// return absolute profile link
        /// </summary>
        /// <param name="userInfo"></param>        
        /// <returns></returns>
        private static string GetUserProfilePageURLGeneral(this UserInfo userInfo, Guid productID)
        {
            string query = "";
            string userID = string.Empty;

            if (userInfo != null)
                userID = userInfo.ID.ToString();

            if (productID != Guid.Empty)
                query = "?pid=" + productID.ToString();

            query += (String.IsNullOrEmpty(userID) ? "" : (String.IsNullOrEmpty(query) ? "?" : "&") + "uid=" + userID);

            var url = VirtualPathUtility.ToAbsolute("~/userprofile.aspx") + query;
            return url;
        }
    }
}
