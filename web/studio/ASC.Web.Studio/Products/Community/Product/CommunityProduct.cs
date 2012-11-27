using System;
using System.Collections.Generic;
using System.Web;
using ASC.Web.Community.Resources;
using ASC.Web.Core;
using ASC.Web.Core.ModuleManagement;
using ASC.Web.Core.Utility;
using System.Linq;

namespace ASC.Web.Community.Product
{
    public class CommunityProduct : ASC.Web.Core.Product
    {
        private static List<IModule> modules;
        private static ProductContext ctx;


        public static Guid ID
        {
            get
            {
                return new Guid("{EA942538-E68E-4907-9394-035336EE0BA8}");
            }
        }

        public override Guid ProductID
        {
            get { return ID; }
        }

        public override string Name
        {
            get { return CommunityResource.ProductName; }
        }

        public override string ExtendedDescription
        {
            get
            {
                return string.Format(CommunityResource.ProductDescriptionExt,"<br/><a class='links' href='#' onclick='javascript:ImportUsersManager.ShowImportControl(); return false;'>",
                                     "</a>"); 
            }
        }

        public override string Description
        {
            get
            {
                return CommunityResource.ProductDescription;
            }
        }

        public override string StartURL
        {
            get { return "~/products/community/"; }
        }

        public override IModule[] Modules
        {
            get { return modules.ToArray(); }
        }

        public override ProductContext Context
        {
            get { return ctx; }
        }


        public override void Init(ProductContext context)
        {
            modules = ModuleManager.LoadModules(HttpContext.Current.Server.MapPath("~/products/community/modules"));

            var globalHandlers = new List<IGlobalHandler>(0);
            context.GlobalHandler = new GlobalHandlerComposite(globalHandlers);

            foreach (var module in modules)
            {
                if (module.Context.GlobalHandler != null) globalHandlers.Add(module.Context.GlobalHandler);
            }

            context.MasterPageFile = "~/products/community/community.master";
            context.ImageFolder = "images";
            context.ThemesFolderVirtualPath = "~/products/community/app_themes";
            context.DisabledIconFileName = "product_disabled_logo.png";
            context.IconFileName = "product_logo.png";
            context.LargeIconFileName = "product_logolarge.png";

            context.UserActivityControlLoader = new UserActivityControlLoader();
            context.DefaultSortOrder = 40;

            context.SubscriptionManager = new CommunitySubscriptionManager();

            context.WhatsNewHandler = new CommunityWhatsNewHandler();
            context.SpaceUsageStatManager = new CommunitySpaceUsageStatManager();

            context.AdminOpportunities = GetAdminOpportunities;
            context.UserOpportunities = GetUserOpportunities;

            ctx = context;
        }

        private List<string> GetAdminOpportunities()
        {
            return CommunityResource.ProductAdminOpportunities.Split('|').ToList();
        }

        private List<string> GetUserOpportunities()
        {
            return CommunityResource.ProductUserOpportunities.Split('|').ToList();
        }

        public override void Shutdown()
        {
        }
    }
}
