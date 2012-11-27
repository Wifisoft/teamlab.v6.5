using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ASC.Core;
using ASC.Core.Users;
using ASC.Projects.Core.Domain;
using ASC.Projects.Engine;
using ASC.Web.Controls;
using ASC.Web.Core;
using ASC.Web.Core.Utility;
using ASC.Web.Core.Utility.Skins;
using ASC.Web.Projects.Classes;
using ASC.Web.Projects.Configuration;
using ASC.Web.Projects.Resources;
using ASC.Web.Studio.Controls.Common;
using ASC.Web.Studio.Core.Users;
using ASC.Web.Studio.Masters;
using ASC.Web.Studio.UserControls.Common;
using ASC.Web.Studio.UserControls.Users;
using ASC.Web.Studio.Utility;


namespace ASC.Web.Projects.Masters
{
    public partial class BasicTemplate : MasterPage, IStudioMaster
    {
        public List<BreadCrumb> BreadCrumbs
        {
            get
            {
                if (_commonContainer.BreadCrumbs == null) _commonContainer.BreadCrumbs = new List<BreadCrumb>();
                return _commonContainer.BreadCrumbs;
            }
        }

        public String InfoMessageText
        {
            set { _commonContainer.Options.InfoMessageText = value; }
        }

        public String InfoPanelClientID
        {
            get { return _commonContainer.GetInfoPanelClientID(); }

        }

        public String CommonContainerHeader
        {
            set { _commonContainer.Options.HeaderBreadCrumbCaption = value; }
        }

        public SideContainer AboutContainer
        {
            get { return _aboutContainer; }
        }

        private string GetUserInterestedProjects()
        {
            var interes = new List<Project>(RequestContext.GetCurrentUserProjects());
            interes.AddRange(RequestContext.GetCurrentUserFollowingProjects());
            return string.Join(",", interes.ConvertAll(p => p.ID.ToString()).ToArray());
        }

        protected void InitControls()
        {
            var searchHandler = (BaseSearchHandlerEx) (SearchHandlerManager.GetHandlersExForProduct(ProductEntryPoint.ID)).Find(sh => sh is SearchHandler);

            if (String.IsNullOrEmpty(Request[UrlConstant.ProjectID]))
                searchHandler.AbsoluteSearchURL = (VirtualPathUtility.ToAbsolute(PathProvider.BaseVirtualPath + "/search.aspx"));
            else
                searchHandler.AbsoluteSearchURL = (VirtualPathUtility.ToAbsolute(PathProvider.BaseVirtualPath + "/search.aspx") + "?prjID=" + Request[UrlConstant.ProjectID]);

            _aboutContainer.ImageURL = WebImageSupplier.GetAbsoluteWebPath("navigation.png");
            _aboutContainer.Title = ProjectsCommonResource.About;
            _aboutContainer.BodyCSSClass = "studioSideBoxBodyAbout";
            _aboutContainer.Visible = false;

            RenderHeader();

            var bottomNavigator = new BottomNavigator();

            _bottomNavigatorPlaceHolder.Controls.Add(bottomNavigator);

            var onlineUsersControl = (OnlineUsers) LoadControl(OnlineUsers.Location);
            onlineUsersControl.ProductId = ProductEntryPoint.ID;
            phOnlineUsers.Controls.Add(onlineUsersControl);


            if (Page.GetType() == typeof (Dashboard) && HaveProjects()) _commonContainer.Visible = false;

            //RSS
            //all interested projects
            //
            InterestedProjectsFeedControl.ContainerId = GetUserInterestedProjects();
            if (RequestContext.IsInConcreteProject())
            {
                //this project feed
                //
                ConcreteProjectFeedControl.Visible = true;
                ConcreteProjectFeedControl.ContainerId = RequestContext.GetCurrentProjectId().ToString();
                ConcreteProjectFeedControl.Title = RequestContext.GetCurrentProject().HtmlTitle.HtmlEncode();
            }
        }

        #region Below Navigation Panel

        public void DisplayBelowNavigationPanel(string text)
        {
            BelowNavigationPanel.Visible = true;
            BelowNavigationPanelText.InnerHtml = text;
        }

        #endregion

        protected bool HaveProjects()
        {
            return RequestContext.HasAnyProjects();
        }

