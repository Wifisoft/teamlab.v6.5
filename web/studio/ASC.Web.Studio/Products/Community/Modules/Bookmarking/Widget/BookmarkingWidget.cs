using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI.WebControls;
using ASC.Web.Core.Utility.Settings;
using ASC.Web.UserControls.Bookmarking.Common.Presentation;
using System.Text;
using ASC.Web.Studio.Utility;
using BookmarkingResource = ASC.Web.UserControls.Bookmarking.Resources.BookmarkingUCResource;
using ASC.Web.Core.Utility.Skins;
using ASC.Web.Controls;
using ASC.Web.Community.Bookmarking.Util;
using ASC.Bookmarking.Business.Permissions;
using ASC.Web.Studio.Controls.Dashboard;

namespace ASC.Web.Community.Bookmarking.Widget
{
    [WidgetPosition(1, 2)]
    public class BookmarkingWidget : WebControl, ICheckEmptyContent
	{
		private int SymbolsCount = 120;

		protected override void RenderContents(System.Web.UI.HtmlTextWriter writer)
		{
			BookmarkingServiceHelper serviceHelper = BookmarkingServiceHelper.GetCurrentInstanse();
			BookmarkingWidgetSettings widgetSettings = SettingsManager.Instance.LoadSettingsFor<BookmarkingWidgetSettings>(serviceHelper.GetCurrentUserID());
			var bookmarksCount = widgetSettings.MaxCountBookmarks;
			var bookmarks = serviceHelper.GetMostRecentBookmarksForWidget(bookmarksCount);
			StringBuilder sb = new StringBuilder();
			foreach (var b in bookmarks)
			{
				sb.Append("<div class='clearFix' style='margin-bottom:20px;'>");
				sb.Append(@"
<table cellspacing='0' cellpadding='0' border='0' style='width: 100%;'><tr valign='top'>");

				sb.Append(@"
	<td style='width:30px; text-align:left;'>");
                sb.AppendFormat("<span class='textMediumDescribe'>{0} {1}</span>", b.Date.ToShortDayMonth(), b.Date.ToShortTimeString());
				sb.Append(@"
	</td>");


				//title
				sb.Append(@"
	<td style='padding-left:10px;'>
		<div style='margin-bottom:5px; word-wrap: break-word;'>");
				
				sb.Append(@"
			<a href='" + VirtualPathUtility.ToAbsolute("~/Products/Community/Modules/Bookmarking/") + BookmarkingServiceHelper.GenerateBookmarkInfoUrl(b.URL) + "'>" + HttpUtility.HtmlEncode(HtmlUtility.GetText(b.Name, SymbolsCount)) + "</a>");
				sb.Append(@"
		</div>");			

				//description
				sb.Append(@"
		<div style='margin-bottom:5px; word-wrap: break-word;'>");
                sb.Append(HttpUtility.HtmlEncode(HtmlUtility.GetText(b.Description, 120, true)));
				sb.Append(@"
		</div>");

				
				//added by
				sb.Append(@"
		<div class='clearFix' style='margin-top:5px;'>");
				sb.AppendFormat(@"			
			<div style='float:left'>
				<img src='{0}' alt=''/>
			</div>
			<div style='float:left; margin-right: 10px;' class='textMediumDescribe'>
				{1}
			</div>", WebImageSupplier.GetAbsoluteWebPath(BookmarkingRequestConstants.UserRaitingOne, BookmarkingConst.BookmarkingId), serviceHelper.GetUserBookmarksCount(b));
				sb.Append(BookmarkingServiceHelper.RenderUserProfile(b.UserCreatorID));

				sb.Append(@"
		</div>
	</td>
</tr>
</table>");
				sb.Append(@"
</div>");
			}

            if (bookmarks.Count > 0 || !BookmarkingPermissionsCheck.PermissionCheckCreateBookmark())
            {
                sb.Append("<div style='margin-top:10px;'>");
                sb.Append("<a href=\"" + VirtualPathUtility.ToAbsolute(BookmarkingRequestConstants.BookmarkingBasePath + "/" + BookmarkingRequestConstants.BookmarkingPageName) + "\">" + BookmarkingResource.AllBookmarks + "</a>");
                sb.Append("</div>");
            }
            else 
            {
                 
                    sb.Append("<div class=\"empty-widget\" style=\"padding:40px; text-align: center;\">" + 
                        string.Format(Resources.BookmarkingResource.NoBookmarksWidgetMessage,
                                      string.Format("<div style=\"padding-top:3px;\"><a class=\"promoAction\" href=\"{0}\">",VirtualPathUtility.ToAbsolute("~/products/community/modules/bookmarking/"+ BookmarkingServiceHelper.GetCreateBookmarkPageUrl())),
                                      "</a></div>")+ "</div>");
                
            }

			writer.Write(sb.ToString());
		}

        #region ICheckEmptyContent Members

        public CheckEmptyContentResult IsEmpty()
        {
            BookmarkingServiceHelper serviceHelper = BookmarkingServiceHelper.GetCurrentInstanse();
            BookmarkingWidgetSettings widgetSettings = SettingsManager.Instance.LoadSettingsFor<BookmarkingWidgetSettings>(serviceHelper.GetCurrentUserID());
            var bookmarksCount = widgetSettings.MaxCountBookmarks;
            var bookmarks = serviceHelper.GetMostRecentBookmarksForWidget(bookmarksCount);

            if (bookmarks.Count > 0)
                return CheckEmptyContentResult.NotEmpty;

            return CheckEmptyContentResult.Empty;
        }

        #endregion
    }
}
