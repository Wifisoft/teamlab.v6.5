using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ASC.Web.Controls.CommentInfoHelper;

namespace ASC.Web.Controls
{

    [ToolboxData("<{0}:CommentsList runat=server></{0}:CommentsList>")]
    public class CommentsList : WebControl
    {
        public delegate string FCKBasePathRequestHandler();

        public event FCKBasePathRequestHandler FCKBasePathRequest;

        public CommentsList()
        {
            ProductId = Guid.Empty;
            var codeHighlighter = new CodeHighlighter();
            this.Controls.Add(codeHighlighter);
        }

        #region Members

        private static bool isClientScriptRegistered;

        private int maxDepthLevel = 8;

        private int commentIndex = 0;

        private string _jsObjName
        {
            get { return String.IsNullOrEmpty(this.BehaviorID) ? "__comments" + this.UniqueID : this.BehaviorID; }
        }

        private IList<CommentInfo> items = new List<CommentInfo>(0);
        private string userPageLinkWithParam = string.Empty;
        private string inactiveMessage = string.Empty;
        private string editCommentToolTip = string.Empty;
        private string responseCommentToolTip = string.Empty;
        private string removeCommentToolTip = string.Empty;

        private string editCommentLink = "Edit";
        private string removeCommentLink = "Remove";
        private string responseCommentLink = "Answer";

        private string previewButton = string.Empty;
        private string saveButton = string.Empty;
        private string cancelButton = string.Empty;
        private string hidePrevuewButton = string.Empty;
        private string addCommentLink = string.Empty;
        private string commentsTitle = string.Empty;
        private string commentsCountTitle = string.Empty;
        private string javaScriptRemoveCommentFunctionName = string.Empty;
        private string javaScriptPreviewCommentFunctionName = string.Empty;
        private string javaScriptAddCommentFunctionName = string.Empty;
        private string javaScriptUpdateCommentFunctionName = string.Empty;
        private string javaScriptLoadBBcodeCommentFunctionName = string.Empty;
        private string javaScriptCallBackAddComment = string.Empty;
        private string objectID = string.Empty;
        private bool isShowAddCommentBtn = true;
        private string confirmRemoveCommentMessage = string.Empty;
        private bool showCaption = true;


        private string responseImageFile;
        private string editImageFile;
        private string removeImageFile;
        private string newImageFile;
        private string commentsImageFile;

        private bool simple = false;


        #endregion

        #region Properties

        public string FckDomainName { get; set; }

        public string OnEditedCommentJS { get; set; }
        public string OnRemovedCommentJS { get; set; }
        public string OnCanceledCommentJS { get; set; }

        public int MaxDepthLevel
        {
            get { return maxDepthLevel; }
            set { maxDepthLevel = value; }
        }

        public string CommentSendingMsg { get; set; }

        public string LoaderImage { get; set; }

        public string AttachButton { get; set; }

        public string RemoveAttachButton { get; set; }

        public Guid ProductId { get; set; }

        public bool EnableAttachmets { get; set; }

        public string HandlerTypeName { get; set; }

        public bool DisableCtrlEnter { get; set; }

        public string AdditionalSubmitText { get; set; }

        public string PID { get; set; }

        public string EditCommentLink
        {
            get { return editCommentLink; }
            set { editCommentLink = value; }
        }

        public string RemoveCommentLink
        {
            get { return removeCommentLink; }
            set { removeCommentLink = value; }
        }

        public string ResponseCommentLink
        {
            get { return responseCommentLink; }
            set { responseCommentLink = value; }
        }

        public bool Simple
        {
            get { return simple; }
            set { simple = value; }
        }

        //FCKEditorsOprions
        public string FCKBasePath
        {

            get
            {
                if (ViewState["FCKBasePath"] == null || ViewState["FCKBasePath"].ToString().Equals(string.Empty))
                {
                    if (FCKBasePathRequest != null)
                    {
                        string result = FCKBasePathRequest();
                        if (!string.IsNullOrEmpty(result))
                        {
                            ViewState["FCKBasePath"] = result.TrimEnd('/') + "/";
                            return result.TrimEnd('/') + "/";
                        }
                    }

                    throw new HttpException("BasePath for FCKEditor is empty.");
                }

                return ViewState["FCKBasePath"].ToString();
            }
            set
            {
                if (!string.IsNullOrEmpty(value))
                    ViewState["FCKBasePath"] = value.TrimEnd('/') + "/";
            }
        }

