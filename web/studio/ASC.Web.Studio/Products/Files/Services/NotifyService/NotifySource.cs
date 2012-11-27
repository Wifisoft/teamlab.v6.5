using System;
using ASC.Notify.Model;
using ASC.Notify.Patterns;
using NotifySourceBase = ASC.Core.Notify.NotifySource;

namespace ASC.Web.Files.Services.NotifyService
{
    public class NotifySource : NotifySourceBase
    {
        private static NotifySource instance = new NotifySource();

        public static NotifySource Instance
        {
            get { return instance; }
        }

        public NotifySource() : base(new Guid("6FE286A4-479E-4c25-A8D9-0156E332B0C0"))
        {
        }

        protected override IActionProvider CreateActionProvider()
        {
            return new ConstActionProvider(
                NotifyConstants.Event_ShareDocument,
                NotifyConstants.Event_UpdateDocument);
        }

        protected override IActionPatternProvider CreateActionPatternProvider()
        {
            return new XmlActionPatternProvider(
                GetType().Assembly,
                "ASC.Web.Files.Services.NotifyService.action_pattern.xml",
                ActionProvider,
                PatternProvider) {GetPatternMethod = ChoosePattern};
        }

        private IPattern ChoosePattern(INotifyAction action, string senderName, Notify.Engine.NotifyRequest request)
        {
            if (action == NotifyConstants.Event_ShareDocument
                || action == NotifyConstants.Event_UpdateDocument)
                return ActionPatternProvider.GetPattern(action, senderName);

            return null;
        }

        protected override IPatternProvider CreatePatternsProvider()
        {
            return new XmlPatternProvider(FilesPatternResource.patterns);
        }
    }
}