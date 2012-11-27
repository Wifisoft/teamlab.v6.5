using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.IO;
using System.Collections.Generic;
using ASC.Web.Studio.Helpers;
using ASC.Web.Core.Helpers;
using ASC.Data.Storage;
using ASC.Web.Studio.Utility;
using ASC.PhotoManager.Data;
using System.Drawing;

namespace ASC.Web.Community.PhotoManager
{
    public partial class upload : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {            
            try
            {
                // Get the data
                HttpPostedFile jpeg_image_upload = Request.Files["Filedata"];
                IDataStore store = StorageFactory.GetStorage(TenantProvider.CurrentTenantID.ToString(), "photo");
                var storage = ASC.PhotoManager.Model.StorageFactory.GetStorage();
                   
                string uid = Request["uid"];
                string eventID = Request["eventID"];
                
                bool clearSession = false;

                Album currentAlbum = null;
                
                var albums = storage.GetAlbums(Convert.ToInt64(eventID), uid);
                    clearSession = true;

                    currentAlbum = 0 < albums.Count ? albums[0] : null;

                    if (currentAlbum == null)
                    {
                        Event Event = storage.GetEvent(Convert.ToInt64(eventID));

                        currentAlbum = new Album();
                        currentAlbum.Event = Event;
                        currentAlbum.UserID = uid;

                        storage.SaveAlbum(currentAlbum);
                    }

                    if (Session["photo_albumid"] != null)
                    {
                        if (currentAlbum.Id != (long)Session["photo_albumid"])
                            clearSession = true;
                        Session["photo_albumid"] = currentAlbum.Id;
                    }
                    else
                        clearSession = true;

                string fileNamePath = Resources.PhotoManagerResource.ImagesPath + uid + "/" + currentAlbum.Id + "/";

                ImageInfo currentImageInfo = new ImageInfo();
                string[] listFiles;

                if (Session["photo_listFiles"] != null && !clearSession)
                    listFiles = (string[])Session["photo_listFiles"];
                else
                {
                    listFiles = store.ListFilesRelative("", fileNamePath, "*.*", false);
                    Session["photo_listFiles"] = listFiles;
                }

                string fileExtension = GetFileExtension(jpeg_image_upload.FileName);
                string fileNameWithOutExtension = GetFileName(jpeg_image_upload.FileName);
                string addSuffix = string.Empty;

                
                //if file already exists
                int i = 1;

                while (CheckFile(listFiles, fileNameWithOutExtension + addSuffix + Constants.THUMB_SUFFIX + fileExtension))
                {
                    addSuffix = "(" + i.ToString() + ")";
                    i++;
                }

                string fileNameThumb = fileNamePath + fileNameWithOutExtension + addSuffix + Constants.THUMB_SUFFIX + "." + Constants.jpeg_extension;
                string fileNamePreview = fileNamePath + fileNameWithOutExtension + addSuffix + Constants.PREVIEW_SUFFIX + "." + Constants.jpeg_extension;
                                

                currentImageInfo.Name = fileNameWithOutExtension;
                currentImageInfo.PreviewPath = fileNamePreview;
                currentImageInfo.ThumbnailPath = fileNameThumb;


                Stream fs = jpeg_image_upload.InputStream;
                
                try
                {
                    EXIFReader reader = new EXIFReader(fs);
                    currentImageInfo.ActionDate = (string)reader[PropertyTagId.DateTime];
                }
                catch { }

                ImageHelper.GenerateThumbnail(fs, fileNameThumb, ref currentImageInfo, store);
                ImageHelper.GeneratePreview(fs, fileNamePreview, ref currentImageInfo, store);

                fs.Dispose();

                AlbumItem image = new AlbumItem(currentAlbum);
                image.Name = currentImageInfo.Name;
                image.Timestamp = ASC.Core.Tenants.TenantUtil.DateTimeNow();
                image.UserID = uid;

                image.Location = currentImageInfo.Name;
                

                image.PreviewSize = new Size(currentImageInfo.PreviewWidth, currentImageInfo.PreviewHeight);
                image.ThumbnailSize = new Size(currentImageInfo.ThumbnailWidth, currentImageInfo.ThumbnailHeight);

                storage.SaveAlbumItem(image);

                currentAlbum.FaceItem = image;
                storage.SaveAlbum(currentAlbum);

                string response = image.Id.ToString();

                byte[] byteArray = System.Text.Encoding.UTF8.GetBytes(response);
                string encodingResponse = Convert.ToBase64String(byteArray);

                
                Response.StatusCode = 200;
                Response.Write(encodingResponse);
            }
            catch
            {
                // If any kind of error occurs return a 500 Internal Server error
                Response.StatusCode = 500;
                Response.Write("An error occured");
                Response.End();
            }
            finally
            {
                Response.End();
            }
        }
        private string GetFileName(string fileName)
        {
            return fileName.Substring(0, fileName.LastIndexOf('.'));
        }

        private bool CheckFile(string[] listFiles, string fileName)
        {
            foreach (var file in listFiles)
            {
                if (fileName == file.ToString())
                    return true;
            }
            return false;
        }

        private string GetFileExtension(string fileName)
        {
            if (fileName == null)
            {
                return fileName;
            }

            return fileName.Substring(fileName.LastIndexOf('.'), fileName.Length - fileName.LastIndexOf('.'));

        }

        private string GetFileNameThumbnail(string fileName)
        {
            return fileName.Insert(fileName.LastIndexOf('.'), Constants.THUMB_SUFFIX);
        }

        private string GetNextFileName(string fileName, int nextValue)
        {
            int position = fileName.LastIndexOf('(');
            if (position < 0)
            {
                position = fileName.Length;
            }
            return fileName.Substring(0, position) + "(" + nextValue + ")";
        }

    }
}
