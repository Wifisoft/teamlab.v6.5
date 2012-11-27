#region Import

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using AjaxPro;
using ASC.Core.Users;
using ASC.Notify.Patterns;
using ASC.Projects.Core.Domain;
using ASC.Projects.Core.Services.NotifyService;
using ASC.Web.Controls;
using ASC.Web.Controls.CommentInfoHelper;
using ASC.Web.Core.Helpers;
using ASC.Web.Core.Users;
using ASC.Web.Core.Utility.Skins;
using ASC.Web.Projects.Classes;
using ASC.Web.Projects.Configuration;
using ASC.Web.Projects.Controls.Tasks;
using ASC.Web.Projects.Resources;
using ASC.Web.Studio.Utility;
using ASC.Projects.Core;
using System.Web.UI.WebControls;
using System.Globalization;
using System.Threading;
using ASC.Core;

#endregion

namespace ASC.Web.Projects.Controls.TimeSpends
{
    public partial class TimeSpendRecord : BaseUserControl
    {
        #region Properties

        public TimeSpend TimeSpend { get; set; }
        public Project Project { get; set; }
        public string CssClass { get; set; }

        #endregion


        #region Events

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        #endregion


        #region Methods

        public string InitUsersDdl()
        {
            StringBuilder sb = new StringBuilder();
            List<Participant> team = Global.EngineFactory.GetProjectEngine().GetTeam(Project.ID);

            team.Sort((oX, oY) =>
            {
                return String.Compare(oX.UserInfo.DisplayUserName(), oY.UserInfo.DisplayUserName());
            });

            foreach (Participant prt in team)
            {
                if (prt.UserInfo.ID == TimeSpend.Person)
                    sb.AppendFormat("<option value='{0}' selected='selected' id='record{2}ddlUser{0}'>{1}</option>", prt.UserInfo.ID, prt.UserInfo.DisplayUserName(), TimeSpend.ID);
                else
                    sb.AppendFormat("<option value='{0}' id='record{2}ddlUser{0}'>{1}</option>", prt.UserInfo.ID, prt.UserInfo.DisplayUserName(), TimeSpend.ID);
            }

            return sb.ToString();
        }

        public string Date(DateTime date)
        {
            return date.ToString(DateTimeExtension.DateFormatPattern);
        }

        public string GetID()
        {
            return TimeSpend.ID.ToString();
        }

        public string ProfileLink()
        {
            return StudioUserInfoExtension.RenderProfileLink(Global.EngineFactory.GetParticipantEngine().GetByID(TimeSpend.Person).UserInfo, ProductEntryPoint.ID);
        }

        public string GetNote()
        {
            if (TimeSpend.RelativeTask != 0)
            {
                var task = Global.EngineFactory.GetTaskEngine().GetByID(TimeSpend.RelativeTask);
                if (TimeSpend.Note != string.Empty)
                    return string.Format("<a href=\"tasks.aspx?prjID={0}&id={1}\">{2}</a><br/>{3}",
                        Project.ID, TimeSpend.RelativeTask, HtmlUtility.GetText(task.Title, 45), TimeSpend.Note.HtmlEncode());
                else
                    return string.Format("<a href=\"tasks.aspx?prjID={0}&id={1}\">{2}</a>",
                        Project.ID, TimeSpend.RelativeTask, HtmlUtility.GetText(task.Title, 45));
            }
            else return TimeSpend.Note.HtmlEncode();
        }

        public string GetAction()
        {
            bool permission = false;
            if (Global.IsAdmin) permission = true;
            if (Global.EngineFactory.GetProjectEngine().GetByID(TimeSpend.Project).Responsible == SecurityContext.CurrentAccount.ID) permission = true;
            if (TimeSpend.Person == SecurityContext.CurrentAccount.ID) permission = true;

            if (permission)
            {
                StringBuilder sb = new StringBuilder();

                sb.AppendFormat("<img style='border:0px; cursor:pointer;' align='absmiddle' title='{0}' alt='{0}' src='{1}' onclick='ASC.Projects.TimeSpendActionPage.ViewEditTimeLogPanel({2})'/>", ProjectsCommonResource.Edit, WebImageSupplier.GetAbsoluteWebPath("edit.png", ProductEntryPoint.ID), TimeSpend.ID);
                sb.AppendFormat("<img style='border:0px; cursor:pointer; margin-left:4px' align='absmiddle' title='{0}' alt='{0}' src='{1}' onclick='ASC.Projects.TimeSpendActionPage.deleteTimeSpend({2})'/>", ProjectsCommonResource.Delete, WebImageSupplier.GetAbsoluteWebPath("trash.png", ProductEntryPoint.ID), TimeSpend.ID);

                return sb.ToString();
            }
            else
            {
                return string.Empty;
            }
        }

        public string GetTitle()
        {
            return string.Format("{0} {1}",
                TimeSpend.RelativeTask != 0 ? TaskResource.Task + ": " + Global.EngineFactory.GetTaskEngine().GetByID(TimeSpend.RelativeTask).Title + "; " : "",
                TimeSpend.Note == string.Empty ? "" : ProjectResource.ProjectDescription + ": " + TimeSpend.Note.HtmlEncode());
        }

        #endregion
    }
}