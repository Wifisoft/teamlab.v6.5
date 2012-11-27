#region Import

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;
using System.Web;
using System.Linq;
using ASC.Collections;
using ASC.Common.Threading.Workers;
using ASC.Data.Storage;
using ASC.Web.CRM.Configuration;
using ASC.Web.Controls;
using ASC.Web.Core.Utility;
using ASC.Web.Core.Utility.Skins;
using ASC.Web.Studio.Core;
using ASC.Web.Studio.Utility;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;

#endregion

namespace ASC.Web.CRM.Classes
{

    public class ResizeWorkerItem
    {
        public int ContactID { get; set; }

        public Size[] RequireFotoSize { get; set; }

        public byte[] ImageData { get; set; }

        public IDataStore DataStore { get; set; }

        public override bool Equals(object obj)
        {
            if (!(obj is ResizeWorkerItem)) return false;

            var item = (ResizeWorkerItem) obj;

            return item.ContactID.Equals(ContactID) && RequireFotoSize.Equals(item.RequireFotoSize) && ImageData.Length == item.ImageData.Length;

        }

        public override int GetHashCode()
        {
            return ContactID ^ RequireFotoSize.GetHashCode() ^ ImageData.Length;
        }
    }

    public class ContactPhotoHandler : IFileUploadHandler
    {
        public FileUploadResult ProcessUpload(HttpContext context)
        {
            var fileUploadResult = new FileUploadResult();

            if (!ProgressFileUploader.HasFilesToUpload(context)) return fileUploadResult;

            var file = new ProgressFileUploader.FileToUpload(context);

            if (String.IsNullOrEmpty(file.FileName) || file.ContentLength == 0)
                throw new InvalidOperationException("Invalid file.");

            if (0 < SetupInfo.MaxImageUploadSize && SetupInfo.MaxImageUploadSize < file.ContentLength)
                throw FileSizeComment.FileImageSizeException;

            if (FileUtility.GetFileTypeByFileName(file.FileName) != FileType.Image)
                throw new UnknownImageFormatException();

            var photoUri = ContactPhotoManager.UploadPhoto(file.InputStream, Convert.ToInt32(context.Request["contactID"]));

            fileUploadResult.Success = true;
            fileUploadResult.Data = photoUri;

            return fileUploadResult;
        }
    }

    public static class ContactPhotoManager
    {
        #region Members

        private static readonly SynchronizedDictionary<int, IDictionary<Size, string>> _photoCache = new SynchronizedDictionary<int, IDictionary<Size, string>>();

        private static readonly WorkerQueue<ResizeWorkerItem> ResizeQueue = new WorkerQueue<ResizeWorkerItem>(2, TimeSpan.FromSeconds(30), 1, true);

        private static readonly Size _bigSize = new Size(145, 145);
        private static readonly Size _mediumSize = new Size(82, 82);
        private static readonly Size _smallSize = new Size(40, 40);

        private static readonly Object _synchronizedObj = new Object();

        #endregion

        #region Private Methods

        private static String FromCache(int contactID, Size photoSize)
        {
            if (_photoCache.ContainsKey(contactID))
                if (_photoCache[contactID].ContainsKey(photoSize))
                    return _photoCache[contactID][photoSize];

            return String.Empty;
        }

        private static void RemoveFromCache(int contactID)
        {
            lock (_synchronizedObj)
            {
                _photoCache.Remove(contactID);
            }
        }

        private static void ToCache(int contactID, String photoUri, Size photoSize)
        {
            lock (_synchronizedObj)
            {

                if (_photoCache.ContainsKey(contactID))
                    if (_photoCache[contactID].ContainsKey(photoSize))
                        _photoCache[contactID][photoSize] = photoUri;
                    else
                        _photoCache[contactID].Add(photoSize, photoUri);
                else
                    _photoCache.Add(contactID, new Dictionary<Size, string> {{photoSize, photoUri}});
            }
        }

        private static String FromDataStore(int contactID, Size photoSize)
        {

            var directoryPath = BuildFileDirectory(contactID);

            if (!Global.GetStore().IsDirectory(directoryPath))
                return String.Empty;

            var filesURI = Global.GetStore().ListFiles(directoryPath, BuildFileName(contactID, photoSize) + "*", false);

            if (filesURI.Length == 0) return String.Empty;

            return filesURI[0].ToString();

        }

        private static String GetPhotoUri(int contactID, bool isCompany, Size photoSize)
        {
            var photoUri = FromCache(contactID, photoSize);

            if (!String.IsNullOrEmpty(photoUri)) return photoUri;

            photoUri = FromDataStore(contactID, photoSize);

            if (String.IsNullOrEmpty(photoUri))
                photoUri = GetDefaultPhoto(isCompany, photoSize);

            ToCache(contactID, photoUri, photoSize);

            return photoUri;
        }

