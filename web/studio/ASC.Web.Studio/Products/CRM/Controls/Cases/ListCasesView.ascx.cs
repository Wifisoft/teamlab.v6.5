using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using ASC.Core.Tenants;
using ASC.Web.Controls;
using ASC.Web.CRM.Configuration;
using ASC.Web.Core.Utility.Skins;
using AjaxPro;
using ASC.CRM.Core.Entities;
using ASC.Web.CRM.Resources;
using ASC.Web.CRM.Classes;
using ASC.CRM.Core;
using Newtonsoft.Json.Linq;


namespace ASC.Web.CRM.Controls.Cases
{
    public partial class ListCasesView : BaseUserControl
    {
        #region Properies

        public static string Location
        {
            get { return PathProvider.GetFileStaticRelativePath("Cases/ListCasesView.ascx"); }
        }

        protected bool MobileVer = false;

        protected bool NoCases = false;

        protected string CookieKeyForPagination
        {
            get { return "casesPageNumber"; }
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
                GetDataFromCookie();
                if (NoCases)
                {
                    InitEmptyScreenControlForNoCases();
                }
                else
                {
                    InitEmptyScreenControlForFilter();
                    RegisterClientScriptForFilter();
                }
            }
            else
            {
                if (!CRMSecurity.IsAdmin)
                    Response.Redirect(PathProvider.StartURL());

                var cases = GetCasesByFilter();

                if (UrlParameters.View != "editor")
                {
                    Response.Clear();
                    Response.ContentType = "text/csv; charset=utf-8";
                    Response.ContentEncoding = Encoding.UTF8;
                    Response.Charset = Encoding.UTF8.WebName;

                    var fileName = "cases.csv";

                    Response.AppendHeader("Content-Disposition", String.Format("attachment; filename={0}", fileName));
                   
                    
                    Response.Write(ExportToCSV.ExportCasesToCSV(cases, false));
                    Response.End();
                }
                else
                {
                    var fileUrl = ExportToCSV.ExportCasesToCSV(cases, true);
                    Response.Redirect(fileUrl);
                }
            }
        }

        #endregion

        #region Methods

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

