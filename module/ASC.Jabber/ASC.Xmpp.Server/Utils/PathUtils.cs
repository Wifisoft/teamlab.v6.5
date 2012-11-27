using System;
using System.IO;

namespace ASC.Xmpp.Server.Utils
{
	static class PathUtils
	{
        private const string DATA_DIRECTORY = "|DataDirectory|";
        private const string DATA_DIRECTORY_KEY = "DataDirectory";


        public static string GetAbsolutePath(string path)
		{
            var currDir = AppDomain.CurrentDomain.BaseDirectory;
            if (path.Trim(Path.DirectorySeparatorChar).StartsWith(DATA_DIRECTORY, StringComparison.CurrentCultureIgnoreCase))
            {
                path = path.Substring(DATA_DIRECTORY.Length).Trim(Path.DirectorySeparatorChar);
                var dataDir = (string)AppDomain.CurrentDomain.GetData(DATA_DIRECTORY_KEY) ?? currDir;
                path = Path.Combine(dataDir, path);
            }
            return Path.IsPathRooted(path) ? path : Path.Combine(currDir, path);
        }
	}
}
