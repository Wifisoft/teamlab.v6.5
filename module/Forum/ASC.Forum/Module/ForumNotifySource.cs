using ASC.Core.Notify;
using ASC.Notify;
using ASC.Notify.Messages;
using ASC.Notify.Model;
using ASC.Notify.Patterns;

namespace ASC.Forum.Module
{
    internal class ForumNotifyClient
    {
        public static INotifyClient NotifyClient{ get; private set; }

        static ForumNotifyClient()
        {
            NotifyClient = ASC.Core.WorkContext.NotifyContext.NotifyService.RegisterClient(ForumNotifySource.Instance);
        }
    }

    internal class ForumNotifySource: NotifySource, IDependencyProvider     
    {
        private static ForumNotifySource _instance = null;

        public static ForumNotifySource Instance 
        {
            get
            {
                if (_instance == null)
                {
                    lock (typeof(ForumNotifySource))
                    {
                        if (_instance == null)
                            _instance = new ForumNotifySource();
                    }
                }

                return _instance;
            }
        }

        private ForumNotifySource()
            : base(ForumSettings.ModuleID)
        {
            
        }


        protected override IActionPatternProvider CreateActionPatternProvider()
        {            
            return new XmlActionPatternProvider(
                  GetType().Assembly,
                  "ASC.Forum.Module.action_pattern.xml",
                  ActionProvider, 
                  PatternProvider 
              );
        }

        protected override IActionProvider CreateActionProvider()
        {
            return new ConstActionProvider(
                    Constants.NewPostByTag,
                    Constants.NewPostInThread,
                    Constants.NewPostInTopic,
                    Constants.NewTopicInForum                    
                );
        }

        protected override IDependencyProvider CreateDependencyProvider()
        {
            return this;
        }

        protected override IPatternProvider CreatePatternsProvider()
        {
             return new XmlPatternProvider(ASC.Forum.Module.Patterns.forum_patterns);
        }

        #region IDependencyProvider Members

        public override ITagValue[] GetDependencies(INoticeMessage message, string objectID, ITag[] tags)
        {  
            return new ITagValue[0];
        }

        #endregion
    }

}
