namespace ASC.Web.Host.HttpHandlers
{
    interface IHttpHandler
    {
        void ProcessRequest(HttpHandlerContext context);
    }
}
