using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;
using AjaxPro;
using ASC.Blogs.Core;
using ASC.Blogs.Core.Domain;
using ASC.Blogs.Core.Resources;
using ASC.Blogs.Core.Security;
using ASC.Core;
using ASC.Web.Community.Blogs.Views;
using ASC.Web.Community.Product;
using ASC.Web.Controls;
using ASC.Web.Core.Utility.Skins;
using ASC.Web.Studio.Core;
using ASC.Web.Studio.Utility;

namespace ASC.Web.Community.Blogs
{

	[AjaxNamespace("BlogsPage")]
    public partial class AddBlog : BasePage
    {
        protected bool _mobileVer = false;
        protected string _text = "";

        protected override void PageLoad()
        {		

            if (SetupInfo.WorkMode == WorkMode.Promo)
                Response.Redirect(ASC.Blogs.Core.Constants.DefaultPageUrl, true);

            _mobileVer = ASC.Web.Core.Mobile.MobileDetector.IsRequestMatchesMobile(this.Context);

            //lbtnPost.Text = BlogsResource.PostButton;
            FCKeditor.BasePath = VirtualPathUtility.ToAbsolute(CommonControlsConfigurer.FCKEditorBasePath);
            FCKeditor.ToolbarSet = "BlogToolbar";
            FCKeditor.EditorAreaCSS = WebSkin.GetUserSkin().BaseCSSFileAbsoluteWebPath;
            FCKeditor.Visible = !_mobileVer;

            if (_mobileVer && Request["mobiletext"] != null)            
                _text = Request["mobiletext"];

            if (CheckTitle(txtTitle.Text))
            {
                mainContainer.Options.InfoMessageText = "";
            }

            var engine = BasePage.GetEngine();

            mainContainer.BreadCrumbs.Add(new ASC.Web.Controls.BreadCrumb() { Caption = BlogsResource.NewPost });

            this.Title = HeaderStringHelper.GetPageTitle(BlogsResource.AddonName, mainContainer.BreadCrumbs);

            InitPreviewTemplate();

			InitSidePanel(engine, TagCloud);
			sideRecentActivity.TenantId = TenantProvider.CurrentTenantID;
			sideRecentActivity.ProductId = Product.CommunityProduct.ID;
			sideRecentActivity.ModuleId = ASC.Blogs.Core.Constants.ModuleID;

            base.InitSubscribers(actions);

			lbCancel.Attributes["name"] = FCKeditor.ClientID;

            if (IsPostBack)
            {
                var control = FindControl(Request.Params["__EVENTTARGET"]);
                if (lbCancel.Equals(control))
                {
                    Response.Redirect(ASC.Blogs.Core.Constants.DefaultPageUrl);
                }
                else
                {
                    TryPostBlog(BasePage.GetEngine());
                }
            }
        }



        private bool CheckTitle(string title)
        {
            if (string.IsNullOrEmpty(title.Trim()))
            {
                return false;
            }
            return true;
        }

        private bool IsExistsTagName(Post post, string tagName)
        {
            foreach (ASC.Blogs.Core.Domain.Tag tag in post.TagList)
            {
                if (tag.Content == tagName)
                    return true;
            }

            return false;
        }

