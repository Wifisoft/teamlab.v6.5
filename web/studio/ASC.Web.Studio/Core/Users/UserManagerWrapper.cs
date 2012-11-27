using System;
using System.Globalization;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using ASC.Core;
using ASC.Core.Tenants;
using ASC.Core.Users;
using ASC.Web.Core;
using ASC.Web.Core.Utility.Settings;
using ASC.Web.Studio.Core.Notify;
using ASC.Web.Studio.Utility;

namespace ASC.Web.Studio.Core.Users
{
    /// <summary>
    /// Web studio user manager helper
    /// </summary>
    public sealed class UserManagerWrapper
    {
        public static Guid AdminID
        {
            get
            {
                return new Guid("00000000-0000-0000-0000-000000000ace");
            }
        }

        private static bool TestUniqueUserName(string uniqueName)
        {
            if (String.IsNullOrEmpty(uniqueName))
                return false;
            return CoreContext.UserManager.GetUserByUserName(uniqueName) == ASC.Core.Users.Constants.LostUser;
        }

        private static string MakeUniqueName(UserInfo userInfo)
        {
            if (string.IsNullOrEmpty(userInfo.Email))
                throw new ArgumentException(Resources.Resource.ErrorEmailEmpty, "userInfo");

            var uniqueName =  new MailAddress(userInfo.Email).User;
            var startUniqueName = uniqueName;
            var i = 0;
            while (!TestUniqueUserName(uniqueName))
            {
                uniqueName = string.Format("{0}{1}", startUniqueName, (++i).ToString(CultureInfo.InvariantCulture));
            }
            return uniqueName;
        }

        public static bool CheckUniqueEmail(Guid userID, string email)
        {
            var foundUser = CoreContext.UserManager.GetUserByEmail(email);
            return foundUser == ASC.Core.Users.Constants.LostUser || foundUser.ID==userID;
        }

        #region Add User
        public static UserInfo AddUser(UserInfo userInfo, string password)
        {
            return AddUser(userInfo, password, false);
        }

        public static UserInfo AddUser(UserInfo userInfo, string password, bool afterInvite)
        {
            return AddUser(userInfo, password, afterInvite, true);
        }

        public static UserInfo AddUser(UserInfo userInfo, string password, bool afterInvite, bool notify)
        {
            if (userInfo == null) throw new ArgumentNullException("userInfo");

            CheckPasswordPolicy(password);

            if (!CheckUniqueEmail(userInfo.ID, userInfo.Email))
                throw new Exception(CustomNamingPeople.Substitute<Resources.Resource>("ErrorEmailAlreadyExists"));

            userInfo.UserName = MakeUniqueName(userInfo);
            if (!userInfo.WorkFromDate.HasValue)
            {
                userInfo.WorkFromDate = TenantUtil.DateTimeNow();
            }

            if (!afterInvite)
                userInfo.ActivationStatus = EmployeeActivationStatus.Pending;
            else
                userInfo.ActivationStatus = EmployeeActivationStatus.Activated;

            var newUserInfo = CoreContext.UserManager.SaveUserInfo(userInfo);
            CoreContext.Authentication.SetUserPassword(newUserInfo.ID, password);

            if ((newUserInfo.Status & EmployeeStatus.Active) == EmployeeStatus.Active && notify)
            {
                //NOTE: Notify user only if it's active
                if (afterInvite)
                {
                    StudioNotifyService.Instance.UserInfoAddedAfterInvite(newUserInfo, password);
                }
                else
                {
                    //Send user invite
                    StudioNotifyService.Instance.UserInfoActivation(newUserInfo);
                }
            }

            return newUserInfo;
        }
        #endregion

