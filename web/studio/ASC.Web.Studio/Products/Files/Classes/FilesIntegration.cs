using System.Collections.Generic;
using ASC.Data.Storage;
using ASC.Files.Core;
using ASC.Files.Core.Security;
using ASC.Web.Files.Classes;

namespace ASC.Web.Files.Api
{
    public static class FilesIntegration
    {
        private static readonly IDictionary<string, IFileSecurityProvider> providers = new Dictionary<string, IFileSecurityProvider>();


        public static object RegisterBunch(string module, string bunch, string data)
        {
            using (var dao = GetFolderDao())
            {
                return dao.GetFolderID(module, bunch, data, true);
            }
        }

        public static bool IsRegisteredFileSecurityProvider(string module, string bunch)
        {
            lock (providers)
            {
                return providers.ContainsKey(module + bunch);
            }

        }

        public static void RegisterFileSecurityProvider(string module, string bunch, IFileSecurityProvider securityProvider)
        {
            lock (providers)
            {
                providers[module + bunch] = securityProvider;
            }
        }

        public static IFileDao GetFileDao()
        {
            return Global.DaoFactory.GetFileDao();
        }

        public static IFolderDao GetFolderDao()
        {
            return Global.DaoFactory.GetFolderDao();
        }

        public static ITagDao GetTagDao()
        {
            return Global.DaoFactory.GetTagDao();
        }

        public static FileSecurity GetFileSecurity()
        {
            return Global.GetFilesSecurity();
        }

        public static IDataStore GetStore()
        {
            return Global.GetStore();
        }


        internal static IFileSecurity GetFileSecurity(string path)
        {
            if (string.IsNullOrEmpty(path)) return null;

            var parts = path.Split('/');
            if (parts.Length < 3) return null;

            IFileSecurityProvider provider;
            lock (providers)
            {
                providers.TryGetValue(parts[0] + parts[1], out provider);
            }
            return provider != null ? provider.GetFileSecurity(parts[2]) : null;
        }
    }
}