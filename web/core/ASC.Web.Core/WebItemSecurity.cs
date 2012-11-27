using System;
using System.Collections.Generic;
using System.Linq;
using ASC.Common.Security;
using ASC.Common.Security.Authorizing;
using ASC.Core;
using ASC.Core.Users;
using SecurityAction = ASC.Common.Security.Authorizing.Action;

namespace ASC.Web.Core
{
    public static class WebItemSecurity
    {
        private static readonly SecurityAction Read = new SecurityAction(new Guid("77777777-32ae-425f-99b5-83176061d1ae"), "ReadWebItem", false, true);


        public static bool IsAvailableForUser(string id, Guid @for)
        {
            // can read or administrator
            var securityObj = WebItemSecurityObject.Create(id);

            var everyone = CoreContext.AuthorizationManager.GetAces(ASC.Core.Users.Constants.GroupEveryone.ID, Read.ID, securityObj).FirstOrDefault();
            if (everyone != null && everyone.Reaction == AceType.Deny)
            {
                return false;
            }

            return SecurityContext.PermissionResolver.Check(CoreContext.Authentication.GetAccountByID(@for), securityObj, null, Read) ||
                IsProductAdministrator(securityObj.WebItemId, @for);
        }


        public static bool IsLicensed(IWebItem item)
        {
            if (item != null)
            {
                try
                {
                    var sysName = item.GetSysName();
                    return Licensing.License.Current.HasFeature(sysName) &&
                           Licensing.License.Current.GetFeature<bool>(sysName);

                }
                catch (Exception)
                {
                    
                }
            }
            return false;
        }

        public static IEnumerable<Tuple<Guid, bool>> GetSecurity(string id)
        {
            var securityObj = WebItemSecurityObject.Create(id);
            return CoreContext.AuthorizationManager
                .GetAces(Guid.Empty, Read.ID, securityObj)
                .Select(a => Tuple.Create(a.SubjectId, a.Reaction == AceType.Allow))
                .ToArray();
        }


        public static void SetSecurity(string id, bool enabled, params Guid[] subjects)
        {
            var securityObj = WebItemSecurityObject.Create(id);

            // remove old aces
            CoreContext.AuthorizationManager.RemoveAllAces(securityObj);

            // set new aces
            if (subjects == null || subjects.Length == 0 || subjects.Contains(ASC.Core.Users.Constants.GroupEveryone.ID))
            {
                subjects = new[] { ASC.Core.Users.Constants.GroupEveryone.ID };
            }
            foreach (var s in subjects)
            {
                var a = new AzRecord(s, Read.ID, enabled ? AceType.Allow : AceType.Deny, securityObj);
                CoreContext.AuthorizationManager.AddAce(a);
            }
        }

        public static WebItemSecurityInfo GetSecurityInfo(string id)
        {
            var info = GetSecurity(id).ToList();
            return new WebItemSecurityInfo
            {
                WebItemId = id,

                Enabled = (info.Any(i => i.Item2) || !info.Any()) && IsLicensed(id),

                Users = info
                    .Select(i => CoreContext.UserManager.GetUsers(i.Item1))
                    .Where(u => u.ID != ASC.Core.Users.Constants.LostUser.ID),

                Groups = info
                    .Select(i => CoreContext.GroupManager.GetGroupInfo(i.Item1))
                    .Where(g => g.ID != ASC.Core.Users.Constants.LostGroupInfo.ID && g.CategoryID != ASC.Core.Users.Constants.SysGroupCategoryId)
            };
        }

        private static bool IsLicensed(string id)
        {
            //Try get item
            try
            {
                return IsLicensed(WebItemManager.Instance[new Guid(id)]);
            }
            catch (Exception)
            {
                
            }
            return true;//TODO:
        }


        public static void SetProductAdministrator(Guid productid, Guid userid, bool administrator)
        {
            if (productid == Guid.Empty)
            {
                productid = ASC.Core.Users.Constants.GroupAdmin.ID;
            }
            if (administrator)
            {
                CoreContext.UserManager.AddUserIntoGroup(userid, productid);
            }
            else
            {
                if (productid == ASC.Core.Users.Constants.GroupAdmin.ID)
                {
                    foreach (var id in WebItemManager.Instance.GetItemsAll().OfType<IProduct>().Select(p => p.ID))
                    {
                        CoreContext.UserManager.RemoveUserFromGroup(userid, id);
                    }
                }
                CoreContext.UserManager.RemoveUserFromGroup(userid, productid);
            }
        }

        public static bool IsProductAdministrator(Guid productid, Guid userid)
        {
            if (CoreContext.UserManager.IsUserInGroup(userid, ASC.Core.Users.Constants.GroupAdmin.ID))
            {
                return true;
            }
            else
            {
                return CoreContext.UserManager.IsUserInGroup(userid, productid);
            }
        }

        public static IEnumerable<UserInfo> GetProductAdministrators(Guid productid)
        {
            var groups = new List<Guid>();
            if (productid == Guid.Empty)
            {
                groups.Add(ASC.Core.Users.Constants.GroupAdmin.ID);
                groups.AddRange(WebItemManager.Instance.GetItemsAll().OfType<IProduct>().Select(p => p.ID));
            }
            else
            {
                groups.Add(productid);
            }

            var users = Enumerable.Empty<UserInfo>();
            foreach (var id in groups)
            {
                users = users.Union(CoreContext.UserManager.GetUsersByGroup(id));
            }
            return users.ToList();
        }


        private class WebItemSecurityObject : ISecurityObject
        {
            public Guid WebItemId
            {
                get;
                private set;
            }


            public Type ObjectType
            {
                get { return GetType(); }
            }

            public object SecurityId
            {
                get { return WebItemId.ToString("N"); }
            }

            public bool InheritSupported
            {
                get { return true; }
            }

            public bool ObjectRolesSupported
            {
                get { return false; }
            }


            public static WebItemSecurityObject Create(string id)
            {
                if (string.IsNullOrEmpty(id))
                {
                    throw new ArgumentNullException("id");
                }

                Guid itemId = Guid.Empty;
                if (32 <= id.Length)
                {
                    itemId = new Guid(id);
                }
                else
                {
                    var w = WebItemManager.Instance
                        .GetItemsAll()
                        .FirstOrDefault(i => id.Equals(i.GetSysName(), StringComparison.InvariantCultureIgnoreCase));
                    if (w != null) itemId = w.ID;
                }
                return new WebItemSecurityObject(itemId);
            }


            private WebItemSecurityObject(Guid itemId)
            {
                WebItemId = itemId;
            }

            public ISecurityObjectId InheritFrom(ISecurityObjectId objectId)
            {
                var s = objectId as WebItemSecurityObject;
                if (s != null)
                {
                    var parent = WebItemSecurityObject.Create(WebItemManager.Instance.GetParentItemID(s.WebItemId).ToString("N")) as WebItemSecurityObject;
                    return parent != null && parent.WebItemId != s.WebItemId ? parent : null;
                }
                return null;
            }

            public IEnumerable<IRole> GetObjectRoles(ISubject account, ISecurityObjectId objectId, SecurityCallContext callContext)
            {
                throw new NotImplementedException();
            }
        }
    }
}
