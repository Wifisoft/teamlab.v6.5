using System;
using System.Globalization;
using AjaxPro;
using ASC.Core;
using ASC.Core.Tenants;
using ASC.Core.Users;
using ASC.Web.Community.News.Code;
using ASC.Web.Community.News.Code.DAO;
using ASC.Web.Community.News.Code.Module;
using ASC.Web.Community.News.Resources;
using ASC.Web.Community.Product;
using ASC.Web.Controls;
using ASC.Web.Studio.Core;
using ASC.Web.Studio.Utility;

namespace ASC.Web.Community.News.Controls
{
    [AjaxNamespace("FeedView")]
    public partial class FeedView : System.Web.UI.UserControl
    {
        protected Guid RequestedUserId
        {
            get
            {
                Guid result = Guid.Empty;
                try
                {
                    result = new Guid(Request["uid"]);
                }
                catch { }

                return result;
            }
        }

        protected string UserIdAttribute
        {
            get
            {
                if (!RequestedUserId.Equals(Guid.Empty))
                {
                    return string.Format(CultureInfo.CurrentCulture, "&uid={0}", RequestedUserId);
                }
                return string.Empty;
            }

        }

        public Feed Feed { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            Utility.RegisterTypeForAjax(typeof(FeedView), this.Page);
        }

        public override void DataBind()
        {
            base.DataBind();

            if (Feed != null)
            {
                newsText.Text = HtmlUtility.GetFull(Feed.Text, ASC.Web.Community.Product.CommunityProduct.ID);
                Date.Text = Feed.Date.Ago();

                EditorButtons.Visible = CommunitySecurity.CheckPermissions(Feed, NewsConst.Action_Edit);
                if (EditorButtons.Visible)
                {
                    EditorButtons.Text = string.Format(CultureInfo.CurrentCulture, "{0}<span class='splitter'>|</span>{1}",
                        string.Format(CultureInfo.CurrentCulture, "<a class=\"feedPrevEditButton" + (SetupInfo.WorkMode == WorkMode.Promo ? " promoAction" : "") + "\" href=\"{0}?docID={1}{3}\">{2}</a>", GetEditUrl(), Feed.Id, NewsResource.EditButton, UserIdAttribute),
                        string.Format(CultureInfo.CurrentCulture, "<a class=\"feedPrevEditButton" + (SetupInfo.WorkMode == WorkMode.Promo ? " promoAction" : "") + "\" href=\"javascript:;\" onclick=\"javascript:if(window.confirm('{0}'))FeedView.Remove('{1}',callbackRemove);\">{2}</a>", NewsResource.ConfirmRemoveMessage, Feed.Id, NewsResource.DeleteButton));
                }

                profileLink.Text = CoreContext.UserManager.GetUsers(new Guid(Feed.Creator)).RenderProfileLink(Community.Product.CommunityProduct.ID);

                if (Feed is FeedPoll)
                {
                    var poll = (FeedPoll)Feed;
                    bool isMakeVote = TenantUtil.DateTimeNow() <= poll.EndDate && !poll.IsUserVote(SecurityContext.CurrentAccount.ID.ToString());

                    var pollForm = new PollForm();
                    pollForm.VoteHandlerType = typeof(PollVoteHandler);
                    pollForm.Answered = !isMakeVote || SecurityContext.DemoMode || (SetupInfo.WorkMode == WorkMode.Promo);
                    pollForm.Name = poll.Caption;
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
                    pollHolder.Controls.Add(pollForm);
                }
                else
                {
                    pollHolder.Visible = false;
                    newsText.Visible = true;
                }
            }
        }

        private string GetEditUrl()
        {
            return Feed is FeedPoll ? FeedUrls.EditPollUrl : FeedUrls.EditNewsUrl;
        }

        [AjaxPro.AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public AjaxResponse Remove(string id)
        {
            try
            {
                CommunitySecurity.DemandPermissions(NewsConst.Action_Edit);
                AjaxResponse resp = new AjaxResponse();
                resp.rs1 = "0";
                if (!string.IsNullOrEmpty(id))
                {
                    var feedId = Convert.ToInt64(id);
                    var storage = FeedStorageFactory.Create();

                    foreach (var comment in storage.GetFeedComments(feedId))
                    {
                        CommonControlsConfigurer.FCKUploadsRemoveForItem("news_comments", comment.Id.ToString());
                    }

                    ActivityPublisher.DeletePost(storage.GetFeed(Convert.ToInt64(id)), SecurityContext.CurrentAccount.ID);

                    storage.RemoveFeed(feedId);
                    CommonControlsConfigurer.FCKUploadsRemoveForItem("news", id);

                    resp.rs1 = id;
                    resp.rs2 = NewsResource.FeedDeleted;
                }
                return resp;
            }
            catch (Exception err)
            {
                return new AjaxResponse { rs1 = "1", rs2 = err.Message, };
            }
        }
    }
}