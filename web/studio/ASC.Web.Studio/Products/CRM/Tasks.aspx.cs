#region Import

using System;
using ASC.CRM.Core;
using ASC.Web.CRM.Controls.Common;
using ASC.Web.Controls;
using ASC.Web.CRM.Classes;
using ASC.Web.CRM.Controls.Tasks;
using ASC.Web.CRM.Resources;
using ASC.Web.Studio.Controls.Common;
using ASC.Web.Studio.Utility;

#endregion

namespace ASC.Web.CRM
{
    public partial class Tasks : BasePage
    {
        #region Events

        protected override void PageLoad()
        {
            if (String.Compare(UrlParameters.Action, "import", true) == 0)
                ExecImportView();
            else
                ExecListTaskView();

            Master.DisabledSidePanel = true;
        }

        #endregion

        #region Methods

        protected void ExecImportView()
        {
            Master.BreadCrumbs.Add(new BreadCrumb
            {
                Caption = CRMTaskResource.ImportTasks
            });

            Master.DisabledSidePanel = true;

            var importViewControl = (ImportFromCSVView)LoadControl(ImportFromCSVView.Location);

            importViewControl.EntityType = EntityType.Task;

            CommonContainerHolder.Controls.Add(importViewControl);

            Title = HeaderStringHelper.GetPageTitle(CRMTaskResource.ImportTasks, Master.BreadCrumbs);
        }

        protected void ExecListTaskView()
        {
            var title = CRMTaskResource.Tasks;

            Master.BreadCrumbs.Add(new BreadCrumb { Caption = title });
            Title = HeaderStringHelper.GetPageTitle(title, Master.BreadCrumbs);

            var ctrlListTaskView = (ListTaskView)LoadControl(ListTaskView.Location);

            ctrlListTaskView.CurrentEntityType = EntityType.Contact;
            ctrlListTaskView.EntityID = 0;
            ctrlListTaskView.CurrentContact = null;
            CommonContainerHolder.Controls.Add(ctrlListTaskView);
        }

        protected void ExecTaskDetailsView(int taskID)
        {
            var task = Global.DaoFactory.GetTaskDao().GetByID(taskID);

            if (!CRMSecurity.CanAccessTo(task))
                Response.Redirect(PathProvider.StartURL());

            Master.BreadCrumbs.Add(new BreadCrumb
            {
                Caption = CRMTaskResource.Tasks,
                NavigationUrl = "tasks.aspx"

            });

            Master.BreadCrumbs.Add(new BreadCrumb
            {
                Caption = task.Title,
                NavigationUrl = String.Empty

            });

            SideActionsPanel.Controls.Add(new NavigationItem
            {
                Name = CRMTaskResource.AddNewTask,
                URL = string.Format("javascript:ASC.CRM.TaskActionView.showTaskPanel(0, 0, {0}, 0);",
                            (int)EntityType.Contact)
            });

            if (CRMSecurity.CanEdit(task))
            {
                SideActionsPanel.Controls.Add(new NavigationItem
                {
                    Name = CRMTaskResource.EditTask,
                    URL = string.Format("javascript:ASC.CRM.TaskActionView.showTaskPanel({0}, {1}, {2}, {3});",
                                taskID, task.ContactID,
                                (int)task.EntityType, task.EntityID)
                });

                SideActionsPanel.Controls.Add(new NavigationItem
                {
                    Name = CRMTaskResource.DeleteTask,
                    URL = string.Format("javascript:ASC.CRM.ListTaskView.deleteTaskItem({0}, true);", taskID)
                });
            }

            var closedBy = string.Empty;

            if (task.IsClosed)
                closedBy = string.Format("<div class='crm_taskTitleClosedByPanel'>{0}<div>", CRMTaskResource.ClosedTask);
            
            Master.CommonContainerHeader = string.Format("{0}{1}", task.Title.HtmlEncode(), closedBy);

            Title = HeaderStringHelper.GetPageTitle(task.Title, Master.BreadCrumbs);
        }

        #endregion
    }
}
