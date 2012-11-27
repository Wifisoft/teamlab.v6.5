using System;
using ASC.Core;
using ASC.Data.Storage;
using ASC.Projects.Core.Domain;
using ASC.Projects.Engine;
using ASC.Web.Core.Users;
using ASC.Web.Core.Utility.Settings;
using ASC.Web.Core.Utility.Skins;
using ASC.Web.Projects.Configuration;
using ASC.Web.Projects.Resources;
using ASC.Web.Studio.Utility;

namespace ASC.Web.Projects.Classes
{
    public class Global
    {

        #region Constants

        public static readonly string DB_ID = "projects";

        public static readonly String FileKeyFormat = "{0}/{1}/{2}/{3}"; // ProjectID/FileID/FileVersion/FileTitle

        public static readonly int EntryCountOnPage = 15;
        public static readonly int VisiblePageCount = 3;

        public static readonly String FileStorageModule = "projects";
        public static readonly String FileStorageModuleTemp = "projects_temp";

        #endregion

        #region Property

        public static EngineFactory EngineFactory
        {
            get { return new EngineFactory(DB_ID, TenantProvider.CurrentTenantID, GetProjectStore()); }
        }

        public static bool IsAdmin
        {
            get { return ProjectSecurity.IsAdministrator(SecurityContext.CurrentAccount.ID); }
        }

        private static ModuleManager _moduleManager;

        public static ModuleManager ModuleManager
        {
            get
            {
                if (_moduleManager == null) _moduleManager = new ModuleManager();
                return _moduleManager;
            }
        }

        private static ModuleSettings _moduleSettings;

        public static ModuleSettings ModuleSettings
        {
            get {
                return _moduleSettings ?? 
                      (_moduleSettings = SettingsManager.Instance.LoadSettings<ModuleSettings>(CoreContext.TenantManager.GetCurrentTenant().TenantId));
            }
        }

        #endregion

        #region Methods

        public static String RenderEntityPlate(EntityType entityType, bool isFixed)
        {

            string entityTitle;
            string backgroundColor;

            switch (entityType)
            {

                case EntityType.Team:
                    entityTitle = ProjectResource.Team.ToLower();
                    backgroundColor = "#473388";
                    break;
                case EntityType.Comment:
                    entityTitle = ProjectsCommonResource.Comment.ToLower();
                    backgroundColor = "#1d5f99";
                    break;
                case EntityType.Task:
                    entityTitle = TaskResource.Task.ToLower();
                    backgroundColor = "#88b601";
                    break;
                case EntityType.Project:
                    entityTitle = ProjectResource.Project.ToLower();
                    backgroundColor = "#f88e14";
                    break;
                case EntityType.Milestone:
                    entityTitle = MilestoneResource.Milestone.ToLower();
                    backgroundColor = "#e34603";
                    break;
                case EntityType.Message:
                    entityTitle = MessageResource.Message.ToLower();
                    backgroundColor = "#e4bc00";
                    break;
                case EntityType.File:
                    entityTitle = ProjectsFileResource.Documents.ToLower();
                    backgroundColor = "#0797ba";
                    break;
                case EntityType.TimeSpend:
                    entityTitle = ProjectsCommonResource.Time.ToLower();
                    backgroundColor = "#f67575";
                    break;
                case EntityType.SubTask:
                    entityTitle = TaskResource.Subtask.ToLower();
                    backgroundColor = "#f67575";
                    break;
                default:
                    entityTitle = "";
                    backgroundColor = "";
                    break;
            }

            entityTitle = entityTitle.Substring(0, 1).ToUpper() + entityTitle.Remove(0, 1);

            if (isFixed)
            {
                return String.Format(@"<div style='background-color: {1}' class='pm-entity-plate-fixed'>{0}</div>", entityTitle, backgroundColor);
            }

            return String.Format(@"<div style='background-color: {1}' class='pm-entity-plate'>{0}</div>", entityTitle, backgroundColor);
        }

        public static String RenderCommonContainerHeader(String title, EntityType entityType)
        {

            return
                String.Format(
                    @"
<table cellspacing='0' cellpadding='0'>    
<tr>
<td style='padding:5px 15px 0 0;vertical-align: top;'>
{0}
</td>
<td>
{1}
</td>
</tr>
</table> 
",
                    RenderEntityPlate(entityType, false), title);

        }

        public static String RenderPrivateProjectHeader(String title)
        {
            return
                String.Format(
                    @"
<table cellspacing='0' cellpadding='0'>    
<tr>
<td style='padding:5px 15px 0 0;vertical-align: top;'>
<img src='{2}' title='{3}' alt='{3}' style='float: left; margin-top: 1px;'/>
{0}
</td>
<td>
{1}
</td>
</tr>
</table> 
",
                    RenderEntityPlate(EntityType.Project, false),
                    title,
                    WebImageSupplier.GetAbsoluteWebPath("lock.png", ProductEntryPoint.ID),
                    ProjectResource.HiddenProject);
        }

        public static string GetHTMLUserAvatar(Guid userID)
        {
            var imgPath = UserPhotoManager.GetSmallPhotoURL(userID);
            if (imgPath != null)
                return "<img class=\"userMiniPhoto\" alt='' src=\"" + imgPath + "\"/>";

            return "";
        }

        public static IDataStore GetProjectStore()
        {
            return StorageFactory.GetStorage(PathProvider.BaseVirtualPath + "web.config", TenantProvider.CurrentTenantID.ToString(), "projects");
        }

        #endregion
    }
}