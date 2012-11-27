#region Usings

using System;
using System.Collections.Generic;
using System.Text;
using ASC.Core;
using ASC.Core.Users;
using ASC.Notify.Recipients;
using ASC.Projects.Core.Domain;
using ASC.Projects.Core.Services.NotifyService;
using ASC.Projects.Engine;
using ASC.Web.Core.Users;
using ASC.Web.Core.Utility.Skins;
using ASC.Web.Projects.Classes;
using ASC.Web.Projects.Resources;
using ASC.Web.Studio.Utility;
using System.Linq;

#endregion

namespace ASC.Web.Projects.Controls.Messages
{
    public partial class DiscussionAction : BaseUserControl
    {
        private static Guid CurrentUserId { get { return SecurityContext.CurrentAccount.ID; } }
        
        private IRecipient[] recipients;

        protected bool IsMobile;

        public Project Project { get; set; }
        public Message Discussion { get; set; }
        public UserInfo Author { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            GetApiData();

            IsMobile = Core.Mobile.MobileDetector.IsRequestMatchesMobile(Context);

            fckEditor.BasePath = CommonControlsConfigurer.FCKEditorBasePath;
            fckEditor.ToolbarSet = "BlogToolbar";
            fckEditor.EditorAreaCSS = WebSkin.GetUserSkin().BaseCSSFileAbsoluteWebPath;
            fckEditor.Visible = !IsMobile;

            var discussionParticipants = new List<Participant>();
            
            if (Discussion != null)
            {
                discussionTitle.Text = Discussion.Title;
                if (!IsMobile)
                {
                    fckEditor.Value = Discussion.Content;
                }
                else
                {
                    discussionContent.Text = Discussion.Content;
                }
                
                recipients = NotifySource.Instance.GetSubscriptionProvider()
                    .GetRecipients(NotifyConstants.Event_NewCommentForMessage, String.Format("{0}_{1}", Discussion.UniqID, Discussion.Project.ID));

                discussionParticipants.AddRange(recipients.Select(r => Global.EngineFactory.GetParticipantEngine().GetByID(new Guid(r.ID))));
                Author = CoreContext.UserManager.GetUsers(Discussion.CreateBy);
            }
            else if (Project != null)
            {
                var projectParticipants = Global.EngineFactory.GetProjectEngine().GetTeam(Project.ID)
                    .OrderBy(p => p.UserInfo, UserInfoComparer.Default)
                    .ToList();
                discussionParticipants.AddRange(projectParticipants);
            }

            Author = CoreContext.UserManager.GetUsers(SecurityContext.CurrentAccount.ID);

            discussionParticipantRepeater.DataSource = discussionParticipants;
            discussionParticipantRepeater.DataBind();

            LoadDiscussionParticipantsSelector();

            LoadDiscussionFilesControl();
        }

        private void LoadDiscussionFilesControl()
        {
            var discussionFilesControl = 
                (Studio.UserControls.Common.Attachments.Attachments)LoadControl(Studio.UserControls.Common.Attachments.Attachments.Location);
            discussionFilesControl.EntityType = "message";
            discussionFilesControl.ModuleName = "projects";
            discussionFilesPlaceHolder.Controls.Add(discussionFilesControl);
        }

        private void LoadDiscussionParticipantsSelector()
        {
            var discussionParticipantsSelector = (Studio.UserControls.Users.UserSelector)LoadControl(Studio.UserControls.Users.UserSelector.Location);
            discussionParticipantsSelector.BehaviorID = "discussionParticipantsSelector";
            discussionParticipantsSelector.DisabledUsers.Add(new Guid());
            discussionParticipantsSelector.Title = MessageResource.DiscussionParticipants;
            discussionParticipantsSelector.SelectedUserListTitle = MessageResource.DiscussionParticipants;

            discussionParticipantsSelectorHolder.Controls.Add(discussionParticipantsSelector);
        }

        private void GetApiData()
        {
            var apiServer = new Api.ApiServer();
            const string fields = "id,title,security";

            var projects = apiServer.GetApiResponse(ProjectSecurity.IsAdministrator(CurrentUserId) ?
                string.Format("api/1.0/project/filter.json?status=open&sortBy=title&sortOrder=ascending&fields={0}", fields) :
                string.Format("api/1.0/project/filter.json?participant={0}&status=open&sortBy=title&sortOrder=ascending&fields={1}", CurrentUserId, fields),
            "GET");
            Page.JsonPublisher(projects, "projects");
        }


        protected string GetDiscussionAction()
        {
            var innerHTML = new StringBuilder();
            var discussionId = Discussion == null ? -1 : Discussion.ID;
            var action = Discussion == null ? MessageResource.AddDiscussion : ProjectsCommonResource.SaveChanges;

            innerHTML.AppendFormat("<a id='discussionActionButton' class='baseLinkButton' discussionId='{0}'>{1}</a>", 
                                    discussionId, action);

            innerHTML.AppendFormat("<a id='discussionPreviewButton' class='baseLinkButton' authorName='{0}' authorAvatarUrl='{1}' authorTitle='{2}'>{3}</a>",
                                   Author.DisplayUserName(), Author.GetBigPhotoURL(), Author.Title.HtmlEncode(), ProjectsCommonResource.Preview);

            innerHTML.AppendFormat("<a id='discussionCancelButton' class='grayLinkButton'>{0}</a>",
                                   ProjectsCommonResource.Cancel);

            return innerHTML.ToString();
        }
    }
}
