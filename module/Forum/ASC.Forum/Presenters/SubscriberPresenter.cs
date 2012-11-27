using System;
using System.Collections.Generic;
using ASC.Forum.Module;
using ASC.Notify.Model;
using ASC.Notify.Recipients;

namespace ASC.Forum
{
    internal class SubscriberPresenter : PresenterTemplate<ISubscriberView>
    {
        public static void UnsubscribeAllOnTopic(int topicID)
        {
            ForumNotifySource.Instance.GetSubscriptionProvider().UnSubscribe(Constants.NewPostInTopic, topicID.ToString());
        }

        public static void UnsubscribeAllOnThread(int threadID)
        {
            ForumNotifySource.Instance.GetSubscriptionProvider().UnSubscribe(Constants.NewPostInThread, threadID.ToString());
        }


        protected override void RegisterView()
        {
            _view.Subscribe += new EventHandler<SubscribeEventArgs>(SubscribeHandler);
            _view.UnSubscribe += new EventHandler<SubscribeEventArgs>(UnSubscribeHandler);
            _view.UnSubscribeForSubscriptionType += new EventHandler<SubscribeEventArgs>(UnSubscribeForSubscriptionTypeHandler);
            _view.GetSubscriptionState += new EventHandler<SubscribeEventArgs>(GetSubscriptionStateHandler);
            
        }
        
        private void UnSubscribeForSubscriptionTypeHandler(object sender, SubscribeEventArgs e)
        {
            var recipient = (IDirectRecipient)ForumNotifySource.Instance.GetRecipientsProvider().GetRecipient(e.UserID.ToString());
            if(recipient!=null)
                ForumNotifySource.Instance.GetSubscriptionProvider().UnSubscribe(e.NotifyAction, recipient);
        }

        void UnSubscribeHandler(object sender, SubscribeEventArgs e)
        {
            var recipient = (IDirectRecipient)ForumNotifySource.Instance.GetRecipientsProvider().GetRecipient(e.UserID.ToString());
            if(recipient!=null)
                ForumNotifySource.Instance.GetSubscriptionProvider().UnSubscribe(e.NotifyAction, e.ObjectID, recipient);
        }

        void GetSubscriptionStateHandler(object sender, SubscribeEventArgs e)
        {
            var recipient = (IDirectRecipient)ForumNotifySource.Instance.GetRecipientsProvider().GetRecipient(e.UserID.ToString());
            if (recipient == null)
                return;

            ISubscriptionProvider subscriptionProvider = ForumNotifySource.Instance.GetSubscriptionProvider();
            List<string> objectIDs = new List<string>(subscriptionProvider.GetSubscriptions(e.NotifyAction,recipient));

            if (e.ObjectID == null && objectIDs.Count == 1 && objectIDs[0] == null)
            {
                _view.IsSubscribe = true;
                return;
            }

            _view.IsSubscribe = (objectIDs.Find(id => String.Compare(id, e.ObjectID, true) == 0) != null);

        }

        private void SubscribeHandler(object sender, SubscribeEventArgs e)
        {            
            var recipient = (IDirectRecipient)ForumNotifySource.Instance.GetRecipientsProvider().GetRecipient(e.UserID.ToString());
            if(recipient!=null)
                ForumNotifySource.Instance.GetSubscriptionProvider().Subscribe(e.NotifyAction, e.ObjectID, recipient);
                
        }
    }
}
