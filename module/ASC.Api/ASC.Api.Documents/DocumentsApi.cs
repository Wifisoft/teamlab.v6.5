using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Configuration;
using ASC.Api.Attributes;
using ASC.Api.Impl;
using ASC.Api.Utils;
using ASC.Common.Data;
using ASC.Web.Files.Classes;
using ASC.Web.Files.Services.WCFService;
using ASC.Files.Core;
using ASC.Api.Collections;
using SortedByType = ASC.Files.Core.SortedByType;
using ASC.Web.Files.Api;
using ASC.CRM.Core;

namespace ASC.Api.Documents
{
    /// <summary>
    /// Provides acces to documents
    /// </summary>
    public class DocumentsApi : Interfaces.IApiEntryPoint
    {
        private readonly ApiContext _context;
        private readonly IFileStorageService _fileStorageService;

        /// <summary>
        /// </summary>
        public string Name
        {
            get { return "files"; }
        }

        /// <summary>
        /// </summary>
        /// <param name="context"></param>
        /// <param name="fileStorageService"></param>
        public DocumentsApi(ApiContext context, IFileStorageService fileStorageService)
        {
            _context = context;
            _fileStorageService = fileStorageService;
            //TODO: Why not to move this code to DAO???
            if (!DbRegistry.IsDatabaseRegistered(FileConstant.DatabaseId))
            {
                DbRegistry.RegisterDatabase(FileConstant.DatabaseId,
                                            WebConfigurationManager.ConnectionStrings[FileConstant.DatabaseId]);
            }

            if (!FilesIntegration.IsRegisteredFileSecurityProvider("crm", "crm_common"))
                FilesIntegration.RegisterFileSecurityProvider("crm", "crm_common", new FileSecurityProvider());

        }



        /// <summary>
        /// Returns the detailed list of files and folders located in the current user 'My Documents' section
        /// </summary>
        /// <short>
        /// My folder
        /// </short>
        /// <category>Folders</category>
        /// <returns>My folder contents</returns>
        [Read("@my")]
        public FolderContentWrapper GetMyFolder()
        {
            return GetFolderContents(Global.FolderMy);
        }

        /// <summary>
        /// Returns the detailed list of files and folders located in the 'Common Documents' section
        /// </summary>
        /// <short>
        /// Common folder
        /// </short>
        /// <returns>Common folder contents</returns>
        [Read("@common")]
        public FolderContentWrapper GetCommonFolder()
        {
            return GetFolderContents(Global.FolderCommon);
        }

        /// <summary>
        /// Returns the detailed list of files and folders located in the 'Shared with Me' section
        /// </summary>
        /// <short>
        /// Shared folder
        /// </short>
        /// <returns>Shared folder contents</returns>
        [Read("@share")]
        public FolderContentWrapper GetShareFolder()
        {
            return GetFolderContents(Global.FolderShare);
        }

        /// <summary>
        /// Returns the detailed list of files and folders located in the 'Recycle Bin' section
        /// </summary>
        /// <short>
        /// Trash folder
        /// </short>
        /// <category>Folders</category>
        /// <returns>Trash folder contents</returns>
        [Read("@trash")]
        public FolderContentWrapper GetTrashFolder()
        {
            return GetFolderContents(Global.FolderTrash);
        }

        /// <summary>
        /// Returns the detailed list of files and folders located in the folder with the ID specified in the request
        /// </summary>
        /// <short>
        /// Folder by ID
        /// </short>
        /// <category>Folders</category>
        /// <param name="folderid">ID of folder</param>
        /// <returns>Folder contents</returns>
        [Read("{folderid}")]
        public FolderContentWrapper GetFolder(string folderid)
        {
            return GetFolderContents(folderid).NotFoundIfNull();
        }

