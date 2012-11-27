using System;
using System.Collections.Generic;
using System.Security;
using System.Security.Principal;
using System.Threading;
using System.Web;
using ASC.Common.Security;
using ASC.Common.Security.Authentication;
using ASC.Common.Security.Authorizing;
using ASC.Core.Security.Authentication;
using ASC.Core.Security.Authorizing;
using ASC.Security.Cryptography;
using AuthConst = ASC.Core.Configuration.Constants;
using UserConst = ASC.Core.Users.Constants;

namespace ASC.Core
{
    public static class SecurityContext
    {
        private const string AUTH_PRINCIPAL = "__Auth.Principal";
        private const string AUTH_COOKIE = "__Auth.Cookie";


        public static IAccount CurrentAccount
        {
            get { return Principal.Identity is IAccount ? (IAccount)Principal.Identity : AuthConst.Guest; }
        }

        public static bool IsAuthenticated
        {
            get { return CurrentAccount.IsAuthenticated; }
        }

        public static bool DemoMode
        {
            get { return IsAuthenticated && CurrentAccount.ID == Constants.Demo.ID; }
        }

        public static IPermissionResolver PermissionResolver
        {
            get;
            private set;
        }


        static SecurityContext()
        {
            var permissionProvider = new PermissionProvider();
            var azManager = new AzManager(new RoleProvider(), permissionProvider);
            PermissionResolver = new PermissionResolver(azManager, permissionProvider);
        }


        public static string AuthenticateMe(string login, string password)
        {
            if (login == null) throw new ArgumentNullException("login");
            if (password == null) throw new ArgumentNullException("password");

            var tenantid = CoreContext.TenantManager.GetCurrentTenant().TenantId;
            var u = CoreContext.UserManager.GetUsers(tenantid, login, Hasher.Base64Hash(password, HashAlg.SHA256));

            AuthenticateMe(new UserAccount(u, tenantid));

            return CookieStorage.EncryptCookie(tenantid, u.ID, null, null);
        }

        public static bool AuthenticateMe(string cookie)
        {
            if (cookie == null) throw new ArgumentNullException("cookie");

            int tenant;
            Guid userid;
            string login;
            string password;
            if (CookieStorage.DecryptCookie(cookie, out tenant, out userid, out login, out password))
            {
                if (tenant != CoreContext.TenantManager.GetCurrentTenant().TenantId)
                {
                    return false;
                }

                try
                {
                    if (userid != Guid.Empty)
                    {
                        AuthenticateMe(new UserAccount(new ASC.Core.Users.UserInfo { ID = userid }, tenant));
                    }
                    else
                    {
                        AuthenticateMe(login, password);
                    }
                    return true;
                }
                catch { }
            }
            return false;
        }

        public static void AuthenticateMe(IAccount account)
        {
            if (account == null) throw new ArgumentNullException("account");

            var roles = new List<string>() { Role.Everyone };


            if (account is ISystemAccount && account.ID == AuthConst.CoreSystem.ID)
            {
                roles.Add(Role.System);
            }

            if (account is IUserAccount)
            {
                var u = CoreContext.UserManager.GetUsers(account.ID);

                if (u.ID == UserConst.LostUser.ID)
                {
                    throw new SecurityException("Invalid username or password.");
                }
                if (u.Status != ASC.Core.Users.EmployeeStatus.Active)
                {
                    throw new SecurityException("Account disabled.");
                }
                if (CoreContext.UserManager.IsUserInGroup(u.ID, UserConst.GroupAdmin.ID))
                {
                    roles.Add(Role.Administrators);
                }
                roles.Add(CoreContext.UserManager.IsUserInGroup(u.ID, UserConst.GroupVisitor.ID) ? Role.Visitors : Role.Users);

                account = new UserAccount(u, CoreContext.TenantManager.GetCurrentTenant().TenantId);
            }

            Principal = new GenericPrincipal(account, roles.ToArray());
        }

        public static void Logout()
        {
            Principal = null;
        }

        public static string SetUserPassword(Guid userID, string password)
        {
            CoreContext.Authentication.SetUserPassword(userID, password);
            if (CurrentAccount.ID == userID)
            {
                return CookieStorage.EncryptCookie(CoreContext.TenantManager.GetCurrentTenant().TenantId, userID, null, null);
            }
            return null;
        }


        public static bool CheckPermissions(params IAction[] actions)
        {
            return PermissionResolver.Check(CurrentAccount, actions);
        }

        public static bool CheckPermissions(ISecurityObject securityObject, params IAction[] actions)
        {
            return CheckPermissions(securityObject, null, actions);
        }

        public static bool CheckPermissions(ISecurityObjectId objectId, ISecurityObjectProvider securityObjProvider, params IAction[] actions)
        {
            return PermissionResolver.Check(CurrentAccount, objectId, securityObjProvider, actions);
        }

        public static void DemandPermissions(params IAction[] actions)
        {
            PermissionResolver.Demand(CurrentAccount, actions);
        }

        public static void DemandPermissions(ISecurityObject securityObject, params IAction[] actions)
        {
            DemandPermissions(securityObject, null, actions);
        }

        public static void DemandPermissions(ISecurityObjectId objectId, ISecurityObjectProvider securityObjProvider, params IAction[] actions)
        {
            PermissionResolver.Demand(CurrentAccount, objectId, securityObjProvider, actions);
        }


        private static IPrincipal Principal
        {
            get
            {
                var principal = GetFromHttpSession(AUTH_PRINCIPAL) as IPrincipal;
                if (principal != null)
                {
                    Thread.CurrentPrincipal = principal;
                    if (HttpContext.Current != null) HttpContext.Current.User = principal;
                }
                return Thread.CurrentPrincipal;
            }
            set
            {
                SetToHttpSession(AUTH_PRINCIPAL, value);
                Thread.CurrentPrincipal = value;
                if (HttpContext.Current != null) HttpContext.Current.User = value;
            }
        }

        private static object GetFromHttpSession(string name)
        {
            return HttpContext.Current != null && HttpContext.Current.Session != null
                ? HttpContext.Current.Session[name]
                : null;
        }

        private static void SetToHttpSession(string name, object obj)
        {
            if (HttpContext.Current != null && HttpContext.Current.Session != null)
            {
                HttpContext.Current.Session[name] = obj;
            }
        }
    }
}