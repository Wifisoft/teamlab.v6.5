using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI.WebControls;
using ASC.Core;
using ASC.Core.Users;
using ASC.Web.Controls;
using ASC.Web.Core;
using ASC.Web.Core.Utility.Skins;
using ASC.Web.Studio.Controls.Common;
using ASC.Web.Studio.Utility;
using ASC.Web.Core.Users;
using ASC.Web.Studio.Core.Users;

namespace ASC.Web.Studio.UserControls.Company
{
    public enum EmployeeSortType
    {
        Name = 0,
        Sector,
        Role,
        Email,
        BirthDay,
        AdmissionDate
    }

    #region UrlQueryManager

    public class UrlQueryManager
    {
        public class AdditionParams
        {
            public string PageVirtualURL { get; set; }
            public List<string> Params { get; set; }

            public AdditionParams()
            {
                this.Params = new List<string>();
            }
        }

        public static List<AdditionParams> AdditionParamsCollection { get; set; }

        static UrlQueryManager()
        {
            AdditionParamsCollection = new List<AdditionParams>
                                           {
                                               new AdditionParams
                                                   {
                                                       PageVirtualURL = "~/messages.aspx",
                                                       Params = new List<string> {"pid", CommonLinkUtility.ParamName_ProductSysName}
                                                   },

                                               new AdditionParams
                                                   {
                                                       PageVirtualURL = "~/employee.aspx",
                                                       Params = new List<string> {"pid", CommonLinkUtility.ParamName_ProductSysName}
                                                   },

                                               new AdditionParams
                                                   {
                                                       PageVirtualURL = "~/management.aspx",
                                                       Params = new List<string> {"pid", "type", CommonLinkUtility.ParamName_ProductSysName}
                                                   },

                                               new AdditionParams
                                                   {
                                                       PageVirtualURL = "~/companystructure.aspx",
                                                       Params = new List<string> {"pid", CommonLinkUtility.ParamName_ProductSysName}
                                                   }


                                           };

        }

        public static string AddDefaultParameters(string relativePath, bool toAbsolute)
        {
            return HttpContext.Current == null ? relativePath : AddDefaultParameters(HttpContext.Current.Request, relativePath, toAbsolute);
        }

        /// <summary>
        /// The function reactor the url and add default parameters
        /// </summary>
        /// <param name="request">Request</param>
        /// <param name="relativePath">Relative or Absolute url with parameters</param>
        /// <param name="toAbsolute">Type of returned path</param>
        /// <returns>App Relative or Absolute path</returns>
        public static string AddDefaultParameters(HttpRequest request, string relativePath, bool toAbsolute)
        {
            if (String.IsNullOrEmpty(relativePath))
                return "";

            var pathPage = relativePath.Split('?')[0];
            var pathParams = (relativePath.IndexOf("?") > 0) ? relativePath.Split('?')[1] : "";

            var additionParams = AdditionParamsCollection.Find(param => String.Equals(param.PageVirtualURL, pathPage, StringComparison.InvariantCultureIgnoreCase));

            if (relativePath.IndexOf("~") == 0)
            {
                if (VirtualPathUtility.IsAbsolute(pathPage) && !toAbsolute)
                    pathPage = VirtualPathUtility.ToAppRelative(pathPage);

                else if (VirtualPathUtility.IsAppRelative(pathPage) && toAbsolute)
                    pathPage = VirtualPathUtility.ToAbsolute(pathPage);
            }

            var addParams = pathPage + "?";
            if (additionParams != null)
            {
                foreach (var param in additionParams.Params)
                {
                    if (request[param] != null)
                        addParams += string.Format("{0}={1}&", param, request[param]);
                }
            }

            if (!string.IsNullOrEmpty(pathParams))
                addParams += pathParams;

            return addParams.TrimEnd('?', '&');
        }


        /// <summary>
        /// The function reactor the url and add default parameters
        /// </summary>
        /// <param name="relativePath">Relative or Absolute url with parameters</param>
        /// <returns>App Relative path</returns>
        public static string AddDefaultParameters(HttpRequest request, string relativePath)
        {
            return AddDefaultParameters(request, relativePath, false);
        }
    }

