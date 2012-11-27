namespace ASC.Files.Core.ProviderDao
{
    internal interface IDaoSelector
    {
        bool IsMatch(object id);
        IFileDao GetFileDao(object id);
        IFolderDao GetFolderDao(object id);
        object ConvertId(object id);
        object GetIdCode(object id);
    }
}