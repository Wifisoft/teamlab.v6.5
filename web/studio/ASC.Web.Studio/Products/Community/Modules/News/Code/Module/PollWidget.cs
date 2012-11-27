using System;
using System.Globalization;
using System.Web;
using System.Web.UI.WebControls;
using ASC.Core;
using ASC.Core.Tenants;
using ASC.Web.Community.News.Code.DAO;
using ASC.Web.Controls;
using ASC.Web.Studio.Controls.Dashboard;
using ASC.Web.Studio.Core;

namespace ASC.Web.Community.News.Code.Module
{
    [WidgetPosition(2, 1)]
	public class PollWidget : WebControl, ICheckEmptyContent
	{
		private PollForm pollForm = new PollForm() { VoteHandlerType = typeof(PollVoteHandler) };

		private FeedPoll poll;

		public PollWidget()
		{
			Controls.Add(pollForm);
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			try
			{
				var storage = FeedStorageFactory.Create();
				var polls = storage.GetFeeds(FeedType.Poll, Guid.Empty, 1, 0);
				if (0 < polls.Count) poll = storage.GetFeed(polls[0].Id) as FeedPoll;
			}
			catch { }
			if (poll == null) return;

            bool isMakeVote = TenantUtil.DateTimeNow() <= poll.EndDate && !poll.IsUserVote(SecurityContext.CurrentAccount.ID.ToString());

			pollForm.VoteHandlerType = typeof(PollVoteHandler);
			pollForm.Answered = !isMakeVote || SecurityContext.DemoMode || (SetupInfo.WorkMode == WorkMode.Promo);
			pollForm.Name = poll.Caption.HtmlEncode();
			pollForm.PollID = poll.Id.ToString(CultureInfo.CurrentCulture);
			pollForm.Singleton = (poll.PollType == FeedPollType.SimpleAnswer);
			pollForm.AdditionalParams = poll.Id.ToString(CultureInfo.CurrentCulture);
			foreach (var variant in poll.Variants)
			{
				pollForm.AnswerVariants.Add(new PollForm.AnswerViarint()
				{
					ID = variant.ID.ToString(CultureInfo.CurrentCulture),
                    Name = variant.Name,
					VoteCount = poll.GetVariantVoteCount(variant.ID)
				});
			}

		}

		protected override void RenderContents(System.Web.UI.HtmlTextWriter writer)
		{
			writer.Write("<div id=\"Poll_DataContent\">");
			writer.Write(@"<div>");

            if (poll != null)
            {
                writer.Write("<div style='padding-bottom:15px;'><a class=\"linkHeaderLightMedium\" href=\"" + FeedUrls.GetFeedUrl(poll.Id) + "\">" + pollForm.Name + "</a></div>");
                pollForm.RenderControl(writer);

                writer.Write("<div style=\"margin-top: 10px;\">");
                writer.Write("<a href=\"" + FeedUrls.GetFeedListUrl(FeedType.Poll) + "\">" + Resources.NewsResource.SeeAllPolls + "</a>");
                writer.Write("</div>");
            }
            else
            {
                writer.Write("<div class=\"empty-widget\" style=\"padding:40px; text-align: center;\">" +
                    String.Format(Resources.NewsResource.NoPollWidgetMessage,
                    string.Format("<div style=\"padding-top:3px;\"><a class=\"promoAction\" href=\"{0}\">",VirtualPathUtility.ToAbsolute("~/products/community/modules/news/editpoll.aspx")),
                    "</a></div>")
                    + "</div>");
            }

			writer.Write("</div>");
			writer.Write("</div>");
		}

        #region ICheckEmptyContent Members

        public CheckEmptyContentResult IsEmpty()
        {
            try
            {
                var storage = FeedStorageFactory.Create();
                var polls = storage.GetFeeds(FeedType.Poll, Guid.Empty, 1, 0);
                if (0 < polls.Count) poll = storage.GetFeed(polls[0].Id) as FeedPoll;
            }
            catch { }
            if (poll == null)
                return CheckEmptyContentResult.NotEmpty;

            return CheckEmptyContentResult.Empty;
        }

        #endregion
    }
}