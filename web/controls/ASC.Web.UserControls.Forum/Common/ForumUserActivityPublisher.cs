using System;
using ASC.Core;
using ASC.Forum;
using ASC.Web.Core.Users.Activity;
using ASC.Web.Studio.Utility;

namespace ASC.Web.UserControls.Forum.Common
{  
    public abstract class ForumUserActivityPublisher : BaseUserActivityPublisher
    {
        private Settings _settings;

        public void InitSettings(Settings settings)
        {
            _settings = settings;
            settings.ActivityPublisher = this;
        }

        private class ForumUserActivity : UserActivity
        {
            public ForumUserActivity()
            {
                this.Date = ASC.Core.Tenants.TenantUtil.DateTimeNow();
                this.UserID = SecurityContext.CurrentAccount.ID;
                this.TenantID = TenantProvider.CurrentTenantID;
            }
        }    

        internal void TopicClose(Topic topic)
        {   
            UserActivityPublisher.Publish(this.GetType(), new ForumUserActivity()
            {
                ModuleID = _settings.ModuleID,
                ProductID = _settings.ProductID,
                URL = _settings.LinkProvider.PostList(topic.ID),
                ActionType = UserActivityConstants.ActivityActionType,
                ContentID = topic.ID.ToString(),                
                BusinessValue = UserActivityConstants.NormalActivity,
                ActionText = (topic.Closed ? Resources.ForumUCResource.TopicCloseActionText : Resources.ForumUCResource.TopicOpenActionText),
                Title = topic.Title,
                ContainerID = topic.ThreadID.ToString()
            });
        }

        internal void TopicSticky(Topic topic)
        {
            UserActivityPublisher.Publish(this.GetType(), new ForumUserActivity()
            {
                ModuleID = _settings.ModuleID,
                ProductID = _settings.ProductID,
                URL = _settings.LinkProvider.PostList(topic.ID),
                ActionType = UserActivityConstants.ActivityActionType,
                ContentID = topic.ID.ToString(),
                BusinessValue = UserActivityConstants.NormalActivity,
                ActionText = (topic.Sticky ? Resources.ForumUCResource.TopicStickyActionText : Resources.ForumUCResource.TopicClearStickyActionText),
                Title = topic.Title,
                ContainerID = topic.ThreadID.ToString()
            });
        }
        internal void DeleteTopic(int threadID, string topicName, string threadName)
        {
            UserActivityPublisher.Publish(this.GetType(), new ForumUserActivity()
                   {
                       ModuleID = _settings.ModuleID,
                       ProductID = _settings.ProductID,
                       URL = _settings.LinkProvider.TopicList(threadID),
                       ActionType = UserActivityConstants.ActivityActionType,
                       ContentID = threadID.ToString(),
                       BusinessValue = UserActivityConstants.NormalActivity,
                       ActionText = string.Format(Resources.ForumUCResource.TopicRemoveActionText, topicName),
                       Title = threadName,
                       ContainerID = threadID.ToString()
                   });
        }

        internal void Vote(string questionName, int topicID)
        {

            UserActivityPublisher.Publish(this.GetType(), new ForumUserActivity()
            {
                ModuleID = _settings.ModuleID,
                ProductID = _settings.ProductID,
                URL = _settings.LinkProvider.PostList(topicID),
                ActionType = UserActivityConstants.ActivityActionType,
                ContentID = topicID.ToString(),
                BusinessValue = UserActivityConstants.SmallContent,
                ActionText = Resources.ForumUCResource.PollVoteActionText,
                Title = questionName
            });
        }

        internal void EditTopic(Topic topic)
        {
            UserActivityPublisher.Publish(this.GetType(), new ForumUserActivity()
                      {
                          ModuleID = _settings.ModuleID,
                          ProductID = _settings.ProductID,
                          URL = _settings.LinkProvider.PostList(topic.ID),
                          ActionType = UserActivityConstants.ActivityActionType,
                          ContentID = topic.ID.ToString(),
                          BusinessValue = UserActivityConstants.SmallContent,
                          ActionText = Resources.ForumUCResource.TopicEditActionText,
                          Title = topic.Title,
                          ContainerID = topic.ThreadID.ToString()
                      });
        }

        internal void NewPost(Post post, string topicTitle, int threadID, string url)
        {
            UserActivityPublisher.Publish(this.GetType(), new ForumUserActivity()
            {
                ModuleID = _settings.ModuleID,
                ProductID = _settings.ProductID,
                URL = url,
                ActionType = UserActivityConstants.ActivityActionType,
                ContentID = post.ID.ToString(),
                BusinessValue = UserActivityConstants.NormalActivity,
                ActionText = Resources.ForumUCResource.PostActionText,
                Title = topicTitle,
                ContainerID = threadID.ToString()
            });
        }

        internal void NewTopic(Topic topic)
        {
            UserActivityPublisher.Publish(this.GetType(), new ForumUserActivity()
            {
                ModuleID = _settings.ModuleID,
                ProductID = _settings.ProductID,
                URL = _settings.LinkProvider.PostList(topic.ID),
                ActionType = UserActivityConstants.ContentActionType,
                ContentID = topic.ID.ToString(),
                BusinessValue = UserActivityConstants.SmallContent,                
                ActionText = (topic.Type == TopicType.Informational ? Resources.ForumUCResource.TopicActionText : Resources.ForumUCResource.PollActionText),
                Title = topic.Title,
                ContainerID = topic.ThreadID.ToString()
            });
        }
    }
}
