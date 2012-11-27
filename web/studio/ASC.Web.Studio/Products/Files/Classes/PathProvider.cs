using System;
using System.Web;
using ASC.Data.Storage;
using ASC.Files.Core;
using ASC.Web.Core.Utility.Skins;

namespace ASC.Web.Files.Classes
{
    public static class PathProvider
    {
        public static readonly String BaseVirtualPath = "~/products/files/";

        public static readonly String ProjectVirtualPath = "~/products/projects/tmdocs.aspx";

        public static readonly String BaseAbsolutePath = VirtualPathUtility.ToAbsolute(BaseVirtualPath).ToLower();

        public static readonly String TemplatePath = "/products/files/templates/";

        public static readonly String StartURL = "~/products/files/";

        public static readonly String GetFileServicePath = VirtualPathUtility.ToAbsolute("~/products/files/services/wcfservice/service.svc/").ToLower();

        public static string GetImagePath(string imgFileName)
        {
            return WebImageSupplier.GetAbsoluteWebPath(imgFileName, Configuration.ProductEntryPoint.ID).ToLower();
        }

        public static String GetFileStaticRelativePath(String fileName)
        {
            if (fileName.EndsWith(".js"))
            {
                return WebPath.GetPath("/products/files/js/" + fileName).ToLowerInvariant();
            }
            if (fileName.EndsWith(".ascx"))
            {
                return VirtualPathUtility.ToAbsolute("~/products/files/controls/" + fileName).ToLowerInvariant();
            }
            if (fileName.EndsWith(".css"))
            {
                return WebSkin.GetUserSkin().GetAbsoluteWebPath("/products/files/app_themes/<theme_folder>/" + fileName).ToLowerInvariant();
            }
            return fileName;
        }

        public static string GetFolderUrl(Folder folder)
        {
            return GetFolderUrl(folder.ID, folder.RootFolderType == FolderType.BUNCH, folder.RootFolderId);
        }

        public static string GetFolderUrl(object folderId, bool bunch, object rootFolderIdIfBunch)
        {
            if (!bunch)
            {
                return VirtualPathUtility.ToAbsolute(BaseVirtualPath + "#" + folderId);
            }

            using (var folderDao = Global.DaoFactory.GetFolderDao())
            {
                var root = rootFolderIdIfBunch ?? folderDao.GetFolder(folderId).RootFolderId;
                int prId;

                return int.TryParse(folderDao.GetFolder(root).Title.Replace("projects/project/", ""), out prId)
                           ? string.Format("{0}?{1}={2}#{3}", VirtualPathUtility.ToAbsolute(ProjectVirtualPath), UrlConstant.ProjectId, prId, folderId)
                           : string.Empty;
            }

        }
    }
}