        /// <summary>
        /// Uploads the file specified with single file upload or standart multipart/form-data method to 'My Documents' section
        /// </summary>
        /// <short>Upload to My</short>
        /// <category>Uploads</category>
        /// <remarks>
        /// <![CDATA[
        ///  Upload can be done in 2 different ways:
        ///  <ol>
        /// <li>Single file upload. You should set Content-Type &amp; Content-Disposition header to specify filename and content type, and send file in request body</li>
        /// <li>Using standart multipart/form-data method</li>
        /// </ol>]]>
        /// </remarks>
        /// <param name="file" visible="false">Request Input stream</param>
        /// <param name="contentType" visible="false">Content-Type Header</param>
        /// <param name="contentDisposition" visible="false">Content-Disposition Header</param>
        /// <param name="files" visible="false">List of files when posted as multipart/form-data</param>
        /// <returns>Uploaded file</returns>
        [Create("@my/upload")]
        public object UploadFileToMy(Stream file, ContentType contentType, ContentDisposition contentDisposition, IEnumerable<System.Web.HttpPostedFileBase> files)
        {
            return UploadFile(Global.FolderMy.ToString(), file, contentType, contentDisposition, files, false);
        }

        /// <summary>
        /// Uploads the file specified with single file upload or standart multipart/form-data method to 'Common Documents' section
        /// </summary>
        /// <short>Upload to Common</short>
        /// <category>Uploads</category>
        /// <remarks>
        /// <![CDATA[
        ///  Upload can be done in 2 different ways:
        ///  <ol>
        /// <li>Single file upload. You should set Content-Type &amp; Content-Disposition header to specify filename and content type, and send file in request body</li>
        /// <li>Using standart multipart/form-data method</li>
        /// </ol>]]>
        /// </remarks>
        /// <param name="file" visible="false">Request Input stream</param>
        /// <param name="contentType" visible="false">Content-Type Header</param>
        /// <param name="contentDisposition" visible="false">Content-Disposition Header</param>
        /// <param name="files" visible="false">List of files when posted as multipart/form-data</param>
        /// <returns>Uploaded file</returns>
        [Create("@common/upload")]
        public object UploadFileToCommon(Stream file, ContentType contentType, ContentDisposition contentDisposition, IEnumerable<System.Web.HttpPostedFileBase> files)
        {
            return UploadFile(Global.FolderCommon.ToString(), file, contentType, contentDisposition, files, false);
        }

        
        /// <summary>
        /// Uploads the file specified with single file upload or standart multipart/form-data method to the selected folder
        /// </summary>
        /// <short>Upload to folder</short>
        /// <category>Uploads</category>
        /// <remarks>
        /// <![CDATA[
        ///  Upload can be done in 2 different ways:
        ///  <ol>
        /// <li>Single file upload. You should set Content-Type &amp; Content-Disposition header to specify filename and content type, and send file in request body</li>
        /// <li>Using standart multipart/form-data method</li>
        /// </ol>]]>
        /// </remarks>
        /// <param name="folderid">Folder ID to upload to</param>
        /// <param name="file" visible="false">Request Input stream</param>
        /// <param name="contentType" visible="false">Content-Type Header</param>
        /// <param name="contentDisposition" visible="false">Content-Disposition Header</param>
        /// <param name="files" visible="false">List of files when posted as multipart/form-data</param>
        /// <param name="createNewIfExist" visible="false">Create New If Exist</param>
        /// <returns>Uploaded file</returns>
        [Create("{folderid}/upload")]
        public object UploadFile(string folderid, Stream file, ContentType contentType, ContentDisposition contentDisposition, IEnumerable<System.Web.HttpPostedFileBase> files, bool createNewIfExist)
        {
            if (files != null && files.Any())
            {
                if (files.Count() == 1)
                {
                    //Only one file. return it
                    var postedFile = files.First();
                    return SaveFile(folderid, postedFile.InputStream, new ContentType(postedFile.ContentType), new ContentDisposition { FileName = postedFile.FileName, Size = postedFile.ContentLength }, createNewIfExist);
                }
                //For case with multiple files
                return files.Select(postedFile =>
                                    SaveFile(folderid, postedFile.InputStream, new ContentType(postedFile.ContentType), new ContentDisposition { FileName = postedFile.FileName, Size = postedFile.ContentLength }, createNewIfExist)).ToList();
            }
            if (file != null)
            {
                return SaveFile(folderid, file, contentType, contentDisposition, createNewIfExist);
            }
            throw new InvalidOperationException("No input files");
        }

