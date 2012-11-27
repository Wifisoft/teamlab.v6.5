#region Import

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ASC.Core;
using ASC.Core.Common;
using ASC.Core.Users;
using ASC.Projects.Data.DAO;
using ASC.Projects.Core;
using ASC.Projects.Core.Services.NotifyService;
using ASC.Projects.Engine;
using ASC.Web.Controls;
using ASC.Web.Core.Utility.Skins;
using ASC.Web.Projects.Classes;
using ASC.Web.Projects.Configuration;
using ASC.Web.Projects.Controls;
using ASC.Web.Projects.Resources;
using ASC.Projects.Core.Domain;
using ASC.Web.Projects.Controls.Tasks;
using ASC.Web.Projects.Controls.IssueTracker;
using ASC.Web.Studio.Controls.Common;
using ASC.Web.Studio.Utility;
using ASC.Data.Storage;

#endregion

namespace ASC.Web.Projects
{
    public partial class IssueTracker : BasePage
    {

        #region Members

        protected Project Project { get; set; }

        #endregion

        protected override void PageLoad()
        {
            if (!Global.ModuleManager.IsVisible(ModuleType.IssueTracker))
                Response.Redirect(ProjectsCommonResource.StartURL);
            
            int projectID;
            int issueID;
            int actionType;

            if (int.TryParse(UrlParameters.ProjectID, out projectID))
            {
                Project = Global.EngineFactory.GetProjectEngine().GetByID(projectID);
                if (Project != null)
                {
                    if (int.TryParse(UrlParameters.EntityID, out issueID))
                    {
                        if (issueID == -1)
                        {
                            ExecAddIssueDialogView(-1);
                        }
                        else if (Global.EngineFactory.GetIssueEngine().IsExist(issueID))
                        {
                            if (int.TryParse(UrlParameters.ActionType, out actionType))
                            {
                                ExecAddIssueDialogView(issueID);
                            }
                            else
                            {
                                ExecIssueDetailsView(issueID);
                            }
                        }
                        else
                        {
                            ElementNotFoundControlView(projectID);
                        }
                    }
                    else
                    {
                        ExecListIssueTrackerView();
                    }
                }
                else
                {
                    Response.Redirect(ProjectsCommonResource.StartURL);
                }
            }
            else
            {
                Response.Redirect(ProjectsCommonResource.StartURL);
            }

            Title = HeaderStringHelper.GetPageTitle(IssueTrackerResource.Issue, Master.BreadCrumbs);
        }

        protected void ExecListIssueTrackerView()
        {
            ListIssueTrackerView cntrlListIssueTrackerView = (ListIssueTrackerView)LoadControl(PathProvider.GetControlVirtualPath("ListIssueTrackerView.ascx"));

            cntrlListIssueTrackerView.Project = Project;

            //if (SecurityContext.CheckPermissions(task.ProjectFat, SecurityProvider, AuthorizationConstants.Action_Task_Create))
                SideActionsPanel.Controls.Add(new NavigationItem
                {
                    Name = IssueTrackerResource.AddIssue,
                    URL  = String.Format("issueTracker.aspx?prjID={0}&id=-1", Project.ID)
                });

            Master.BreadCrumbs.Add(new BreadCrumb
            {
                Caption = ProjectResource.Projects,
                NavigationUrl = "projects.aspx"

            });

            Master.BreadCrumbs.Add(new BreadCrumb
            {
                Caption = Project.HtmlTitle.HtmlEncode(),
                NavigationUrl = "projects.aspx?prjID=" + Project.ID

            });

            Master.BreadCrumbs.Add(new BreadCrumb
            {
                Caption = IssueTrackerResource.AllIssues

            });

            /*if (SecurityContext.CheckPermissions(ProjectFat, SecurityProvider, AuthorizationConstants.Action_Task_Create))
                SideActionsPanel.Controls.Add(new NavigationItem
                {
                    Name = TaskResource.AddNewTask,
                    URL = "javascript:ASC.Projects.TaskActionPage.init(-1, null, null); ASC.Projects.TaskActionPage.show()"
                });*/

            _content.Controls.Add(cntrlListIssueTrackerView);
        }

