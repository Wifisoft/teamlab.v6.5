using System.ServiceModel;

namespace ASC.FullTextIndex
{
    [ServiceContract]
    public interface ITextIndexService
    {
        [OperationContract]
        bool SupportModule(string module);

        [OperationContract]
        TextSearchResult Search(int tenant, string query, string module);
    }
}
