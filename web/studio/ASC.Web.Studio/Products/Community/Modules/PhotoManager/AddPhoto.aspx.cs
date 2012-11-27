using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Web;
using AjaxPro;
using ASC.Core;
using ASC.PhotoManager;
using ASC.PhotoManager.Data;
using ASC.PhotoManager.Helpers;
using ASC.PhotoManager.Resources;
using ASC.Web.Community.Product;
using ASC.Web.Core.Helpers;
using ASC.Web.Studio.Controls.Common;
using ASC.Web.Studio.Core;
using ASC.Web.Studio.Helpers;
using ASC.Web.Studio.Utility;

namespace ASC.Web.Community.PhotoManager
{
    [AjaxNamespace("AddPhoto")]
    public partial class AddPhoto : BasePage
    {
        private bool editable = true;
        protected Album selectedAlbum;
        protected long requestedEvent = -1;

        #region Methods

        protected override void PageLoad()
        {
            if (SetupInfo.WorkMode == WorkMode.Promo)
                Response.Redirect(PhotoConst.PAGE_DEFAULT, true);

            if (!CommunitySecurity.CheckPermissions(PhotoConst.Action_AddPhoto))
                Response.Redirect(PhotoConst.PAGE_DEFAULT);

            if (Core.Mobile.MobileDetector.IsRequestMatchesMobile(Context))
                Response.Redirect(PhotoConst.PAGE_DEFAULT);

            formContainer.Options.IsPopup = true;

            mainContainer.BreadCrumbs.Add(new Web.Controls.BreadCrumb {Caption = PhotoManagerResource.PhotoTitle, NavigationUrl = PhotoConst.PAGE_DEFAULT});
            mainContainer.BreadCrumbs.Add(new Web.Controls.BreadCrumb {Caption = PhotoManagerResource.UploadPhotoTitle});

            Title = HeaderStringHelper.GetPageTitle(PhotoManagerResource.AddonName, mainContainer.BreadCrumbs);

            Utility.RegisterTypeForAjax(typeof (AddPhoto), Page);
            btnUpload.Text = PhotoManagerResource.FinishUploadLink;
            btnSave.Text = PhotoManagerResource.SavePhotosButton;

            if (!IsPostBack)
            {
                Session["imagesInfo"] = new List<ImageInfo>();
            }
            if (!string.IsNullOrEmpty(Request.QueryString[PhotoConst.PARAM_EVENT]))
            {
                try
                {
                    requestedEvent = Convert.ToInt64(Request.QueryString[PhotoConst.PARAM_EVENT]);
                }
                catch
                {
                    requestedEvent = -1;
                }
            }

            GetRequestParams();

            sideRecentActivity.TenantId = TenantProvider.CurrentTenantID;
            sideRecentActivity.ProductId = Product.CommunityProduct.ID;
            sideRecentActivity.ModuleId = PhotoConst.ModuleID;

            _uploadSwitchHolder.Controls.Add(new FileUploaderModeSwitcher());
        }

        private void GetRequestParams()
        {
            var selectedID = Request.QueryString[PhotoConst.PARAM_ALBUM];

            long id;
            if (!string.IsNullOrEmpty(selectedID) && long.TryParse(selectedID, out id))
            {
                var storage = StorageFactory.GetStorage();
                selectedAlbum = storage.GetAlbum(id);
                editable = false;
            }

            if (selectedAlbum == null)
                if (!Int64.TryParse(Request[PhotoConst.PARAM_EVENT], out requestedEvent))
                    requestedEvent = -1;
        }

        public string RenderEventsSelector()
        {
            var sb = new StringBuilder();
            var storage = StorageFactory.GetStorage();
            var events = storage.GetEvents(0, int.MaxValue);

            long selectedEvent = -1;

            if (requestedEvent != -1) selectedEvent = requestedEvent;

            try
            {
                if (selectedAlbum != null)
                    selectedEvent = selectedAlbum.Event.Id;

            }
            catch
            {
            }

            sb.Append("<select " + (editable ? string.Empty : "disabled=\"true\" ") + " id=\"events_selector\" name=\"events_selector\" onchange=\"javascript:PhotoManager.EventsSelectorHandle();\" class=\"comboBox\" style=\"width: 100%;\">");
            sb.Append("<option class=\"textMediumDescribe\" value=\"-1\"  " + (selectedEvent == (long) (-1) ? "selected" : string.Empty) + ">" + PhotoManagerResource.ChooseEventTitle + "</option>");

            foreach (var item in events)
            {
                sb.Append("<option " + (selectedEvent == item.Id ? "selected" : string.Empty) + " value=\"" + item.Id.ToString() + "\" >" + HttpUtility.HtmlEncode(item.Name) + "</option>");
            }

            sb.Append("</select>");

            return sb.ToString();
        }

