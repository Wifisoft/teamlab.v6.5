using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using AjaxPro;
using ASC.Web.Controls;
using ASC.Web.Core;
using ASC.Web.Core.ModuleManagement.Common;
using ASC.Web.Core.Utility;
using ASC.Web.Core.Utility.Skins;
using ASC.Web.Studio.Controls.Common;
using ASC.Web.Studio.Core.Search;
using ASC.Web.Studio.Core.SearchHandlers;
using ASC.Web.Studio.Masters;
using ASC.Web.Studio.UserControls.Common.Search;
using ASC.Web.Studio.Utility;


namespace ASC.Web.Studio
{
    [AjaxNamespace("SearchController")]
    public partial class Search : MainPage
    {
        protected string _searchText;

        private Guid CommunityID
        {
            get { return new Guid("EA942538-E68E-4907-9394-035336EE0BA8"); }
        }

        private Guid EmployeeID
        {
            get { return new Guid("86d29ee3-5e8b-4835-88fd-02b463de146f"); }
        }

        protected override void OnPreInit(EventArgs e)
        {
            base.OnPreInit(e);
            SetProductMasterPage();
        }


        protected void Page_Load(object sender, EventArgs e)
        {
            var master = Master as IStudioMaster;
            if (master == null) return;
            
            AjaxPro.Utility.RegisterTypeForAjax(this.GetType());
            
            //top navigator
            if (master is StudioTemplate)
            {
                (master as StudioTemplate).TopNavigationPanel.CustomTitle = Resources.Resource.Search;
                (master as StudioTemplate).TopNavigationPanel.CustomTitleIconURL = WebImageSupplier.GetAbsoluteWebPath("search.png");
            }

            master.DisabledSidePanel = true;
            
            Guid productID;
            if (!String.IsNullOrEmpty(Request["productID"]))
                productID = new Guid(Request["productID"]);
            else
                productID = GetProductID();
            


            _searchText = Request["search"] ?? "";
            var data = SearchAll(_searchText, productID);

            var container = new Container {Body = new PlaceHolder(), Header = new PlaceHolder()};
            container.BreadCrumbs.Add(new BreadCrumb {Caption = Resources.Resource.MainTitle, NavigationUrl = productID.Equals(Guid.Empty) ? CommonLinkUtility.GetDefault() : VirtualPathUtility.ToAbsolute(ProductManager.Instance[productID].StartURL)});
            container.BreadCrumbs.Add(new BreadCrumb {Caption = HeaderStringHelper.GetHTMLSearchHeader(_searchText)});
            master.ContentHolder.Controls.Add(container);

            Title = HeaderStringHelper.GetPageTitle(Resources.Resource.Search, container.BreadCrumbs);

            if (data.Count <= 0)
            {
                var emptyScreenControl=new EmptyScreenControl
                {
                    ImgSrc = WebImageSupplier.GetAbsoluteWebPath("empty_search.png"),
                    Header = Resources.Resource.SearchNotFoundMessage,
                    Describe = Resources.Resource.SearchNotFoundDescript
                };
                container.Body.Controls.Add(emptyScreenControl);
            }
            else
            {
                var oSearchView = (SearchResults) LoadControl(SearchResults.Location);
                //data.Sort(new SearchComparer());
                oSearchView.DataSourceObj = data;
                container.Body.Controls.Add(oSearchView);
            }
        }

        private List<SearchResult> SearchAll(string text, Guid productID)
        {
            var searchResult = SearchByModules(productID, text);

            if (!productID.Equals(Guid.Empty) && !productID.Equals(CommunityID))
            {
                if (searchResult.Count > 1)
                    searchResult.Sort(new SearchComparer());
                return searchResult;
            }

            var groupedData = GroupDataModules(searchResult);

            foreach (var result in groupedData)
                result.Items.Sort(new DateSearchComparer());
            groupedData.Sort(new SearchComparer());

            return groupedData;
        }

        private List<SearchResult> GroupDataModules(List<SearchResult> data)
        {
            var guids = new List<string>();

            // get all modules ids
            foreach (var searchResult in data)
                if (!guids.Contains(searchResult.ProductID.ToString()))
                    guids.Add(searchResult.ProductID.ToString());

            if (guids.Count == 0)
                return data;

            var groupedData = new List<SearchResult>();

            foreach (var productID in guids)
            {
                foreach (var searchResult in data)
                {
                    SearchResult item;
                    var g = new Guid(productID);

                    if (searchResult.ProductID == g)
                    {
                        item = GetContainer(groupedData, g);

                        foreach (var searchResultItem in searchResult.Items)
                        {
                            if (searchResultItem.Additional == null)
                                searchResultItem.Additional = new Dictionary<string, object>();

                            if (!searchResultItem.Additional.ContainsKey("imageRef"))
                                searchResultItem.Additional.Add("imageRef", searchResult.LogoURL);
                            if (!searchResultItem.Additional.ContainsKey("Hint"))
                                searchResultItem.Additional.Add("Hint", searchResult.Name);
                        }
                        item.Items.AddRange(searchResult.Items);

                        if (item.PresentationControl == null) item.PresentationControl = searchResult.PresentationControl;
                        if (String.IsNullOrEmpty(item.MoreURL)) item.MoreURL = searchResult.MoreURL;
                        if (String.IsNullOrEmpty(item.LogoURL)) item.LogoURL = searchResult.LogoURL;
                        if (String.IsNullOrEmpty(item.Name)) item.Name = searchResult.Name;
                    }
                }
            }

            return groupedData;
        }

