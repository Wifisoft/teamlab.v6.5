using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;
using AjaxPro;
using ASC.Blogs.Core;
using ASC.Blogs.Core.Domain;
using ASC.Web.Community.Blogs.Views;
using ASC.Web.Community.Product;
using ASC.Web.Core.Utility.Skins;
using ASC.Web.Studio.Core;
using ASC.Web.Studio.Utility;

namespace ASC.Web.Community.Blogs
{
    public partial class EditBlog : BasePage
    {
        protected bool _mobileVer = false;
        protected string _text = "";

        private string BlogId
        {
            get
            {
                return Request.QueryString["blogID"];
            }
        }

        protected override void PageLoad()
        {
            Utility.RegisterTypeForAjax(typeof(AddBlog));

            if (SetupInfo.WorkMode == WorkMode.Promo)
                Response.Redirect(ASC.Blogs.Core.Constants.DefaultPageUrl, true);

            if (String.IsNullOrEmpty(BlogId))
                Response.Redirect(ASC.Blogs.Core.Constants.DefaultPageUrl);

            _mobileVer = ASC.Web.Core.Mobile.MobileDetector.IsRequestMatchesMobile(this.Context);

            var engine = BasePage.GetEngine();

            AjaxPro.Utility.RegisterTypeForAjax(typeof(EditBlog), this.Page);

            FCKeditor.BasePath = VirtualPathUtility.ToAbsolute(CommonControlsConfigurer.FCKEditorBasePath);
            FCKeditor.ToolbarSet = "BlogToolbar";
            FCKeditor.EditorAreaCSS = WebSkin.GetUserSkin().BaseCSSFileAbsoluteWebPath;

            FCKeditor.Visible = !_mobileVer;

            if (_mobileVer && IsPostBack)
                _text = Request["mobiletext"];

            mainContainer.BreadCrumbs.Add(new ASC.Web.Controls.BreadCrumb() { Caption = ASC.Blogs.Core.Resources.BlogsResource.AddonName, NavigationUrl = VirtualPathUtility.ToAbsolute(ASC.Blogs.Core.Constants.BaseVirtualPath) });
            mainContainer.BreadCrumbs.Add(new ASC.Web.Controls.BreadCrumb() { Caption = ASC.Blogs.Core.Resources.BlogsResource.EditPostTitle });

            this.Title = HeaderStringHelper.GetPageTitle(ASC.Blogs.Core.Resources.BlogsResource.AddonName, mainContainer.BreadCrumbs);

            
			InitSidePanel(engine, TagCloud);
			sideRecentActivity.TenantId = TenantProvider.CurrentTenantID;
			sideRecentActivity.ProductId = Product.CommunityProduct.ID;
			sideRecentActivity.ModuleId = ASC.Blogs.Core.Constants.ModuleID;

            base.InitSubscribers(actions);

            ShowForEdit(engine);

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
                    if (CheckTitle(txtTitle.Text))
                    {
                        var pageEngine = BasePage.GetEngine();
                        var post = pageEngine.GetPostById(new Guid(hidBlogID.Value));
                        UpdatePost(post, engine);
                    }
                    else
                    {
                        mainContainer.Options.InfoMessageText = ASC.Blogs.Core.Resources.BlogsResource.BlogTitleEmptyMessage;
                        mainContainer.Options.InfoType = ASC.Web.Controls.InfoType.Alert;
                    }
                }
            }
        }

        private void InitPreviewTemplate(Post post)
        {
            ViewBlogView control = (ViewBlogView)LoadControl("~/Products/Community/Modules/Blogs/Views/ViewBlogView.ascx");
            control.IsPreview = true;
            control.post = post;

            PlaceHolderPreview.Controls.Add(new Literal() { Text = "<div class='headerPanel' style='margin-top:20px;'>" + ASC.Blogs.Core.Resources.BlogsResource.PreviewButton + "</div>" });
            PlaceHolderPreview.Controls.Add(control);
            PlaceHolderPreview.Controls.Add(new Literal() { Text = "<div style='margin-top:10px;'><a class='baseLinkButton' href='javascript:void(0);' onclick='BlogsManager.HidePreview(); return false;'>" + ASC.Blogs.Core.Resources.BlogsResource.HideButton + "</a></div>" });

        }


        private void ShowForEdit(BlogsEngine engine)
        {

            if (!IsPostBack)
            {
                Post post = engine.GetPostById(new Guid(BlogId));

                InitPreviewTemplate(post);

                if (post != null && CommunitySecurity.CheckPermissions(post, ASC.Blogs.Core.Constants.Action_EditRemovePost))
                {
                    hdnUserID.Value = post.UserID.ToString();

                    if (Request.QueryString["action"] == "delete")
                    {
                        foreach (var comment in engine.GetPostComments(post.ID))
                        {
                            CommonControlsConfigurer.FCKUploadsRemoveForItem("blogs_comments", comment.ID.ToString());
                        }

                        engine.DeletePost(post);
                        CommonControlsConfigurer.FCKUploadsRemoveForItem("blogs", post.ID.ToString());
                        Response.Redirect(ASC.Blogs.Core.Constants.DefaultPageUrl);
                        return;
                    }
                    else
                    {
                        txtTitle.Text = Server.HtmlDecode(post.Title);

                        if (_mobileVer)
                            _text = post.Content;
                        else
                            FCKeditor.Value = post.Content;

                        hidBlogID.Value = post.ID.ToString();

                        LoadTags(post.TagList);
                    }
                }
                else
                {
                    Response.Redirect(ASC.Blogs.Core.Constants.DefaultPageUrl);
                    return;
                }


            }
        }

        private void LoadTags(IList<Tag> tags)
        {
            StringBuilder sb = new StringBuilder();

            int i = 0;
            foreach (Tag tag in tags)
            {
                if (i != 0)
                    sb.Append(", " + tag.Content);
                else
                    sb.Append(tag.Content);
                i++;
            }

            txtTags.Text = sb.ToString();
        }



        private bool CheckTitle(string title)
        {
            if (string.IsNullOrEmpty(title) || string.IsNullOrEmpty(title.Trim()))
            {
                return false;
            }
            return true;
        }

        private bool IsExistsTagName(Post post, string tagName)
        {
            foreach (Tag tag in post.TagList)
            {
                if (tag.Content == tagName)
                    return true;
            }

            return false;
        }

        protected override string RenderRedirectUpload()
        {
            return string.Format("{0}://{1}:{2}{3}", Request.GetUrlRewriter().Scheme, Request.GetUrlRewriter().Host, Request.GetUrlRewriter().Port, VirtualPathUtility.ToAbsolute("~/") + "fckuploader.ashx?esid=blogs&iid=" + BlogId);
        }



        public void UpdatePost(Post post, BlogsEngine engine)
        {
            post.Title = GetLimitedText(txtTitle.Text);
            post.Content = _mobileVer ? (Request["mobiletext"]??"") : FCKeditor.Value;

            post.TagList = new List<Tag>();

            foreach (string tagName in txtTags.Text.Split(','))
            {
                if (tagName != string.Empty && !IsExistsTagName(post, tagName))
                {
                    Tag tag = new Tag(post);
                    tag.Content = GetLimitedText(tagName.Trim());
                    post.TagList.Add(tag);
                }
            }


            engine.SavePost(post,false,false);

            CommonControlsConfigurer.FCKEditingComplete("blogs", post.ID.ToString(), post.Content, true);



            Response.Redirect("viewblog.aspx?blogid=" + post.ID.ToString());

        }

        #region Events

        protected void lbCancel_Click(object sender, EventArgs e)
        {
            CommonControlsConfigurer.FCKEditingCancel("blogs", BlogId);
            Response.Redirect(ASC.Blogs.Core.Constants.DefaultPageUrl);
        }


        #endregion

    }
}
