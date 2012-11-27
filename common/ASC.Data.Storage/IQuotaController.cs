namespace ASC.Data.Storage
{
    public interface IQuotaController
    {
        void QuotaUsedAdd(string module, string domain, string dataTag, long size);

        void QuotaUsedDelete(string module, string domain, string dataTag, long size);

        void QuotaUsedSet(string module, string domain, string dataTag, long size);

        long QuotaUsedGet(string module, string domain);
    }
}