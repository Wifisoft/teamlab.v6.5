#region usings

using System.Web;

#endregion

namespace ASC.Api.Interfaces
{
    public interface IApiHttpHandler:IHttpHandler
    {
        void Process(HttpContextBase context);
    }
}