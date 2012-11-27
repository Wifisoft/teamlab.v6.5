#region Import

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ASC.Core.Tenants;
using ASC.Web.Controls;
using ASC.Web.CRM.Configuration;
using AjaxPro;
using ASC.CRM.Core;
using ASC.Web.CRM.Classes;
using ASC.CRM.Core.Entities;
using ASC.Web.CRM.Resources;
using ASC.Web.Core.Utility.Skins;
using System.Text;
using ASC.Web.Studio.Utility;
using ASC.Web.CRM.Controls.Common;
using System.Net.Mail;
using ASC.Web.CRM.Controls.Tasks;
using ASC.Web.Studio.Core.Users;
using ASC.Web.CRM.Controls.Settings;
using ASC.Common.Threading.Progress;
using Newtonsoft.Json.Linq;
using ASC.Core;
using ASC.Core.Users;

#endregion

namespace ASC.Web.CRM.Controls.Contacts
{
    [AjaxNamespace("AjaxPro.ListContactView")]
    public partial class ListContactView : BaseUserControl
    {
        #region Properies

        public static string Location
        {
            get { return PathProvider.GetFileStaticRelativePath("Contacts/ListContactView.ascx"); }
        }

        public bool IsSimpleView { get; set; }

        public int EntityID { get; set; }

        public EntityType EntityType { get; set; }

        protected FredCK.FCKeditorV2.FCKeditor Editor { get; set; }

        protected bool MobileVer = false;

        protected string CookieKeyForPagination
        {
            get { return "contactPageNumber"; }
        }

        protected int EntryCountOnPage { get; set; }
        protected int CurrentPageNumber { get; set; }

        protected string Anchor { get; set; }

        #endregion

        #region Events

        protected void Page_Load(object sender, EventArgs e)
        {
            MobileVer = ASC.Web.Core.Mobile.MobileDetector.IsRequestMatchesMobile(this.Context);

            if (UrlParameters.Action != "export")
            {
                InitPage();
                RegisterClientScript();
                if (!IsSimpleView)
                {
                    GetDataFromCookie();
                }
            }
            else
            {
                if(!CRMSecurity.IsAdmin)
                    Response.Redirect(PathProvider.StartURL());

                var contacts = GetContactsByFilter();

                if (UrlParameters.View != "editor")
                {
                    Response.Clear();
                    Response.ContentType = "text/csv; charset=utf-8";
                    Response.ContentEncoding = Encoding.UTF8;
                    Response.Charset = Encoding.UTF8.WebName;
                    var fileName = "contacts.csv";

                    Response.AppendHeader("Content-Disposition", String.Format("attachment; filename={0}", fileName)); Response.Write(ExportToCSV.ExportContactsToCSV(contacts, false));
                    Response.End();
                }
                else
                {
                    var fileUrl = ExportToCSV.ExportContactsToCSV(contacts, true);
                    Response.Redirect(fileUrl);
                }
            }
        }

        #endregion

        #region Methods

        private void InitPage()
        {
            Utility.RegisterTypeForAjax(typeof (AjaxProHelper));
            Utility.RegisterTypeForAjax(typeof (ListContactView));
            Utility.RegisterTypeForAjax(typeof (CommonSettingsView));

            if (IsSimpleView) return;

            //init popup
            _deletePanel.Options.IsPopup = true;
            _setPermissionsPanel.Options.IsPopup = true;
            _createLinkPanel.Options.IsPopup = true;
            _smtpSettingsPanel.Options.IsPopup = true;

            //init PrivatePanel
            var privatePanel = (PrivatePanel) LoadControl(PrivatePanel.Location);
            var usersWhoHasAccess = new List<string> {CustomNamingPeople.Substitute<CRMCommonResource>("CurrentUser").HtmlEncode()};
            privatePanel.UsersWhoHasAccess = usersWhoHasAccess;
            privatePanel.DisabledUsers = new List<Guid> { SecurityContext.CurrentAccount.ID };
            privatePanel.HideNotifyPanel = true;
            _phPrivatePanel.Controls.Add(privatePanel);

            //init taskAction
            var taskActionView = (TaskActionView) LoadControl(TaskActionView.Location);
            taskActionView.HideContactSelector = true;
            _phTaskActionView.Controls.Add(taskActionView);

            if (!MobileVer)
            {

                //init FCKeditor
                Editor = new FredCK.FCKeditorV2.FCKeditor
                             {
                                 ID = "_fcKeditor",
                                 Height = 400,
                                 BasePath = CommonControlsConfigurer.FCKEditorBasePath,
                                 EditorAreaCSS = WebSkin.GetUserSkin().BaseCSSFileAbsoluteWebPath
                             };
                _phFCKeditor.Controls.Add(Editor);

                //init uploader
                var uploader = (FileUploader) LoadControl(FileUploader.Location);
                _phfileUploader.Controls.Add(uploader);
            }

            InitEmptyScreens();

        }

