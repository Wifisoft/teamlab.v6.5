using System;
using System.Collections.Generic;
using System.Web;
using ASC.Forum;
using ASC.Web.Community.Product;
using ASC.Web.Core.ModuleManagement.Common;
using ASC.Web.Core.Utility;
using ASC.Web.Core.Utility.Skins;
using ASC.Web.Studio.Controls.Common;
using ASC.Web.Studio.Utility;

namespace ASC.Web.Community.Forum
{
    public class ForumSearchHandler : BaseSearchHandlerEx
    {
        #region ISearchHandler Members

        public override SearchResultItem[] Search(string text)
        {           
            var topicCount = 0;
            var findTopicList = ForumDataProvider.SearchTopicsByText(TenantProvider.CurrentTenantID,text, 1, -1, out topicCount);
            

            List<SearchResultItem> searchResult = new List<SearchResultItem>();
            foreach (var topic in findTopicList)
            {
                SearchResultItem searchItem = new SearchResultItem()
                {
                    Name = topic.Title,
                    Description =  String.Format(Resources.ForumResource.FindTopicDescription, topic.ThreadTitle),
                    URL = VirtualPathUtility.ToAbsolute(ForumManager.BaseVirtualPath + "/posts.aspx") + "?t=" + topic.ID,
                    Date = topic.CreateDate
                };

                searchResult.Add(searchItem);
            }

            return searchResult.ToArray();
        }

        #endregion

        public override string AbsoluteSearchURL
        {
            get { return VirtualPathUtility.ToAbsolute(ForumManager.BaseVirtualPath + "/search.aspx");}
        }

        public override ImageOptions Logo
        {
            get 
            {
                return new ImageOptions() { ImageFileName = "forum_mini_icon.png", PartID = ForumManager.ModuleID };
            }
        }

        public override string SearchName
        {
            get { return Resources.ForumResource.SearchDefaultString; }
        }

        public override Guid ModuleID
        {
            get { return ForumManager.ModuleID; }
        }

        public override string PlaceVirtualPath
        {
            get { return ForumManager.BaseVirtualPath; }
        }

        public override Guid ProductID
        {
            get { return CommunityProduct.ID; }
        }

        public override IItemControl Control
        {
            get { return new ResultsView(); }
        }
    }
}