    #endregion

    public partial class EmployeeViewer : System.Web.UI.UserControl
    {

        public static string Location
        {
            get { return "~/UserControls/Company/EmployeeViewer.ascx"; }
        }

        private string _HeadItemId = string.Empty;

        protected EmployeeSortType _sortType;

        protected bool _invenrt;

        public enum ViewType
        {
            Departments,
            Department,
            Employees
        }

        public ViewType WhatView
        {
            get
            {
                if (DepartmentList != null)
                {
                    if (DepartmentId != Guid.Empty) return ViewType.Department;
                    return ViewType.Departments;
                }

                return ViewType.Employees;
            }
        }

        protected bool _cardView = true;

        protected string _sortedImg
        {
            get { return Web.Core.Utility.Skins.WebImageSupplier.GetAbsoluteWebPath(_invenrt ? "sort_down.png" : "sort_up.png"); }
        }

        protected string _blankImg
        {
            get { return Web.Core.Utility.Skins.WebImageSupplier.GetAbsoluteWebPath("blank.gif"); }
        }

        protected string DepartmentList
        {
            get { return Request["deplist"]; }
        }

        protected EmployeeStatus EmployeeStatus
        {
            get
            {

                var eStat = Request["es"] ?? "";
                return eStat == "0" ? EmployeeStatus.Terminated : EmployeeStatus.Active;
            }
        }


        protected bool IsPending
        {
            get
            {

                var eStat = Request["eas"] ?? "";
                return eStat == "0";
            }
        }

        private string _strDepartmentIdStore = string.Empty;

        private string _strDepartmentId
        {
            get
            {
                if (!string.IsNullOrEmpty(_strDepartmentIdStore))
                {
                    return _strDepartmentIdStore;
                }

                var result = string.Empty;

                var depId = Guid.Empty;
                if (!string.IsNullOrEmpty(Request["depId"]))
                {
                    result = Request["depId"];
                }
                else if (!string.IsNullOrEmpty(DepartmentList))
                {
                    result = DepartmentList;
                }

                if (!string.IsNullOrEmpty(result))
                {
                    try
                    {
                        depId = new Guid(result);
                    }
                    catch
                    {
                    }

                    if (Guid.Empty.Equals(depId))
                    {
                        result = string.Empty;
                    }

                }
                _strDepartmentIdStore = result;
                return result;
            }
        }

        public Guid DepartmentId
        {
            get
            {
                var result = Guid.Empty;

                if (!string.IsNullOrEmpty(Request["deplist"]))
                {
                    try
                    {
                        result = new Guid(Request["deplist"]);
                    }
                    catch
                    {
                    }

                }

                return result;
            }
        }

        public GroupInfo Department
        {
            get { return Array.Find(CoreContext.UserManager.GetDepartments(), (d) => d.ID == DepartmentId); }
        }


        protected bool CanEditDepartment()
        {
            return SecurityContext.CheckPermissions(Constants.Action_EditGroups);
        }


