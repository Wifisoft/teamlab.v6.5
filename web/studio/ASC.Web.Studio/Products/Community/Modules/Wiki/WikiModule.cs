using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ASC.Web.Core.ModuleManagement;
using ASC.Web.Community.Wiki;
using ASC.Web.Core;
using ASC.Web.Community.Wiki.Common;
using ASC.Web.Core.Users.Activity;
using ASC.Web.UserControls.Wiki;
using ASC.Web.UserControls.Wiki.Resources;

[assembly: Module(typeof(WikiModule))]

namespace ASC.Web.Community.Wiki
{
    public class WikiModule : Module
    {
        public override Guid ID
        {
            get { return WikiManager.ModuleId; }
        }

        public override string Name
        {
            get { return WikiResource.ModuleName; }
        }

        public override string Description
        {
            get { return WikiResource.ModuleDescription; }
        }

        public override string StartURL
        {
            get { return "~/products/community/modules/wiki/"; }
        }

        public override IEnumerable<Widget> Widgets
        {
            get
            {
                var w = new Widget
                {
                    ID = new Guid("A9B76803-48C1-4526-8C47-6EA602CDB784"),
                    Name = WikiResource.WikiWidgetTitle,
                    Description = WikiResource.WikiWidgetDescription,
                    StartURL = "~/products/community/modules/wiki/",
                    SettingsType = typeof(WikiWidgetSettingsProvider),
                    WidgetType = typeof(WikiWidget),
                    Context = new WebItemContext { IconFileName = "WikiLogo32.png" }
                };
                return new[] { w };
            }
        }

        public static string GetCreateContentPageUrl()
        {
            return VirtualPathUtility.ToAbsolute(WikiManager.BaseVirtualPath + "/default.aspx") + "?action=new";
        }


        public WikiModule()
        {
            Context = new ModuleContext
            {
                DefaultSortOrder = 5,
                ThemesFolderVirtualPath = "~/Products/Community/Modules/Wiki/App_Themes",
                ImageFolder = "wikiimages",
                SmallIconFileName = "WikiLogo16.png",
                IconFileName = "WikiLogo32.png",
                SubscriptionManager = new WikiSubscriptionManager(),
                StatisticProvider = new WikiStatisticProvider(),
                UserActivityPublishers = new List<IUserActivityPublisher>() { new WikiActivityPublisher() },
                GetCreateContentPageAbsoluteUrl = GetCreateContentPageUrl,
                SearchHandler = new WikiSearchHandler(),
            };
        }
    }
}
