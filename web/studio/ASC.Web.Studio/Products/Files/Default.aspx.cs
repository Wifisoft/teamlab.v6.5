using ASC.Web.Files.Classes;
using ASC.Web.Files.Resources;
using ASC.Web.Files.Controls;
using ASC.Web.Files.Import;
using ASC.Web.Studio.Core;

namespace ASC.Web.Files
{
    public partial class _Default : BasePage
    {
        protected override void PageLoad()
        {
            if (ASC.Core.SecurityContext.IsAuthenticated)
            {
                CommonContainerHolder.Controls.Add(LoadControl(MainMenu.Location));
            }

            var mainContent = (MainContent) LoadControl(MainContent.Location);
            if (!SetupInfo.IsPersonal)
            {
                mainContent.FolderIDShare = Global.FolderShare;
                mainContent.FolderIDCommonRoot = Global.FolderCommon;
            }
            mainContent.FolderIDUserRoot = Global.FolderMy;
            mainContent.FolderIDTrash = Global.FolderTrash;
            mainContent.TitlePage = FilesCommonResource.TitlePage;
            CommonContainerHolder.Controls.Add(mainContent);

            if (Global.EnableShare)
                CommonContainerHolder.Controls.Add(LoadControl(AccessRights.Location));

            if (ASC.Core.SecurityContext.IsAuthenticated)
            {
                if (ImportConfiguration.SupportImport)
                    CommonContainerHolder.Controls.Add(LoadControl(ImportControl.Location));
            }

            Master.DisabledSidePanel = true;
            Master.CommonContainerHeaderVisible = false;
        }
    }
}