        protected void Page_Load(object sender, EventArgs e)
        {
            //ListTab2.ImageUrl = WebImageSupplier.GetAbsoluteWebPath("employee_card_icon.png");
            //DepTab.ImageUrl = WebImageSupplier.GetAbsoluteWebPath("employee_department_icon.png");
            EmployeeTabs.DisableJavascriptSwitch = true;
            _sortType = EmployeeSortType.Name;

            string cookieViewMode = CookiesManager.GetCookies(CookiesType.EmployeeViewMode);
            if (Request["list"] != null)
            {
                if (String.Equals(Request["list"], "card", StringComparison.InvariantCultureIgnoreCase))
                    cookieViewMode = "card";
                else
                    cookieViewMode = "list";

                CookiesManager.SetCookies(CookiesType.EmployeeViewMode, cookieViewMode);
            }

            if (String.Equals(cookieViewMode, "card", StringComparison.InvariantCultureIgnoreCase))
                _cardView = true;

            else if (String.Equals(cookieViewMode, "list", StringComparison.InvariantCultureIgnoreCase))
                _cardView = false;

            _invenrt = !string.IsNullOrEmpty(Request["sort"]) && Request["sort"].Length > 3 && Request["sort"].Substring(0, 3).Equals("Inv", StringComparison.CurrentCultureIgnoreCase);

            try
            {
                var strSortType = (_invenrt ? Request["sort"].Substring(3) : Request["sort"]);
                _sortType = (EmployeeSortType)Enum.Parse(typeof(EmployeeSortType), strSortType, true);
            }
            catch
            {
            }



            if (!Page.IsPostBack)
            {
                DepTab.TabName = CustomNamingPeople.Substitute<Resources.Resource>("Departments");
                DepTab.OnClickText = GetTabNavigationUrl("DepTab");

                ListTab.TabName = CustomNamingPeople.Substitute<Resources.Resource>("Employees");
                ListTab.OnClickText = GetTabNavigationUrl("ListTab");

                if (DepartmentList != null)
                {
                    DepTab.IsSelected = true;
                    BindDepartments();
                }
                else
                {
                    ListTab.IsSelected = true;
                    BindEmployees();
                }

                BindBreadCrumbs();
            }
        }

        private string GetTabNavigationUrl(string tabID)
        {
            try
            {
                var relativeUrl = GetTabUrl(tabID);
                relativeUrl = string.Format("document.location.href='{0}';return false;", relativeUrl.TrimStart("~/".ToCharArray()));
                return relativeUrl;
            }
            catch
            {
                return string.Empty;
            }
        }


        #region private functions


        private void InsertCompany(List<BreadCrumb> result)
        {
            result.Insert(0, new BreadCrumb() { Caption = ASC.Web.Studio.Core.Users.CustomNamingPeople.Substitute<Resources.Resource>("Employees"), NavigationUrl = GetLinkByGuid(Guid.Empty) });
        }

        private void BindBreadCrumbs()
        {
            var result = new List<BreadCrumb>();

            if (EmployeeStatus == EmployeeStatus.Terminated)
            {
                InsertCompany(result);
                result.Add(new BreadCrumb { Caption = CustomNamingPeople.Substitute<Resources.Resource>("DisableEmployeesTitle") });

                EmployeeContainer.BreadCrumbs = result;
                Page.Title = HeaderStringHelper.GetPageTitle(CustomNamingPeople.Substitute<Resources.Resource>("Employees"), result);
                return;
            }

            if (IsPending)
            {
                InsertCompany(result);
                result.Add(new BreadCrumb { Caption = CustomNamingPeople.Substitute<Resources.Resource>("PendingEmployeesTitle") });

                EmployeeContainer.BreadCrumbs = result;
                Page.Title = HeaderStringHelper.GetPageTitle(CustomNamingPeople.Substitute<Resources.Resource>("Employees"), result);
                return;
            }


            GroupInfo group = null;
            if (!string.IsNullOrEmpty(_strDepartmentId))
            {
                try
                {
                    group = CoreContext.GroupManager.GetGroupInfo(new Guid(_strDepartmentId));
                }
                catch { }
            }

            if (DepartmentList == null)
            {
                if (string.IsNullOrEmpty(_strDepartmentId) && string.IsNullOrEmpty(Request["search"]))
                {

                    InsertCompany(result);

                }
                if (!string.IsNullOrEmpty(_strDepartmentId) && string.IsNullOrEmpty(Request["search"]))
                {

                    if (group == null)
                        PrepereStartState();

                    else
                    {
                        var grParent = group;
                        while (grParent.Parent != null)
                        {
                            result.Insert(0, new BreadCrumb { NavigationUrl = GetLinkByGuid(grParent.Parent.ID), Caption = grParent.Parent.Name });
                            grParent = grParent.Parent;
                        }

                        InsertCompany(result);
                        result.Add(new BreadCrumb { NavigationUrl = GetLinkByGuid(group.ID), Caption = group.Name });
                    }


                }
                else if (!string.IsNullOrEmpty(_strDepartmentId) && !string.IsNullOrEmpty(Request["search"]))
                {

                    if (group == null)
                        PrepereStartState();
                    else
                    {
                        result.Insert(0, new BreadCrumb { NavigationUrl = GetLinkByGuid(group.ID), Caption = group.Name });
                        while (group.Parent != null)
                        {
                            result.Insert(0, new BreadCrumb { NavigationUrl = GetLinkByGuid(group.Parent.ID), Caption = group.Parent.Name });
                            group = group.Parent;
                        }

                        InsertCompany(result);
                        result.Add(new BreadCrumb { NavigationUrl = GetLinkByGuid(Guid.Empty), Caption = string.Format("\"{0}\"", Request["search"]) });
                    }
                }
                else if (string.IsNullOrEmpty(_strDepartmentId) && !string.IsNullOrEmpty(Request["search"]))
                {

                    InsertCompany(result);
                    result.Add(new BreadCrumb { NavigationUrl = GetLinkByGuid(Guid.Empty), Caption = HeaderStringHelper.GetHTMLSearchHeader(Request["search"]) });
                }


            }
            else
            {
                result.Add(new BreadCrumb { NavigationUrl = GetDepartmentsLink(), Caption = CustomNamingPeople.Substitute<Resources.Resource>("Departments") });
                if (Department != null)
                {
                    result.Add(new BreadCrumb
                                   {
                                       NavigationUrl = GetDepartmentsLink(DepartmentId),
                                       Caption = Department.Name
                                   });
                }
            }

            if (result.Count > 0)
                EmployeeContainer.BreadCrumbs = result;

            Page.Title = HeaderStringHelper.GetPageTitle(CustomNamingPeople.Substitute<Resources.Resource>("Employees"), result);

        }

