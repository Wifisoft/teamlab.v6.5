using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Linq;
using ASC.Web.Core.Utility.Skins;
using ASC.Web.CRM.Configuration;
using ASC.Web.Studio.Utility;
using AjaxPro;
using ASC.CRM.Core;
using ASC.CRM.Core.Entities;
using ASC.Web.CRM.Classes;
using ASC.Web.CRM.Controls.Common;
using ASC.Web.CRM.Controls.Deals;
using ASC.Web.CRM.Controls.Tasks;
using ASC.Web.CRM.Resources;
using ASC.Web.Studio.Controls.Common;
using ASC.Core;
using ASC.Core.Users;

namespace ASC.Web.CRM.Controls.Contacts
{
    [AjaxNamespace("AjaxPro.ContactDetailsView")]
    public partial class ContactDetailsView : BaseUserControl
    {
        #region Properies

        public static string Location
        {
            get { return PathProvider.GetFileStaticRelativePath("Contacts/ContactDetailsView.ascx"); }
        }

        public Contact TargetContact { get; set; }

        protected List<Contact> ContactsToMerge { get; set; }

        protected bool ShowEventLinkToPanel
        {
            get
            {
                var dealsCount = Global.DaoFactory.GetDealDao().GetDealsCount();
                var casesCount = Global.DaoFactory.GetCasesDao().GetCasesCount();

                return dealsCount + casesCount > 0;
            }
        }

        protected bool MobileVer = false;

        #endregion

        #region Events

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            SocialMediaTab.TabName = "Twitter";
            SocialMediaTab.TabAnchorName = "twitter";
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            MobileVer = ASC.Web.Core.Mobile.MobileDetector.IsRequestMatchesMobile(this.Context);

            Utility.RegisterTypeForAjax(GetType());
            Utility.RegisterTypeForAjax(typeof (AjaxProHelper));

            _mergePanelPopup.Options.IsPopup = true;

            List<CustomField> data;

            if (TargetContact is Company)
                data = Global.DaoFactory.GetCustomFieldDao().GetEnityFields(EntityType.Company, Convert.ToInt32(UrlParameters.ID), false);
            else
                data = Global.DaoFactory.GetCustomFieldDao().GetEnityFields(EntityType.Person, Convert.ToInt32(UrlParameters.ID), false);

            var networks =
                Global.DaoFactory.GetContactInfoDao().GetList(TargetContact.ID, null, null, null).ConvertAll(
                    n => new
                             {
                                 Data = n.Data.HtmlEncode(),
                                 InfoType = n.InfoType,
                                 IsPrimary = n.IsPrimary,
                                 CategoryName = n.CategoryToString(),
                                 InfoTypeLocalName = n.InfoType.ToLocalizedString()
                             });

            InitTagControl();
            ExecHistoryView(TargetContact.ID);
            ExecTasksView();
            ExecFilesView();
            ExecDealsView(TargetContact.ID);

            if (TargetContact is Company)
                ExecPeopleContainerView(TargetContact.ID);
            else
                ContactsTab.Visible = false;

            ContactsToMerge = Global.DaoFactory.GetContactDao().GetContactsByPrefix(TargetContact.GetTitle(), 0, 0, 30).Where(n => n.ID != TargetContact.ID).ToList();
            InitMergeSelector();

            String json;
            using (var stream = new MemoryStream())
            {
                var serializer = new DataContractJsonSerializer(data.GetType());
                serializer.WriteObject(stream, data);
                json = Encoding.UTF8.GetString(stream.ToArray());
            }

            var listItems = Global.DaoFactory.GetListItemDao().GetItems(ListType.ContactStatus);


            var script =
                String.Format(
                    @"<script type='text/javascript'>
                                        var customFieldList = {0};
                                        var contactNetworks = {1};
                                        var sliderListItems = {2};
                                        var imgExst = {3};
                    </script>",
                    json,
                    JavaScriptSerializer.Serialize(networks),
                    JavaScriptSerializer.Serialize(new
                                                       {
                                                           ID = TargetContact.ID,
                                                           Status = TargetContact.StatusID,
                                                           PositionsCount = listItems.Count,
                                                           Items = listItems
                                                       }),
                    JavaScriptSerializer.Serialize(FileUtility.ImageExts)
                    );

            Page.ClientScript.RegisterStartupScript(typeof (ContactDetailsView), Guid.NewGuid().ToString(), script);

            _ctrlLoadPhotoContainer.Options.IsPopup = true;
            _ctrlImgDefaultAvatarSmall.ImageUrl = ContactPhotoManager.GetMediumSizePhoto(0, (TargetContact is Company));

        }

        #endregion

        #region Methods

        protected void ExecDealsView(int contactID)
        {
            DealsTab.TabAnchorName = "deals";
            DealsTab.TabName = CRMDealResource.Deals;

            var listDealViewControl = (ListDealView)LoadControl(ListDealView.Location);
            listDealViewControl.contactID = contactID;

            _phDealsView.Controls.Add(listDealViewControl);
        }

