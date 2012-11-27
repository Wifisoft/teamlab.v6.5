using System;
using ASC.Core;
using ASC.Projects.Engine;
using ASC.Web.Controls;
using ASC.Web.Core.Utility.Skins;
using ASC.Web.Projects.Classes;
using ASC.Web.Projects.Configuration;
using ASC.Web.Projects.Resources;
using ASC.Web.Studio.Masters;
using ASC.Web.Studio.Utility;

namespace ASC.Web.Projects
{
    public partial class Projects : BasePage
    {
        public bool IsEmptyListProjects { get; set; }

        protected override void PageLoad()
        {
            if (RequestContext.IsInConcreteProject())
            {
                Server.Transfer(String.Concat(PathProvider.BaseAbsolutePath, "ProjectOverview.aspx"));
            }
            else
            {
                if (String.Compare(UrlParameters.ActionType, "add", StringComparison.OrdinalIgnoreCase) == 0)
                {
                    if (ProjectSecurity.IsAdministrator(SecurityContext.CurrentAccount.ID))
                    {
                        Server.Transfer(String.Concat(PathProvider.BaseAbsolutePath, "ProjectAction.aspx"));
                    }
                    else
                    {
                        Response.Redirect("projects.aspx");
                    }
                }
            }

            ((IStudioMaster)Master).DisabledSidePanel = true;

            Master.BreadCrumbs.Add(new BreadCrumb
            {
                Caption = ProjectResource.Projects,
                NavigationUrl = "projects.aspx"

            });

            Title = HeaderStringHelper.GetPageTitle(ProjectResource.Projects, Master.BreadCrumbs);

            IsEmptyListProjects = !RequestContext.HasAnyProjects();
            if(IsEmptyListProjects)
            {
                var button = "";
                if (ProjectSecurity.CanCreateProject())
                {
                    button = string.Format("<a href='projects.aspx?action=add' class='projectsEmpty baseLinkAction'>{0}<a>",
                                      ProjectResource.CreateFirstProject);
                }
                var escNoProj = new Studio.Controls.Common.EmptyScreenControl
                {
                    Header = ProjectResource.EmptyListProjHeader,
                    ImgSrc = WebImageSupplier.GetAbsoluteWebPath("projects_logo.png", ProductEntryPoint.ID),
                    Describe = ProjectResource.EmptyListProjDescribe,
                    ID = "escNoProj",
                    ButtonHTML = button
                };
                _escNoProj.Controls.Add(escNoProj);
            }
            else
            {
                var list = (Controls.Projects.ProjectsList)LoadControl(PathProvider.GetControlVirtualPath("ProjectsList.ascx"));
                __listProjects.Controls.Add(list);

                var advansedFilter = new Studio.Controls.Common.AdvansedFilter { BlockID = "AdvansedFilter" };
                _content.Controls.Add(advansedFilter);

                var emptyScreenControlFilter = new Studio.Controls.Common.EmptyScreenControl
                {
                    ImgSrc = WebImageSupplier.GetAbsoluteWebPath("empty-filter.png", ProductEntryPoint.ID),
                    Header = ProjectsCommonResource.Filter_NoProjects,
                    Describe = ProjectResource.DescrEmptyListProjFilter,
                    ID="emptyFilter",
                    ButtonHTML = String.Format("<a href='javascript:void(0)' class='baseLinkAction'>{0}</a>",
                           ProjectsFilterResource.ClearFilter)
                };
                projectListEmptyScreen.Controls.Add(emptyScreenControlFilter);
            }     
        }
    }
}
