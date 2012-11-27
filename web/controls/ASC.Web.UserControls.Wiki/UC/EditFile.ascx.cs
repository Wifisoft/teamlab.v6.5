using System;
using System.Web;
using System.Web.UI.WebControls;
using ASC.Core.Tenants;
using ASC.Data.Storage;
using ASC.Web.UserControls.Wiki.Data;
using ASC.Web.UserControls.Wiki.Handlers;
using IO = System.IO;

namespace ASC.Web.UserControls.Wiki.UC
{
    public partial class EditFile : BaseUserControl
    {
        public string FileName
        {
            get
            {
                if (ViewState["FileName"] == null)
                    return string.Empty;
                return ViewState["FileName"].ToString();
            }
            set { ViewState["FileName"] = value; }
        }

        private File _fileInfo;

        protected File CurrentFile
        {
            get
            {
                if (_fileInfo == null)
                {
                    if (string.IsNullOrEmpty(FileName))
                        return null;

                    _fileInfo = Wiki.GetFile(FileName);
                }
                return _fileInfo;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (CurrentFile != null && !string.IsNullOrEmpty(CurrentFile.FileName))
                {
                    RisePublishVersionInfo(CurrentFile);
                }
            }
        }

        protected string GetUploadFileName()
        {
            if (CurrentFile == null)
                return string.Empty;

            return CurrentFile.UploadFileName;
        }

        protected string GetFileLink()
        {
            var file = Wiki.GetFile(FileName);
            if (file == null)
            {
                RisePageEmptyEvent();
                return string.Empty; // "nonefile.png";
            }

            var ext = file.FileLocation.Split('.')[file.FileLocation.Split('.').Length - 1];
            if (!string.IsNullOrEmpty(ext) && !WikiFileHandler.ImageExtentions.Contains(ext.ToLower()))
            {
                return string.Format(@"<a class=""wikiEditButton"" href=""{0}"" title=""{1}"">{2}</a>",
                                     ResolveUrl(string.Format(ImageHandlerUrlFormat, FileName)),
                                     file.FileName,
                                     Resources.WikiUCResource.wikiFileDownloadCaption);
            }

            return string.Format(@"<img src=""{0}"" style=""max-width:300px; max-height:200px"" />",
                                 ResolveUrl(string.Format(ImageHandlerUrlFormat, FileName)));
        }

        private static string GetFileLocation(string fileName, WikiSection section, string rootFile)
        {
            string firstFolder = "0", secondFolder = "00";
            var letter = (byte) fileName[0];

            secondFolder = letter.ToString("x");
            firstFolder = secondFolder.Substring(0, 1);

            var fileLocation = IO.Path.Combine(firstFolder, secondFolder);
            fileLocation = IO.Path.Combine(fileLocation, EncodeSafeName(fileName)); //TODO: encode nameprep here

            return fileLocation;
        }

        private static string EncodeSafeName(string fileName)
        {
            return fileName;
        }

        public static void DeleteTempContent(string fileName, string configLocation, WikiSection section, int tenantId, HttpContext context)
        {
            var storage = StorageFactory.GetStorage(configLocation, tenantId.ToString(), section.DataStorage.ModuleName, context);
            storage.Delete(section.DataStorage.TempDomain, fileName);
        }

        public static void DeleteContent(string fileName, string configLocation, WikiSection section, int tenantId, HttpContext context)
        {
            var storage = StorageFactory.GetStorage(configLocation, tenantId.ToString(), section.DataStorage.ModuleName, context);
            storage.Delete(section.DataStorage.DefaultDomain, fileName);
        }

        public static SaveResult MoveContentFromTemp(Guid UserId, string fromFileName, string toFileName, string configLocation, WikiSection section, int tenantId, HttpContext context, string rootFile, out string _fileName)
        {
            var storage = StorageFactory.GetStorage(configLocation, tenantId.ToString(), section.DataStorage.ModuleName, context);

            var fileName = toFileName;
            var fileLocation = GetFileLocation(fileName, section, rootFile);
            var file = new File
                           {
                               FileName = fileName,
                               UploadFileName = fileName,
                               UserID = UserId,
                               FileLocation = fileLocation,
                               FileSize = (int) storage.GetFileSize(section.DataStorage.TempDomain, fromFileName),
                           };

            var wiki = new WikiEngine();
            wiki.SaveFile(file);

            storage.Move(section.DataStorage.TempDomain, fromFileName, section.DataStorage.DefaultDomain, fileLocation);
            _fileName = file.FileName;

            return SaveResult.Ok;
        }

