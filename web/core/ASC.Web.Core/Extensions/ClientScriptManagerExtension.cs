using System.Threading;
using ASC.Security.Cryptography;

namespace System.Web.UI
{
    public static class ClientScriptManagerExtension
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="clientScriptManager"></param>
        /// <param name="type">The type of the client script to register</param>
        /// <param name="resourseType"></param>
        /// <param name="jsObjectName"></param>
        public static void RegisterJavaScriptResource(this ClientScriptManager clientScriptManager, Type resourseType, String jsObjectName)
        {
            if (!clientScriptManager.IsStartupScriptRegistered(typeof(Page), jsObjectName))
            {
                var param = string.Join("|", new[] { resourseType.AssemblyQualifiedName, jsObjectName, Thread.CurrentThread.CurrentCulture.Name });
                var queryString = "?d=" + HttpUtility.UrlEncode(InstanceCrypto.Encrypt(param));
                var scriptContent = string.Format("<script type=\"text/javascript\" src=\"{0}\"></script>", String.Concat(VirtualPathUtility.ToAbsolute("~/JavaScriptResource.ashx"), queryString));
                clientScriptManager.RegisterStartupScript(typeof(Page), jsObjectName, scriptContent, false);
            }
        }
    }
}
