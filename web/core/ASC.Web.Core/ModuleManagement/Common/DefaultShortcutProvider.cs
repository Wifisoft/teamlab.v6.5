using System;

namespace ASC.Web.Core.ModuleManagement.Common
{
    class DefaultShortcutProvider : IShortcutProvider
    {
        public string GetAbsoluteWebPathForShortcut(Guid shortcutID, string currentUrl)
        {
            return string.Empty;
        }

        public bool CheckPermissions(Guid shortcutID, string currentUrl)
        {
            return false;
        }
    }
}
