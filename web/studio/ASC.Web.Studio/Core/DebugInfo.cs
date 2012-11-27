using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Web;
using ASC.Core;

namespace ASC.Web.Studio.Core
{
    class DebugInfo
    {
        private static readonly DateTime compileDateTime;


        public static bool ShowDebugInfo
        {
            get
            {
#if DEBUG
                return true;
#else
                return false;
#endif
            }
        }

        public static string DebugString
        {
            get
            {
                if (HttpContext.Current == null) return "Unknown (HttpContext is null)";

                var sb = new StringBuilder();
                sb.AppendFormat("Version: {0} {1}\\n", HttpContext.Current.Request.GetUrlRewriter().Host, compileDateTime);
                sb.AppendFormat("User: {0}\\n", SecurityContext.CurrentAccount);
                sb.AppendFormat("User Agent: {0}\\n", HttpContext.Current.Request.UserAgent);
                sb.AppendFormat("Url: {0}", HttpContext.Current.Request.Url);
                sb.AppendFormat("Rewriten Url: {0}", HttpContext.Current.Request.GetUrlRewriter());
                return sb.ToString();
            }
        }


        static DebugInfo()
        {
            try
            {
                const int PE_HEADER_OFFSET = 60;
                const int LINKER_TIMESTAMP_OFFSET = 8;
                var b = new byte[2048];
                using (var s = new FileStream(Assembly.GetCallingAssembly().Location, FileMode.Open, FileAccess.Read))
                {
                    s.Read(b, 0, 2048);
                }
                var i = BitConverter.ToInt32(b, PE_HEADER_OFFSET);
                var secondsSince1970 = BitConverter.ToInt32(b, i + LINKER_TIMESTAMP_OFFSET);
                compileDateTime = new DateTime(1970, 1, 1).AddSeconds(secondsSince1970);
            }
            catch { }
        }
    }
}
