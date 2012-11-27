using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web.UI;
using ASC.Web.Studio.Controls.Common;
using System.Web;

namespace ASC.Web.Community.Bookmarking.Util
{
	public class BookmarkingNavigationItem : NavigationItem
	{

		public string BookmarkingClientID { get; set; }

		public bool DisplayOnPage { get; set; }

		protected override void RenderContents(HtmlTextWriter writer)
		{
			if (String.IsNullOrEmpty(BookmarkingClientID))
			{
				base.RenderContents(writer);
				return;
			}
			if (!String.IsNullOrEmpty(Name) && !String.IsNullOrEmpty(URL))
			{
				var display = DisplayOnPage ? "block" : "none";
				
				writer.Write(
					String.Format("<a href=\"{0}\" title=\"{1}\" id='{2}' style='display:{3}' class='linkAction" + (IsPromo ? " promoAction" : "") + "'>",
						ResolveUrl(this.URL), HttpUtility.HtmlEncode(Description), BookmarkingClientID, display
					));
				writer.Write(HttpUtility.HtmlEncode(Name));
				writer.Write("</a>");
			}
			else
				base.RenderContents(writer);
		}
	}
}
