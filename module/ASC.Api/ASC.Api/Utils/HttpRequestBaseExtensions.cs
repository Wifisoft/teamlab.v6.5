
namespace System.Web
{
    public static class HttpRequestBaseExtensions
    {
        public static Uri GetUrlRewriter(this HttpRequestBase request)
        {
            return request != null ? HttpRequestExtensions.GetUrlRewriter(request.Headers, request.Url) : null;
        }
    }
}