        private SearchResult GetContainer(ICollection<SearchResult> newData, Guid key)
        {
            foreach (var searchResult in newData.Where(searchResult => searchResult.ProductID == key && searchResult.ModuleID != EmployeeID))
                return searchResult;

            var newResult = CreateCertainContainer(key);
            newData.Add(newResult);
            return newResult;
        }

        private SearchResult CreateCertainContainer(Guid key)
        {
            var certainProduct = ProductManager.Instance.Products.Find(x => x.ProductID == key);
            var container = new SearchResult
                                {
                                    ProductID = key,
                                    MoreURL = (key == CommunityID) ? String.Concat(VirtualPathUtility.ToAbsolute("~/search.aspx"), "?product=",key,"&search=", _searchText) : String.Empty,
                                    Name = (certainProduct != null) ? certainProduct.Name : String.Empty,
                                    LogoURL = (certainProduct != null) ? certainProduct.GetIconAbsoluteURL() : String.Empty
                                };

            if (key == CommunityID || key == Guid.Empty)
                container.PresentationControl = new CommonResultsView {MaxCount = 7, Text = _searchText};

            return container;
        }

        private List<SearchResult> SearchByModules(Guid productID, string _searchText)
        {
            var searchResults = new List<SearchResult>();
            if (String.IsNullOrEmpty(_searchText))
                return searchResults;

            var handlers = productID.Equals(Guid.Empty)
                               ? SearchHandlerManager.GetAllHandlersEx()
                               : SearchHandlerManager.GetHandlersExForCertainProduct(productID);

            foreach (var sh in handlers)
            {
                var module = WebItemManager.Instance[sh.ModuleID];
                if (module != null && module.IsDisabled())
                    continue;

                var items = sh.Search(_searchText);

                if (items.Length == 0)
                    continue;

                var searchResult = new SearchResult
                                       {
                                           ModuleID = sh.ModuleID,
                                           ProductID = sh.ProductID,
                                           PresentationControl = (ItemSearchControl) sh.Control
                                       };
                BuildName(searchResult, sh, module);
                BuildLogo(searchResult, sh, module);
                BuildMoreUrl(searchResult, sh, ref productID);
                searchResult.PresentationControl.Text = _searchText;
                searchResult.PresentationControl.MaxCount = 7;
                searchResult.Items.AddRange(items);

                searchResults.Add(searchResult);
            }
            return searchResults;
        }

        private void BuildName(SearchResult searchResult, ISearchHandlerEx sh, IWebItem module)
        {
            searchResult.Name = GetName(sh, module);
        }

        private string GetName(ISearchHandlerEx sh, IWebItem module)
        {
            if (sh.GetType().Equals(typeof (EmployeeSearchHendler)))
                return Core.Users.CustomNamingPeople.Substitute<Resources.Resource>("Employees");

            return module != null ? module.Name : sh.SearchName;
        }

        private void BuildLogo(SearchResult searchResult, ISearchHandlerEx sh, IWebItem module)
        {
            searchResult.LogoURL = GetLogo(sh, module);
        }

        private string GetLogo(ISearchHandlerEx sh, IWebItem module)
        {

            if (sh.GetType().Equals(typeof (EmployeeSearchHendler)))
                return WebImageSupplier.GetAbsoluteWebPath("employee.png");

            if (module != null)
                return module.GetIconAbsoluteURL();

            return WebImageSupplier.GetAbsoluteWebPath(sh.Logo.ImageFileName, sh.Logo.PartID);
        }

        private void BuildMoreUrl(SearchResult searchResult, ISearchHandlerEx sh, ref Guid productID)
        {
            searchResult.MoreURL = GetMoreUrl(sh, ref productID);
        }

        private string GetMoreUrl(ISearchHandlerEx sh, ref Guid productID)
        {
            var path = sh.AbsoluteSearchURL;

            if (sh.ProductID.Equals(Guid.Empty) && !productID.Equals(Guid.Empty))
                path = sh.AbsoluteSearchURL + (sh.AbsoluteSearchURL.IndexOf("?") != -1 ? "&" : "?") + CommonLinkUtility.GetProductParamsPair(productID);

            path = path + (path.IndexOf("?") != -1 ? "&search=" : "?search=") + HttpUtility.UrlEncode(_searchText, Encoding.UTF8);

            return path;
        }

        [AjaxMethod]
        public string GetAllData(string productID, string phrase)
        {
            var obj = SearchAll(phrase, new Guid(productID));
            var control = obj[0].PresentationControl;
            control.Items = obj[0].Items;
            control.MaxCount = int.MaxValue;
            var stringWriter = new StringWriter();
            var htmlWriter = new HtmlTextWriter(stringWriter);
            control.RenderControl(htmlWriter);
            return stringWriter.ToString();
        }
    }
}