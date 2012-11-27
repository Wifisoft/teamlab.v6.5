using System;
using System.Text;
using AjaxPro;
using ASC.Core;
using ASC.Forum;
using ASC.Web.Studio.Core;
using ASC.Web.Studio.Utility;

namespace ASC.Web.Community.Forum
{
	[AjaxNamespace("Subscriber")]
	public class Subscriber : ISubscriberView
	{
		public Subscriber()
		{
			ForumManager.Instance.PresenterFactory.GetPresenter<ISubscriberView>().SetView(this);
		}

		#region Topic
		[AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
		public AjaxResponse SubscribeOnTopic(int idTopic, int statusNotify)
		{
			AjaxResponse resp = new AjaxResponse();

			if (statusNotify == 1 && Subscribe != null)
			{
				Subscribe(this, new SubscribeEventArgs(SubscriptionConstants.NewPostInTopic,
																  idTopic.ToString(),
																  SecurityContext.CurrentAccount.ID));

				resp.rs1 = "1";
				resp.rs2 = RenderTopicSubscription(false, idTopic);
			}
			else if (UnSubscribe != null)
			{
				UnSubscribe(this, new SubscribeEventArgs(SubscriptionConstants.NewPostInTopic,
																  idTopic.ToString(),
																  SecurityContext.CurrentAccount.ID));

				resp.rs1 = "1";
				resp.rs2 = RenderTopicSubscription(true, idTopic);
			}
			else
			{
				resp.rs1 = "0";
				resp.rs2 = Resources.ForumResource.ErrorSubscription;
			}
			return resp;
		}

		public string RenderTopicSubscription(bool isSubscribe, int topicID)
		{
			StringBuilder sb = new StringBuilder();

			sb.Append("<div id=\"forum_subcribeOnTopicBox\">");
			sb.Append("<a class='linkAction" + (SetupInfo.WorkMode == WorkMode.Promo ? " promoAction" : "") + "' title='" + Resources.ForumResource.SubscribeTopicHelp + "' href=\"#\" onclick=\"ForumSubscriber.SubscribeOnTopic('" + topicID + "', " + (isSubscribe ? 1 : 0) + "); return false;\">" + (!isSubscribe ? Resources.ForumResource.UnSubscribeOnTopic : Resources.ForumResource.SubscribeOnTopic) + "</a>");
			sb.Append("</div>");

			return sb.ToString();
		}

		public bool IsTopicSubscribe(int topicID)
		{
			if (GetSubscriptionState != null)
				GetSubscriptionState(this, new SubscribeEventArgs(SubscriptionConstants.NewPostInTopic,
																  topicID.ToString(),
																  SecurityContext.CurrentAccount.ID));

			return IsSubscribe;
		}


		#endregion

		#region Thread
		public string RenderThreadSubscription(bool isSubscribe, int threadID)
		{
			StringBuilder sb = new StringBuilder();

			sb.Append("<div id=\"forum_subcribeOnThreadBox\">");
			sb.Append("<a class='linkAction" + (SetupInfo.WorkMode == WorkMode.Promo ? " promoAction" : "") + "' href=\"#\" title='" + Resources.ForumResource.SubscribeThreadHelp + "' onclick=\"ForumSubscriber.SubscribeOnThread('" + threadID + "', " + (isSubscribe ? 1 : 0) + "); return false;\">" + (!isSubscribe ? Resources.ForumResource.UnSubscribeOnThread : Resources.ForumResource.SubscribeOnThread) + "</a>");
			sb.Append("</div>");

			return sb.ToString();
		}

		public bool IsThreadSubscribe(int threadID)
		{
			if (GetSubscriptionState != null)
				GetSubscriptionState(this, new SubscribeEventArgs(SubscriptionConstants.NewPostInThread,
																  threadID.ToString(),
																  SecurityContext.CurrentAccount.ID));

			return IsSubscribe;
		}

		[AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
		public AjaxResponse SubscribeOnThread(int idThread, int statusNotify)
		{
			AjaxResponse resp = new AjaxResponse();
			if (statusNotify == 1 && Subscribe != null)
			{
				Subscribe(this, new SubscribeEventArgs(SubscriptionConstants.NewPostInThread,
																  idThread.ToString(),
																  SecurityContext.CurrentAccount.ID));

				resp.rs1 = "1";
				resp.rs2 = RenderThreadSubscription(false, idThread);
			}
			else if (UnSubscribe != null)
			{
				UnSubscribe(this, new SubscribeEventArgs(SubscriptionConstants.NewPostInThread,
																  idThread.ToString(),
																  SecurityContext.CurrentAccount.ID));

				resp.rs1 = "1";
				resp.rs2 = RenderThreadSubscription(true, idThread);
			}
			else
			{
				resp.rs1 = "0";
				resp.rs2 = Resources.ForumResource.ErrorSubscription;
			}
			return resp;
		}
		#endregion

		#region Topic on forum
		public string RenderNewTopicSubscription(bool isSubscribe)
		{
			StringBuilder sb = new StringBuilder();

			sb.Append("<div id=\"forum_subcribeOnNewTopicBox\">");
			sb.Append("<a class='linkAction" + (SetupInfo.WorkMode == WorkMode.Promo ? " promoAction" : "") + "' href=\"#\" onclick=\"ForumSubscriber.SubscribeOnNewTopics(" + (isSubscribe ? 1 : 0) + "); return false;\">" + (!isSubscribe ? Resources.ForumResource.UnsubscribeOnNewTopicInForum : Resources.ForumResource.SubscribeOnNewTopicInForum) + "</a>");
			sb.Append("</div>");

			return sb.ToString();
		}

		public bool IsNewTopicSubscribe()
		{
			if (GetSubscriptionState != null)
				GetSubscriptionState(this, new SubscribeEventArgs(SubscriptionConstants.NewTopicInForum,
																  null,
																  SecurityContext.CurrentAccount.ID));

			return IsSubscribe;
		}

		[AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
		public AjaxResponse SubscribeOnNewTopic(int statusNotify)
		{
			AjaxResponse resp = new AjaxResponse();
			if (statusNotify == 1 && Subscribe != null)
			{
				Subscribe(this, new SubscribeEventArgs(SubscriptionConstants.NewTopicInForum,
																  null,
																  SecurityContext.CurrentAccount.ID));

				resp.rs1 = "1";
				resp.rs2 = RenderNewTopicSubscription(false);
			}
			else if (UnSubscribe != null)
			{
				UnSubscribe(this, new SubscribeEventArgs(SubscriptionConstants.NewTopicInForum,
																  null,
																  SecurityContext.CurrentAccount.ID));

				resp.rs1 = "1";
				resp.rs2 = RenderNewTopicSubscription(true);
			}
			else
			{
				resp.rs1 = "0";
				resp.rs2 = Resources.ForumResource.ErrorSubscription;
			}
			return resp;
		}
		#endregion

		#region Tag

		public string RenderTagSubscription(bool isSubscribe, int tagID)
		{
			StringBuilder sb = new StringBuilder();

			sb.Append("<div id=\"forum_subcribeOnTagBox\">");
			sb.Append("<a class='linkAction" + (SetupInfo.WorkMode == WorkMode.Promo ? " promoAction" : "") + "' href=\"#\" onclick=\"ForumSubscriber.SubscribeOnTag('" + tagID + "', " + (isSubscribe ? 1 : 0) + "); return false;\">" + (!isSubscribe ? Resources.ForumResource.UnsubscribeButton : Resources.ForumResource.SubscribeOnTag) + "</a>");
			sb.Append("</div>");

			return sb.ToString();
		}

		[AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
		public AjaxResponse SubscribeOnTag(int idTag, int statusNotify)
		{
			AjaxResponse resp = new AjaxResponse();
			if (statusNotify == 1 && Subscribe != null)
			{
				Subscribe(this, new SubscribeEventArgs(SubscriptionConstants.NewPostByTag,
																  idTag.ToString(),
																  SecurityContext.CurrentAccount.ID));

				resp.rs1 = "1";
				resp.rs2 = RenderTagSubscription(false, idTag);
			}
			else if (UnSubscribe != null)
			{
				UnSubscribe(this, new SubscribeEventArgs(SubscriptionConstants.NewPostByTag,
																  idTag.ToString(),
																  SecurityContext.CurrentAccount.ID));

				resp.rs1 = "1";
				resp.rs2 = RenderTagSubscription(true, idTag);
			}
			else
			{
				resp.rs1 = "0";
				resp.rs2 = Resources.ForumResource.ErrorSubscription;
			}
			return resp;
		}

		public bool IsTagSubscribe(int tagID)
		{
			if (GetSubscriptionState != null)
				GetSubscriptionState(this, new SubscribeEventArgs(SubscriptionConstants.NewPostByTag,
																  tagID.ToString(),
																  SecurityContext.CurrentAccount.ID));

			return IsSubscribe;
		}

		#endregion

		#region ISubscriberView Members

		public event EventHandler<SubscribeEventArgs> GetSubscriptionState;

		public bool IsSubscribe { get; set; }

		public event EventHandler<SubscribeEventArgs> Subscribe;

		public event EventHandler<SubscribeEventArgs> UnSubscribe;

		public event EventHandler<SubscribeEventArgs> UnSubscribeForSubscriptionType;

		#endregion

		private void OnUnSubscribeForSubscriptionType(SubscribeEventArgs e)
		{
			if (UnSubscribeForSubscriptionType != null) UnSubscribeForSubscriptionType(this, e);
		}
	}
}
