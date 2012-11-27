using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using ASC.Core;
using ASC.Web.Core.Utility;
using ASC.Web.Core.Utility.Settings;
using ASC.Web.Core.WebZones;

namespace ASC.Web.Core
{
    [Flags]
    public enum ItemAvailableState
    {
        Normal = 1,
        Disabled = 2,
        NonPublicForPortal = 8,
        All = Normal | Disabled | NonPublicForPortal
    }

    public class WebItemManager
    {
        private readonly static List<string> allowedItems;

        private readonly List<IWebItem> items;


        public static WebItemManager Instance
        {
            get;
            private set;
        }


        public IGlobalHandler ItemGlobalHandlers
        {
            get;
            private set;
        }

        public IWebItem this[Guid id]
        {
            get { return items.Find(i => id.Equals(i.ID)); }
        }


        static WebItemManager()
        {
            allowedItems = (ConfigurationManager.AppSettings["web.items"] ?? string.Empty)
                .Split(new[] { ',', ';', ' ' }, StringSplitOptions.RemoveEmptyEntries)
                .ToList();
            Instance = new WebItemManager();
        }

        private WebItemManager()
        {
            items = new List<IWebItem>();
        }

        public void LoadItems()
        {
            var handlers = new List<IGlobalHandler>();
            ItemGlobalHandlers = new GlobalHandlerComposite(handlers);

            ProductManager.Instance.LoadProducts();
            handlers.AddRange(ProductManager.Instance.GlobalHandlers);

            AddonManager.Instance.LoadAddons();
            handlers.AddRange(AddonManager.Instance.GlobalHandlers);
        }

        public bool RegistryItem(IWebItem webItem)
        {
            var systemname = GetSystemName(webItem);
            if (!items.Contains(webItem) && (allowedItems.Contains(systemname) || allowedItems.Count == 0))
            {
                items.Add(webItem);
                items.Sort((x, y) => GetSortOrder(x, null).CompareTo(GetSortOrder(y, null)));
                return true;
            }
            return false;
        }

        public List<IWebItem> GetItems(WebZoneType webZone)
        {
            return GetItems(webZone, ItemAvailableState.Normal);
        }

        public List<IWebItem> GetItems(WebZoneType webZone, ItemAvailableState avaliableState)
        {
            var webitems = items.FindAll(item =>
            {
                if ((avaliableState & ItemAvailableState.Disabled) != ItemAvailableState.Disabled && item.IsDisabled())
                {
                    return false;
                }
                var attribute = Attribute.GetCustomAttribute(item.GetType(), typeof(WebZoneAttribute), true) as WebZoneAttribute;
                return attribute != null && (attribute.Type & webZone) != 0;
            });
            return webitems;
        }

        public List<IWebItem> GetSubItems(Guid parentItemID)
        {
            return GetSubItems(parentItemID, ItemAvailableState.Normal);
        }

        public List<IWebItem> GetSubItems(Guid parentItemID, ItemAvailableState avaliableState)
        {
            var items = GetItems(WebZoneType.All, avaliableState);
            var modules = items.OfType<IProduct>()
                .Where(p => p.ID == parentItemID)
                .SelectMany(p => p.Modules.Select(m => m.ID));

            return items.FindAll(item => modules.Contains(item.ID));

        }

        public Guid GetParentItemID(Guid itemID)
        {
            foreach (var p in items.OfType<IProduct>())
            {
                if (p.Modules.Any(m => m.ID == itemID)) return p.ID;
            }
            return Guid.Empty;
        }

        public static int GetSortOrder(IWebItem item, WebItemSettings settings)
        {
            if (item == null) return 0;

            if (settings != null && item.IsSubItem())
            {
                var s = settings.SettingsCollection.Find(o => o.ItemID == item.ID);
                if (s != null && s.SortOrder != int.MinValue) return s.SortOrder;
            }

            var index = allowedItems.IndexOf(GetSystemName(item));
            if (index != -1) return index;

            return item.Context != null ? item.Context.DefaultSortOrder : 0;
        }

        public static int GetSortOrder(IWebItem item)
        {
            if (item == null) return 0;

            var index = allowedItems.IndexOf(GetSystemName(item));
            if (index != -1) return index;

            return item.Context != null ? item.Context.DefaultSortOrder : 0;
        }

        public List<IWebItem> GetItemsAll()
        {
            return items.ToList();
        }

        internal static string GetSystemName(IWebItem webitem)
        {
            if (webitem == null)
            {
                return string.Empty;
            }

            if (webitem is IRenderMyTools)
            {
                return ((IRenderMyTools)webitem).ParameterName;
            }

            if (string.IsNullOrEmpty(webitem.StartURL))
            {
                return webitem.GetType().Name;
            };

            var parts = webitem.StartURL.ToLower().Split('/', '\\').ToList();

            if (webitem is IProduct)
            {
                var index = parts.FindIndex(s => "products".Equals(s));
                if (0 <= index && index < parts.Count - 1)
                {
                    return parts[index + 1];
                }
            }

            if (webitem is IModule)
            {
                var index = parts.FindIndex(s => "modules".Equals(s));
                if (0 <= index && index < parts.Count - 1)
                {
                    return parts[index + 1];
                }
            }

            if (webitem is IAddon)
            {
                var index = parts.FindIndex(s => "addons".Equals(s));
                if (0 <= index && index < parts.Count - 1)
                {
                    return parts[index + 1];
                }
            }

            return parts[parts.Count - 1].Split('.')[0];
        }

        private WebItemSettings GetCurrentSettings()
        {
            return SettingsManager.Instance.LoadSettings<WebItemSettings>(CoreContext.TenantManager.GetCurrentTenant().TenantId);
        }
    }
}