        private void RegisterClientScript()
        {
            if (!IsSimpleView)
            {
                var listItems = Global.DaoFactory.GetListItemDao().GetItems(ListType.ContactStatus);
                var tags = Global.DaoFactory.GetTagDao().GetAllTags(EntityType.Contact).ToList();
                var admins = CoreContext.UserManager.GetUsersByGroup(Constants.GroupAdmin.ID).ToList();

                Page.ClientScript.RegisterClientScriptBlock(typeof(ListContactView), "84E6574D-58C1-4843-A113-A34B9B02ED48",
                                                            "contactTypes = "
                                                            + JavaScriptSerializer.Serialize(listItems.ConvertAll(item => new
                                                                                                                              {
                                                                                                                                  value = item.ID,
                                                                                                                                  title = item.Title,
                                                                                                                                  classname = "colorFilterItem color_" + item.Color.Replace("#", "").ToLower()
                                                                                                                              }))
                                                            + "; ", true);

                Page.ClientScript.RegisterClientScriptBlock(typeof(ListContactView), "FDFF4CDD-44FF-43fc-B4E0-F6BE0A05116F",
                                                            "contactTags = "
                                                            + JavaScriptSerializer.Serialize(tags.ConvertAll(item => new
                                                                                                                         {
                                                                                                                             value = item.HtmlEncode(),
                                                                                                                             title = item.HtmlEncode()
                                                                                                                         }))
                                                            + "; ", true);

                Page.ClientScript.RegisterClientScriptBlock(typeof(ListContactView), "A358AFBD-D076-4aaa-9C56-61019B896C9D",
                                                            "smtpSettings = "
                                                            + JavaScriptSerializer.Serialize(Global.TenantSettings.SMTPServerSetting)
                                                            + "; ", true);

                Page.ClientScript.RegisterClientScriptBlock(typeof(ListContactView), "6C1D8EBD-297A-4db7-A666-CC69FAB2230C",
                                                            "pageTitles = "
                                                            + JavaScriptSerializer.Serialize(new
                                                            {
                                                                contacts = CRMContactResource.Contacts,
                                                                composeMail = CRMCommonResource.ComposeMail,
                                                                previewMail = CRMCommonResource.PreviewMail,
                                                                mailSend = CRMCommonResource.MailSend
                                                            })
                                                            + "; ", true);

                Page.ClientScript.RegisterClientScriptBlock(typeof(ListContactView), "8119F554-8043-41b1-99E0-E15FD1A4B754",
                                                            "adminList = "
                                                            + JavaScriptSerializer.Serialize(admins.ConvertAll(item => new
                                                                                                                        {
                                                                                                                            avatarSmall = item.GetSmallPhotoURL(),
                                                                                                                            displayName = item.DisplayUserName(),
                                                                                                                            id = item.ID,
                                                                                                                            title = item.Title.HtmlEncode()
                                                                                                                        }))
                                                            + "; ", true);
            }
            else
            {
                Page.ClientScript.RegisterClientScriptBlock(typeof(ListContactView), "A358AFBD-D076-4aaa-9C56-61019B896C9D",
                                                            "entityData = "
                                                            + JavaScriptSerializer.Serialize(new
                                                                                                 {
                                                                                                     id = EntityID,
                                                                                                     type = EntityType.ToString().ToLower()
                                                                                                 })
                                                            + "; ", true);
            }
        }

        private void GetPageNumberAndCountOnPageFromCookie()
        {
            var cookieForPagination = Request.Cookies[CookieKeyForPagination];

            if (cookieForPagination != null)
            {
                var cookieForPaginationJson = System.Web.HttpUtility.UrlDecode(cookieForPagination.Value);
                try
                {
                    var paginationObj = JObject.Parse(cookieForPaginationJson);
                    CurrentPageNumber = paginationObj.Value<int>("page");
                    EntryCountOnPage = paginationObj.Value<int>("countOnPage");

                }
                catch (Exception)
                {
                    CurrentPageNumber = 1;
                    EntryCountOnPage = Global.EntryCountOnPage;

                }
            }
            else
            {
                CurrentPageNumber = 1;
                EntryCountOnPage = Global.EntryCountOnPage;
            }
        }

        private class FilterObject
        {
            public int StartIndex { get; set; }
            public int Count { get; set; }
            public string SortBy { get; set; }
            public string SortOrder { get; set; }
            public string FilterValue { get; set; }
            public List<string> Tags { get; set; }
            public string ContactListView { get; set; }
            public int ContactType { get; set; }