        private string AddPreviewImage(string fileName, string imageName, long imageID, bool isAlbumFace, int imageNumber, int width, int height)
        {
            var sb = new StringBuilder();

            sb.Append("<span><input name=\"image_id_" + imageNumber + "\" name=\"image_id_" + imageNumber + "\" type=\"hidden\" value=\"" + imageID + "\" />");
            sb.Append("<div style=\"width: 200px;height:200px;background-color: #EBF0F4;padding:5px;\">" + ImageHTMLHelper.GetHTMLThumb(fileName, 200, width, height) + "</div><div></div>");
            sb.Append("<div class='textBigDescribe clearFix' style=\"padding-left:5px;padding-top:5px;width: 200px;\"><div style='padding-top: 5px;float:left'>" + PhotoManagerResource.EditPhotoNameTitle + "</div><div style='float:right'><label class='textMediumDescribe' for=\"face_" + imageNumber + "\">" + PhotoManagerResource.AlbumCoverTitle + "</label><input maxlength=\"255\" type=\"radio\" id=\"face_" + imageNumber + "\" name=\"album_face\" value=\"" + imageID + "\" " + (isAlbumFace ? "checked" : "") + " /></div></div><div><input  class=\"textEdit\" style=\"width:200px;margin-top:5px;\" name=\"" + ASC.PhotoManager.PhotoConst.PARAM_EDIT_NAME + imageNumber + "\" id=\"" + ASC.PhotoManager.PhotoConst.PARAM_EDIT_NAME + imageNumber + "\" value='" + imageName + "' type=\"text\"/></div>");
            sb.Append("</span>");

            return sb.ToString();
        }

        private List<string> CreateImagesInfo(string encodingString)
        {
            var info = new List<string>();

            foreach (var response in encodingString.Split('|'))
            {
                var byteArray = Convert.FromBase64String(response);
                var convertedResponse = Encoding.UTF8.GetString(byteArray);

                info.Add(convertedResponse);
            }

            return info;
        }

        private List<string> CreateImagesInfoBySimple()
        {
            var info = new List<string>();
            var store = Data.Storage.StorageFactory.GetStorage(TenantProvider.CurrentTenantID.ToString(), "photo");
            var storage = StorageFactory.GetStorage();

            var uid = SecurityContext.CurrentAccount.ID.ToString();
            var eventID = Request["events_selector"];

            var albums = storage.GetAlbums(Convert.ToInt64(eventID), uid);

            var currentAlbum = 0 < albums.Count ? albums[0] : null;

            if (currentAlbum == null)
            {
                var Event = storage.GetEvent(Convert.ToInt64(eventID));

                currentAlbum = new Album {Event = Event, UserID = uid};

                storage.SaveAlbum(currentAlbum);
            }
            var fileNamePath = PhotoConst.ImagesPath + uid + "/" + currentAlbum.Id + "/";

            var listFiles = store.ListFilesRelative("", fileNamePath, "*.*", false);

            for (var j = 0; j < Request.Files.Count; j++)
            {
                var file = Request.Files[j];

                if (file.ContentLength > SetupInfo.MaxUploadSize)
                    continue;

                if (string.IsNullOrEmpty(file.FileName))
                    continue;

                var currentImageInfo = new ImageInfo();

                var fileExtension = FileUtility.GetFileExtension(file.FileName);
                var fileNameWithOutExtension = FileUtility.GetFileName(file.FileName);
                var addSuffix = string.Empty;

                var i = 1;

                while (CheckFile(listFiles, fileNameWithOutExtension + addSuffix + PhotoConst.THUMB_SUFFIX + fileExtension))
                {
                    addSuffix = "(" + i.ToString() + ")";
                    i++;
                }

                var fileNameThumb = fileNamePath + fileNameWithOutExtension + addSuffix + PhotoConst.THUMB_SUFFIX + "." + PhotoConst.jpeg_extension;
                var fileNamePreview = fileNamePath + fileNameWithOutExtension + addSuffix + PhotoConst.PREVIEW_SUFFIX + "." + PhotoConst.jpeg_extension;


                currentImageInfo.Name = fileNameWithOutExtension;
                currentImageInfo.PreviewPath = fileNamePreview;
                currentImageInfo.ThumbnailPath = fileNameThumb;
                var fs = file.InputStream;

                try
                {
                    var reader = new EXIFReader(fs);
                    currentImageInfo.ActionDate = (string) reader[PropertyTagId.DateTime];
                }
                catch
                {
                }

                ImageHelper.GenerateThumbnail(fs, fileNameThumb, ref currentImageInfo, store);
                ImageHelper.GeneratePreview(fs, fileNamePreview, ref currentImageInfo, store);

                fs.Dispose();

                var image = new AlbumItem(currentAlbum)
                                {
                                    Name = currentImageInfo.Name,
                                    Timestamp = ASC.Core.Tenants.TenantUtil.DateTimeNow(),
                                    UserID = uid,
                                    Location = currentImageInfo.Name,
                                    PreviewSize = new Size(currentImageInfo.PreviewWidth, currentImageInfo.PreviewHeight),
                                    ThumbnailSize = new Size(currentImageInfo.ThumbnailWidth, currentImageInfo.ThumbnailHeight)
                                };

                storage.SaveAlbumItem(image);

                currentAlbum.FaceItem = image;

                storage.SaveAlbum(currentAlbum);

                info.Add(image.Id.ToString());
            }

            return info;
        }

