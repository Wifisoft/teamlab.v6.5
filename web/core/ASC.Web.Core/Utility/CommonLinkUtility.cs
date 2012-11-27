using System;
using System.Net;
using System.Runtime.Remoting.Messaging;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Configuration;
using ASC.Core;
using ASC.Core.Users;
using ASC.Web.Core;
using log4net;

namespace ASC.Web.Studio.Utility
{
    public enum ManagementType
    {
        General = 0,
        Statistic = 5,
        Account = 6,
        Customization = 7,
        Mail = 2,
        Tariff = 4,
        AccessRights = 8
    }

    public enum UserProfileType
    {
        General,
        Activity,
        Statistic,
        Subscriptions
    }

    public enum MyStaffType
    {
        General,
        Activity,
        Subscriptions,
        Customization
    }

    public static class CommonLinkUtility
    {
        private const string FilesBaseVirtualPath = "~/products/files/";
        private static readonly string FilesBaseAbsolutePath = VirtualPathUtility.ToAbsolute(FilesBaseVirtualPath).ToLower();
        private static readonly Regex regFilePathTrim = new Regex("/[^/]*\\.aspx", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static readonly bool standalone;
        private static readonly Uri serverRoot;

        public const string ParamName_ProductSysName = "product";
        public const string ParamName_UserUserName = "user";
        public const string ParamName_UserUserID = "uid";

        public const string FileId = "fileID";
        public const string Version = "version";
        public const string Action = "action";

        public static readonly string FileHandlerPath = FilesBaseAbsolutePath + "filehandler.ashx";


        static CommonLinkUtility()
        {
            try
            {
                var uriBuilder = new UriBuilder(Uri.UriSchemeHttp, "localhost");
                if (HttpContext.Current != null)
                {
                    var u = HttpContext.Current.Request.GetUrlRewriter();
                    uriBuilder = new UriBuilder(u.Scheme, u.Host, u.Port);
                }

                standalone = CoreContext.Configuration.Standalone;
                if (standalone)
                {
                    var webhost = WebConfigurationManager.AppSettings["web.host"];
                    if (!string.IsNullOrEmpty(webhost))
                    {
                        uriBuilder = new UriBuilder(webhost);
                    }
                    else
                    {
                        if (uriBuilder.Host == "localhost")
                        {
                            try
                            {
                                uriBuilder.Host = Dns.GetHostName();
                            }
                            catch
                            {
                            }
                        }
                    }
                }

                serverRoot = uriBuilder.Uri;


                DocServiceApiUrl = WebConfigurationManager.AppSettings["files.docservice.url.api"] ?? "";

            }
            catch (Exception error)
            {
                LogManager.GetLogger("ASC.Web").Error(error);
            }
        }


        public static string VirtualRoot
        {
            get { return VirtualPathUtility.ToAbsolute("~"); }
        }

        public static string ServerRootPath
        {
            get
            {
                /*
                 * NOTE: fixed bug with warning on SSL certificate when coming from Email to teamlab. 
                 * Valid only for users that have custom domain set. For that users we should use a http scheme
                 * Like https://mydomain.com that maps to <alias>.teamlab.com
                */
                var basedomain = WebConfigurationManager.AppSettings["core.base-domain"];
                var useHttp = !string.IsNullOrEmpty(basedomain) && !CoreContext.TenantManager.GetCurrentTenant().TenantDomain.EndsWith("." + basedomain, StringComparison.OrdinalIgnoreCase);
                Uri result;
                if (HttpContext.Current != null)
                {
                    var u = HttpContext.Current.Request.GetUrlRewriter();
                    result = new UriBuilder(useHttp ? Uri.UriSchemeHttp : u.Scheme, u.Host, useHttp && u.IsDefaultPort ? 80 : u.Port).Uri;
                }
                else if (standalone)
                {
                    result = serverRoot;
                }
                else
                {
                    var u = serverRoot;
                    result = new UriBuilder(useHttp ? Uri.UriSchemeHttp : u.Scheme, CoreContext.TenantManager.GetCurrentTenant().TenantDomain, useHttp && u.IsDefaultPort ? 80 : u.Port).Uri;
                }
                return result.ToString().TrimEnd('/');
            }
        }

        public static string GetFullAbsolutePath(string virtualPath)
        {
            if (String.IsNullOrEmpty(virtualPath))
                return ServerRootPath;

            if (virtualPath.StartsWith("http://", StringComparison.InvariantCultureIgnoreCase) ||
                virtualPath.StartsWith("mailto:", StringComparison.InvariantCultureIgnoreCase) ||
                virtualPath.StartsWith("javascript:", StringComparison.InvariantCultureIgnoreCase) ||
                virtualPath.StartsWith("https://", StringComparison.InvariantCultureIgnoreCase))
                return virtualPath;


            if (virtualPath.StartsWith("~"))
                virtualPath = VirtualRoot + "/" + virtualPath.Substring(1);

            virtualPath = ("/" + virtualPath).Replace("//", "/").Replace("//", "/");

            var result = ServerRootPath + virtualPath;

            return result;
        }

        public static string Logout
        {
            get { return VirtualPathUtility.ToAbsolute("~/auth.aspx") + "?t=logout"; }
        }

        public static string GetDefault()
        {
            return VirtualPathUtility.ToAbsolute("~/");
        }


        public static string GetMyStaff(MyStaffType type)
        {
            return GetMyStaff((type).ToString().ToLower());
        }

        public static string GetMyStaff(string type)
        {
            return GetFullAbsolutePath("~/my.aspx") + ((type != MyStaffType.General.ToString().ToLower()) ? ("?type=" + (type)) : "");
        }

        public static string GetEmployees()
        {
            return GetEmployees(GetProductID());
        }

        public static string GetEmployees(Guid productID)
        {
            return GetEmployees(productID, EmployeeStatus.Active);
        }

        public static string GetEmployees(Guid productID, EmployeeStatus empStatus)
        {
            return VirtualPathUtility.ToAbsolute("~/employee.aspx") + "?" + GetProductParamsPair(productID) +
                   (empStatus == EmployeeStatus.Terminated ? "&es=0" : string.Empty);
        }

        public static string GetEmployees(Guid productID, EmployeeActivationStatus empActivationStatus)
        {
            return VirtualPathUtility.ToAbsolute("~/employee.aspx") + "?" + GetProductParamsPair(productID) +
                   (empActivationStatus == EmployeeActivationStatus.Pending ? "&eas=0" : string.Empty);
        }

        public static string GetUserDepartment(Guid userID)
        {
            return GetUserDepartment(userID, GetProductID());
        }

        public static string GetUserDepartment(Guid userID, Guid productID)
        {
            var groups = CoreContext.UserManager.GetUserGroups(userID);
            if (groups != null && groups.Length > 0)
                return VirtualPathUtility.ToAbsolute("~/employee.aspx") + "?" + GetProductParamsPair(productID) + "&deplist=" + groups[0].ID.ToString();

            return GetEmployees(productID);
        }

        public static string GetDepartment(Guid productID, Guid departmentID)
        {
            return VirtualPathUtility.ToAbsolute("~/employee.aspx") + "?" + GetProductParamsPair(productID) + "&deplist=" + departmentID.ToString();
        }

        #region user profile link

        public static string GetUserProfile(string user)
        {
            return GetUserProfile(user, GetProductID());
        }

        public static string GetUserProfile(string user, Guid productID)
        {
            return GetUserProfile(user, productID, UserProfileType.General);
        }

        public static string GetUserProfile(Guid productID)
        {
            return GetUserProfile(null, productID, UserProfileType.General);
        }

        public static string GetUserProfile(Guid userID, Guid productID)
        {
            return GetUserProfile(userID, productID, UserProfileType.General);
        }

        public static string GetUserProfile(Guid userID, Guid productID, UserProfileType userProfileType)
        {
            if (!CoreContext.UserManager.UserExists(userID))
                return GetEmployees(productID);

            return GetUserProfile(userID.ToString(), productID, userProfileType);
        }

        public static string GetUserProfile(string user, Guid productID, UserProfileType userProfileType)
        {
            return GetUserProfile(user, productID, userProfileType, true);
        }

        public static string GetUserProfile(string user, Guid productID, UserProfileType userProfileType, bool absolute)
        {
            var queryParams = "";

            if (!String.IsNullOrEmpty(user))
            {
                var guid = Guid.Empty;
                if (!String.IsNullOrEmpty(user) && 32 <= user.Length && user[8] == '-')
                {
                    try
                    {
                        guid = new Guid(user);
                    }
                    catch
                    {
                    }
                }

                queryParams = guid != Guid.Empty ? GetUserParamsPair(guid) : ParamName_UserUserName + "=" + HttpUtility.UrlEncode(user);
            }

            if (productID != Guid.Empty)
            {
                queryParams += (String.IsNullOrEmpty(queryParams) ? "?" : "&") + GetProductParamsPair(productID);
            }
            var url = VirtualPathUtility.ToAbsolute("~/userprofile.aspx") + "?" + queryParams;
            if (!absolute)
            {
                url = "userprofile.aspx" + "?" + queryParams;
            }
            switch (userProfileType)
            {
                case UserProfileType.General:
                    break;

                case UserProfileType.Activity:
                    url += "#activity";
                    break;

                case UserProfileType.Statistic:
                    url += "#statistic";
                    break;

                case UserProfileType.Subscriptions:
                    url += "#subscriptions";
                    break;
            }

            return url;
        }

        #endregion

        public static Guid GetProductID()
        {
            var productID = Guid.Empty;

            if (HttpContext.Current != null)
            {
                IProduct product;
                IModule module;
                GetLocationByRequest(out product, out module);
                if (product != null) productID = product.ID;
            }

            if (productID == Guid.Empty)
            {
                var pid = CallContext.GetData("asc.web.product_id");
                if (pid != null) productID = (Guid) pid;
            }

            return productID;
        }

        public static void GetLocationByRequest(out IProduct currentProduct, out IModule currentModule)
        {
            var currentURL = string.Empty;
            if (HttpContext.Current != null && HttpContext.Current.Request != null)
            {
                currentURL = HttpContext.Current.Request.GetUrlRewriter().AbsoluteUri;

                // http://[hostname]/[virtualpath]/[AjaxPro.Utility.HandlerPath]/[assembly],[classname].ashx
                if (currentURL.Contains("/" + AjaxPro.Utility.HandlerPath + "/") && HttpContext.Current.Request.UrlReferrer != null)
                {
                    currentURL = HttpContext.Current.Request.UrlReferrer.AbsoluteUri;
                }
            }

            GetLocationByUrl(currentURL, out currentProduct, out currentModule);
        }

        public static IWebItem GetWebItemByUrl(string currentURL)
        {
            if (!String.IsNullOrEmpty(currentURL))
            {

                var itemName = GetWebItemNameFromUrl(currentURL);
                if (!string.IsNullOrEmpty(itemName))
                {
                    foreach (var item in WebItemManager.Instance.GetItemsAll())
                    {
                        var _itemName = GetWebItemNameFromUrl(item.StartURL);
                        if (String.Compare(itemName, _itemName, StringComparison.InvariantCultureIgnoreCase) == 0)
                            return item;
                    }
                }
                else
                {
                    var urlParams = HttpUtility.ParseQueryString(new Uri(currentURL).Query);
                    var productByName = GetProductBySysName(urlParams[ParamName_ProductSysName]);
                    var pid = productByName == null ? Guid.Empty : productByName.ID;

                    if (pid == Guid.Empty && !String.IsNullOrEmpty(urlParams["pid"]))
                    {
                        try
                        {
                            pid = new Guid(urlParams["pid"]);
                        }
                        catch
                        {
                            pid = Guid.Empty;
                        }
                    }

                    if (pid != Guid.Empty)
                        return ProductManager.Instance[pid];
                }
            }

            return null;
        }

        public static void GetLocationByUrl(string currentURL, out IProduct currentProduct, out IModule currentModule)
        {
            currentProduct = null;
            currentModule = null;

            if (!String.IsNullOrEmpty(currentURL))
            {
                var urlParams = HttpUtility.ParseQueryString(new Uri(currentURL).Query);
                var productByName = GetProductBySysName(urlParams[ParamName_ProductSysName]);
                var pid = productByName == null ? Guid.Empty : productByName.ID;

                if (pid == Guid.Empty && !String.IsNullOrEmpty(urlParams["pid"]))
                {
                    try
                    {
                        pid = new Guid(urlParams["pid"]);
                    }
                    catch
                    {
                        pid = Guid.Empty;
                    }
                }

                var productName = GetProductNameFromUrl(currentURL);
                var moduleName = GetModuleNameFromUrl(currentURL);

                if (!string.IsNullOrEmpty(productName) || !string.IsNullOrEmpty(moduleName))
                {
                    foreach (var product in ProductManager.Instance.Products)
                    {
                        var _productName = GetProductNameFromUrl(product.StartURL);
                        if (!string.IsNullOrEmpty(_productName))
                        {
                            if (String.Compare(productName, _productName, StringComparison.InvariantCultureIgnoreCase) == 0)
                            {
                                currentProduct = product;

                                if (!String.IsNullOrEmpty(moduleName))
                                {
                                    foreach (var module in product.Modules)
                                    {
                                        var _moduleName = GetModuleNameFromUrl(module.StartURL);
                                        if (!string.IsNullOrEmpty(_moduleName))
                                        {
                                            if (String.Compare(moduleName, _moduleName, StringComparison.InvariantCultureIgnoreCase) == 0)
                                            {
                                                currentModule = module;
                                                break;
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    foreach (var module in product.Modules)
                                    {
                                        if (!module.StartURL.Equals(product.StartURL) && currentURL.Contains(regFilePathTrim.Replace(module.StartURL, string.Empty)))
                                        {
                                            currentModule = module;
                                            break;
                                        }
                                    }
                                }

                                break;
                            }
                        }
                    }
                }

                if (pid != Guid.Empty)
                    currentProduct = ProductManager.Instance[pid];
            }
        }

        private static string GetWebItemNameFromUrl(string url)
        {
            var name = GetModuleNameFromUrl(url);
            if (String.IsNullOrEmpty(name))
            {
                name = GetProductNameFromUrl(url);
                if (String.IsNullOrEmpty(name))
                {
                    try
                    {
                        var pos = url.IndexOf("/addons/", StringComparison.InvariantCultureIgnoreCase);
                        if (0 <= pos)
                        {
                            url = url.Substring(pos + 8).ToLower();
                            pos = url.IndexOf('/');
                            return 0 < pos ? url.Substring(0, pos) : url;
                        }
                    }
                    catch
                    {
                    }
                    return null;
                }

            }

            return name;
        }

        private static string GetProductNameFromUrl(string url)
        {
            try
            {
                var pos = url.IndexOf("/products/", StringComparison.InvariantCultureIgnoreCase);
                if (0 <= pos)
                {
                    url = url.Substring(pos + 10).ToLower();
                    pos = url.IndexOf('/');
                    return 0 < pos ? url.Substring(0, pos) : url;
                }
            }
            catch
            {
            }
            return null;
        }

        private static string GetModuleNameFromUrl(string url)
        {
            try
            {
                var pos = url.IndexOf("/modules/", StringComparison.InvariantCultureIgnoreCase);
                if (0 <= pos)
                {
                    url = url.Substring(pos + 9).ToLower();
                    pos = url.IndexOf('/');
                    return 0 < pos ? url.Substring(0, pos) : url;
                }
            }
            catch
            {
            }
            return null;
        }

        public static string GetProductParamsPair(Guid productId)
        {
            var result = "";
            if (productId != Guid.Empty)
            {
                var currentProduct = ProductManager.Instance[productId];
                if (currentProduct != null)
                    result = String.Format("{0}={1}",
                                           ParamName_ProductSysName,
                                           WebItemExtension.GetSysName(currentProduct as IWebItem));
            }

            return result;
        }

        private static IProduct GetProductBySysName(string sysName)
        {
            IProduct result = null;

            if (!String.IsNullOrEmpty(sysName))
                foreach (var product in ProductManager.Instance.Products)
                {
                    if (String.CompareOrdinal(sysName, WebItemExtension.GetSysName(product as IWebItem)) == 0)
                    {
                        result = product;
                        break;
                    }
                }

            return result;
        }

        public static string GetUserParamsPair(Guid userID)
        {
            if (CoreContext.UserManager.UserExists(userID))
                return GetUserParamsPair(CoreContext.UserManager.GetUsers(userID));
            else
                return "";
        }

        public static string GetUserParamsPair(UserInfo user)
        {
            if (user == null || string.IsNullOrEmpty(user.UserName))
                return "";
            else
                return String.Format("{0}={1}", ParamName_UserUserName, HttpUtility.UrlEncode(user.UserName.ToLowerInvariant()));
        }


        #region management links

        public static string GetAdministration(ManagementType managementType)
        {
            var query = "";
            if (managementType == ManagementType.General)
                return VirtualPathUtility.ToAbsolute("~/management.aspx") + query;

            return VirtualPathUtility.ToAbsolute("~/management.aspx") + query + (String.IsNullOrEmpty(query) ? "?" : "&") + "type=" + ((int) managementType).ToString();
        }

        #endregion


        #region files

        public static string DocServiceApiUrl { get; private set; }

        public static string FileViewUrlString
        {
            get { return FileHandlerPath + "?" + Action + "=view&" + FileId + "={0}"; }
        }

        public static string GetFileViewUrl(object fileId)
        {
            return GetFileViewUrl(fileId, 0);
        }

        public static string GetFileViewUrl(object fileId, int fileVersion)
        {
            return string.Format(FileViewUrlString, HttpUtility.UrlEncode(fileId.ToString()))
                   + (fileVersion == 0 ? string.Empty : "&" + Version + "=" + fileVersion);
        }


        public static string FileDownloadUrlString
        {
            get { return FileHandlerPath + "?" + Action + "=download&" + FileId + "={0}"; }
        }

        public static string GetFileDownloadUrl(object fileId)
        {
            return GetFileDownloadUrl(fileId, 0);
        }

        public static string GetFileDownloadUrl(object fileId, int fileVersion)
        {
            return string.Format(FileDownloadUrlString, HttpUtility.UrlEncode(fileId.ToString()))
                   + (fileVersion == 0 ? string.Empty : "&" + Version + "=" + fileVersion);
        }


        public static string FileWebViewerUrlString
        {
            get { return FilesBaseAbsolutePath + "docviewer.aspx?" + FileId + "={0}"; }
        }

        public static string GetFileWebViewerUrl(object fileId)
        {
            return GetFileWebViewerUrl(fileId, 0);
        }

        public static string GetFileWebViewerUrl(object fileId, int fileVersion)
        {
            return GetFileWebViewerUrl(fileId, 0, false);
        }

        public static string GetFileWebViewerUrl(object fileId, int fileVersion, bool forMobile)
        {
            var viewerUrl =
                forMobile
                    ? VirtualPathUtility.ToAbsolute("~/../products/files/") + "docviewer.aspx?" + FileId + "={0}"
                    : FileWebViewerUrlString;

            return string.Format(viewerUrl, HttpUtility.UrlEncode(fileId.ToString()))
                   + (fileVersion == 0 ? string.Empty : "&" + Version + "=" + fileVersion);
        }

        public static string FileWebEditorUrlString
        {
            get { return FilesBaseAbsolutePath + "doceditor.aspx?" + FileId + "={0}"; }
        }

        public static string GetFileWebEditorUrl(object fileId)
        {
            return string.Format(FileWebEditorUrlString, HttpUtility.UrlEncode(fileId.ToString()));
        }

        #endregion
    }
}