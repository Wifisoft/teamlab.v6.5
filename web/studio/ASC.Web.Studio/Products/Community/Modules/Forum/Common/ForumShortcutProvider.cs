using System;
using System.Text.RegularExpressions;
using System.Web;
using ASC.Forum;
using ASC.Web.Studio.Utility;
using ASC.Web.UserControls.Forum.Common;

namespace ASC.Web.Community.Forum
{
    public static class ForumShortcutProvider
    {
        public static string GetCreateContentPageUrl()
        {
            return ValidateCreateTopicOrPoll(false) ? VirtualPathUtility.ToAbsolute(ForumManager.BaseVirtualPath + "/newpost.aspx") + "?m=0" : null;
        }

        public static string GetAbsoluteWebPathForShortcut(Guid shortcutID)
        {
            if (shortcutID.Equals(new Guid("24CD48B2-C40F-43ec-B3A6-3212C51B8D34")) || shortcutID.Equals(new Guid("84DF7BE7-315B-4ba3-9BE1-1E348F6697A5")))
            {
                var ispool = shortcutID.Equals(new Guid("84DF7BE7-315B-4ba3-9BE1-1E348F6697A5"));
                if (ForumManager.Instance.CurrentPage.Page == ForumPage.TopicList || ForumManager.Instance.CurrentPage.Page == ForumPage.NewPost)
                {
                    var threadID = GetThreadID();
                    if (threadID != -1)
                    {
                        return GetUrl(threadID, ispool);
                    }
                    if (ForumManager.Instance.CurrentPage.Page == ForumPage.NewPost)
                    {
                        var topic = GetTopicByID();
                        if (topic != null)
                        {
                            return GetUrl(topic.ThreadID, ispool);
                        }
                    }
                }
                else if (ForumManager.Instance.CurrentPage.Page == ForumPage.PostList || ForumManager.Instance.CurrentPage.Page == ForumPage.EditTopic)
                {
                    var topic = GetTopicByID();
                    if (topic != null)
                    {
                        return GetUrl(topic.ThreadID, ispool);
                    }
                }
                return GetUrl(0, ispool);
            }

            else if (shortcutID.Equals(new Guid("FA5C4BD5-25E7-41c8-A0DC-64DC2A977391")))
            {
                if (ForumManager.Instance.CurrentPage.Page == ForumPage.PostList || ForumManager.Instance.CurrentPage.Page == ForumPage.NewPost || ForumManager.Instance.CurrentPage.Page == ForumPage.EditTopic)
                {
                    int topicID = GetTopicID();
                    if (topicID != -1)
                    {
                        return VirtualPathUtility.ToAbsolute(ForumManager.BaseVirtualPath + "/newpost.aspx") + "?t=" + topicID;
                    }
                }
            }
            return string.Empty;
        }

        public static bool CheckPermissions(Guid shortcutID)
        {
            if (shortcutID.Equals(new Guid("A04A7DBF-6B73-4579-BECE-3F6E346133DB")))
            {
                return ForumManager.Instance.ValidateAccessSecurityAction(ForumAction.GetAccessForumEditor, null);
            }
            else if (shortcutID.Equals(new Guid("87A6B7FC-E872-49db-A327-CEA9CBA59CCC")))
            {
                return ForumManager.Instance.ValidateAccessSecurityAction(ForumAction.GetAccessTagEditor, null);
            }
            else if (shortcutID.Equals(new Guid("24CD48B2-C40F-43ec-B3A6-3212C51B8D34")) || shortcutID.Equals(new Guid("84DF7BE7-315B-4ba3-9BE1-1E348F6697A5")))
            {
                var ispool = shortcutID.Equals(new Guid("84DF7BE7-315B-4ba3-9BE1-1E348F6697A5"));
                if (ForumManager.Instance.CurrentPage.Page == ForumPage.TopicList || ForumManager.Instance.CurrentPage.Page == ForumPage.NewPost)
                {
                    var threadID = GetThreadID();
                    if (threadID != 0)
                    {
                        return ValidateCreateTopicOrPoll(ispool, new Thread { ID = threadID });
                    }
                    if (ForumManager.Instance.CurrentPage.Page == ForumPage.NewPost)
                    {

                        var topicID = GetTopicID();
                        if (topicID != 0)
                        {
                            return ValidateCreateTopicOrPoll(ispool, new Topic { ID = topicID });
                        }
                    }
                }
                else if (ForumManager.Instance.CurrentPage.Page == ForumPage.PostList || ForumManager.Instance.CurrentPage.Page == ForumPage.EditTopic)
                {
                    var topicID = GetTopicID();
                    if (topicID != 0)
                    {
                        return ValidateCreateTopicOrPoll(ispool, new Topic { ID = topicID });
                    }
                }
                return ValidateCreateTopicOrPoll(ispool);
            }
            else if (shortcutID.Equals(new Guid("FA5C4BD5-25E7-41c8-A0DC-64DC2A977391")))
            {
                if (ForumManager.Instance.CurrentPage.Page == ForumPage.PostList || ForumManager.Instance.CurrentPage.Page == ForumPage.NewPost || ForumManager.Instance.CurrentPage.Page == ForumPage.EditTopic)
                {
                    var topicID = GetTopicID();
                    if (topicID != 0)
                    {
                        return ForumManager.Instance.ValidateAccessSecurityAction(ForumAction.PostCreate, new Topic { ID = topicID });
                    }
                }
            }
            return false;
        }


        private static Topic GetTopicByID()
        {
            var topicID = GetTopicID();
            return ForumDataProvider.GetTopicByID(TenantProvider.CurrentTenantID, topicID);
        }

        private static int GetThreadID()
        {
            var currentUrl = HttpContext.Current != null && HttpContext.Current.Request != null ? HttpContext.Current.Request.Url.AbsoluteUri : string.Empty;
            var regExp = new Regex(@"[\?&]f=(?<threadID>[0-9]*)&?", RegexOptions.IgnoreCase | RegexOptions.Compiled);
            var m = regExp.Match(currentUrl);
            if (m.Success)
            {
                int res;
                if (int.TryParse(m.Groups["threadID"].Value, out res)) return res;
            }
            return -1;

        }

        private static int GetTopicID()
        {
            var currentUrl = HttpContext.Current != null && HttpContext.Current.Request != null ? HttpContext.Current.Request.Url.AbsoluteUri : string.Empty;
            var regExp = new Regex(@"[\?&]t=(?<topicID>[0-9]*)&?", RegexOptions.IgnoreCase | RegexOptions.Compiled);
            var m = regExp.Match(HttpContext.Current.Request.Url.AbsoluteUri);
            if (m.Success)
            {
                int res;
                if (int.TryParse(m.Groups["topicID"].Value, out res)) return res;
            }

            return -1;
        }


        private static string GetUrl(int thread, bool ispool)
        {
            var url = VirtualPathUtility.ToAbsolute(ForumManager.BaseVirtualPath + "/newpost.aspx") + "?";
            if (0 < thread) url += "f=" + thread + "&";
            return url + "m=" + (ispool ? "1" : "0");
        }


        private static bool ValidateCreateTopicOrPoll(bool isPool)
        {
            return ValidateCreateTopicOrPoll(isPool, null);
        }

        private static bool ValidateCreateTopicOrPoll(bool isPool, object targetObject)
        {
            return ForumManager.Instance.ValidateAccessSecurityAction((isPool ? ForumAction.PollCreate : ForumAction.TopicCreate), targetObject);
        }
    }
}
