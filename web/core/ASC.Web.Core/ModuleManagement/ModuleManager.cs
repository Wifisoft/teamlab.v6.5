using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using log4net;

namespace ASC.Web.Core.ModuleManagement
{
    public static class ModuleManager
    {
        private static readonly ILog log = LogManager.GetLogger("ASC.Web");


        public static List<IModule> LoadModules(string path)
        {
            var modules = new List<IModule>();
            var product = Path.GetFileName(Path.GetDirectoryName(path));
            foreach (var dir in Directory.GetDirectories(path))
            {
                var assemblyPath = Path.Combine(dir, string.Format("bin\\ASC.Web.{0}.{1}.dll", product, Path.GetFileName(dir)));
                if (File.Exists(assemblyPath))
                {
                    try
                    {
                        var assembly = Assembly.LoadFrom(assemblyPath);
                        var attr = assembly.GetCustomAttributes(typeof(ModuleAttribute), false).Cast<ModuleAttribute>().FirstOrDefault();
                        if (attr != null)
                        {
                            var m = (IModule)Activator.CreateInstance(attr.ModuleType);
                            modules.Add(m);
                            WebItemManager.Instance.RegistryItem(m);
                        }
                    }
                    catch (Exception exc)
                    {
                        log.Error(String.Format("Couldn't load product {0}", Path.GetFileName(path)), exc);
                    }
                }
            }
            modules.Sort((m1, m2) => m1.Context.DefaultSortOrder.CompareTo(m2.Context.DefaultSortOrder));
            return modules;
        }

        public static string GetModuleResource(string resourceClassTypeName, string resourseKey)
        {
            return string.Empty;
        }
    }
}
