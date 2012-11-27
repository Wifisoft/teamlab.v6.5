#region usings

using System.Web;

#endregion

namespace ASC.Api.Interfaces
{
    public interface IApiAuthorization
    {
        bool Authorize(HttpContextBase context);
        bool OnAuthorizationFailed(HttpContextBase context);
    }
}