        protected UserInfo GetCeoUserInfo()
        {
            return CoreContext.UserManager.GetCompanyCEO();
        }

        protected void DeleteDepartment(object sender, EventArgs e)
        {
            try
            {
                var depID = (sender as LinkButton).CommandName;
                CoreContext.GroupManager.DeleteGroup(new Guid(depID));
                Response.Redirect(Request.GetUrlRewriter().ToString());
            }
            catch
            {
            }
        }

        protected void EmployeeTabs_TabSelected(object sender, string selectedTabId)
        {
            var pageUrl = GetTabUrl(selectedTabId);

            Response.Redirect(pageUrl);
        }

        private string GetTabUrl(string selectedTabId)
        {
            var pageUrl = string.Empty;

            if (selectedTabId == "ListTab")
            {
                pageUrl = string.Format("{0}?depID={1}&search={2}&page={3}&sort={4}{5}", Page.AppRelativeVirtualPath,
                                        _strDepartmentId, Request["search"], Request["page"],
                                        Request["sort"], Request["list"] == null ? string.Empty : "&list=" + Request["list"]);
            }
            if (selectedTabId == "DepTab")
            {
                pageUrl = string.Format("{0}?deplist={1}&search={2}&page={3}&sort={4}{5}", Page.AppRelativeVirtualPath,
                                        _strDepartmentId, Request["search"], Request["page"], Request["sort"], Request["list"] == null ? string.Empty : "&list=" + Request["list"]);
            }
            pageUrl = UrlQueryManager.AddDefaultParameters(Request, pageUrl);
            return pageUrl;
        }

        private void PrepereStartState()
        {
            Response.Redirect(GetLinkByGuid(Guid.Empty));
        }

        private string GetLinkByGuid(Guid id)
        {
            var pageUrl = string.Format("{0}?depID={1}&search=&page=1&sort={2}{3}", Page.AppRelativeVirtualPath,
                                        id == Guid.Empty ? string.Empty : id.ToString(),
                                        Request["sort"], Request["list"] == null ? string.Empty : "&list=" + Request["list"]);
            return UrlQueryManager.AddDefaultParameters(Request, pageUrl, true);
        }

        protected string GetDepartmentsLink()
        {
            return GetDepartmentsLink(Guid.Empty);
        }


        protected string GetDepartmentsLink(string depId)
        {
            var result = Guid.Empty;
            try
            {
                result = new Guid(depId);
            }
            catch
            {
            }

            return GetDepartmentsLink(result);

        }