        public string FCKToolbar
        {
            get
            {
                if (ViewState["FCKToolbar"] == null || ViewState["FCKToolbar"].ToString().Equals(string.Empty))
                {
                    return "Mini";
                }

                return ViewState["FCKToolbar"].ToString();
            }
            set
            {
                if (!string.IsNullOrEmpty(value))
                    ViewState["FCKToolbar"] = value;
            }
        }

        public int FCKHeight
        {
            get
            {
                int result = 0;
                try
                {
                    result = Convert.ToInt32(ViewState["FCKHeight"]);
                }
                catch
                {
                }

                if (result > 0)
                    return result;

                return 250;

            }
            set { ViewState["FCKHeight"] = value; }
        }

        public Unit FCKWidth
        {
            get
            {
                if (ViewState["FCKWidth"] == null || Unit.Parse(ViewState["FCKWidth"].ToString()).IsEmpty)
                {
                    return Unit.Parse("100%");
                }

                return Unit.Parse(ViewState["FCKWidth"].ToString());
            }
            set { ViewState["FCKWidth"] = value.ToString(); }
        }

        public string FCKEditorAreaCss
        {
            get
            {
                if (ViewState["FCKEditorAreaCss"] == null)
                {
                    return string.Empty;
                }

                return ViewState["FCKEditorAreaCss"].ToString();
            }
            set { ViewState["FCKEditorAreaCss"] = value.ToString(); }
        }

        public bool IsShowAddCommentBtn
        {
            get { return isShowAddCommentBtn; }
            set { isShowAddCommentBtn = value; }
        }

        public bool ShowCaption
        {
            get { return showCaption; }
            set { showCaption = value; }
        }

        public string BehaviorID { get; set; }

        public IList<CommentInfo> Items
        {
            get { return items; }
            set { items = value; }
        }

        public Func<string, string> UserProfileUrlResolver { get; set; }

        public string UserPageLinkWithParam
        {
            get { return userPageLinkWithParam; }
            set { userPageLinkWithParam = value; }
        }

        public string InactiveMessage
        {
            get { return inactiveMessage; }
            set { inactiveMessage = value; }
        }

        public string EditCommentToolTip
        {
            get { return editCommentToolTip; }
            set { editCommentToolTip = value; }
        }

        public string ResponseCommentToolTip
        {
            get { return responseCommentToolTip; }
            set { responseCommentToolTip = value; }
        }

        public string RemoveCommentToolTip
        {
            get { return removeCommentToolTip; }
            set { removeCommentToolTip = value; }
        }

        public string PreviewButton
        {
            get { return previewButton; }
            set { previewButton = value; }
        }

        public string SaveButton
        {
            get { return saveButton; }
            set { saveButton = value; }
        }

        public string CancelButton
        {
            get { return cancelButton; }
            set { cancelButton = value; }
        }

        public string HidePrevuewButton
        {
            get { return hidePrevuewButton; }
            set { hidePrevuewButton = value; }
        }

        public string AddCommentLink
        {
            get { return addCommentLink; }
            set { addCommentLink = value; }
        }

        public string CommentsTitle
        {
            get { return commentsTitle; }
            set { commentsTitle = value; }
        }

        public string CommentsCountTitle
        {
            get { return commentsCountTitle; }
            set { commentsCountTitle = value; }
        }

        public string JavaScriptRemoveCommentFunctionName
        {
            get { return javaScriptRemoveCommentFunctionName; }
            set { javaScriptRemoveCommentFunctionName = value; }
        }

        public string JavaScriptPreviewCommentFunctionName
        {
            get { return javaScriptPreviewCommentFunctionName; }
            set { javaScriptPreviewCommentFunctionName = value; }
        }

        public string JavaScriptAddCommentFunctionName
        {
            get { return javaScriptAddCommentFunctionName; }
            set { javaScriptAddCommentFunctionName = value; }
        }

        public string JavaScriptUpdateCommentFunctionName
        {
            get { return javaScriptUpdateCommentFunctionName; }
            set { javaScriptUpdateCommentFunctionName = value; }
        }

