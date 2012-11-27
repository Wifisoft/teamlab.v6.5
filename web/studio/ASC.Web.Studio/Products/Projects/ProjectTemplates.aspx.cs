using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using ASC.Core;
using ASC.Web.Core.Utility.Skins;
using ASC.Web.Projects.Configuration;
using ASC.Web.Studio.Masters;
using ASC.Web.Studio.Utility;
using AjaxPro;
using ASC.Projects.Core.Domain;
using ASC.Web.Controls;
using ASC.Web.Projects.Classes;
using ASC.Web.Projects.Resources;
using ASC.Projects.Engine;

namespace ASC.Web.Projects
{
    [AjaxNamespace("AjaxPro.ProjectTemplates2")]
    public partial class ProjectTemplates : BasePage
    {
        protected List<Template> ListTemplates { get; set; }

        public bool EmptyListTemplates { get; set; }

        protected override void PageLoad()
        {
            if (!ProjectSecurity.IsAdministrator(SecurityContext.CurrentAccount.ID))
                HttpContext.Current.Response.Redirect(ProjectsCommonResource.StartURL, true);

            Utility.RegisterTypeForAjax(GetType(), Page);
            ((IStudioMaster)Master).DisabledSidePanel = true;

            ListTemplates = Global.EngineFactory.GetTemplateEngine().GetTemplates();
            if (ListTemplates.Count != 0)
            {
                EmptyListTemplates = false;
                JsonPublisher(ListTemplates, "templates");
                _hintPopup.Options.IsPopup = true;
            }
            else
            {
                EmptyListTemplates = true;
            }

            var escNoTmpl = new Studio.Controls.Common.EmptyScreenControl
            {
                Header = ProjectTemplatesResource.EmptyListTemplateHeader,
                ImgSrc = WebImageSupplier.GetAbsoluteWebPath("project-templates_logo.png", ProductEntryPoint.ID),
                Describe = ProjectTemplatesResource.EmptyListTemplateDescr,
                ID = "escNoTmpl",
                ButtonHTML = string.Format("<a href='editprojecttemplate.aspx' class='projectsEmpty baseLinkAction'>{0}<a>", ProjectTemplatesResource.EmptyListTemplateButton)
            };
            _escNoTmpl.Controls.Add(escNoTmpl);

            Master.BreadCrumbs.Add(new BreadCrumb
            {
                Caption = ProjectTemplatesResource.AllProjectTmpl
            });

            Title = HeaderStringHelper.GetPageTitle(ProjectTemplatesResource.AllProjectTmpl, Master.BreadCrumbs);
        }

        [AjaxMethod]
        public bool RemoveTemplate(int templId)
        {  
            Global.EngineFactory.GetTemplateEngine().RemoveTemplate(templId);
            return true;
        }
    }
}
