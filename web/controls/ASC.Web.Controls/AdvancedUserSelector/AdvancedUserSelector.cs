using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using ASC.Core;
using ASC.Core.Users;

namespace ASC.Web.Controls
{
    [ToolboxData("<{0}:AdvancedUserSelector runat=server></{0}:AdvancedUserSelector>")]
    public class AdvancedUserSelector : Control
    {
        #region Fields

        private string _selectorID = Guid.NewGuid().ToString().Replace('-', '_');
        private string _jsObjName;
        private string _linkText = Resources.AdvancedUserSelectorResource.LinkText;

        private bool IsMobileVersion
        {
            get { return Core.Mobile.MobileDetector.IsRequestMatchesMobile(Context); }
        }

        #endregion

        #region Properties

        public int InputWidth { get; set; }
        public Guid SelectedUserId { get; set; }
        public bool IsLinkView { get; set; }

        public string LinkText
        {
            get { return _linkText; }
            set { _linkText = value; }
        }

        public string AdditionalFunction { get; set; }
        public List<UserInfo> UserList { get; set; }
        public List<Guid> DisabledUsers { get; set; }

        #endregion

        #region Methods

        public AdvancedUserSelector()
        {
            InputWidth = 230;
        }

        #endregion

        #region Events

        protected override void OnLoad(EventArgs e)
        {
            base.OnInit(e);
            _jsObjName = String.IsNullOrEmpty(ID) ? "advancedUserSelector" + UniqueID.Replace('$', '_') : ID;

            if (!Page.ClientScript.IsClientScriptIncludeRegistered(GetType(), "ASC_Controls_AdvUserSelector_Script"))
                Page.ClientScript.RegisterClientScriptInclude("ASC_Controls_AdvUserSelector_Script",
                                                              Page.ClientScript.GetWebResourceUrl(GetType(), "ASC.Web.Controls.AdvancedUserSelector.js.AdvUserSelectorScript.js"));

            if (!Page.ClientScript.IsClientScriptBlockRegistered("ASC_Controls_AdvUserSelector_Style"))
                Page.ClientScript.RegisterClientScriptBlock(GetType(), "ASC_Controls_AdvUserSelector_Style",
                                                            "<link href=\"" + Page.ClientScript.GetWebResourceUrl(GetType(), "ASC.Web.Controls.AdvancedUserSelector.css.default.css") + "\" type=\"text/css\" rel=\"stylesheet\"/>", false);


            var scriptInit = new StringBuilder();
            
            scriptInit.AppendFormat("\nASC.Controls.AdvancedUserSelector._profiles = '{0}';\n", new Api.ApiServer().GetApiResponse("api/1.0/people.json?fields=id,displayname,avatarsmall,groups", "GET"));
            scriptInit.AppendFormat("\nASC.Controls.AdvancedUserSelector._groups = '{0}';\n", new Api.ApiServer().GetApiResponse("api/1.0/group.json", "GET"));
            scriptInit.AppendFormat("\nASC.Controls.AdvancedUserSelector.UserNameFormat = {0};\n", (int)UserFormatter.GetUserDisplayDefaultOrder());
            

            if (!Page.ClientScript.IsClientScriptBlockRegistered(GetType(), "ASC_Controls_AdvUserSelector_ScriptInit"))
                Page.ClientScript.RegisterClientScriptBlock(GetType(), "ASC_Controls_AdvUserSelector_ScriptInit",scriptInit.ToString(), true);

            
            var script = new StringBuilder();            

            script.AppendFormat("var {0} = new ASC.Controls.AdvancedUserSelector.UserSelectorPrototype('{1}', '{0}', '&lt;{2}&gt;', '{3}', {4}, {5}, '{6}');\n",
                                _jsObjName,
                                _selectorID,
                                Resources.AdvancedUserSelectorResource.EmptyList,
                                Resources.AdvancedUserSelectorResource.ClearFilter,
                                IsMobileVersion.ToString().ToLower(),
                                IsLinkView.ToString().ToLower(),
                                IsMobileVersion ? _linkText.HtmlEncode().ReplaceSingleQuote() : "");
            
            
            if (UserList != null && UserList.Count > 0)
            {
                if (DisabledUsers != null && DisabledUsers.Count > 0)
                    UserList.RemoveAll(ui => (DisabledUsers.Find(dui => dui.Equals(ui.ID)) != Guid.Empty));

                script.AppendFormat("\n{0}.UserIDs = [", _jsObjName);
                foreach (var u in UserList.SortByUserName())
                {
                    script.AppendFormat("'{0}',", u.ID);
                                       
                }
                if(UserList.Count>0)
                    script.Remove(script.Length-1,1);

                script.Append("];\n");

            }

            if (DisabledUsers != null && DisabledUsers.Count > 0)
            {
                script.AppendFormat("\n{0}.DisabledUserIDs = [", _jsObjName);
                foreach (var u in DisabledUsers)
                {
                    script.AppendFormat("'{0}',", u);

                }              
                script.Remove(script.Length - 1, 1);
                script.Append("];\n");
            }

            Page.ClientScript.RegisterClientScriptBlock(GetType(), Guid.NewGuid().ToString(), script.ToString(), true);

            script = new StringBuilder();

            script.AppendFormat("{0}.AllDepartmentsGroupName = '{1}';\n", _jsObjName, Resources.AdvancedUserSelectorResource.AllDepartments.HtmlEncode().ReplaceSingleQuote());
 
            if (!String.IsNullOrEmpty(AdditionalFunction))
                script.AppendFormat("{0}.AdditionalFunction = {1};", _jsObjName, AdditionalFunction);


            if (!Guid.Empty.Equals(SelectedUserId))
                script.AppendFormat("{0}.SelectedUserId = '{1}';\n", _jsObjName, SelectedUserId);
            else if (IsMobileVersion)
                script.AppendFormat("{0}.SelectedUserId = {0}.Me().find('option:first').attr('selected', 'selected').val();", _jsObjName);

            script.Append("jq(function(){jq(document).click(function(event){\n");
            script.Append(_jsObjName + ".dropdownRegAutoHide(event);\n");
            script.Append("}); });\n");

            Page.ClientScript.RegisterStartupScript(GetType(), Guid.NewGuid().ToString(), script.ToString(), true);

            
        }

