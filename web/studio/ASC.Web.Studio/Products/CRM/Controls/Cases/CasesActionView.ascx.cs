using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using AjaxPro;
using ASC.CRM.Core.Entities;
using ASC.Data.Storage;
using ASC.Web.CRM.Classes;
using ASC.Web.CRM.Resources;
using ASC.CRM.Core;
using ASC.Web.CRM.Controls.Common;
using ASC.Core;
using ASC.Web.Core.Users.Activity;
using ASC.Web.Studio.Core.Users;

namespace ASC.Web.CRM.Controls.Cases
{
    public partial class CasesActionView : BaseUserControl
    {
        
        #region Properies

        public static string Location { get { return PathProvider.GetFileStaticRelativePath("Cases/CasesActionView.ascx"); } }

        public ASC.CRM.Core.Entities.Cases TargetCase { get; set; }

        protected bool HavePermission { get; set; }

        #endregion

        #region Events

        protected void Page_Load(object sender, EventArgs e)
        {
            Utility.RegisterTypeForAjax(typeof(ContactSelector));

            HavePermission = TargetCase == null ||
                             (CRMSecurity.IsAdmin || TargetCase.CreateBy == SecurityContext.CurrentAccount.ID);

            if (IsPostBack) return;
            
            List<CustomField> data;

            if (TargetCase != null)
            {
                data = Global.DaoFactory.GetCustomFieldDao().GetEnityFields(EntityType.Case, TargetCase.ID, true);
                saveCaseButton.Text = CRMCasesResource.SaveChanges;
                cancelButton.Attributes.Add("href", String.Format("cases.aspx?id={0}", TargetCase.ID));
                
            }
            else
            {
                data = Global.DaoFactory.GetCustomFieldDao().GetFieldsDescription(EntityType.Case);
                saveCaseButton.Text = CRMCasesResource.AddThisCaseButton;
                saveAndCreateCaseButton.Text = CRMCasesResource.AddThisAndCreateCaseButton;
                cancelButton.Attributes.Add("href",
                             Request.UrlReferrer != null
                                 ? Request.UrlReferrer.OriginalString
                                 : "cases.aspx");
            }

            Page.JsonPublisher(data, "casesEditCustomFieldList");

            InitContactSelector();
            InitPrivatePanel();
        }

        #endregion

        #region Methods

        public String GetCaseTitle()
        {
            return TargetCase == null ? String.Empty : TargetCase.Title.HtmlEncode();
        }

        protected void InitPrivatePanel()
        {
            var cntrlPrivatePanel = (PrivatePanel)LoadControl(PrivatePanel.Location);

            cntrlPrivatePanel.CheckBoxLabel = CRMCasesResource.PrivatePanelCheckBoxLabel;

            if (TargetCase != null)
            {
                cntrlPrivatePanel.IsPrivateItem = CRMSecurity.IsPrivate(TargetCase);
                if (cntrlPrivatePanel.IsPrivateItem)
                    cntrlPrivatePanel.SelectedUsers = CRMSecurity.GetAccessSubjectTo(TargetCase);
            }

            var usersWhoHasAccess = new List<string> { CustomNamingPeople.Substitute<CRMCommonResource>("CurrentUser").HtmlEncode() };

            cntrlPrivatePanel.UsersWhoHasAccess = usersWhoHasAccess;
            cntrlPrivatePanel.DisabledUsers = new List<Guid> { SecurityContext.CurrentAccount.ID };
            phPrivatePanel.Controls.Add(cntrlPrivatePanel);
        }

        protected void InitContactSelector()
        {
            var selector = (ContactSelector)LoadControl(ContactSelector.Location);
            selector.CurrentType = ContactSelector.SelectorType.All;

         

            if (TargetCase != null)
            {
                selector.SelectedContacts = Global.DaoFactory.GetContactDao().GetContacts(Global.DaoFactory.GetCasesDao().GetMembers(TargetCase.ID));
            }
            else
            {
                var URLContactID = UrlParameters.ContactID;
                if (URLContactID != 0)
                {
                    var target = Global.DaoFactory.GetContactDao().GetByID(URLContactID);
                    if (target != null)
                    {
                        selector.SelectedContacts = new List<Contact>{target};
                    }
                }
            }
            selector.ShowChangeButton = true;
            selector.ShowContactImg = true;
            selector.ShowDeleteButton = true;
            selector.ShowAddButton = true;
            selector.ShowAllDeleteButton = true;
            selector.ShowOnlySelectorContent = false;
            selector.ShowNewCompanyContent = true;
            selector.ShowNewContactContent = true;
            selector.ID = "casesContactSelector";
            selector.DescriptionText = CRMCommonResource.FindContactByName;
            selector.DeleteContactText = CRMCommonResource.DeleteParticipant;
            selector.AddButtonText = CRMCommonResource.AddParticipant;
            phContactSelector.Controls.Add(selector);
        }

