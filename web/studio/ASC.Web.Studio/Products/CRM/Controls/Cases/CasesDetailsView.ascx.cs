using System;
using ASC.Web.CRM.Configuration;
using ASC.Web.Core.Utility.Skins;
using AjaxPro;
using ASC.CRM.Core;
using ASC.Web.CRM.Controls.Common;
using ASC.Web.CRM.Controls.Tasks;
using ASC.Web.CRM.Resources;
using ASC.Web.CRM.Classes;
using ASC.Web.CRM.Controls.Contacts;
using ASC.Web.Studio.Controls.Common;
using ASC.Core;
using ASC.Core.Users;
using System.Linq;


namespace ASC.Web.CRM.Controls.Cases
{
    [AjaxNamespace("AjaxPro.CasesDetailsView")]
    public partial class CasesDetailsView : BaseUserControl
    {
        #region Properies

        public static string Location
        {
            get { return PathProvider.GetFileStaticRelativePath("Cases/CasesDetailsView.ascx"); }
        }

        public ASC.CRM.Core.Entities.Cases TargetCase { get; set; }

        #endregion

        #region Events

        protected void Page_Load(object sender, EventArgs e)
        {
            Utility.RegisterTypeForAjax(GetType());

            FilesTab.TabAnchorName = "files";
            FilesTab.TabName = CRMCommonResource.Documents;


            var tagViewControl = (TagView) LoadControl(TagView.Location);
            tagViewControl.Tags = Global.DaoFactory.GetTagDao().GetEntityTags(EntityType.Case, TargetCase.ID);
            tagViewControl.TargetEntityType = EntityType.Case;
            phTagContainer.Controls.Add(tagViewControl);

            var customFieldList = Global.DaoFactory.GetCustomFieldDao().GetEnityFields(EntityType.Case, TargetCase.ID, false);
            Page.JsonPublisher(customFieldList, "casesCustomFieldList");

            ExecHistoryView();
            ExecPeopleInCaseView();
            ExecTasksView();
            ExecFilesView();
        }

        #endregion

        #region Methods

        public void ExecPeopleInCaseView()
        {
            ContactsTab.TabAnchorName = "contacts";
            var people = Global.DaoFactory.GetContactDao().GetContacts(Global.DaoFactory.GetCasesDao().GetMembers(TargetCase.ID));
            //ContactsTab.TabName = people.Count > 0 ? String.Format("{0} ({1})", CRMCasesResource.PeopleInCase, people.Count) : CRMCasesResource.PeopleInCase;
            ContactsTab.TabName = CRMCasesResource.PeopleInCase;
            //init ListContactView
            var listContactView = (ListContactView)LoadControl(ListContactView.Location);
            listContactView.IsSimpleView = true;
            listContactView.EntityID = TargetCase.ID;
            listContactView.EntityType = EntityType.Case;
            _phContactsView.Controls.Add(listContactView);

            //init ContactSelector
            var selector = (ContactSelector) LoadControl(ContactSelector.Location);
            selector.CurrentType = ContactSelector.SelectorType.All;
            selector.SelectedContacts = people;
            selector.ShowOnlySelectorContent = true;
            selector.ShowNewCompanyContent = true;
            selector.ShowNewContactContent = true;
            selector.ID = "casesContactSelector";
            selector.DescriptionText = CRMCommonResource.FindContactByName;
            phContactSelector.Controls.Add(selector);


            //init EmptyScreen
            var emptyParticipantScreenControl = new EmptyScreenControl
                                                    {
                                                        ID = "emptyCaseParticipantPanel",
                                                        ImgSrc = WebImageSupplier.GetAbsoluteWebPath("empty_screen_case_participants.png", ProductEntryPoint.ID),
                                                        Header = CRMCasesResource.EmptyPeopleInCaseContent,
                                                        Describe = CRMCasesResource.EmptyPeopleInCaseDescript,
                                                        ButtonHTML = String.Format(@"
                                                            <a class='linkAddMediumText baseLinkAction' onclick='javascript:jq(""#caseParticipantPanel"").show();jq(""#emptyCaseParticipantPanel"").hide();'>{0}</a>",
                                                            CRMCommonResource.AddParticipant)
                                                    };
            _phEmptyPeopleView.Controls.Add(emptyParticipantScreenControl);
        }

        public void ExecHistoryView()
        {
            HistoryTab.TabName = CRMCommonResource.History;
            HistoryTab.TabAnchorName = "history";

            var historyViewControl = (HistoryView) LoadControl(HistoryView.Location);

            historyViewControl.TargetEntityType = EntityType.Case;
            historyViewControl.TargetEntityID = TargetCase.ID;
            historyViewControl.TargetContactID = 0;

            historyViewControl.Title = String.Format(CRMCasesResource.AddNoteToCase, TargetCase.Title.HtmlEncode());

            _phHistoryView.Controls.Add(historyViewControl);
        }

        public void ExecTasksView()
        {
            TasksTab.TabAnchorName = "tasks";
            TasksTab.TabName = CRMTaskResource.Tasks;

            var ctrlListTaskView = (ListTaskView)LoadControl(ListTaskView.Location);
            ctrlListTaskView.CurrentContact = null;
            ctrlListTaskView.CurrentEntityType = EntityType.Case;
            ctrlListTaskView.EntityID = TargetCase.ID;

            if (CRMSecurity.IsPrivate(TargetCase))
            {
                var users = CRMSecurity.GetAccessSubjectTo(TargetCase).Keys.ToList<Guid>();
                //with admins
                var admins = CoreContext.UserManager.GetUsersByGroup(Constants.GroupAdmin.ID).ToList();
                admins.AddRange(from u in users
                                where !CoreContext.UserManager.IsUserInGroup(u, Constants.GroupAdmin.ID)
                                select CoreContext.UserManager.GetUsers(u));
                ctrlListTaskView.UserList = admins.SortByUserName();
            }

            _phTasksView.Controls.Add(ctrlListTaskView);
        }

        public void ExecFilesView()
        {
            FilesTab.TabAnchorName = "files";
            FilesTab.TabName = CRMCommonResource.Documents;

            var filesViewControl = (Studio.UserControls.Common.Attachments.Attachments)LoadControl(Studio.UserControls.Common.Attachments.Attachments.Location);
            filesViewControl.EntityType = "case";
            filesViewControl.ModuleName = "crm";
            _phFilesView.Controls.Add(filesViewControl);
        }

        #endregion

        #region Ajax Methods

        [AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public string ChangeCaseStatus(int isClosed, Int32 caseID)
        {
            var tmpCase = Global.DaoFactory.GetCasesDao().GetByID(caseID);
            tmpCase.IsClosed = Convert.ToBoolean(isClosed);
            Global.DaoFactory.GetCasesDao().UpdateCases(tmpCase);
            return Convert.ToBoolean(isClosed) ? CRMCasesResource.CaseStatusClosed : CRMCasesResource.CaseStatusOpened;
        }

        #endregion

    }
}