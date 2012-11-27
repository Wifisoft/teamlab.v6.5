using System;
using ASC.Notify.Model;
using ASC.Notify.Patterns;
using ASC.Notify.Recipients;
using log4net;

namespace ASC.Notify.Engine
{
    sealed class ProviderResolver
    {
        private static readonly ILog log = log4net.LogManager.GetLogger("ASC.Notify");


        public static T Get<T>(INotifySource source) where T : class
        {
            return GetInternal<T>(source);
        }

        public static T GetEnsure<T>(INotifySource source) where T : class
        {
            var result = GetInternal<T>(source);
            if (result == null)
            {
                throw new NotifyException(String.Format("\"{0}\" not instanced from notify source.", typeof(T).Name));
            }
            return result;
        }

        private static T GetInternal<T>(INotifySource source) where T : class
        {
            T result = null;
            if (source == null) return null;
            if (typeof(T) == typeof(IActionPatternProvider))
                result = (T)WrappedGet(() => source.GetActionPatternProvider());
            if (typeof(T) == typeof(IActionProvider))
                result = (T)WrappedGet(() => source.GetActionProvider());
            if (typeof(T) == typeof(IPatternProvider))
                result = (T)WrappedGet(() => source.GetPatternProvider());
            if (typeof(T) == typeof(IDependencyProvider))
                result = (T)WrappedGet(() => source.GetDependencyProvider());
            if (typeof(T) == typeof(ISubscriptionProvider))
                result = (T)WrappedGet(() => source.GetSubscriptionProvider());
            if (typeof(T) == typeof(IRecipientProvider))
                result = (T)WrappedGet(() => source.GetRecipientsProvider());
            if (typeof(T) == typeof(ISubscriptionSource))
                result = (T)WrappedGet(() => source.GetSubscriptionSource());
            return result;
        }

        private static T WrappedGet<T>(Func<T> get) where T : class
        {
            try
            {
                return get();
            }
            catch(Exception e)
            {
                log.Warn(e);
                return default(T);
            }
        }
    }
}