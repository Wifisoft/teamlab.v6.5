using System;
using System.Collections.Generic;
using ASC.Core;
using ASC.Web.Community.News;
using ASC.Web.Community.News.Code;
using ASC.Web.Community.News.Code.Module;
using ASC.Web.Community.News.Resources;
using ASC.Web.Core;
using ASC.Web.Core.ModuleManagement;
using ASC.Web.Core.Users.Activity;
using ASC.Web.Community.Product;

[assembly: Module(typeof(NewsModule))]

namespace ASC.Web.Community.News
{
    public class NewsModule : Module
    {
        public override Guid ID
        {
            get { return NewsConst.ModuleId; }
        }

        public override string Name
        {
            get { return NewsResource.AddonName; }
        }

        public override string Description
        {
            get { return NewsResource.AddonDescriptionResourceKey; }
        }

        public override string StartURL
        {
            get { return "~/products/community/modules/news/"; }
        }

        public override IEnumerable<Widget> Widgets
        {
            get
            {
                var w1 = new Widget
                             {
                                 ID = new Guid("DF8F2054-FB17-4f65-ABF4-EFA0B7624DBC"),
                                 Name = NewsResource.NewsWidgetName,
                                 Description = NewsResource.NewsWidgetDescription,
                                 StartURL = "~/products/community/modules/news/",
                                 SettingsType = typeof (WidgetSettingsProvider),
                                 WidgetType = typeof (FeedWidget),
                                 Context = new WebItemContext {IconFileName = "32x_news.png"}
                             };
                var w2 = new Widget
                             {
                                 ID = new Guid("442BFA43-E8A2-4b37-A2D5-5CD51FB50CBA"),
                                 Name = NewsResource.PollWidgetName,
                                 Description = NewsResource.PollWidgetDescription,
                                 StartURL = "~/products/community/modules/news/?type=poll",
                                 WidgetType = typeof (PollWidget),
                                 Context = new WebItemContext {IconFileName = "32x_poll.png"}
                             };
                return new[] {w1, w2};
            }
        }


        public NewsModule()
        {
            Context = new ModuleContext
                          {
                              ThemesFolderVirtualPath = "~/products/community/modules/news/app_themes",
                              ImageFolder = "newsimages",
                              SmallIconFileName = "newslogo.png",
                              IconFileName = "32x_news.png",
                              SubscriptionManager = new SubscriptionManager(),
                              StatisticProvider = new StatisticProvider(),
                              UserActivityPublishers = new List<IUserActivityPublisher> {new ActivityPublisher()},
                              GetCreateContentPageAbsoluteUrl = () => CommunitySecurity.CheckPermissions(NewsConst.Action_Add) ? FeedUrls.EditNewsUrl : null,
                              SearchHandler = new SearchHandler(),
                          };
        }
    }
}