        public string JavaScriptLoadBBcodeCommentFunctionName
        {
            get { return javaScriptLoadBBcodeCommentFunctionName; }
            set { javaScriptLoadBBcodeCommentFunctionName = value; }
        }

        public string JavaScriptCallBackAddComment
        {
            get { return javaScriptCallBackAddComment; }
            set { javaScriptCallBackAddComment = value; }
        }

        public string ConfirmRemoveCommentMessage
        {
            get { return confirmRemoveCommentMessage; }
            set { confirmRemoveCommentMessage = value; }
        }

        public string ObjectID
        {
            get { return objectID; }
            set { objectID = value; }
        }

        public int TotalCount { get; set; }

        #endregion

        #region Methods

        internal string RealUserProfileLinkResolver(string userID)
        {
            if (UserProfileUrlResolver != null)
                return UserProfileUrlResolver(userID);
            else
                return UserPageLinkWithParam + "=" + userID;
        }

        public string GetClientScripts(string siteLink, Page Page)
        {
            isClientScriptRegistered = true;

            var sb = new StringBuilder();

            if (!simple)
            {
                sb.Append(@"<script type=""text/javascript"" language=""javascript"" src=""" + FCKBasePath + "fckeditor.js\"></script>");
            }

            string scriptLocation = Page.ClientScript.GetWebResourceUrl(this.GetType(), "ASC.Web.Controls.Comments.js.comments.js");
            sb.Append(@"<script type=""text/javascript"" language=""javascript"" src=""" + scriptLocation + "\"></script>");

            string paramsScript = string.Format(@"
                    CommentsManagerObj.javaScriptAddCommentFunctionName = '{0}';
                    CommentsManagerObj.javaScriptLoadBBcodeCommentFunctionName = '{1}';
                    CommentsManagerObj.javaScriptUpdateCommentFunctionName = '{2}';
                    CommentsManagerObj.javaScriptCallBackAddComment = '{3}';
                    CommentsManagerObj.javaScriptPreviewCommentFunctionName = '{4}';
                    CommentsManagerObj.isSimple = {5};                    
                    CommentsManagerObj._jsObjName = '{6}';
                    CommentsManagerObj.PID = '{7}';                    
                    CommentsManagerObj.isDisableCtrlEnter = {8};
                    CommentsManagerObj.inactiveMessage = '{9}';
                    CommentsManagerObj.EnableAttachmets = {10};
                    CommentsManagerObj.RemoveAttachButton = '{11}';
                    CommentsManagerObj.FckUploadHandlerPath = '{12}';
                    CommentsManagerObj.maxLevel = {13};
                    ",
                                                this.javaScriptAddCommentFunctionName, this.javaScriptLoadBBcodeCommentFunctionName,
                                                this.javaScriptUpdateCommentFunctionName, this.javaScriptCallBackAddComment,
                                                this.javaScriptPreviewCommentFunctionName, this.simple.ToString().ToLower(), this._jsObjName,
                                                this.PID, this.DisableCtrlEnter.ToString().ToLower(), this.inactiveMessage, this.EnableAttachmets.ToString().ToLower(),
                                                this.RemoveAttachButton, string.Format("{0}{1}", siteLink, VirtualPathUtility.ToAbsolute("~/") + "fckuploader.ashx"), this.MaxDepthLevel);


            paramsScript += string.Format(@"
                    CommentsManagerObj.OnEditedCommentJS = '{0}';
                    CommentsManagerObj.OnRemovedCommentJS = '{1}';
                    CommentsManagerObj.OnCanceledCommentJS = '{2}';
                    CommentsManagerObj.FckDomainName = '{3}';",
                                          OnEditedCommentJS, OnRemovedCommentJS, OnCanceledCommentJS, FckDomainName);


            paramsScript +=
                "\njq(document).ready (function(){\n" +
                "if(jq('#comments_Uploader').length>0 && '" + this.HandlerTypeName + "' != '')\n" +
                "{\n" +
                "new AjaxUpload('comments_Uploader', {\n" +
                "action: 'ajaxupload.ashx?type=" + this.HandlerTypeName + "',\n" +
                "onComplete: CommentsManagerObj.UploadCallBack\n" +
                "});\n}\n" +
                "});";

            if (!Simple)
            {
                paramsScript += string.Format(@"function InitEditor(){0}CommentsManagerObj.InitEditor('{1}','{2}','{3}','{4}','{5}');{6}", "{", FCKBasePath, FCKToolbar, FCKHeight, FCKWidth, FCKEditorAreaCss, "}");
                // paramsScript += string.Format(@"jq(document).ready(function(){0}CommentsManagerObj.InitEditor('{1}','{2}','{3}','{4}','{5}');{6});", "{", FCKBasePath, FCKToolbar, FCKHeight, FCKWidth, FCKEditorAreaCss, "}");
            }

            sb.Append("<script type=\"text/javascript\" language=\"javascript\">" + paramsScript + "</script>");


            if (this.Simple && !this.DisableCtrlEnter)
            {
                scriptLocation = Page.ClientScript.GetWebResourceUrl(this.GetType(), "ASC.Web.Controls.Comments.js.onReady.js");
                sb.Append(@"<script type=""text/javascript"" language=""javascript"" src=""" + scriptLocation + "\"></script>");

            }

            return sb.ToString();
        }

