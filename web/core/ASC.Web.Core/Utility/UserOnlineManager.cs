using System;
using System.Collections.Generic;
using System.Web;
using ASC.Core;
using ASC.Core.Users;
using ASC.Web.Core;
using ASC.Web.Core.Users;
using ASC.Core.Tenants;


namespace ASC.Web.Studio.Utility
{
    public class UserOnlineManager
    {
        public class UserVisitInfo
        {
            public UserInfo UserInfo { get; set; }
            public DateTime LastVisit { get; set; }
            public string CurrentURL { get; set; }
            public int TenantID { get; set; }
        }

        private Object _thisLock = new Object();

        private static UserOnlineManager _manager;

        public static UserOnlineManager Instance
        {
            get
            {
                if (_manager == null)
                {
                    lock (typeof(UserOnlineManager))
                    {
                        if (_manager == null)
                        {
                            _manager = new UserOnlineManager();
                        }
                    }
                }
                return _manager;
            }
        }

        private List<UserVisitInfo> _userVisits;

        private UserOnlineManager()
        {
            _userVisits = new List<UserVisitInfo>();
        }

        public List<UserVisitInfo> OnlineUsers
        {
            get
            {
                var online = new List<UserVisitInfo>();
                lock (_thisLock)
                {
                    _userVisits.RemoveAll(uv => (TenantUtil.DateTimeNow() - uv.LastVisit).Minutes >= 5);
                    online = _userVisits.FindAll(u => u.TenantID == TenantProvider.CurrentTenantID);
                    online.Sort((x, y) => UserInfoComparer.Default.Compare(x.UserInfo, y.UserInfo));

                }
                return online;
            }
        }

        public void UpdateOnlineUserInfo(Guid userID)
        {
            if (OnlineUsers.Find(uvi => uvi.UserInfo.ID.Equals(userID) && uvi.TenantID == TenantProvider.CurrentTenantID) != null)
            {
                UnRegistryOnlineUser(userID);
                RegistryOnlineUser(userID);
            }
        }

        public void RegistryOnlineUser(Guid userID)
        {
            if (SecurityContext.DemoMode)
                return;

            lock (_thisLock)
            {
                string currentURL = "";
                if (HttpContext.Current != null && HttpContext.Current.Server != null && HttpContext.Current.Request != null)
                    currentURL = HttpContext.Current.Request.GetUrlRewriter().AbsoluteUri;


                UserVisitInfo userVisit = _userVisits.Find(uv => uv.UserInfo.ID.Equals(userID)&& uv.TenantID == TenantProvider.CurrentTenantID);

                if (userVisit == null)
                    _userVisits.Add(new UserVisitInfo()
                    {
                        UserInfo = CoreContext.UserManager.GetUsers(userID),
                        TenantID = TenantProvider.CurrentTenantID,
                        CurrentURL = currentURL,
                        LastVisit = TenantUtil.DateTimeNow()
                    });
                else
                {
                    userVisit.CurrentURL = currentURL;
                    userVisit.LastVisit = TenantUtil.DateTimeNow();
                }
            }
        }

        public void UnRegistryOnlineUser(Guid userID)
        {
            lock (_thisLock)
            {
                _userVisits.RemoveAll(uv => uv.UserInfo.ID.Equals(userID) && uv.TenantID == TenantProvider.CurrentTenantID);
            }
        }


        public bool IsMessagesPage()
        {
            string currentURL = "";
            if (HttpContext.Current != null)
            {
                currentURL = HttpContext.Current.Request.CurrentExecutionFilePath;
                if (currentURL.IndexOf(VirtualPathUtility.ToAbsolute("~/Messages.aspx"), StringComparison.InvariantCultureIgnoreCase) != -1)
                    return true;
            }

            return false;
        }

        public bool IsEmployeesPage()
        {
            string currentURL = "";
            if (HttpContext.Current != null)
            {
                currentURL = HttpContext.Current.Request.CurrentExecutionFilePath;
                if (currentURL.IndexOf(VirtualPathUtility.ToAbsolute("~/Employee.aspx"), StringComparison.InvariantCultureIgnoreCase) != -1)
                    return true;
            }

            return false;
        }

        public bool IsAdministrationPage()
        {
            string currentURL = "";
            if (HttpContext.Current != null)
            {
                currentURL = HttpContext.Current.Request.CurrentExecutionFilePath;
                if (currentURL.IndexOf(VirtualPathUtility.ToAbsolute("~/Management.aspx"), StringComparison.InvariantCultureIgnoreCase) != -1)
                    return true;
            }

            return false;
        }

        public bool IsSearchPage()
        {
            string currentURL = "";
            if (HttpContext.Current != null)
            {
                currentURL = HttpContext.Current.Request.CurrentExecutionFilePath;
                if (currentURL.IndexOf(VirtualPathUtility.ToAbsolute("~/Search.aspx"), StringComparison.InvariantCultureIgnoreCase) != -1)
                    return true;
            }

            return false;
        }


        public bool IsUserProfilePage()
        {
            string currentURL = "";
            if (HttpContext.Current != null)
            {
                currentURL = HttpContext.Current.Request.CurrentExecutionFilePath;
                if (currentURL.IndexOf(VirtualPathUtility.ToAbsolute("~/UserProfile.aspx"), StringComparison.InvariantCultureIgnoreCase) != -1)
                    return true;
            }

            return false;
        }

       
        public IModule GetCurrentModule(string currentURL)
        {
            IProduct product;
            IModule module;
            GetProductLocation(currentURL, out product, out module);

            return module;
        }

        public IModule GetCurrentModule()
        {
            IProduct product;
            IModule module;
            GetProductLocation(out product, out module);

            return module;
        }

        public IProduct GetCurrentProduct()
        {
            IProduct product;
            IModule module;
            GetProductLocation(out product, out module);

            return product;
        }

        public void GetProductLocation(out IProduct currentProduct, out IModule currentModule)
        {
            currentProduct = null;
            currentModule = null;
            if (HttpContext.Current != null)
                CommonLinkUtility.GetLocationByRequest(out currentProduct, out currentModule);
        }


        public void GetProductLocation(string currentURL, out IProduct currentProduct, out IModule currentModule)
        {
            CommonLinkUtility.GetLocationByUrl(currentURL, out currentProduct, out currentModule);
        }
    }
}
