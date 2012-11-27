using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using ASC.Core.Tenants;
using ASC.Data.Storage;
using ASC.Web.Core;
using ASC.Web.Controls;
using ASC.Web.Core.Utility.Skins;
using ASC.Web.Studio.UserControls.Users;

namespace ASC.Web.Studio.Controls.Common
{
    [ToolboxData("<{0}:AdvansedFilter runat=server></{0}:AdvansedFilter>")]
    public class AdvansedFilter : Control
    {
        #region Properties

        public string BlockID { get; set; }

        #endregion

        protected override void OnLoad(EventArgs e)
        {
            base.OnInit(e);

            var js = new StringBuilder();
            js.Append("<script id=\"template-filter-combobox-options\" type=\"text/x-jquery-tmpl\">");
            js.Append("  <option class=\"${classname}\" value=\"${value}\"{{if typeof def === 'boolean' && def === true}} selected=\"selected\"{{/if}}>${title}</option>");
            js.Append("</script>");

            js.Append("<script id=\"template-filter-filtervalues\" type=\"text/x-jquery-tmpl\">");
            js.Append("  <div class=\"advansed-item-list advansed-filter-list\">");
            js.Append("    <ul class=\"item-list filter-list\"></ul>");
            js.Append("  </div>");
            js.Append("</script>");

            js.Append("<script id=\"template-filter-sortervalues\" type=\"text/x-jquery-tmpl\">");
            js.Append("  <div class=\"advansed-item-list advansed-sorter-list\">");
            js.Append("    <ul class=\"item-list sorter-list\"></ul>");
            js.Append("  </div>");
            js.Append("</script>");

            js.Append("<script id=\"template-filter-container\" type=\"text/x-jquery-tmpl\">");
            js.Append("  <div class=\"advansed-filter empty-filter-list\"><div class=\"advansed-filter-wrapper\">");
            js.Append("    <label class=\"advansed-filter-state btn-start-filter\"></label>");
            js.Append("    <label class=\"advansed-filter-sort btn-show-sorters\"></label>");
            js.Append("    <div class=\"advansed-filter-container\">");
            js.Append("      <div class=\"advansed-filter-filters empty-list\">");
            js.Append("        <div class=\"btn-show-hidden-filters\">" + Resources.UserControlsCommonResource.BtnHiddenFilter + "</div>");
            js.Append("        <div class=\"hidden-filters-container\">");
            js.Append("          <div class=\"control-top hidden-filters-container-top\"></div>");
            js.Append("        </div>");
            js.Append("      </div>");
            js.Append("      <div class=\"advansed-filter-button btn-show-filters\"><div class=\"inner-text\"><span class=\"text\">" + Resources.UserControlsCommonResource.LblFilterButton + "</span></span></div></div>");
            js.Append("      <div class=\"advansed-filter-input\">");
            js.Append("        <label class=\"advansed-filter-reset btn-reset-filter\"></label>");
            js.Append("        <input class=\"advansed-filter advansed-filter-input advansed-filter-complete\" type=\"text\" placeholder=\"" + Resources.UserControlsCommonResource.LblFilterPlaceholder + "\" />");
            js.Append("      </div>");
            js.Append("    </div>");
            js.Append("    {{if filtervalues.length > 0}}");
            js.Append("      {{tmpl '#template-filter-filtervalues'}}");
            js.Append("    {{/if}}");
            js.Append("    {{if sortervalues.length > 0}}");
            js.Append("      {{tmpl '#template-filter-sortervalues'}}");
            js.Append("    {{/if}}");
            js.Append("  </div><div class=\"advansed-filter-helper\"></div></div>");
            js.Append("</script>");

            js.Append("<script id=\"template-filter-item\" type=\"text/x-jquery-tmpl\">");
            js.Append("  <div class=\"default-value\">");
            js.Append("    <span class=\"title\">{{if filtertitle}}${filtertitle}{{else}}${title}{{/if}}</span>");
            js.Append("      <span class=\"selector-wrapper\">");
            js.Append("        <span class=\"daterange-selector from-daterange-selector\">");
            js.Append("          <span class=\"label\">" + Resources.UserControlsCommonResource.LblFrom + "</span>");
            js.Append("          <span class=\"advansed-filter-dateselector-date dateselector-from-date\">");
            js.Append("            <span class=\"btn-show-datepicker btn-show-datepicker-container\"><span class=\"btn-show-datepicker btn-show-datepicker-title\"></span></span>");
            js.Append("            <span class=\"advansed-filter-datepicker-container asc-datepicker\">");
            js.Append("              <span class=\"control-top dateselector-top\"></span>");
            js.Append("              <span class=\"datepicker-container\"></span>");
            js.Append("            </span>");
            js.Append("          </span>");
            js.Append("        </span>");
            js.Append("        <span class=\"daterange-selector to-daterange-selector\">");
            js.Append("          <span class=\"label\">" + Resources.UserControlsCommonResource.LblTo + "</span>");
            js.Append("          <span class=\"advansed-filter-dateselector-date dateselector-to-date\">");
            js.Append("            <span class=\"btn-show-datepicker btn-show-datepicker-container\"><span class=\"btn-show-datepicker btn-show-datepicker-title\"></span></span>");
            js.Append("            <span class=\"advansed-filter-datepicker-container asc-datepicker\">");
            js.Append("              <span class=\"control-top dateselector-top\"></span>");
            js.Append("              <span class=\"datepicker-container\"></span>");
            js.Append("            </span>");
            js.Append("          </span>");
            js.Append("        </span>");
            js.Append("        {{if options}}");
            js.Append("          <span class=\"combobox-selector\">");
            js.Append("            <select class=\"advansed-filter-combobox\"{{if $data.multiple === true}} multiple=\"multiple\"{{/if}}>");
            js.Append("              {{tmpl(options) '#template-filter-combobox-options'}}");
            js.Append("            </select>");
            js.Append("          </span>");
            js.Append("        {{/if}}");
            js.Append("        <span class=\"group-selector\">");
            //js.Append("          <span class=\"custom-value\"><span class=\"value\"></span>&nbsp;<small>▼</small></span>");
            js.Append("          <span class=\"custom-value\"><span class=\"inner-text\"><span class=\"value\"></span></span></span>");
            //js.Append("          <span class=\"default-value\"><span class=\"value\">" + Resources.UserControlsCommonResource.LblSelect + "</span>&nbsp;<small>▼</small></span>");
            js.Append("          <span class=\"default-value\"><span class=\"inner-text\"><span class=\"value\">" + Resources.UserControlsCommonResource.LblSelect + "</span></span></span>");
            js.Append("        </span>");
            js.Append("        <span class=\"person-selector\">");
            //js.Append("          <span class=\"custom-value\"><span class=\"value\"></span>&nbsp;<small>▼</small></span>");
            js.Append("          <span class=\"custom-value\"><span class=\"inner-text\"><span class=\"value\"></span></span></span>");
            //js.Append("          <span class=\"default-value\"><span class=\"value\">" + Resources.UserControlsCommonResource.LblSelect + "</span>&nbsp;<small>▼</small></span>");
            js.Append("          <span class=\"default-value\"><span class=\"inner-text\"><span class=\"value\">" + Resources.UserControlsCommonResource.LblSelect + "</span></span></span>");
            js.Append("        </span>");
            js.Append("      </span>");
            js.Append("      <span class=\"btn-delete\"></span>");
            js.Append("  </div>");
            js.Append("</script>");

            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "jquery-advansedfilter", "<link rel=\"stylesheet\" type=\"text/css\" href=\"" + WebSkin.GetUserSkin().GetAbsoluteWebPath("/skins/<theme_folder>/jquery-advansedfilter.css") + "\" />", false);
            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "advansedfilter-templates", js.ToString(), false);
            Page.ClientScript.RegisterClientScriptInclude(this.GetType(), "jquery-customcombobox", WebPath.GetPath("js/jquery-customcombobox.js"));
            Page.ClientScript.RegisterClientScriptInclude(this.GetType(), "jquery-advansedfilter", WebPath.GetPath("js/jquery-advansedfilter.js"));

