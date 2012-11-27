using System;
using System.Collections.Generic;

namespace ASC.Files.Core
{
    public interface IProviderDao : IDisposable
    {
        IProviderInfo GetProviderInfo(int linkId);
        List<IProviderInfo> GetProvidersInfo(FolderType folderType);
        int SaveProviderInfo(string providerName, string customerTitle, AuthData authData, FolderType folderType);
        int UpdateProviderInfo(int linkId, string customerTitle, AuthData authData, FolderType folderType);
        int UpdateProviderInfo(int linkId, string customerTitle);
        void RemoveProviderInfo(int linkId);
    }
}