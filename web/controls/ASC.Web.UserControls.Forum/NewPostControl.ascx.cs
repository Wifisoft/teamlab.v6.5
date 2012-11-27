using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using AjaxPro;
using ASC.Core;
using ASC.Core.Users;
using ASC.Forum;
using ASC.Notify;
using ASC.Notify.Recipients;
using ASC.Web.Controls;
using ASC.Web.Controls.FileUploader.HttpModule;
using ASC.Web.Core.Utility.Skins;
using ASC.Web.Studio.Utility;
using ASC.Web.UserControls.Forum.Common;
using ASC.Web.Studio.Controls.Common;
using System.Linq;
using System.Text.RegularExpressions;
using ASC.Notify.Model;
using System.IO;

namespace ASC.Web.UserControls.Forum
{

    public class ForumAttachmentUploadHanler : FileUploadHandler
    {
        public override FileUploadHandler.FileUploadResult ProcessUpload(HttpContext context)
        {
            var result = new FileUploadResult() { Success = false };
            try
            {
                if (ProgressFileUploader.HasFilesToUpload(context))
                {
                    var settingsID = new Guid(context.Request["SettingsID"]);
                    var settings = ForumManager.GetSettings(settingsID);
                    var thread = ForumDataProvider.GetThreadByID(TenantProvider.CurrentTenantID, Convert.ToInt32(context.Request["ThreadID"]));
                    if (thread == null) return result;

                    var forumManager = settings.ForumManager;
                    var offsetPhysicalPath = string.Empty;
                    forumManager.GetAttachmentVirtualDirPath(thread, settingsID, new Guid(context.Request["UserID"]), out offsetPhysicalPath);

                    var file = new ProgressFileUploader.FileToUpload(context);

                    var newFileName = GetFileName(file.FileName);
                    var origFileName = newFileName;

                    var i = 1;
                    var store = forumManager.GetStore();
                    while (store.IsFile(offsetPhysicalPath + "\\" + newFileName))
                    {
                        var ind = origFileName.LastIndexOf(".");
                        newFileName = ind != -1 ? origFileName.Insert(ind, "_" + i.ToString()) : origFileName + "_" + i.ToString();
                        i++;
                    }

                    result.FileName = newFileName;
                    result.FileURL = store.Save(offsetPhysicalPath + "\\" + newFileName, file.InputStream).ToString();
                    result.Data = new
                    {
                        OffsetPhysicalPath = offsetPhysicalPath + "\\" + newFileName,
                        FileName = newFileName,
                        Size = file.ContentLength,
                        ContentType = file.FileContentType,
                        SettingsID = settingsID
                    };
                    result.Success = true;
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }

            return result;
        }
    }

    public enum NewPostType
    {
        Post = 0,
        Topic = 1,
        Poll = 2
    }
    public enum PostAction
    {
        Normal = 0,
        Quote = 1,
        Reply = 2,
        Edit = 3
    }

    public enum SubscriveViewType
    {
        Checked,
        Unchecked,
        Disable
    }



    class TagComparer : IEqualityComparer<Tag>
    {

        #region IEqualityComparer<Tag> Members

        public bool Equals(Tag x, Tag y)
        {
            if (Object.ReferenceEquals(x, y))
                return true;

            if (Object.ReferenceEquals(x, null) || Object.ReferenceEquals(y, null))
                return false;

            return x.Name.Equals(y.Name, StringComparison.InvariantCultureIgnoreCase);
        }

        public int GetHashCode(Tag obj)
        {
            if (Object.ReferenceEquals(obj, null)) return 0;
            return string.IsNullOrEmpty(obj.Name) ? 0 : obj.Name.GetHashCode();
        }

        #endregion
    }

