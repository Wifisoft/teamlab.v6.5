using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using AjaxPro;
using ASC.Core;
using ASC.Data.Storage;
using ASC.Web.Core;
using ASC.Web.Core.Utility.Settings;
using ASC.Web.Core.Utility.Skins;
using ASC.Web.Core.WebZones;
using ASC.Web.Studio.Core;
using ASC.Web.Studio.Utility;

namespace ASC.Web.Studio.UserControls.FirstTime
{
    [AjaxNamespace("ModulesController")]
    public partial class Modules : System.Web.UI.UserControl
    {
        public static string Location { get { return "~/UserControls/FirstTime/Modules/Modules.ascx"; } }

        protected void Page_Load(object sender, EventArgs e)
        {
            AjaxPro.Utility.RegisterTypeForAjax(this.GetType());

            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "Modules_style", "<link rel=\"stylesheet\" type=\"text/css\" href=\"" + WebSkin.GetUserSkin().GetAbsoluteWebPath("usercontrols/firsttime/Modules/css/<theme_folder>/Modules.css") + "\">", false);
            Page.ClientScript.RegisterClientScriptInclude(typeof(string), "Modules_script", WebPath.GetPath("usercontrols/firsttime/Modules/js/Modules.js"));

            var items = WebItemManager.Instance.GetItems(WebZoneType.All, ItemAvailableState.All);

            _itemsRepeater.DataSource = GetDataSource();
            _itemsRepeater.DataBind();

        }
        private class Item
        {
            public string Name { get; set; }
            public Guid ID { get; set; }
            public bool Disabled { get; set; }
            public bool SortDisabled { get; set; }
            public int SortOrder { get; set; }
            public string LogoPath { get; set; }
            public string Desciption { get; set; }
        }

        private List<Item> GetDataSource()
        {
            var data = new List<Item>();

            var items = WebItemManager.Instance.GetItems(WebZoneType.StartProductList, ItemAvailableState.All);
                //.GetItems(WebZoneType.All, ItemAvailableState.All);

            foreach (var wi in items)
            {
                if (wi.IsSubItem() /*|| !(wi is IProduct)*/) continue;
                var item = new Item
                               {

                                   ID = wi.ID,
                                   Name = wi.Name,
                                   Disabled = wi.IsDisabled(),
                                   SortDisabled = wi.Context != null && wi.Context.SortDisabled,
                                   LogoPath = wi.GetIconAbsoluteURL(),
                                   Desciption = wi.Description
                               };
                data.Add(item);
            }
            return data;
        }

        [AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public object SaveModules(List<ASC.Web.Core.Utility.Settings.WebItemSettings.WebItemOption> options)
        {
            try
            {
                SecurityContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

                var itemSettings = SettingsManager.Instance.LoadSettings<WebItemSettings>(TenantProvider.CurrentTenantID);

                foreach (var itemOption in options)
                {
                    foreach (var webItemOption in itemSettings.SettingsCollection)
                    {
                        if (webItemOption.ItemID == itemOption.ItemID)
                        {
                            webItemOption.Disabled = itemOption.Disabled;
                            continue;
                        }
                    }
                }

                //foreach (var webItemOption in itemSettings.SettingsCollection)
                //{
                //    foreach (var itemOption in options)
                //    {
                //        if (webItemOption.ItemID == itemOption.ItemID)
                //        {
                //            webItemOption.Disabled = itemOption.Disabled;
                //            continue;
                //        }
                //    }
                //}

                var settings = new WebItemSettings();
                foreach (var opt in itemSettings.SettingsCollection.Select((o, i) => { o.SortOrder = i; return o; }))
                    settings.SettingsCollection.Add(opt);
                SettingsManager.Instance.SaveSettings(settings, TenantProvider.CurrentTenantID);

                return new { Status = 1, Message = Resources.Resource.WizardModulesSaved };
            }
            catch (Exception ex)
            {

                return new { Status = 0, Message = ex.Message };
            }
        }
    }
}