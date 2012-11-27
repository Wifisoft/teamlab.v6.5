using System;
using System.Linq;
using System.Web;
using ASC.Core;
using ASC.Core.Users;
using ASC.Projects.Engine;
using ASC.Web.Controls;
using ASC.Web.Projects.Classes;
using ASC.Web.Projects.Configuration;
using ASC.Web.Projects.Controls;
using ASC.Web.Projects.Controls.Common;
using ASC.Web.Projects.Resources;
using ASC.Web.Studio.Utility;

namespace ASC.Web.Projects
{
    public partial class Search : BasePage
    {
        public int ProjectID { get; set; }
        public String SearchText { get; set; }
        public int NumberOfStaffFound { get; set; }


        protected override void PageLoad()
        {
            InitPage();
            //InitEmployeesSearchRepeater();

            if (string.IsNullOrEmpty(SearchText))
            {
                rptContent.DataSource = null;
                rptContent.DataBind();
                phContent.Controls.Add(new EmptyScreenControl(string.Empty) {HeaderContent = ProjectsCommonResource.NothingHasBeenFound});
            }
            else
            {
                var items = Global.EngineFactory.GetSearchEngine().Search(SearchText, ProjectID == -1 ? 0 : ProjectID);
                if (0 < items.Count)
                {
                    rptContent.DataSource = items;
                }
                else
                {
                    rptContent.DataSource = null;
                    if (NumberOfStaffFound == 0)
                    {
                        phContent.Controls.Add(new EmptyScreenControl(string.Empty) {HeaderContent = ProjectsCommonResource.NothingHasBeenFound});
                    }
                }
                rptContent.DataBind();
            }
        }


        public void InitPage()
        {
            SearchText = UrlParameters.Search;

            int prjID;
            if (!int.TryParse(UrlParameters.ProjectID, out prjID))
            {
                prjID = -1;
            }

            ProjectID = prjID;

            Master.BreadCrumbs.Add(new BreadCrumb {Caption = ProjectsCommonResource.Dashboard, NavigationUrl = "./"});
            Master.BreadCrumbs.Add(new BreadCrumb
                                       {
                                           Caption = HeaderStringHelper.GetHTMLSearchHeader(SearchText),
                                           NavigationUrl = string.Format("search.aspx?pid={0}&search={1}", ProductEntryPoint.ID, SearchText)
                                       });

            var project = ProjectID != -1 ? Global.EngineFactory.GetProjectEngine().GetByID(ProjectID) : null;
            if (project != null)
            {
                Master.BreadCrumbs.Add(new BreadCrumb
                                           {
                                               Caption = project.HtmlTitle,
                                               NavigationUrl = string.Format("search.aspx?prjID={0}&search={1}", ProjectID, SearchText)
                                           });
            }

            Title = HeaderStringHelper.GetPageTitle(ProjectsCommonResource.ModuleName, Master.BreadCrumbs);
            Master.DisabledSidePanel = true;
        }

        public string GetSearchView(SearchGroup searchGroup)
        {
            var page = new System.Web.UI.Page();
            var oSearchView = (SearchView) LoadControl(PathProvider.GetControlVirtualPath("SearchView.ascx"));

            oSearchView.SearchGroup = searchGroup;
            oSearchView.IsListView = ProjectID == -1;
            oSearchView.SearchText = SearchText;

            page.Controls.Add(oSearchView);
            var writer = new System.IO.StringWriter();
            HttpContext.Current.Server.Execute(page, writer, false);
            var output = writer.ToString();
            writer.Close();
            return output;
        }

        public void InitEmployeesSearchRepeater()
        {
            if (ProjectID == -1)
            {
                var users = CoreContext.UserManager.Search(SearchText, EmployeeStatus.Active);
                NumberOfStaffFound = users.Length;

                if (0 < NumberOfStaffFound)
                {
                    EmployeesSearchRepeater.DataSource = NumberOfStaffFound > 5 ? users.Take(5) : users;
                }
                else
                {
                    EmployeesSearchRepeater.DataSource = null;
                }
                EmployeesSearchRepeater.DataBind();
            }
        }
    }
}