using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Web;
using System.Xml;
using ASC.Web.Core;
using ASC.Web.Core.Users.Activity;
using ASC.Web.Files.Classes;
using ASC.Web.Files.Resources;
using ASC.Web.Core.WebZones;
using ASC.Files.Core;
using System.Web.UI;

namespace ASC.Web.Files.Configuration
{
    [WebZoneAttribute(WebZoneType.CustomProductList | WebZoneType.StartProductList | WebZoneType.TopNavigationProductList)]
    public class ProductEntryPoint : Product, IModule, IRenderCustomNavigation
    {
        #region Members

        public static readonly Guid ID = new Guid("{E67BE73D-F9AE-4ce1-8FEC-1880CB518CB4}");

        private ProductContext _productContext;

        private ModuleContext _moduleContext;

        #endregion

        public override void Init(ProductContext productContext)
        {
            productContext.ThemesFolderVirtualPath = String.Concat(PathProvider.BaseVirtualPath, "App_Themes");
            productContext.ImageFolder = "images";
            productContext.MasterPageFile = String.Concat(PathProvider.BaseVirtualPath, "Masters/BasicTemplate.Master").ToLower();
            productContext.DisabledIconFileName = "product_disabled_logo.png";
            productContext.IconFileName = "product_logo.png";
            productContext.LargeIconFileName = "product_logolarge.png";
            productContext.DefaultSortOrder = 30;

            productContext.UserActivityControlLoader = new UserActivityControlLoader();
            productContext.UserActivityPublishers = new List<IUserActivityPublisher> {new FilesActivityPublisher()};
            UserActivityManager.AddFilter(new FilesActivityPublisher());
            productContext.SubscriptionManager = new SubscriptionManager();
            productContext.SpaceUsageStatManager = new FilesSpaceUsageStatManager();
            productContext.AdminOpportunities = GetAdminOpportunities;
            productContext.UserOpportunities = GetUserOpportunities;
            productContext.CanNotBeDisabled = true;

            _moduleContext = new ModuleContext {SearchHandler = new SearchHandler(), StatisticProvider = new StatisticProvider()};
            _productContext = productContext;
        }

        private List<string> GetAdminOpportunities()
        {
            return FilesCommonResource.ProductAdminOpportunities.Split('|').ToList();
        }

        private List<string> GetUserOpportunities()
        {
            return FilesCommonResource.ProductUserOpportunities.Split('|').ToList();
        }

        public String GetModuleResource(String ResourceClassTypeName, String ResourseKey)
        {
            if (string.IsNullOrEmpty(ResourseKey)) return string.Empty;
            try
            {
                return (String) Type.GetType(ResourceClassTypeName).GetProperty(ResourseKey, BindingFlags.Static | BindingFlags.Public).GetValue(null, null);
            }
            catch (Exception)
            {
                return String.Empty;
            }
        }

        private static Dictionary<String, XmlDocument> _xslTemplates;

        public void ProcessRequest(HttpContext context)
        {
            if (_xslTemplates == null)
                _xslTemplates = new Dictionary<String, XmlDocument>();

            if (String.IsNullOrEmpty(context.Request["id"]) || String.IsNullOrEmpty(context.Request["name"]))
                return;

            var TemplateName = context.Request["name"];
            var TemplatePath = context.Request["id"];
            var Template = new XmlDocument();
            try
            {
                Template.Load(context.Server.MapPath(String.Format("~{0}{1}.xsl", TemplatePath, TemplateName)));
            }
            catch (Exception)
            {
                return;
            }
            if (Template.GetElementsByTagName("xsl:stylesheet").Count == 0)
                return;

            var Aliases = new Dictionary<String, String>();

            var RegisterAliases = Template.GetElementsByTagName("register");
            while ((RegisterAliases = Template.GetElementsByTagName("register")).Count > 0)
            {
                var RegisterAlias = RegisterAliases.Item(0);
                if (!String.IsNullOrEmpty(RegisterAlias.Attributes["alias"].Value) &&
                    !String.IsNullOrEmpty(RegisterAlias.Attributes["type"].Value))
                    Aliases.Add(RegisterAlias.Attributes["alias"].Value, RegisterAlias.Attributes["type"].Value);
                RegisterAlias.ParentNode.RemoveChild(RegisterAlias);
            }

            var CurrentResources = Template.GetElementsByTagName("resource");

            while ((CurrentResources = Template.GetElementsByTagName("resource")).Count > 0)
            {
                var CurrentResource = CurrentResources.Item(0);
                if (!String.IsNullOrEmpty(CurrentResource.Attributes["name"].Value))
                {
                    var FullName = CurrentResource.Attributes["name"].Value.Split('.');
                    if (FullName.Length == 2 && Aliases.ContainsKey(FullName[0]))
                    {
                        var ResourceValue =
                            Template.CreateTextNode(GetModuleResource(Aliases[FullName[0]], FullName[1]));
                        CurrentResource.ParentNode.InsertBefore(ResourceValue, CurrentResource);
                    }
                }
                CurrentResource.ParentNode.RemoveChild(CurrentResource);
            }

            context.Response.ContentType = "text/xml";
            context.Response.Write(Template.InnerXml);
        }

        public override void Shutdown()
        {

        }

        public override Guid ProductID
        {
            get { return ID; }
        }

        public override string Name
        {
            get { return FilesCommonResource.ProductName; }
        }


        public override string ExtendedDescription
        {
            get { return string.Format(FilesCommonResource.ProductDescriptionEx, "<span style='display:none'>", "</span>"); }
        }

        public override string Description
        {
            get { return FilesCommonResource.ProductDescription; }
        }

        public override string StartURL
        {
            get { return PathProvider.StartURL; }
        }

        ModuleContext IModule.Context
        {
            get { return _moduleContext; }
        }

        public override IModule[] Modules
        {
            get { return new[] {this}; }
        }

        public override ProductContext Context
        {
            get { return _productContext; }
        }

        public string ModuleSysName
        {
            get { return "Files"; }
        }

        #region IRenderCustomNavigation Members

        public string RenderCustomNavigation(Page page)
        {
            if (!Global.EnableShare) return string.Empty;

            var newTags = Global.DaoFactory.GetTagDao().GetTags(ASC.Core.SecurityContext.CurrentAccount.ID, TagType.New);
            var count = newTags.Count();

            var result =
                string.Format(@"
                <li class='top-item-box feed {0}'>
                    <a class='inner-text' href='{1}' title ='{2}'>
                        <span class='inner-label' id='topNavigationNewInShare' style='display:{3}'> {4}</span>
                    </a>        
                </li>
                ",
                              count > 0 ? "has-led" : "",
                              PathProvider.GetFolderUrl(Global.FolderShare, false, null),
                              FilesCommonResource.NavigationPanelTitle,
                              count > 0 ? "inline" : "none",
                              count);

            return result;
        }

        public Control LoadCustomNavigationControl(Page page)
        {
            return null;
        }

        #endregion
    }
}