using System;
using System.Collections;
using System.Globalization;
using System.Net;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using ASC.Security.Cryptography;

namespace ASC.Web.Studio.HttpHandlers
{

    public class JavaScriptResourceHandler : IHttpHandler
    {
        private static readonly JavaScriptSerializer jsSerializer = new JavaScriptSerializer();


        public bool IsReusable
        {
            get { return true; }
        }

        public void ProcessRequest(HttpContext context)
        {
            var d = context.Request.QueryString["d"];
            if (string.IsNullOrEmpty(d))
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return;
            }

            var param = HttpUtility.UrlDecode(InstanceCrypto.Decrypt(d)).Split('|');
            if (param.Length != 3)
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return;
            }

            context.Response.Clear();
            context.Response.ContentType = "text/javascript";
            context.Response.Charset = "utf-8";

            var etag = d.GetHashCode().ToString();
            context.Response.Cache.SetETag(etag);
            if (string.Equals(context.Request.Headers["If-None-Match"], etag))
            {
                context.Response.StatusCode = (int)HttpStatusCode.NotModified;
            }
            else
            {
                context.Response.Cache.SetCacheability(HttpCacheability.Public);
                context.Response.Write(JavaScriptResourceSerialize(Type.GetType(param[0], true), param[1], CultureInfo.GetCultureInfo(param[2])));
            }
        }


        private static String JavaScriptResourceSerialize(Type resourceType, String className, CultureInfo culture)
        {
            var resourceManager = (ResourceManager)resourceType.GetProperty("ResourceManager", BindingFlags.Static | BindingFlags.Public).GetValue(null, null);
            var sb = new StringBuilder(className + "={ ");
            var neutralSet = resourceManager.GetResourceSet(CultureInfo.InvariantCulture, true, true);
            foreach (DictionaryEntry entry in neutralSet)
            {
                var key = (string)entry.Key;
                var value = resourceManager.GetString(key, culture).ReplaceSingleQuote();
                jsSerializer.Serialize(key, sb);
                sb.Append(':');
                jsSerializer.Serialize(value, sb);
                sb.Append(',');
            }
            sb.Length -= 1;
            sb.Append("};");
            return sb.ToString();
        }
    }
}