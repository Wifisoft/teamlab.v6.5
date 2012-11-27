using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;
using ASC.Core;
using ASC.Core.Users;
using ASC.PhotoManager;
using ASC.PhotoManager.Data;
using ASC.PhotoManager.Helpers;
using ASC.PhotoManager.Model;
using ASC.PhotoManager.Resources;
using ASC.Web.Community.Product;
using ASC.Web.Core.Users;
using ASC.Web.Core.Utility.Skins;
using ASC.Web.Studio.Controls.Common;
using ASC.Web.Studio.Core;
using ASC.Web.Studio.Utility;

namespace ASC.Web.Community.PhotoManager
{
    public partial class Photo : BasePage
    {
        #region Members

        private Album album;
        private int _selectedPage;
        private int countAlbumItems;

        private IImageStorage service;

        #endregion

        #region Properties

        #endregion

        #region Methods

        public string Mode
        {
            get { return Request.QueryString[PhotoConst.ALBUM_MODE] ?? PhotoConst.ALBUM_MODE_VIEW; }
        }

        protected override void PageLoad()
        {
            lbtnEdit.CssClass = "linkAction" + (SetupInfo.WorkMode == WorkMode.Promo ? " promoAction" : "");

            confirmContainer.Options.IsPopup = true;

            var store = Data.Storage.StorageFactory.GetStorage(TenantProvider.CurrentTenantID.ToString(), "photo");

            var storage = StorageFactory.GetStorage();
            service = storage;

            InitPageParams();

            if (!IsPostBack)
            {
                countAlbumItems = 0;

                if (!String.IsNullOrEmpty(AlbumID))
                {
                    mainContainer.BreadCrumbs.Add(new Web.Controls.BreadCrumb {Caption = PhotoManagerResource.PhotoTitle, NavigationUrl = PhotoConst.PAGE_DEFAULT});
                    pnlCurrentAlbum.Visible = true;
                    pnlUserAlbums.Visible = false;

                    album = storage.GetAlbum(Convert.ToInt64(AlbumID));

                    if (album != null)
                    {
                        var editRemovePermissions = CommunitySecurity.CheckPermissions(album, PhotoConst.Action_EditRemovePhoto);

                        if (album == null)
                            Response.Redirect(PhotoConst.PAGE_DEFAULT);

                        if (editRemovePermissions)
                        {
                            pnlEditPhoto.Visible = true;
                        }

                        ltrLinkAllPhoto.Text = "<a href='" + PhotoConst.PAGE_PHOTO + "?" + PhotoConst.PARAM_USER + "=" + album.UserID + "'>" + PhotoManagerResource.AllAuthorAlbumsTitle + "</a>";

                        var caption = (string.IsNullOrEmpty(album.Caption) ? DisplayUserSettings.GetFullUserName(new Guid(album.UserID), false) : album.Caption);

                        mainContainer.BreadCrumbs.Add(new Web.Controls.BreadCrumb {Caption = album.Event.Name, NavigationUrl = PhotoConst.PAGE_DEFAULT + "?" + PhotoConst.PARAM_EVENT + "=" + album.Event.Id});
                        mainContainer.BreadCrumbs.Add(new Web.Controls.BreadCrumb {Caption = caption});

                        var cssStyle = ImageHTMLHelper.GetImagePreviewSizeLimit(album.FaceItem, 260);
                        if (album.FaceItem != null)
                            ltrAlbumFace.Text = "<a href='" + PhotoConst.PAGE_PHOTO_DETAILS + "?" + PhotoConst.PARAM_PHOTO + "=" + album.FaceItem.Id + "'><img " + cssStyle + " style='border:0px solid #000;margin:8px 0px;' src=\"" + ImageHTMLHelper.GetImageUrl(album.FaceItem.ExpandedStorePreview, store) + "\" /></a>";

                        var countComments = album.CommentsCount;
                        var countViews = album.ViewsCount;
                        LoadAlbumsLinks(album.Event);
                        ltrAlbumInfo.Text = "<div class=\"textMediumDescribe\" style=\"padding: 15px 10px 0px;\"><span class='textBaseSmall'>" + Grammatical.PhotosCount(album.ImagesCount) + "</span><span class='splitter'>|</span>" + Grammatical.ViewsCount(countViews) + "<span class='splitter'>|</span>" + Grammatical.CommentsCount(countComments) + "</div>" +
                                            "<div class=\"textMediumDescribe\" style=\"padding: 5px 10px; max-width:255px; overflow:hidden;\">" + PhotoManagerResource.PostedByTitle + ": " + CoreContext.UserManager.GetUsers(new Guid(album.UserID)).RenderProfileLink(Product.CommunityProduct.ID) + "</div><div class=\"textMediumDescribe\" style=\"padding: 0px 10px;\">" + PhotoManagerResource.LastUpdateTitle + ": " + album.LastUpdate.ToShortDateString() + "</div>";

                        foreach (var albumItem in storage.GetAlbumItems(album))
                        {
                            ltrPhoto.Text += ImageHTMLHelper.GetSmallHTMLImage(albumItem, countAlbumItems, false, 75, store);
                            countAlbumItems++;
                        }
                    }
                    else
                    {
                        pnlCurrentAlbum.Controls.Clear();
                        pnlCurrentAlbum.Controls.Add(new Literal {Text = string.Format("<div class=\"noContentBlock\">{0}</div>", PhotoManagerResource.NoFoundMessage)});
                        albumsContainer.Visible = false;
                    }
                }
                else
                {
                    pnlCurrentAlbum.Visible = false;
                    pnlUserAlbums.Visible = true;
                    albumsContainer.Visible = false;
                    LoadUserAllPhoto(!String.IsNullOrEmpty(UserID) ? UserID : currentUserID.ToString(), store);
                }
            }

            Title = HeaderStringHelper.GetPageTitle(PhotoManagerResource.AddonName, mainContainer.BreadCrumbs);

            sideRecentActivity.TenantId = TenantProvider.CurrentTenantID;
            sideRecentActivity.ProductId = Product.CommunityProduct.ID;
            sideRecentActivity.ModuleId = PhotoConst.ModuleID;
        }