        protected override void Render(HtmlTextWriter writer)
        {
            base.Render(writer);

            var sb = new StringBuilder();
            sb.AppendFormat("<div id='{0}'>", _jsObjName);

            if (IsMobileVersion)
            {

                sb.AppendFormat("<select class='comboBox' style='width:{0}px;' onchange='javascript:{1}.SelectUser(this);' >", InputWidth, _jsObjName);
                sb.AppendFormat("<option style='max-width:300px;' value='{0}' {2}>{1}</option>",
                                -1,
                                _linkText.HtmlEncode(),
                                Guid.Empty.Equals(SelectedUserId) ? "selected = 'selected'" : string.Empty);

                var accounts = CoreContext.Authentication.GetUserAccounts().OrderBy(a => a.Department).ThenBy(a => a.Name);

                var department = "";
                foreach (var account in accounts)
                {
                    if (!String.Equals(account.Department, department, StringComparison.InvariantCultureIgnoreCase)
                        && !String.IsNullOrEmpty(account.Department.HtmlEncode()))
                    {
                        if (department != "")
                            sb.Append("</optgroup>");

                        department = account.Department;
                        sb.AppendFormat("<optgroup label='{0}' style='max-width:300px;'>", department.HtmlEncode());
                    }
                    sb.AppendFormat("<option style='max-width:300px;' value='{0}' {2}>{1}</option>",
                                    account.ID,
                                    account.Name.HtmlEncode(),
                                    Equals(account.ID, SelectedUserId) ? "selected = 'selected'" : string.Empty);
                }

                sb.AppendFormat("</select>");

            }
            else
            {

                var valueForInput = Guid.Empty.Equals(SelectedUserId) ? string.Empty : CoreContext.UserManager.GetUsers(SelectedUserId).DisplayUserName().ReplaceSingleQuote();

                if (IsLinkView)
                {
                    sb.AppendFormat("<div><span class='addUserLink' onclick='{0}.OnInputClick({0}, event);'><a class='baseLinkAction linkMedium'>{1}</a><img src='{2}' align='absmiddle' style='margin-left:3px'></span></div>",
                                    _jsObjName,
                                    LinkText.HtmlEncode(),
                                    Page.ClientScript.GetWebResourceUrl(GetType(), "ASC.Web.Controls.AdvancedUserSelector.images.sort_down_black.png"));
                }
                else
                {
                    var peopleImgStyle = valueForInput.Trim() == string.Empty ? "display:none;" : "display:block;";
                    var searchImgStyle = valueForInput.Trim() == string.Empty ? "display:block;" : "display:none;";

                    sb.AppendFormat(@"
                        <table cellspacing='0' cellpadding='1' class='borderBase adv-userselector-inputContainer' width='{0}px'>
                            <tbody>
                                <tr>
                                    <td width='16px'>
                                        <img align='absmiddle' src='{1}' id='peopleImg' style='margin:2px;{7}'/>
                                        <img align='absmiddle' src='{2}' id='searchImg' style='{8}'/>
                                    </td>
                                    <td>
                                        <input type='text' id='inputUserName' autocomplete='off'
                                            oninput='{3}.SuggestUser(event);' onpaste='{3}.SuggestUser(event);' onkeyup='{3}.SuggestUser(event);'
	                                        onclick='{3}.OnInputClick({3}, event);' onkeydown='{3}.ChangeSelection(event);'
                                            class='textEdit' style='width:100%;' value='{5}'/>
                                        <input id='login' name='login' value='{6}' type='hidden'/>
                                    </td>
                                    <td width='20px' onclick='{3}.OnInputClick({3}, event);'>
                                        <img align='absmiddle' src='{4}' style='cursor:pointer;'/>
                                    </td>
                                </tr>
                            </tbody>
                        </table>",
                                    InputWidth + 8,
                                    Page.ClientScript.GetWebResourceUrl(GetType(), "ASC.Web.Controls.AdvancedUserSelector.images.people_icon.png"),
                                    Page.ClientScript.GetWebResourceUrl(GetType(), "ASC.Web.Controls.AdvancedUserSelector.images.search.png"),
                                    _jsObjName,
                                    Page.ClientScript.GetWebResourceUrl(GetType(), "ASC.Web.Controls.AdvancedUserSelector.images.collapse_down_dark.png"),
                                    valueForInput,
                                    SelectedUserId,
                                    peopleImgStyle,
                                    searchImgStyle);
                }


                sb.AppendFormat("<div id='DepsAndUsersContainer' class='adv-userselector-DepsAndUsersContainer' {0}>", IsLinkView ? "style='height:230px'" : string.Empty);

                if (IsLinkView)
                {
                    sb.Append("<div style='margin-bottom: 10px;'>");
                    sb.Append("<div style=width:50%;'>");
                    sb.AppendFormat(@"
                        <table cellspacing='0' cellpadding='1' class='borderBase adv-userselector-inputContainer' width='100%' style='height: 18px;'>
                            <tbody>
                                <tr>
                                    <td>
                                        <input type='text' id='inputUserName' autocomplete='off'
                                            oninput='javascript:{0}.SuggestUser(event);' onpaste='javascript:{0}.SuggestUser(event);' onkeyup='javascript:{0}.SuggestUser(event);'
	                                        onclick='{0}.OnInputClick({0}, event);' onkeydown='{0}.ChangeSelection(event);'
                                            class='textEdit' style='width:100%;' value='{1}'/>
                                    </td>
                                    <td width='16px'>
                                        <img align='absmiddle' src='{2}'
                                         onclick='{0}.ClearFilter();'   
                                         style='cursor:pointer;' title='{3}'/>
                                    </td>
                                </tr>
                            </tbody>
                        </table>",
                                    _jsObjName,
                                    valueForInput,
                                    Page.ClientScript.GetWebResourceUrl(GetType(), "ASC.Web.Controls.AdvancedUserSelector.images.cross_grey.png"),
                                    Resources.AdvancedUserSelectorResource.ClearFilter);

                    sb.AppendFormat("<input id='login' name='login' value='{0}' type='hidden'/>", SelectedUserId);
                    sb.Append("</div>");
                    sb.Append("</div>");
                }

                sb.Append("  <div id='divUsers' class='adv-userselector-users'></div>");
                sb.Append("  <div id='divDepartments' class='adv-userselector-deps'></div>");
                sb.Append("</div>");

            }
            sb.Append("</div>");

            writer.Write(sb.ToString());
        }

        #endregion

    }
}