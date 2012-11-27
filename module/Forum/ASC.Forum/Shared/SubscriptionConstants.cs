using ASC.Forum.Module;
using ASC.Notify.Model;
using ASC.Notify;

namespace ASC.Forum
{
    public static class SubscriptionConstants
    {
        public static INotifyAction NewPostInTopic { get { return Constants.NewPostInTopic; } }

        public static INotifyAction NewPostInThread { get { return Constants.NewPostInThread; } }

        public static INotifyAction NewPostByTag { get { return Constants.NewPostByTag; } }

        public static INotifyAction NewTopicInForum { get { return Constants.NewTopicInForum; } }

        public static ISubscriptionProvider SubscriptionProvider { get { return ForumNotifySource.Instance.GetSubscriptionProvider(); } }

        public static INotifyClient NotifyClient { get { return ForumNotifyClient.NotifyClient; } }

        public static string SyncName { get { return "asc_forum"; } }

        
        
    }
}
