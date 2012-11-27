using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using ASC.Core;
using ASC.Core.Tenants;
using ASC.Web.CRM.Configuration;
using ASC.Web.Core.Utility.Skins;
using AjaxPro;
using ASC.CRM.Core.Entities;
using ASC.Web.CRM.Classes;
using ASC.Web.CRM.Resources;
using ASC.CRM.Core;
using Newtonsoft.Json.Linq;
using ASC.Core.Users;
using Microsoft;

namespace ASC.Web.CRM.Controls.Tasks
{
    public partial class ListTaskView : BaseUserControl
    {
        #region Properies

        public static string Location
        {
            get { return PathProvider.GetFileStaticRelativePath("Tasks/ListTaskView.ascx"); }
        }

        public bool HideContactSelector { get; set; }

        public int EntityID { get; set; }
        public Contact CurrentContact { get; set; }
        public EntityType CurrentEntityType { get; set; }
        public List<UserInfo> UserList { get; set; }

        protected bool MobileVer = false;

        protected bool NoTasks = false;

        protected string Anchor { get; set; }

        #endregion

        #region Events

        protected void Page_Load(object sender, EventArgs e)
        {
            MobileVer = ASC.Web.Core.Mobile.MobileDetector.IsRequestMatchesMobile(this.Context);

            if (UrlParameters.Action != "export")
            {
                InitEmptyScreenControls();

                Utility.RegisterTypeForAjax(typeof (AjaxProHelper));

                if (EntityID == 0 && CurrentContact == null)//the main page with tasks
                {
                    RegisterClientScriptForFilter();
                    GetDataFromCookie();
                }
                else // the tab with tasks
                {
                    if (CurrentContact != null)
                        Page.ClientScript.RegisterClientScriptBlock(GetType(), "8adeda71-2ad7-43b7-9939-f69c776dfbc0",
                                                                "contactForInitTaskActionPanel = "
                                                                + JavaScriptSerializer.Serialize(new
                                                                         {
                                                                             id = CurrentContact.ID,
                                                                             displayName = CurrentContact.GetTitle().HtmlEncode(),
                                                                             smallFotoUrl = ContactPhotoManager.GetSmallSizePhoto(CurrentContact.ID, CurrentContact is Company)
                                                                         })
                                                                + "; ", true);
                    if (UserList != null && UserList.Count > 0)
                        _taskActionView.UserList = UserList;
                }
            }
            else // export to csv
            {
                if (!CRMSecurity.IsAdmin)
                    Response.Redirect(PathProvider.StartURL());

                var tasks = GetTasksByFilter();

                if (UrlParameters.View != "editor")
                {
                    Response.Clear();
                    Response.ContentType = "text/csv; charset=utf-8";
                    Response.ContentEncoding = Encoding.UTF8;
                    Response.Charset = Encoding.UTF8.WebName;

                    var fileName = "tasks.csv";

                    Response.AppendHeader("Content-Disposition", String.Format("attachment; filename={0}", fileName));


                    Response.Write(ExportToCSV.ExportTasksToCSV(tasks, false));
                    Response.End();
                }
                else
                {
                    var fileUrl = ExportToCSV.ExportTasksToCSV(tasks, true);
                    Response.Redirect(fileUrl);
                }
            }
        }

        #endregion

        #region Methods

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

        private void RegisterClientScriptForFilter()
        {

            var taskCategories = Global.DaoFactory.GetListItemDao().GetItems(ListType.TaskCategory);
            Page.ClientScript.RegisterClientScriptBlock(GetType(), "592b6140-cc8b-4c63-a866-c300e2af536e",
                                                    "taskCategories = "
                                                    + JavaScriptSerializer.Serialize(taskCategories.ConvertAll(item => new
                                                    {
                                                        value = item.ID,
                                                        title = item.Title.HtmlEncode()
                                                    }))
                                                    + "; ", true);
        }

        private void InitEmptyScreenControls()
        {
            tasksEmptyScreen.ImgSrc = WebImageSupplier.GetAbsoluteWebPath("empty_screen_tasks.png", ProductEntryPoint.ID);
            tasksEmptyScreen.Header = CRMTaskResource.EmptyContentTasksHeader;
            tasksEmptyScreen.Describe = String.Format(CRMTaskResource.EmptyContentTasksDescribe,
                                                        //types
                                                      "<span class='hintCategories baseLinkAction' >", "</span>");

            var buttonHtml = String.Format(
                    "<a class='linkAddMediumText baseLinkAction' onclick='ASC.CRM.TaskActionView.showTaskPanel(0, {0}, \"{1}\", {2});'>{3}</a>",
                        CurrentContact == null ? 0 : CurrentContact.ID,
                        CurrentEntityType == EntityType.Any ? String.Empty : CurrentEntityType.ToString().ToLower(),
                        EntityID,
                        EntityID == 0 && CurrentContact == null ? CRMTaskResource.CreateFirstTask : CRMTaskResource.CreateTask);

            if (!MobileVer && EntityID == 0 && CurrentContact == null)
                buttonHtml += String.Format(@"<br/><a class='crm-importLink' href='tasks.aspx?action=import'>{0}</a>",
                                            CRMTaskResource.ImportTasks);

            tasksEmptyScreen.ButtonHTML = buttonHtml;


            emptyContentForTasksFilter.ImgSrc = WebImageSupplier.GetAbsoluteWebPath("empty_screen_filter.png", ProductEntryPoint.ID);
            emptyContentForTasksFilter.Header = CRMTaskResource.EmptyContentTasksFilterHeader;
            emptyContentForTasksFilter.Describe = CRMTaskResource.EmptyContentTasksFilterDescribe;
            emptyContentForTasksFilter.ButtonHTML = String.Format(
                                            @"<a class='crm-clearFilterButton' href='javascript:void(0);'
                                                        onclick='ASC.CRM.ListTaskView.advansedFilter.advansedFilter(null);'>
                                                            {0}
                                            </a>",
                                            CRMCommonResource.ClearFilter);
        }

