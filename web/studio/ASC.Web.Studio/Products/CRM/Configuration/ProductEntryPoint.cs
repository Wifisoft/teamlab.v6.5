#region Import

using System;
using System.Collections.Generic;
using System.Web;
using ASC.CRM.Core;
using ASC.Web.Core;
using ASC.Web.Core.ModuleManagement;
using ASC.Web.Core.Users.Activity;
using ASC.Web.CRM.Resources;
using ASC.Web.Core.Utility;
using ASC.Web.Files.Api;
using System.Linq;
using ASC.Web.Studio.Core.SearchHandlers;

#endregion

namespace ASC.Web.CRM.Configuration
{
  
    public class ProductEntryPoint : Product
    {
       
        #region Members

        public static readonly Guid ID = new Guid("{6743007C-6F95-4d20-8C88-A8601CE5E76D}");
      
        private ProductContext _productContext;
       
        #endregion

        public override void Init(ProductContext productContext)
        {
            productContext.ThemesFolderVirtualPath = String.Concat(PathProvider.BaseVirtualPath, "App_Themes");
            productContext.ImageFolder = "images";
            productContext.MasterPageFile = String.Concat(PathProvider.BaseVirtualPath, "Masters/BasicTemplate.Master");
            productContext.DisabledIconFileName = "product_disabled_logo.png";
            productContext.IconFileName = "product_logo.png";
            productContext.LargeIconFileName = "product_logolarge.png";
            productContext.UserActivityPublishers = new List<IUserActivityPublisher>() { new TimeLinePublisher() };
            productContext.DefaultSortOrder = 20;
            productContext.SubscriptionManager = new ProductSubscriptionManager();
            productContext.SpaceUsageStatManager = new CRMSpaceUsageStatManager();
            productContext.AdminOpportunities = GetAdminOpportunities;
            productContext.UserOpportunities = GetUserOpportunities;
           
            if (!FilesIntegration.IsRegisteredFileSecurityProvider("crm", "crm_common"))
                FilesIntegration.RegisterFileSecurityProvider("crm", "crm_common", new FileSecurityProvider());
            
            _productContext = productContext;

            SearchHandlerManager.Registry(new SearchHandler());
         
        }

        private List<string> GetAdminOpportunities()
        {
            return CRMCommonResource.ProductAdminOpportunities.Split('|').ToList();
        }

        private List<string> GetUserOpportunities()
        {
            return CRMCommonResource.ProductUserOpportunities.Split('|').ToList();
        }

        public override void Shutdown()
        {

        }

        public override Guid ProductID
        {
            get { return ID; }
        }

        public override IModule[] Modules
        {
            get { return new List<IModule>().ToArray(); }
        }

        public override string Name
        {
            get { return CRMCommonResource.ProductName; }
        }

        public override string ExtendedDescription
        {
            get
            {
                return string.Format(CRMCommonResource.ProductDescriptionEx,
                                     string.Format("<br/><a class='links'  href='{0}?action=import'>",
                                                   VirtualPathUtility.ToAbsolute("~/products/crm/default.aspx")),
                                     "</a>");
            }
        }

        public override string Description
        {
            get
            {
                return CRMCommonResource.ProductDescription;
            }
        }

        public override string StartURL
        {
            get { return PathProvider.StartURL(); }
        }

       
        public override ProductContext Context
        {
            get { return _productContext; }
        }

        public string ModuleSysName
        {
            get;
            set;
        }
    }
}