        private FileWrapper SaveFile(string folderid, Stream file, ContentType contentType, ContentDisposition contentDisposition, bool createNewIfExist)
        {
            if (contentType == null && contentDisposition != null)
            {
                //Get from extension
                contentType = new ContentType(Common.Web.MimeMapping.GetMimeMapping(contentDisposition.FileName));
            }
            if (contentDisposition == null && contentType != null)
            {
                contentDisposition = new ContentDisposition {FileName = "file" + Common.Web.MimeMapping.GetExtention(contentType.MediaType)};
            }

            contentDisposition.ThrowIfNull(new ArgumentException("content-disposition is not set"));
            contentType.ThrowIfNull(new ArgumentException("content-type is not set"));

            var resultFile = DocumentUtils.UploadFile(folderid, contentDisposition.FileName, file.Length, contentType.MediaType, file, createNewIfExist);
            return new FileWrapper(resultFile);
        }

        /// <summary>
        /// Creates a text (.txt) file in 'My Documents' section with the title and contents sent in the request
        /// </summary>
        /// <short>Create txt in 'My'</short>
        /// <category>File Creation</category>
        /// <param name="title">File title</param>
        /// <param name="content">File contents</param>
        /// <returns>Folder contents</returns>
        [Create("@my/text")]
        public FolderContentWrapper CreateTextFileInMy(string title, string content)
        {
            return CreateTextFile(Global.FolderMy.ToString(), title, content);
        }

        /// <summary>
        /// Creates a text (.txt) file in 'Common Documents' section with the title and contents sent in the request
        /// </summary>
        /// <short>Create txt in 'Common'</short>
        /// <category>File Creation</category>
        /// <param name="title">File title</param>
        /// <param name="content">File contents</param>
        /// <returns>Folder contents</returns>
        [Create("@common/text")]
        public FolderContentWrapper CreateTextFileInCommon(string title, string content)
        {
            return CreateTextFile(Global.FolderCommon.ToString(), title, content);
        }

        /// <summary>
        /// Creates a text (.txt) file in the selected folder with the title and contents sent in the request
        /// </summary>
        /// <short>Create txt</short>
        /// <category>File Creation</category>
        /// <param name="folderid">ID of folder</param>
        /// <param name="title">File title</param>
        /// <param name="content">File contents</param>
        /// <returns>Folder contents</returns>
        [Create("{folderid}/text")]
        public FolderContentWrapper CreateTextFile(string folderid, string title, string content)
        {
            if (title == null) throw new ArgumentNullException("title");
            //Try detect content
            var contentType = "text/plain";
            var extension = ".txt";
            if (!string.IsNullOrEmpty(content))
            {
                if (Regex.IsMatch(content, @"<([^\s>]*)(\s[^<]*)>"))
                {
                    contentType = "text/html";
                    extension = ".html";
                }
            }
            CreateFile(folderid, title, content, contentType, extension);
            return GetFolderContents(folderid);
        }

        private static void CreateFile(string folderid, string title, string content, string contentType, string extension)
        {
            using (var memStream = new MemoryStream(Encoding.UTF8.GetBytes(content)))
            {

                DocumentUtils.UploadFile(folderid,
                                         title.EndsWith(extension, StringComparison.OrdinalIgnoreCase) ? title : (title + extension),
                                         memStream.Length, contentType, memStream);
            }
        }

        /// <summary>
        /// Creates an html (.html) file in the selected folder with the title and contents sent in the request
        /// </summary>
        /// <short>Create html</short>
        /// <category>File Creation</category>
        /// <param name="folderid">ID of folder</param>
        /// <param name="title">File title</param>
        /// <param name="content">File contents</param>
        /// <returns>Folder contents</returns>
        [Create("{folderid}/html")]
        public FolderContentWrapper CreateHtmlFile(string folderid, string title, string content)
        {
            if (title == null) throw new ArgumentNullException("title");
            CreateFile(folderid, title, content, "text/html", ".html");
            return GetFolderContents(folderid);
        }

