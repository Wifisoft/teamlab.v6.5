using System;
using System.Collections.Generic;
using System.Linq;
using ASC.Core;
using ASC.Web.Core.Utility.Settings;
using ASC.Web.Core.Utility.Skins;

namespace ASC.Web.Core
{
    public static class WebItemExtension
    {
        public static string GetSysName(this IWebItem webitem)
        {
            if (string.IsNullOrEmpty(webitem.StartURL)) return string.Empty;

            var sysname = string.Empty;
            var parts = webitem.StartURL.ToLower().Split('/', '\\').ToList();

            var index = parts.FindIndex(s => "products".Equals(s));
            if (0 <= index && index < parts.Count - 1)
            {
                sysname = parts[index + 1];
                index = parts.FindIndex(s => "modules".Equals(s));
                if (0 <= index && index < parts.Count - 1)
                {
                    sysname += "-" + parts[index + 1];
                }
                else if (index == parts.Count - 1)
                {
                    sysname = parts[index].Split('.')[0];
                }
                return sysname;
            }

            index = parts.FindIndex(s => "addons".Equals(s));
            if (0 <= index && index < parts.Count - 1)
            {
                sysname = parts[index + 1];
            }

            return sysname;
        }

        public static string GetDisabledIconAbsoluteURL(this IWebItem item)
        {
            if (item == null || item.Context == null || String.IsNullOrEmpty(item.Context.DisabledIconFileName)) return string.Empty;

            return WebImageSupplier.GetAbsoluteWebPath(item.Context.DisabledIconFileName, item.ID);
        }

        public static string GetSmallIconAbsoluteURL(this IWebItem item)
        {
            if (item == null || item.Context == null || String.IsNullOrEmpty(item.Context.SmallIconFileName)) return string.Empty;

            return WebImageSupplier.GetAbsoluteWebPath(item.Context.SmallIconFileName, item.ID);
        }

        public static string GetIconAbsoluteURL(this IWebItem item)
        {
            if (item == null || item.Context == null || String.IsNullOrEmpty(item.Context.IconFileName)) return string.Empty;

            return WebImageSupplier.GetAbsoluteWebPath(item.Context.IconFileName, item.ID);
        }

        public static string GetLargeIconAbsoluteURL(this IWebItem item)
        {
            if (item == null || item.Context == null || String.IsNullOrEmpty(item.Context.LargeIconFileName)) return string.Empty;

            return WebImageSupplier.GetAbsoluteWebPath(item.Context.LargeIconFileName, item.ID);
        }

        public static List<string> GetUserOpportunities(this IWebItem item)
        {
            return item.Context.UserOpportunities != null ? item.Context.UserOpportunities() : new List<string>();
        }

        public static List<string> GetAdminOpportunities(this IWebItem item)
        {
            return item.Context.AdminOpportunities != null ? item.Context.AdminOpportunities() : new List<string>();
        }

        public static bool HasComplexHierarchyOfAccessRights(this IWebItem item)
        {
            return item.Context.HasComplexHierarchyOfAccessRights;
        }

        public static bool CanNotBeDisabled(this IWebItem item)
        {
            return item.Context.CanNotBeDisabled;
        }

        public static IHtmlInjectionProvider GetInjectionProvider(this IWebItem item)
        {
            if (item != null && item.Context != null && item.Context.HtmlInjectionProviderType != null)
            {
                var type = item.Context.HtmlInjectionProviderType;
                if (type.GetInterfaces().Any(interfaceType => interfaceType == typeof(IHtmlInjectionProvider)))
                {
                    return (IHtmlInjectionProvider)Activator.CreateInstance(type);
                }
            }
            return null;
        }


        public static bool IsDisabled(this IWebItem item)
        {
            return item != null &&
                (!WebItemSecurity.IsAvailableForUser(item.ID.ToString("N"), SecurityContext.CurrentAccount.ID) || !item.IsLicensed());
        }

        public static bool IsLicensed(this IWebItem item)
        {
            return WebItemSecurity.IsLicensed(item);
        }

        public static List<IWebItem> SortItems(this IEnumerable<IWebItem> items, WebItemSettings settings)
        {
            return items.OrderBy(i => WebItemManager.GetSortOrder(i, settings)).ToList();
        }

        public static bool IsSubItem(this IWebItem item)
        {
            return item is IModule && !(item is IProduct); // module and not product
        }
    }
}