    [AjaxNamespace("PostCreator")]
    public partial class NewPostControl : UserControl,
                                           INotifierView,
                                           ISubscriberView,
                                           IContextInitializer
    {
        public Guid SettingsID { get; set; }
        public string NewTopicPageTitle { get; set; }
        public string NewPollPageTitle { get; set; }
        public string NewPostPageTitle { get; set; }
        public NewPostType PostType { get; set; }
        public PostAction PostAction { get; set; }
        public Topic Topic { get; set; }
        public Thread Thread { get; set; }
        public Post ParentPost { get; set; }
        public Post EditedPost { get; set; }

        protected string _text = "";
        private string _subject = "";
        private string _tagString = "";
        private string _tagValues = "";
        private ForumManager _forumManager;
        private bool _isSelectForum;
        private List<ThreadCategory> _categories = null;
        private List<Thread> _threads = null;

        protected string _errorMessage = "";
        protected SubscriveViewType _subscribeViewType;
        protected PostTextFormatter _formatter = PostTextFormatter.FCKEditor;
        protected UserInfo _currentUser = null;
        protected Settings _settings;
        protected int _threadForAttachFiles = 0;
        protected string _attachmentsString = "";

        protected bool _mobileVer = false;

        protected void Page_Load(object sender, EventArgs e)
        {
            _mobileVer = ASC.Web.Core.Mobile.MobileDetector.IsRequestMatchesMobile(this.Context);
            _settings = ForumManager.GetSettings(SettingsID);
            _forumManager = _settings.ForumManager;
            _currentUser = CoreContext.UserManager.GetUsers(SecurityContext.CurrentAccount.ID);

            Utility.RegisterTypeForAjax(typeof(TagSuggest));
            Utility.RegisterTypeForAjax(this.GetType());
            Utility.RegisterTypeForAjax(typeof(PostControl));

            _uploadSwitchHolder.Controls.Add(new FileUploaderModeSwitcher());

            FCKeditor.BasePath = VirtualPathUtility.ToAbsolute(CommonControlsConfigurer.FCKEditorBasePath);
            FCKeditor.ToolbarSet = "ForumToolbar";
            FCKeditor.EditorAreaCSS = WebSkin.GetUserSkin().BaseCSSFileAbsoluteWebPath;
            FCKeditor.Visible = !_mobileVer;

            PostType = NewPostType.Topic;
            PostAction = PostAction.Normal;

            int idTopic = 0;
            int idThread = 0;


            if (!String.IsNullOrEmpty(Request[_settings.TopicParamName]))
            {
                try
                {
                    idTopic = Convert.ToInt32(Request[_settings.TopicParamName]);
                }
                catch { idTopic = 0; }

                if (idTopic == 0)
                {
                    Response.Redirect(_settings.StartPageAbsolutePath);
                    return;
                }
                else
                    PostType = NewPostType.Post;

            }
            else if (!String.IsNullOrEmpty(Request[_settings.ThreadParamName]))
            {
                try
                {
                    idThread = Convert.ToInt32(Request[_settings.ThreadParamName]);
                }
                catch { idThread = 0; }

                if (idThread == 0)
                {
                    Response.Redirect(_settings.StartPageAbsolutePath);
                    return;
                }
                else
                    PostType = NewPostType.Topic;

                int topicType = 0;
                try
                {
                    topicType = Convert.ToInt32(Request[_settings.PostParamName]);
                }
                catch
                {
                    topicType = 0;
                }
                if (topicType == 1)
                    PostType = NewPostType.Poll;
            }
            else
            {
                int topicType = 0;
                try
                {
                    topicType = Convert.ToInt32(Request[_settings.PostParamName]);
                }
                catch
                {
                    topicType = 0;
                }
                if (topicType == 1)
                    PostType = NewPostType.Poll;


                if (IsPostBack)
                {
                    if (!String.IsNullOrEmpty(Request["forum_thread_id"]))
                    {
                        try
                        {
                            idThread = Convert.ToInt32(Request["forum_thread_id"]);
                        }
                        catch { idThread = 0; }
                    }
                }
                else
                {
                    ForumDataProvider.GetThreadCategories(TenantProvider.CurrentTenantID, out _categories, out _threads);


                    foreach (var thread in _threads)
                    {
                        bool isAllow = false;
                        if (PostType == NewPostType.Topic)
                            isAllow = _forumManager.ValidateAccessSecurityAction(ForumAction.TopicCreate, thread);
                        else if (PostType == NewPostType.Poll)
                            isAllow = _forumManager.ValidateAccessSecurityAction(ForumAction.PollCreate, thread);

                        if (isAllow)
                        {
                            idThread = thread.ID;
                            Thread = thread;
                            break;
                        }
                    }
                }

                if (idThread == 0)
                {
                    Response.Redirect(_settings.StartPageAbsolutePath);
                    return;
                }

                _isSelectForum = true;
            }


            if (PostType == NewPostType.Topic || PostType == NewPostType.Poll)
            {
                if (Thread == null)
                    Thread = ForumDataProvider.GetThreadByID(TenantProvider.CurrentTenantID, idThread);

                if (Thread == null)
                {
                    Response.Redirect(_settings.StartPageAbsolutePath);
                    return;
                }

                if (Thread.Visible == false)
                    Response.Redirect(_settings.StartPageAbsolutePath);

                _threadForAttachFiles = Thread.ID;

                if (PostType == NewPostType.Topic)
                {
                    if (!_forumManager.ValidateAccessSecurityAction(ForumAction.TopicCreate, Thread))
                    {
                        Response.Redirect(_settings.StartPageAbsolutePath);
                        return;
                    }

                    this.Page.Title = this.NewTopicPageTitle;
                }
                else if (PostType == NewPostType.Poll)
                {
                    if (!_forumManager.ValidateAccessSecurityAction(ForumAction.PollCreate, Thread))
                    {
                        Response.Redirect(_settings.StartPageAbsolutePath);
                        return;
                    }

                    this.Page.Title = this.NewPollPageTitle;
                }
            }
            else if (PostType == NewPostType.Post)
            {
                int parentPostId = 0;
                this.Page.Title = this.NewPostPageTitle;

                if (!String.IsNullOrEmpty(Request[_settings.ActionParamName]) && !String.IsNullOrEmpty(Request[_settings.PostParamName]))
                {
                    try
                    {
                        PostAction = (PostAction)Convert.ToInt32(Request[_settings.ActionParamName]);
                    }
                    catch { PostAction = PostAction.Normal; }
                    try
                    {
                        parentPostId = Convert.ToInt32(Request[_settings.PostParamName]);
                    }
                    catch
                    {
                        parentPostId = 0;
                        PostAction = PostAction.Normal;
                    }
                }


                Topic = ForumDataProvider.GetTopicByID(TenantProvider.CurrentTenantID, idTopic);
                if (Topic == null)
                {
                    Response.Redirect(_settings.StartPageAbsolutePath);
                    return;
                }

                if (new Thread() { ID = Topic.ThreadID }.Visible == false)
                {
                    Response.Redirect(_settings.StartPageAbsolutePath);
                    return;
                }



                var recentPosts = ForumDataProvider.GetRecentTopicPosts(TenantProvider.CurrentTenantID, Topic.ID, 5,
                                              (PostAction == PostAction.Normal || PostAction == PostAction.Edit) ? 0 : parentPostId);

                if (recentPosts.Count > 0)
                {
                    Label titleRecentPosts = new Label();
                    titleRecentPosts.Text = "<div class=\"headerPanel\" style='margin-top:20px;'>" + Resources.ForumUCResource.RecentPostFromTopic + "</div>";
                    _recentPostsHolder.Controls.Add(titleRecentPosts);


                    int i = 0;
                    foreach (Post post in recentPosts)
                    {
                        PostControl postControl = (PostControl)LoadControl(_settings.UserControlsVirtualPath + "/PostControl.ascx");
                        postControl.Post = post;
                        postControl.SettingsID = SettingsID;
                        postControl.IsEven = (i % 2 == 0);
                        _recentPostsHolder.Controls.Add(postControl);
                        i++;
                    }
                }


                _threadForAttachFiles = Topic.ThreadID;

                if (PostAction == PostAction.Quote || PostAction == PostAction.Reply || PostAction == PostAction.Normal)
                {
                    if (!_forumManager.ValidateAccessSecurityAction(ForumAction.PostCreate, Topic))
                    {
                        Response.Redirect(_settings.StartPageAbsolutePath);
                        return;
                    }

                    if (PostAction == PostAction.Quote || PostAction == PostAction.Reply)
                    {
                        ParentPost = ForumDataProvider.GetPostByID(TenantProvider.CurrentTenantID, parentPostId);

                        if (ParentPost == null)
                        {
                            Response.Redirect(_settings.StartPageAbsolutePath);
                            return;
                        }
                    }

                    _subject = Topic.Title;

                    if (PostAction == PostAction.Quote)
                    {
                        _text = String.Format(@"<div class=""mainQuote"">
                                                    <div class=""quoteCaption""><span class=""bold"">{0}</span>&nbsp;{1}</div>
                                                    <div id=""quote"" >
                                                    <div class=""bord""><div class=""t""><div class=""r"">
                                                    <div class=""b""><div class=""l""><div class=""c"">
                                                        <div class=""reducer"">
                                                            {2}
                                                        </div>
                                                    </div></div></div>
                                                    </div></div></div>
                                                </div>
                                                </div><br/>",
                                            ParentPost.Poster.DisplayUserName(),
                                            Resources.ForumUCResource.QuoteCaptioon_Wrote + ":",
                                            ParentPost.Text);

                    }

                    if (PostAction == PostAction.Reply)
                        _text = "<span class=\"headerPanel\">To: " + ParentPost.Poster.DisplayUserName() + "</span><br/><br/>";

                }
                else if (PostAction == PostAction.Edit)
                {
                    EditedPost = ForumDataProvider.GetPostByID(TenantProvider.CurrentTenantID, parentPostId);

                    if (EditedPost == null)
                    {
                        Response.Redirect(_settings.StartPageAbsolutePath);
                        return;
                    }

                    if (!_forumManager.ValidateAccessSecurityAction(ForumAction.PostEdit, EditedPost))
                    {
                        Response.Redirect("default.aspx");
                        return;
                    }

                    Topic = ForumDataProvider.GetTopicByID(TenantProvider.CurrentTenantID, EditedPost.TopicID);
                    _text = EditedPost.Text;
                    _subject = EditedPost.Subject;
                }
            }

            if (!IsPostBack && !_mobileVer)
                FCKeditor.Value = _text;

            if (PostType != NewPostType.Poll)
                _pollMaster.Visible = false;
            else
                _pollMaster.QuestionFieldID = "forum_subject";


            if (IsPostBack)
            {
                _attachmentsString = Request["forum_attachments"] ?? "";

                #region IsPostBack

                if (PostType == NewPostType.Topic)
                {
                    _subject = string.IsNullOrEmpty(Request["forum_subject"]) ? string.Empty : Request["forum_subject"].Trim();
                }
                else if (PostType == NewPostType.Poll)
                {
                    _subject = string.IsNullOrEmpty(_pollMaster.Name) ? string.Empty : _pollMaster.Name.Trim();
                }

                if (String.IsNullOrEmpty(_subject) && PostType != NewPostType.Post)
                {
                    _subject = "";
                    _errorMessage = "<div class=\"errorBox\">" + Resources.ForumUCResource.ErrorSubjectEmpty + "</div>";
                    return;
                }

                if (!String.IsNullOrEmpty(Request["forum_tags"]))
                {
                    Regex r = new Regex(@"\s*,\s*", RegexOptions.Compiled);
                    _tagString = r.Replace(Request["forum_tags"].Trim(), ",");
                }

                if (!String.IsNullOrEmpty(Request["forum_search_tags"]))
                    _tagValues = Request["forum_search_tags"].Trim();

                _text = Request["forum_text"].Trim();
                if (String.IsNullOrEmpty(_text))
                {
                    _text = "";
                    _errorMessage = "<script type='text/javascript'>jq(document).ready (function(){ForumManager.ShowInfoMessage('" + Resources.ForumUCResource.ErrorTextEmpty + "');});</script>";
                    return;
                }
                else
                {
                    _text = _text.Replace("<br />", "<br />\r\n").TrimEnd('\r','\n');

                }

                if (String.IsNullOrEmpty(Request["forum_topSubscription"]))
                    _subscribeViewType = SubscriveViewType.Disable;
                else
                {
                    if (String.Equals(Request["forum_topSubscription"], "1", StringComparison.InvariantCultureIgnoreCase))
                        _subscribeViewType = SubscriveViewType.Checked;
                    else
                        _subscribeViewType = SubscriveViewType.Unchecked;
                }


                if (PostType == NewPostType.Post)
                {
                    if (PostAction == PostAction.Edit)
                    {

                        EditedPost.Subject = _subject;
                        EditedPost.Text = _text;
                        EditedPost.Formatter = _formatter;

                        try
                        {
                            ForumDataProvider.UpdatePost(TenantProvider.CurrentTenantID, EditedPost.ID, EditedPost.Subject, EditedPost.Text, EditedPost.Formatter);
                            if (IsAllowCreateAttachment)
                                CreateAttachments(EditedPost);

                            CommonControlsConfigurer.FCKEditingComplete(_settings.FileStoreModuleID, EditedPost.ID.ToString(), EditedPost.Text, true);
                            var redirectUrl = _settings.PostPageAbsolutePath + "&t=" + EditedPost.TopicID.ToString() + "&p=1#" + EditedPost.ID.ToString();
                            Response.Redirect(redirectUrl);
                            return;
                        }
                        catch (Exception ex)
                        {
                            _errorMessage = "<div class=\"errorBox\">" + ex.Message.HtmlEncode() + "</div>";
                            return;
                        }

                    }
                    else
                    {

                        var post = new Post(Topic.Title, _text);
                        post.TopicID = Topic.ID;
                        post.ParentPostID = ParentPost == null ? 0 : ParentPost.ID;
                        post.Formatter = _formatter;
                        post.IsApproved = _forumManager.ValidateAccessSecurityAction(ForumAction.ApprovePost, Topic);

                        try
                        {
                            post.ID = ForumDataProvider.CreatePost(TenantProvider.CurrentTenantID, post.TopicID, post.ParentPostID,
                                                        post.Subject, post.Text, post.IsApproved, post.Formatter);

                            Topic.PostCount++;

                            CommonControlsConfigurer.FCKEditingComplete(_settings.FileStoreModuleID, post.ID.ToString(), post.Text, false);

                            if (IsAllowCreateAttachment)
                                CreateAttachments(post);

                            NotifyAboutNewPost(post);

                            if (_subscribeViewType != SubscriveViewType.Disable)
                            {
                                _forumManager.PresenterFactory.GetPresenter<ISubscriberView>().SetView(this);
                                if (this.GetSubscriptionState != null)
                                    this.GetSubscriptionState(this, new SubscribeEventArgs(SubscriptionConstants.NewPostInTopic, Topic.ID.ToString(), SecurityContext.CurrentAccount.ID));

                                if (this.IsSubscribe && _subscribeViewType == SubscriveViewType.Unchecked)
                                {
                                    if (this.UnSubscribe != null)
                                        this.UnSubscribe(this, new SubscribeEventArgs(SubscriptionConstants.NewPostInTopic, Topic.ID.ToString(), SecurityContext.CurrentAccount.ID));
                                }
                                else if (!this.IsSubscribe && _subscribeViewType == SubscriveViewType.Checked)
                                {
                                    if (this.Subscribe != null)
                                        this.Subscribe(this, new SubscribeEventArgs(SubscriptionConstants.NewPostInTopic, Topic.ID.ToString(), SecurityContext.CurrentAccount.ID));
                                }
                            }

                            int numb_page = Convert.ToInt32(Math.Ceiling(Topic.PostCount / (_settings.PostCountOnPage * 1.0)));

                            var postURL = _settings.LinkProvider.Post(post.ID, Topic.ID, numb_page);

                            if (_settings.ActivityPublisher != null)
                                _settings.ActivityPublisher.NewPost(post, Topic.Title, Topic.ThreadID, postURL);

                            Response.Redirect(postURL);
                            return;
                        }
                        catch (Exception ex)
                        {
                            _errorMessage = "<div class=\"errorBox\">" + ex.Message.HtmlEncode() + "</div>";
                            return;
                        }
                #endregion
                    }
                }
                if (PostType == NewPostType.Topic || PostType == NewPostType.Poll)
                {
                    if (PostType == NewPostType.Poll && _pollMaster.AnswerVariants.Count < 2)
                    {
                        _errorMessage = "<div class=\"errorBox\">" + Resources.ForumUCResource.ErrorPollVariantCount + "</div>";
                        return;
                    }

                    try
                    {
                        var topic = new Topic(_subject, TopicType.Informational);
                        topic.ThreadID = Thread.ID;
                        topic.Tags = CreateTags(Thread);
                        topic.Type = (PostType == NewPostType.Poll ? TopicType.Poll : TopicType.Informational);

                        topic.ID = ForumDataProvider.CreateTopic(TenantProvider.CurrentTenantID, topic.ThreadID, topic.Title, topic.Type);
                        Topic = topic;

                        foreach (var tag in topic.Tags)
                        {
                            if (tag.ID == 0)
                                ForumDataProvider.CreateTag(TenantProvider.CurrentTenantID, topic.ID, tag.Name, tag.IsApproved);
                            else
                                ForumDataProvider.AttachTagToTopic(TenantProvider.CurrentTenantID, tag.ID, topic.ID);
                        }

                        var post = new Post(topic.Title, _text);
                        post.TopicID = topic.ID;
                        post.ParentPostID = 0;
                        post.Formatter = _formatter;
                        post.IsApproved = _forumManager.ValidateAccessSecurityAction(ForumAction.ApprovePost, Topic);

                        post.ID = ForumDataProvider.CreatePost(TenantProvider.CurrentTenantID, post.TopicID, post.ParentPostID,
                                                        post.Subject, post.Text, post.IsApproved, post.Formatter);

                        CommonControlsConfigurer.FCKEditingComplete(_settings.FileStoreModuleID, post.ID.ToString(), post.Text, false);

                        if (IsAllowCreateAttachment)
                            CreateAttachments(post);

                        if (PostType == NewPostType.Poll)
                        {
                            var answerVariants = new List<string>();
                            foreach (var answVariant in _pollMaster.AnswerVariants)
                                answerVariants.Add(answVariant.Name);

                            topic.QuestionID = ForumDataProvider.CreatePoll(TenantProvider.CurrentTenantID, topic.ID,
                                                _pollMaster.Singleton ? QuestionType.OneAnswer : QuestionType.SeveralAnswer,
                                                topic.Title, answerVariants);
                        }

                        NotifyAboutNewPost(post);

                        if (_subscribeViewType == SubscriveViewType.Checked)
                        {
                            _forumManager.PresenterFactory.GetPresenter<ISubscriberView>().SetView(this);
                            if (this.Subscribe != null)
                                this.Subscribe(this, new SubscribeEventArgs(SubscriptionConstants.NewPostInTopic, topic.ID.ToString(), SecurityContext.CurrentAccount.ID));
                        }

                        if (_settings.ActivityPublisher != null)
                            _settings.ActivityPublisher.NewTopic(topic);

                        Response.Redirect(_settings.LinkProvider.Post(post.ID, topic.ID, 1));
                        return;

                    }
                    catch (Exception ex)
                    {
                        _errorMessage = "<div class=\"errorBox\">" + ex.Message.HtmlEncode() + "</div>";
                        return;
                    }
                }
            }
            else
            {
                _forumManager.PresenterFactory.GetPresenter<ISubscriberView>().SetView(this);
                if (PostType == NewPostType.Poll || PostType == NewPostType.Topic)
                {
                    if (this.GetSubscriptionState != null)
                        this.GetSubscriptionState(this, new SubscribeEventArgs(SubscriptionConstants.NewPostInThread, Thread.ID.ToString(), SecurityContext.CurrentAccount.ID));

                    if (this.IsSubscribe)
                        _subscribeViewType = SubscriveViewType.Disable;
                    else
                        _subscribeViewType = SubscriveViewType.Checked;
                }
                else if (PostType == NewPostType.Post && PostAction != PostAction.Edit)
                {
                    if (this.GetSubscriptionState != null)
                        this.GetSubscriptionState(this, new SubscribeEventArgs(SubscriptionConstants.NewPostInThread, Topic.ThreadID.ToString(), SecurityContext.CurrentAccount.ID));

                    if (this.IsSubscribe)
                        _subscribeViewType = SubscriveViewType.Disable;
                    else
                    {
                        if (SubscriptionConstants.SubscriptionProvider.IsUnsubscribe(new DirectRecipient(SecurityContext.CurrentAccount.ID.ToString(), SecurityContext.CurrentAccount.Name),
                                                                                 SubscriptionConstants.NewPostInTopic, Topic.ID.ToString()))
                        {
                            _subscribeViewType = SubscriveViewType.Unchecked;
                        }
                        else
                            _subscribeViewType = SubscriveViewType.Checked;
                    }
                }
                else
                    _subscribeViewType = SubscriveViewType.Disable;
            }
        }

        private void NotifyAboutNewPost(Post post)
        {
            string url = VirtualPathUtility.ToAbsolute("~/");
            int numb_page = Convert.ToInt32(Math.Ceiling(Topic.PostCount / (_settings.PostCountOnPage * 1.0)));

            string hostUrl = CommonLinkUtility.ServerRootPath;

            string topicURL = hostUrl + _settings.LinkProvider.PostList(Topic.ID);
            string postURL = hostUrl + _settings.LinkProvider.Post(post.ID, Topic.ID, numb_page);
            string threadURL = hostUrl + _settings.LinkProvider.TopicList(Topic.ThreadID);
            string userURL = hostUrl + CommonLinkUtility.GetUserProfile(post.PosterID, _settings.ProductID);

            string postText = HtmlUtility.GetFull(post.Text, _settings.ProductID);

            var initatorInterceptor = new InitiatorInterceptor(new DirectRecipient(SecurityContext.CurrentAccount.ID.ToString(), SecurityContext.CurrentAccount.Name));
            try
            {
                SubscriptionConstants.NotifyClient.AddInterceptor(initatorInterceptor);

                var poster = CoreContext.UserManager.GetUsers(SecurityContext.CurrentAccount.ID);

                _forumManager.PresenterFactory.GetPresenter<INotifierView>().SetView(this);
                SubscriptionConstants.NotifyClient.BeginSingleRecipientEvent(SubscriptionConstants.SyncName);
                if (SendNotify != null)
                {
                    if (PostType == NewPostType.Poll || PostType == NewPostType.Topic)
                        SendNotify(this, new NotifyEventArgs(SubscriptionConstants.NewTopicInForum,
                                            null) { ThreadTitle = Topic.ThreadTitle, TopicId = Topic.ID, TopicTitle = Topic.Title, Poster = poster, Date = post.CreateDate.ToShortString(), PostURL = postURL, TopicURL = topicURL, ThreadURL = threadURL, UserURL = userURL, PostText = postText });


                    SendNotify(this, new NotifyEventArgs(SubscriptionConstants.NewPostInThread,
                                         Topic.ThreadID.ToString()) { ThreadTitle = Topic.ThreadTitle, TopicTitle = Topic.Title, Poster = poster, Date = post.CreateDate.ToShortString(), PostURL = postURL, TopicURL = topicURL, ThreadURL = threadURL, UserURL = userURL, PostText = postText, TopicId = Topic.ID, PostId = post.ID, TenantId = Topic.TenantID});

                    SendNotify(this, new NotifyEventArgs(SubscriptionConstants.NewPostInTopic,
                                                Topic.ID.ToString()) { ThreadTitle = Topic.ThreadTitle, TopicTitle = Topic.Title, Poster = poster, Date = post.CreateDate.ToShortString(), PostURL = postURL, TopicURL = topicURL, ThreadURL = threadURL, UserURL = userURL, PostText = postText, TopicId = Topic.ID, PostId = post.ID, TenantId = Topic.TenantID });


                }
                SubscriptionConstants.NotifyClient.EndSingleRecipientEvent(SubscriptionConstants.SyncName);
            }
            finally
            {
                SubscriptionConstants.NotifyClient.RemoveInterceptor(initatorInterceptor.Name);
            }
        }

        protected void TopicHeader()
        {
            if (PostType == NewPostType.Post || PostType == NewPostType.Poll)
                return;

            StringBuilder sb = new StringBuilder();
            sb.Append("<div class=\"requiredField\">");
            sb.Append("<span class=\"requiredErrorText\"></span>");
            sb.Append("<div class=\"headerPanel\" style='margin-top:0px;'>");
            sb.Append(Resources.ForumUCResource.Topic);
            sb.Append("</div>");
            sb.Append("<div style=\"padding-top:3px;\">");
            sb.Append("<input class=\"textEdit\" style=\"width:100%;\" maxlength=\"450\" name=\"forum_subject\" id=\"forum_subject\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(_subject) + "\" />");
            sb.Append("</div>");
            sb.Append("</div>");
            Response.Write(sb.ToString());
        }

        protected string RenderForumSelector()
        {
            if (!_isSelectForum)
                return "";

            if (_threads == null)
            {
                ForumDataProvider.GetThreadCategories(TenantProvider.CurrentTenantID, out _categories, out _threads);

                _threads.RemoveAll(t => !t.Visible);
            }

            StringBuilder sb = new StringBuilder();
            sb.Append("<div class=\"headerPanel\" style='margin-top:0px;'>");
            sb.Append(Resources.ForumUCResource.Thread);
            sb.Append("</div>");

            sb.Append("<div style=\"padding-top:3px;  padding-bottom: 20px;\">");
            sb.Append("<select name=\"forum_thread_id\" class=\"comboBox\" style='width:100%;'>");

            foreach (var forum in _threads)
            {
                bool isAllow = false;
                if (PostType == NewPostType.Topic)
                    isAllow = _forumManager.ValidateAccessSecurityAction(ForumAction.TopicCreate, forum);
                else if (PostType == NewPostType.Poll)
                    isAllow = _forumManager.ValidateAccessSecurityAction(ForumAction.PollCreate, forum);

                if (isAllow)
                    sb.Append("<option value=\"" + forum.ID + "\">" + forum.Title.HtmlEncode() + "</option>");
            }

            sb.Append("</select>");
            sb.Append("</div>");

            return sb.ToString();
        }



        private List<Tag> CreateTags(Thread thread)
        {
            if (string.IsNullOrEmpty(_tagString.Trim(',')) || !_forumManager.ValidateAccessSecurityAction(ForumAction.TagCreate, thread))
            {
                return new List<Tag>();
            }

            List<Tag> list;
            List<Tag> existingTags = ForumDataProvider.GetAllTags(TenantProvider.CurrentTenantID);

            var newTags = (from tagName in _tagString.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                           where !existingTags.Exists(et => et.Name.Equals(tagName, StringComparison.InvariantCultureIgnoreCase))
                           select new Tag() { ID = 0, Name = tagName }).Distinct(new TagComparer());

            var exTags = from exTag in existingTags
                         from tagName in _tagString.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                         where tagName.Equals(exTag.Name, StringComparison.InvariantCultureIgnoreCase)
                         select exTag;

            list = new List<Tag>(newTags.ToArray());
            list.AddRange(exTags.ToArray());
            return list;
        }

        protected void AddTags()
        {
            if (PostType != NewPostType.Topic && PostType != NewPostType.Poll)
                return;

            if (!_forumManager.ValidateAccessSecurityAction(ForumAction.TagCreate, Thread))
                return;

            StringBuilder sb = new StringBuilder();
            sb.Append("<div style=\"margin-top:20px;\">");

            sb.Append("<div class=\"headerPanel\">" + Resources.ForumUCResource.Tags + "</div>");
            sb.Append("<div>");
            sb.Append("<input autocomplete=\"off\" class=\"textEdit\" style=\"width:100%\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(_tagString) + "\" maxlength=\"3000\" id=\"forum_tags\" name=\"forum_tags\"/>");
            sb.Append("<input type='hidden' id='forum_search_tags' name='forum_search_tags'/>");
            sb.Append("</div>");
            sb.Append("<div class=\"textMediumDescribe\" style=\"padding-top:2px;\">" + Resources.ForumUCResource.HelpForTags + "</div>");


            sb.Append("<script> var ForumTagSearchHelper = new SearchHelper('forum_tags','forum_sh_item','forum_sh_itemselect','',\"ForumManager.SaveSearchTags(\'forum_search_tags\',ForumTagSearchHelper.SelectedItem.Value,ForumTagSearchHelper.SelectedItem.Help);\",\"TagSuggest\", \"GetSuggest\",\"'" + _settings.ID + "',\",true,false);</script>");
            sb.Append("</div>");
            Response.Write(sb.ToString());
        }

        #region Attachments

        public bool IsAllowCreateAttachment
        {
            get
            {
                if (ASC.Web.Core.Mobile.MobileDetector.IsRequestMatchesMobile(Context))
                    return false;

                if (PostType == NewPostType.Post)
                    return _forumManager.ValidateAccessSecurityAction(ForumAction.AttachmentCreate, Topic);

                else if (PostType == NewPostType.Poll || PostType == NewPostType.Topic)
                    return _forumManager.ValidateAccessSecurityAction(ForumAction.AttachmentCreate, Thread);

                return false;
            }
        }

        private void CreateAttachments(Post post)
        {
            foreach (var attachItem in _attachmentsString.Split(new string[] { "%$%" }, StringSplitOptions.RemoveEmptyEntries))
            {
                Attachment attachment = new Attachment();
                var attachmentOptions = attachItem.Split(new string[] { "$@$" }, StringSplitOptions.RemoveEmptyEntries);
                attachment.OffsetPhysicalPath = GetNotNullArrayItem(attachmentOptions, 0);

                if (_forumManager.GetStore().IsFile(attachment.OffsetPhysicalPath) == false)
                    continue;

                attachment.PostID = post.ID;
                attachment.Name = GetNotNullArrayItem(attachmentOptions, 1);
                attachment.Size = Convert.ToInt32(GetNotNullArrayItem(attachmentOptions, 2));
                attachment.MIMEContentType = GetNotNullArrayItem(attachmentOptions, 3);

                attachment.ID = ForumDataProvider.CreateAttachment(TenantProvider.CurrentTenantID, post.ID, attachment.Name, attachment.OffsetPhysicalPath,
                        attachment.Size, attachment.ContentType, attachment.MIMEContentType);

                post.Attachments.Add(attachment);
            }
        }
        #region Array Items Helper
        private static string GetNotNullArrayItem(string[] a, int index)
        {
            if (a == null || a.Length == 0)
            {
                return string.Empty;
            }
            return a.Length > index ? a[index] : string.Empty;
        }
        #endregion
        #endregion

        protected string RenderSubscription()
        {
            if (_subscribeViewType == SubscriveViewType.Disable)
                return "";

            return "<input id='forum_topSubscriptionState' name='forum_topSubscription' value='" + (_subscribeViewType == SubscriveViewType.Checked ? "1" : "0") + "' type='hidden'/><input id=\"forum_topSubscription\" " + (_subscribeViewType == SubscriveViewType.Checked ? "checked='checked'" : "") + " type=\"checkbox\"/><label style='margin-left:5px; padding-top:2px;' for=\"forum_topSubscription\">" + Resources.ForumUCResource.SubscribeOnTopic + "</label>";
        }



        [AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public AjaxResponse Preview(string text, Guid settingsID)
        {
            _settings = ForumManager.GetSettings(settingsID);
            UserInfo currentUser = CoreContext.UserManager.GetUsers(SecurityContext.CurrentAccount.ID);
            StringBuilder sb = new StringBuilder();

            sb.Append("<div class=\"tintLight borderBase clearFix\" style=\"padding:10px 0px; border-top:none; border-left:none; border-right:none;\">");
            sb.Append("<table cellpadding=\"0\" cellspacing=\"0\" style='width:100%;'>");
            sb.Append("<tr valign=\"top\">");

            sb.Append("<td align=\"center\" style='width:180px; padding:0px 5px;'>");

            sb.Append("<div class=\"forum_postBoxUserSection\" style=\"overflow: hidden; width:150px;\">");
            sb.Append("<a class=\"linkHeader\"  href=\"" + CommonLinkUtility.GetUserProfile(currentUser.ID, _settings.ProductID) + "\">" + currentUser.DisplayUserName() + "</a>");

            sb.Append("<div style=\"margin:5px 0px;\" class=\"textMediumDescribe\">");
            sb.Append(HttpUtility.HtmlEncode(currentUser.Title));
            sb.Append("</div>");

            sb.Append("<a href=" + CommonLinkUtility.GetUserProfile(currentUser.ID, _settings.ProductID) + ">");
            sb.Append(_settings.ForumManager.GetHTMLImgUserAvatar(currentUser.ID));
            sb.Append("</a>");
            sb.Append("</div>");
            sb.Append("</td>");

            //post
            sb.Append("<td>");
            sb.Append("<div style='margin-bottom:5px; padding:0px 5px;'>");
            sb.Append(DateTimeService.DateTime2StringPostStyle(DateTimeService.CurrentDate()));
            sb.Append("</div>");

            var previewID = Guid.NewGuid().ToString();

            sb.Append("<div id=\"forum_message_" + previewID + "\" class=\"forum_mesBox\" style=\"width:550px;\">");
            sb.Append(text);
            sb.Append("</div>");

            sb.Append("</td></tr></table>");
            sb.Append("</div>");

            AjaxResponse resp = new AjaxResponse();
            resp.rs1 = sb.ToString();
            return resp;
        }

        [AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public void RemoveAttachment(Guid settingsID, string offsetPath)
        {
            var _settings = ForumManager.GetSettings(settingsID);
            _settings.ForumManager.RemoveAttachments(offsetPath);

        }

        #region INotifierView Members

        public event EventHandler<NotifyEventArgs> SendNotify;

        #endregion

        #region ISubscriberView Members

        public event EventHandler<SubscribeEventArgs> GetSubscriptionState;

        public bool IsSubscribe { get; set; }

        public event EventHandler<SubscribeEventArgs> Subscribe;

        public event EventHandler<SubscribeEventArgs> UnSubscribe;

        public event EventHandler<SubscribeEventArgs> UnSubscribeForSubscriptionType;

        #endregion


        protected string RenderRedirectUpload()
        {
            return string.Format("{0}://{1}:{2}{3}", Request.GetUrlRewriter().Scheme, Request.GetUrlRewriter().Host, Request.GetUrlRewriter().Port,
                VirtualPathUtility.ToAbsolute("~/") + "fckuploader.ashx?esid=" + _settings.FileStoreModuleID + (PostAction == PostAction.Edit ? "&iid=" + EditedPost.ID.ToString() : ""));
        }

        protected void OnUnSubscribeForSubscriptionType(INotifyAction action, string objId, Guid userId)
        {
            if (UnSubscribeForSubscriptionType != null) UnSubscribeForSubscriptionType(this, new SubscribeEventArgs(action, objId, userId));
        }

        [AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public string CancelPost(Guid settingsID, string itemID)
        {
            var _settings = ForumManager.GetSettings(settingsID);
            if (String.IsNullOrEmpty(itemID) == false)
                CommonControlsConfigurer.FCKEditingCancel(_settings.FileStoreModuleID, itemID);
            else
                CommonControlsConfigurer.FCKEditingCancel(_settings.FileStoreModuleID);

            if (httpContext != null && httpContext.Request != null &&
                httpContext.Request.UrlReferrer != null &&
                !string.IsNullOrEmpty(httpContext.Request.UrlReferrer.Query))
            {
                var q = httpContext.Request.UrlReferrer.Query;
                var start = q.IndexOf("t=");

                if (start == -1)
                    start = q.IndexOf("f=");

                start += 2;


                var end = q.IndexOf("&", start, StringComparison.CurrentCultureIgnoreCase);
                if (end == -1)
                    end = q.Length;

                var t = q.Substring(start, end - start);

                return (q.IndexOf("t=") > 0 ? _settings.PostPageAbsolutePath + "t=" : _settings.TopicPageAbsolutePath + "f=") + t;
            }
            else
                return _settings.StartPageAbsolutePath;
        }

        #region IContextInitializer Members

        public void InitializeContext(HttpContext context)
        {
            httpContext = context;
        }

        HttpContext httpContext = null;

        #endregion
    }

}
