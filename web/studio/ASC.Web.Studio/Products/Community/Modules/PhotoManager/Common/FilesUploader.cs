using System;
using System.Drawing;
using System.Linq;
using System.Web;
using ASC.PhotoManager;
using ASC.PhotoManager.Data;
using ASC.Web.Controls;
using ASC.Web.Controls.FileUploader.HttpModule;
using ASC.Web.Core;
using ASC.Web.Core.Helpers;
using ASC.Web.Studio.Helpers;
using ASC.Web.Studio.Utility;
using StorageFactory = ASC.PhotoManager.Data.StorageFactory;

namespace ASC.Web.Community.PhotoManager.Common
{
    public class FilesUploader : FileUploadHandler
    {
        public override FileUploadResult ProcessUpload(HttpContext context)
        {
            if (!ASC.Core.SecurityContext.AuthenticateMe(CookiesManager.GetCookies(CookiesType.AuthKey)))
            {
                return new FileUploadResult
                           {
                               Success = false,
                               Message = "Permission denied"
                           };
            }

            var result = "";

            try
            {
                if (ProgressFileUploader.HasFilesToUpload(context))
                {
                    var postedFile = new ProgressFileUploader.FileToUpload(context);
                    var fileName = postedFile.FileName;
                    var inputStream = postedFile.InputStream;


                    var store = Data.Storage.StorageFactory.GetStorage(TenantProvider.CurrentTenantID.ToString(), "photo");
                    var storage = StorageFactory.GetStorage();

                    var uid = context.Request["uid"];
                    var eventID = context.Request["eventID"];

                    var albums = storage.GetAlbums(Convert.ToInt64(eventID), uid);

                    var currentAlbum = 0 < albums.Count ? albums[0] : null;

                    if (currentAlbum == null)
                    {
                        var Event = storage.GetEvent(Convert.ToInt64(eventID));

                        currentAlbum = new Album
                                           {
                                               Event = Event,
                                               UserID = uid
                                           };

                        storage.SaveAlbum(currentAlbum);
                    }

                    if (context.Session["photo_albumid"] != null)
                    {
                        context.Session["photo_albumid"] = currentAlbum.Id;
                    }

                    var fileNamePath = PhotoConst.ImagesPath + uid + "/" + currentAlbum.Id + "/";

                    var currentImageInfo = new ImageInfo();

                    var listFiles = store.ListFilesRelative("", fileNamePath, "*.*", false);
                    context.Session["photo_listFiles"] = listFiles;

                    var fileExtension = FileUtility.GetFileExtension(fileName);
                    var fileNameWithOutExtension = GetFileName(fileName);
                    var addSuffix = string.Empty;

                    //if file already exists
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

                    var fs = inputStream;

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

                    var response = image.Id.ToString();

                    var byteArray = System.Text.Encoding.UTF8.GetBytes(response);
                    result = Convert.ToBase64String(byteArray);
                }

            }
            catch (Exception ex)
            {
                return new FileUploadResult
                           {
                               Success = false,
                               Message = ex.Message,
                           };
            }

            return new FileUploadResult
                       {
                           Success = true,
                           Data = "",
                           Message = result
                       };
        }

        #region Private methods

        public override string GetFileName(string fileName)
        {
            return FileUtility.GetFileName(fileName);
        }

        private bool CheckFile(string[] listFiles, string fileName)
        {
            return listFiles.Any(file => fileName == file);
        }

        #endregion
    }
}