using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;
using ASC.Core;
using ASC.PhotoManager;
using ASC.PhotoManager.Data;
using ASC.PhotoManager.Helpers;
using ASC.PhotoManager.Resources;
using ASC.Web.Core.Users;
using ASC.Web.Core.Utility.Skins;
using ASC.Web.Studio.Controls.Common;
using ASC.Web.Studio.Utility;
using ASC.Web.Community.Product;

namespace ASC.Web.Community.PhotoManager
{
    public partial class LastCommented : BasePage
    {
        #region Members

        private int _selectedPage;
        private int _CountPhotoPerPage = 3;
        //private ASC.Web.Controls.BBCodeParser.Parser postParser = new ASC.Web.Controls.BBCodeParser.Parser(CommonControlsConfigurer.CoreConfig);

        #endregion

        #region Methods

        protected override void PageLoad()
        {

            mainContainer.BreadCrumbs.Add(new Web.Controls.BreadCrumb {Caption = PhotoManagerResource.PhotoTitle, NavigationUrl = PhotoConst.PAGE_DEFAULT});
            mainContainer.BreadCrumbs.Add(new Web.Controls.BreadCrumb {Caption = PhotoManagerResource.LastCommentedTitle});

            Title = HeaderStringHelper.GetPageTitle(PhotoManagerResource.AddonName, mainContainer.BreadCrumbs);

            InitPageParams();

            if (!IsPostBack)
                LoadData();
        }

        private void LoadData()
        {
            var storage = StorageFactory.GetStorage();
            var list = storage.GetAlbumItemsLastCommented((_selectedPage - 1)*_CountPhotoPerPage, _CountPhotoPerPage);
            var count = (int) storage.GetAlbumItemsLastCommentedCount();

            if (list.Count == 0)
            {
                var emptyScreenControl = new EmptyScreenControl
                                             {
                                                 ImgSrc = WebImageSupplier.GetAbsoluteWebPath("150x_photo_icon.png", PhotoConst.ModuleID),
                                                 Header = PhotoManagerResource.EmptyScreenLatestCommentsCaption,
                                                 Describe = PhotoManagerResource.EmptyScreenText,
                                                 ButtonHTML = CommunitySecurity.CheckPermissions(PhotoConst.Action_AddPhoto) ? String.Format("<a class='linkAddMediumText' href='" + PhotoConst.PAGE_ADD_PHOTO + "'>{0}</a>", PhotoManagerResource.EmptyScreenLatestCommentsLink) : String.Empty
                                             };

                _contentHolder.Controls.Add(emptyScreenControl);
            }
            else
            {
                var pageNavigator = new Web.Controls.PageNavigator
                                        {
                                            PageUrl = PhotoConst.PAGE_LAST_COMMENTED + "?t=&",
                                            CurrentPageNumber = _selectedPage,
                                            EntryCountOnPage = _CountPhotoPerPage,
                                            VisiblePageCount = 5,
                                            ParamName = "page",
                                            EntryCount = count
                                        };

                pageNavigatorHolder.Controls.Add(pageNavigator);

                RenderImagesList(list);
            }
        }

