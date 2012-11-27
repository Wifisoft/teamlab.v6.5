#region Import

using System;
using System.Collections.Generic;
using System.Linq;
using AjaxPro;
using ASC.Core.Users;
using ASC.CRM.Core;
using ASC.CRM.Core.Entities;
using ASC.Web.Core.Utility.Skins;
using ASC.Web.CRM.Classes;
using ASC.Web.CRM.Configuration;
using ASC.Web.CRM.Resources;
using ASC.Web.Studio.Controls.Common;
using ASC.Core;
using Constants = ASC.Core.Users.Constants;


#endregion

namespace ASC.Web.CRM.Controls.Common
{
    [AjaxNamespace("AjaxPro.HistoryView")]
    public partial class HistoryView : BaseUserControl
    {
        #region Property

        public static String Location { get { return PathProvider.GetFileStaticRelativePath("Common/HistoryView.ascx"); } }

        public String Title { get; set; }

        public int TargetEntityID { get; set; }

        public EntityType TargetEntityType { get; set; }

        public int TargetContactID { get; set; }

        protected List<Deal> Deals { get; set; }

        public List<Contact> ContactMembers { get; set; }

        protected bool MobileVer = false;

        #endregion

        #region Methods

        #endregion

        #region Events

        private void Page_Load(object sender, EventArgs e)
        {
            MobileVer = ASC.Web.Core.Mobile.MobileDetector.IsRequestMatchesMobile(this.Context);
            
            Utility.RegisterTypeForAjax(GetType());

            var emptyScreenControl = new EmptyScreenControl
                                         {
                                             ID = "emptyContentForEventsFilter",
                                             ImgSrc = WebImageSupplier.GetAbsoluteWebPath("empty_screen_filter.png", ProductEntryPoint.ID),
                                             Header = CRMCommonResource.EmptyHistoryHeader,
                                             Describe = CRMCommonResource.EmptyHistoryDescription,
                                             ButtonHTML = String.Format(@"
                        <a class='crm-clearFilterButton' href='javascript:void(0);'
                            onclick='ASC.CRM.HistoryView.advansedFilter.advansedFilter(null);'>
                                {0}
                        </a>",
                        CRMCommonResource.ClearFilter)
            };
            _phEmptyContent.Controls.Add(emptyScreenControl);



            switch (TargetEntityType)
            {
                case EntityType.Opportunity:
                    ContactMembers = Global.DaoFactory.GetContactDao().GetContacts(Global.DaoFactory.GetDealDao().GetMembers(TargetEntityID));
                    break;
                case EntityType.Case:
                    ContactMembers = Global.DaoFactory.GetContactDao().GetContacts(Global.DaoFactory.GetCasesDao().GetMembers(TargetEntityID));
                    break;
            }

            var eventsCategories = Global.DaoFactory.GetListItemDao().GetItems(ListType.HistoryCategory);
            Page.ClientScript.RegisterClientScriptBlock(GetType(), "592b6142-cc8b-4c63-a866-c300e2af536e",
                                                        "eventsCategories = " +
                                                        JavaScriptSerializer.Serialize(eventsCategories.ConvertAll
                                                                                           (item => new
                                                                                                        {
                                                                                                            value = item.ID,
                                                                                                            title = item.Title.HtmlEncode()
                                                                                                        }
                                                                                           )) + "; ", true);

            if (TargetContactID != 0)
                Deals = Global.DaoFactory.GetDealDao().GetDeals(String.Empty, Guid.Empty, 0, null, TargetContactID, null,
                                                                true, DateTime.MinValue,
                                                                DateTime.MinValue, 0, 0,
                                                                new OrderBy(DealSortedByType.Title, true));
                    
                    


            var userSelector = (Studio.UserControls.Users.UserSelector) LoadControl(Studio.UserControls.Users.UserSelector.Location);
            userSelector.BehaviorID = "UserSelector";

            if (!MobileVer)
            {
                //init uploader
                var uploader = (FileUploader) LoadControl(FileUploader.Location);
                _phfileUploader.Controls.Add(uploader);
            }

            //init event categorySelector
            var cntrlCategorySelector = (CategorySelector) LoadControl(CategorySelector.Location);
            cntrlCategorySelector.Categories = Global.DaoFactory.GetListItemDao().GetItems(ListType.HistoryCategory);
            cntrlCategorySelector.ID = "eventCategorySelector";
            cntrlCategorySelector.MaxWidth = 130;
            phCategorySelector.Controls.Add(cntrlCategorySelector);

            var users = new List<Guid>();

            switch (TargetEntityType)
            {
                case EntityType.Contact:
                    var contact = Global.DaoFactory.GetContactDao().GetByID(TargetContactID);
                    if (CRMSecurity.IsPrivate(contact))
                        users = CRMSecurity.GetAccessSubjectTo(contact).Keys.ToList<Guid>();
                    break;
                case EntityType.Opportunity:
                    var deal = Global.DaoFactory.GetDealDao().GetByID(TargetEntityID);
                    if (CRMSecurity.IsPrivate(deal))
                        users = CRMSecurity.GetAccessSubjectTo(deal).Keys.ToList<Guid>();
                    break;
                case EntityType.Case:
                    var caseItem = Global.DaoFactory.GetCasesDao().GetByID(TargetEntityID);
                    if (CRMSecurity.IsPrivate(caseItem))
                        users = CRMSecurity.GetAccessSubjectTo(caseItem).Keys.ToList<Guid>();
                    break;
            }

            //init userSelectorListView
            var selector = (UserSelectorListView)LoadControl(UserSelectorListView.Location);
            if (users.Count > 0)
            {
                //with admins
                var admins = CoreContext.UserManager.GetUsersByGroup(Constants.GroupAdmin.ID).ToList();
                admins.AddRange(from u in users
                                where !CoreContext.UserManager.IsUserInGroup(u, Constants.GroupAdmin.ID)
                                select CoreContext.UserManager.GetUsers(u));
                selector.UserList = admins.SortByUserName();
                //without admins
                //selector.UserList = users.Select(u => CoreContext.UserManager.GetUsers(u)).ToList().SortByUserName();
            }
            selector.DisabledUsers = new List<Guid> {SecurityContext.CurrentAccount.ID};
            _phUserSelectorListView.Controls.Add(selector);
        }

        #endregion

        [AjaxMethod]
        public String GetEntityItems(int contactId)
        {
            var deals = Global.DaoFactory.GetDealDao().GetDeals(String.Empty, Guid.Empty, 0, null, contactId, null,
                                                        true, DateTime.MinValue,
                                                        DateTime.MinValue, 0, 0,
                                                        new OrderBy(DealSortedByType.Title, true));
            var cases = Global.DaoFactory.GetCasesDao().GetCases(String.Empty, contactId, null, null, 0, 0,
                                                        new OrderBy(SortedByType.Title, true));


            var data = deals.ConvertAll(row => new
                                        {
                                            id = row.ID,
                                            title = row.Title.HtmlEncode(),
                                            type = "opportunity"
                                        });


            data.AddRange(cases.ConvertAll(row => new
                                          {
                                              id = row.ID,
                                              title = row.Title.HtmlEncode(),
                                              type = "case"
                                          }));

            return JavaScriptSerializer.Serialize(data);
        }
    }
}