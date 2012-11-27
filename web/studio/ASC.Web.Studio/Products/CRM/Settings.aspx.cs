#region Import

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ASC.CRM.Core;
using ASC.Web.Controls;
using ASC.Web.CRM.Classes;
using ASC.Web.CRM.Controls.Settings;
using ASC.Web.CRM.Resources;
using ASC.Web.Studio.Controls.Common;
using ASC.Web.Studio.Utility;
using ASC.Web.CRM.Controls.Contacts;
using ASC.Data.Storage;

#endregion

namespace ASC.Web.CRM
{
    public partial class Settings : BasePage
    {
        protected override void PageLoad()
        {
            if (!CRMSecurity.IsAdmin)
                Response.Redirect(PathProvider.StartURL());

            Master.DisabledSidePanel = false;

            var typeValue = (HttpContext.Current.Request["type"] ?? "common").ToLower();
            ListItemView listItemViewControl;

            switch (typeValue)
            {

                case "common":

                    CommonContainerHolder.Controls.Add(LoadControl(CommonSettingsView.Location));

                    Master.BreadCrumbs.Add(new BreadCrumb
                    {
                        Caption = CRMSettingResource.CommonSettings
                    });

                    Title = HeaderStringHelper.GetPageTitle(CRMSettingResource.CommonSettings, Master.BreadCrumbs);
                   
                    break;
                case "deal_milestone":
                    var dealMilestoneViewControl = (DealMilestoneView)LoadControl(DealMilestoneView.Location);
                    CommonContainerHolder.Controls.Add(dealMilestoneViewControl);
                   
                    Master.BreadCrumbs.Add(new BreadCrumb
                    {
                        Caption = CRMDealResource.DealMilestone
                    });
                    Title = HeaderStringHelper.GetPageTitle(CRMDealResource.DealMilestone, Master.BreadCrumbs);
                   
                    break;
                case "task_category":
                    listItemViewControl = (ListItemView)LoadControl(ListItemView.Location);
                    listItemViewControl.CurrentTypeValue = ListType.TaskCategory;
                    listItemViewControl.AddButtonText = CRMSettingResource.AddThisCategory;
                    listItemViewControl.AddPopupWindowText = CRMSettingResource.AddNewCategory;
                    listItemViewControl.AjaxProgressText = CRMSettingResource.AddCategoryInProgressing;
                    listItemViewControl.DeleteText = CRMSettingResource.DeleteCategory;
                    listItemViewControl.EditText = CRMSettingResource.EditCategory;
                    listItemViewControl.EditPopupWindowText = CRMSettingResource.EditSelectedCategory;
                    listItemViewControl.DescriptionText = CRMSettingResource.DescriptionTextTaskCategory;
                    listItemViewControl.DescriptionTextEditDelete = CRMSettingResource.DescriptionTextTaskCategoryEditDelete;
                    CommonContainerHolder.Controls.Add(listItemViewControl);
                    Master.BreadCrumbs.Add(new BreadCrumb
                    {
                        Caption = CRMTaskResource.TaskCategories
                    });
                    Title = HeaderStringHelper.GetPageTitle(CRMTaskResource.TaskCategories, Master.BreadCrumbs);
                    break;
                case "history_category":
                    listItemViewControl = (ListItemView)LoadControl(ListItemView.Location);
                    listItemViewControl.CurrentTypeValue = ListType.HistoryCategory;
                    listItemViewControl.AddButtonText = CRMSettingResource.AddThisCategory;
                    listItemViewControl.AddPopupWindowText = CRMSettingResource.AddNewCategory;
                    listItemViewControl.AjaxProgressText = CRMSettingResource.AddCategoryInProgressing;
                    listItemViewControl.DeleteText = CRMSettingResource.DeleteCategory;
                    listItemViewControl.EditText = CRMSettingResource.EditCategory;
                    listItemViewControl.EditPopupWindowText = CRMSettingResource.EditSelectedCategory;
                    listItemViewControl.DescriptionText = CRMSettingResource.DescriptionTextHistoryCategory;
                    listItemViewControl.DescriptionTextEditDelete = CRMSettingResource.DescriptionTextHistoryCategoryEditDelete;
                    CommonContainerHolder.Controls.Add(listItemViewControl);
                    Master.BreadCrumbs.Add(new BreadCrumb
                    {
                        Caption = CRMSettingResource.HistoryCategories
                    });
                    Title = HeaderStringHelper.GetPageTitle(CRMSettingResource.HistoryCategories, Master.BreadCrumbs);
                    break;

                case "contact_stage":
                    listItemViewControl = (ListItemView)LoadControl(ListItemView.Location);
                    listItemViewControl.CurrentTypeValue = ListType.ContactStatus;
                    listItemViewControl.AddButtonText = CRMSettingResource.AddThisStage;
                    listItemViewControl.AddPopupWindowText = CRMSettingResource.AddNewStage;
                    listItemViewControl.AjaxProgressText = CRMSettingResource.AddContactStageInProgressing;
                    listItemViewControl.DeleteText = CRMSettingResource.DeleteContactStage;
                    listItemViewControl.EditText = CRMSettingResource.EditContactStage;
                    listItemViewControl.EditPopupWindowText = CRMSettingResource.EditSelectedContactStage;
                    listItemViewControl.DescriptionText = CRMSettingResource.DescriptionTextContactStage;
                    listItemViewControl.DescriptionTextEditDelete = CRMSettingResource.DescriptionTextContactStageEditDelete;
                    CommonContainerHolder.Controls.Add(listItemViewControl);
                    Master.BreadCrumbs.Add(new BreadCrumb
                    {
                        Caption = CRMContactResource.ContactStage
                    });
                    Title = HeaderStringHelper.GetPageTitle(CRMContactResource.ContactStage, Master.BreadCrumbs);
                    break;
                case "tag":
                    var tagSettingsViewControl = (TagSettingsView)LoadControl(TagSettingsView.Location);
                    CommonContainerHolder.Controls.Add(tagSettingsViewControl);

                    Master.BreadCrumbs.Add(new BreadCrumb
                    {
                        Caption = CRMCommonResource.Tags
                    });
                    Title = HeaderStringHelper.GetPageTitle(CRMCommonResource.Tags, Master.BreadCrumbs);

                    break;

                case "web_to_lead_form":
                    
                    CommonContainerHolder.Controls.Add(LoadControl(WebToLeadFormView.Location));

                    Master.BreadCrumbs.Add(new BreadCrumb
                    {
                        Caption = CRMSettingResource.WebToLeadsForm
                    });

                    Title = HeaderStringHelper.GetPageTitle(CRMSettingResource.WebToLeadsForm, Master.BreadCrumbs);
                    break;
                case "task_template":

                    CommonContainerHolder.Controls.Add(LoadControl(TaskTemplateView.Location));

                    Master.BreadCrumbs.Add(new BreadCrumb
                    {
                        Caption = CRMSettingResource.TaskTemplates
                    });

                    Title = HeaderStringHelper.GetPageTitle(CRMSettingResource.TaskTemplates, Master.BreadCrumbs);
                    break;
                default:

                    Master.BreadCrumbs.Add(new BreadCrumb
                    {
                        Caption = CRMSettingResource.CustomFields
                    });

                    typeValue = "custom_field";

                    CommonContainerHolder.Controls.Add(LoadControl(CustomFieldsView.Location));
                    Title = HeaderStringHelper.GetPageTitle(CRMSettingResource.CustomFields, Master.BreadCrumbs);
                    break;
            }

            SideNavigatorPanel.Controls.Add(new NavigationItem
            {
                Selected = String.Compare(typeValue, "common", true) == 0,
                Name = CRMSettingResource.CommonSettings,
                URL = "settings.aspx?type=common"
            });

            SideNavigatorPanel.Controls.Add(new NavigationItem
            {
                Selected = String.Compare(typeValue, "custom_field", true) == 0,
                Name = CRMSettingResource.CustomFields,
                URL = "settings.aspx?type=custom_field"
            });

            SideNavigatorPanel.Controls.Add(new NavigationItem
            {
                Selected = String.Compare(typeValue, "deal_milestone", true) == 0,
                Name = CRMDealResource.DealMilestone,
                URL = "settings.aspx?type=deal_milestone"
            });

            SideNavigatorPanel.Controls.Add(new NavigationItem
            {
                Selected = String.Compare(typeValue, "contact_stage", true) == 0,
                Name = CRMContactResource.ContactStage,
                URL = "settings.aspx?type=contact_stage"
            });

            SideNavigatorPanel.Controls.Add(new NavigationItem
            {
                Selected = String.Compare(typeValue, "task_category", true) == 0,
                Name = CRMTaskResource.TaskCategories,
                URL = "settings.aspx?type=task_category"
            });

            SideNavigatorPanel.Controls.Add(new NavigationItem
            {
                Selected = String.Compare(typeValue, "history_category", true) == 0,
                Name = CRMSettingResource.HistoryCategories,
                URL = "settings.aspx?type=history_category"
            });

            SideNavigatorPanel.Controls.Add(new NavigationItem
            {
                Selected = String.Compare(typeValue, "tag", true) == 0,
                Name = CRMCommonResource.Tags,
                URL = "settings.aspx?type=tag"
            });

            SideNavigatorPanel.Controls.Add(new NavigationItem
            {
                Selected = String.Compare(typeValue, "web_to_lead_form", true) == 0,
                Name = CRMSettingResource.WebToLeadsForm,
                URL = "settings.aspx?type=web_to_lead_form"
            });

            //SideNavigatorPanel.Controls.Add(new NavigationItem
            //{
            //    Selected = String.Compare(typeValue, "task_template", true) == 0,
            //    Name = CRMSettingResource.TaskTemplates,
            //    URL = "settings.aspx?type=task_template"
            //});
        }
    }
}
