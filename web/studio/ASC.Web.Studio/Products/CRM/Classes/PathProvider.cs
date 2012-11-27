#region Import

using System;
using System.Web;
using ASC.Data.Storage;
using ASC.Web.Core.Utility.Skins;

#endregion

namespace ASC.Web.CRM
{
    public  static class PathProvider
    {

        public static readonly String BaseVirtualPath = "~/products/crm/";
        public static readonly String BaseAbsolutePath = VirtualPathUtility.ToAbsolute(BaseVirtualPath).ToLower();

        public static String StartURL()
        {
            return "~/products/crm/";
        }

        public static string BaseSiteUrl
        {
            get
            {
                HttpContext context = HttpContext.Current;
                string baseUrl = context.Request.GetUrlRewriter().Scheme + "://" + context.Request.GetUrlRewriter().Authority + context.Request.ApplicationPath.TrimEnd('/') + '/';
                return baseUrl;
            }
        }

        public static string GetVirtualPath(string physicalPath)
        {
            string rootpath = HttpContext.Current.Server.MapPath("~/");
            physicalPath = physicalPath.Replace(rootpath, "");
            physicalPath = physicalPath.Replace("\\", "/");

            return "~/" + physicalPath;
        }

        public static String GetFileStaticRelativePath(String fileName)
        {
            if (fileName.EndsWith(".js"))
            {
                return WebPath.GetPath("/products/crm/js/" + fileName);
            }
            if (fileName.EndsWith(".ascx"))
            {
                return VirtualPathUtility.ToAbsolute("~/products/crm/controls/" + fileName);
            }
            if (fileName.EndsWith(".css"))
            {
                return WebSkin.GetUserSkin().GetAbsoluteWebPath("/products/crm/app_themes/<theme_folder>/css/" + fileName);
            }
            if (fileName.EndsWith(".png") || fileName.EndsWith(".gif") || fileName.EndsWith(".jpg"))
            {
                return WebSkin.GetUserSkin().GetAbsoluteWebPath("/products/crm/app_themes/<theme_folder>/images/" + fileName);
            }
            return fileName;       
        }
    }
}