        private bool CheckFile(string[] listFiles, string fileName)
        {
            return listFiles.Any(file => fileName == file);
        }

        private void SavePhotoItem(int i)
        {
            var storage = StorageFactory.GetStorage();
            var image = storage.GetAlbumItem(Convert.ToInt64(Request.Form["image_id_" + i]));

            if (selectedAlbum == null)
                selectedAlbum = image.Album;

            image.Name = GetLimitedText(Request.Form[PhotoConst.PARAM_EDIT_NAME + i]);

            storage.SaveAlbumItem(image);
        }

        #endregion

        #region AJAX events

        [AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public string SafeSession()
        {
            return "";
        }

        [AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public string CreateEvent(string name, string description, string dateTime)
        {
            CommunitySecurity.DemandPermissions(PhotoConst.Action_AddEvent);

            var storage = StorageFactory.GetStorage();

            DateTime date;
            DateTime.TryParse(dateTime, out date);

            var item = new Event
                           {
                               Name = GetLimitedText(name),
                               Description = GetLimitedText(description),
                               Timestamp = date,
                               UserID = SecurityContext.CurrentAccount.ID.ToString()
                           };

            storage.SaveEvent(item);

            return "<option value=\"" + item.Id.ToString() + "\" onclick=\"javascript:PhotoManager.LoadEvent(" + item.Id.ToString() + ");\">" + HttpUtility.HtmlEncode(item.Name) + "</option>";
        }

        #endregion

        #region Events

        protected void btnUpload_Click(object sender, EventArgs e)
        {
            var photoCount = 0;
            var simpleUploader = false;

            var sb = new StringBuilder();
            IList<AlbumItem> images = new List<AlbumItem>();
            var storage = StorageFactory.GetStorage();

            try
            {
                var eventID = Convert.ToInt64(Request.Form["events_selector"]);
                var authorID = SecurityContext.CurrentAccount.ID.ToString();

                Album currentAlbum = null;

                if (selectedAlbum != null)
                {
                    currentAlbum = selectedAlbum;
                }
                else if (string.IsNullOrEmpty(authorID))
                    return;
                else if (authorID != "0")
                {
                    var albums = storage.GetAlbums(eventID, authorID);
                    currentAlbum = 0 < albums.Count ? albums[0] : null;
                }

                if (currentAlbum == null)
                {
                    var Event = storage.GetEvent(eventID);

                    currentAlbum = new Album();
                    currentAlbum.Event = Event;
                    currentAlbum.UserID = SecurityContext.CurrentAccount.ID.ToString();

                    storage.SaveAlbum(currentAlbum);
                }

                var imagesInfo = !simpleUploader ? CreateImagesInfo(Request.Form["phtm_imagesInfo"]) : CreateImagesInfoBySimple();

                if (imagesInfo != null)
                {
                    var i = 0;

                    var store = Data.Storage.StorageFactory.GetStorage(TenantProvider.CurrentTenantID.ToString(), "photo");

                    foreach (var info in imagesInfo)
                    {
                        var item = storage.GetAlbumItem(Convert.ToInt64(info));
                        images.Add(item);

                        if (photoCount != 0 && photoCount%3 == 0)
                            sb.Append("</div>");

                        if (photoCount%3 == 0)
                            sb.Append("<div class=\"borderLight tintMediumLight clearFix\" style=\"padding:20px; border-left:none;border-right:none;margin-bottom:8px;\">");


                        sb.Append("<div style='float:left;margin-bottom:5px;" + (photoCount%3 == 0 ? "" : "margin-left:22px; ") + "'>");

                        sb.Append(AddPreviewImage(ImageHTMLHelper.GetImageUrl(item.ExpandedStorePreview, store), item.Name, item.Id,
                                                  i == 0, i, item.PreviewSize.Width, item.PreviewSize.Height));

                        sb.Append("</div>");

                        i++;
                        photoCount++;
                    }
                }

                sb.Append("</div>");

                ltrUploadedImages.Text = sb.ToString();
                pnlImageForm.Visible = false;
                pnlSave.Visible = true;

                storage.SaveAlbum(currentAlbum, images);
            }
            catch (Exception)
            {
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            var i = 0;

            while (!string.IsNullOrEmpty(Request.Form["image_id_" + i]))
            {
                SavePhotoItem(i);
                i++;
            }

            var storage = StorageFactory.GetStorage();

            long albumItemId;
            if (!string.IsNullOrEmpty(Request.Form["album_face"]) && long.TryParse(Request.Form["album_face"], out albumItemId))
            {
                var item = storage.GetAlbumItem(albumItemId);
                selectedAlbum.FaceItem = item;
                storage.SaveAlbum(selectedAlbum);
            }

            Response.Redirect(PhotoConst.PAGE_PHOTO + "?" + PhotoConst.PARAM_ALBUM + "=" + selectedAlbum.Id, true);
        }

        #endregion
    }
}