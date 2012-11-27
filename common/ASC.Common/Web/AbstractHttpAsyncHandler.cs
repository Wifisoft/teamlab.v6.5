using System;
using System.Web;
using System.Web.SessionState;

namespace ASC.Common.Web
{
    public abstract class AbstractHttpAsyncHandler : IHttpAsyncHandler, IReadOnlySessionState
    {
        private Action<HttpContext> processRequest;


        public bool IsReusable
        {
            get { return false; }
        }


        public void ProcessRequest(HttpContext context)
        {
            HttpContext.Current = context;
            OnProcessRequest(context);
        }

        public IAsyncResult BeginProcessRequest(HttpContext context, AsyncCallback cb, object extraData)
        {
            processRequest = ProcessRequest;
            return processRequest.BeginInvoke(context, cb, extraData);
        }

        public void EndProcessRequest(IAsyncResult result)
        {
            processRequest.EndInvoke(result);
        }


        public abstract void OnProcessRequest(HttpContext context);
    }
}