        private void LoadAlbumsLinks(Event Event)
        {
            var sb = new StringBuilder();

            albumsContainer.Title = PhotoManagerResource.OtherAlbums;
            albumsContainer.HeaderCSSClass = "studioSideBoxTagCloudHeader";
            albumsContainer.ImageURL = WebImageSupplier.GetAbsoluteWebPath("photo_albums.png", PhotoConst.ModuleID);

            foreach (var album in service.GetAlbums(Event.Id, null))
            {
                var caption = (string.IsNullOrEmpty(album.Caption) ? DisplayUserSettings.GetFullUserName(new Guid(album.UserID)) : album.Caption);

                sb.Append("<div style=\"margin-top: 10px;padding-left:20px;word-wrap: break-word;\">");
                sb.Append("<a class=\"linkAction\" href=\"" + PhotoConst.PAGE_PHOTO + "?" + PhotoConst.PARAM_ALBUM + "=" + album.Id + "\">" + caption + "</a>");
                sb.Append("</div>");
            }

            ltrAlbums.Text = sb.ToString();
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

        private void LoadUserAllPhoto(string userID, Data.Storage.IDataStore store)
        {
            mainContainer.BreadCrumbs.Add(new Web.Controls.BreadCrumb {Caption = PhotoManagerResource.PhotoTitle, NavigationUrl = PhotoConst.PAGE_DEFAULT});
            mainContainer.BreadCrumbs.Add(new Web.Controls.BreadCrumb {Caption = DisplayUserSettings.GetFullUserName(new Guid(userID), false)});

            var list = StorageFactory.GetStorage().GetAlbums(0, userID);

            if (list == null || list.Count == 0)
            {
                var emptyScreenControl = new EmptyScreenControl
                                             {
                                                 ImgSrc = WebImageSupplier.GetAbsoluteWebPath("150x_photo_icon.png", PhotoConst.ModuleID),
                                                 Describe = PhotoManagerResource.EmptyScreenText
                                             };

                if (userID == SecurityContext.CurrentAccount.ID.ToString())
                {
                    emptyScreenControl.Header = PhotoManagerResource.YouHaveNoPhotosTitle;
                    emptyScreenControl.ButtonHTML = CommunitySecurity.CheckPermissions(PhotoConst.Action_AddPhoto) ? String.Format("<a class='linkAddMediumText' href='" + PhotoConst.PAGE_ADD_PHOTO + "'>{0}</a>", PhotoManagerResource.EmptyScreenLink) : String.Empty;
                }
                else
                {
                    emptyScreenControl.Header = PhotoManagerResource.UserHaveNoPhotosTitle;
                }

                _contentHolder.Controls.Add(emptyScreenControl);
            }
            else
            {
                _contentHolder.Controls.Add(new Literal {Text = ImageHTMLHelper.DrawAlbumsAlone(list, store)});
            }
        }

        #endregion

        #region Events

        protected void lbtnEdit_Click(object sender, EventArgs e)
        {
            var storage = StorageFactory.GetStorage();

            if (!String.IsNullOrEmpty(AlbumID))
            {
                album = storage.GetAlbum(Convert.ToInt64(AlbumID));

                IList<string> selectedItems = new List<string>();

                foreach (var item in storage.GetAlbumItems(album))
                {
                    selectedItems.Add(item.Id.ToString());
                }

                Session[PhotoConst.PARAM_SELECTED_ITEMS] = selectedItems;

                if (selectedItems.Count > 0)
                {
                    Response.Redirect(PhotoConst.PAGE_EDIT_PHOTO + "?" + PhotoConst.PARAM_ALBUM + "=" + Request.QueryString[PhotoConst.PARAM_ALBUM]);
                }
            }
        }

        protected void lbtnRemove_Click(object sender, EventArgs e)
        {
            var storage = StorageFactory.GetStorage();
            var store = Data.Storage.StorageFactory.GetStorage(TenantProvider.CurrentTenantID.ToString(), "photo");

            if (!String.IsNullOrEmpty(AlbumID))
            {
                album = storage.GetAlbum(Convert.ToInt64(AlbumID));

                CommunitySecurity.DemandPermissions(album, PhotoConst.Action_EditRemovePhoto);

                store.DeleteFiles(PhotoConst.ImagesPath + album.UserID + "/" + album.Id + "/", "*", false);
                storage.RemoveAlbum(album.Id);

                Response.Redirect(PhotoConst.PAGE_PHOTO);
            }
        }

        #endregion
    }
}