            public int CurrentPageNumber { get; set; }
            public FilterObject()
            { }
        };

        private FilterObject GetFilterObjectFromCookie()
        {
            GetPageNumberAndCountOnPageFromCookie();
            var result = new FilterObject
                 {
                     CurrentPageNumber = CurrentPageNumber,
                     Count = EntryCountOnPage,
                     StartIndex = CurrentPageNumber > 0 ? (CurrentPageNumber - 1) * EntryCountOnPage : 0
                 };

            var cookieKey = Request.Url.AbsoluteUri.Substring(0, Request.Url.AbsoluteUri.Length - Request.Url.Query.Length);

            var cookie = Request.Cookies[System.Web.HttpUtility.UrlEncode(cookieKey)];

            if (cookie != null && !String.IsNullOrEmpty(cookie.Value))
            {
                Anchor = cookie.Value;

                try
                {
                    var cookieJson = Encoding.UTF8.GetString(Convert.FromBase64String(HttpUtility.UrlDecode(Anchor)));
                    
                    var jsonArray = cookieJson.Split(';');

                    foreach (var filterItem in jsonArray)
                    {
                        var filterObj = JObject.Parse(filterItem);

                        var filterParam = JObject.Parse(Encoding.UTF8.GetString(Convert.FromBase64String(filterObj.Value<string>("params"))));

                        switch (filterObj.Value<string>("id"))
                        {
                            case "sorter":
                                result.SortBy = filterParam.Value<string>("id");
                                result.SortOrder = filterParam.Value<string>("sortOrder");
                                break;
                            case "text":
                                result.FilterValue = filterParam.Value<string>("value");
                                break;
                            case "tags":
                                result.Tags = new List<string>();
                                result.Tags = filterParam.Value<JArray>("value").ToList().ConvertAll(n => n.ToString());
                                break;
                            case "withopportunity":
                            case "person":
                            case "company":
                                result.ContactListView = filterParam.Value<string>("value");
                                break;
                            case "contactType":
                                result.ContactType = filterParam.Value<int>("value");
                                break;
                        }
                    }
                }
                catch (Exception)
                {
                    result.CurrentPageNumber = CurrentPageNumber;
                    result.Count = EntryCountOnPage;
                    result.StartIndex = CurrentPageNumber > 0 ? (CurrentPageNumber - 1) * EntryCountOnPage : 0;
                    result.SortBy = "displayname";
                    result.SortOrder = "ascending";
                }
            }
            else
            {
                result.CurrentPageNumber = CurrentPageNumber;
                result.Count = EntryCountOnPage;
                result.StartIndex = CurrentPageNumber > 0 ? (CurrentPageNumber - 1) * EntryCountOnPage : 0;
                result.SortBy = "displayname";
                result.SortOrder = "ascending";
                Anchor = String.Empty;
            }

            return result;
        }

        private void GetDataFromCookie()
        {
            //init data for first request
            var filterObj = GetFilterObjectFromCookie();

            var queryString = String.Format("startIndex={0}&count={1}", filterObj.StartIndex, filterObj.Count);

            queryString += String.Format("&sortBy={0}", filterObj.SortBy);
            queryString += String.Format("&sortOrder={0}", filterObj.SortOrder);
            queryString += !String.IsNullOrEmpty(filterObj.FilterValue) ? String.Format("&filterValue={0}", filterObj.FilterValue) : "";
            if (filterObj.Tags != null && filterObj.Tags.Count != 0)
            {
                queryString = filterObj.Tags.Aggregate(queryString,
                                                       (current, tag) => current + String.Format("&tags[]={0}", tag));
            }
            queryString += !String.IsNullOrEmpty(filterObj.ContactListView) ? String.Format("&contactListView={0}", filterObj.ContactListView) : "";
            queryString += filterObj.ContactType != 0 ? String.Format("&contactType={0}", filterObj.ContactType) : "";
                
            var apiServer = new Api.ApiServer();
            var contactsForFirstRequest = apiServer.GetApiResponse(
                String.Format("api/1.0/crm/contact/filter.json?{0}", queryString), "GET");

            Page.JsonPublisher(contactsForFirstRequest, "contactsForFirstRequest");

            //init Page Navigator
            _phPagerContent.Controls.Add(new PageNavigator
            {
                ID = "contactPageNavigator",
                CurrentPageNumber = filterObj.CurrentPageNumber,
                VisibleOnePage = false,
                EntryCount = 0,
                VisiblePageCount = Global.VisiblePageCount,
                EntryCountOnPage = filterObj.Count
            });
        }

