using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using ASC.Core;
using ASC.Core.Billing;
using ASC.Core.Users;
using ASC.Data.Storage;
using ASC.Web.Core;
using ASC.Web.Core.ModuleManagement.Common;
using ASC.Web.Core.Utility;
using ASC.Web.Core.Utility.Skins;
using ASC.Web.Core.WebZones;
using ASC.Web.Studio.Controls.Common;
using ASC.Web.Studio.Core;
using ASC.Web.Studio.Core.SearchHandlers;
using ASC.Web.Studio.Utility;

namespace ASC.Web.Studio.UserControls.Common
{
    public partial class TopNavigationPanel : System.Web.UI.UserControl
    {
        public static string Location { get { return "~/UserControls/Common/TopNavigationPanel/TopNavigationPanel.ascx"; } }

        #region SearchHandlerItem
        private class SearchHandlerItem : IComparable<SearchHandlerItem>
        {
            public Guid ID { get; private set; }

            public ISearchHandlerEx Handler { get; set; }

            public bool Active { get; set; }

            public string LogoURL { get; set; }

            public SearchHandlerItem()
            {
                this.ID = Guid.NewGuid();
            }

            #region IComparable<SearchHandlerItem> Members

            public int CompareTo(SearchHandlerItem other)
            {
                if (other == null)
                    return -1;

                if (this.Handler != null && other.Handler != null)
                {
                    if (this.Handler.GetType().Equals(typeof(StudioSearchHandler)) && !other.Handler.GetType().Equals(typeof(StudioSearchHandler)))
                        return -1;
                    if (!this.Handler.GetType().Equals(typeof(StudioSearchHandler)) && other.Handler.GetType().Equals(typeof(StudioSearchHandler)))
                        return 1;

                    return String.Compare(this.Handler.SearchName, other.Handler.SearchName);
                }

                return 0;
            }

            #endregion
        }
        #endregion

        protected Guid _currentProductID;
        protected UserInfo CurrentUser;
        protected IProduct CurrentProduct;
        protected bool DisplayModuleList;


        //public bool FullSearch { get; set; }

        public bool DisableSearch { get; set; }
        public Type SingleSearchHandlerType { get; set; }
        public bool DisableProductNavigation { get; set; }
        public bool DisableTrialPeriod { get; set; }
        public List<NavigationItem> NavigationItems = new List<NavigationItem>();

        public string CustomTitle { get; set; }

        public string CustomTitleIconURL { get; set; }

        public string CustomInfoHTML { get; set; }

        public bool? DisableUserInfo { get; set; }

        public string CustomTitleURL { get; set; }

        private List<SearchHandlerItem> _handlerItems = new List<SearchHandlerItem>();

        private List<IWebItem> _customNavItems;

        protected bool _singleSearch;

        private IWebItem GetCurrentWebItem { get { return CommonLinkUtility.GetWebItemByUrl(Context.Request.Url.AbsoluteUri); } }

        private string GetAddonNameOrEmpty()
        {
            var item = GetCurrentWebItem;

            if (item == null)
                return Resources.Resource.SelectProduct;

            if (item.ID == new Guid("{32D24CB5-7ECE-4606-9C94-19216BA42086}") || item.ID == new Guid("{2A923037-8B2D-487b-9A22-5AC0918ACF3F}"))
                return item.Name;

            return Resources.Resource.SelectProduct;
        }

        //private string GetAddonNameOrEmptyClass()
        //{
        //    var item = GetCurrentWebItem;

        //    if (item == null)
        //        return "";

        //    if (item.ID == new Guid("{32D24CB5-7ECE-4606-9C94-19216BA42086}") || item.ID == new Guid("{2A923037-8B2D-487b-9A22-5AC0918ACF3F}"))
        //        return item.ProductClassName;

        //    return "";
        //}
        private void InitScripts()
        {
            Page.ClientScript.RegisterClientScriptInclude(this.GetType(), "topnavpanel_script", WebPath.GetPath("usercontrols/common/topnavigationpanel/js/topnavigator.js"));
            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "topnavpanel_style", "<link rel=\"stylesheet\" type=\"text/css\" href=\"" + WebSkin.GetUserSkin().GetAbsoluteWebPath("usercontrols/common/topnavigationpanel/css/<theme_folder>/topnavigationpanel.css") + "\">", false);

            if (!DisableSearch && !_currentProductID.Equals(Guid.Empty))
                RegisterSearchHandlers(false);

            if (_currentProductID.Equals(Guid.Empty))
                RegisterSearchHandlers(true);
        }