        protected void BuildProjectsList(ref StringBuilder innerHTML, String selectedPageString)
        {
            var verticalLinePositions = new ArrayList();
            var count = 0;

            innerHTML.AppendFormat("<div id='projects_dropdown' class='pm-dropdown' style='display:none;' >");

            var projects = new List<Project>(RequestContext.GetCurrentUserProjects());
            verticalLinePositions.Add(projects.Count);

            var verticalLinePosition = projects.Count;

            if (verticalLinePosition < 10)
            {
                projects.AddRange(RequestContext.GetCurrentUserFollowingProjects());
                verticalLinePositions.Add(projects.Count);
            }

            if (verticalLinePosition < 10)
            {

                var otherProjects = Global.EngineFactory.GetProjectEngine().GetAll(ProjectStatus.Open, (10 - verticalLinePosition) + 1);
                otherProjects.Sort((x, y) => string.Compare(x.Title, y.Title));
                projects.AddRange(otherProjects.FindAll(item => !projects.Contains(item)));
            }

            foreach (var project in projects)
            {
                if (count + 1 > 10)
                {
                    innerHTML.AppendFormat(@"<a  class='linkSmall' style='margin:5px 10px; display:block;padding:2px;' href='{0}'>{1}</a>",
                                           String.Concat(PathProvider.BaseAbsolutePath, String.Format("projects.aspx?{0}={1}", UrlConstant.ProjectsFilter, Web.Projects.Controls.Projects.ListProjectView.ProjectFilter.All)),
                                           ProjectResource.SeeMore
                        );
                    break;
                }
                innerHTML.AppendFormat(@"<a  class='pm-dropdown-item'  href='{0}'>{1}</a>", String.Concat(PathProvider.BaseAbsolutePath, selectedPageString + ".aspx?prjID=" + project.ID), project.HtmlTitle.HtmlEncode());
                count++;

                if (verticalLinePositions.Contains(count))
                    innerHTML.Append("<div style='margin-top: 0px; margin-bottom: 0px; font-size: 1px; border-top: 1px solid rgb(209, 209, 209);' class='pm-dropdown-item'><!--– –--></div>");
            }

            innerHTML.AppendFormat("</div>");
        }

        protected String RenderAllProjectsBlock(int projectID, String selectedPageString)
        {
            var innerHTML = new StringBuilder();
            innerHTML.AppendLine("<div   style='padding-top: 2px;' >");
            innerHTML.AppendLine("<table>");
            innerHTML.AppendLine("<tr>");
            innerHTML.AppendFormat(@"<td  style='padding-left: 30px; height: 30px;'> 
                                            <a id='linkAllProjectsCombobox' href='{2}'>
                                                {0}
                                            </a>
                                             <a href='javascript:void(0)' id='projects_dropdown_switch' onclick='javascript:{3}'>
                                               <img style='border: 0px none ;' src='{1}'/>
                                             </a>
                                     </td>",
                                   ProjectResource.AllProjects,
                                   WebImageSupplier.GetAbsoluteWebPath("top_comb_arrow.gif", ProductEntryPoint.ID),
                                   String.Concat(PathProvider.BaseAbsolutePath, "projects.aspx"),
                                   "jq.dropdownToggle().toggle(jq(this).prev(), \"projects_dropdown\")"
                );

            if (RequestContext.IsInConcreteProject())
            {
                innerHTML.AppendFormat(@"<td style='padding-left: 15px; padding-right: 15px;'>
                                            <img src='{1}' />
                                         </td>
                                         <td>
                                            <span id='projTitle'>{0}</span>  
                                         </td>",
                                       RequestContext.GetCurrentProject().HtmlTitle.HtmlEncode(),
                                       WebImageSupplier.GetAbsoluteWebPath("top_split.gif", ProductEntryPoint.ID));
            }

            BuildProjectsList(ref innerHTML, selectedPageString);
            innerHTML.AppendLine("</table>");
            innerHTML.AppendLine("</div>");
            return innerHTML.ToString();
        }

