#region Usings

using System;
using ASC.Projects.Core.Domain;
using ASC.Projects.Engine;
using ASC.Web.Controls;
using ASC.Web.Projects.Classes;
using ASC.Web.Projects.Controls;
using ASC.Web.Projects.Controls.Messages;
using ASC.Web.Projects.Resources;
using ASC.Web.Studio.Masters;
using ASC.Web.Studio.Utility;
using AjaxPro;

#endregion

namespace ASC.Web.Projects
{
    public partial class Messages : BasePage
    {
        protected Project Project { get; set; }

        protected Message Discussion { get; set; }

        protected override void PageLoad()
        {
            Project = RequestContext.GetCurrentProject(false);

            if (RequestContext.IsInConcreteProject())
            {
                var project = RequestContext.GetCurrentProject(false);

                if (project == null)
                {
                    Response.Redirect("messages.aspx", true);
                }
                else
                {
                    if (!ProjectSecurity.CanReadMessages(Project))
                    {
                        Response.Redirect("projects.aspx?prjID=" + project.ID, true);
                    }
                }
            }
            ((IStudioMaster)Master).DisabledSidePanel = true;

            int discussionId;
            if (int.TryParse(UrlParameters.EntityID, out discussionId))
            {
                if (Project == null) return;

                Discussion = Global.EngineFactory.GetMessageEngine().GetByID(discussionId);

                if (string.Compare(UrlParameters.ActionType, "edit", StringComparison.OrdinalIgnoreCase) == 0)
                {
                    if (ProjectSecurity.CanEdit(Discussion))
                    {
                        LoadDiscussionActionControl(Project, Discussion);
                    }
                    else
                    {
                        Response.Redirect("messages.aspx", true);
                    }
                }
                else if (Discussion != null && ProjectSecurity.CanRead(Discussion.Project) && Discussion.Project.ID == Project.ID)
                {
                    LoadDiscussionDetailsControl(Project, Discussion);
                }
                else
                {
                    LoadElementNotFoundControl(Project.ID);
                }
            }
            else
            {
                if (string.Compare(UrlParameters.ActionType, "add", StringComparison.OrdinalIgnoreCase) == 0)
                {
                    if (ProjectSecurity.CanCreateMessage(Project))
                    {
                        LoadDiscussionActionControl(Project, null);
                    }
                    else
                    {
                        Response.Redirect("messages.aspx", true);
                    }
                }
                else
                {
                    LoadDiscussionsListControl(Project == null ? -1 : Project.ID);
                }
            }

            InitBreadCrumbs();

            Title = HeaderStringHelper.GetPageTitle(MessageResource.Messages, Master.BreadCrumbs);
        }

        private void LoadDiscussionDetailsControl(Project project, Message discussion)
        {
            var discussionDetails = (DiscussionDetails)LoadControl(PathProvider.GetControlVirtualPath("DiscussionDetails.ascx"));
            discussionDetails.Discussion = discussion;
            discussionDetails.Project = project;
            contentHolder.Controls.Add(discussionDetails);
        }

        private void LoadDiscussionsListControl(int projectId)
        {
            var discussionsList = (DiscussionsList)LoadControl(PathProvider.GetControlVirtualPath("DiscussionsList.ascx"));
            discussionsList.ProjectId = projectId;
            contentHolder.Controls.Add(discussionsList);
        }

        private void LoadDiscussionActionControl(Project project, Message discussion)
        {
            var discussionAction = (DiscussionAction)LoadControl(PathProvider.GetControlVirtualPath("DiscussionAction.ascx"));
            discussionAction.Project = project;
            discussionAction.Discussion = discussion;
            contentHolder.Controls.Add(discussionAction);
        }

        private void LoadElementNotFoundControl(int projectId)
        {
            contentHolder.Controls.Add(new ElementNotFoundControl
            {
                Header = MessageResource.MessageNotFound_Header,
                Body = MessageResource.MessageNotFound_Body,
                RedirectURL = String.Format("messages.aspx?prjID={0}", projectId),
                RedirectTitle = MessageResource.MessageNotFound_RedirectTitle
            });
        }

        public void InitBreadCrumbs()
        {
            if (Project == null)
            {
                Master.BreadCrumbs.Add(new BreadCrumb(MessageResource.Messages));
                return;
            }

            Master.BreadCrumbs.Add(new BreadCrumb(ProjectResource.Projects, "projects.aspx"));
            Master.BreadCrumbs.Add(new BreadCrumb(Project.HtmlTitle, "projects.aspx?prjID=" + Project.ID));
            Master.BreadCrumbs.Add(new BreadCrumb(MessageResource.Messages, "messages.aspx?prjID=" + Project.ID));

            if (string.Compare(UrlParameters.ActionType, "add", StringComparison.OrdinalIgnoreCase) == 0 && Discussion == null)
            {
                Master.BreadCrumbs.Add(new BreadCrumb(MessageResource.NewMessage));
                return;
            }

            if (string.IsNullOrEmpty(UrlParameters.EntityID)) return;

            if (Discussion != null && ProjectSecurity.CanRead(Discussion.Project) && Discussion.Project.ID == Project.ID)
            {
                Master.BreadCrumbs.Add(new BreadCrumb());
            }
            else
            {
                Master.BreadCrumbs.Clear();
            }
        }
    }
}