        private void RenderImagesList(IList<AlbumItem> list)
        {
            var sb = new StringBuilder();
            var storage = StorageFactory.GetStorage();
            var i = 0;
            var store = Data.Storage.StorageFactory.GetStorage(TenantProvider.CurrentTenantID.ToString(), "photo");

            sb.Append("<table cellpadding=\"3\" cellspacing=\"0\" border=\"0\" class=\"borderBase\" width='100%' style=\"border-right:none;border-left:none;border-bottom:none;\">");

            foreach (var image in list)
            {
                var limit = ImageHTMLHelper.GetImageSizeLimit(image, 75);
                //var comments = storage.GetComments(image.Id);
                var count = image.CommentsCount;
                var countViews = image.ViewsCount;


                sb.Append("<tr " + (i%2 == 0 ? "class='tintMedium'" : "") + " valign='top'><td class=\"borderBase\" style=\"border-top:none; border-right:none; border-left:none; width:105px;\"><div style='padding:15px 10px;'>");
                sb.Append("<a href=\"" + PhotoConst.PAGE_PHOTO_DETAILS + "?" + PhotoConst.PARAM_PHOTO + "=" + image.Id + "\"><img style='border: 0px solid;' " + limit + "  title=\"" + HttpUtility.HtmlEncode(image.Name) + "\" src=\"" + ImageHTMLHelper.GetImageUrl(image.ExpandedStoreThumb, store) + "\" /></a></div></td>");
                sb.Append("<td class=\"borderBase\" style=\"border-top:none; border-right:none; border-left:none;\"><div style='padding-top:10px;padding-bottom:10px;width: 620px;word-wrap: break-word;overflow: hidden;'>");
                sb.Append("<a class='linkHeaderLight' href=\"" + PhotoConst.PAGE_PHOTO_DETAILS + "?" + PhotoConst.PARAM_PHOTO + "=" + image.Id + "\">" + HttpUtility.HtmlEncode(image.Name) + "</a></span>");
                sb.Append("<div style='margin-top:5px;'><span class='textMediumDescribe'>" + PhotoManagerResource.PostedToTitle + ":&nbsp;&nbsp;<a class='linkDescribe' href=\"" + PhotoConst.PAGE_DEFAULT + "?" + PhotoConst.PARAM_EVENT + "=" + image.Album.Event.Id + "\">" + HttpUtility.HtmlEncode(image.Album.Event.Name) + "</a><span class='textMediumDescribe'>,&nbsp;</span>");
                sb.Append("<a class='linkDescribe' href=\"" + PhotoConst.PAGE_PHOTO + "?" + PhotoConst.PARAM_ALBUM + "=" + image.Album.Id + "\">" + DisplayUserSettings.GetFullUserName(new Guid(image.UserID)) + "</a></div>");
                sb.Append("<div style='margin-top:15px;' class=\"textBigDescribe\">" + Grammatical.ViewsCount(countViews) + " <span class='splitter'>|</span>" + Grammatical.CommentsCount(count) + "</div>");

                var comments = storage.GetLatestCommentsByImage(image.Id, 5);

                foreach (var comment in comments)
                {
                    //Comment comment = comments[j];
                    sb.Append("<div style='padding:30px 0px 0px;'>");
                    sb.Append("<table cellpadding='0' cellspacing='0' border='0'>");
                    sb.Append("<tr><td valign='top' width='34' rowspan='2'>");
                    sb.Append(ImageHTMLHelper.GetHTMLImgUserAvatar(new Guid(comment.UserID)));
                    sb.Append("</td>");
                    sb.Append("<td>");
                    sb.Append("<div style='margin-left:15px;'><a class='linkHeaderSmall' href='" + CommonLinkUtility.GetUserProfile(new Guid(comment.UserID), Product.CommunityProduct.ID) + "'>" + DisplayUserSettings.GetFullUserName(new Guid(comment.UserID)) + "</a>");
                    sb.Append("<span class='textMediumDescribe' style='padding-left:10px;'>" + PhotoManagerResource.SaidTitle + ":</span></div>");
                    sb.Append("<div class='textMediumDescribe' style='margin:5px 0px 0px 15px;'>" + comment.Timestamp.AgoSentence() + "</div>");
                    sb.Append("</td>");
                    sb.Append("</tr>");

                    sb.Append("<tr>");
                    sb.Append("<td>");
                    sb.Append("<div style='margin:15px 0px 0px 15px;'>" + comment.Text + "</div>");
                    sb.Append("</td>");
                    sb.Append("</tr>");

                    sb.Append("</table>");
                    sb.Append("</div>");
                }
                i++;
                sb.Append("<div style='float:right; padding:0px 20px 10px'><a href='" + PhotoConst.PAGE_PHOTO_DETAILS + "?" + PhotoConst.PARAM_PHOTO + "=" + image.Id + "'>" + PhotoManagerResource.AddCommentLink + "</a></div>");
                sb.Append("</td></tr>");
            }

            sb.Append("</table>");

            _contentHolder.Controls.Add(new Literal {Text = sb.ToString()});
        }

        private void InitPageParams()
        {
            if (Request.QueryString[PhotoConst.PARAM_PAGE] == null || Request.QueryString[PhotoConst.PARAM_PAGE] == string.Empty)
                _selectedPage = 1;
            else
            {
                try
                {
                    _selectedPage = Convert.ToInt32(Request.QueryString[PhotoConst.PARAM_PAGE]);
                }
                catch
                {
                    _selectedPage = 1;
                }
            }

            if (_selectedPage <= 0)
                _selectedPage = 1;
        }

        #endregion
    }
}