#if DEBUG
namespace ASC.Web.Files.Tests
{
    using System.Configuration;
    using ASC.Common.Data;
    using ASC.Files.Core;
    using ASC.Files.Core.Data;
    using ASC.Files.Core.Security;
    using log4net;

    public class BaseTest
    {
        protected ILog Logger
        {
            get;
            private set;
        }

        protected IFileDao FileDAO
        {
            get;
            private set;
        }

        protected IFolderDao FolderDAO
        {
            get;
            private set;
        }

        protected ISecurityDao SecurityDAO
        {
            get;
            private set;
        }

        protected int Tenant
        {
            get;
            private set;
        }


        public BaseTest()
        {
            Logger = LogManager.GetLogger("ASC.Files.Tests");

            DbRegistry.RegisterDatabase("files", ConfigurationManager.ConnectionStrings["files"]);

            var factory = new DaoFactory(Tenant, "files");
            FolderDAO = factory.GetFolderDao();
            FileDAO = factory.GetFileDao();
            SecurityDAO = factory.GetSecurityDao();
        }
    }
}
#endif