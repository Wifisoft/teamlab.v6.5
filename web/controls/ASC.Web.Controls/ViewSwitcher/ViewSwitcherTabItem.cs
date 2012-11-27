using System;
using System.Text;
using System.Web.UI;

namespace ASC.Web.Controls
{

	public class ViewSwitcherTabItem : Control
	{
		public string OnClickText { get; set; }

		public string TabName { get; set; }

        public string TabAnchorName { get; set; }

		private bool selected;

		public bool IsSelected
		{
			get
			{
				return selected;
			}
			set
			{
				selected = value;
			}
		}

		public bool SkipRender { get; set; }

		public string SortItemsDivID { get; set; }

		public bool HideSortItems { get; set; }

		public string DivID = Guid.NewGuid().ToString();

		public string GetSortLink(bool renderAllTabs)
		{
           var tabCssName = String.IsNullOrEmpty(TabAnchorName) ?
               (IsSelected ? "viewSwitcherTabSelected" : "viewSwitcherTab") :
               String.Format("{0} viewSwitcherTab_{1}",
               IsSelected ? "viewSwitcherTabSelected" : "viewSwitcherTab", TabAnchorName);
            
            var javascriptText = string.Format(@" onclick=""{0} {1} viewSwitcherToggleTabs(this.id);"" ",
                String.IsNullOrEmpty(OnClickText) ? String.Empty : OnClickText + ";",
                renderAllTabs && !String.IsNullOrEmpty(TabAnchorName) ? String.Format("ASC.Controls.AnchorController.move('{0}');", TabAnchorName) : "");

			var sb = new StringBuilder();
			sb.AppendFormat(@"<li id='{0}_ViewSwitcherTab' class='{1}' {2}>{3}</li>", DivID, tabCssName, javascriptText, TabName.HtmlEncode());
			return sb.ToString();
		}

		public void RenderTabContent(HtmlTextWriter writer)
		{
			if (!SkipRender)
			{
				writer.Write(string.Format("<div id='{0}'{1}>", DivID, IsSelected ? string.Empty : " style='display: none;'"));
				this.RenderControl(writer);
				writer.Write("</div>");
			}
		}
	}
}