        protected void ExecPeopleContainerView(int companyID)
        {
            ContactsTab.TabAnchorName = "contacts";
            var people = Global.DaoFactory.GetContactDao().GetMembers(companyID);
            //ContactsTab.TabName = people.Count > 0 ? String.Format("{0} ({1})", CRMContactResource.Persons, people.Count) : CRMContactResource.Persons;
            ContactsTab.TabName = CRMContactResource.Persons;
            //init ListContactView
            var listContactView = (ListContactView)LoadControl(ListContactView.Location);
            listContactView.IsSimpleView = true;
            listContactView.EntityID = companyID;
            listContactView.EntityType = EntityType.Company;
            _phPeopleInCompany.Controls.Add(listContactView);

            //init ContactSelector
            var selector = (ContactSelector) LoadControl(ContactSelector.Location);
            selector.CurrentType = ContactSelector.SelectorType.PersonsWithoutCompany;
            selector.SelectedContacts = people;
            selector.ShowOnlySelectorContent = true;
            selector.ShowNewContactContent = true;
            selector.ID = "contactSelector";
            selector.DescriptionText = CRMContactResource.FindContactByName;
            phPeopleInCompanySelector.Controls.Add(selector);

            //init EmptyScreen
            var emptyParticipantScreenControl = new EmptyScreenControl
                                                    {
                                                        ID = "emptyPeopleInCompanyPanel",
                                                        ImgSrc = WebImageSupplier.GetAbsoluteWebPath("empty_screen_company_participants.png", ProductEntryPoint.ID),
                                                        Header = CRMContactResource.EmptyContentPeopleHeader,
                                                        Describe = CRMContactResource.EmptyContentPeopleDescribe,
                                                        ButtonHTML = String.Format(@"
                                                            <a class='linkAssignContacts baseLinkAction' onclick='javascript:jq(""#peopleInCompanyPanel"").show();jq(""#emptyPeopleInCompanyPanel"").hide();'>{0}</a>",
                                                                               CRMContactResource.AssignContact)
                                                    };
            _phEmptyPeopleInCompany.Controls.Add(emptyParticipantScreenControl);

        }

        protected void ExecHistoryView(int contactID)
        {
            EventsTab.TabName = CRMCommonResource.History;
            EventsTab.TabAnchorName = "history";

            var historyViewControl = (HistoryView) LoadControl(HistoryView.Location);

            historyViewControl.TargetContactID = contactID;
            historyViewControl.TargetEntityID = 0;
            historyViewControl.TargetEntityType = EntityType.Contact;

            historyViewControl.Title = String.Format(CRMContactResource.AddNoteToContact,
                                                     Global.DaoFactory.GetContactDao().GetByID(contactID).GetTitle().HtmlEncode());

            _phHistoryView.Controls.Add(historyViewControl);
        }

        protected void ExecFilesView()
        {
            FilesTab.TabAnchorName = "files";
            FilesTab.TabName = CRMCommonResource.Documents;

            var filesViewControl = (Studio.UserControls.Common.Attachments.Attachments)LoadControl(Studio.UserControls.Common.Attachments.Attachments.Location);
            filesViewControl.EntityType = "contact";
            filesViewControl.ModuleName = "crm";
            _phFilesView.Controls.Add(filesViewControl);
        }

        protected void ExecTasksView()
        {
            TasksTab.TabAnchorName = "tasks";
            TasksTab.TabName = CRMTaskResource.Tasks;

            var ctrlListTaskView = (ListTaskView)LoadControl(ListTaskView.Location);
            ctrlListTaskView.CurrentEntityType = EntityType.Contact;
            ctrlListTaskView.CurrentContact = TargetContact;
            ctrlListTaskView.HideContactSelector = true;

            if (CRMSecurity.IsPrivate(TargetContact))
            {
                var users = CRMSecurity.GetAccessSubjectTo(TargetContact).Keys.ToList<Guid>();
                //with admins
                var admins = CoreContext.UserManager.GetUsersByGroup(Constants.GroupAdmin.ID).ToList();
                admins.AddRange(from u in users
                                where !CoreContext.UserManager.IsUserInGroup(u, Constants.GroupAdmin.ID)
                                select CoreContext.UserManager.GetUsers(u));
                ctrlListTaskView.UserList = admins.SortByUserName();
            }

            _phTasksView.Controls.Add(ctrlListTaskView);
        }

        protected void InitTagControl()
        {
            var tagViewControl = (TagView) LoadControl(TagView.Location);
            tagViewControl.Tags = Global.DaoFactory.GetTagDao().GetEntityTags(EntityType.Contact, TargetContact.ID);
            tagViewControl.TargetEntityType = EntityType.Contact;

            phTagContainer.Controls.Add(tagViewControl);
        }

        protected void InitMergeSelector()
        {
            var cntrlContactSelector = (ContactSelector) LoadControl(ContactSelector.Location);
            cntrlContactSelector.CurrentType = TargetContact is Company ? ContactSelector.SelectorType.Companies : ContactSelector.SelectorType.Persons;
            cntrlContactSelector.DescriptionText = TargetContact is Company ? CRMContactResource.FindCompanyByName : CRMContactResource.FindEmployeeByName;
            cntrlContactSelector.ID = "contactToMergeSelector";
            cntrlContactSelector.ShowContactImg = true;
            cntrlContactSelector.ShowChangeButton = true;
            cntrlContactSelector.ShowDeleteButton = false;
            cntrlContactSelector.ShowOnlySelectorContent = false;
            cntrlContactSelector.IsInPopup = true;
            cntrlContactSelector.ExcludedArrayIDs = new List<Int32> {TargetContact.ID};
            _phContactToMergeSelector.Controls.Add(cntrlContactSelector);
        }

        #endregion

        #region Ajax Methods

        [AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public int ChangeStatus(int id, int statusValue)
        {
            var contact = Global.DaoFactory.GetContactDao().GetByID(id);

            contact.StatusID = statusValue;

            Global.DaoFactory.GetContactDao().UpdateContact(contact);

            return contact.StatusID;

        }

        [AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public String MergeContacts(int fromID, int toID, bool isCompany)
        {
            Global.DaoFactory.GetContactDao().MergeDublicate(fromID, toID);
            var urlType = isCompany ? "" : String.Format("&{0}=people", UrlConstant.Type);

            return String.Format("default.aspx?{0}={1}{2}", UrlConstant.ID, toID, urlType);
        }

        #endregion
    }
}