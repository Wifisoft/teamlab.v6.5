using ASC.Core.Notify;
using ASC.Notify.Model;
using ASC.Notify.Patterns;

namespace ASC.PhotoManager.Service
{
    public class PhotoManagerNotifySource : NotifySource, IDependencyProvider
    {
        public PhotoManagerNotifySource()
            : base(PhotoConst.ModuleId)
        {

        }

        protected override IActionPatternProvider CreateActionPatternProvider()
        {
            return new XmlActionPatternProvider(
                    GetType().Assembly,
                    "ASC.PhotoManager.Service.accordings.xml",
                    ActionProvider,
                    PatternProvider) { GetPatternMethod = ChoosePattern };
        }

        private IPattern ChoosePattern(INotifyAction action, string senderName, ASC.Notify.Engine.NotifyRequest request)
        {
            if (action == PhotoConst.NewEventComment)
            {
                if (request.Arguments.Exists((tv) => tv.Tag.Name == "FEED_TYPE"))
                {
                    return ActionPatternProvider.GetPattern(PhotoConst.NewPhotoUploaded, senderName) ?? ActionPatternProvider.GetPattern(PhotoConst.NewPhotoUploaded);
                }
            }
            return null;
        }

        protected override IActionProvider CreateActionProvider()
        {
            return new ConstActionProvider(PhotoConst.NewPhotoUploaded, PhotoConst.NewEventComment);
        }
        protected override IPatternProvider CreatePatternsProvider()
        {
            return new XmlPatternProvider(Resources.PhotoPatternResource.patterns);
        }
    }
}
