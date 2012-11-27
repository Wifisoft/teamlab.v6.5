namespace ASC.Api.Interfaces.Storage
{
    public interface IApiKeyValueStorage
    {
        object Get(IApiEntryPoint entrypoint, string key);
        void Set(IApiEntryPoint entrypoint, string key, object @object);
        bool Exists(IApiEntryPoint entrypoint, string key);
        void Remove(IApiEntryPoint entrypoint, string key);
        void Clear(IApiEntryPoint entrypoint);
    }
}