        protected string GetDepartmentsLink(Guid depId)
        {
            var pageUrl = string.Format("{0}?deplist={1}{2}", Page.AppRelativeVirtualPath, depId.Equals(Guid.Empty) ? string.Empty : depId.ToString(),
                                        Request["list"] == null ? string.Empty : "&list=" + Request["list"]);
            return UrlQueryManager.AddDefaultParameters(Request, pageUrl, true);
        }

        private string GetLinkForPager()
        {
            var pageUrl = string.Format("{0}?depID={1}&search={2}&sort={3}{4}", Page.AppRelativeVirtualPath,
                                        _strDepartmentId, Request["search"],
                                        Request["sort"], Request["list"] == null ? string.Empty : "&list=" + Request["list"]);

            if (EmployeeStatus == EmployeeStatus.Terminated)
                pageUrl += "&es=0";

            if (IsPending)
                pageUrl += "&eas=0";

            return UrlQueryManager.AddDefaultParameters(Request, pageUrl, true);
        }


        private void BindDepartments()
        {
            if (DepartmentId.Equals(Guid.Empty))
            {
                rptDepartment.Visible = true;
                ucDepartmentEdit.Visible = false;

                var groups = new List<GroupInfo>();
                foreach (var g in CoreContext.GroupManager.GetGroups())
                {
                    AppendGroupsWithChild(groups, g);
                }

                groups.Sort((x, y) => string.Compare(x.Name, y.Name));

                rptDepartment.DataSource = groups;
                rptDepartment.DataBind();
            }
            else
            {
                rptDepartment.Visible = false;
                ucDepartmentEdit.Visible = true;
                ucDepartmentEdit.DepId = DepartmentId;
            }

        }

        private void AppendGroupsWithChild(List<GroupInfo> groups, GroupInfo group)
        {
            groups.Add(group);
            foreach (var g in group.Descendants)
                AppendGroupsWithChild(groups, g);
        }



        protected string GetGroupName(GroupInfo group)
        {
            return group.Name.HtmlEncode();
        }

        protected string GetMasterRenderLink(GroupInfo group)
        {
            var pid = CommonLinkUtility.GetProductID();
            var dephead = CoreContext.UserManager.GetUsers((CoreContext.UserManager.GetDepartmentManager(group.ID)));
            return dephead == null || dephead == Constants.LostUser ? "" : dephead.RenderProfileLink(pid);
        }

        protected string GetContainGroupCaption(GroupInfo group)
        {
            return string.Format(Resources.Resource.DepartmentHeadCount, CoreContext.UserManager.GetUsersByGroup(group.ID).Length);
        }

        protected string GetContainGroupUrl(GroupInfo group)
        {
            return GetLinkByGuid(group.ID);
        }

        protected string GetCardListSwUrl()
        {
            var pageUrl = GetCardViewLinkUrl(_cardView);

            return UrlQueryManager.AddDefaultParameters(Request, pageUrl, true);
        }

        private string GetCardViewLinkUrl(bool isCardView)
        {
            var pageUrl = string.Empty;

            if (_cardView)
            {
                pageUrl = string.Format("{0}?depID={1}&search={4}&page={3}&sort={2}&list=list", Page.AppRelativeVirtualPath,
                                        _strDepartmentId, Request["sort"], Request["page"], Request["search"]);

            }
            else
            {
                pageUrl = string.Format("{0}?depID={1}&search={4}&page={3}&sort={2}&list=card", Page.AppRelativeVirtualPath,
                                        _strDepartmentId, Request["sort"], Request["page"], Request["search"]);

            }

            if (EmployeeStatus == EmployeeStatus.Terminated)
                pageUrl += "&es=0";

            if (IsPending)
                pageUrl += "&eas=0";

            return pageUrl;
        }

        private string GetCardViewLinkRelativeUrl(bool isCardView)
        {
            var pageUrl = UrlQueryManager.AddDefaultParameters(Request, GetCardViewLinkUrl(isCardView));
            return pageUrl.TrimStart("~/".ToCharArray());
        }

