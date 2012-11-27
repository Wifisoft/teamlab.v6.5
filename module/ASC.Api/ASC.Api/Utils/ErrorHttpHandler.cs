#region usings

using System.Web;

#endregion

namespace ASC.Api.Utils
{
    public class ErrorHttpHandler : IHttpHandler
    {
        private readonly int _code;
        private readonly string _message;

        public ErrorHttpHandler(int code, string message)
        {
            _code = code;
            _message = message;
        }

        #region IHttpHandler Members

        public void ProcessRequest(HttpContext context)
        {
            context.Response.StatusCode = _code;
            context.Response.StatusDescription = _message;
            context.Response.Write(string.IsNullOrEmpty(_message)?"error":_message);
        }

        public bool IsReusable
        {
            get { return false; }
        }

        #endregion
    }
}