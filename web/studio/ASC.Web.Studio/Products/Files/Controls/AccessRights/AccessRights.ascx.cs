using System;
using System.Web;
using System.Web.UI;
using ASC.Files.Core;
using ASC.Files.Core.Security;
using ASC.Web.Files.Classes;
using ASC.Web.Files.Resources;
using ASC.Web.Studio.UserControls.Common;

namespace ASC.Web.Files.Controls
{
    public partial class AccessRights : UserControl
    {
        public static string Location
        {
            get { return PathProvider.GetFileStaticRelativePath("AccessRights/AccessRights.ascx"); }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            var sharingSetting = (SharingSettings) LoadControl(SharingSettings.Location);
            sharingSetting.BodyCaption = FilesUCResource.SharingListCaption;
            _sharingContainer.Controls.Add(sharingSetting);

            confirmUnsubscribeDialog.Options.IsPopup = true;

            RegisteredScript();
        }

        private void RegisteredScript()
        {
            if (Page.ClientScript.IsStartupScriptRegistered(GetType(), "{BC0B4987-672B-416a-8828-C80065BEAC4D}"))
                return;

            var script =

                String.Format(
                    @"
                      ASC.Files.Constants.AceStatusEnum = {{}};
                      ASC.Files.Constants.AceStatusEnum.None = {0};
                      ASC.Files.Constants.AceStatusEnum.ReadWrite = {1};
                      ASC.Files.Constants.AceStatusEnum.Read = {2};
                      ASC.Files.Constants.AceStatusEnum.Restrict = {3};

                      ASC.Files.Constants.ShareLinkId = ""{4}"";
                      ASC.Files.Constants.ShareLinkRead = ""{5}"";
                      ASC.Files.Constants.ShareLinkReadWrite = ""{6}"";
                    ",
                    (int) FileShare.None,
                    (int) FileShare.ReadWrite,
                    (int) FileShare.Read,
                    (int) FileShare.Restrict,
                    FileConstant.ShareLinkId,
                    VirtualPathUtility.ToAbsolute(PathProvider.BaseVirtualPath + "docviewer.aspx").ToLower(),
                    VirtualPathUtility.ToAbsolute(PathProvider.BaseVirtualPath + "doceditor.aspx").ToLower()
                    );

            Page.ClientScript.RegisterStartupScript(GetType(), "{BC0B4987-672B-416a-8828-C80065BEAC4D}", script, true);
        }
    }
}