using System;
using System.Collections;

namespace ASC.Web.Core.Users.Activity
{
    public static class UserActivityPublisher
    {
        private static Hashtable _publishers = Hashtable.Synchronized(new Hashtable());

        internal static void Registry(BaseUserActivityPublisher publisher)
        {
            lock (_publishers)
            {
                if (!_publishers.ContainsKey(publisher.GetType()))
                    _publishers.Add(publisher.GetType(), publisher);
            }
        }

        internal static void UnRegistry(BaseUserActivityPublisher publisher)
        {
            lock (_publishers)
            {
                if (_publishers.ContainsKey(publisher.GetType()))
                    _publishers.Remove(publisher.GetType());
            }
        }

        public static void Publish<T>(UserActivity userActivity) where T : BaseUserActivityPublisher
        { 
            Type publisherType = typeof(T);
            if (_publishers.Contains(publisherType) && _publishers[publisherType]!=null)
                (_publishers[publisherType] as BaseUserActivityPublisher).Publish(userActivity);
        }

        public static void Publish(Type publisherType, UserActivity userActivity)
        {   
            if (_publishers.Contains(publisherType) && _publishers[publisherType] != null)
                (_publishers[publisherType] as BaseUserActivityPublisher).Publish(userActivity);
        }
    }

    public abstract class BaseUserActivityPublisher : IUserActivityPublisher, IDisposable
    {
        public BaseUserActivityPublisher()
        {
            UserActivityPublisher.Registry(this);
        }

        public virtual void Publish(UserActivity userActivity)
        {
            OnDoUserActivity(new UserActivityEventArgs() { UserActivity = userActivity });
        }

        #region IUserActivityPublisher Members

        private event EventHandler<UserActivityEventArgs> _doUserActivity = null;

        private void OnDoUserActivity(UserActivityEventArgs e)
        {
            EventHandler<UserActivityEventArgs> handler = _doUserActivity;
            if (handler != null) handler(this, e);
        }

        public virtual event EventHandler<UserActivityEventArgs> DoUserActivity
        {
            add
            {
                if (_doUserActivity==null)
                    _doUserActivity += value;
                else
                {
                    throw new ArgumentException("Only one invocator allowed!");
                }
            }
            remove
            {
                _doUserActivity -= value;
            }
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            UserActivityPublisher.UnRegistry(this);
        }

        #endregion
    }
}
