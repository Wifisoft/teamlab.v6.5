using System;
using ASC.Core;
using ASC.Web.Core.Utility.Settings;

namespace ASC.Web.Core.Utility.Skins
{
    public static class WebImageSupplier
    {
        public static string GetAbsoluteWebPath(string imgFileName)
        {
            return GetAbsoluteWebPath(imgFileName, Guid.Empty);
        }

        public static string GetAbsoluteWebPath(string imgFileName, Guid moduleID)
        {
            var skin = WebSkin.GetUserSkin();
            return skin != null ? skin.GetImageAbsoluteWebPath(imgFileName, moduleID) : null;
        }

        public static string GetImageFolderAbsoluteWebPath()
        {
            return GetImageFolderAbsoluteWebPath(Guid.Empty);
        }

        public static string GetImageFolderAbsoluteWebPath(Guid moduleID)
        {
            return WebSkin.GetUserSkin().GetImageFolderAbsoluteWebPath(moduleID);
        }
    }
}
