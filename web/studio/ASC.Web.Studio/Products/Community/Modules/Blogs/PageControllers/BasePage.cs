using System;
using System.Text;
using System.Web;
using AjaxPro;
using ASC.Blogs.Core;
using ASC.Core;
using ASC.Data.Storage;
using ASC.Web.Community.Blogs.Controls;
using ASC.Web.Core.Utility.Skins;
using ASC.Web.Studio;
using ASC.Web.Studio.Controls.Common;
using ASC.Web.Studio.Utility;

namespace ASC.Web.Community.Blogs
{
    public abstract class BasePage : MainPage
    {
        protected int GetBlogMaxImageWidth
        {
            get { return 570; }
        }

        public void InitSidePanel(BlogsEngine engine, TagCloud tagCloud)
        {
            tagCloud.SetTags(engine.GetTopTagsList(40));
        }

        public static BlogsEngine GetEngine()
        {
            return BlogsEngine.GetEngine(TenantProvider.CurrentTenantID);
        }

        /// <summary>
        /// Page_Load of the Page Controller pattern.
        /// See http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dnpatterns/html/ImpPageController.asp
        /// </summary>
        protected void Page_Load(object sender, EventArgs e)
        {
            Utility.RegisterTypeForAjax(typeof (AddBlog));
            PageLoad();
        }

        protected abstract void PageLoad();

        protected string GetLimitedText(string text)
        {
            if (string.IsNullOrEmpty(text))
                return "";

            return text.Length > ASC.Blogs.Core.Constants.MAX_TEXT_LENGTH ? text.Substring(0, ASC.Blogs.Core.Constants.MAX_TEXT_LENGTH) : text;
        }

        private Subscriber _subscriber;

        protected void InitSubscribers(ActionContainer actionsContainer)
        {
            InitSubscribers(actionsContainer, Guid.Empty);
        }

        protected void InitSubscribers(ActionContainer actionsContainer, Guid blogAutorID)
        {
            if (!SecurityContext.DemoMode && SecurityContext.IsAuthenticated)
            {
                Utility.RegisterTypeForAjax(typeof (Subscriber));
                _subscriber = new Subscriber();

                AddNewPostSubscriber(actionsContainer);

                if (this is Default && !String.IsNullOrEmpty(Request["userID"]))
                {
                    var userID = Guid.Empty;
                    try
                    {
                        userID = new Guid(Request["userID"]);

                    }
                    catch
                    {
                        userID = Guid.Empty;
                    }

                    if (!userID.Equals(Guid.Empty))
                        AddPersonalBlogSubscriber(userID, actionsContainer);
                }

                if (this is ViewBlog)
                {
                    if (!blogAutorID.Equals(Guid.Empty))
                        AddPersonalBlogSubscriber(blogAutorID, actionsContainer);


                    var blogID = Guid.Empty;
                    try
                    {
                        blogID = new Guid(Request["blogID"]);
                    }
                    catch
                    {
                        blogID = Guid.Empty;
                    }


                    if (!blogID.Equals(Guid.Empty))
                        AddCommentsSubscriber(blogID, actionsContainer);

                }
            }
        }

        private void AddNewPostSubscriber(ActionContainer actionsContainer)
        {
            var isSubscribe = _subscriber.IsNewPostsSubscribe();
            var sb = new StringBuilder();
            sb.Append(_subscriber.RenderNewPostsSubscription(!isSubscribe));

            actionsContainer.ActionsPlaceHolder.Controls.Add(new HtmlMenuItem(sb.ToString()));
        }

        private void AddPersonalBlogSubscriber(Guid userID, ActionContainer actionsContainer)
        {

            var isSubscribe = _subscriber.IsPersonalBlogSubscribe(userID);
            var sb = new StringBuilder();
            sb.Append(_subscriber.RenderPersonalBlogSubscription(!isSubscribe, userID));

            actionsContainer.ActionsPlaceHolder.Controls.Add(new HtmlMenuItem(sb.ToString()));
        }

        private void AddCommentsSubscriber(Guid blogID, ActionContainer actionsContainer)
        {
            var isSubscribe = _subscriber.IsCommentsSubscribe(blogID);
            var sb = new StringBuilder();
            sb.Append(_subscriber.RenderCommentsSubscription(!isSubscribe, blogID));

            actionsContainer.ActionsPlaceHolder.Controls.Add(new HtmlMenuItem(sb.ToString()));
        }

        protected virtual string RenderRedirectUpload()
        {
            return string.Format("{0}://{1}:{2}{3}", Request.GetUrlRewriter().Scheme, Request.GetUrlRewriter().Host, Request.GetUrlRewriter().Port, VirtualPathUtility.ToAbsolute("~/") + "fckuploader.ashx?esid=blogs");
        }

        protected string RenderScripts()
        {
            var sb = new StringBuilder();
            sb.AppendFormat("<link href=\"{0}\" rel=\"stylesheet\" type=\"text/css\"/>",
                            WebSkin.GetUserSkin().GetAbsoluteWebPath("/products/community/modules/blogs/app_themes/<theme_folder>/blogstyle.css"));

            sb.AppendFormat("<script language=\"javascript\" src=\"{0}\" type=\"text/javascript\"></script>",
                            WebPath.GetPath("/products/community/modules/blogs/js/blogs.js"));

            sb.AppendFormat("<script language=\"javascript\" src=\"{0}\" type=\"text/javascript\"></script>",
                            WebPath.GetPath("js/tagsautocompletebox.js"));



            string script = @"
function createSearchHelper() {

	var ForumTagSearchHelper = new SearchHelper(
		'_txtTags', 
		'tagAutocompleteItem', 
		'tagAutocompleteSelectedItem', 
		'', 
		'', 
		'BlogsPage',
		'GetSuggest',
		'', 
		true,
		false
	);
}
";

            Page.ClientScript.RegisterClientScriptBlock(typeof (string), "blogsTagsAutocompleteInitScript", script, true);

            return sb.ToString();

        }
    }
}