        public static SaveResult DirectFileSave(Guid UserId, string _fileName, byte[] _fileContent, string rootFile, WikiSection section, string configLocation, int tenantId, HttpContext context, out string outFileName)
        {
            outFileName = string.Empty;
            //var fileName = _fileName;
            //var fileLocation = GetFileLocation(fileName, section, rootFile);
            //var file = new File
            //               {

            //                   FileName = fileName,
            //                   UploadFileName = fileName,
            //                   UserID = UserId,
            //                   FileLocation = fileLocation,
            //                   FileSize = _fileContent.Length
            //               };

            var wikiEngine = new WikiEngine();
            //wikiEngine.SaveFile(file);
            var file = wikiEngine.CreateOrUpdateFile(new File { FileName = _fileName, FileSize = _fileContent.Length });

            try
            {
                FileContentSave(file.FileLocation/*fileLocation*/, _fileContent, section, configLocation, tenantId, context);
            }
            catch (TenantQuotaException)
            {
                wikiEngine.RemoveFile(file.FileName);
                return SaveResult.FileSizeExceeded;
            }

            outFileName = file.FileName;

            return SaveResult.Ok;
        }

        public static SaveResult DirectFileSave(Guid UserId, FileUpload fuFile, string rootFile, WikiSection section, string configLocation, int tenantId, HttpContext context)
        {
            if (!fuFile.HasFile)
                return SaveResult.FileEmpty;

            //var fileName = fuFile.FileName;
            //var fileLocation = GetFileLocation(fileName, section, rootFile);
            //var file = new File
            //               {
            //                   FileName = fileName,
            //                   UploadFileName = fileName,
            //                   UserID = UserId,
            //                   FileLocation = fileLocation,
            //                   FileSize = fuFile.FileBytes.Length,
            //               };

            var wikiEngine = new WikiEngine();
            //wikiEngine.SaveFile(file);
            var file = wikiEngine.CreateOrUpdateFile(new File {FileName = fuFile.FileName, FileSize = fuFile.FileBytes.Length});
            try
            {
                FileContentSave(file.FileLocation/*fileLocation*/, fuFile.FileBytes, section, configLocation, tenantId, context);
            }
            catch (TenantQuotaException)
            {
                wikiEngine.RemoveFile(file.FileName);
                return SaveResult.FileSizeExceeded;
            }

            return SaveResult.Ok;
        }

        private static void FileContentSave(string location, byte[] fileContent, WikiSection section, string configLocation, int tenantId, HttpContext context)
        {
            var storage = StorageFactory.GetStorage(configLocation, tenantId.ToString(), section.DataStorage.ModuleName, context);
            FileContentSave(storage, location, fileContent, section);
        }

        private static void FileContentSave(string location, byte[] fileContent, WikiSection section, int tenantId)
        {
            var storage = StorageFactory.GetStorage(tenantId.ToString(), section.DataStorage.ModuleName);
            FileContentSave(storage, location, fileContent, section);
        }

        private static void FileContentSave(IDataStore storage, string location, byte[] fileContent, WikiSection section)
        {
            using (var ms = new IO.MemoryStream(fileContent))
            {
                storage.Save(section.DataStorage.DefaultDomain, location, ms);
            }
        }

        public SaveResult Save(Guid userId)
        {
            string fileName;
            return Save(userId, out fileName);
        }

        public SaveResult Save(Guid userId, out string fileName)
        {
            fileName = string.Empty;
            if (!fuFile.HasFile)
                return SaveResult.FileEmpty;

            var file = CurrentFile ?? new File {FileName = fuFile.FileName, UploadFileName = fuFile.FileName};
            //var fileLocation = GetFileLocation(file.FileName, WikiSection.Section, Page.MapPath("~"));
            //file.UserID = userId;
            //file.FileLocation = fileLocation;
            file.FileSize = fuFile.FileBytes.Length;
            //Wiki.SaveFile(file);
            file = Wiki.CreateOrUpdateFile(file);

            try
            {
                FileContentSave(file.FileLocation/*fileLocation*/, fuFile.FileBytes, WikiSection.Section, TenantId);
            }
            catch (TenantQuotaException)
            {
                Wiki.RemoveFile(file.FileName);
                return SaveResult.FileSizeExceeded;
            }

            _fileInfo = file;

            RisePublishVersionInfo(file);
            fileName = file.FileName;

            return SaveResult.Ok;
        }
    }
}