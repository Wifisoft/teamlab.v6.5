using System;
using System.Collections.Generic;
using System.Web;
using ASC.Blogs.Core;
using ASC.Blogs.Core.Domain;
using ASC.Core;
using ASC.Notify.Model;
using ASC.Notify.Recipients;
using ASC.Web.Core.Subscriptions;
using ASC.Web.Core.Users;
using SubscrType = ASC.Web.Core.Subscriptions.SubscriptionType;

namespace ASC.Web.Community.Blogs
{
    public class BlogsSubscriptionManager : ISubscriptionManager
    {
        private Guid _blogSubscriptionTypeID = new Guid("{4954EB99-1402-46e6-80B6-8734ABE9B8C2}");
        private Guid _blogPersSubscriptionTypeID = new Guid("{8D5AAC98-076A-44be-A718-508124BCE107}");
        private Guid _commentSubscriptionTypeID = new Guid("{615508B1-5FF9-449d-B6A9-831498EE3A93}");

        private List<SubscriptionObject> GetSubscriptionObjectsByType(Guid productID, Guid moduleID, Guid typeID)
        {
            var _engine = BasePage.GetEngine();

            List<SubscriptionObject> subscriptionObjects = new List<SubscriptionObject>();
            ISubscriptionProvider subscriptionProvider = _engine.NotifySource.GetSubscriptionProvider();

            if (typeID.Equals(_blogSubscriptionTypeID))
            {
                List<string> list = new List<string>(
                subscriptionProvider.GetSubscriptions(
                ASC.Blogs.Core.Constants.NewPost,
                _engine.NotifySource.GetRecipientsProvider().GetRecipient(SecurityContext.CurrentAccount.ID.ToString()))
                );

                if (list.Contains(null))
                {
                    subscriptionObjects.Add(new SubscriptionObject()
                    {
                        ID = new Guid(Constants._NewBlogSubscribeCategory).ToString(),
                        Name = ASC.Blogs.Core.Resources.BlogsResource.SubscribeOnNewPostTitle,
                        URL = string.Empty,
                        SubscriptionType = GetSubscriptionTypes().Find(st => st.ID.Equals(_blogSubscriptionTypeID))
                    });
                }
            }

            else if (typeID.Equals(_blogPersSubscriptionTypeID))
            {
                List<string> list = new List<string>(
            subscriptionProvider.GetSubscriptions(
                ASC.Blogs.Core.Constants.NewPostByAuthor,
                _engine.NotifySource.GetRecipientsProvider().GetRecipient(SecurityContext.CurrentAccount.ID.ToString()))
                );
                if (list.Count > 0)
                {
                    foreach (string id in list)
                    {
                        if (!string.IsNullOrEmpty(id))
                        {
                            subscriptionObjects.Add(new SubscriptionObject()
                            {
                                ID = id,
                                Name = DisplayUserSettings.GetFullUserName(new Guid(id)),
                                URL = VirtualPathUtility.ToAbsolute("~/products/community/modules/blogs/") + "?userid=" + id,
                                SubscriptionType = GetSubscriptionTypes().Find(st => st.ID.Equals(_blogPersSubscriptionTypeID))
                            });
                        }
                    }
                }
            }

            else if (typeID.Equals(_commentSubscriptionTypeID))
            {
                List<string> list = new List<string>(
           subscriptionProvider.GetSubscriptions(
               ASC.Blogs.Core.Constants.NewComment,
               _engine.NotifySource.GetRecipientsProvider().GetRecipient(SecurityContext.CurrentAccount.ID.ToString()))
               );

                if (list.Count > 0)
                {
                    IList<Post> postList = _engine.SelectPostsInfo(list.ConvertAll(s => new Guid(s)));

                    foreach (Post post in postList)
                    {
                        if (post != null)
                        {
                            subscriptionObjects.Add(new SubscriptionObject()
                            {
                                ID = post.ID.ToString(),
                                Name = post.Title,
                                URL = VirtualPathUtility.ToAbsolute("~/products/community/modules/blogs/viewblog.aspx") + "?blogid=" + post.ID.ToString(),
                                SubscriptionType = GetSubscriptionTypes().Find(st => st.ID.Equals(_commentSubscriptionTypeID))
                            });
                        }
                    }
                }
            }


            return subscriptionObjects;
        }

