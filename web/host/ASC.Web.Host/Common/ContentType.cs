namespace ASC.Web.Host.Common
{
    using System.IO;
    using Microsoft.Win32;

    class ContentType
    {
        internal static string GetContentType(string fileName)
        {
            string mime = "application/octetstream";
            string ext = Path.GetExtension(fileName).ToLower();
            RegistryKey rk = Registry.ClassesRoot.OpenSubKey(ext);
            if (rk != null && rk.GetValue("Content Type") != null)
            {
                mime = rk.GetValue("Content Type").ToString();
            }
            return (string.Format("Content-Type: {0}\r\n", mime));
        }
    }
}