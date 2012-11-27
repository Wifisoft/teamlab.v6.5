using System;
using System.Collections.Generic;
using System.Web.UI;
using AjaxPro;
using ASC.Forum;
using ASC.Web.Controls;
using ASC.Web.Studio.Utility;
using ASC.Web.UserControls.Forum.Common;
using ASC.Core;
using ASC.Web.Studio.Core;
using System.Web.UI.WebControls;

namespace ASC.Web.UserControls.Forum
{
    internal class PollVoteHandler : IVoteHandler  
    {
        #region IVoteHandler Members

        public bool VoteCallback(string pollID, List<string> selectedVariantIDs, string additionalParams, out string errorMessage)
        {   
            errorMessage = "";
            var settingsID = new Guid(additionalParams.Split(',')[0]);
            int idQuestion = Convert.ToInt32(additionalParams.Split(',')[1]);
            var _forumManager = ForumManager.GetForumManager(settingsID);
                    

            List<int> variantIDs = new List<int>(0);
            foreach (string id in selectedVariantIDs)
            {
                if (!String.IsNullOrEmpty(id))
                    variantIDs.Add(Convert.ToInt32(id));
            }

            var q = ForumDataProvider.GetPollByID(TenantProvider.CurrentTenantID, idQuestion);
            if (SetupInfo.WorkMode == WorkMode.Promo
                || q == null
                || !_forumManager.ValidateAccessSecurityAction(ForumAction.PollVote, q)
                || ForumDataProvider.IsUserVote(TenantProvider.CurrentTenantID, idQuestion,SecurityContext.CurrentAccount.ID))
            {
                errorMessage = Resources.ForumUCResource.ErrorAccessDenied;
                return false;
            }

            try
            {
                ForumDataProvider.PollVote(TenantProvider.CurrentTenantID, idQuestion, variantIDs);
            }
            catch (Exception e)
            {
                errorMessage = e.Message.HtmlEncode();
                return false;
            }

            var settings = ForumManager.GetSettings(settingsID);
            if (settings != null && settings.ActivityPublisher != null)
                settings.ActivityPublisher.Vote(q.Name, q.TopicID);
            
            return true;
        }

        #endregion

    }

    [AjaxNamespace("PostListControl")]
    public partial class PostListControl : UserControl
    {  
        public Guid SettingsID { get; set; }

        public Topic Topic { get; set; }

        protected Settings _settings;
        private ForumManager _forumManager;

        protected void Page_Load(object sender, EventArgs e)
        {
            _settings = ForumManager.GetSettings(SettingsID);
            _forumManager = _settings.ForumManager;

            if (this.Topic == null)
                Response.Redirect(_settings.StartPageAbsolutePath);

            if ((new Thread() { ID = Topic.ThreadID }).Visible == false)
                Response.Redirect(_settings.StartPageAbsolutePath);
            

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
            
            int postCountInTopic = 0;
            var posts = ForumDataProvider.GetPosts(TenantProvider.CurrentTenantID, Topic.ID, currentPageNumber, _settings.PostCountOnPage, out postCountInTopic);
            
            PageNavigator pageNavigator = new PageNavigator()
            {
                PageUrl = _settings.LinkProvider.PostList(Topic.ID),
                CurrentPageNumber = currentPageNumber,
                EntryCountOnPage = _settings.PostCountOnPage,
                VisiblePageCount = 5,
                EntryCount = postCountInTopic
            };

            bottomPageNavigatorHolder.Controls.Add(pageNavigator);

            int i = 0;
            foreach (Post post in posts)
            {
                PostControl postControl = (PostControl)LoadControl(_settings.UserControlsVirtualPath+"/PostControl.ascx");
                postControl.Post = post;
                postControl.IsEven = (i%2==0);
                postControl.SettingsID = SettingsID;
                postControl.CurrentPageNumber = currentPageNumber;
				postControl.PostsCount = Topic.PostCount;
                this.postListHolder.Controls.Add(postControl);
                i++;
            }

            ForumDataProvider.SetTopicVisit(Topic);

            if (Topic.Type == TopicType.Poll)
            {
                var q = ForumDataProvider.GetPollByID(TenantProvider.CurrentTenantID, Topic.QuestionID);
                if(q==null)
                    return;

                var isVote = ForumDataProvider.IsUserVote(TenantProvider.CurrentTenantID, q.ID, SecurityContext.CurrentAccount.ID);

                var pollForm = new PollForm();
                pollForm.VoteHandlerType = typeof(PollVoteHandler);
                pollForm.Answered =isVote ||Topic.Closed
                                    || !_forumManager.ValidateAccessSecurityAction(ForumAction.PollVote, q) 
                                    ||  ASC.Core.SecurityContext.DemoMode || (SetupInfo.WorkMode == WorkMode.Promo);

                pollForm.Name = q.Name;
                pollForm.PollID = q.ID.ToString();
                pollForm.Singleton = (q.Type == QuestionType.OneAnswer);
                pollForm.AdditionalParams = _settings.ID.ToString() + "," + q.ID.ToString();
                foreach (var variant in q.AnswerVariants)
                {
                    pollForm.AnswerVariants.Add(new PollForm.AnswerViarint()
                                                     {
                                                         ID = variant.ID.ToString(),
                                                         Name = variant.Name,
                                                         VoteCount = variant.AnswerCount
                                                     });
                }


                pollHolder.Controls.Add(new Literal() { Text = "<div style='position:relative; padding-left:20px; margin-bottom:15px;'>" });
                pollHolder.Controls.Add(pollForm);
                pollHolder.Controls.Add(new Literal() { Text = "</div>" });
            }
        }
    }
}