            public bool? IsClosed { get; set; }
            public List<string> Tags { get; set; }

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
                            case "closed":
                            case "opened":
                                result.IsClosed = filterParam.Value<bool>("value");
                                break;
                            case "tags":
                                result.Tags = new List<string>();
                                result.Tags = filterParam.Value<JArray>("value").ToList().ConvertAll(
                                        n => n.ToString());
                                break;
                        }
                    }
                }
                catch (Exception)
                {
                    result.CurrentPageNumber = CurrentPageNumber;
                    result.Count = EntryCountOnPage;
                    result.StartIndex = CurrentPageNumber > 0 ? (CurrentPageNumber - 1) * EntryCountOnPage : 0;
                    result.SortBy = "title";
                    result.SortOrder = "ascending";
                }
            }
            else
            {
                result.CurrentPageNumber = CurrentPageNumber;
                result.Count = EntryCountOnPage;
                result.StartIndex = CurrentPageNumber > 0 ? (CurrentPageNumber - 1) * EntryCountOnPage : 0;
                result.SortBy = "title";
                result.SortOrder = "ascending";
                Anchor = String.Empty;
            }
            return result;
        }

        protected void GetDataFromCookie()
        {
            //init data for first request
            var filterObj = GetFilterObjectFromCookie();

            var filterNotEmpty = false;
            if (!String.IsNullOrEmpty(filterObj.FilterValue) || filterObj.IsClosed != null || filterObj.Tags != null)
                filterNotEmpty = true;

            var queryString = String.Format("startIndex={0}&count={1}", filterObj.StartIndex, filterObj.Count);

            queryString += String.Format("&sortBy={0}", filterObj.SortBy);
            queryString += String.Format("&sortOrder={0}", filterObj.SortOrder);
            queryString += !String.IsNullOrEmpty(filterObj.FilterValue) ? String.Format("&filterValue={0}", filterObj.FilterValue) : "";
            queryString += filterObj.IsClosed != null ? String.Format("&isClosed={0}", filterObj.IsClosed.ToString().ToLower()): "";
            
            if (filterObj.Tags != null && filterObj.Tags.Count != 0)
            {
                queryString = filterObj.Tags.Aggregate(queryString,
                                                       (current, tag) => current + String.Format("&tags[]={0}", tag));
            }

            var apiServer = new Api.ApiServer();
            var casesForFirstRequest = apiServer.GetApiResponse(
                String.Format("api/1.0/crm/case/filter.json?{0}", queryString), "GET");

            var casesRequest = JObject.Parse(Encoding.UTF8.GetString(Convert.FromBase64String(casesForFirstRequest)));
            Page.JsonPublisher(casesForFirstRequest, "casesForFirstRequest");


            NoCases = !filterNotEmpty && casesRequest["count"].ToString() == "0";

            //init Page Navigator
            _phPagerContent.Controls.Add(new PageNavigator
            {
                ID = "casesPageNavigator",
                CurrentPageNumber = filterObj.CurrentPageNumber,
                VisibleOnePage = false,
                EntryCount = 0,
                VisiblePageCount = Global.VisiblePageCount,
                EntryCountOnPage = filterObj.Count
            });
        }

        protected  void RegisterClientScriptForFilter()
        {
            var tags = Global.DaoFactory.GetTagDao().GetAllTags(EntityType.Case).ToList();
            Page.ClientScript.RegisterClientScriptBlock(typeof(ListCasesView), "d6c5c3db-dce4-423a-ac96-8304c71dfae2",
                                                        "caseTags = "
                                                        + JavaScriptSerializer.Serialize(tags.ConvertAll(item => new
                                                        {
                                                            value = item.HtmlEncode(),
                                                            title = item.HtmlEncode()
                                                        }))
                                                        + "; ", true);
        }

        protected void InitEmptyScreenControlForNoCases()
        {
            casesEmptyScreen.ImgSrc = WebImageSupplier.GetAbsoluteWebPath("empty_screen_cases.png",
                                             ProductEntryPoint.ID);
            casesEmptyScreen.Header = CRMCasesResource.EmptyContentCasesHeader;
            casesEmptyScreen.Describe = CRMCasesResource.EmptyContentCasesDescribe;


            var buttonHtml =  String.Format("<a class='linkAddMediumText' href='cases.aspx?action=manage'>{0}</a>",
                              CRMCasesResource.AddFirstCase);

            if (!MobileVer)
                buttonHtml += String.Format(@"<br/><a class='crm-importLink' href='cases.aspx?action=import'>{0}</a>",
                                            CRMCasesResource.ImportCases);

            casesEmptyScreen.ButtonHTML = buttonHtml;


        }

        protected void InitEmptyScreenControlForFilter()
        {
            emptyContentForCasesFilter.ImgSrc = WebImageSupplier.GetAbsoluteWebPath("empty_screen_filter.png",
                                                                                    ProductEntryPoint.ID);
            emptyContentForCasesFilter.Header = CRMCasesResource.EmptyContentCasesFilterHeader;
            emptyContentForCasesFilter.Describe = CRMCasesResource.EmptyContentCasesFilterDescribe;
            emptyContentForCasesFilter.ButtonHTML = String.Format(
                                            @"<a class='crm-clearFilterButton' href='javascript:void(0);'
                                                        onclick='ASC.CRM.ListCasesView.advansedFilter.advansedFilter(null);'>
                                                            {0}
                                            </a>",
                    CRMCommonResource.ClearFilter);
        }

        protected List<ASC.CRM.Core.Entities.Cases> GetCasesByFilter()
        {
            var filterObj = GetFilterObjectFromCookie();

            var sortBy = SortedByType.Title;
            if (!String.IsNullOrEmpty(filterObj.SortBy) && filterObj.SortBy == SortedByType.DateAndTime.ToString().ToLower())
                sortBy = SortedByType.DateAndTime;
            if (!String.IsNullOrEmpty(filterObj.SortBy) && filterObj.SortBy == SortedByType.CreateBy.ToString().ToLower())
                sortBy = SortedByType.CreateBy;      

            var isAsc = !String.IsNullOrEmpty(filterObj.SortOrder) && filterObj.SortOrder != "descending";

            return Global.DaoFactory.GetCasesDao().GetCases(filterObj.FilterValue,
                                                            0,
                                                            filterObj.IsClosed,
                                                            filterObj.Tags,
                                                            0, 0,
                                                            new OrderBy(sortBy, isAsc));
        }

        #endregion

    }
}