        protected void RenderHeader()
        {
            var topNavigationPanel = (TopNavigationPanel) LoadControl(TopNavigationPanel.Location);
            topNavigationPanel.SingleSearchHandlerType = typeof (SearchHandler);

            var absolutePathWithoutQuery = Request.Url.AbsolutePath.Substring(0, Request.Url.AbsolutePath.IndexOf(".aspx"));
            var sysName = absolutePathWithoutQuery.Substring(absolutePathWithoutQuery.LastIndexOf('/') + 1);
            var project = RequestContext.GetCurrentProject(false);
            var projectID = -1;

            if (RequestContext.IsInConcreteProject())
            {
                projectID = project.ID;

                var rigthItems = new List<NavigationItem>();
                foreach (var webitem in WebItemManager.Instance.GetSubItems(ProductEntryPoint.ID))
                {
                    var module = webitem as IModule;

                    var navigationItem = new NavigationItem()
                                             {
                                                 URL = String.Format(webitem.StartURL, projectID),
                                                 Name = webitem.Name,
                                                 Description = webitem.Description,
                                                 Selected = String.Compare(sysName, module.ModuleSysName, true) == 0
                                             };

                    var added = false;
                    if (String.Compare(module.ModuleSysName, "History", true) == 0 ||
                        String.Compare(module.ModuleSysName, "ProjectAction", true) == 0 ||
                        String.Compare(module.ModuleSysName, "ProjectTeam", true) == 0)
                    {
                        navigationItem.RightAlign = true;
                        rigthItems.Add(navigationItem);
                        added = true;
                    }

                    //hide in private projects
                    if (String.Compare(module.ModuleSysName, "Messages", true) == 0 && !ProjectSecurity.CanReadMessages(RequestContext.GetCurrentProject()))
                    {
                        continue;
                    }
                    if (String.Compare(module.ModuleSysName, "TMDocs", true) == 0 && !ProjectSecurity.CanReadFiles(RequestContext.GetCurrentProject()))
                    {
                        continue;
                    }

                    if (String.Compare(module.ModuleSysName, "TMDocs", true) == 0)
                    {
                        navigationItem.Selected = String.Compare(sysName, "tmdocs", true) == 0;
                        navigationItem.Name = ProjectsFileResource.Documents;
                        navigationItem.URL = PathProvider.BaseAbsolutePath + "tmdocs.aspx?prjID=" + projectID;
                    }

                    if (!added)
                        topNavigationPanel.NavigationItems.Add(navigationItem);
                }

                rigthItems.Reverse();
                topNavigationPanel.NavigationItems.AddRange(rigthItems);

            }
            else
            {
                topNavigationPanel.NavigationItems.Add(new NavigationItem
                                                           {
                                                               URL = String.Concat(PathProvider.BaseAbsolutePath, "default.aspx"),
                                                               Name = ProjectsCommonResource.Dashboard,
                                                               Description = "",
                                                               Selected = String.Compare(sysName, "Default", true) == 0
                                                           });

                topNavigationPanel.NavigationItems.Add(new NavigationItem
                                                           {
                                                               URL = String.Concat(PathProvider.BaseAbsolutePath, "projects.aspx"),
                                                               Name = ProjectResource.Projects,
                                                               Description = "",
                                                               Selected = String.Compare(sysName, "Projects", true) == 0 || String.Compare(sysName, "ProjectAction", true) == 0
                                                           });


                topNavigationPanel.NavigationItems.Add(new NavigationItem
                                                           {
                                                               URL = String.Concat(PathProvider.BaseAbsolutePath, "milestones.aspx"),
                                                               Name = MilestoneResource.Milestones,
                                                               Description = "",
                                                               Selected = String.Compare(sysName, "milestones", true) == 0
                                                           });

                topNavigationPanel.NavigationItems.Add(new NavigationItem
                                                           {
                                                               URL = String.Concat(PathProvider.BaseAbsolutePath, "tasks.aspx"),
                                                               Name = TaskResource.Tasks,
                                                               Description = "",
                                                               Selected = String.Compare(sysName, "tasks", true) == 0
                                                           });

                topNavigationPanel.NavigationItems.Add(new NavigationItem
                                                           {
                                                               URL = String.Concat(PathProvider.BaseAbsolutePath, "messages.aspx"),
                                                               Name = MessageResource.Messages,
                                                               Description = "",
                                                               Selected = String.Compare(sysName, "messages", true) == 0
                                                           });
              
                topNavigationPanel.NavigationItems.Add(new NavigationItem
                                                           {
                                                               URL = String.Concat(PathProvider.BaseAbsolutePath, "reports.aspx"),
                                                               Name = ReportResource.Reports,
                                                               Description = "",
                                                               Selected = String.Compare(sysName, "Reports", true) == 0 || String.Compare(sysName, "Templates", true) == 0
                                                           });

                topNavigationPanel.NavigationItems.Add(new NavigationItem
                                                            {
                                                                URL = String.Concat(PathProvider.BaseAbsolutePath, "history.aspx"),
                                                                Name = ProjectsCommonResource.History,
                                                                Description = "",
                                                                Selected = String.Compare(sysName, "History", true) == 0
                                                            });


                if (ProjectSecurity.IsAdministrator(SecurityContext.CurrentAccount.ID))
                {
                    topNavigationPanel.NavigationItems.Add(new NavigationItem
                                                               {
                                                                   URL = String.Concat(PathProvider.BaseAbsolutePath, "projectTemplates.aspx"),
                                                                   Name = ProjectResource.ProjectTemplates,
                                                                   Description = "",
                                                                   Selected = String.Compare(sysName, "ProjectTemplates", true) == 0,
                                                                   RightAlign = true
                                                               });
                }

                topNavigationPanel.NavigationItems.Add(new NavigationItem
                                                           {
                                                               URL = CommonLinkUtility.GetEmployees(ProductEntryPoint.ID),
                                                               Name = CustomNamingPeople.Substitute<ProjectsCommonResource>("Employees"),
                                                               Description = "",
                                                               Selected = UserOnlineManager.Instance.IsEmployeesPage() ||UserOnlineManager.Instance.IsUserProfilePage(),
                                                               RightAlign = true

                                                           });

            }
            if (RequestContext.HasAnyProjects())
            {
                var pageName = "default";
                switch (sysName)
                {
                    case "userprofile":
                    case "default":
                    case "reports":
                    case "projectTemplates":
                    case "createprojectfromtemplate":
                    case "editprojecttemplate":
                        pageName = "projects";
                        break;

                    case "settings":
                        pageName = "projectAction";
                        break;

                    case "employee":
                        pageName = "projectTeam";
                        break;
                    default:
                        pageName = sysName;
                        break;
                }
                topNavigationPanel.CustomInfoHTML = RenderAllProjectsBlock(projectID, pageName);
            }
            _topNavigationPanelPlaceHolder.Controls.Add(topNavigationPanel);
        }


