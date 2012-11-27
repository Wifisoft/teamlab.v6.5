#region Import

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using ASC.Core;
using ASC.Core.Tenants;
using ASC.Web.Controls;
using ASC.Web.CRM.Configuration;
using ASC.Web.Core.Utility.Skins;
using AjaxPro;
using ASC.CRM.Core;
using ASC.CRM.Core.Entities;
using ASC.Web.CRM.Classes;
using ASC.Web.CRM.Controls.Common;
using ASC.Web.CRM.Resources;
using Newtonsoft.Json.Linq;

#endregion

namespace ASC.Web.CRM.Controls.Deals
{
    public partial class ListDealView : BaseUserControl
    {
        #region Properies

        public static string Location
        {
            get { return PathProvider.GetFileStaticRelativePath("Deals/ListDealView.ascx"); }
        }

        public int contactID { get; set; }

        protected bool MobileVer = false;

        protected bool NoDeals = false;

        protected string CookieKeyForPagination
        {
            get { return "dealPageNumber"; }
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
                if (contactID == 0)
                {
                    GetDataFromCookie();
                    if (NoDeals)
                    {
                        InitEmptyScreenForNoDeals();
                    }
                    else
                    {
                        InitEmptyScreenForFilter();
                        
                        InitContactSelectorForFilter();

                        RegisterClientScriptForFilter();
                    }
                }
                else //if (contactID != 0)
                {
                    NoDeals = false;

                    InitEmptyScreenForNoDealsInContact();
                }
            }
            else if (contactID == 0)
            {
                if (!CRMSecurity.IsAdmin)
                    Response.Redirect(PathProvider.StartURL());

                var deals = GetDealsByFilter();

                if (UrlParameters.View != "editor")
                {
                    Response.Clear();
                    Response.ContentType = "text/csv; charset=utf-8";
                    Response.ContentEncoding = Encoding.UTF8;
                    Response.Charset = Encoding.UTF8.WebName;
                    var fileName = "opportunity.csv";

                    Response.AppendHeader("Content-Disposition", String.Format("attachment; filename={0}", fileName)); Response.Write(ExportToCSV.ExportDealsToCSV(deals, false));
                    Response.End();
                }
                else
                {
                    var fileUrl = ExportToCSV.ExportDealsToCSV(deals, true);
                    Response.Redirect(fileUrl);
                }
            }
        }

        #endregion

        #region Methods

        private void InitEmptyScreenForNoDeals()
        {
            dealsEmptyScreen.ImgSrc = WebImageSupplier.GetAbsoluteWebPath("empty_screen_deals.png", ProductEntryPoint.ID);
            dealsEmptyScreen.Header = CRMDealResource.EmptyContentDealsHeader;
            dealsEmptyScreen.Describe = String.Format(CRMDealResource.EmptyContentDealsDescribe,
                                                        //stages
                                                       "<span class='hintStages baseLinkAction' >",
                                                       "</span>");

            var buttonHtml = String.Format("<a class='linkAddMediumText' href='deals.aspx?action=manage'>{0}</a>",
                                                        CRMDealResource.CreateFirstDeal);

            if (!MobileVer)
                buttonHtml += String.Format(@"<br/><a class='crm-importLink' href='deals.aspx?action=import'>{0}</a>",
                                           CRMDealResource.ImportDeals);

            dealsEmptyScreen.ButtonHTML = buttonHtml;

        }

        private void InitEmptyScreenForFilter()
        {
            emptyContentForDealsFilter.ImgSrc = WebImageSupplier.GetAbsoluteWebPath("empty_screen_filter.png", ProductEntryPoint.ID);
            emptyContentForDealsFilter.Header = CRMDealResource.EmptyContentDealsFilterHeader;
            emptyContentForDealsFilter.Describe = CRMDealResource.EmptyContentDealsFilterDescribe;
            emptyContentForDealsFilter.ButtonHTML = String.Format(
                                                @"<a class='crm-clearFilterButton' href='javascript:void(0);'
                                                        onclick='ASC.CRM.ListDealView.advansedFilter.advansedFilter(null);'>
                                                            {0}
                                                  </a>",
                                                CRMCommonResource.ClearFilter);
        }

