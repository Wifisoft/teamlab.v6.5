#region usings

using System;
using System.Web;
using ASC.Api.Interfaces;

#endregion

namespace ASC.Api.Impl
{
    public class ApiAllowAllAuthorization : IApiAuthorization
    {
        #region IApiAuthorization Members

        public bool Authorize(HttpContextBase context)
        {
            return true;
        }

        public bool OnAuthorizationFailed(HttpContextBase context)
        {
            return false;
        }

        #endregion
    }
}