            var groupSelector = Page.LoadControl(GroupSelector.Location) as GroupSelector;
            //groupSelector.ID = "groupSelector";
            groupSelector.JsId = "groupSelector";
            //groupSelector.ClickButtonId = "someGrouSelectorButton11";
            Controls.Add(groupSelector);

            var userSelector = new AdvancedUserSelector {ID = "userSelector"};
            userSelector.IsLinkView = true;
            this.Controls.Add(userSelector);
        }

        protected override void Render(HtmlTextWriter writer)
        {
            //base.Render(writer);

            //ASC.Web.Controls.AdvancedUserSelector UserSelector = new ASC.Web.Controls.AdvancedUserSelector();
            //UserSelector.ID = "userSelector";

            var sb = new StringBuilder();

            sb.Append("<div class=\"clearFix\">");
            sb.Append("  <div id=\"" + (String.IsNullOrEmpty(BlockID) ? ClientID : BlockID) + "\" class=\"advansed-filter empty-filter-list\"><div class=\"advansed-filter-wrapper\">");
            sb.Append("    <label class=\"advansed-filter-label\">" + Resources.UserControlsCommonResource.LblFilter + "</label>");
            sb.Append("    <label class=\"advansed-filter-state btn-start-filter\"></label>");
            sb.Append("    <label class=\"advansed-filter-sort btn-show-sorters\"></label>");
            sb.Append("    <div class=\"advansed-filter-sort-container\">");
            sb.Append("      <span class=\"btn-toggle-sorter\"></span>");
            sb.Append("      <span class=\"title\">" + Resources.UserControlsCommonResource.LblSort + ":&nbsp;<span class=\"value\"></span></span>");
            sb.Append("    </div>");

            sb.Append("    <div class=\"advansed-item-list advansed-sorter-list\"><ul class=\"item-list sorter-list\"></ul></div>");
            sb.Append("    <div class=\"advansed-item-list advansed-filter-list\"><ul class=\"item-list filter-list\"></ul></div>");

            sb.Append("    <div class=\"advansed-filter-control advansed-filter-groupselector\">");
            sb.Append("      <div class=\"advansed-filter-control-container advansed-filter-groupselector-container\">");
            sb.Append("        <div class=\"control-top groupselector-top\"></div>");

            writer.Write(sb.ToString());
            this.Controls[0].RenderControl(writer);

            sb = new StringBuilder();
            sb.Append("      </div>");
            sb.Append("    </div>");

            sb.Append("    <div class=\"advansed-filter-control advansed-filter-userselector\">");
            sb.Append("      <div class=\"advansed-filter-control-container advansed-filter-userselector-container\">");
            sb.Append("        <div class=\"control-top userselector-top\"></div>");

            writer.Write(sb.ToString());
            this.Controls[1].RenderControl(writer);

            sb = new StringBuilder();
            sb.Append("      </div>");
            sb.Append("    </div>");

            sb.Append("    <div class=\"advansed-filter-container\">");
            sb.Append("      <div class=\"advansed-filter-filters empty-list\">");
            sb.Append("        <div class=\"btn-show-hidden-filters\">" + Resources.UserControlsCommonResource.BtnHiddenFilter + "</div>");
            sb.Append("        <div class=\"hidden-filters-container\">");
            sb.Append("          <div class=\"control-top hidden-filters-container-top\"></div>");
            sb.Append("        </div>");
            sb.Append("      </div>");
            sb.Append("      <div class=\"advansed-filter-button btn-show-filters\"><div class=\"inner-text\"><span class=\"text\"><span>" + Resources.UserControlsCommonResource.LblFilterButton + "</span></span></div></div>");
            sb.Append("      <div class=\"advansed-filter-input\">");
            sb.Append("        <label class=\"advansed-filter-reset btn-reset-filter\"></label>");
            sb.Append("        <input type=\"text\" class=\"advansed-filter advansed-filter-input advansed-filter-complete\" placeholder=\"" + Resources.UserControlsCommonResource.LblFilterPlaceholder + "\" />");
            sb.Append("      </div>");
            sb.Append("    </div>");

            sb.Append("  </div><div class=\"advansed-filter-helper\"></div></div>");
            sb.Append("</div>");

            writer.Write(sb.ToString());
        }
    }
}