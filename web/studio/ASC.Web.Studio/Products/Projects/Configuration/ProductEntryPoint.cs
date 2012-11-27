using System;
using System.Collections.Generic;
using System.Web.Configuration;
using ASC.Common.Data;
using ASC.Core;
using ASC.Core.Tenants;
using ASC.Notify;
using ASC.Notify.Engine;
using ASC.Projects.Core.Services.NotifyService;
using ASC.Projects.Data;
using ASC.Projects.Engine;
using ASC.Web.Core;
using ASC.Web.Core.Users.Activity;
using ASC.Web.Core.Utility;
using ASC.Web.Files.Api;
using ASC.Web.Projects.Classes;
using ASC.Web.Projects.Controls.Projects;
using ASC.Web.Projects.Resources;
using log4net;
using ConfigurationManager = ASC.Projects.Engine.ConfigurationManager;
using System.Web;
using System.Linq;

namespace ASC.Web.Projects.Configuration
{
    public class ProductEntryPoint : Product
    {
        private ProductContext context;

        private List<NavigationWebItem> navigationsItems = new List<NavigationWebItem>();

        public static readonly Guid ID = EngineFactory.ProductId;


        public override Guid ProductID
        {
            get { return ID; }
        }

        public override string Name
        {
            get { return ProjectsCommonResource.ProductName; }
        }

        public override string ExtendedDescription
        {
            get { return string.Format(ProjectsCommonResource.ProductDescriptionEx, "<span style='display:none'>", "</span>"); }
        }

        public override string Description
        {
            get
            {
                return ProjectsCommonResource.ProductDescription;
            }

        }

        public override string StartURL
        {
            get
            {
                try
                {
                    return string.Concat(PathProvider.BaseAbsolutePath, "default.aspx");
                }
                catch
                {
                    return ProjectsCommonResource.StartURL.ToLower();
                }
            }
        }

        public override IModule[] Modules
        {
            get { return Global.ModuleManager.Modules.ToArray(); }
        }

        public override ProductContext Context
        {
            get { return context; }
        }

        public string ModuleDescription
        {
            get { return string.Empty; }
        }

        public Guid ModuleID
        {
            get { return ID; }
        }

        public string ModuleName
        {
            get { return ProjectsCommonResource.ModuleName; }
        }

        public SendInterceptorSkeleton InterceptorSecurity
        {
            get
            {
                return new SendInterceptorSkeleton(
                    "ProjectInterceptorSecurity",
                    InterceptorPlace.DirectSend,
                    InterceptorLifetime.Global,
                    (nreq, place) =>
                    {
                        var entityType = nreq.ObjectID.Split('_')[0];
                        var entityId = Convert.ToInt32(nreq.ObjectID.Split('_')[1]);

                        switch (entityType)
                        {
                            case "Task":
                                var task = Global.EngineFactory.GetTaskEngine().GetByID(entityId);
                                return task != null && !ProjectSecurity.CanRead(task, new Guid(nreq.Recipient.ID));
                            case "Message":
                                var discussion = Global.EngineFactory.GetMessageEngine().GetByID(entityId);
                                return !ProjectSecurity.CanRead(discussion, new Guid(nreq.Recipient.ID));
                            case "Milestone":
                                var milestone = Global.EngineFactory.GetMilestoneEngine().GetByID(entityId);
                                return !ProjectSecurity.CanRead(milestone, new Guid(nreq.Recipient.ID));
                        }

                        return false;
                    });
            }
        }