        private bool IsEmptySubscriptionType(Guid productID, Guid moduleID, Guid typeID)
        {
            var type = GetSubscriptionTypes().Find(t => t.ID.Equals(typeID));

            var objIDs = SubscriptionProvider.GetSubscriptions(type.NotifyAction, new DirectRecipient(SecurityContext.CurrentAccount.ID.ToString(), ""));
            if (objIDs != null && objIDs.Length > 0)
                return false;

            return true;
        }


        public BlogsSubscriptionManager()
        {
        }

        private INotifyAction GetNotifyActionBySubscriptionType(ASC.Blogs.Core.SubscriptionType subscriptionType)
        {
            switch (subscriptionType)
            {
                case ASC.Blogs.Core.SubscriptionType.NewBlog:
                    return ASC.Blogs.Core.Constants.NewPost;

                case ASC.Blogs.Core.SubscriptionType.NewBlogPers:
                    return ASC.Blogs.Core.Constants.NewPostByAuthor;

                case ASC.Blogs.Core.SubscriptionType.NewComment:
                    return ASC.Blogs.Core.Constants.NewComment;

            }
            return null;
        }

        private ASC.Blogs.Core.SubscriptionType GetBlogsSubscriptionType(Guid subscriptionTypeID)
        {
            if (subscriptionTypeID.Equals(_blogSubscriptionTypeID))
                return ASC.Blogs.Core.SubscriptionType.NewBlog;

            else if (subscriptionTypeID.Equals(_blogPersSubscriptionTypeID))
                return ASC.Blogs.Core.SubscriptionType.NewBlogPers;

            else if (subscriptionTypeID.Equals(_commentSubscriptionTypeID))
                return ASC.Blogs.Core.SubscriptionType.NewComment;

            return ASC.Blogs.Core.SubscriptionType.NewBlog;
        }

        #region ISubscriptionManager Members

        public System.Collections.Generic.List<SubscriptionObject> GetSubscriptionObjects()
        {
            var _engine = BasePage.GetEngine();

            List<SubscriptionObject> subscriptionObjects = new List<SubscriptionObject>();
            ISubscriptionProvider subscriptionProvider = _engine.NotifySource.GetSubscriptionProvider();

            #region new blogs
            List<string> list = new List<string>(
                subscriptionProvider.GetSubscriptions(
                    ASC.Blogs.Core.Constants.NewPost,
                    _engine.NotifySource.GetRecipientsProvider().GetRecipient(SecurityContext.CurrentAccount.ID.ToString()))
                    );

            if (list.Contains(null))
            {
                subscriptionObjects.Add(new SubscriptionObject()
                {
                    ID = new Guid(Constants._NewBlogSubscribeCategory).ToString(),
                    Name = ASC.Blogs.Core.Resources.BlogsResource.SubscribeOnNewPostTitle,
                    URL = string.Empty,
                    SubscriptionType = GetSubscriptionTypes().Find(st => st.ID.Equals(_blogSubscriptionTypeID))
                });
            }
            #endregion

            #region personal posts
            list = new List<string>(
                subscriptionProvider.GetSubscriptions(
                    ASC.Blogs.Core.Constants.NewPostByAuthor,
                    _engine.NotifySource.GetRecipientsProvider().GetRecipient(SecurityContext.CurrentAccount.ID.ToString()))
                    );
            if (list.Count > 0)
            {
                foreach (string id in list)
                {
                    if (!string.IsNullOrEmpty(id))
                    {
                        subscriptionObjects.Add(new SubscriptionObject()
                        {
                            ID = id,
                            Name = DisplayUserSettings.GetFullUserName(new Guid(id)),
                            URL = VirtualPathUtility.ToAbsolute("~/products/community/modules/blogs/") + "?userid=" + id,
                            SubscriptionType = GetSubscriptionTypes().Find(st => st.ID.Equals(_blogPersSubscriptionTypeID))
                        });
                    }
                }
            }
            #endregion

            #region new comments

            list = new List<string>(
               subscriptionProvider.GetSubscriptions(
                   ASC.Blogs.Core.Constants.NewComment,
                   _engine.NotifySource.GetRecipientsProvider().GetRecipient(SecurityContext.CurrentAccount.ID.ToString()))
                   );

            if (list.Count > 0)
            {
                IList<Post> postList = _engine.SelectPostsInfo(list.ConvertAll(s => new Guid(s)));

                foreach (Post post in postList)
                {
                    if (post != null)
                    {
                        subscriptionObjects.Add(new SubscriptionObject()
                            {
                                ID = post.ID.ToString(),
                                Name = post.Title,
                                URL = VirtualPathUtility.ToAbsolute("~/products/community/modules/blogs/viewblog.aspx") + "?blogid=" + post.ID.ToString(),
                                SubscriptionType = GetSubscriptionTypes().Find(st => st.ID.Equals(_commentSubscriptionTypeID))
                            });
                    }
                }
            }

            #endregion

            return subscriptionObjects;
        }