        private class FilterObject
        {
            public int StartIndex { get; set; }
            public int Count { get; set; }
            public string SortBy { get; set; }
            public string SortOrder { get; set; }
            public string FilterValue { get; set; }

            public Guid ResponsibleID { get; set; }

            public bool? IsClosed { get; set; }
            
            public DateTime FromDate { get; set; }
            public DateTime ToDate { get; set; }

            public int CategoryID { get; set; }
           
            public FilterObject()
            {
                IsClosed = null;
                FromDate = DateTime.MinValue;
                ToDate = DateTime.MinValue;
            }
        };

        private FilterObject GetFilterObjectFromCookie()
        {
            var result = new FilterObject
            {
                Count = Global.EntryCountOnPage,
                StartIndex = 0
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

                            case "overdue":
                            case "today":
                            case "theNext":
                                var valueString = filterParam.Value<string>("value");
                                var fromToArray = JArray.Parse(valueString);
                                if (fromToArray.Count != 2) continue;
                                result.FromDate = !String.IsNullOrEmpty(fromToArray[0].Value<String>()) ? UrlParameters.ApiDateTimeParse(fromToArray[0].Value<String>()) : DateTime.MinValue;
                                result.ToDate = !String.IsNullOrEmpty(fromToArray[1].Value<String>()) ? UrlParameters.ApiDateTimeParse(fromToArray[1].Value<String>()) : DateTime.MinValue;
                                break;
                            case "fromToDate":
                                result.FromDate = UrlParameters.ApiDateTimeParse(filterParam.Value<string>("from"));
                                result.ToDate = UrlParameters.ApiDateTimeParse(filterParam.Value<string>("to"));
                                break;
                            case "categoryID":
                                result.CategoryID = filterParam.Value<int>("value");
                                break;
                            case "openTask":
                            case "closedTask":
                                result.IsClosed = filterParam.Value<bool>("value");
                                break;
                        }
                    }
                }
                catch (Exception)
                {
                    result.Count = Global.EntryCountOnPage;
                    result.StartIndex = 0;
                    result.SortBy = "deadline";
                    result.SortOrder = "ascending";
                }
            }
            else
            {
                result.Count = Global.EntryCountOnPage;
                result.StartIndex = 0;
                result.SortBy = "deadline";
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
            if (!String.IsNullOrEmpty(filterObj.FilterValue) || filterObj.CategoryID != 0
                    || filterObj.FromDate != DateTime.MinValue || filterObj.ToDate != DateTime.MinValue
                    || filterObj.IsClosed != null || filterObj.ResponsibleID != Guid.Empty)
                filterNotEmpty = true;

            var queryString = String.Format("startIndex={0}&count={1}", filterObj.StartIndex, filterObj.Count);

            queryString += String.Format("&sortBy={0}", filterObj.SortBy);
            queryString += String.Format("&sortOrder={0}", filterObj.SortOrder);
            queryString += !String.IsNullOrEmpty(filterObj.FilterValue) ? String.Format("&filterValue={0}", filterObj.FilterValue) : "";
            queryString += filterObj.ResponsibleID != Guid.Empty ? String.Format("&responsibleID={0}", filterObj.ResponsibleID) : "";
            queryString += filterObj.FromDate != DateTime.MinValue ? String.Format("&fromDate={0}", DateToStringApi(filterObj.FromDate)) : "";
            queryString += filterObj.ToDate != DateTime.MinValue ? String.Format("&toDate={0}", DateToStringApi(filterObj.ToDate)) : "";

            queryString += filterObj.CategoryID != 0 ? String.Format("&categoryID={0}", filterObj.CategoryID) : "";
            queryString += filterObj.IsClosed != null ? String.Format("&isClosed={0}", filterObj.IsClosed.ToString().ToLower()) : "";


            var apiServer = new Api.ApiServer();
            var tasksForFirstRequest = apiServer.GetApiResponse(
               String.Format("api/1.0/crm/task/filter.json?{0}", queryString), "GET");

            var tasksRequest = JObject.Parse(Encoding.UTF8.GetString(Convert.FromBase64String(tasksForFirstRequest)));

            NoTasks = !filterNotEmpty && tasksRequest["count"].ToString() == "0";

            Page.JsonPublisher(tasksForFirstRequest, "tasksForFirstRequest");
        }

        protected List<Task> GetTasksByFilter()
        {
            var filterObj = GetFilterObjectFromCookie();

            var sortBy = TaskSortedByType.DeadLine;
            if (!String.IsNullOrEmpty(filterObj.SortBy) && filterObj.SortBy == TaskSortedByType.Category.ToString().ToLower())
                sortBy = TaskSortedByType.Category;
            if (!String.IsNullOrEmpty(filterObj.SortBy) && filterObj.SortBy == TaskSortedByType.Title.ToString().ToLower())
                sortBy = TaskSortedByType.Title;

            var isAsc = !String.IsNullOrEmpty(filterObj.SortOrder) && filterObj.SortOrder != "descending";

            return Global.DaoFactory.GetTaskDao().GetTasks(filterObj.FilterValue,
                                                           filterObj.ResponsibleID,
                                                           filterObj.CategoryID,
                                                           filterObj.IsClosed,
                                                           filterObj.FromDate,
                                                           filterObj.ToDate,
                                                           EntityType.Any, 0, 0, 0,
                                                           new OrderBy(sortBy, isAsc));
        }

        #endregion
    }
}