        public static UserInfo SaveUserInfo(UserInfo userInfo, string password)
        {
            if (userInfo == null) throw new ArgumentNullException("userInfo");

            CheckPasswordPolicy(password);

            if (!CheckUniqueEmail(userInfo.ID, userInfo.Email))
                throw new Exception(CustomNamingPeople.Substitute<Resources.Resource>("ErrorEmailAlreadyExists"));

            var prevUserInfo = CoreContext.UserManager.GetUsers(userInfo.ID);
            var newUserInfo = CoreContext.UserManager.SaveUserInfo(userInfo);
            CoreContext.Authentication.SetUserPassword(newUserInfo.ID, password);

            StudioNotifyService.Instance.UserInfoUpdated(prevUserInfo, newUserInfo, password);

            UserOnlineManager.Instance.UpdateOnlineUserInfo(userInfo.ID);

            return newUserInfo;
        }

        public static UserInfo SaveUserInfo(UserInfo userInfo)
        {
            if (userInfo == null) throw new ArgumentNullException("userInfo");

            if (!CheckUniqueEmail(userInfo.ID, userInfo.Email))
                throw new Exception(CustomNamingPeople.Substitute<Resources.Resource>("ErrorEmailAlreadyExists"));

            var prevUserInfo = CoreContext.UserManager.GetUsers(userInfo.ID);
            var newUserInfo = CoreContext.UserManager.SaveUserInfo(userInfo);

            StudioNotifyService.Instance.UserInfoUpdated(prevUserInfo, newUserInfo);

            UserOnlineManager.Instance.UpdateOnlineUserInfo(userInfo.ID);

            return newUserInfo;
        }

        #region Password
        public static void SetUserPassword(Guid userID, string password)
        {
            CheckPasswordPolicy(password);

            var cookie = SecurityContext.SetUserPassword(userID, password);
            StudioNotifyService.Instance.UserPasswordChanged(userID, password);

            if (cookie != null)
                CookiesManager.SetCookies(CookiesType.AuthKey, cookie);
        }

        public static void CheckPasswordPolicy(string password)
        {
            if (String.IsNullOrEmpty(password))
                throw new Exception(Resources.Resource.ErrorPasswordEmpty);

            var passwordSettingsObj =
             SettingsManager.Instance.LoadSettings<StudioPasswordSettings>(TenantProvider.CurrentTenantID);

            if (!CheckPasswordRegex(passwordSettingsObj, password))
                throw new Exception(GenerateErrorMessage(passwordSettingsObj));
        }

        public static void SendUserPassword(string email)
        {
            if (String.IsNullOrEmpty(email)) throw new ArgumentNullException("email");

            var userInfo = CoreContext.UserManager.GetUserByEmail(email);
            if (!CoreContext.UserManager.UserExists(userInfo.ID) || string.IsNullOrEmpty(userInfo.Email))
            {
                throw new Exception(String.Format(Resources.Resource.ErrorUserNotFoundByEmail, email));
            }
            StudioNotifyService.Instance.UserPasswordChange(userInfo);
        }

        const string Noise = "1234567890mnbasdflkjqwerpoiqweyuvcxnzhdkqpsdk@%&;";

        public static string GeneratePassword()
        {
            var ps = SettingsManager.Instance.LoadSettings<StudioPasswordSettings>(TenantProvider.CurrentTenantID);

            return String.Format("{0}{1}{2}{3}",
                GeneratePassword(ps.MinLength, ps.MinLength, Noise.Substring(0, Noise.Length - 4)),
                ps.Digits ? GeneratePassword(1, 1, Noise.Substring(0, 10)) : String.Empty,
                ps.UpperCase ? GeneratePassword(1, 1, Noise.Substring(10, 20).ToUpper()) : String.Empty,
                ps.SpecSymbols ? GeneratePassword(1, 1, Noise.Substring(Noise.Length - 4, 4).ToUpper()) : String.Empty);
        }

        static int counter = 0;
        internal static string GeneratePassword(int minLength, int maxLength, string noise)
        {
            var rnd = new Random(Interlocked.Increment(ref counter));
            var length = minLength + rnd.Next(maxLength - maxLength);

            var pwd = string.Empty;
            while (length-- > 0)
            {
                pwd += noise.Substring(rnd.Next(noise.Length - 1), 1);
            }
            return pwd;
        }