        protected void SaveOrUpdateCase(Object sender, CommandEventArgs e)
        {
            int caseID;

            if (TargetCase != null)
            {
                caseID = TargetCase.ID;
                TargetCase.Title = Request["caseTitle"];
                Global.DaoFactory.GetCasesDao().UpdateCases(TargetCase);
                SetPermission(TargetCase);
                
                TimeLinePublisher.Cases(TargetCase, 
                                        CRMCommonResource.ActionText_Update,
                                        UserActivityConstants.ActivityActionType,
                                        UserActivityConstants.NormalActivity);
            }
            else
            {
                caseID = Global.DaoFactory.GetCasesDao().CreateCases(Request["caseTitle"]);
                var newCase = Global.DaoFactory.GetCasesDao().GetByID(caseID);
                SetPermission(newCase);

                TimeLinePublisher.Cases(newCase,
                                       CRMCommonResource.ActionText_Create,
                                       UserActivityConstants.ContentActionType,
                                       UserActivityConstants.ImportantContent);
            }


            Global.DaoFactory.GetCasesDao().SetMembers(caseID,
                                                       !String.IsNullOrEmpty(Request["memberID"])
                                                           ? Request["memberID"].Split(',').Select(
                                                               id => Convert.ToInt32(id)).ToArray()
                                                           : new List<int>().ToArray());


            foreach (var customField in Request.Form.AllKeys)
            {
                if (!customField.StartsWith("customField_")) continue;
                int fieldID = Convert.ToInt32(customField.Split('_')[1]);
                String fieldValue = Request.Form[customField];
               
                if (String.IsNullOrEmpty(fieldValue) && TargetCase == null)
                    continue;
               
                Global.DaoFactory.GetCustomFieldDao().SetFieldValue(EntityType.Case,caseID, fieldID, fieldValue);
            }

            Response.Redirect(String.Compare(e.CommandArgument.ToString(), "0", true) == 0
                                  ? String.Format("cases.aspx?id={0}", caseID)
                                  : "cases.aspx?action=manage");
        }

        protected void SetPermission(ASC.CRM.Core.Entities.Cases targetCase, bool isPrivate, string selectedUsers)
        {
            if (isPrivate)
            {
                var selectedUserList = selectedUsers
                    .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(item => new Guid(item)).ToList();

                CRMSecurity.MakePublic(targetCase);

                if (selectedUserList.Count > 0)
                    CRMSecurity.SetAccessTo(targetCase, selectedUserList);
            }
            else
            {
                CRMSecurity.MakePublic(targetCase);
            }
        }

        protected void SetPermission(ASC.CRM.Core.Entities.Cases caseItem)
        {
            if (CRMSecurity.IsAdmin || caseItem.CreateBy == SecurityContext.CurrentAccount.ID)
            {
                var isPrivate = Convert.ToBoolean(Request["isPrivateCase"]);
                var notifyPrivateUsers = Convert.ToBoolean(Request.Form["notifyPrivateUsers"]);

                if (isPrivate)
                {
                    var selectedUserList = Request["selectedUsersCase"]
                        .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(item => new Guid(item)).ToList();

                    selectedUserList.Add(ASC.Core.SecurityContext.CurrentAccount.ID);

                    CRMSecurity.SetAccessTo(caseItem, selectedUserList);

                    if (notifyPrivateUsers)
                        ASC.Web.CRM.Services.NotifyService.NotifyClient.Instance.SendAboutSetAccess(EntityType.Case, caseItem.ID, selectedUserList.ToArray());
                }
                else
                {
                    CRMSecurity.MakePublic(caseItem);
                }
            }
        }

        #endregion
    }
}