using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using ASC.Data.Storage;
using ASC.Web.Core.Utility.Skins;

namespace ASC.Web.Projects.Classes
{

    public class PathProvider
    {
        private static Dictionary<String, String> virtualPathCache;


        public static readonly String BaseVirtualPath = "~/Products/Projects/".ToLower();

        public static readonly String BaseAbsolutePath = VirtualPathUtility.ToAbsolute(BaseVirtualPath).ToLower();


        static PathProvider()
        {
            virtualPathCache = new Dictionary<String, String>();
        }


        public static String GetFileStaticRelativePath(String fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return string.Empty;
            }
            if (fileName.EndsWith(".js"))
            {
                return WebPath.GetPath("/Products/Projects/js/" + fileName);
            }
            if (fileName.EndsWith(".png"))
            {
                return WebPath.GetPath("/Products/Projects/App_Themes/Default/Images/" + fileName);
            }
            if (fileName.EndsWith(".css"))
            {
                return WebSkin.GetUserSkin().GetAbsoluteWebPath("/Products/Projects/App_Themes/<theme_folder>/css/" + fileName);
            }
            return string.Empty;
        }

        public static String GetControlVirtualPath(String fileName)
        {
            lock (virtualPathCache)
            {
                var result = string.Empty;
                if (!virtualPathCache.TryGetValue(fileName, out result))
                {
                    var controlsAbsolutePath = HttpContext.Current.Server.MapPath(string.Concat(BaseVirtualPath, "Controls"));
                    var findedFiles = FindFile(controlsAbsolutePath, fileName);
                    if (findedFiles.Count == 0)
                    {
                        throw new FileNotFoundException();
                    }
                    if (1 < findedFiles.Count)
                    {
                        throw new Exception("Found " + findedFiles.Count + " files");
                    }
                    result = GetVirtualPath(findedFiles[0]);
                    if (!string.IsNullOrEmpty(result))
                    {
                        virtualPathCache.Add(fileName, result);
                    }
                }
                return result;
            }
        }

        public static string GetVirtualPath(string physicalPath)
        {
            var rootpath = HttpContext.Current.Server.MapPath("~/");
            return "~/" + physicalPath.Replace(rootpath, string.Empty).Replace("\\", "/");
        }

        private static List<String> FindFile(String directoryPath, String fileName)
        {
            var result = new List<string>();
            result.AddRange(Directory.GetFiles(directoryPath, fileName));
            foreach (var dir in Directory.GetDirectories(directoryPath))
            {
                result.AddRange(FindFile(dir, fileName));
            }
            return result;
        }
    }
}