        protected void InitEmptyScreens()
        {
            //init emptyScreen for filter
            emptyContentForContactsFilter.ImgSrc = WebImageSupplier.GetAbsoluteWebPath("empty_screen_filter.png", ProductEntryPoint.ID);
            emptyContentForContactsFilter.Header = CRMContactResource.EmptyContactListFilterHeader;
            emptyContentForContactsFilter.Describe = CRMContactResource.EmptyContactListFilterDescribe;
            emptyContentForContactsFilter.ButtonHTML = String.Format(@"<a class='crm-clearFilterButton' href='javascript:void(0);'
                                                onclick='ASC.CRM.ListContactView.advansedFilter.advansedFilter(null);'>{0}</a>",
                                                CRMCommonResource.ClearFilter);


            //init emptyScreen for all list
            var buttonHtml = String.Format(@"
                                            <a class='linkAddMediumText' href='default.aspx?action=manage'>{0}</a><br/>
                                            <a class='linkAddMediumText' href='default.aspx?action=manage&type=people'>{1}</a>",
                                           CRMContactResource.CreateFirstCompany,
                                           CRMContactResource.CreateFirstPerson);
            if (!MobileVer)
                buttonHtml += String.Format(@"<br/><a class='crm-importLink' href='default.aspx?action=import'>{0}</a>",
                                           CRMContactResource.ImportContacts);

            contactsEmptyScreen.ImgSrc = WebImageSupplier.GetAbsoluteWebPath("empty_screen_persons.png", ProductEntryPoint.ID);
            contactsEmptyScreen.Header = CRMContactResource.EmptyContactListHeader;
            contactsEmptyScreen.Describe = String.Format(CRMContactResource.EmptyContactListDescription,
                                                         //types
                                                         "<span class='hintTypes baseLinkAction' >", "</span>",
                                                         //csv
                                                         "<span class='hintCsv baseLinkAction' >", "</span>");
            contactsEmptyScreen.ButtonHTML = buttonHtml;
        }

        protected List<Contact> GetContactsByFilter()
        {
            var filterObj = GetFilterObjectFromCookie();

            var contactListViewType = ContactListViewType.All;
            if (!String.IsNullOrEmpty(filterObj.ContactListView))
            {
                if (filterObj.ContactListView == ContactListViewType.Company.ToString().ToLower())
                    contactListViewType = ContactListViewType.Company;
                if (filterObj.ContactListView == ContactListViewType.Person.ToString().ToLower())
                    contactListViewType = ContactListViewType.Person;
                if (filterObj.ContactListView == ContactListViewType.WithOpportunity.ToString().ToLower())
                    contactListViewType = ContactListViewType.WithOpportunity;
            }

            var sortBy = ContactSortedByType.DisplayName;
            if (!String.IsNullOrEmpty(filterObj.SortBy) && filterObj.SortBy == ContactSortedByType.ContactType.ToString().ToLower())
                sortBy = ContactSortedByType.ContactType;

            var isAsc = !String.IsNullOrEmpty(filterObj.SortOrder) && filterObj.SortOrder != "descending";

            return Global.DaoFactory.GetContactDao().GetContacts(
                                                            filterObj.FilterValue,
                                                            filterObj.Tags,
                                                            filterObj.ContactType,
                                                            contactListViewType,
                                                            0,
                                                            0,
                                                            new OrderBy(sortBy, isAsc));
        }

        protected string RenderTagSelector(bool isCompany)
        {
            var sb = new StringBuilder();
            var manager = new MailTemplateManager();
            var tags = manager.GetTags(isCompany);

            var current = tags[0].Category;
            sb.AppendFormat("<optgroup label='{0}'>", current);

            foreach (var tag in tags)
            {
                if (tag.Category != current)
                {
                    current = tag.Category;
                    sb.Append("</optgroup>");
                    sb.AppendFormat("<optgroup label='{0}'>", current);
                }

                sb.AppendFormat("<option value='{0}'>{1}</option>",
                                tag.Name,
                                tag.DisplayName);
            }

            sb.Append("</optgroup>");

            return sb.ToString();
        }

        #endregion

        #region AjaxMethods

        [AjaxMethod]
        public IProgressItem SendEmail(List<int> fileIDs, List<int> contactIds, String subjectTemplate, String bodyTemplate, bool storeInHistory)
        {
            return MailSender.Start(fileIDs, contactIds, subjectTemplate, bodyTemplate, storeInHistory);
        }

        [AjaxMethod]
        public IProgressItem GetStatus()
        {
            return MailSender.GetStatus();
        }

        [AjaxMethod]
        public IProgressItem Cancel()
        {
            MailSender.Cancel();

            return GetStatus();
        }

        [AjaxMethod]
        public string GetMessagePreview(string template, int contactId)
        {
            var manager = new MailTemplateManager();

            return manager.Apply(template, contactId);
        }

        #endregion

    }
}