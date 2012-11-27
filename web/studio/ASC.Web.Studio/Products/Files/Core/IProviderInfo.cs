using System;

namespace ASC.Files.Core
{
    public interface IProviderInfo
    {
        int ID { get; set; }
        string ProviderName { get; }
        Guid Owner { get; }
        FolderType RootFolderType { get; }
        DateTime CreateOn { get; }
        string CustomerTitle { get; }
        string UserName { get; }

        object RootFolderId { get; }

        bool CheckAccess();
    }
}