using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using AjaxPro;
using ASC.Core;
using ASC.Notify.Model;
using ASC.Notify.Recipients;
using ASC.Web.Community.News.Code;
using ASC.Web.Community.News.Code.DAO;
using ASC.Web.Community.News.Code.Module;
using ASC.Web.Community.Product;
using ASC.Web.Controls;
using ASC.Web.Studio.Controls.Common;
using ASC.Web.Studio.Core;
using ASC.Web.Studio.Utility;

namespace ASC.Web.Community.News
{
    [AjaxNamespace("MainAjaxMaster")]
    public partial class NewsMaster : System.Web.UI.MasterPage
    {
        public string SearchText { get; set; }

        public List<BreadCrumb> BreadcrumbsControl
        {
            get { return MainNewsContainer.BreadCrumbs; }
        }

        private static IDirectRecipient IAmAsRecipient
        {
            get { return (IDirectRecipient) NewsNotifySource.Instance.GetRecipientsProvider().GetRecipient(SecurityContext.CurrentAccount.ID.ToString()); }
        }

        protected Guid RequestedUserId
        {
            get
            {
                var result = Guid.Empty;
                try
                {
                    result = new Guid(Request["uid"]);
                }
                catch
                {
                }

                return result;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Utility.RegisterTypeForAjax(this.GetType(), this.Page);

            SearchText = "";
            if (!string.IsNullOrEmpty(Request["search"]))
            {
                SearchText = Request["search"];
            }
            PanelManage.Visible = CommunitySecurity.CheckPermissions(NewsConst.Action_Add);

            WriteNavigation();

            editNews.URL = string.Format(CultureInfo.CurrentCulture, "{0}{1}", ResolveUrl("editnews.aspx"), RequestedUserId.Equals(Guid.Empty) ? string.Empty : string.Format(CultureInfo.CurrentCulture, "?uid={0}", RequestedUserId)).ToLower();
            editNews.Name = Resources.NewsResource.NewsAdd;
            editNews.IsPromo = (SetupInfo.WorkMode == WorkMode.Promo);

            editPoll.URL = string.Format(CultureInfo.CurrentCulture, "{0}{1}", ResolveUrl("editpoll.aspx"), RequestedUserId.Equals(Guid.Empty) ? string.Empty : string.Format(CultureInfo.CurrentCulture, "?uid={0}", RequestedUserId)).ToLower();
            editPoll.Name = Resources.NewsResource.PollAdd;
            editPoll.IsPromo = (SetupInfo.WorkMode == WorkMode.Promo);


            sideRecentActivity.TenantId = TenantProvider.CurrentTenantID;
            sideRecentActivity.ProductId = Product.CommunityProduct.ID;
            sideRecentActivity.ModuleId = News.NewsConst.ModuleId;

            NotifyOnNews();
        }

        public void SetInfoMessage(string message, InfoType type)
        {
            this.MainNewsContainer.Options.InfoType = type;
            this.MainNewsContainer.Options.InfoMessageText = message;
        }

        public void WriteNavigation()
        {
            var usedFeedTypes = FeedStorageFactory.Create().GetUsedFeedTypes();
            if (usedFeedTypes.Count == 0)
            {
                NewsNavigator.Visible = false;
            }
            else
            {
                niTypeAllNews.URL = FeedUrls.GetFeedListUrl(RequestedUserId);
                niTypeAllNews.Name = Resources.NewsResource.NewsBreadCrumbs;
                AddNewsTypes(usedFeedTypes);
            }
        }

        private void AddNewsTypes(IList<FeedType> usedFeedTypes)
        {
            foreach (var feedType in usedFeedTypes)
            {
                var feedTypeInfo = FeedTypeInfo.FromFeedType(feedType);
                NewsNavigator.Controls.Add(
                    new NavigationItem()
                        {
                            Name = feedTypeInfo.TypeName,
                            ID = feedTypeInfo.TypeName,
                            URL = FeedUrls.GetFeedListUrl(feedType, RequestedUserId)
                        }
                    );
            }
        }

        private void NotifyOnNews()
        {
            var sb = new StringBuilder();
            var subscriptionProvider = NewsNotifySource.Instance.GetSubscriptionProvider();

            var userList = new List<string>();
            if (IAmAsRecipient != null)
            {
                userList = new List<string>(
                    subscriptionProvider.GetSubscriptions(
                        NewsConst.NewFeed,
                        IAmAsRecipient)
                    );
            }

            var subscribed = userList.Contains(null);

            sb.Append("<div id=\"news_notifies\">");

            sb.Append("<a id=\"notify_news\" class=\"linkAction" + (SetupInfo.WorkMode == WorkMode.Promo ? " promoAction" : "") + "\" href=\"javascript:void(0);\" />" + (!subscribed ? Resources.NewsResource.NotifyOnUploadsMessage : Resources.NewsResource.UnNotifyOnUploadsMessage) + "</a>");

            var feedID = Request.QueryString["docid"] ?? string.Empty;
            var isSubsribedOnComments = subscriptionProvider.IsSubscribed(NewsConst.NewComment, IAmAsRecipient, feedID);
            var displaySubscribeOnComments = !string.IsNullOrEmpty(feedID);
            if (displaySubscribeOnComments)
            {
                sb.AppendFormat("<div style='margin-top: 10px;'><a id='notiryOnNewComments' class='linkAction{0}' href='javascript:void(0);' />{1}</a></div>",
                                SetupInfo.WorkMode == WorkMode.Promo ? " promoAction" : string.Empty,
                                !isSubsribedOnComments ? Resources.NewsResource.SubscribeOnNewComments.ReplaceSingleQuote() : Resources.NewsResource.UnsubscribeFromNewComments.ReplaceSingleQuote());
            }

            const string AjaxFunc = "AjaxPro.onLoading = function(b){ if(b){	jq('#news_notifies').block(); } else { jq('#news_notifies').unblock();} };";

            const string subscribeFunc = @"
    var NotifyNewsUploads={3};
	jq('#{0}').click(
		function() {{
			{4}
			MainAjaxMaster.SubscribeOnNews(NotifyNewsUploads,
				function(result) {{
					NotifyNewsUploads = result.value;
					if(!NotifyNewsUploads){{
						jq('#{0}').html('{1}');
					}} else {{
						jq('#{0}').html('{2}');
					}}
				}}
			);
		}}
	);";

            sb.Append("<script type='text/javascript'>");
            sb.AppendFormat(subscribeFunc,
                            "notify_news",
                            Resources.NewsResource.NotifyOnUploadsMessage.ReplaceSingleQuote(),
                            Resources.NewsResource.UnNotifyOnUploadsMessage.ReplaceSingleQuote(),
                            subscribed.ToString().ToLower(),
                            AjaxFunc);


            if (displaySubscribeOnComments)
            {
                sb.AppendFormat(@"
            var NotifyNewsComments={0};
            jq('#notiryOnNewComments').click(function(){{
	            {4}
	            MainAjaxMaster.SubscribeOnComments(NotifyNewsComments, '{1}',
		            function(result){{
			            NotifyNewsComments = result.value;
			            if(!NotifyNewsComments){{
				            jq('#notiryOnNewComments').html('{2}');
			            }} else {{
				            jq('#notiryOnNewComments').html('{3}');
			            }}			
		            }});	
            }});
            ",
                                isSubsribedOnComments.ToString().ToLower(),
                                feedID,
                                Resources.NewsResource.SubscribeOnNewComments.ReplaceSingleQuote(),
                                Resources.NewsResource.UnsubscribeFromNewComments.ReplaceSingleQuote(),
                                AjaxFunc);
            }

            sb.Append(@"
</script>
</div>");

            NewsActions.Controls.Add(new HtmlMenuItem(sb.ToString()));

        }

        [AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public bool SubscribeOnNews(bool isSubscribe)
        {

            ISubscriptionProvider subscriptionProvider = NewsNotifySource.Instance.GetSubscriptionProvider();
            if (IAmAsRecipient == null)
            {
                return false;
            }
            if (!isSubscribe)
            {
                subscriptionProvider.Subscribe(NewsConst.NewFeed, null, IAmAsRecipient);
                return true;
            }
            else
            {
                subscriptionProvider.UnSubscribe(NewsConst.NewFeed, null, IAmAsRecipient);
                return false;
            }
        }

        [AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public bool SubscribeOnComments(bool isSubscribe, string commentID)
        {

            var subscriptionProvider = NewsNotifySource.Instance.GetSubscriptionProvider();
            if (IAmAsRecipient == null)
            {
                return false;
            }
            if (!isSubscribe)
            {
                subscriptionProvider.Subscribe(NewsConst.NewComment, commentID, IAmAsRecipient);
                return true;
            }
            else
            {
                subscriptionProvider.UnSubscribe(NewsConst.NewComment, commentID, IAmAsRecipient);
                return false;
            }
        }
    }
}