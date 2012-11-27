using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace ASC.Forum
{
    public class ForumPresenterFactory : IPresenterFactory
    {
        public ForumPresenterSettings PresenterSettings { get; set; }
       
        public ForumPresenterFactory(ForumPresenterSettings settings)
        {
            this.PresenterSettings = settings;
        }

        public IPresenter GetPresenter<T>() where T : class
        {
            IPresenter presenter = null;            

            if (typeof(T).Equals(typeof(ISecurityActionView)))
                presenter = new SecurityActionPresenter();
         
            else if (typeof(T).Equals(typeof(INotifierView)))
                presenter = new NotifierPresenter();

            else if (typeof(T).Equals(typeof(ISubscriberView)))
                presenter = new SubscriberPresenter();

            else if (typeof(T).Equals(typeof(ISubscriptionGetcherView)))
                presenter = new SubscriptionGetcherPresenter();
            
            if (presenter != null && PresenterSettings != null)
                presenter.InitPresenter(PresenterSettings);                

            return presenter;
        }    
    }
}
