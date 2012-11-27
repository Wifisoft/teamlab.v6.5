using System;
using ASC.Files.Core.ProviderDao;
using ASC.Files.Core.Security;
using ASC.Files.Core.ThirdPartyDao;


namespace ASC.Files.Core.Data
{
    public class DaoFactory : IDaoFactory
    {
        private readonly int tenantID;
        private readonly String storageKey;

        public DaoFactory(int tenantID, String storageKey)
        {
            this.tenantID = tenantID;
            this.storageKey = storageKey;
        }


        public IFileDao GetFileDao()
        {
            return new ProviderFileDao();
        }

        public IFolderDao GetFolderDao()
        {
            return new ProviderFolderDao();
        }

        public ITagDao GetTagDao()
        {
            return new TagDao(tenantID, storageKey);
        }

        public ISecurityDao GetSecurityDao()
        {
            return new SecurityDao(tenantID, storageKey);
        }

        public IProviderDao GetProviderDao()
        {
            //NOTE: Added cached provider, so Sergey DON'T PANIC! Everything gonna be alright...Everything gonna be ok...
            return new CachedSharpBoxAccountDao(tenantID, storageKey); //TODO: can use factory
        }
    }
}