        private static String BuildFileDirectory(int contactID)
        {
            if (contactID == 0)
                throw new ArgumentException();

            var s = contactID.ToString("000000");

            return String.Concat("photos/", s.Substring(0, 2), "/",
                                 s.Substring(2, 2), "/",
                                 s.Substring(4), "/");
        }

        private static String BuildFileName(int contactID, Size photoSize)
        {
            return String.Format("contact_{0}_{1}_{2}", contactID, photoSize.Width, photoSize.Height);
        }

        private static String BuildFilePath(int contactID, Size photoSize, String imageExtension)
        {

            var s = contactID.ToString("000000");

            if (photoSize.IsEmpty || contactID == 0)
                throw new ArgumentException();

            return String.Concat(BuildFileDirectory(contactID), BuildFileName(contactID, photoSize), imageExtension);

        }

        private static byte[] ToByteArray(Stream inputStream)
        {

            var data = new byte[inputStream.Length];

            var br = new BinaryReader(inputStream);
            br.Read(data, 0, (int) inputStream.Length);
            br.Close();

            return data;
        }

        private static string GetImgFormatName(ImageFormat format)
        {

            if (format.Equals(ImageFormat.Bmp)) return "bmp";
            if (format.Equals(ImageFormat.Emf)) return "emf";
            if (format.Equals(ImageFormat.Exif)) return "exif";
            if (format.Equals(ImageFormat.Gif)) return "gif";
            if (format.Equals(ImageFormat.Icon)) return "icon";
            if (format.Equals(ImageFormat.Jpeg)) return "jpeg";
            if (format.Equals(ImageFormat.MemoryBmp)) return "MemoryBMP";
            if (format.Equals(ImageFormat.Png)) return "png";
            if (format.Equals(ImageFormat.Tiff)) return "tiff";
            if (format.Equals(ImageFormat.Wmf)) return "wmf";

            return "jpg";
        }

        private static ImageCodecInfo GetCodecInfo(ImageFormat format)
        {
            var mimeType = string.Format("image/{0}", GetImgFormatName(format));
            if (mimeType == "image/jpg") mimeType = "image/jpeg";
            var encoders = ImageCodecInfo.GetImageEncoders();
            foreach (var e in
                encoders.Where(e => e.MimeType.Equals(mimeType, StringComparison.InvariantCultureIgnoreCase)))
            {
                return e;
            }
            return 0 < encoders.Length ? encoders[0] : null;
        }

        private static byte[] SaveToBytes(Image img)
        {
            return SaveToBytes(img, img.RawFormat);
        }

        private static byte[] SaveToBytes(Image img, ImageFormat source)
        {
            byte[] data = new byte[0];
            using (var memoryStream = new MemoryStream())
            {
                var encParams = new EncoderParameters(1);
                encParams.Param[0] = new EncoderParameter(Encoder.Quality, (long) 100);
                img.Save(memoryStream, GetCodecInfo(source), encParams);
                data = memoryStream.ToArray();
            }
            return data;
        }


        private static void ExecResizeImage(ResizeWorkerItem resizeWorkerItem)
        {
            foreach (var fotoSize in resizeWorkerItem.RequireFotoSize)
            {


                var data = resizeWorkerItem.ImageData;
                using (var stream = new MemoryStream(data))
                using (var img = new Bitmap(stream))
                {
                    var imgFormat = img.RawFormat;
                    if (fotoSize != img.Size)
                    {
                        using (var img2 = DoThumbnail(img, fotoSize))
                        {
                            data = SaveToBytes(img2, imgFormat);
                        }
                    }
                    else
                    {
                        data = SaveToBytes(img);
                    }

                    var fileExtension = String.Concat("." + GetImgFormatName(imgFormat));

                    var photoPath = BuildFilePath(resizeWorkerItem.ContactID, fotoSize, fileExtension);

                    using (var fileStream = new MemoryStream(data))
                    {
                        var photoUri = resizeWorkerItem.DataStore.Save(photoPath, fileStream).ToString();

                        photoUri = String.Format("{0}?cd={1}", photoUri, DateTime.UtcNow.Ticks);

                        ToCache(resizeWorkerItem.ContactID, photoUri, fotoSize);
                    }
                }

            }

        }

        private static String GetDefaultPhoto(bool isCompany, Size photoSize)
        {
            if (isCompany)
                return WebImageSupplier.GetAbsoluteWebPath(String.Format("empty_company_logo_{0}_{1}.png", photoSize.Height, photoSize.Width), ProductEntryPoint.ID);

            return WebImageSupplier.GetAbsoluteWebPath(String.Format("empty_people_logo_{0}_{1}.png", photoSize.Height, photoSize.Width), ProductEntryPoint.ID);

        }

        #endregion

        #region Public Methods

