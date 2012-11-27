using ASC.Core.Notify;
using ASC.Notify.Model;
using ASC.Notify.Patterns;


namespace ASC.Blogs.Core.Service
{
    public class BlogNotifySource
        : NotifySource,
        IDependencyProvider
    {
        public BlogNotifySource()
            : base(Constants.ModuleId)
        {            
        }

        protected override IActionPatternProvider CreateActionPatternProvider()
        {
            return new XmlActionPatternProvider(
                    GetType().Assembly,
                    "ASC.Blogs.Core.Service.accordings.xml",
                    ActionProvider,
                    PatternProvider
                );
        }

        protected override IActionProvider CreateActionProvider()
        {
            return new ConstActionProvider(
                    Constants.NewPost,
                    Constants.NewPostByAuthor,
                    Constants.NewComment
                );
        }
        protected override IPatternProvider CreatePatternsProvider()
        {
            return new XmlPatternProvider(BlogPatternsResource.patterns);
        }
    }
}
