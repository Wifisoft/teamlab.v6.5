using System;
using System.Collections.Generic;
using AjaxPro;
using ASC.Data.Storage;
using ASC.Web.Core.Utility.Skins;
using ASC.Web.Studio.Core.Users;
using ASC.Core;
using ASC.Web.Studio.Core;

namespace ASC.Web.Studio.UserControls.Management
{
    [AjaxNamespace("NamingPeopleContentController")]
    public partial class NamingPeopleSettingsContent : System.Web.UI.UserControl
    {
        public static string Location { get { return "~/UserControls/Management/NamingPeopleSettings/NamingPeopleSettingsContent.ascx"; } }
        protected void Page_Load(object sender, EventArgs e)
        {
            AjaxPro.Utility.RegisterTypeForAjax(this.GetType());
            Page.ClientScript.RegisterClientScriptInclude(typeof(string), "peoplenamecontent_script", WebPath.GetPath("usercontrols/management/namingpeoplesettings/js/namingpeoplecontent.js"));
            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "namingpeoplecontent_style", "<link rel=\"stylesheet\" type=\"text/css\" href=\"" + WebSkin.GetUserSkin().GetAbsoluteWebPath("usercontrols/management/namingpeoplesettings/css/<theme_folder>/namingpeople.css") + "\">", false);


            var schemas = new List<object>();
            var currentSchemaId = CustomNamingPeople.Current.Id;

            foreach (var schema in CustomNamingPeople.GetSchemas())
            {
                schemas.Add(new
                {
                    Id = schema.Key,
                    Name = schema.Value,
                    Current = string.Equals(schema.Key, currentSchemaId, StringComparison.InvariantCultureIgnoreCase)
                });
            }

            namingSchemaRepeater.DataSource = schemas;
            namingSchemaRepeater.DataBind();
        }

        [AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public object GetPeopleNames(string schemaId)
        {
            SecurityContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

            var names = CustomNamingPeople.GetPeopleNames(schemaId);

            return new
            {
                Id = names.Id,
                UserCaption = names.UserCaption,
                UsersCaption = names.UsersCaption,
                GroupCaption = names.GroupCaption,
                GroupsCaption = names.GroupsCaption,
                UserPostCaption = names.UserPostCaption,
                RegDateCaption = names.RegDateCaption,
                GroupHeadCaption = names.GroupHeadCaption,
                GlobalHeadCaption = names.GlobalHeadCaption,
                AddUsersCaption = names.AddUsersCaption
            };

        }

        [AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public object SaveNamingSettings(string schemaId)
        {
            try
            {
                SecurityContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

                CustomNamingPeople.SetPeopleNames(schemaId);
                return new { Status = 1, Message = Resources.Resource.SuccessfullySaveSettingsMessage };

            }
            catch (Exception e)
            {
                return new { Status = 0, Message = e.Message };
            }
        }


        [AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public object SaveCustomNamingSettings(string usrCaption, string usrsCaption, string addUsersCaption, string grpCaption, string grpsCaption,
                                               string usrStatusCaption, string regDateCaption,
                                               string grpHeadCaption, string globalHeadCaption)
        {
            try
            {
                SecurityContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

                usrCaption = (usrCaption ?? "").Trim();
                usrsCaption = (usrsCaption ?? "").Trim();
                grpCaption = (grpCaption ?? "").Trim();
                grpsCaption = (grpsCaption ?? "").Trim();
                usrStatusCaption = (usrStatusCaption ?? "").Trim();
                regDateCaption = (regDateCaption ?? "").Trim();
                grpHeadCaption = (grpHeadCaption ?? "").Trim();
                globalHeadCaption = (globalHeadCaption ?? "").Trim();
                addUsersCaption = (addUsersCaption ?? "").Trim();

                if (String.IsNullOrEmpty(usrCaption)
                    || String.IsNullOrEmpty(usrsCaption)
                    || String.IsNullOrEmpty(addUsersCaption)
                    || String.IsNullOrEmpty(grpCaption)
                    || String.IsNullOrEmpty(grpsCaption)
                    || String.IsNullOrEmpty(usrStatusCaption)
                    || String.IsNullOrEmpty(regDateCaption)
                    || String.IsNullOrEmpty(grpHeadCaption)
                    || String.IsNullOrEmpty(globalHeadCaption))
                {
                    throw new Exception(Resources.Resource.ErrorEmptyFields);
                }

                var names = new PeopleNamesItem
                {
                    UserCaption = usrCaption.Substring(0, Math.Min(30, usrCaption.Length)),
                    UsersCaption = usrsCaption.Substring(0, Math.Min(30, usrsCaption.Length)),
                    GroupCaption = grpCaption.Substring(0, Math.Min(30, grpCaption.Length)),
                    GroupsCaption = grpsCaption.Substring(0, Math.Min(30, grpsCaption.Length)),
                    UserPostCaption = usrStatusCaption.Substring(0, Math.Min(30, usrStatusCaption.Length)),
                    RegDateCaption = regDateCaption.Substring(0, Math.Min(30, regDateCaption.Length)),
                    GroupHeadCaption = grpHeadCaption.Substring(0, Math.Min(30, grpHeadCaption.Length)),
                    GlobalHeadCaption = globalHeadCaption.Substring(0, Math.Min(30, globalHeadCaption.Length)),
                    AddUsersCaption = addUsersCaption.Substring(0, Math.Min(30, addUsersCaption.Length))
                };

                CustomNamingPeople.SetPeopleNames(names);

                return new { Status = 1, Message = Resources.Resource.SuccessfullySaveSettingsMessage };

            }
            catch (Exception e)
            {
                return new { Status = 0, Message = e.Message };
            }
        }
    }
}