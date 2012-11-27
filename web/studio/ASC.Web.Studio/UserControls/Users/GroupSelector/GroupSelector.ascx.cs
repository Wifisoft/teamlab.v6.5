using System;
using System.Linq;
using System.Web.UI;
using ASC.Data.Storage;
using ASC.Web.Core.Utility.Skins;
using ASC.Core;
using System.Text;

namespace ASC.Web.Studio.UserControls.Users
{
    public partial class GroupSelector : UserControl
    {
        public static string Location
        {
            get { return "~/UserControls/Users/GroupSelector/GroupSelector.ascx"; }
        }

        private bool IsMobileVersion
        {
            get { return Web.Core.Mobile.MobileDetector.IsRequestMatchesMobile(Context); }
        }

        private string _linkText = Core.Users.CustomNamingPeople.Substitute<Resources.Resource>("AddGroupsForSharingButton");

        protected string _selectorID = Guid.NewGuid().ToString().Replace('-', '_');

        public string JsId { get; set; }
        public bool WithGroupEveryone { get; set; }
        public bool WithGroupAdmin { get; set; }

        public string LinkText
        {
            get { return _linkText; }
            set { _linkText = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(JsId))
                JsId = "groupSelector_" + _selectorID;

            Page.ClientScript.RegisterClientScriptInclude(typeof (string), "groupselector_script", WebPath.GetPath("usercontrols/users/groupselector/js/groupselector.js"));
            Page.ClientScript.RegisterClientScriptBlock(GetType(), "groupselector_style", "<link rel=\"stylesheet\" type=\"text/css\" href=\"" + WebSkin.GetUserSkin().GetAbsoluteWebPath("usercontrols/users/groupselector/css/<theme_folder>/groupselector.css") + "\">", false);

            var sb = new StringBuilder();
            sb.AppendFormat(" var {0} = new ASC.Controls.GroupSelector('{1}', {2});",
                            JsId,
                            _selectorID,
                            IsMobileVersion.ToString().ToLower());

            var strAppendGroup = "{0}.Groups.push({{Id : '{1}', Name : '{2}'}});";
            if (IsMobileVersion)
                sb.AppendFormat(strAppendGroup, JsId, -1, Resources.UserControlsCommonResource.LblSelect.HtmlEncode().ReplaceSingleQuote());
            if (WithGroupEveryone)
                sb.AppendFormat(strAppendGroup, JsId, ASC.Core.Users.Constants.GroupEveryone.ID, Resources.UserControlsCommonResource.Everyone.HtmlEncode().ReplaceSingleQuote());
            if (WithGroupAdmin)
                sb.AppendFormat(strAppendGroup, JsId, ASC.Core.Users.Constants.GroupAdmin.ID, Resources.UserControlsCommonResource.Admin.HtmlEncode().ReplaceSingleQuote());

            foreach (var grp in CoreContext.GroupManager.GetGroups().OrderBy(g => g.Name))
            {
                sb.AppendFormat(strAppendGroup, JsId, grp.ID, grp.Name.HtmlEncode().ReplaceSingleQuote());
            }

            Page.ClientScript.RegisterClientScriptBlock(GetType(), "group_selector_script_" + _selectorID, sb.ToString(), true);

            var str = IsMobileVersion ?
                                          @"<script id=""groupSelectorListTemplate"" type=""text/x-jquery-tmpl"">
                                            {{each(i, gpr) Groups}} 
                                                 <option value=""${gpr.Id}"" style='max-width:300px;'>
                                                     ${gpr.Name}
                                                 </option>
                                            {{/each}}
                                        </script>"
                          : @"<script id=""groupSelectorListTemplate"" type=""text/x-jquery-tmpl"">
                                {{each(i, gpr) Groups}} 
                                     <div class=""group"" data=""${gpr.Id}"" title=""${gpr.Name}"">
                                         ${gpr.Name}
                                     </div>
                                {{/each}}
                            </script>";
            Page.ClientScript.RegisterClientScriptBlock(GetType(), "group_selector_tmpl_script", str, false);
        }
    }
}