        private void BindEmployees()
        {
            EmployeeTabs.SortItemsHeader = Resources.Resource.ViewAs;
            ListViewLink.SortLabel = Resources.Resource.EmployeesShowAsList;
            ListViewLink.SortUrl = GetCardViewLinkRelativeUrl(false);
            ListViewLink.IsSelected = !_cardView;

            CardViewLink.SortLabel = Resources.Resource.EmployeesShowAsCards;
            CardViewLink.SortUrl = GetCardViewLinkRelativeUrl(true);
            CardViewLink.IsSelected = _cardView;

            EmployeeList.Visible = true;
            EmployeeCardsPageNavigator.Visible = false;
            EmployeeCardsPageNavigator.VisiblePageCount = 5;

            var result = BindListEmployee();


            if (EmployeeList.Visible)
            {
                var cardsOnPage = 10;
                var rowsOnPage = 10;

                if (_cardView)
                    EmployeeCardsPageNavigator.EntryCountOnPage = cardsOnPage;
                else
                    EmployeeCardsPageNavigator.EntryCountOnPage = rowsOnPage;

                if (Request["onpage"] != null)
                    EmployeeCardsPageNavigator.EntryCountOnPage = Convert.ToInt32(Request["onpage"]);

                EmployeeCardsPageNavigator.Visible = true;
                EmployeeCardsPageNavigator.EntryCount = result.Count;
                EmployeeCardsPageNavigator.PageUrl = GetLinkForPager();

                try
                {
                    if (String.IsNullOrEmpty(Request["page"]))
                        EmployeeCardsPageNavigator.CurrentPageNumber = 1;
                    else
                        EmployeeCardsPageNavigator.CurrentPageNumber = Convert.ToInt32(Request["page"]);
                }
                catch
                {
                }

                if (EmployeeCardsPageNavigator.CurrentPageNumber < 1
                    || EmployeeCardsPageNavigator.CurrentPageNumber > result.Count / EmployeeCardsPageNavigator.EntryCountOnPage + 1)
                    Response.Redirect(Page.AppRelativeVirtualPath);


                var dataSource = new List<UserInfo>();
                for (var i = (EmployeeCardsPageNavigator.CurrentPageNumber - 1) * EmployeeCardsPageNavigator.EntryCountOnPage;
                     (i < EmployeeCardsPageNavigator.CurrentPageNumber * EmployeeCardsPageNavigator.EntryCountOnPage) && i < result.Count; i++)
                {
                    dataSource.Add(result[i]);
                }


                if (_cardView)
                {
                    rprCardEmployee.DataSource = dataSource;
                    rprCardEmployee.DataBind();
                }
                else
                {
                    rprListEmployee.DataSource = dataSource;
                    rprListEmployee.DataBind();
                }
            }
            if (result == null || result.Count == 0)
            {
                if (!string.IsNullOrEmpty(_strDepartmentId) && string.IsNullOrEmpty(Request["search"]))
                {
                    _empContentHolder.Controls.Clear();
                    _empContentHolder.Controls.Add(new NotFoundControl { Text = CustomNamingPeople.Substitute<Resources.Resource>("NotFoundEmployeesMessage") });
                }
                else
                {
                    EmployeeTabs.Visible = false;

                    _notFoundMessage.ImgSrc = WebImageSupplier.GetAbsoluteWebPath("empty_search.png");
                    _notFoundMessage.Header = CustomNamingPeople.Substitute<Resources.Resource>("NotFoundEmployeesMessage");
                    _notFoundMessage.Describe = Resources.Resource.SearchNotFoundDescript;
                    _notFoundMessage.Visible = true;
                }
            }
        }

