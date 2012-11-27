namespace ASC.Web.Host.HttpHandlers
{
    class AspNetHttpHandler : IHttpHandler
    {
        public void ProcessRequest(HttpHandlerContext context)
        {
            context.Host.ProcessRequest(context.Connection, context.Identity);
        }
    }
}