        public List<ASC.Web.Core.Subscriptions.SubscriptionType> GetSubscriptionTypes()
        {

            var subscriptionTypes = new List<SubscrType>();


            subscriptionTypes.Add(new SubscrType()
            {
                ID = _blogSubscriptionTypeID,
                Name = ASC.Blogs.Core.Resources.BlogsResource.SubscribeOnNewPostTitle,
                NotifyAction = ASC.Blogs.Core.Constants.NewPost,
                Single = true,
                IsEmptySubscriptionType = new IsEmptySubscriptionTypeDelegate(IsEmptySubscriptionType)
            });

            subscriptionTypes.Add(new SubscrType()
            {
                ID = _blogPersSubscriptionTypeID,
                Name = ASC.Blogs.Core.Resources.BlogsResource.SubscribeOnAuthorTitle,
                NotifyAction = ASC.Blogs.Core.Constants.NewPostByAuthor,
                GetSubscriptionObjects = new GetSubscriptionObjectsDelegate(GetSubscriptionObjectsByType),
                IsEmptySubscriptionType = new IsEmptySubscriptionTypeDelegate(IsEmptySubscriptionType)
            });

            subscriptionTypes.Add(new SubscrType()
            {
                ID = _commentSubscriptionTypeID,
                Name = ASC.Blogs.Core.Resources.BlogsResource.SubscribeOnNewCommentsTitle,
                NotifyAction = ASC.Blogs.Core.Constants.NewComment,
                GetSubscriptionObjects = new GetSubscriptionObjectsDelegate(GetSubscriptionObjectsByType),
                IsEmptySubscriptionType = new IsEmptySubscriptionTypeDelegate(IsEmptySubscriptionType)
            });


            return subscriptionTypes;

        }

        public void UnsubscribeForObject(string subscriptionObjectID, Guid subscriptionTypeID)
        {
            var _engine = BasePage.GetEngine();

            ISubscriptionProvider subscriptionProvider = _engine.NotifySource.GetSubscriptionProvider();

            subscriptionProvider.UnSubscribe(
                 GetNotifyActionBySubscriptionType(
                 GetBlogsSubscriptionType(subscriptionTypeID)),                             
                 (subscriptionTypeID == _blogSubscriptionTypeID ? null : subscriptionObjectID),
                 _engine.NotifySource.GetRecipientsProvider().GetRecipient(SecurityContext.CurrentAccount.ID.ToString())
            );

        }

        public void UnsubscribeForType(Guid subscriptionTypeID)
        {
        }



        #endregion

        #region ISubscriptionManager Members


        public ISubscriptionProvider SubscriptionProvider
        {
            get { return BasePage.GetEngine().NotifySource.GetSubscriptionProvider(); }
        }

        #endregion
    }
}