        private void RegisterSearchHandlers(bool fullSearch)
        {
            #region search scripts
            StringBuilder sb = new StringBuilder();

            string searchText = Page.Request["search"];
            var activeHandler = SearchHandlerManager.GetActiveHandlerEx();

            var allHandlers = SearchHandlerManager.GetHandlersExForProduct(_currentProductID);

            if (_currentProductID.Equals(Guid.Empty) || !allHandlers.Exists(sh => !sh.ProductID.Equals(Guid.Empty)))
            {
                if (!fullSearch)
                    allHandlers.RemoveAll(sh => sh is StudioSearchHandler);
                else
                    allHandlers.RemoveAll(sh => !(sh is StudioSearchHandler));

                _singleSearch = true;
            }

            if (SingleSearchHandlerType != null)
            {
                allHandlers.RemoveAll(sh => !sh.GetType().Equals(SingleSearchHandlerType));
                _singleSearch = true;
            }


            bool isFirst = true;
            foreach (var sh in allHandlers)
            {
                if (sh == null)
                    continue;

                var module = WebItemManager.Instance[sh.ModuleID];
                if (module != null && module.IsDisabled() )
                    continue;

                var shi = new SearchHandlerItem()
                {
                    Handler = sh,
                    LogoURL = (sh.Logo != null) ? WebImageSupplier.GetAbsoluteWebPath(sh.Logo.ImageFileName, sh.Logo.PartID) : "",
                    Active = String.IsNullOrEmpty(searchText) ? (sh.GetType().Equals(typeof(StudioSearchHandler)) || _singleSearch) :
                                                (sh.Equals(activeHandler) || (activeHandler == null && isFirst))
                };

                _handlerItems.Add(shi);



                string absoluteSearchURL = sh.AbsoluteSearchURL;

                if (sh.ProductID.Equals(Guid.Empty) && !this._currentProductID.Equals(Guid.Empty))
                    absoluteSearchURL = absoluteSearchURL + (absoluteSearchURL.IndexOf("?") != -1 ? "&" : "?") + CommonLinkUtility.GetProductParamsPair(this._currentProductID);


                sb.Append(" Searcher.AddHandler(new SearchHandler('" + shi.ID + "','" + sh.SearchName.Replace("'", "\\'") + "','" + shi.LogoURL + "'," + (shi.Active ? "true" : "false") + ",'" + absoluteSearchURL + "')); ");

                isFirst = false;
            }


            _handlerItems.Sort((h1, h2) =>
            {
                return h1.CompareTo(h2);
            });


            //script
            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "topnavpanel_init_script", sb.ToString(), true);
            #endregion
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.Page is Auth || this.Page is _Default)
                _currentProductID = Guid.Empty;
            else
                _currentProductID = CommonLinkUtility.GetProductID();

            CurrentProduct = ProductManager.Instance[_currentProductID];

            InitScripts();

            if (!DisableUserInfo.HasValue)
            {
                _guestInfoHolder.Visible =  !(Page is Auth) && CoreContext.TenantManager.GetCurrentTenant().Public && !SecurityContext.IsAuthenticated;
                _userInfoHolder.Visible = SecurityContext.IsAuthenticated && !(Page is Wizard);

            }
            else 
            {
                _guestInfoHolder.Visible = !DisableUserInfo.Value && !(Page is Auth) && CoreContext.TenantManager.GetCurrentTenant().Public && !SecurityContext.IsAuthenticated;
                _userInfoHolder.Visible = !DisableUserInfo.Value && SecurityContext.IsAuthenticated && !(Page is Wizard);
            }

            if (SecurityContext.IsAuthenticated)
                CurrentUser = CoreContext.UserManager.GetUsers(SecurityContext.CurrentAccount.ID);

            if (this.Page is Auth || this.Page is _Default)
                DisableProductNavigation = true;

            var tariff = CoreContext.TenantManager.GetTariff(CoreContext.TenantManager.GetCurrentTenant().TenantId);
            var tenantState = tariff.State;

            if ((this.Page is Auth)||((tenantState.Equals(TariffState.Trial)==false)&&(tenantState.Equals(TariffState.NotPaid)==false)))         
                DisableTrialPeriod = true;

            if(((tenantState.Equals(TariffState.Trial))&&(!IsAdministrator)))
            {
                DisableTrialPeriod = true;
            }
            _customNavItems = WebItemManager.Instance.GetItems(WebZoneType.CustomProductList);

            if (DisableProductNavigation)
                _productListHolder.Visible = false;
            else
            {

                var productsList = WebItemManager.Instance.GetItems(WebZoneType.TopNavigationProductList);
                DisplayModuleList = productsList.Any();
                _productRepeater.DataSource = productsList;
                _productRepeater.DataBind();

                var _addons = _customNavItems.Where(pr => ((pr.ID == new Guid("{32D24CB5-7ECE-4606-9C94-19216BA42086}") || pr.ID == new Guid("{2A923037-8B2D-487b-9A22-5AC0918ACF3F}"))));
                //if (GetCurrentWebItem != null)
                //    _addons = _addons.Where(pr => pr.ID != GetCurrentWebItem.ID);
                _addonRepeater.DataSource = _addons.ToList();
                _addonRepeater.DataBind();


                MoreProductsRepeater.DataBind();
            }

            DisableSearch = (DisableSearch || _handlerItems == null || _handlerItems.Count == 0);
            _searchHolder.Visible = !DisableSearch;

            if (NavigationItems.Count == 0)
                _navItemRepeater.Visible = false;
            else
            {
                _navItemRepeater.DataSource = NavigationItems;
                _navItemRepeater.DataBind();
            }

            if (String.IsNullOrEmpty(_title) && DisableSearch)
                _contentSectionHolder.Visible = false;

            foreach (var item in _customNavItems)
            {
                var render = WebItemManager.Instance[item.ID] as IRenderCustomNavigation;
                if (render != null)
                {
                    try
                    {
                        var control = render.LoadCustomNavigationControl(this.Page);
                        if (control != null)
                        {
                            _customNavControls.Controls.Add(control);
                        }
                    }
                    catch (Exception ex)
                    {
                        log4net.LogManager.GetLogger("ASC.Web.Studio").Error(ex);
                    }
                }
            }

            myToolsItemRepeater.DataSource = WebItemManager.Instance.GetItems(WebZoneType.MyTools).OfType<IRenderMyTools>();
            myToolsItemRepeater.DataBind();
        }

        protected string CurrentProductName
        {
            get
            {
                return
                    CurrentProduct == null
                        ? GetAddonNameOrEmpty()
                        : CurrentProduct.Name;
            }
        }

        //protected string CurrentProductClassName
        //{
        //    get
        //    {
        //        return
        //         CurrentProduct == null
        //                ? GetAddonNameOrEmptyClass()
        //                : CurrentProduct.ProductClassName;
        //    }
        //}
        protected bool CanLogout
        {
            get
            {
                return (SetupInfo.WorkMode != WorkMode.Promo);
            }
        }
        protected bool IsAdministrator
        {
            get
            {
                return SecurityContext.CheckPermissions(
                    ASC.Core.Users.Constants.Action_AddRemoveUser,
                    ASC.Core.Users.Constants.Action_EditUser,
                    ASC.Core.Users.Constants.Action_EditGroups);
            }
        }

        protected string _searchText
        {
            get { return (Page.Request["search"] ?? "").HtmlEncode().ReplaceSingleQuote(); }
        }

        protected string _searchLogoUrl
        {
            get
            {
                if (DisableSearch)
                    return "";

                var sh = _handlerItems.Find(h => h.Active);
                return sh != null ? sh.LogoURL : "";
            }
        }

        protected string _title
        {
            get
            {
                if (String.IsNullOrEmpty(this.CustomTitle))
                    return CurrentProduct != null ? CurrentProduct.Name : "";
                else
                    return this.CustomTitle;

            }
        }

        protected string _titleURL
        {
            get
            {
                if (String.IsNullOrEmpty(this.CustomTitleURL))
                    return CurrentProduct != null ? VirtualPathUtility.ToAbsolute(CurrentProduct.StartURL) : "";
                else
                    return this.CustomTitleURL;

            }
        }

        protected string _titleIconURL
        {
            get
            {
                if (String.IsNullOrEmpty(this.CustomTitleIconURL))
                    return CurrentProduct != null ? CurrentProduct.GetIconAbsoluteURL() : "";
                else
                    return this.CustomTitleIconURL;

            }
        }
        protected string SearchText
        {
            get { return (Page.Request["search"] ?? "").HtmlEncode(); }
        }
        protected string getNumDaysPeriod
        {
            get
            {
                var tariff = CoreContext.TenantManager.GetTariff(CoreContext.TenantManager.GetCurrentTenant().TenantId);
                var tenantState = tariff.State;
                var dueDate = tariff.DueDate;               
                DateTime nowDate = DateTime.Today;
                var count = dueDate.Subtract(nowDate.Date).Days;
                string end, text;

                switch (tenantState)
                {
                    case TariffState.Trial:
                        text = Resources.Resource.TrialPeriod;
                        break;
                    case TariffState.NotPaid:
                        text = Resources.Resource.DeactivationPeriod;
                        break;
                    default:
                        text = "";
                        break;
                }

                var num = count  %100;
                if (num >= 11 && num <= 19)
                {
                    end = Resources.Resource.DaysTwo;
                }
                else
                {
                    var i = count%10;
                    switch (i)
                    {
                        case (1):end = Resources.Resource.Day; break;
                        case (2):
                        case (3):
                        case (4): end = Resources.Resource.DaysOne; break;
                        default: end = Resources.Resource.DaysTwo;
                            break;
                    }
                }
                return text +": " + count + " " + end;
            }
        }
        protected string RenderCustomNavigation()
        {
            var sb = new StringBuilder();
            _customNavItems.Reverse();
            foreach (var item in _customNavItems)
            {
                var render = WebItemManager.Instance[item.ID] as IRenderCustomNavigation;
                if (render != null)
                {
                    string rendered = render.RenderCustomNavigation(Page);
                    if (!string.IsNullOrEmpty(rendered))
                    {
                        sb.Append(rendered);
                    }
                }
                else
                {
                    sb.Append("<li class=\"mailBox\" style=\"float:right;\">");
                    sb.Append("<a href=\"" + VirtualPathUtility.ToAbsolute(item.StartURL) + "\">" + item.Name.HtmlEncode() + "</a>");
                    sb.Append("</li>");
                }
            }

            return sb.ToString();
        }

        protected string RenderSearchHandlers()
        {
            if (DisableSearch)
                return "";

            var sb = new StringBuilder();
            foreach (var shi in _handlerItems)
            {
                sb.Append("<div id='studio_shItem_" + shi.ID + "' onclick=\"Searcher.SelectSearchPlace('" + shi.ID + "');\" class='" + (shi.Active ? "searchHandlerActiveItem" : "searchHandlerItem") + "'>");
                if (!String.IsNullOrEmpty(shi.LogoURL))
                    sb.Append("<img alt='' align='absmiddle' src='" + shi.LogoURL + "' style='margin-right:5px;'/>");

                sb.Append(shi.Handler.SearchName.HtmlEncode());
                sb.Append("</div>");
            }
            return sb.ToString();
        }

        #region Render Navigation Item

        protected string RenderNavigationItem(NavigationItem item)
        {
            var hasSubItems = (item.SubItems != null && item.SubItems.Count > 0);
            Guid popupId = Guid.NewGuid();
            var navItem = string.Format(@"
<a class='{0}' href='{1}' {2} {3}>                        
	<span>{4}</span>{5}{6}
</a>",
         item.Selected ? "selectedTab" : "tab",	//class
         hasSubItems ? "javascript:void(0);" : item.URL,	//Url
         item.RightAlign ? "style='float: right;'" : string.Empty,	//float right if item.RigthAlign
         hasSubItems ? string.Format(@"onclick=""PopupPanelHelper.ShowPanel(jq(this), '{0}', 'auto', true, event); return false;""", popupId) : string.Empty,
         HttpUtility.HtmlEncode(item.Name),	//item name
         string.IsNullOrEmpty(item.ModuleStatusIconFileName) ?			//ModuleStatusIconFileName
                                    string.Empty :
                                    string.Format(@"<img src='{0}' style='border: 0px solid White; margin-left: 7px; padding-top:-2px;'/>", item.ModuleStatusIconFileName),
         hasSubItems ? string.Format(@"<img src='{0}' style='border: 0px solid White; margin-left: 6px;'/>", WebImageSupplier.GetAbsoluteWebPath("tri.png")) : string.Empty			//Arrow icon
         );
            if (!hasSubItems)
            {
                return navItem;
            }
            StringBuilder popup = new StringBuilder(navItem);

            popup.AppendFormat("<div id='{0}' class='subMenuItemsPopupBox switchBox'>", popupId);
            foreach (var subItem in item.SubItems)
            {
                popup.AppendFormat("<a href='{0}' class='subMenuItemsPopupBoxItem'>{1}</a>", subItem.URL, HttpUtility.HtmlEncode(subItem.Name));
            }
            popup.Append("</div>");
            return popup.ToString();
        }
        #endregion

        protected string RenderDebugInfo()
        {
            return
                DebugInfo.ShowDebugInfo
                    ? string.Format("<li><a class=\"dropdown-item\" href=\"javascript:alert('{0}')\" >Debug Info</a></li>",
                                    DebugInfo.DebugString.HtmlEncode().ReplaceSingleQuote())
                    : string.Empty;
        }

    }
}