        internal static string GenerateErrorMessage(StudioPasswordSettings passwordSettings)
        {
            var error = new StringBuilder();

            error.AppendFormat("{0} ", Resources.Resource.ErrorPasswordMessage);
            error.AppendFormat(Resources.Resource.ErrorPasswordShort, passwordSettings.MinLength);
            if (passwordSettings.UpperCase)
                error.AppendFormat(", {0}", Resources.Resource.ErrorPasswordNoUpperCase);
            if (passwordSettings.Digits)
                error.AppendFormat(", {0}", Resources.Resource.ErrorPasswordNoDigits);
            if (passwordSettings.SpecSymbols)
                error.AppendFormat(", {0}", Resources.Resource.ErrorPasswordNoSpecialSymbols);

            return error.ToString();
        }

        public static string GetPasswordHelpMessage()
        {
            var info = new StringBuilder();
            var passwordSettings = SettingsManager.Instance.LoadSettings<StudioPasswordSettings>(TenantProvider.CurrentTenantID);
            info.AppendFormat("{0} ", Resources.Resource.ErrorPasswordMessageStart);
            info.AppendFormat(Resources.Resource.ErrorPasswordShort, passwordSettings.MinLength);
            if (passwordSettings.UpperCase)
                info.AppendFormat(", {0}", Resources.Resource.ErrorPasswordNoUpperCase);
            if (passwordSettings.Digits)
                info.AppendFormat(", {0}", Resources.Resource.ErrorPasswordNoDigits);
            if (passwordSettings.SpecSymbols)
                info.AppendFormat(", {0}", Resources.Resource.ErrorPasswordNoSpecialSymbols);

            return info.ToString();
        }

        internal static bool CheckPasswordRegex(StudioPasswordSettings passwordSettings, string password)
        {
            var pwdBuilder = new StringBuilder(@"^(?=.*\p{Ll}{0,})");

            if (passwordSettings.Digits)
                pwdBuilder.Append(@"(?=.*\d)");

            if (passwordSettings.UpperCase)
                pwdBuilder.Append(@"(?=.*\p{Lu})");

            if (passwordSettings.SpecSymbols)
                pwdBuilder.Append(@"(?=.*[\W])");

            pwdBuilder.Append(@".{");
            pwdBuilder.Append(passwordSettings.MinLength);
            pwdBuilder.Append(@",}$");

            return new Regex(pwdBuilder.ToString()).IsMatch(password);
        }
        #endregion

        public static void TransferUser2Department(Guid userID, Guid depID)
        {
            var user = CoreContext.UserManager.GetUsers(userID);
            var department = CoreContext.GroupManager.GetGroupInfo(depID);
            if (!department.ID.Equals(ASC.Core.Users.Constants.LostGroupInfo.ID) &&
                !user.ID.Equals(ASC.Core.Users.Constants.LostUser.ID))
            {

                var groups = CoreContext.UserManager.GetUserGroups(userID);
                GroupInfo oldDepartment = null;
                if (groups.Length > 0)
                    oldDepartment = groups[0];

                if (oldDepartment != null && oldDepartment.ID.Equals(department.ID))
                    return;

                if (oldDepartment != null)
                {
                    CoreContext.UserManager.RemoveUserFromGroup(user.ID, oldDepartment.ID);
                }

                CoreContext.UserManager.AddUserIntoGroup(user.ID, department.ID);
                user.Department = department.Name;
                CoreContext.UserManager.SaveUserInfo(user);
            }
        }

        public static void RenameDepartment(Guid depID, string newName)
        {
            var department = Array.Find(CoreContext.UserManager.GetDepartments(), (d) => d.ID == depID);
            if (department != null)
            {
                department.Name = newName;
                CoreContext.GroupManager.SaveGroupInfo(department);
                var users = CoreContext.UserManager.GetUsersByGroup(department.ID);
                foreach (var user in users)
                {
                    user.Department = department.Name;
                    CoreContext.UserManager.SaveUserInfo(user);
                }
            }
        }

        public static bool ValidateEmail(string email)
        {
            return new Regex(@"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*").IsMatch(email);
        }
    }
}
