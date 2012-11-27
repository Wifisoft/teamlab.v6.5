namespace ASC.Api.Interfaces
{
    public interface IApiStoragePath
    {
        string GetDataDirectory(IApiEntryPoint entryPoint);
    }
}