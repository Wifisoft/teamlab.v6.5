using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.IO;

namespace ASC.Web.Controls
{
	[
	ToolboxData("<{0}:ViewSwitcher runat=\"server\"/>"),
	ParseChildren(ChildrenAsProperties = true), PersistChildren(true),
	Themeable(true)
	]
	public class ViewSwitcher : Control
	{

		#region Sort and Tab items

		private List<ViewSwitcherBaseItem> _sortItems;

		[Description("List of tabs."),
		Category("Data"), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)
		]
		public List<ViewSwitcherBaseItem> SortItems
		{
			get
			{
				if (_sortItems == null)
				{
					_sortItems = new List<ViewSwitcherBaseItem>();
				}
				return _sortItems;
			}
			set
			{
				var v = value;
				if (v != null && v is List<ViewSwitcherBaseItem>)
				{
					_sortItems = v;
				}
			}
		}

		protected bool HasSortItems
		{
			get
			{
				return SortItems.Count > 0;
			}
		}

		private List<ViewSwitcherTabItem> _tabItems;


		[Description("List of tabs."),
		Category("Data"), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)
		]
		public List<ViewSwitcherTabItem> TabItems
		{
			get { return _tabItems ?? (_tabItems = new List<ViewSwitcherTabItem>()); }
		    set{ _tabItems = value; }
		}

		protected bool HasTabItems
		{
			get
			{
				return TabItems.Count > 0;
			}
		}

		public bool DisableJavascriptSwitch { get; set; }

		public bool RenderAllTabs { get; set; }

		#endregion

		public string SortItemsHeader { get; set; }

		protected string SortItemsDivID = Guid.NewGuid().ToString();

		protected override void OnInit(EventArgs e)
		{
			foreach (var tab in TabItems)
			{
                if (tab.Visible)
                {
                    this.Controls.Add(tab);
                }
			}
			base.OnInit(e);
		}


		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);
			InitScripts();
		}

		protected override void Render(HtmlTextWriter writer)
		{
			StringBuilder sb = new StringBuilder();
			if (HasTabItems)
			{
				sb.AppendFormat(@"
<div class='viewSwitcher'>
	<ul class='clearFix viewSwitcherAreaWithBottomBorder'>");
				
				foreach (var tab in TabItems)
				{
					tab.SortItemsDivID = SortItemsDivID;
                    if (tab.Visible)
                    {
                        sb.Append(tab.GetSortLink(RenderAllTabs));
                    }
				}
			}

			if (HasSortItems)
			{
				if (HasTabItems)
				{
					sb.Append(@"
		<li align='right' style='float: right; list-style: none;'>
			<div class='clearFix'>");
				}

				sb.Append(@"
				<table cellspacing='0' cellpadding='0'>
					<tr>");

				if (!string.IsNullOrEmpty(SortItemsHeader))
				{
					sb.AppendFormat(@"
						<td class='viewSwitcherItem'>{0}</td>", SortItemsHeader.HtmlEncode());
				}

				foreach (var sortItem in SortItems)
				{
					sb.AppendFormat(@"
						<td>{0}</td>", sortItem.GetSortLink);
				}

				sb.Append(@"
					</tr>
				</table>");


				if (HasTabItems)
				{
					sb.Append(@"
			</div>
		</li>");
				}
			}


			if (HasTabItems)
			{
				sb.Append(@"
	</ul>
</div>");
			}

			writer.Write(sb.ToString());


			foreach (var tab in TabItems)
			{
                if (tab.Visible)
                {
                    if (RenderAllTabs)
                    {
                        tab.RenderTabContent(writer);
                        continue;
                    }
                    if (tab.IsSelected && DisableJavascriptSwitch)
                    {
                        tab.RenderTabContent(writer);
                    }
                }
			}
		}


		#region Init Scripts
		private void InitScripts()
		{
			InitViewSwitcherScripts(Page, TabItems);
		}

		public static void InitViewSwitcherScripts(Page p, List<ViewSwitcherTabItem> tabs)
		{
			if (!p.ClientScript.IsClientScriptIncludeRegistered(typeof(ViewSwitcher), "ViewSwitcherJavaScript"))
			{
				string ViewSwitcherJavaScriptLocation = p.ClientScript.GetWebResourceUrl(typeof(ViewSwitcher), "ASC.Web.Controls.ViewSwitcher.js.viewswitcher.js");
                p.ClientScript.RegisterClientScriptInclude(typeof(ViewSwitcher), "ViewSwitcherJavaScript", ViewSwitcherJavaScriptLocation);
			}


			if (!p.ClientScript.IsClientScriptBlockRegistered(typeof(ViewSwitcher),"ViewSwithcerCssStyle"))
			{
				StringBuilder sb = new StringBuilder();
				sb.AppendFormat("<link rel=\"stylesheet\" text=\"text/css\" href=\"{0}\" />", p.ClientScript.GetWebResourceUrl(typeof(ViewSwitcher), "ASC.Web.Controls.ViewSwitcher.css.viewswitcher.css"));

				((System.Web.UI.HtmlControls.HtmlHead)p.Header).Controls.Add(new LiteralControl(sb.ToString()));
				p.ClientScript.RegisterClientScriptBlock(typeof(ViewSwitcher), "ViewSwithcerCssStyle", "");
			}


			if (!p.ClientScript.IsClientScriptBlockRegistered("ViewSwitcherHideToggleJavaScript"))
			{
				var hideDropdownScript = @"
jq(document).click(
	function(event) 
	{
		jq('div[id^=""viewSwitcherDropdownToggle""]').each(
			function(){
				var elID = this.id.split('viewSwitcherDropdownToggle')[1];
				viewSwitcherDropdownRegisterAutoHide(event, this.id, 'viewSwitcherDropdownList' + elID);
			}
		);
	}
);";

				p.ClientScript.RegisterClientScriptBlock(typeof(string), "ViewSwitcherHideToggleJavaScript", hideDropdownScript, true);
			}


			if (!p.ClientScript.IsClientScriptBlockRegistered("toggleViewSwitcherScript") && tabs != null && tabs.Count > 0)
			{
				try
				{
					ViewSwitcherTabItem tab = null;
					try
					{
						tab = (from t in tabs
							   where t.IsSelected
							   select t).First<ViewSwitcherTabItem>();
					}
					catch
					{
						if (tabs.Count > 0)
						{
							tab = tabs[0];
							tab.IsSelected = true;
						}
					}

					var firstBootScript = @"
jq(document).ready(
	 function() {
		 viewSwitcherToggleTabs("
									+ string.Format("'{0}_ViewSwitcherTab'", tab.DivID) +
								@");															
	}
);";

					p.ClientScript.RegisterClientScriptBlock(typeof(string), "toggleViewSwitcherScript", firstBootScript, true);
				}
				catch { }
			}
		}
		#endregion

		public string RenderViewSwitcher(List<ViewSwitcherBaseItem> sortItems)
		{
			StringBuilder sb = new StringBuilder();
			using (StringWriter sw = new StringWriter(sb))
			{
				using (HtmlTextWriter textWriter = new HtmlTextWriter(sw))
				{
					var viewSwithcer = new ViewSwitcher();
					viewSwithcer.SortItems = sortItems;					
					viewSwithcer.RenderControl(textWriter);
				}
			}

			return System.Text.RegularExpressions.Regex.Replace(sb.ToString(), @"\s+", " ");
		}
	}
}
