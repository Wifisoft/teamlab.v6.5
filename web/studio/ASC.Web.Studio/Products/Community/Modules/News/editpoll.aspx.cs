using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web;
using ASC.Core.Users;
using ASC.Web.Community.News.Code;
using ASC.Web.Community.News.Code.DAO;
using ASC.Web.Community.News.Resources;
using ASC.Web.Community.Product;
using ASC.Web.Controls;
using ASC.Web.Studio;
using ASC.Web.Studio.Core;
using ASC.Web.Studio.Utility;

namespace ASC.Web.Community.News
{
	public partial class EditPoll : MainPage
	{
		private RequestInfo info;

		private RequestInfo Info
		{
			get
			{
				if (info == null) info = new RequestInfo(Request);
				return info;
			}
		}

		private List<BreadCrumb> Breadcrumb
		{
			get { return (Master as NewsMaster).BreadcrumbsControl; }
		}

		public long FeedId
		{
			get { return ViewState["FeedID"] != null ? Convert.ToInt32(ViewState["FeedID"]) : 0; }
			set { ViewState["FeedID"] = value; }
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			if (SetupInfo.WorkMode == WorkMode.Promo)
                Response.Redirect(FeedUrls.MainPageUrl, true);

			if (!CommunitySecurity.CheckPermissions(NewsConst.Action_Add))
                Response.Redirect(FeedUrls.MainPageUrl, true);

			Breadcrumb.Add(new BreadCrumb() { Caption = NewsResource.NewsBreadCrumbs, NavigationUrl = VirtualPathUtility.ToAbsolute("~/products/community/modules/news/") });
			if (Info.HasUser) Breadcrumb.Add(new BreadCrumb() { Caption = Info.User.DisplayUserName(false), NavigationUrl = string.Format(CultureInfo.CurrentCulture, "{0}?uid={1}", VirtualPathUtility.ToAbsolute("~/products/community/modules/news/"), Info.UserId) });

			var storage = FeedStorageFactory.Create();
			Feed feed = null;
			long docID = 0;
			if (!string.IsNullOrEmpty(Request["docID"]) && long.TryParse(Request["docID"], out docID))
			{
				feed = storage.GetFeed(docID);
			}
			if (!IsPostBack)
			{
				_errorMessage.Text = "";
				if (feed != null)
				{
					if (!CommunitySecurity.CheckPermissions(feed, NewsConst.Action_Edit))
					{
                        Response.Redirect(FeedUrls.MainPageUrl, true);
					}

					FeedId = docID;
					FeedPoll pollFeed = feed as FeedPoll;
					if (pollFeed != null)
					{
						_pollMaster.QuestionFieldID = "feedName";
						var question = pollFeed;
						_pollMaster.Singleton = (question.PollType == FeedPollType.SimpleAnswer);
						_pollMaster.Name = feed.Caption;
						_pollMaster.ID = question.Id.ToString(CultureInfo.CurrentCulture);

						foreach (var variant in question.Variants)
						{
							_pollMaster.AnswerVariants.Add(new ASC.Web.Controls.PollFormMaster.AnswerViarint()
							{
								ID = variant.ID.ToString(CultureInfo.CurrentCulture),
								Name = variant.Name
							});
						}
					}
				}
				else
				{
					_pollMaster.QuestionFieldID = "feedName";
				}
			}
            else
			{
                SaveFeed();
			}

			if (feed != null)
			{
				Breadcrumb.Add(new BreadCrumb() { Caption = feed.Caption, NavigationUrl = VirtualPathUtility.ToAbsolute("~/products/community/modules/news/") + "?docid=" + docID + Info.UserIdAttribute });
				Breadcrumb.Add(new BreadCrumb() { Caption = NewsResource.NewsEditBreadCrumbsPoll, NavigationUrl = VirtualPathUtility.ToAbsolute("~/products/community/modules/news/editpoll.aspx") + "?docid=" + docID + Info.UserIdAttribute });
				lbCancel.NavigateUrl = VirtualPathUtility.ToAbsolute("~/products/community/modules/news/") + "?docid=" + docID + Info.UserIdAttribute;
			}
			else
			{
				Breadcrumb.Add(new BreadCrumb() { Caption = NewsResource.NewsAddBreadCrumbsPoll, NavigationUrl = VirtualPathUtility.ToAbsolute("~/products/community/modules/news/editpoll.aspx") + (string.IsNullOrEmpty(Info.UserIdAttribute) ? string.Empty : "?" + Info.UserIdAttribute.Substring(1)) });
				lbCancel.NavigateUrl = VirtualPathUtility.ToAbsolute("~/products/community/modules/news/") + (string.IsNullOrEmpty(Info.UserIdAttribute) ? string.Empty : "?" + Info.UserIdAttribute.Substring(1));
			}

			this.Title = HeaderStringHelper.GetPageTitle(Resources.NewsResource.AddonName, Breadcrumb);

		}

		protected void SaveFeed()
		{
			if (String.IsNullOrEmpty(_pollMaster.Name))
			{
				_errorMessage.Text = "<div class='errorBox'>" + Resources.NewsResource.ErrorEmptyQuestion + "</div>";
				return;
			}

			if (_pollMaster.AnswerVariants.Count < 2)
			{
				_errorMessage.Text = "<div class='errorBox'>" + Resources.NewsResource.ErrorPollVariantCount + "</div>";
				return;
			}

			var isEdit = FeedId != 0;
			var storage = FeedStorageFactory.Create();

			var feed = isEdit ? (FeedPoll)storage.GetFeed(FeedId) : new FeedPoll();
			feed.Caption = _pollMaster.Name;
			feed.PollType = _pollMaster.Singleton ? FeedPollType.SimpleAnswer : FeedPollType.MultipleAnswer;

			int i = 0;
			foreach (var answVariant in _pollMaster.AnswerVariants)
			{
				FeedPollVariant answerVariant = null;
				try
				{
					answerVariant = feed.Variants[i];
				}
				catch { }
				if (answerVariant == null)
				{
					answerVariant = new FeedPollVariant();
					feed.Variants.Add(answerVariant);
				}
				answerVariant.Name = answVariant.Name;
				i++;
			}
			while (i != feed.Variants.Count)
			{
				feed.Variants.RemoveAt(i);
			}

			storage.SaveFeed(feed, isEdit, FeedType.Poll);



			Response.Redirect(VirtualPathUtility.ToAbsolute("~/products/community/modules/news/") + "?docid=" + feed.Id + Info.UserIdAttribute);
		}
	}
}