		[AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
		public AjaxResponse GetSuggest(string text, string varName, int limit)
		{
			AjaxResponse resp = new AjaxResponse();

			string startSymbols = text;
			int ind = startSymbols.LastIndexOf(",");
			if (ind != -1)
				startSymbols = startSymbols.Substring(ind + 1);

			startSymbols = startSymbols.Trim();

			var engine = BasePage.GetEngine();

			List<string> tags = new List<string>();

			if (!string.IsNullOrEmpty(startSymbols))
			{
				tags = engine.GetTags(startSymbols, limit);
			}

			StringBuilder resNames = new StringBuilder();
			StringBuilder resHelps = new StringBuilder();

			foreach (var tag in tags)
			{
				resNames.Append(tag);
				resNames.Append("$");
				resHelps.Append(tag);
				resHelps.Append("$");
			}
			resp.rs1 = resNames.ToString().TrimEnd('$');
			resp.rs2 = resHelps.ToString().TrimEnd('$');
			resp.rs3 = text;
			resp.rs4 = varName;

			return resp;
		}


        [AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public string[] GetPreview(string title, string html)
        {
            string[] result = new string[2];

            result[0] = HttpUtility.HtmlEncode(title);
            result[1] = HtmlUtility.GetFull(html, ASC.Web.Community.Product.CommunityProduct.ID);
            
            return result;
        }


        private Post AddNewBlog(BlogsEngine engine)
        {
            Guid authorId = SecurityContext.CurrentAccount.ID;

            if (CommunitySecurity.CheckPermissions(
                    new PersonalBlogSecObject(CoreContext.UserManager.GetUsers(authorId)), 
                    ASC.Blogs.Core.Constants.Action_AddPost))
            {
                Post newPost = new Post();
                newPost.Content =  _mobileVer? (Request["mobiletext"]??"") : FCKeditor.Value;

                DateTime dateNow = ASC.Core.Tenants.TenantUtil.DateTimeNow();

                newPost.Datetime = dateNow;
                newPost.Title = GetLimitedText(txtTitle.Text);
                newPost.UserID = authorId;

                newPost.TagList = new List<ASC.Blogs.Core.Domain.Tag>();
                foreach (string tagName in txtTags.Text.Split(','))
                {
                    if (tagName != string.Empty && !IsExistsTagName(newPost, tagName))
                    {
                        ASC.Blogs.Core.Domain.Tag tag = new ASC.Blogs.Core.Domain.Tag();
                        tag.Content = GetLimitedText(tagName.Trim());
                        newPost.TagList.Add(tag);
                    }
                }
                

                var blog = engine.EnsurePersonalBlog(authorId);
                engine.SavePost(newPost, true, Request.Form["notify_comments"] == "on");

                CommonControlsConfigurer.FCKEditingComplete("blogs", newPost.ID.ToString(), newPost.Content, false);

                return newPost;
            }
            else
            {
                Response.Redirect("addblog.aspx");
                return null;
            }
        }

        private void TryPostBlog(BlogsEngine engine)
        {
            if (CheckTitle(txtTitle.Text))
            {
                var post = AddNewBlog(engine);

                if (post != null)
                    Response.Redirect("viewblog.aspx?blogid=" + post.ID.ToString());
                else
                    Response.Redirect(ASC.Blogs.Core.Constants.DefaultPageUrl);
            }
            else
            {
                mainContainer.Options.InfoMessageText = BlogsResource.BlogTitleEmptyMessage;
                mainContainer.Options.InfoType = ASC.Web.Controls.InfoType.Alert;
            }
        }
        
        private void InitPreviewTemplate()
        {
            Post post = new Post()
            {
                Datetime = ASC.Core.Tenants.TenantUtil.DateTimeNow(),
                Title = string.Empty,
                Content = string.Empty,
                UserID = SecurityContext.CurrentAccount.ID
            };

            ViewBlogView control = (ViewBlogView)LoadControl("~/products/community/modules/blogs/views/viewblogview.ascx");
            control.IsPreview = true;
            control.post = post;

            PlaceHolderPreview.Controls.Add(new Literal() { Text = "<div class='headerPanel' style='margin-top:20px;'>" + BlogsResource.PreviewButton + "</div>"});
            PlaceHolderPreview.Controls.Add(control);
            PlaceHolderPreview.Controls.Add(new Literal() { Text = "<div style='margin-top:10px;'><a class='baseLinkButton' href='javascript:void(0);' onclick='BlogsManager.HidePreview(); return false;'>" + BlogsResource.HideButton + "</a></div>" });
            
        }

        #region Events

        //protected void lbtnPost_Click(object sender, EventArgs e)
        //{
        //    TryPostBlog(BasePage.GetEngine());
        //}

        protected void lbCancel_Click(object sender, EventArgs e)
        {
            CommonControlsConfigurer.FCKEditingCancel("blogs");
            Response.Redirect(ASC.Blogs.Core.Constants.DefaultPageUrl);
        }

        #endregion
    }
}
