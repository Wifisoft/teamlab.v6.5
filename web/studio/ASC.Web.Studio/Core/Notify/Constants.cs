using ASC.Common.Notify;
using ASC.Notify.Model;
using ASC.Notify.Patterns;

namespace ASC.Web.Studio.Core.Notify
{
    internal sealed class Constants
    {

        public static ITag TagUserName = new Tag("UserName");
		public static ITag TagUserEmail = new Tag("UserEmail");
        public static ITag TagSubject = new Tag("Subject");
        public static ITag TagBody = new Tag("Body");
		public static ITag TagMyStaffLink = new Tag("MyStaffLink");
        public static ITag TagInviteLink = new Tag("InviteLink");
        public static ITag TagDate = new Tag("Date");
        public static ITag TagIP = new Tag("IP");
        public static ITag TagPassword = new Tag("Password");        
        public static ITag TagWebStudioLink = new Tag("WebStudioLink");
        public static ITag TagAuthor = new Tag("Author");
        public static ITag TagAuthorLink = new Tag("AuthorLink");
        public static ITag TagProfileChanges = new Tag("ProfileChanges");
        public static ITag TagIsPasswordChange = new Tag("IsPasswordChange");
        public static ITag TagActivities = new Tag("Activities"); 
        public static ITag TagBackupUrl = new Tag("BackupUrl");
        public static ITag TagBackupHours = new Tag("BackupHours");

        public static ITag TagDeactivateUrl = new Tag("DeactivateUrl");
        public static ITag TagActivateUrl = new Tag("ActivateUrl");
        public static ITag TagDeleteUrl = new Tag("DeleteUrl");
        public static ITag TagOwnerName = new Tag("OwnerName");


        public static INotifyAction ActionAdminNotify = new NotifyAction("admin_notify", "admin notifications");
        public static INotifyAction ActionSelfProfileUpdated = new NotifyAction("self_profile_updated", "self profile updated");
        public static INotifyAction ActionUserHasJoin = new NotifyAction("user_has_join", "user has join");
        public static INotifyAction ActionUserMessageToAdmin = new NotifyAction("for_admin_notify", "for_admin_notify");

        public static INotifyAction ActionResetPassword = new NotifyAction("reset_pwd", "reset password");
        public static INotifyAction ActionPasswordChanged = new NotifyAction("change_pwd", "password changed");

        public static INotifyAction ActionYouAdded = new NotifyAction("you_added", "You added");
        public static INotifyAction ActionYouAddedAfterInvite = new NotifyAction("you_added_after_invite", "You added after invite");
        public static INotifyAction ActionYourProfileUpdated = new NotifyAction("profile_updated", "profile updated");
        public static INotifyAction ActionSendPassword = new NotifyAction("send_pwd", "send password");
        public static INotifyAction ActionInviteUsers = new NotifyAction("invite", "invite users");
        public static INotifyAction ActionJoinUsers = new NotifyAction("join", "join users");
        public static INotifyAction ActionSendWhatsNew = new NotifyAction("send_whats_new", "send whats new");
        public static INotifyAction ActionBackupCreated = new NotifyAction("backup_created", "backup created");
        public static INotifyAction ActionPortalDeactivate = new NotifyAction("portal_deactivate", "portal deactivate");
        public static INotifyAction ActionPortalDelete = new NotifyAction("portal_delete", "portal delete");
        public static INotifyAction ActionProfileDelete = new NotifyAction("profile_delete", "profile_delete");
		public static INotifyAction ActionDnsChange = new NotifyAction("dns_change", "dns_change");

        public static INotifyAction ActionConfirmOwnerChange = new NotifyAction("owner_confirm_change", "owner_confirm_change");
        public static INotifyAction ActionActivateUsers = new NotifyAction("activate", "activate");
        public static INotifyAction ActionActivateEmail = new NotifyAction("activate_email", "activate_email");
        public static INotifyAction ActionEmailChange = new NotifyAction("change_email", "change_email");        
        public static INotifyAction ActionPasswordChange = new NotifyAction("change_password", "change_password");
        public static INotifyAction ActionPhoneChange = new NotifyAction("change_phone", "change_phone");

        public static INotifyAction ActionCongratulations = new NotifyAction("congratulations", "congratulations");
    }

    public sealed class CommonTags
    {

        public const string VirtualRootPath = "__VirtualRootPath";

        /// <summary>
        /// Current product id - __ProductID
        /// </summary>
        public const string ProductID = "__ProductID";

        /// <summary>
        /// Default product url - __ProductUrl
        /// </summary>
        public const string ProductUrl = "__ProductUrl";

        /// <summary>
        /// Curreent DateTime - __DateTime
        /// </summary>
        public const string DateTime = "__DateTime";

        /// <summary>
        /// Current user id - __AuthorID
        /// </summary>
        public const string AuthorID = "__AuthorID";
        
        /// <summary>
        /// Current user fullname - __AuthorName
        /// </summary>
        public const string AuthorName = "__AuthorName";


        public const string SendFrom = NotifyArgumentConstants.MessageFrom;
        /// <summary>
        /// Current user profile link - __AuthorUrl
        /// </summary>
        public const string AuthorUrl = "__AuthorUrl";


        /// <summary>
        /// Helper with methods __Helper
        /// </summary>
        public const string Helper = "__Helper";
        /// <summary>
        /// Recipient identifier - __RecipientID
        /// </summary>
        public const string RecipientID = "__RecipientID";
        
        /// <summary>
        /// Subscription settings url - RecipientSubscriptionConfigURL
        /// </summary>
        public const string RecipientSubscriptionConfigURL = "RecipientSubscriptionConfigURL";

    }
}
