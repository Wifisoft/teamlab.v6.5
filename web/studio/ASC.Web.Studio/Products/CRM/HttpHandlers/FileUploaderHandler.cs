#region Import

using System;
using System.Runtime.Remoting.Messaging;
using System.Web;
using ASC.Common.Web;
using ASC.Files.Core;
using ASC.Web.Controls;
using ASC.Web.Controls.FileUploader.HttpModule;
using ASC.Web.Studio.Core;

#endregion

namespace ASC.Web.CRM.Classes
{
    public class FileUploaderHandler : FileUploadHandler
    {
        public override FileUploadResult ProcessUpload(HttpContext context)
        {
            var fileUploadResult = new FileUploadResult();

            if (!ProgressFileUploader.HasFilesToUpload(context)) return fileUploadResult;

            var file = new ProgressFileUploader.FileToUpload(context);

            if (String.IsNullOrEmpty(file.FileName) || file.ContentLength == 0)
                throw new InvalidOperationException("Invalid file.");

            if (0 < SetupInfo.MaxUploadSize && SetupInfo.MaxUploadSize < file.ContentLength)
                throw FileSizeComment.FileSizeException;

            if (CallContext.GetData("CURRENT_ACCOUNT") == null)
                CallContext.SetData("CURRENT_ACCOUNT", new Guid(context.Request["UserID"]));


            var fileName = file.FileName.LastIndexOf('\\') != -1
                               ? file.FileName.Substring(file.FileName.LastIndexOf('\\') + 1)
                               : file.FileName;


            var document = new File
                               {
                                   Title = fileName,
                                   FolderID = Global.DaoFactory.GetFileDao().GetRoot(),
                                   ContentLength = file.ContentLength
                               };

            document.ContentType = MimeMapping.GetMimeMapping(document.Title);

            document = Global.DaoFactory.GetFileDao().SaveFile(document, file.InputStream);

            fileUploadResult.Data = document.ID;
            fileUploadResult.FileName = document.Title;
            fileUploadResult.FileURL = document.FileUri;


            fileUploadResult.Success = true;


            return fileUploadResult;

        }
    }
}