using System;
using System.Net;
using System.Web;
using ASC.Common.Security.Authorizing;
using ASC.Core;

namespace ASC.Web.Studio.Core
{
    public class TcpIpFilterModule : IHttpModule
    {
        public void Init(HttpApplication context)
        {
            // after restore session and cookie authentication
            context.AcquireRequestState += OnAcquireRequestState;
        }

        public void OnAcquireRequestState(Object source, EventArgs e)
        {
            if (!SecurityContext.CurrentAccount.IsAuthenticated)
            {
                return;
            }
            if (SecurityContext.CurrentAccount.ID == CoreContext.TenantManager.GetCurrentTenant().OwnerId)
            {
                return;
            }

            var context = ((HttpApplication)source).Context;
            if (context.Request.Url.LocalPath.EndsWith(".aspx", StringComparison.InvariantCultureIgnoreCase))
            {
                try
                {
                    IPAddress ip;
                    if (IPAddress.TryParse(context.Request.UserHostName, out ip))
                    {
                        SecurityContext.DemandPermissions(new TcpIpFilterSecurityObject(ip), TcpIpFilterActions.TcpIpFilterAction);
                    }
                }
                catch (AuthorizingException error)
                {
                    throw new HttpException((int)HttpStatusCode.Forbidden, HttpStatusCode.Forbidden.ToString(), error);
                }
            }
        }

        public void Dispose()
        {

        }
    }
}
