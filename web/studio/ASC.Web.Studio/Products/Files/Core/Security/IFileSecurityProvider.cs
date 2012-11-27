namespace ASC.Files.Core.Security
{
    public interface IFileSecurityProvider
    {
        IFileSecurity GetFileSecurity(string data);
    }
}