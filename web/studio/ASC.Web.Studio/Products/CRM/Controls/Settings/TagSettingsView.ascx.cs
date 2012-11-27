using System;
using System.Linq;
using ASC.Web.CRM.Configuration;
using ASC.Web.Core.Utility.Skins;
using AjaxPro;
using ASC.CRM.Core;
using ASC.Web.CRM.Classes;
using ASC.Web.CRM.Resources;
using ASC.Web.Studio.Controls.Common;

namespace ASC.Web.CRM.Controls.Settings
{
    public partial class TagSettingsView : BaseUserControl
    {
        #region Members

        public static string Location
        {
            get { return PathProvider.GetFileStaticRelativePath("Settings/TagSettingsView.ascx"); }
        }

        #endregion

        #region Events

        protected void Page_Load(object sender, EventArgs e)
        {
            _manageTagPopup.Options.IsPopup = true;

            var entityType = StringToEntityType(Request["view"]);

            _switcherEntityType.SortItemsHeader = CRMCommonResource.Show + ":";
            
            _forContacts.SortLabel = CRMSettingResource.BothPersonAndCompany;
            _forContacts.SortUrl = "settings.aspx?type=tag";
            _forContacts.IsSelected = entityType == EntityType.Contact;


            _forDeals.SortLabel = CRMCommonResource.DealModuleName;
            _forDeals.SortUrl = String.Format("settings.aspx?type=tag&view={0}", EntityType.Opportunity.ToString().ToLower());
            _forDeals.IsSelected = entityType == EntityType.Opportunity;


            _forCases.SortLabel = CRMCommonResource.CasesModuleName;
            _forCases.SortUrl = String.Format("settings.aspx?type=tag&view={0}", EntityType.Case.ToString().ToLower());
            _forCases.IsSelected = entityType == EntityType.Case;


            var tagList = Global.DaoFactory.GetTagDao().GetAllTags(entityType).ToList();

            Page.ClientScript.RegisterClientScriptBlock(typeof(TagSettingsView), "e7302d41-5ae9-4a0a-b156-0b11515ec67c",
                                                        "tagList = "
                                                        + JavaScriptSerializer.Serialize(tagList.ConvertAll(item => new { value = item.HtmlEncode() }))
                                                        + "; ", true);
            Page.ClientScript.RegisterClientScriptBlock(typeof (TagSettingsView), "4b363cea-b734-4f85-91d8-72f70c150a24",
                                                        String.Format("relativeItemsCountArray = {0};", Global.DaoFactory.GetTagDao().GetTagsLinkCountJSON(entityType)),
                                                        true);

            var emptyScreenControl = new EmptyScreenControl
                                         {
                                             ImgSrc = WebImageSupplier.GetAbsoluteWebPath("empty_screen_tags.png", ProductEntryPoint.ID),
                                             Header = CRMSettingResource.EmptyContentTags,
                                             Describe = CRMSettingResource.EmptyContentTagsDescript,
                                             ButtonHTML = String.Format("<a id='addTag' class='linkAddMediumText baseLinkAction' onclick='ASC.CRM.TagSettingsView.showAddTagPanel();' >{0}</a>",
                                                                        CRMSettingResource.AddTag)
                                         };
            _phEmptyContent.Controls.Add(emptyScreenControl);
        }

        #endregion

        #region Methods

        private static EntityType StringToEntityType(string type)
        {
            switch (type)
            {
                case "opportunity":
                    return EntityType.Opportunity;
                case "case":
                    return EntityType.Case;
                default:
                    return EntityType.Contact;
            }
        }
        #endregion
    }
}