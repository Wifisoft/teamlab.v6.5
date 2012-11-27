using System;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;
using AjaxPro;
using ASC.Core;
using ASC.Web.Community.News.Code.DAO;
using ASC.Web.Core.Utility.Settings;
using ASC.Web.Studio.Controls.Dashboard;

namespace ASC.Web.Community.News.Code.Module
{
	[AjaxNamespace("FeedWidget")]
    [WidgetPosition(2, 0)]
	public class FeedWidget : WebControl, ICheckEmptyContent
	{
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			Utility.RegisterTypeForAjax(typeof(FeedWidget));
		}

		protected override void RenderContents(System.Web.UI.HtmlTextWriter writer)
		{
			base.RenderContents(writer);
			writer.Write(RenderWidget());
		}

		private static string RenderWidget()
		{
			var widgetSettings = SettingsManager.Instance.LoadSettingsFor<FeedWidgetSettings>(SecurityContext.CurrentAccount.ID);
			var widget = new StringBuilder();
			widget.Append("<div id=\"Feed_DataContent\">");

            var feeds = FeedStorageFactory.Create().GetFeeds(FeedType.AllNews, Guid.Empty, widgetSettings.NewsCount, 0);
            if (feeds.Count > 0)
            {

                foreach (var feed in feeds)
                {
                    widget.Append(@"<div style=""padding-bottom: 10px;"">");
                    widget.Append("<table cellspacing='0' cellpadding='0' border='0'><tr valign='top'><td width='25'>");
                    widget.AppendFormat(@"<span class=""textMediumDescribe"">{0}<br/>{1}</span>", feed.Date.ToShortDayMonth(), feed.Date.ToShortTimeString());
                    widget.Append("</td>");
                    widget.Append("<td>");
                    widget.Append("<div style=\"padding-left: 10px;\">");
                    widget.AppendFormat(@"<a href=""{0}"">{1}</a>", FeedUrls.GetFeedUrl(feed.Id), feed.Caption.HtmlEncode());
                    widget.Append("</div>");
                    widget.Append("</td>");
                    widget.Append("</tr>");
                    widget.Append("</table>");
                    widget.Append(@"</div>");
                }

                widget.Append(@"<div style=""margin-top: 10px;"">");
                widget.AppendFormat(@"<a href=""{1}"">{0}</a>", Resources.NewsResource.SeeAllNews, FeedUrls.MainPageUrl);
                widget.Append(@"</div>");
            }
            else
            {
                widget.Append("<div class=\"empty-widget\" style=\"padding:40px; text-align: center;\">" + 
                    String.Format(Resources.NewsResource.NoFeedWidgetMessage,
                                  string.Format("<div style=\"padding-top:3px;\"><a class=\"promoAction\" href=\"{0}\">",VirtualPathUtility.ToAbsolute("~/products/community/modules/news/editnews.aspx")),
                                  "</a></div>")
                                  + "</div>");
            
            }


			widget.Append("</div>");
			return widget.ToString();
		}

		[AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
		public static string UpdateContent()
		{
			return RenderWidget();
		}

        #region ICheckEmptyContent Members

        public CheckEmptyContentResult IsEmpty()
        {
            var widgetSettings = SettingsManager.Instance.LoadSettingsFor<FeedWidgetSettings>(SecurityContext.CurrentAccount.ID);
			var feeds = FeedStorageFactory.Create().GetFeeds(FeedType.AllNews, Guid.Empty, widgetSettings.NewsCount, 0);
            if (feeds.Count > 0)
                return CheckEmptyContentResult.NotEmpty;

            return CheckEmptyContentResult.Empty;
        }

        #endregion
    }
}