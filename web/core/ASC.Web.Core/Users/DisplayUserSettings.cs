﻿using System;
using System.Runtime.Serialization;
using ASC.Core;
using ASC.Core.Users;
using ASC.Web.Core.Utility.Settings;

namespace ASC.Web.Core.Users
{
    [Serializable]
    [DataContract]
    public class DisplayUserSettings : ISettings
    {
        public Guid ID
        {
            get { return new Guid("2EF59652-E1A7-4814-BF71-FEB990149428"); }
        }

        [DataMember(Name = "IsDisableGettingStarted")]
        public bool IsDisableGettingStarted { get; set; }


        public ISettings GetDefault()
        {
            return new DisplayUserSettings
                       {
                           IsDisableGettingStarted = false,
                       };
        }

        public static string GetFullUserName(Guid userID)
        {
            return GetFullUserName(CoreContext.UserManager.GetUsers(userID));
        }

        public static string GetFullUserName(Guid userID, bool withHtmlEncode)
        {
            return GetFullUserName(CoreContext.UserManager.GetUsers(userID), withHtmlEncode);
        }

        public static string GetFullUserName(UserInfo userInfo)
        {
            return GetFullUserName(userInfo, DisplayUserNameFormat.Default, true);
        }

        public static string GetFullUserName(UserInfo userInfo, bool withHtmlEncode)
        {
            return GetFullUserName(userInfo, DisplayUserNameFormat.Default, withHtmlEncode);
        }

        public static string GetFullUserName(UserInfo userInfo, DisplayUserNameFormat format, bool withHtmlEncode)
        {
            if (userInfo == null || userInfo.ID.Equals(Common.Security.Authorizing.Constants.Demo.ID))
            {
                return "Demo";
            }
            if (!userInfo.ID.Equals(Guid.Empty) && !CoreContext.UserManager.UserExists(userInfo.ID))
            {
                return "profile removed";
            }
            var result = UserFormatter.GetUserName(userInfo, format);
            return withHtmlEncode ? result.HtmlEncode() : result;
        }
    }
}