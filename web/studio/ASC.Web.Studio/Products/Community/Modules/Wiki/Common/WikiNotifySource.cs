using System.Web;
using System.Web.Configuration;
using System.Web.Hosting;
using ASC.Core.Notify;
using ASC.Notify.Model;
using ASC.Notify.Patterns;
using ASC.Web.UserControls.Wiki;
using ASC.Web.UserControls.Wiki.Handlers;

namespace ASC.Web.Community.Wiki.Common
{
    public class WikiNotifySource : NotifySource, IDependencyProvider
    {
        private string defPageHref;


        public static WikiNotifySource Instance
        {
            get;
            private set;
        }


        static WikiNotifySource()
        {
            Instance = new WikiNotifySource();
        }

        public WikiNotifySource()
            : base(WikiManager.ModuleId)
        {
            defPageHref = VirtualPathUtility.ToAbsolute(WikiManager.ViewVirtualPath);
        }


        public string GetDefPageHref()
        {
            return defPageHref;
        }


        protected override IActionPatternProvider CreateActionPatternProvider()
        {
            return new XmlActionPatternProvider(
                GetType().Assembly,
                "ASC.Web.Community.Wiki.Common.Patterns.accordings.xml",
                ActionProvider,
                PatternProvider) { GetPatternMethod = new GetPatternCallback(ChoosePattern) };
        }

        protected override IActionProvider CreateActionProvider()
        {
            return new ConstActionProvider(Constants.NewPage, Constants.EditPage);
        }

        protected override IPatternProvider CreatePatternsProvider()
        {
            return new XmlPatternProvider(Patterns.WikiPatternsResource.patterns);
        }


        private IPattern ChoosePattern(INotifyAction action, string senderName, ASC.Notify.Engine.NotifyRequest request)
        {
            if (action == Constants.EditPage)
            {
                var tag = request.Arguments.Find((tv) => tv.Tag.Name == "ChangeType");
                if (tag != null && tag.Value.ToString() == "new wiki page comment")
                {
                    if (senderName == "email.sender") return PatternProvider.GetPattern("3");
                    if (senderName == "messanger.sender") return PatternProvider.GetPattern("3_jabber");
                }
            }
            return null;
        }
    }
}
