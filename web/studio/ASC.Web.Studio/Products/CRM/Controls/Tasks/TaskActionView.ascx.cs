using System;
using ASC.Core;
using System.Text;
using ASC.Core.Users;
using ASC.Data.Storage;
using ASC.Web.CRM.Classes;
using ASC.CRM.Core;
using ASC.Web.CRM.Controls.Common;
using ASC.Web.CRM.Resources;
using AjaxPro;
using System.Collections.Generic;

namespace ASC.Web.CRM.Controls.Tasks
{
    public partial class TaskActionView : BaseUserControl
    {
        #region Members

        public static string Location = "~/products/crm/controls/tasks/taskactionview.ascx";

        public bool HideContactSelector { get; set; }

        public List<UserInfo> UserList { get; set; }

        #endregion

        #region Events

        protected void Page_Load(object sender, EventArgs e)
        {
            Utility.RegisterTypeForAjax(typeof(ContactSelector));

            _taskContainer.Options.IsPopup = true;

            //init task responsible
            taskResponsibleSelector.SelectedUserId = SecurityContext.CurrentAccount.ID;
            taskResponsibleSelector.AdditionalFunction = "selectResponsible";
            if (UserList != null && UserList.Count > 0)
                taskResponsibleSelector.UserList = UserList;

            //init task categorySelector
            var cntrlCategorySelector = (CategorySelector)LoadControl(CategorySelector.Location);
            cntrlCategorySelector.Categories = Global.DaoFactory.GetListItemDao().GetItems(ListType.TaskCategory);
            cntrlCategorySelector.ID = "taskCategorySelector";
            phCategorySelector.Controls.Add(cntrlCategorySelector);

            //init task ContactSelector
            var selector = (ContactSelector)LoadControl(ContactSelector.Location);
            selector.CurrentType = ContactSelector.SelectorType.All;
            selector.SelectedContacts = null;
            selector.ShowChangeButton = true;
            selector.ShowContactImg = true;
            selector.ShowDeleteButton = false;
            selector.ShowNewCompanyContent = true;
            selector.ShowNewContactContent = true;
            selector.IsInPopup = true;
            selector.ID = "taskContactSelector";
            selector.DescriptionText = CRMCommonResource.FindContactByName;
            phContactSelector.Controls.Add(selector);
        }

        #endregion

        #region Methods

        protected string InitCategorySelect()
        {
            var sb = new StringBuilder();

            foreach(var category in Global.DaoFactory.GetListItemDao().GetItems(ListType.TaskCategory))
            {
                sb.AppendFormat("<option value='{0}' id='optCategory_{0}'>{1}</option>", category.ID, category.Title.HtmlEncode().ReplaceSingleQuote());
            }

            return sb.ToString();
        }

        protected string InitHoursSelect()
        {
            var sb = new StringBuilder();

            sb.Append("<option value='-1' id='optDeadlineHours_-1' selected='selected'>--</option>");

            for (int i = 0; i < 24; i++)
            {
                sb.AppendFormat("<option value='{0}' id='optDeadlineHours_{0}'>{1}</option>", i, i < 10 ? "0" + i.ToString() : i.ToString());
            }

            return sb.ToString();
        }

        protected string InitMinutesSelect()
        {
            var sb = new StringBuilder();

            sb.Append("<option value='-1' id='optDeadlineMinutes_-1' selected='selected'>--</option>");

            for (int i = 0; i < 60; i++)
            {
                sb.AppendFormat("<option value='{0}' id='optDeadlineMinutes_{0}'>{1}</option>", i, i < 10 ? "0" + i.ToString() : i.ToString());
            }

            return sb.ToString();
        }

        #endregion
    }
}