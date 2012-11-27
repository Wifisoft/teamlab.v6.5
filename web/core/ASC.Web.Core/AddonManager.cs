using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using log4net;

namespace ASC.Web.Core
{

    public class AddonManager
    {
        private static readonly ILog log = LogManager.GetLogger("ASC.Web");

        public static AddonManager Instance
        {
            get;
            private set;
        }

        public List<IAddon> Addons { get; private set; }

        public List<IGlobalHandler> GlobalHandlers { get; private set; }


        static AddonManager()
        {
            Instance = new AddonManager();
        }

        private AddonManager()
        {
            Addons = new List<IAddon>();
            AddonContexts = Hashtable.Synchronized(new Hashtable());
        }

        public Hashtable AddonContexts { get; private set; }

        public void LoadAddons()
        {
            if (HttpContext.Current == null) return;

            GlobalHandlers = new List<IGlobalHandler>();

            var addonsDir = HttpContext.Current.Server.MapPath("~/addons");
            if (!Directory.Exists(addonsDir)) return;

            foreach (var path in Directory.GetDirectories(addonsDir))
            {
                var productAssemblyPath = Path.Combine(path, "bin\\ASC.Web." + Path.GetFileName(path) + ".dll");
                if (File.Exists(productAssemblyPath))
                {
                    try
                    {
                        var addonAssembly = Assembly.LoadFrom(productAssemblyPath);
                        var attr = addonAssembly.GetCustomAttributes(typeof(AddonAttribute), false).Cast<AddonAttribute>().FirstOrDefault();
                        if (attr != null)
                        {
                            var addon = (IAddon)Activator.CreateInstance(attr.AddonType);

                            var addonContext = new AddonContext { AssemblyName = addonAssembly.FullName };

                            addon.Init(addonContext);
                            AddonContexts.Add(addon.ID, addonContext);

                            if (addonContext.GlobalHandler != null)
                            {
                                GlobalHandlers.Add(addonContext.GlobalHandler);
                            }
                            //todo: register useractivity handler

                            Addons.Add(addon);
                            WebItemManager.Instance.RegistryItem(addon);

                            log.DebugFormat("Addon {0} loaded", addon.Name);
                        }
                    }
                    catch (Exception exc)
                    {
                        log.Error(String.Format("Couldn't load addon {0}", Path.GetFileName(path)), exc);
                    }
                }
            }
        }
    }
}
