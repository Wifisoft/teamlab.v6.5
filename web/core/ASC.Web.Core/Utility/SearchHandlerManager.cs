using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ASC.Web.Core.ModuleManagement.Common;
using ASC.Web.Core.Utility.Skins;

namespace ASC.Web.Core.Utility
{
    public static class SearchHandlerManager
    {
        private static readonly List<ISearchHandlerEx> handlers = new List<ISearchHandlerEx>();

        public static void Registry(ISearchHandlerEx handler)
        {
            lock (handlers)
            {
                if (handler != null && !handlers.Any(h => h.GetType() == handler.GetType()))
                {
                    handlers.Add(handler);
                }
            }
        }

        public static void UnRegistry(ISearchHandlerEx handler)
        {
            lock (handlers)
            {
                if (handler != null)
                {
                    handlers.RemoveAll(h => h.GetType() == handler.GetType());
                }
            }
        }

        public static List<ISearchHandlerEx> GetAllHandlersEx()
        {
            lock (handlers)
            {
                return handlers.ToList();
            }
        }

        public static ISearchHandlerEx GetActiveHandlerEx()
        {
            if (HttpContext.Current == null || HttpContext.Current.Request == null)
            {
                return null;
            }

            var cur_vp = HttpContext.Current.Request.CurrentExecutionFilePath.TrimEnd('/');

            var result = default(ISearchHandlerEx);
            var matchCount = int.MaxValue;
            foreach (var sh in GetAllHandlersEx())
            {
                foreach (var virtualPath in sh.PlaceVirtualPath.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    var vp = VirtualPathUtility.ToAbsolute(virtualPath).TrimEnd('/');
                    if (cur_vp.IndexOf(vp, StringComparison.InvariantCultureIgnoreCase) != -1)
                    {
                        int diff = cur_vp.Length - vp.Length;
                        if (diff == 0)
                        {
                            return sh;
                        }
                        else if (matchCount > diff)
                        {
                            matchCount = diff;
                            result = sh;
                        }
                    }
                }
            }
            return result;
        }

        public static List<ISearchHandlerEx> GetHandlersExForProduct(Guid productID)
        {
            lock (handlers)
            {
                return handlers
                    .Where(h => h.ProductID.Equals(productID) || h.ProductID.Equals(Guid.Empty))
                    .ToList();
            }
        }

        public static List<ISearchHandlerEx> GetHandlersExForCertainProduct(Guid productID)
        {
            lock (handlers)
            {
                return handlers
                    .Where(h => h.ProductID.Equals(productID))
                    .ToList();
            }
        }
    }

    public abstract class BaseSearchHandlerEx : ISearchHandlerEx
    {
        public abstract ImageOptions Logo { get; }

        public abstract string SearchName { get; }

        public virtual string AbsoluteSearchURL { get; set; }

        public abstract string PlaceVirtualPath { get; }


        public virtual IItemControl Control { get { return null; } }

        public virtual Guid ProductID { get { return Guid.Empty; } }

        public virtual Guid ModuleID { get { return Guid.Empty; } }

        public BaseSearchHandlerEx()
        {
            AbsoluteSearchURL = string.Empty;
        }


        public abstract SearchResultItem[] Search(string text);
    }
}


