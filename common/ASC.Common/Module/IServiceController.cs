
namespace ASC.Common.Module
{
    public interface IServiceController
    {
        string ServiceName
        {
            get;
        }

        void Start();

        void Stop();
    }
}
