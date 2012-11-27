
namespace ASC.Web.Host
{
    public static class ServerFactory
    {
        public static IServer CreateHTTPWebServer()
        {
            return new ASC.Web.Host.HttpRequestProcessor.Server();
        }
    }
}