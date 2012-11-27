using ASC.Common.Security;
using ASC.Common.Security.Authorizing;
using ASC.Core;
using ASC.Web.Core;

namespace ASC.Web.Community.Product
{
    public static class CommunitySecurity
    {
        private static bool IsAdministrator()
        {
            return CoreContext.UserManager.IsUserInGroup(SecurityContext.CurrentAccount.ID, ASC.Core.Users.Constants.GroupAdmin.ID) ||
                WebItemSecurity.IsProductAdministrator(CommunityProduct.ID, SecurityContext.CurrentAccount.ID);
        }


        public static bool CheckPermissions(params IAction[] actions)
        {
            return CheckPermissions(null, null, actions);
        }

        public static bool CheckPermissions(ISecurityObject securityObject, params IAction[] actions)
        {
            return CheckPermissions(securityObject, null, actions);
        }

        public static bool CheckPermissions(ISecurityObjectId objectId, ISecurityObjectProvider securityObjProvider, params IAction[] actions)
        {
            if (IsAdministrator()) return true;
            return SecurityContext.CheckPermissions(objectId, securityObjProvider, actions);
        }

        public static void DemandPermissions(params IAction[] actions)
        {
            DemandPermissions(null, null, actions);
        }

        public static void DemandPermissions(ISecurityObject securityObject, params IAction[] actions)
        {
            DemandPermissions(securityObject, null, actions);
        }

        public static void DemandPermissions(ISecurityObjectId objectId, ISecurityObjectProvider securityObjProvider, params IAction[] actions)
        {
            if (IsAdministrator()) return;
            SecurityContext.DemandPermissions(objectId, securityObjProvider, actions);
        }
    }
}