        protected void ExecAddIssueDialogView(int issueID)
        {
            AddIssueDialogView cntrlAddIssueDialogView = (AddIssueDialogView)LoadControl(PathProvider.GetControlVirtualPath("AddIssueDialogView.ascx"));            

            cntrlAddIssueDialogView.Participants = Global.EngineFactory.GetProjectEngine().GetTeam(Project.ID);

            if (issueID != -1)
            {
                var issue = Global.EngineFactory.GetIssueEngine().GetIssue(issueID);

                cntrlAddIssueDialogView.Title              = issue.Title;
                cntrlAddIssueDialogView.DetectedInVersion  = issue.DetectedInVersion;
                cntrlAddIssueDialogView.Description        = issue.Description;
                cntrlAddIssueDialogView.CorrectedInVersion = issue.CorrectedInVersion;
                cntrlAddIssueDialogView.Priority           = issue.Priority;
            }
            else
            {
                cntrlAddIssueDialogView.Title              = "";
                cntrlAddIssueDialogView.DetectedInVersion  = "";
                cntrlAddIssueDialogView.Description        = "";
                cntrlAddIssueDialogView.CorrectedInVersion = "";
                cntrlAddIssueDialogView.Priority           = IssuePriority.Normal;
            }

            //if (SecurityContext.CheckPermissions(task.ProjectFat, SecurityProvider, AuthorizationConstants.Action_Task_Create))
            SideActionsPanel.Controls.Add(new NavigationItem
            {
                Name = IssueTrackerResource.AddIssue,
                URL = String.Format("issueTracker.aspx?prjID={0}&id=-1", Project.ID)
            });

            Master.BreadCrumbs.Add(new BreadCrumb
            {
                Caption = ProjectResource.Projects,
                NavigationUrl = "projects.aspx"

            });

            Master.BreadCrumbs.Add(new BreadCrumb
            {
                Caption = Project.HtmlTitle.HtmlEncode(),
                NavigationUrl = "projects.aspx?prjID=" + Project.ID

            });

            Master.BreadCrumbs.Add(new BreadCrumb
            {
                Caption = IssueTrackerResource.AllIssues,
                NavigationUrl = "issueTracker.aspx?prjID=" + Project.ID
            });

            Master.BreadCrumbs.Add(new BreadCrumb
            {
                Caption = IssueTrackerResource.AddNewIssue
            });

            _content.Controls.Add(cntrlAddIssueDialogView);
        }

        protected void ExecIssueDetailsView(int issueID)
        {
            IssueDetailsView cntrlIssueDetailsView = (IssueDetailsView)LoadControl(PathProvider.GetControlVirtualPath("IssueDetailsView.ascx"));

            cntrlIssueDetailsView.Target = Global.EngineFactory.GetIssueEngine().GetIssue(issueID);

            //if (SecurityContext.CheckPermissions(task.ProjectFat, SecurityProvider, AuthorizationConstants.Action_Task_Create))
            SideActionsPanel.Controls.Add(new NavigationItem
            {
                Name = IssueTrackerResource.AddIssue,
                URL = String.Format("issueTracker.aspx?prjID={0}&id=-1", Project.ID)
            });

            //switch (task.Status)
            //{

                //case TaskStatus.NotAccept:

                    //if (SecurityContext.CheckPermissions(task, SecurityProvider, AuthorizationConstants.Action_Task_Update))
                        SideActionsPanel.Controls.Add(new NavigationItem
                        {
                            Name = ProjectsCommonResource.Edit,
                            URL = String.Format("issueTracker.aspx?prjID={0}&id={1}&action=edit", Project.ID, issueID)
                        });



                    //break;

            //}

            //if (SecurityContext.CheckPermissions(task, SecurityProvider, AuthorizationConstants.Action_Task_Remove))
                SideActionsPanel.Controls.Add(new NavigationItem
                {
                    Name = ProjectsCommonResource.Delete,
                    URL = String.Format("javascript:ASC.Projects.IssueTrackerActionPage.execIssueDelete({0}, '{1}')",
                                                                       issueID, cntrlIssueDetailsView.Target.Title)
                });

            Master.BreadCrumbs.Add(new BreadCrumb
            {
                Caption = ProjectResource.Projects,
                NavigationUrl = "projects.aspx"

            });

            Master.BreadCrumbs.Add(new BreadCrumb
            {
                Caption = Project.HtmlTitle.HtmlEncode(),
                NavigationUrl = "projects.aspx?prjID=" + Project.ID

            });

            Master.BreadCrumbs.Add(new BreadCrumb
            {
                Caption = IssueTrackerResource.AllIssues,
                NavigationUrl = "issueTracker.aspx?prjID=" + Project.ID
            });

            Master.BreadCrumbs.Add(new BreadCrumb
            {
                Caption = cntrlIssueDetailsView.Target.Title
            });

            _content.Controls.Add(cntrlIssueDetailsView);
        }

        protected void ElementNotFoundControlView(int prjID)
        {

            _content.Controls.Add(new ElementNotFoundControl
            {
                Header = TaskResource.TaskNotFound_Header,
                Body = TaskResource.TaskNotFound_Body,
                RedirectURL = String.Format("issueTracker.aspx?prjID={0}", prjID),
                RedirectTitle = TaskResource.TaskNotFound_RedirectTitle
            });

            SideActionsPanel.Visible = false;
        }
    }
}