using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using ASC.Core;
using ASC.Core.Billing;
using ASC.Web.Core;
using ASC.Web.Core.Utility.Skins;
using ASC.Web.Studio.Core;
using ASC.Web.Studio.UserControls.Management;

namespace ASC.Web.Studio.UserControls.Statistics
{
    public partial class ProductQuotes : UserControl
    {
        public static string Location
        {
            get { return "~/UserControls/Statistics/ProductQuotes/ProductQuotes.ascx"; }
        }

        protected Tariff _tariff;

        public long CurrentSize { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "productquotes_style", "<link rel=\"stylesheet\" type=\"text/css\" href=\"" + WebSkin.GetUserSkin().GetAbsoluteWebPath("usercontrols/statistics/productquotes/css/<theme_folder>/productquotes_style.css") + "\">", false);

            _tariff = CoreContext.TenantManager.GetTariff(CoreContext.TenantManager.GetCurrentTenant().TenantId);

            var data = new List<object>();
            foreach (var item in WebItemManager.Instance.GetItems(Web.Core.WebZones.WebZoneType.All, ItemAvailableState.All))
            {
                if (item.Context == null || item.Context.SpaceUsageStatManager == null)
                    continue;

                data.Add(new Product() {Id = item.ID, Name = item.Name, Icon = item.GetIconAbsoluteURL()});
            }

            _itemsRepeater.ItemDataBound += new System.Web.UI.WebControls.RepeaterItemEventHandler(_itemsRepeater_ItemDataBound);
            _itemsRepeater.DataSource = data;
            _itemsRepeater.DataBind();

            if (!CoreContext.Configuration.Standalone)
            {
                _buyNowHolder.Controls.Add(LoadControl(BuyNow.Location));
            }
        }

        private void _itemsRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {

            var product = e.Item.DataItem as Product;
            var webItem = WebItemManager.Instance[product.Id];

            var data = new List<object>();
            var items = webItem.Context.SpaceUsageStatManager.GetStatData();

            foreach (var it in items)
            {
                data.Add(new {Name = it.Name, Icon = it.ImgUrl, Size = FileSizeComment.FilesSizeToString(it.SpaceUsage), Url = it.Url});
            }

            if (items.Count == 0)
            {
                e.Item.FindControl("_emptyUsageSpace").Visible = true;
                e.Item.FindControl("_showMorePanel").Visible = false;

            }
            else
            {
                var repeater = (Repeater) e.Item.FindControl("_usageSpaceRepeater");
                repeater.DataSource = data;
                repeater.DataBind();


                e.Item.FindControl("_showMorePanel").Visible = (items.Count > 10);
                e.Item.FindControl("_emptyUsageSpace").Visible = false;
            }
        }

        protected String RenderCreatedDate()
        {
            return String.Format("{0}", TenantStatisticsProvider.GetCreationDate().ToShortDateString());
        }

        protected String RenderUsersTotal()
        {
            return TenantStatisticsProvider.GetUsersCount().ToString();
        }

        protected String GetMaxTotalSpace()
        {
            return FileSizeComment.FilesSizeToString(CoreContext.TenantManager.GetTenantQuota(CoreContext.TenantManager.GetCurrentTenant().TenantId).MaxTotalSize);
        }

        protected String RenderUsedSpace()
        {
            var used = TenantStatisticsProvider.GetUsedSize();
            return FileSizeComment.FilesSizeToString(used);
        }

        protected sealed class Product
        {
            public Guid Id { get; set; }
            public String Name { get; set; }
            public String Icon { get; set; }
            public long Size { get; set; }
        }
    }
}