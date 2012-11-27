using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using ASC.Data.Storage.Configuration;

namespace ASC.Data.Storage
{
    public static class StorageFactory
    {
        private const string DefaultTenantName = "default";

        public static IDataStore GetStorage(string tenant, string module)
        {
            return GetStorage(tenant, module, HttpContext.Current);
        }

        public static IDataStore GetStorage(string tenant, string module, HttpContext context)
        {
            return GetStorage(string.Empty, tenant, module, context);
        }

        private static IDataStore GetStoreAndCache(string tenant, string module, StorageConfigurationSection section,
                                                   HttpContext context, IQuotaController controller)
        {
            IDataStore store = GetDataStore(tenant, section, module, context, controller);
            if (store != null)
            {
                DataStoreCache.Put(store, tenant, module);
            }
            return store;
        }

        public static IDataStore GetStorage(string configpath, string tenant, string module)
        {
            return GetStorage(configpath, tenant, module, HttpContext.Current);
        }

        public static IDataStore GetStorage(string configpath, string tenant, string module, HttpContext context)
        {
            return GetStorage(configpath, tenant, module, context, new TennantQuotaController(tenant));
        }

        public static IDataStore GetStorage(string configpath, string tenant, string module, HttpContext context,
                                            IQuotaController controller)
        {
            if (tenant == null) tenant = DefaultTenantName;

            //Make tennant path
            tenant = TennantPath.CreatePath(tenant);

            IDataStore store = DataStoreCache.Get(tenant, module);
            if (store == null)
            {
                StorageConfigurationSection section = GetSection(configpath);
                if (section == null) throw new InvalidOperationException("config section not found");
                store = GetStoreAndCache(tenant, module, section, context, controller);
            }
            return store;
        }

        private static IDataStore GetDataStore(string tenant, StorageConfigurationSection section, string module,
                                               HttpContext context, IQuotaController controller)
        {
            ModuleConfigurationElement moduleElement = section.Modules.GetModuleElement(module);
            if (moduleElement == null) throw new ArgumentException("no such module");
            HandlerConfigurationElement handler = section.Handlers.GetHandler(moduleElement.Type);
            return
                ((IDataStore) Activator.CreateInstance(handler.Type, tenant, moduleElement, context)).Configure(
                handler.GetProperties()).SetQuotaController(moduleElement.Count?controller:null/*don't count quota if specified on module*/);
        }


        public static IEnumerable<string> GetModuleList(string configpath)
        {
            StorageConfigurationSection section = GetSection(configpath);
            return section.Modules.Cast<ModuleConfigurationElement>().Where(x => x.Visible).Select(x => x.Name);
        }

        private static StorageConfigurationSection GetSection(string configpath)
        {
            StorageConfigurationSection section;
            if (!string.IsNullOrEmpty(configpath))
            {
                if (configpath.Contains("\\") && !Uri.IsWellFormedUriString(configpath, UriKind.Relative))
                    //Not mapped path
                {
                    var configMap = new ExeConfigurationFileMap
                                        {
                                            ExeConfigFilename =
                                                string.Compare(Path.GetExtension(configpath), ".config", true) == 0
                                                    ? configpath
                                                    : Path.Combine(configpath, "web.config")
                                        };
                    section =
                        (StorageConfigurationSection)
                        ConfigurationManager.OpenMappedExeConfiguration(configMap, ConfigurationUserLevel.None).
                            GetSection(Schema.SECTION_NAME);
                }
                else
                {
                    section = (StorageConfigurationSection)
                              WebConfigurationManager.OpenWebConfiguration(configpath).GetSection(Schema.SECTION_NAME);
                }
            }
            else
            {
                //Nothing worked, try local
                section = (StorageConfigurationSection) ConfigurationManager.GetSection(Schema.SECTION_NAME);
            }
            return section;
        }

        public static IEnumerable<string> GetDomainList(string configpath, string modulename)
        {
            StorageConfigurationSection section = GetSection(configpath);
            if (section == null) throw new ArgumentException("config section not found");
            return
                section.Modules.Cast<ModuleConfigurationElement>().Where(
                    x => x.Name.Equals(modulename, StringComparison.OrdinalIgnoreCase)).Single().Domains.Cast
                    <DomainConfigurationElement>().Where(x => x.Visible).Select(x => x.Name);
        }
    }
}