using System;
using System.Text;
using System.Web;
using System.Xml.Linq;
using ASC.Web.Talk.Addon;

namespace ASC.Web.Talk.HttpHandlers
{
    public class UnreadMessagesHandler : IHttpHandler
    {
        public bool IsReusable
        {
            // To enable pooling, return true here.
            // This keeps the handler in memory.
            get { return false; }
        }

        public void ProcessRequest(HttpContext context)
        {
            var count = 0;
            if (context.Request.Url != null && !string.IsNullOrEmpty(context.Request.Url.Query))
            {
                foreach (var query in context.Request.Url.Query.Trim().Trim('?').Split('&'))
                {
                    if (query.StartsWith("u=", StringComparison.CurrentCultureIgnoreCase))
                    {
                        var username = query.Substring(2);
                        count = TalkAddon.GetMessageCount(username);
                    }
                }
            }
            context.Response.Cache.SetCacheability(HttpCacheability.Public);
            context.Response.ContentType = "application/xml";
            context.Response.Charset = Encoding.UTF8.WebName;
            var xml = new XDocument(new XElement("response", new XElement("value", count))).ToString();
            context.Response.Write(Encoding.UTF8.GetString(Encoding.Convert(Encoding.Unicode, Encoding.UTF8, Encoding.Unicode.GetBytes(xml))));
        }
    }
}
