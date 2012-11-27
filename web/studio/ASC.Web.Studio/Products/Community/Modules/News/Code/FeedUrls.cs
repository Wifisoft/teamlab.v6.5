using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

namespace ASC.Web.Community.News.Code
{
    internal static class FeedUrls
    {
        public static string BaseVirtualPath
        {
            get { return "~/products/community/modules/news"; }
        }


        public static string MainPageName
        {
            get { return "/"; }
        }

        public static string MainPageVirtualPath
        {
            get { return string.Format("{0}/{1}", BaseVirtualPath, MainPageName); }
        }

        public static string MainPageUrl
        {
            get { return VirtualPathUtility.ToAbsolute(MainPageVirtualPath); }
        }


        public static string EditNewsName
        {
            get { return "editnews.aspx"; }
        }

        public static string EditPollName
        {
            get { return "editpoll.aspx"; }
        }

        public static string EditNewsUrl
        {
            get { return VirtualPathUtility.ToAbsolute(string.Format("{0}/{1}", BaseVirtualPath, EditNewsName)); }
        }

        public static string EditOrderUrl
        {
            get { return VirtualPathUtility.ToAbsolute(string.Format("{0}/{1}", BaseVirtualPath, EditNewsName)) + "?type=" + FeedType.Order.ToString(); }
        }

        public static string EditAdvertUrl
        {
            get { return VirtualPathUtility.ToAbsolute(string.Format("{0}/{1}", BaseVirtualPath, EditNewsName)) + "?type=" + FeedType.Advert.ToString(); }
        }

        public static string EditPollUrl
        {
            get { return VirtualPathUtility.ToAbsolute(string.Format("{0}/{1}", BaseVirtualPath, EditPollName)); }
        }

        public static string GetFeedAbsolutePath(long feedId)
        {
            return VirtualPathUtility.ToAbsolute(BaseVirtualPath) + "?docid=" + feedId.ToString();
        }

        public static string GetFeedVirtualPath(long feedId)
        {
            return string.Format("{0}?docid={1}", MainPageVirtualPath, feedId);
        }

        public static string GetFeedUrl(long feedId)
        {
            return GetFeedUrl(feedId, Guid.Empty);
        }

        public static string GetFeedUrl(long feedId, Guid userId)
        {
            var url = string.Format("{0}?docid={1}", MainPageUrl, feedId);
            if (userId != Guid.Empty) url += "&uid=" + userId.ToString();
            return url;
        }

        public static string GetFeedListUrl(FeedType feedType)
        {
            return GetFeedListUrl(feedType, Guid.Empty);
        }

        public static string GetFeedListUrl(Guid userId)
        {
            return GetFeedListUrl(FeedType.All, userId);
        }

        public static string GetFeedListUrl(FeedType feedType, Guid userId)
        {
            var url = MainPageUrl;
            string p1 = null;
            string p2 = null;

            if (feedType != FeedType.All) p1 = "type=" + feedType.ToString();
            if (userId != Guid.Empty) p2 = "uid=" + userId.ToString();
            if (p1 == null && p2 == null) return url;

            url += "?";
            if (p1 != null) url += p1 + "&";
            if (p2 != null) url += p2 + "&";
            return url.Substring(0, url.Length - 1);
        }
    }
}