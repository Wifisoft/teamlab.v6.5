using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using ASC.Data.Storage;
using ASC.Web.Core.Utility.Skins;
using ASC.Web.Studio.Core.Search;

namespace ASC.Web.Studio.UserControls.Common.Search
{
    public partial class SearchResults : UserControl
    {
        public static string Location
        {
            get { return "~/UserControls/Common/Search/SearchResults.ascx"; }
        }

        internal int MaxResultCount = 5;

        public object DataSourceObj { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            Page.ClientScript.RegisterClientScriptInclude(GetType(), "searchresults_script", WebPath.GetPath("usercontrols/common/search/js/searchresults.js"));
            Page.ClientScript.RegisterClientScriptBlock(GetType(), "searchresults_style", "<link rel=\"stylesheet\" type=\"text/css\" href=\"" + WebSkin.GetUserSkin().GetAbsoluteWebPath("usercontrols/common/search/css/<theme_folder>/searchResults.css") + "\">", false);

            results.ItemDataBound += results_ItemDataBound;
            results.DataSource = DataSourceObj;
            results.DataBind();
        }

        private void results_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var control = ((SearchResult) e.Item.DataItem).PresentationControl;
                if (control == null)
                    return;
                control.Items = ((SearchResult) e.Item.DataItem).Items;
                e.Item.FindControl("resultItems").Controls.Add(control);
            }
        }
    }
}