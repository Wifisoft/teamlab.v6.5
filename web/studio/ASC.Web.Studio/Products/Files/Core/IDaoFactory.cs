using ASC.Files.Core.Security;

namespace ASC.Files.Core
{
    public interface IDaoFactory
    {
        IFolderDao GetFolderDao();

        IFileDao GetFileDao();

        ITagDao GetTagDao();

        ISecurityDao GetSecurityDao();

        IProviderDao GetProviderDao();
    }
}