        public static void DeletePhoto(int contactID)
        {

            if (contactID == 0)
                throw new ArgumentException();

            lock (_synchronizedObj)
            {

                ResizeQueue.GetItems().Where(item => item.ContactID == contactID)
                    .All(item =>
                             {
                                 ResizeQueue.Remove(item);
                                 return true;
                             });

                var photoDirectory = BuildFileDirectory(contactID);

                if (Global.GetStore().IsDirectory(photoDirectory))
                    Global.GetStore().DeleteFiles(photoDirectory, "*", true);

                RemoveFromCache(contactID);
            }

        }

        public static String GetSmallSizePhoto(int contactID, bool isCompany)
        {

            if (contactID == 0)
                return GetDefaultPhoto(isCompany, _smallSize);

            return GetPhotoUri(contactID, isCompany, _smallSize);
        }

        public static String GetMediumSizePhoto(int contactID, bool isCompany)
        {
            if (contactID == 0)
                return GetDefaultPhoto(isCompany, _mediumSize);

            return GetPhotoUri(contactID, isCompany, _mediumSize);
        }

        private static Image DoThumbnail(Image image, Size size)
        {
            var width = size.Width;
            var height = size.Height;
            var realWidth = image.Width;
            var realHeight = image.Height;

            var maxSide = realWidth > realHeight ? realWidth : realHeight;

            var alignWidth = (maxSide == realWidth);

            double scaleFactor = (alignWidth) ? (realWidth/(1.0*width)) : (realHeight/(1.0*height));

            if (scaleFactor < 1) scaleFactor = 1;


            int finalWidth, finalHeigth;

            finalWidth = (int) (realWidth/scaleFactor);
            finalHeigth = (int) (realHeight/scaleFactor);

            var thumbnail = new Bitmap(finalWidth, finalHeigth);

            using (var graphic = Graphics.FromImage(thumbnail))
            {
                graphic.SmoothingMode = SmoothingMode.AntiAlias;
                graphic.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphic.PixelOffsetMode = PixelOffsetMode.HighQuality;
                graphic.DrawImage(image, 0, 0, finalWidth, finalHeigth);
            }

            return thumbnail;

        }


        public static String GetBigSizePhoto(int contactID, bool isCompany)
        {
            if (contactID == 0)
                return GetDefaultPhoto(isCompany, _bigSize);

            return GetPhotoUri(contactID, isCompany, _bigSize);
        }

        private static String ResizeToBigSize(byte[] imageData, int contactID)
        {

            var resizeWorkerItem = new ResizeWorkerItem
                                       {
                                           ContactID = contactID,
                                           RequireFotoSize = new[] {_bigSize},
                                           ImageData = imageData,
                                           DataStore = Global.GetStore()
                                       };

            ExecResizeImage(resizeWorkerItem);

            return FromCache(contactID, _bigSize);

        }

        private static void ExecGenerateThumbnail(byte[] imageData, int contactID)
        {

            var resizeWorkerItem = new ResizeWorkerItem
                                       {
                                           ContactID = contactID,
                                           RequireFotoSize = new[] {_mediumSize, _smallSize},
                                           ImageData = imageData,
                                           DataStore = Global.GetStore()
                                       };

            if (!ResizeQueue.GetItems().Contains(resizeWorkerItem))
                ResizeQueue.Add(resizeWorkerItem);

            if (!ResizeQueue.IsStarted) ResizeQueue.Start(ExecResizeImage);
        }

        public static String UploadPhoto(Stream inputStream, int contactID)
        {

            var imageData = ToByteArray(inputStream);

            return UploadPhoto(imageData, contactID);
        }

        private static byte[] ToByteArray(Stream inputStream, int streamLength)
        {
            using (var br = new BinaryReader(inputStream))
            {
                return br.ReadBytes(streamLength);
            }
        }

        public static String UploadPhoto(String imageUrl, int contactID)
        {

            var request = (HttpWebRequest) WebRequest.Create(imageUrl);

            using (var response = request.GetResponse())
            {
                using (var inputStream = response.GetResponseStream())
                {

                    var imageData = ToByteArray(inputStream, (int) response.ContentLength);

                    return UploadPhoto(imageData, contactID);
                }
            }
        }

        public static String UploadPhoto(byte[] imageData, int contactID)
        {
            if (contactID == 0)
                throw new ArgumentException();

            DeletePhoto(contactID);

            ExecGenerateThumbnail(imageData, contactID);

            return ResizeToBigSize(imageData, contactID);

        }

        #endregion
    }

    public class UnknownImageFormatException : Exception
    {
        public UnknownImageFormatException()
            : base("unknown image file type")
        {
        }

        public UnknownImageFormatException(Exception inner)
            : base("unknown image file type", inner)
        {
        }
    }

    public class ImageWeightLimitException : Exception
    {
        public ImageWeightLimitException()
            : base("image with is too large")
        {
        }
    }

    public class ImageSizeLimitException : Exception
    {
        public ImageSizeLimitException()
            : base("image size is too large")
        {
        }
    }
}