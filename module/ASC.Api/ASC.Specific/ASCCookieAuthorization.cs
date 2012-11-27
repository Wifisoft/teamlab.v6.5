#region usings

using System;
using System.Web;
using ASC.Api.Interfaces;
using ASC.Api.Logging;
using ASC.Api.Utils;
using ASC.Core;
using ASC.Web.Core;

#endregion

namespace ASC.Specific
{
    public class AscCookieAuthorization : IApiAuthorization
    {
        private readonly ILog _log;

        public AscCookieAuthorization(ILog log)
        {
            _log = log;
        }

        public bool Authorize(HttpContextBase context)
        {
            if (!SecurityContext.IsAuthenticated)
            {
                try
                {
                    var cookie = CookiesManager.GetRequestVar(CookiesType.AuthKey).If(x => string.IsNullOrEmpty(x), () => context.Request.Headers["Authorization"]);
                    if (string.IsNullOrEmpty(cookie))
                    {
                        
                    }
                    if (cookie != null && !string.IsNullOrEmpty(cookie))
                    {

                        if (!SecurityContext.AuthenticateMe(cookie))
                        {
                            _log.Warn("ASC cookie auth failed with cookie={0}",cookie);
                        }
                        return Core.SecurityContext.IsAuthenticated;

                    }
                    _log.Debug("no ASC cookie");
                }
                catch (Exception e)
                {
                    _log.Error(e,"ASC cookie auth error");
                }
            }
            return Core.SecurityContext.IsAuthenticated;
            
        }

        public bool OnAuthorizationFailed(HttpContextBase context)
        {
            return false;
        }
    }
}