        /// <summary>
        /// Creates an html (.html) file in 'My Documents' section with the title and contents sent in the request
        /// </summary>
        /// <short>Create html in 'My'</short>
        /// <category>File Creation</category>
        /// <param name="title">File title</param>
        /// <param name="content">File contents</param>
        /// <returns>Folder contents</returns>
        [Create("@my/html")]
        public FolderContentWrapper CreateHtmlFileInMy(string title, string content)
        {
            return CreateHtmlFile(Global.FolderMy.ToString(), title, content);
        }


        /// <summary>
        /// Creates an html (.html) file in 'Common Documents' section with the title and contents sent in the request
        /// </summary>
        /// <short>Create html in 'Common'</short>
        /// <category>File Creation</category>
        /// <param name="title">File title</param>
        /// <param name="content">File contents</param>
        /// <returns>Folder contents</returns>        
        [Create("@common/html")]
        public FolderContentWrapper CreateHtmlFileInCommon(string title, string content)
        {
            return CreateHtmlFile(Global.FolderCommon.ToString(), title, content);
        }


        /// <summary>
        /// Creates a new folder with the title sent in the request. The ID of a parent folder can be also specified.
        /// </summary>
        /// <short>
        /// New folder
        /// </short>
        /// <category>Folders</category>
        /// <param name="folderid">ID of parent folder</param>
        /// <param name="title">Title of new folder</param>
        /// <returns>New folder contents</returns>
        [Create("{folderid}")]
        public FolderContentWrapper CreateFolder(string folderid, string title)
        {
            var folder = _fileStorageService.CreateNewFolder(title, folderid);
            return GetFolderContents(folder.ID);
        }

        /// <summary>
        /// Creates a new file in the specified folder with the title sent in the request
        /// </summary>
        /// <short>Create file</short>
        /// <category>File Creation</category>
        /// <param name="folderid">Folder ID</param>
        /// <param name="title">Title of file</param>
        /// <returns>New file info</returns>
        [Create("{folderid}/file")]
        public FileWrapper CreateFile(string folderid, string title)
        {
            var file = _fileStorageService.CreateNewFile(folderid, title);
            return new FileWrapper(file);
        }

        /// <summary>
        /// Renames the selected folder to the new title specified in the request
        /// </summary>
        /// <short>
        /// Rename folder
        /// </short>
        /// <category>Folders</category>
        /// <param name="folderid">Folder ID</param>
        /// <param name="title">new title</param>
        /// <returns>Folder contents</returns>
        [Update("{folderid}")]
        public FolderContentWrapper RenameFolder(string folderid, string title)
        {
            _fileStorageService.FolderRename(folderid, title);
            return GetFolderContents(folderid);
        }

        /// <summary>
        /// Returns a detailed information about the file with the ID specified in the request
        /// </summary>
        /// <short>File information</short>
        /// <category>File information</category>
        /// <returns>File info</returns>
        [Read("file/{fileid}")]
        public FileWrapper GetFileInfo(string fileid)
        {
            var file = _fileStorageService.GetLastFileVersion(fileid).NotFoundIfNull("File not found");

            return new FileWrapper(file);
        }

        /// <summary>
        /// Renames the selected file to the new title specified in the request
        /// </summary>
        /// <short>Rename file</short>
        /// <category>Files</category>
        /// <param name="fileid">File ID</param>
        /// <param name="title">New title</param>
        /// <returns>File info</returns>
        [Update("file/{fileid}")]
        public FileWrapper RenameFile(string fileid, string title)
        {
            _fileStorageService.FileRename(fileid.ToString(CultureInfo.InvariantCulture), title);
            return GetFileInfo(fileid);
        }