        private void RegisterClientScripts()
        {
            if (!Page.ClientScript.IsClientScriptBlockRegistered("commentsScript"))
            {
                if (!simple)
                {
                    var script = @"<script language=""javascript"" src=""" + FCKBasePath + "fckeditor.js\"></script>";
                    Page.Header.Controls.Add(new LiteralControl(script));
                }

                Page.ClientScript.RegisterClientScriptInclude(this.GetType(), "ajaxupload_script", VirtualPathUtility.ToAbsolute("~/js/ajaxupload.3.5.js"));

                var scriptLocation = Page.ClientScript.GetWebResourceUrl(this.GetType(), "ASC.Web.Controls.Comments.js.comments.js");
                Page.ClientScript.RegisterClientScriptInclude("commentsScript", scriptLocation);

                var paramsScript = string.Format(@"
                    CommentsManagerObj.javaScriptAddCommentFunctionName = '{0}';
                    CommentsManagerObj.javaScriptLoadBBcodeCommentFunctionName = '{1}';
                    CommentsManagerObj.javaScriptUpdateCommentFunctionName = '{2}';
                    CommentsManagerObj.javaScriptCallBackAddComment = '{3}';
                    CommentsManagerObj.javaScriptPreviewCommentFunctionName = '{4}';
                    CommentsManagerObj.isSimple = {5};                    
                    CommentsManagerObj._jsObjName = '{6}';
                    CommentsManagerObj.PID = '{7}';                    
                    CommentsManagerObj.isDisableCtrlEnter = {8};
                    CommentsManagerObj.inactiveMessage = '{9}';
                    CommentsManagerObj.EnableAttachmets = {10};
                    CommentsManagerObj.RemoveAttachButton = '{11}';
                    CommentsManagerObj.FckUploadHandlerPath = '{12}';
                    CommentsManagerObj.maxLevel = {13};
                    ",
                                                 this.javaScriptAddCommentFunctionName, this.javaScriptLoadBBcodeCommentFunctionName,
                                                 this.javaScriptUpdateCommentFunctionName, this.javaScriptCallBackAddComment,
                                                 this.javaScriptPreviewCommentFunctionName, this.simple.ToString().ToLower(), this._jsObjName,
                                                 this.PID, this.DisableCtrlEnter.ToString().ToLower(), this.inactiveMessage, this.EnableAttachmets.ToString().ToLower(),
                                                 this.RemoveAttachButton, string.Format("{0}://{1}:{2}{3}", Page.Request.GetUrlRewriter().Scheme, Page.Request.GetUrlRewriter().Host, Page.Request.GetUrlRewriter().Port, VirtualPathUtility.ToAbsolute("~/") + "fckuploader.ashx"), this.MaxDepthLevel);


                paramsScript += string.Format(@"
                    CommentsManagerObj.OnEditedCommentJS = '{0}';
                    CommentsManagerObj.OnRemovedCommentJS = '{1}';
                    CommentsManagerObj.OnCanceledCommentJS = '{2}';
                    CommentsManagerObj.FckDomainName = '{3}';",
                                              OnEditedCommentJS, OnRemovedCommentJS, OnCanceledCommentJS, FckDomainName);


                paramsScript +=
                    "\njq(document).ready (function(){\n" +
                    "if(jq('#comments_Uploader').length>0 && '" + this.HandlerTypeName + "' != '')\n" +
                    "{\n" +
                    "new AjaxUpload('comments_Uploader', {\n" +
                    "action: 'ajaxupload.ashx?type=" + this.HandlerTypeName + "',\n" +
                    "onComplete: CommentsManagerObj.UploadCallBack\n" +
                    "});\n}\n" +
                    "});";

                if (!Simple)
                {
                    paramsScript += string.Format(@"
                    jq(document).ready (function()
                    {0}
                        CommentsManagerObj.InitEditor('{1}', '{2}', '{3}', '{4}', '{5}');
                    {6});", "{", FCKBasePath, FCKToolbar, FCKHeight, FCKWidth, FCKEditorAreaCss, "}");
                }

                Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "paramsCommentsScript", paramsScript, true);


                if (this.Simple && !this.DisableCtrlEnter)
                {
                    scriptLocation = Page.ClientScript.GetWebResourceUrl(this.GetType(), "ASC.Web.Controls.Comments.js.onReady.js");
                    Page.ClientScript.RegisterClientScriptInclude("onReadyScript", scriptLocation);
                }
            }
        }

