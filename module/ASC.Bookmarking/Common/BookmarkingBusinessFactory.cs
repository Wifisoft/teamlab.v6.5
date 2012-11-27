using System;
using System.Web;

namespace ASC.Bookmarking.Common
{

	public static class BookmarkingBusinessFactory
	{
		public static T GetObjectFromSession<T>() where T : class, new()
		{
		    T obj;
			var key = typeof(T).ToString();
            if (HttpContext.Current.Session != null)
            {
                obj = (T) HttpContext.Current.Session[key];
                if (obj == null)
                {
                    obj = Activator.CreateInstance<T>();
                    HttpContext.Current.Session[key] = obj;
                }
            }
            else
            {
                obj = (T)HttpContext.Current.Items[key];
                if (obj == null)
                {
                    obj = Activator.CreateInstance<T>();
                    HttpContext.Current.Items[key] = obj;
                }
            }
		    return obj;
		}
	}
}