        protected void WriteClientScripts()
        {
            if (RequestContext.IsInConcreteProject())
            {
                var project = RequestContext.GetCurrentProject();
                var script = String.Format("ASC.Projects.Common.IAmIsManager = {0};", (project.Responsible == SecurityContext.CurrentAccount.ID).ToString().ToLower());

                if (!Page.ClientScript.IsClientScriptBlockRegistered(typeof (BasicTemplate), "{00F020A4-49F9-4d47-AE99-C8A44EB8FEB8}"))
                    Page.ClientScript.RegisterClientScriptBlock(typeof (BasicTemplate), "{00F020A4-49F9-4d47-AE99-C8A44EB8FEB8}", script, true);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            InitControls();

            Page.ClientScript.RegisterJavaScriptResource(typeof (ProjectsJSResource), "ProjectJSResources");

            WriteClientScripts();

            Page.EnableViewState = false;
        }


        public PlaceHolder ContentHolder
        {
            get
            {
                _commonContainer.Visible = false;
                return _contentHolder;
            }
        }

        public PlaceHolder SideHolder
        {
            get { return (Master as IStudioMaster).SideHolder; }
        }

        public PlaceHolder TitleHolder
        {
            get { return (Master as IStudioMaster).TitleHolder; }
        }

        public PlaceHolder FooterHolder
        {
            get { return (Master as IStudioMaster).FooterHolder; }
        }

        public bool DisabledSidePanel
        {
            get { return (this.Master as IStudioMaster).DisabledSidePanel; }
            set { (this.Master as IStudioMaster).DisabledSidePanel = value; }
        }

        public bool? LeftSidePanel
        {
            get { return (this.Master as IStudioMaster).LeftSidePanel; }
            set { (this.Master as IStudioMaster).LeftSidePanel = value; }
        }
    }
}