        /// <summary>
        /// Deletes the file with the ID specified in the request
        /// </summary>
        /// <short>Delete file</short>
        /// <category>Files</category>
        /// <param name="fileid">File ID</param>
        /// <returns>Operation result</returns>
        [Delete("file/{fileid}")]
        public IEnumerable<FileOperationResult> DeleteFile(string fileid)
        {
            var result = _fileStorageService.DeleteItems(new Web.Files.Services.WCFService.ItemList<string> {fileid.ToString(CultureInfo.InvariantCulture)});
            return result.ToSmartList();
        }

        /// <summary>
        /// Returns a detailed information about all the available file versions with the ID specified in the request
        /// </summary>
        /// <short>File versions</short>
        /// <category>File information</category>
        /// <param name="fileid">ID of file</param>
        /// <returns>List of file infos</returns>
        [Read("file/{fileid}/history")]
        public IEnumerable<FileWrapper> GetFileVersionInfo(string fileid)
        {
            var files = _fileStorageService.GetFileHistory(fileid);
            return files.Select(x => new FileWrapper(x)).ToSmartList();
        }

        /// <summary>
        /// Returns a detailed information about shared file with the ID specified in the request
        /// </summary>
        /// <short>File sharing</short>
        /// <category>File information</category>
        /// <param name="fileid">ID of file</param>
        /// <returns>List of file sharing info</returns>
        [Read("file/{fileid}/share")]
        public IEnumerable<FileShareWrapper> GetFileSecurityInfo(string fileid)
        {
            var fileShares = _fileStorageService.GetSharedInfo(String.Format("file_{0}", fileid));
            return fileShares.Select(x => new FileShareWrapper(x)).ToSmartList();
        }

        /// <summary>
        /// Returns a detailed information about shared folder with the ID specified in the request
        /// </summary>
        /// <short>Folder sharing</short>
        /// <param name="folderid">ID of the folder</param>
        /// <returns>List of folder sharing info</returns>
        [Read("folder/{folderid}/share")]
        public IEnumerable<FileShareWrapper> GetFolderSecurityInfo(string folderid)
        {
            var fileShares = _fileStorageService.GetSharedInfo(String.Format("folder_{0}", folderid));
            return fileShares.Select(x => new FileShareWrapper(x)).ToSmartList();
        }

        /// <summary>
        /// Sets file sharing with the ID of file specified in the request
        /// </summary>
        /// <param name="fileid">ID of file</param>
        /// <param name="share">Collection of sharing rites</param>
        /// <param name="notify">Should notify people</param>
        /// <param name="sharingMessage">Sharing message to send when notifying</param>
        /// <remarks>
        /// Each of the FileShareParams is pair of the ShareTo - id of the user with whom we want to share and Access - access type which we want to grant to the user (Read, ReadWrite, etc) 
        /// </remarks>
        /// <returns>List of file sharing info</returns>
        [Create("file/{fileid}/share")]//TODO: think maybe it's more likely to be PUT
        public IEnumerable<FileShareWrapper> SetFileSecurityInfo(string fileid, IEnumerable<FileShareParams> share, bool notify,string sharingMessage)
        {
            if (share!=null && share.Any())
            {
                _fileStorageService.SetAceObject(
                    new Web.Files.Services.WCFService.ItemList<AceWrapper>(share.Select(x => x.ToAceObject())),
                    fileid, 
                    notify, 
                    sharingMessage);
            }
            return GetFileSecurityInfo(fileid);
        }

        private FolderContentWrapper GetFolderContents(object folderId)
        {
            return new FolderContentWrapper(_fileStorageService.GetFolderItems(folderId.ToString(),
                                                                               _context.StartIndex.ToString(CultureInfo.InvariantCulture),
                                                                               _context.Count.ToString(CultureInfo.InvariantCulture),
                                                                               ((int) FilterType.None).ToString(CultureInfo.InvariantCulture),
                                                                               new OrderBy(SortedByType.AZ, true),
                                                                               null,
                                                                               string.Empty,
                                                                               false));
        }
    }
}