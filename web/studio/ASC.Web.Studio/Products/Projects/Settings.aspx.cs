#region Import

using System;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using System.Web;
using ASC.Web.Studio.Masters;
using AjaxPro;
using ASC.Core;
using ASC.Core.Users;
using ASC.Projects.Engine;
using ASC.Web.Controls;
using ASC.Web.Core.Utility.Settings;
using ASC.Web.Projects.Classes;
using ASC.Web.Projects.Configuration;
using ASC.Web.Projects.Resources;
using ASC.Web.Studio.Utility;

#endregion

namespace ASC.Web.Projects
{
    [AjaxNamespace("AjaxPro.SettingsPage")]
    public partial class Settings : BasePage
    {
        #region Properties

        protected ModuleSettings ModuleSettings { get; set; }

        protected bool NewVersion { get; set; }

        #endregion

        protected override void PageLoad()
        {
            ((IStudioMaster)Master).DisabledSidePanel = true;

            if (!ProjectSecurity.IsAdministrator(SecurityContext.CurrentAccount.ID))
                HttpContext.Current.Response.Redirect(ProjectsCommonResource.StartURL, true);

            AjaxPro.Utility.RegisterTypeForAjax(typeof(Settings));

            InitPage();
        }

        public void InitPage()
        {
            Master.BreadCrumbs.Add(new BreadCrumb
            {
                Caption = SettingsResource.ImportFromBasecamp
            });

            Title = HeaderStringHelper.GetPageTitle(SettingsResource.ImportFromBasecamp, Master.BreadCrumbs);

            HiddenFieldForPermission.Value = CoreContext.UserManager.IsUserInGroup(SecurityContext.CurrentAccount.ID, Constants.GroupAdmin.ID) ? "1" : "0";

            ModuleSettings = SettingsManager.Instance.LoadSettings<ModuleSettings>(CoreContext.TenantManager.GetCurrentTenant().TenantId);

            import_info_container.Options.IsPopup = true;
        }

        public void EndProcessInformation(IAsyncResult result)
        {

        }

        public string GetDefaultTabURL(DefaultTab tab)
        {
            switch (tab)
            {
                case DefaultTab.Dashboard:
                    return String.Concat(PathProvider.BaseAbsolutePath, "default.aspx");
                case DefaultTab.Projects:
                    return String.Concat(PathProvider.BaseAbsolutePath, "projects.aspx");
                case DefaultTab.MyTasks:
                    return String.Concat(PathProvider.BaseAbsolutePath, "mytasks.aspx");
                case DefaultTab.Reports:
                    return String.Concat(PathProvider.BaseAbsolutePath, "reports.aspx");
            }
            return string.Empty;
        }

        [AjaxMethod]
        public ImportStatus StartImportFromBasecamp(string url, string token, bool processClosed, bool disableNotifications)
        {
            ProjectSecurity.DemandAuthentication();

            //Validate all data
            if (string.IsNullOrEmpty(url))
                throw new ArgumentException(SettingsResource.EmptyURL);
            if (string.IsNullOrEmpty(token))
                throw new ArgumentException(SettingsResource.EmptyToken);
            if (!Uri.IsWellFormedUriString(url, UriKind.Absolute))
                throw new ArgumentException(SettingsResource.MalformedUrl);
            if (!Regex.IsMatch(url.Trim(), @"^http[s]{0,1}://.+\.basecamphq\.com[/]{0,1}$"))
                throw new ArgumentException(SettingsResource.NotBasecampUrl);

            ImportQueue.Add(url, token, processClosed, disableNotifications);
            return ImportQueue.GetStatus();
        }

        [AjaxMethod]
        public ImportStatus GetImportFromBasecampStatus()
        {
            ProjectSecurity.DemandAuthentication();

            return ImportQueue.GetStatus();
        }
    }


    public class ModuleSettingsWrapper
    {
        public bool ViewCalendar { get; set; }
        public bool ViewFiles { get; set; }
        public bool ViewTimeTracking { get; set; }
        public bool ViewIssueTracker { get; set; }
    }

    public enum DefaultTab
    {
        Dashboard = 0,
        Projects = 1,
        MyTasks = 2,
        Reports = 3
    }

    [Serializable]
    [DataContract]
    public class ModuleSettings : ISettings
    {
        [DataMember(Name = "ShowCalendar")]
        public bool ShowCalendar { get; set; }

        [DataMember(Name = "ShowFiles")]
        public bool ShowFiles { get; set; }

        [DataMember(Name = "ShowTimeTracking")]
        public bool ShowTimeTracking { get; set; }

        [DataMember(Name = "ShowIssueTracker")]
        public bool ShowIssueTracker { get; set; }

        public Guid ID
        {
            get { return new Guid("{649301E2-BA73-4a65-884D-12A0E8FDFCE4}"); }
        }
        public ISettings GetDefault()
        {
            return new ModuleSettings { 
                ShowCalendar = true, 
                ShowFiles = true, 
                ShowTimeTracking = true, 
                ShowIssueTracker = true};
        }
    }
}
