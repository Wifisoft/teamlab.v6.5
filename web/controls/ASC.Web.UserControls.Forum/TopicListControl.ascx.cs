using System;
using System.Collections.Generic;
using System.Web.UI;
using ASC.Core;
using ASC.Forum;
using ASC.Web.Controls;
using ASC.Web.Studio.Utility;
using ASC.Web.UserControls.Forum.Common;

namespace ASC.Web.UserControls.Forum
{
    public partial class TopicListControl : UserControl
    {
        public int ThreadID { get; set; }

        public List<Topic> Topics { get; private set; }

        public Guid SettingsID { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            var settings = ForumManager.GetSettings(SettingsID);

            Topics = new List<Topic>();
           
            if (ThreadID == 0)
                Response.Redirect(settings.StartPageAbsolutePath);

            int currentPageNumber = 0;
            if (!String.IsNullOrEmpty(Request["p"]))
            {
                try
                {
                    currentPageNumber = Convert.ToInt32(Request["p"]);
                }
                catch { currentPageNumber = 0; }
            }
            if (currentPageNumber <= 0)
                currentPageNumber = 1;

            int topicCountInThread = 0;
            Topics = ForumDataProvider.GetTopics(TenantProvider.CurrentTenantID, SecurityContext.CurrentAccount.ID, ThreadID, currentPageNumber, settings.TopicCountOnPage, out topicCountInThread);

            ForumDataProvider.SetThreadVisit(TenantProvider.CurrentTenantID, ThreadID);

            int i = 0;
            foreach (Topic topic in Topics)
            {
                TopicControl topicControl = (TopicControl)LoadControl(settings.UserControlsVirtualPath+"/TopicControl.ascx");
                topicControl.Topic = topic;
                topicControl.SettingsID = SettingsID;
                topicControl.IsEven = (i % 2 == 0);
                this.topicListHolder.Controls.Add(topicControl);
                i++;
            }

            PageNavigator pageNavigator = new PageNavigator()
            {
                PageUrl = settings.LinkProvider.TopicList(ThreadID),
                CurrentPageNumber = currentPageNumber,
                EntryCountOnPage = settings.TopicCountOnPage,
                VisiblePageCount = 5,
                EntryCount = topicCountInThread
            };

            bottomPageNavigatorHolder.Controls.Add(pageNavigator);
        }
    }
}