        #endregion

        #region Events

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            simple = Core.Mobile.MobileDetector.IsRequestMatchesMobile(this.Context);

            if (Visible)
            {
                RegisterClientScripts();
                isClientScriptRegistered = true;
            }

        }



        protected override void RenderContents(HtmlTextWriter writer)
        {
            if (!isClientScriptRegistered)
                RegisterClientScripts();

            var sb = new StringBuilder();

            int visibleCommentsCount;
            visibleCommentsCount = TotalCount;

            var isEmpty = CommentsHelper.IsEmptyComments(items);

            if (showCaption)
            {
                sb.Append("<div id='commentsTitle' style=\"margin-left:5px; padding-bottom:16px;" + (isEmpty ? "display:none;" : "") + "\" class=\"headerPanel\" >" + commentsTitle + "</div>");
            }
            sb.Append("<a name=\"comments\"></a>");

            sb.Append("<div id=\"mainContainer\" style='width:100%;" + (visibleCommentsCount%2 == 0 ? "border-bottom:1px solid #ddd;" : "") + "width:720px;word-wrap: break-word;" + (isEmpty ? "display:none;" : "") + "'>");
            sb.Append(RenderComments() + "</div>");

            sb.Append("<br />");
            sb.Append("<br />");

            if (isShowAddCommentBtn)
            {
                sb.Append("<a id=\"add_comment_btn\" class=\"baseLinkButton promoAction\" onclick=\"javascript:CommentsManagerObj.AddNewComment();\">" + addCommentLink + "</a>");
            }

            sb.Append("<div id=\"commentBox\" style=\"margin-top: 5px; display:none;\">");
            sb.Append("<input type=\"hidden\" id=\"hdnParentComment\" value=\"\" />");
            sb.Append("<input type=\"hidden\" id=\"hdnAction\" value=\"\" />");
            sb.Append("<input type=\"hidden\" id=\"hdnCommentID\" value=\"\" />");
            sb.Append("<input type=\"hidden\" id=\"hdnObjectID\" value=\"" + objectID + "\" />");
            sb.AppendFormat("<input type='hidden' id='EmptyCommentErrorMessage' value='{0}' />", ASC.Web.Controls.Resources.CommentsResource.EmptyCommentErrorMessage);
            sb.AppendFormat("<input type='hidden' id='CancelNonEmptyCommentErrorMessage' value='{0}' />", ASC.Web.Controls.Resources.CommentsResource.CancelNonEmptyCommentErrorMessage);

            sb.Append("<a name='add_comment'></a>");
            sb.Append("<div id=\"CommentsFckEditorPlaceHolder_" + _jsObjName + "\">");

            if (Simple)
                sb.Append("<textarea id='simpleTextArea' name='simpleTextArea' style='width: 100%; height:124px;'></textarea>");

            sb.Append("</div>");
            sb.Append("<div id=\"comment_attachments\" style=\"padding:5px;\">");
            sb.Append("</div>");
            sb.Append("<input id=\"hdn_comment_attachments\" type=\"hidden\" value=\"\" />");
            sb.Append("<div id='comments_btns' style=\"margin-top:10px;height:20px;\" >");
            sb.Append("<a href=\"javascript:;\"  id=\"btnAddComment\" class=\"baseLinkButton promoAction\" onclick=\"javascript:CommentsManagerObj.AddComment_Click();return false;\" style=\"margin-right:8px;\">" + saveButton + "</a>");

            if (EnableAttachmets)
            {
                sb.Append("<a href=\"javascript:void(0);\" id=\"comments_Uploader\" class=\"baseLinkButton promoAction\" style=\"margin-right:8px;\">" + AttachButton + "</a>");
            }

            sb.AppendFormat("<a href='javascript:;' id='btnPreview' class='baseLinkButton promoAction' onclick='javascript:CommentsManagerObj.Preview_Click();return false;' style='margin-right:8px;'>{0}</a>", previewButton);
            sb.AppendFormat("<a href='javascript:void(0);' id='btnCancel' class='grayLinkButton cancelFckEditorChangesButtonMarker' name='{1}' onclick='CommentsManagerObj.Cancel();' />{0}</a>", cancelButton, "CommentsFckEditor_" + this._jsObjName);


            sb.Append("</div>");

            sb.Append("<div class='clearFix' id='comments_loader' style=\"margin-top:10px;display:none;\" >");
            sb.AppendFormat("<div class='textMediumDescribe'>{0}</div><img src='{1}'/>", this.CommentSendingMsg, this.LoaderImage);
            sb.Append("</div>");


            sb.Append("<div id=\"previewBox\" style=\"display: none; margin-top: 20px;\">");
            sb.Append("<div class='headerPanel' style=\"margin-top: 0px;\">" + this.previewButton + "</div>");
            sb.Append("<div id=\"previewBoxBody\"></div>");
            sb.Append("<br/><a href=\"javascript:void(0);\"  onclick=\"CommentsManagerObj.HidePreview(); return false;\" class=\"baseLinkButton\" style=\"margin-right:8px;\">" + hidePrevuewButton + "</a>");
            sb.Append("</div>");

            sb.Append("</div>");

            writer.Write(sb.ToString());
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            this.InitializeImages();
        }