        public override void Init(ProductContext productContext)
        {
            if (!DbRegistry.IsDatabaseRegistered(Global.DB_ID))
                DbRegistry.RegisterDatabase(Global.DB_ID, WebConfigurationManager.ConnectionStrings[Global.DB_ID]);

            new SearchHandler();

            ConfigurationManager.Configure(ID, PathProvider.BaseVirtualPath, String.Empty, Global.FileStorageModule);

            productContext.ThemesFolderVirtualPath = String.Concat(PathProvider.BaseVirtualPath, "App_Themes");
            productContext.ImageFolder = "images";
            productContext.MasterPageFile = String.Concat(PathProvider.BaseVirtualPath, "Masters/BasicTemplate.Master");
            productContext.DisabledIconFileName = "product_disabled_logo.png";
            productContext.IconFileName = "product_logo.png";
            productContext.LargeIconFileName = "product_logolarge.png";
            productContext.SubscriptionManager = new ProductSubscriptionManager();
            productContext.UserActivityControlLoader = new ProjectActivity();
            productContext.WhatsNewHandler = new WhatsNewHandler();
            productContext.UserActivityPublishers = new List<IUserActivityPublisher>() { new TimeLinePublisher() };
            productContext.DefaultSortOrder = 10;
            productContext.SpaceUsageStatManager = new ProjectsSpaceUsageStatManager();
            productContext.AdminOpportunities = GetAdminOpportunities;
            productContext.UserOpportunities = GetUserOpportunities;
            productContext.HasComplexHierarchyOfAccessRights = true;

            context = productContext;

            NotifyClient.Instance.Client.RegisterSendMethod(SendMsgMilestoneDeadline, TimeSpan.FromDays(1), DateTime.UtcNow.Date.AddHours(7));
            NotifyClient.Instance.Client.RegisterSendMethod(ReportHelper.SendAutoReports, TimeSpan.FromHours(1), DateTime.UtcNow.Date.AddHours(DateTime.UtcNow.Hour));
            NotifyClient.Instance.Client.RegisterSendMethod(TaskHelper.SendAutoReminderAboutTask, TimeSpan.FromHours(1), DateTime.UtcNow.Date.AddHours(DateTime.UtcNow.Hour));

            NotifyClient.Instance.Client.AddInterceptor(InterceptorSecurity);

            UserActivityManager.AddFilter(new WhatsNewHandler());

            FilesIntegration.RegisterFileSecurityProvider("projects", "project", new SecurityAdapterProvider());
            SearchHandlerManager.Registry(new SearchHandler());
        }

        private List<string> GetAdminOpportunities()
        {
            return ProjectsCommonResource.ProductAdminOpportunities.Split('|').ToList();
        }

        private List<string> GetUserOpportunities()
        {
            return ProjectsCommonResource.ProductUserOpportunities.Split('|').ToList();
        }

        public override void Shutdown()
        {
            NotifyClient.Instance.Client.UnregisterSendMethod(SendMsgMilestoneDeadline);
            NotifyClient.Instance.Client.UnregisterSendMethod(ReportHelper.SendAutoReports);
            NotifyClient.Instance.Client.UnregisterSendMethod(TaskHelper.SendAutoReminderAboutTask);

            NotifyClient.Instance.Client.RemoveInterceptor(InterceptorSecurity.Name);
        }

        private void SendMsgMilestoneDeadline(DateTime scheduleDate)
        {
            var date = DateTime.UtcNow.AddDays(2);
            foreach (var r in new DaoFactory(Global.DB_ID, Tenant.DEFAULT_TENANT).GetMilestoneDao().GetInfoForReminder(date))
            {
                var tenant = CoreContext.TenantManager.GetTenant((int)r[0]);
                if (tenant == null || tenant.Status != TenantStatus.Active) continue;

                var localTime = TenantUtil.DateTimeFromUtc(tenant, date);
                if (localTime.Date == ((DateTime)r[2]).Date)
                {
                    try
                    {
                        CoreContext.TenantManager.SetCurrentTenant(tenant);
                        var m = new DaoFactory(Global.DB_ID, tenant.TenantId).GetMilestoneDao().GetById((int)r[1]);
                        if (m != null)
                        {
                            var sender = !m.Responsible.Equals(Guid.Empty) ? m.Responsible : m.Project.Responsible;
                            NotifyClient.Instance.SendMilestoneDeadline(sender, m);
                        }
                    }
                    catch (Exception ex)
                    {
                        LogManager.GetLogger("ASC.Projects.Tasks").Error("SendMsgMilestoneDeadline", ex);
                    }
                }
            }
        }
    }
}