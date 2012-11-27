using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using ASC.Core;
using ASC.Projects.Core.Domain;
using ASC.Projects.Engine;
using ASC.Web.Controls;
using ASC.Web.Core.Helpers;
using ASC.Web.Projects.Classes;
using ASC.Web.Projects.Resources;
using ASC.Web.Studio.Masters;
using ASC.Web.Studio.Utility;
using AjaxPro;

namespace ASC.Web.Projects
{
    [AjaxNamespace("AjaxPro.EditProjectTemplate")]
    public partial class EditProjectTemplate : BasePage
    {
        private int _projectTmplId;
        public Template Templ { get; set; }

        protected override void PageLoad()
        {
            if (!ProjectSecurity.IsAdministrator(SecurityContext.CurrentAccount.ID))
                HttpContext.Current.Response.Redirect(ProjectsCommonResource.StartURL, true);

            Utility.RegisterTypeForAjax(typeof(EditProjectTemplate));
            ((IStudioMaster)Master).DisabledSidePanel = true;

            if (Int32.TryParse(UrlParameters.EntityID, out _projectTmplId))
            {
                Templ = Global.EngineFactory.GetTemplateEngine().GetTemplate(_projectTmplId);
                JsonPublisher(Templ, "template");
            }

           InitBreadCrumbs();

        }

        protected string ChooseMonthNumeralCase(double count)
        {
            return count == 0 ? string.Empty : count + " " + ChooseNumeralCase(count, GrammaticalResource.MonthNominative,
                GrammaticalResource.MonthGenitiveSingular, GrammaticalResource.MonthGenitivePlural);
        }
        protected static string ChooseNumeralCase(double number, string nominative, string genitiveSingular, string genitivePlural)
        {
            if (number == 0.5)
            {
                if (
                    String.Compare(
                        System.Threading.Thread.CurrentThread.CurrentUICulture.ThreeLetterISOLanguageName,
                        "rus", true) == 0)
                {
                    return genitiveSingular;
                }
            }
            if(number == 1)
            {
                return nominative;
            }
            var count = (int)number;
            return GrammaticalHelper.ChooseNumeralCase(count, nominative, genitiveSingular, genitivePlural);
        }
        [AjaxMethod]
        public void SaveTemplate(Template template)
        {            
            Global.EngineFactory.GetTemplateEngine().SaveTemplate(template);
        }
        [AjaxMethod]
        public int SaveTemplateAndCreateProject(Template template)
        {
            var tmpl = Global.EngineFactory.GetTemplateEngine().SaveTemplate(template);
            return  tmpl.Id;
        }
        private void InitBreadCrumbs()
        {
            var title = _projectTmplId == 0 ? ProjectTemplatesResource.CreateProjTmpl : ProjectTemplatesResource.EditProjTmpl;

            Master.BreadCrumbs.Add(new BreadCrumb(title));

            Title = HeaderStringHelper.GetPageTitle(title, Master.BreadCrumbs);

        }
    }
}