        #endregion

        #region Methods

        private void InitializeImages()
        {
            responseImageFile = Page.ClientScript.GetWebResourceUrl(this.GetType(), "ASC.Web.Controls.Comments.Images.comment_response.png");
            editImageFile = Page.ClientScript.GetWebResourceUrl(this.GetType(), "ASC.Web.Controls.Comments.Images.comment_edit.png");
            removeImageFile = Page.ClientScript.GetWebResourceUrl(this.GetType(), "ASC.Web.Controls.Comments.Images.comment_delete.png");
            newImageFile = Page.ClientScript.GetWebResourceUrl(this.GetType(), "ASC.Web.Controls.Comments.Images.new.png");
            commentsImageFile = Page.ClientScript.GetWebResourceUrl(this.GetType(), "ASC.Web.Controls.Comments.Images.comments_small.png");
            //LoaderImage = Page.ClientScript.GetWebResourceUrl(this.GetType(), "ASC.Web.Controls.Comments.Images.progress_loader.gif");

        }

        private string RenderComments()
        {
            commentIndex = 1;

            return RenderComments(items, userPageLinkWithParam, 1);
        }

        private string RenderComments(IList<CommentInfo> comments, string userPageLinkWithParam, int commentLevel)
        {
            var sb = new StringBuilder();
            if (comments != null && comments.Count > 0)
            {

                foreach (var comment in comments)
                {
                    comment.CommentBody = HtmlUtility.GetFull(comment.CommentBody, ProductId);
                    sb.Append(
                        CommentsHelper.GetOneCommentHtmlWithContainer(
                            this,
                            comment,
                            commentLevel == 1 || commentLevel > maxDepthLevel,
                            cmnts => RenderComments(cmnts, userPageLinkWithParam, commentLevel + 1),
                            ref commentIndex
                            )
                        );
                }
            }
            return sb.ToString();
        }

        #endregion
    }
}