        private void InitEmptyScreenForNoDealsInContact()
        {
            emptyContentForDealsFilter.ImgSrc = WebImageSupplier.GetAbsoluteWebPath("empty_screen_deals.png", ProductEntryPoint.ID);
            emptyContentForDealsFilter.Header = CRMDealResource.EmptyContentDealsHeader;
            emptyContentForDealsFilter.Describe = String.Format(CRMDealResource.EmptyContentDealsDescribe,
                                                "<span class='hintStages baseLinkAction' >", "</span>");
            emptyContentForDealsFilter.ButtonHTML = String.Format(
                            "<a class='linkAddMediumText' href='deals.aspx?action=manage&contactID={0}'>{1}</a>",
                            contactID,
                            CRMDealResource.CreateDeal);
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

            public Guid ResponsibleID { get; set; }
            
            public String StageType { get; set; }
            public int OpportunityStagesID { get; set; }

            public DateTime FromDate { get; set; }
            public DateTime ToDate { get; set; }

            public int ContactID { get; set; }
            public bool? ContactAlsoIsParticipant { get; set; }

            public List<string> Tags { get; set; }

            public int CurrentPageNumber { get; set; }
            public FilterObject()
            {
                ContactAlsoIsParticipant = null;
                FromDate = DateTime.MinValue;
                ToDate = DateTime.MinValue;
            }
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

                            case "my":
                            case "responsibleID":
                                result.ResponsibleID = new Guid(filterParam.Value<string>("value"));
                                break;

                            case "stageTypeOpen":
                            case "stageTypeClosedAndWon":
                            case "stageTypeClosedAndLost":
                                result.StageType = filterParam.Value<string>("value");
                                break;
                            case "opportunityStagesID":
                                result.OpportunityStagesID = filterParam.Value<int>("value");
                                break;

                            case "lastMonth":
                            case "yesterday":
                            case "today":
                            case "thisMonth":
                                var valueString = filterParam.Value<string>("value");
                                var fromToArray = JArray.Parse(valueString);
                                if (fromToArray.Count != 2) continue;
                                result.FromDate = UrlParameters.ApiDateTimeParse(fromToArray[0].Value<String>());
                                result.ToDate = UrlParameters.ApiDateTimeParse(fromToArray[1].Value<String>());
                            
                         
                                break;
                            case "fromToDate":
                                result.FromDate = UrlParameters.ApiDateTimeParse(filterParam.Value<string>("from"));
                                result.ToDate = UrlParameters.ApiDateTimeParse(filterParam.Value<string>("to"));
                                break;

                            case "participantID":
                                result.ContactID = filterParam.Value<int>("id");
                                result.ContactAlsoIsParticipant = true;
                                break;
                            case "contactID":
                                result.ContactID = filterParam.Value<int>("id");
                                result.ContactAlsoIsParticipant = false;
                                break;

                            case "tags":
                                result.Tags = new List<string>();
                                result.Tags = filterParam.Value<JArray>("value").ToList().ConvertAll(n => n.ToString());
                                break;
                        }
                    }
                }
                catch (Exception)
                {
                    result.CurrentPageNumber = CurrentPageNumber;
                    result.Count = EntryCountOnPage;
                    result.StartIndex = CurrentPageNumber > 0 ? (CurrentPageNumber - 1) * EntryCountOnPage : 0;
                    result.SortBy = "stage";
                    result.SortOrder = "ascending";
                }
            }
            else
            {
                result.CurrentPageNumber = CurrentPageNumber;
                result.Count = EntryCountOnPage;
                result.StartIndex = CurrentPageNumber > 0 ? (CurrentPageNumber - 1) * EntryCountOnPage : 0;
                result.SortBy = "stage";
                result.SortOrder = "ascending";
                Anchor = String.Empty;
            }

            return result;
        }

        private void GetDataFromCookie()
        {
            //init data for first request
            var filterObj = GetFilterObjectFromCookie();

            var filterNotEmpty = false;
            if (filterObj.ContactID != 0 || !String.IsNullOrEmpty(filterObj.FilterValue)
                || filterObj.FromDate != DateTime.MinValue || filterObj.ToDate != DateTime.MinValue || filterObj.OpportunityStagesID != 0
                || filterObj.ResponsibleID != Guid.Empty || filterObj.StageType != null
                || filterObj.Tags != null && filterObj.Tags.Count != 0)
                    filterNotEmpty = true;

            var queryString = String.Format("startIndex={0}&count={1}", filterObj.StartIndex, filterObj.Count);
            queryString += String.Format("&sortBy={0}", filterObj.SortBy);
            queryString += String.Format("&sortOrder={0}", filterObj.SortOrder);
            queryString += !String.IsNullOrEmpty(filterObj.FilterValue) ? String.Format("&filterValue={0}", filterObj.FilterValue) : "";

            queryString += filterObj.ResponsibleID != Guid.Empty ? String.Format("&responsibleID={0}", filterObj.ResponsibleID) : "";
            queryString += !String.IsNullOrEmpty(filterObj.StageType) ? String.Format("&stageType={0}", filterObj.StageType) : "";
            queryString += filterObj.OpportunityStagesID != 0 ? String.Format("&opportunityStagesID={0}", filterObj.OpportunityStagesID) : "";


            queryString += filterObj.FromDate != DateTime.MinValue && filterObj.ToDate != DateTime.MinValue
                               ? String.Format("&fromDate={0}&toDate={1}", DateToStringApi(filterObj.FromDate),
                                               DateToStringApi(filterObj.ToDate))
                               : "";

            queryString += filterObj.ContactID != 0 && filterObj.ContactAlsoIsParticipant != null
                                        ? String.Format("&contactID={0}&contactAlsoIsParticipant={1}",
                                                    filterObj.ContactID, filterObj.ContactAlsoIsParticipant)
                                        : "";
            if (filterObj.Tags != null)
            {
                queryString = filterObj.Tags.Aggregate(queryString,
                                                       (current, tag) => current + String.Format("&tags[]={0}", tag));
            }


            var apiServer = new Api.ApiServer();
            var opportunitiesForFirstRequest = apiServer.GetApiResponse(
                          String.Format("api/1.0/crm/opportunity/filter.json?{0}", queryString), "GET");

            
            var opportunitiesRequest = JObject.Parse(Encoding.UTF8.GetString(Convert.FromBase64String(opportunitiesForFirstRequest)));
            Page.JsonPublisher(opportunitiesForFirstRequest, "opportunitiesForFirstRequest");

            NoDeals = !filterNotEmpty && opportunitiesRequest["count"].ToString() == "0";

            //init Page Navigator
            _phPagerContent.Controls.Add(new PageNavigator
            {
                ID = "dealPageNavigator",
                CurrentPageNumber = filterObj.CurrentPageNumber,
                VisibleOnePage = false,
                EntryCount = 0,
                VisiblePageCount = Global.VisiblePageCount,
                EntryCountOnPage = filterObj.Count,
            });
        }

        private void InitContactSelectorForFilter()
        {
            Utility.RegisterTypeForAjax(typeof(ContactSelector));

            contactSelectorForFilter.ShowAddButton = false;
            contactSelectorForFilter.ShowAllDeleteButton = false;
            contactSelectorForFilter.ShowChangeButton = true;
            contactSelectorForFilter.ShowContactImg = false;
            contactSelectorForFilter.ShowDeleteButton = false;
            contactSelectorForFilter.ShowNewCompanyContent = false;
            contactSelectorForFilter.ShowNewContactContent = false;
            contactSelectorForFilter.ShowOnlySelectorContent = true;
            contactSelectorForFilter.CurrentType = ContactSelector.SelectorType.All;
            contactSelectorForFilter.SelectedContacts = null;
        }

        private void RegisterClientScriptForFilter()
        {
            var dealMilestones = Global.DaoFactory.GetDealMilestoneDao().GetAll();
            var tags = Global.DaoFactory.GetTagDao().GetAllTags(EntityType.Opportunity).ToList();

            Page.ClientScript.RegisterClientScriptBlock(GetType(), "29ab0c80-1afc-44f8-b081-169fa2c22bd1",
                                                           "dealMilestones = " +
                                                           JavaScriptSerializer.Serialize(dealMilestones.ConvertAll(
                                                               item => new
                                                                           {
                                                                               value = item.ID,
                                                                               title = item.Title,
                                                                               classname = "colorFilterItem color_" + item.Color.Replace("#", "").ToLower()
                                                                           })) + "; ", true);

            Page.ClientScript.RegisterClientScriptBlock(GetType(), "d6c5c3db-dce4-423a-ac96-8304c71dfae2",
                                                           "dealTags = " +
                                                           JavaScriptSerializer.Serialize(tags.ConvertAll(
                                                               item => new
                                                                           {
                                                                               value = item.HtmlEncode(),
                                                                               title = item.HtmlEncode()
                                                                           })) + "; ", true);
        }

        private static String DateToStringApi(DateTime dateTime)
        {
            var timeZoneOffset = CoreContext.TenantManager.GetCurrentTenant().TimeZone.BaseUtcOffset;
            var tzoHours = timeZoneOffset.Hours;
            var tzoMinutes = timeZoneOffset.Minutes;
            var tzoTotalMinutes = timeZoneOffset.TotalMinutes;
            var stringOffset = "";
            stringOffset = tzoTotalMinutes != 0
                               ? String.Format("0000000{0}{1}:{2}",
                                               tzoTotalMinutes > 0 ? "+" : "-",
                                               tzoHours < 10 ? "0" + tzoHours : tzoHours.ToString(),
                                               tzoMinutes < 10 ? "0" + tzoMinutes : tzoMinutes.ToString())
                               : "000Z";


            return HttpUtility.UrlEncode(String.Format("{0}-{1}-{2}T{3}-{4}-{5}.{6}", dateTime.Year,
                                    dateTime.Month < 10 ? "0" + dateTime.Month : dateTime.Month.ToString(),
                                    dateTime.Day < 10 ? "0" + dateTime.Day : dateTime.Day.ToString(),
                                    dateTime.Hour < 10 ? "0" + dateTime.Hour : dateTime.Hour.ToString(),
                                    dateTime.Minute < 10 ? "0" + dateTime.Minute : dateTime.Minute.ToString(),
                                    dateTime.Second < 10 ? "0" + dateTime.Second : dateTime.Second.ToString(),
                                    stringOffset));
        }

        protected List<Deal> GetDealsByFilter()
        {
            var filterObj = GetFilterObjectFromCookie();

            var sortBy = DealSortedByType.Title;
            if (!String.IsNullOrEmpty(filterObj.SortBy))
            {
                if (filterObj.SortBy == DealSortedByType.BidValue.ToString().ToLower())
                    sortBy = DealSortedByType.BidValue;
                if (filterObj.SortBy == DealSortedByType.DateAndTime.ToString().ToLower())
                    sortBy = DealSortedByType.DateAndTime;
                if (filterObj.SortBy == DealSortedByType.Stage.ToString().ToLower())
                    sortBy = DealSortedByType.Stage;
                if (filterObj.SortBy == DealSortedByType.Responsible.ToString().ToLower())
                    sortBy = DealSortedByType.Responsible;
            }
            var isAsc = !String.IsNullOrEmpty(filterObj.SortOrder) && filterObj.SortOrder != "descending";

            DealMilestoneStatus? stageType = null;
            if (!String.IsNullOrEmpty(filterObj.StageType))
            {
                if (filterObj.StageType.ToLower() == DealMilestoneStatus.Open.ToString().ToLower())
                    stageType = DealMilestoneStatus.Open;
                if (filterObj.StageType.ToLower() == DealMilestoneStatus.ClosedAndLost.ToString().ToLower())
                    stageType = DealMilestoneStatus.ClosedAndLost;
                if (filterObj.StageType.ToLower() == DealMilestoneStatus.ClosedAndWon.ToString().ToLower())
                    stageType = DealMilestoneStatus.ClosedAndWon;
            }

            return Global.DaoFactory.GetDealDao().GetDeals(filterObj.FilterValue,
                                                           filterObj.ResponsibleID,
                                                           filterObj.OpportunityStagesID,
                                                           filterObj.Tags,
                                                           filterObj.ContactID,
                                                           stageType,
                                                           filterObj.ContactAlsoIsParticipant,
                                                           filterObj.FromDate,
                                                           filterObj.ToDate,
                                                           0, 0,
                                                           new OrderBy(sortBy, isAsc));
        }

        #endregion

    }
}