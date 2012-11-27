using System.Web;
using ASC.Core.Notify;
using ASC.Notify.Model;
using ASC.Notify.Patterns;

namespace ASC.Web.UserControls.Wiki
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
                "ASC.Web.UserControls.Wiki.Code.Patterns.accordings.xml",
                ActionProvider,
                PatternProvider) { GetPatternMethod = ChoosePattern };
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