        private List<UserInfo> BindListEmployee()
        {
            var result = new List<UserInfo>();
            var um = CoreContext.UserManager;

            GroupInfo group = null;
            if (!string.IsNullOrEmpty(_strDepartmentId))
            {
                try
                {
                    group = CoreContext.GroupManager.GetGroupInfo(new Guid(_strDepartmentId));
                }
                catch
                {
                }
            }

            if (EmployeeStatus == EmployeeStatus.Terminated)
            {
                result.AddRange(um.GetUsers(EmployeeStatus.Terminated));
            }
            else if (IsPending)
            {
                var users = new List<UserInfo>(um.GetUsers(EmployeeStatus.Active)).FindAll(u=> u.ActivationStatus == EmployeeActivationStatus.Pending);
                result.AddRange(users);
            }
            else if (string.IsNullOrEmpty(_strDepartmentId) && string.IsNullOrEmpty(Request["search"]))
            {
                result.AddRange(um.GetUsers(EmployeeStatus.Active));
            }
            else if (string.IsNullOrEmpty(_strDepartmentId) && !string.IsNullOrEmpty(Request["search"]))
            {
                result.AddRange(Search(Guid.Empty, Request["search"]));
            }
            else if (!string.IsNullOrEmpty(_strDepartmentId) && string.IsNullOrEmpty(Request["search"]))
            {
                if (group == null)
                    PrepereStartState();
                else
                    result.AddRange(CoreContext.UserManager.GetUsersByGroup(group.ID));

            }
            else if (!string.IsNullOrEmpty(_strDepartmentId) && !string.IsNullOrEmpty(Request["search"]))
            {
                if (group == null)
                    PrepereStartState();
                else
                    result.AddRange(Search(group.ID, Request["search"]));
            }


            if (_cardView)
                result = result.SortByUserName();
            else
                SortEmployees(ref result);

            return result;
        }

        private void SortEmployees(ref List<UserInfo> list)
        {
            switch (_sortType)
            {
                case EmployeeSortType.Name:
                    list = list.SortByUserName();
                    break;
                case EmployeeSortType.Sector:
                    list.Sort(new UserInfoComparer(UserSortOrder.Department));
                    break;
                case EmployeeSortType.Role:
                    list.Sort(new UserInfoComparer(UserSortOrder.Post));
                    break;
                case EmployeeSortType.Email:
                    list.Sort(new UserInfoComparer(UserSortOrder.Email));
                    break;
                case EmployeeSortType.BirthDay:
                    list.Sort(new UserInfoComparer(UserSortOrder.BirthDate));
                    break;
                case EmployeeSortType.AdmissionDate:
                    list.Sort(new UserInfoComparer(UserSortOrder.WorkFromDate));
                    break;
                default:
                    list = list.SortByUserName();
                    break;
            }

            if (_invenrt)
            {
                list.Reverse();
            }
        }

        private List<UserInfo> Search(Guid groupID, string expression)
        {
            var result = new List<UserInfo>(); // TODO Realize Search Engine Temp!!!
            if (groupID == Guid.Empty)
                result.AddRange(CoreContext.UserManager.Search(expression, EmployeeStatus.Active));
            else
                result.AddRange(CoreContext.UserManager.Search(expression, EmployeeStatus.Active, groupID));

            return result;
        }

        #endregion


        protected string GetEmployeeName(UserInfo ui)
        {
            return ui.DisplayUserName();
        }

        protected string GetEmployeeUrl(UserInfo ui)
        {
            return ui == null ? "" : CommonLinkUtility.GetUserProfile(ui.ID, CommonLinkUtility.GetProductID());
        }

        protected string GetEmployeeSectorUrl(UserInfo ui)
        {
            if (ui == null) return "";
            var pid = CommonLinkUtility.GetProductID();
            return CommonLinkUtility.GetUserDepartment(ui.ID, pid);
        }

        protected string GetSortUrl(EmployeeSortType type)
        {
            var pageUrl = string.Format("{0}?depID={1}&search={2}&page={3}&sort={4}{5}", Page.AppRelativeVirtualPath,
                                        _strDepartmentId, Request["search"], Request["page"],
                                        type.Equals(_sortType) && !_invenrt ? "Inv" + type.ToString() : type.ToString(),
                                        Request["list"] == null ? string.Empty : "&list=" + Request["list"]);


            if (EmployeeStatus == EmployeeStatus.Terminated)
                pageUrl += "&es=0";

            if (IsPending)
                pageUrl += "&eas=0";

            return UrlQueryManager.AddDefaultParameters(Request, pageUrl, true);
        }
    }
}