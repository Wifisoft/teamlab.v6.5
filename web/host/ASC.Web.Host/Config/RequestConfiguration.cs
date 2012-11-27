namespace ASC.Web.Host.Config
{
    class RequestConfiguration
    {
        public static string[] DefaultFileNames = new[]
        {
            "default.aspx", "default.htm", "default.html", "index.htm", "index.html"
        };

        public static string[] RestrictedDirs = new[]
        {
            "/bin", "/_private_folder", "/app_browsers", "/app_code", "/app_data", "/app_localresources", "/app_globalresources", "/app_webreferences"
        };
    }
}